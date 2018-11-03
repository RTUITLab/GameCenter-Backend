using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;
using Newtonsoft.Json;
using WolfBack.Services;
using WolfBack.Services.Interfaces;
using WolfBack.Settings;
using WolfBack.SignalR;

namespace WolfBack
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
#if DEBUG     
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LocalDB"),
                b => b.MigrationsAssembly("WolfBack")));
#else
             services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("WolfDB"),
                b => b.MigrationsAssembly("WolfBack")));
#endif

            services.Configure<DefaultAdmin>(Configuration.GetSection(nameof(DefaultAdmin)));

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("JwtOptions")["Key"]));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer(options =>
              {
                  options.RequireHttpsMetadata = false;
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidIssuer = Configuration.GetSection("JwtOptions")["Issuer"],

                      ValidateAudience = true,
                      ValidAudience = Configuration.GetSection("JwtOptions")["Audience"],

                      ValidateLifetime = true,

                      IssuerSigningKey = key,

                      ValidateIssuerSigningKey = true
                  };
              });
            services.AddAuthentication();
            services.AddCors();
            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore); ;
            services.AddSignalR();
            services.AddSingleton<IKeyService, KeyService>();
            services.AddSingleton<IQueueService, QueueService>();
            services.AddSpaStaticFiles(config => config.RootPath = "wwwroot");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(config =>
                config.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials());
            app.UseAuthentication();
            app.UseMvc();
            app.UseSignalR(routes =>
                routes.MapHub<ChatHub>("/hub")
            );
            app.UseSpaStaticFiles();
            app.UseSpa(spa => { });
        }
    }
}
