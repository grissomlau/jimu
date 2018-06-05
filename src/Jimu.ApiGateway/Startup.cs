using System.Text;
using Autofac;
using Jimu.ApiGateway.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;

namespace Jimu.ApiGateway
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
            services.AddCors();
            services.AddHttpContextAccessor();
            var jwtOptions = Configuration.GetSection("JwtOptions").Get<JwtOptions>();
            // The key length needs to be of sufficient length, or otherwise an error will occur.
            var tokenSecretKey = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                // Token signature will be verified using a private key.
                ValidateIssuerSigningKey = true,
                RequireSignedTokens = true,
                IssuerSigningKey = new SymmetricSecurityKey(tokenSecretKey),

                // Token will only be valid if contains "accelist.com" for "iss" claim.
                ValidateIssuer = jwtOptions.ValidateIssuer,
                ValidIssuer = jwtOptions.ValidIssuer,

                // Token will only be valid if contains "accelist.com" for "aud" claim.
                ValidateAudience = jwtOptions.ValidateAudience,
                ValidAudience = jwtOptions.ValidAudience,

                // Token will only be valid if not expired yet, with 5 minutes clock skew.
                ValidateLifetime = jwtOptions.ValidateLifetime,
                RequireExpirationTime = false,
                ClockSkew = jwtOptions.TimeSpanClockSkew,

                ValidateActor = false
            };
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = tokenValidationParameters;
        });
            services.AddMvc(o =>
            {
                o.ModelBinderProviders.Insert(0, new JimuQueryStringModelBinderProvider());
                o.ModelBinderProviders.Insert(1, new JimuModelBinderProvider());
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            services.AddTransient(provider =>new AuthService(tokenSecretKey));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseStaticHttpContext();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<HttpStatusCodeExceptionMiddleware>();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
            //app.UseMvc();
            app.UseStatusCodePages();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}");

                routes.MapRoute(
                "Path",
                "{*path}",
                new { controller = "Services", action = "Path" });
            });

            JimuClient.Start(new ContainerBuilder(), Configuration.GetSection("JimuOptions").Get<JimuOptions>());
        }
    }
}
