using Microsoft.OpenApi.Models;
using MessengerDAL;
using Microsoft.EntityFrameworkCore;
using Messenger.Middleware;
using Messenger.Interfaces;
using Messenger.Contexts;
using Messenger.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Messenger.Options;
using System.Text;
using Messenger.Repositories;

namespace Messenger
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // Use this method to add services to the container.  
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("MessengerAPI-v2", new OpenApiInfo { Title = "Messenger API", Version = "v2" });
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            services.AddDbContext<MessengerContext>(options => options.UseNpgsql(Configuration.GetConnectionString("MessengerDb")));
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserTypeRepository, UserTypeRepository>();
            services.AddTransient<IUserChatRepository, UserChatRepository>();

            services.AddTransient<IChatRepository, ChatRepository>();
            services.AddTransient<IChatService, ChatService>();
            services.AddTransient<IChatLinkRepository, ChatLinkRepository>();

            services.AddTransient<ILinkService, LinkService>();

            services.AddTransient<IMessageRepository, MessageRepository>();
            //services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IMessageFileRepository, MessageFileRepository>();            
            
            services.AddTransient<IConfirmationCodeRepository, ConfirmationCodeRepository>();

            services.AddTransient<IFileRepository, FileRepository>();
            //services.AddTransient<IFileService, FileService>();

            services.AddTransient<ISessionRepository, SessionRepository>();
            services.AddScoped<IServiceContext, ServiceContext>();
            services.AddTransient<ITokenService, TokenService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // укзывает, будет ли валидироваться издатель при валидации токена
                    ValidateIssuer = true,
                    // строка, представляющая издателя
                    ValidIssuer = Configuration["Jwt:Issuer"],

                    // будет ли валидироваться потребитель токена
                    ValidateAudience = true,
                    // установка потребителя токена
                    ValidAudience = Configuration["Jwt:Audience"],
                    // будет ли валидироваться время существования
                    ValidateLifetime = true,

                    // установка ключа безопасности
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                    // валидация ключа безопасности
                    ValidateIssuerSigningKey = true,
                };
            });

            services.Configure<JwtOptions>(Configuration.GetSection("Jwt"));
            services.Configure<EmailOptions>(Configuration.GetSection("Email"));
            services.Configure<CodeOptions>(Configuration.GetSection("Code"));
        }
        // Use this method to configure the HTTP request pipeline.  
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if(env.IsDevelopment())
{
                app.UseSwagger();
                app.UseSwaggerUI(swagger =>
                {
                    swagger.SwaggerEndpoint("/swagger/MessengerAPI-v2/swagger.json", "Messanger API v2");
                });
            }
            app.UseMiddleware<JwtMiddleware>();
            app.UseRouting();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
        }
    }
}
