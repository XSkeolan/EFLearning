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
            if (context.Request.Path.Value.StartsWith("/api/private"))
            {
                string jwtToken = context.Request.Headers.Authorization;
                if (string.IsNullOrEmpty(jwtToken))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync(ResponseErrors.UNAUTHORIZE);
                    return;
                }

                string token = jwtToken.Split(' ')[1];

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token);
                var tokenS = jsonToken as JwtSecurityToken;

                Guid sessionId = Guid.Parse(tokenS.Claims.First(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType).Value);

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
