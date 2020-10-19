using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopCreator.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace ShopCreator
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
            string connection = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<DataContext>(options => options.UseSqlServer(connection));
            
            services.AddControllersWithViews();

            //    services.AddDefaultIdentity<IdentityUser>(options =>
            //options.SignIn.RequireConfirmedEmail = false)
            //    .AddEntityFrameworkStores<DataContext>();
            //services.ConfigureApplicationCookie(options => options.LoginPath = "/LogIn");



            services
          .AddAuthentication(options =>
          {
              options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
              options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
          })
          .AddCookie(
                 options =>
                 {
                     options.LoginPath = "/login";
                     options.LogoutPath = "/login";
                 }
           )
         .AddGoogle(o =>
        {
            IConfigurationSection googleAuthNSection =
                Configuration.GetSection("Authentication:Google");

            //options.ClientId = googleAuthNSection["ClientId"];
            //options.ClientSecret = googleAuthNSection["ClientSecret"];

            o.ClientId = Configuration["ClientId"];
            o.ClientSecret = Configuration["ClientSecret"];
            //o.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
            //o.ClaimActions.Clear();
            //o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            //o.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            //o.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
            //o.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
            //o.ClaimActions.MapJsonKey("urn:google:profile", "link");
            //o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        });
          
          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/LogIn/Error");
                app.UseHsts();
            }
          
            app.UseCors();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(end => { end.MapControllers();
                end.MapControllerRoute(
               name: "default2",
               pattern: "{controller}/{action=Index}/{id?}");
                end.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
               


            });



        }
    }
}
