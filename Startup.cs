using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MS.Poc.Server.Authorization;
using MS.Poc.Server.Core;
using MS.Poc.Server.Repositories;

namespace Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, // indicating whether the Issuer should be validated. True means Yes validation required
                    ValidateAudience = true, // indicating whether the audience will be validated during token validation
                    ValidateLifetime = true, // indicating whether the lifetime will be validated during token validation
                    ValidateIssuerSigningKey = true, // controls if validation of the SecurityKey that signed the securityToken is called.
                    ValidIssuer = Configuration["Jwt:Issuer"],// a valid issuer that will be used to check against the token’s issuer
                    ValidAudience = Configuration["Jwt:Audience"], // a valid audience that will be used to check against the token’s audience
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"])), // public key used for validating incoming JWT tokens
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddSingleton<ITokenRepository, TokenRepository>()

            //services.AddMemoryCache();
            .AddStackExchangeRedisCache(options =>
            {
                Configuration.Bind("RedisCache", options);
            })

            .AddSingleton<IUserRepository, UserRepository>();

            services.AddSingleton<IAuthorizationHandler, RoleHandler>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(RoleRequirement.User, policy => policy.Requirements.Add(new RoleRequirement(RoleRequirement.User)));
                options.AddPolicy(RoleRequirement.Admin, policy => policy.Requirements.Add(new RoleRequirement(RoleRequirement.Admin)));
            });

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .AllowCredentials()
                .AllowAnyMethod()
                .WithHeaders(HeaderNames.ContentType, HeaderNames.AccessControlAllowOrigin, HeaderNames.Authorization)
                //.AllowAnyHeader()
                .WithOrigins("http://localhost:4200"/*, "http://localhost:8080", "http://localhost:3000"*/);
            }));

            services.AddLogging(builder => builder
                .AddConsole() // Register the logger with the ILoggerBuilder
                .AddDebug()
                .AddEventSourceLogger()
                .SetMinimumLevel(LogLevel.Debug) // Set the minimum log level to Information
            );

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // https://stackoverflow.com/questions/36641338/how-get-current-user-in-asp-net-core

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
