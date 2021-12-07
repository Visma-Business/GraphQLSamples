using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using IdentityModel.Client;
using Microsoft.IdentityModel.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MvcCode
{
   public class Startup
   {
      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
      }

      public IConfiguration Configuration { get; }

      public void ConfigureServices(IServiceCollection services)
      {
         JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

         services.AddControllersWithViews();

         services.AddHttpClient();

         services.AddOptions();

         services.Configure<AppSettings>(Configuration);

         services.AddSingleton<IDiscoveryCache>(r =>
         {
            var factory = r.GetRequiredService<IHttpClientFactory>();
            return new DiscoveryCache(Configuration.GetValue<string>("Authority"), () => factory.CreateClient());
         });

         services.AddAuthentication(options =>
         {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = "oidc";
         })
         .AddCookie(options =>
         {
            options.Cookie.Name = "mvccode";
         })
         .AddOpenIdConnect("oidc", options =>
         {
            options.Events.OnTokenResponseReceived = (tokenResponse) =>
            {
               var accessToken = tokenResponse.TokenEndpointResponse.AccessToken;
               return Task.CompletedTask;
            };
            options.Events.OnRemoteFailure = (err) =>
            {
               return Task.CompletedTask;
            };
            options.Events.OnMessageReceived = (msg) =>
            {
               return Task.CompletedTask;
            };

            options.Authority = Configuration.GetValue<string>("Authority");
            options.RequireHttpsMetadata = false;            

            options.ClientId = Configuration.GetValue<string>("ClientId");
            options.ClientSecret = Configuration.GetValue<string>("ClientSecret");

            options.ResponseType = "code id_token";
            options.UsePkce = true;

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Scope.Add("business-graphql-api:access-group-based");
            options.Scope.Add("offline_access"); // only if offline access is required

            options.GetClaimsFromUserInfoEndpoint = true;
            options.SaveTokens = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
               NameClaimType = JwtClaimTypes.Name,
               RoleClaimType = JwtClaimTypes.Role,
            };
         });         
      }

      public void Configure(IApplicationBuilder app)
      {
         IdentityModelEventSource.ShowPII = true;

         app.UseDeveloperExceptionPage();
         app.UseHttpsRedirection();
         app.UseStaticFiles();

         app.UseRouting();

         app.UseAuthentication();
         app.UseAuthorization();

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapDefaultControllerRoute()
                   .RequireAuthorization();
         });
      }
   }
}