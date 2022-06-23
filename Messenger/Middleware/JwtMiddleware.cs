using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace Messenger.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceContext serviceContext, MessengerContext messengerContext)
        {
            Console.WriteLine(context.Request.Path.Value);
            if (context.Request.Path.Value != null && context.Request.Path.Value.StartsWith("/api/private"))
            {
                string jwtToken = context.Request.Headers.Authorization;
                if (string.IsNullOrEmpty(jwtToken))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync(ResponseErrors.UNAUTHORIZE);
                    return;
                }

                JwtSecurityToken jwtSecureToken;
                try
                {
                    string token = jwtToken.Split(' ')[1];
                    jwtSecureToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                }
                catch(Exception ex)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync(ResponseErrors.UNAUTHORIZE + " " + ex.Message);
                    return;
                }

                if(!Guid.TryParse(jwtSecureToken.Claims.First(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType).Value, out Guid sessionId))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync(ResponseErrors.UNAUTHORIZE);
                    return;
                }

                Session? session = await messengerContext.Sessions.FindAsync(sessionId);

                if (session == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync(ResponseErrors.SESSION_NOT_FOUND);
                    return;
                }

                if (session.DateEnd < DateTime.UtcNow)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync(ResponseErrors.TOKEN_EXPIRED);
                    return;
                }

                serviceContext.ConfigureSession(session);
            }

            await _next(context);
        }
    }
}
