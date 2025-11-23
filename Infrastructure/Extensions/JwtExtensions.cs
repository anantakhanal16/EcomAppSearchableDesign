using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Application.Dtos;
using Application.Helpers;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Jwt
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("JwtSettings");
            var jwtSettings = jwtSection.Get<JwtSettings>();

            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new ArgumentNullException("JwtSettings configuration is missing.");
            }

            var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
                        var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (string.IsNullOrEmpty(userId))
                        {
                            context.Fail("Invalid token: missing user ID");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userId);
                        if (user == null)
                        {
                            context.Fail("User no longer exists");
                            return;
                        }

                        var tokenSecurityStamp = context.Principal?.FindFirst("securityStamp")?.Value;
                        var currentSecurityStamp = await userManager.GetSecurityStampAsync(user);
                        if (tokenSecurityStamp != currentSecurityStamp) context.Fail("Token is no longer valid due to logout or password reset");
                    },

                    OnChallenge = context =>
                    {
                        ///send mail to admin remaining  mail setup need to be done i have not done due to time constrains

                        //send mail to admin psudocode
                        context.HandleResponse();
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = HttpResponses<string>.FailResponse("Unauthorized access", HttpStatusCode.Unauthorized);
                        var json = JsonSerializer.Serialize(response);
                        return context.Response.WriteAsync(json);
                    },

                    OnForbidden = context =>
                    {
                        ///send mail to admin remaining 
                        //send mail to admin psudocode

                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = HttpResponses<string>.FailResponse("Access denied", HttpStatusCode.Forbidden);
                        var json = JsonSerializer.Serialize(response);
                        return context.Response.WriteAsync(json);
                    }
                };
            });

            services.AddAuthorization();
            return services;
        }
    }
}