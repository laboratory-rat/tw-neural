using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Api.Models;
using AutoMapper;
using Domain.User;
using Infrastructure;
using Manager;
using Manager.HostedServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MRNeural.Tools;
using Repository;
using Repository.Store;
using Swashbuckle.AspNetCore.Swagger;
using Tools.Storage;
using TwitterConnector;

namespace Api
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
            services.AddDbContext<ApiDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySQLConnection"), b => b.MigrationsAssembly("Api")));
            ContextSeed(ApiDbContext.CreateFromConnectionString(Configuration.GetConnectionString("MySQLConnection")));


            services.AddIdentity<EntityUser, IdentityRole>().AddEntityFrameworkStores<ApiDbContext>();

            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,

                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
                        ValidateLifetime = true,

                        IssuerSigningKey = AuthOptions.GetKey(),
                        ValidateIssuerSigningKey = true,
                    };
                });

            services.AddCors();
            services.AddCors((o) => o.AddPolicy("Client", builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()));

            services
                .AddMvc()
                .AddJsonOptions(jsonOptions =>
                {
                    jsonOptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            services.AddAutoMapper();

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("Client"));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "LR twitter neural api", Version = "v1" });
            });

            services.AddTransient<ApiUserStore, ApiUserStore>();
            services.AddTransient(x => new AppDataModel
            {
                FacebookAppId = Configuration["Auth:Facebook:Id"],
                FacebookAppSecret = Configuration["Auth:Facebook:Secret"],
            });

            services.AddTransient(x => new StorageBlobClient(Configuration["Storage:Blob:ConnectionString"]));
            services.AddTransient(x => new TwitterClient(Configuration["Auth:Twitter:Key"], Configuration["Auth:Twitter:Secret"], Configuration["Auth:Twitter:RedirectUrl"]));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>()?.HttpContext?.User);
            services.AddTransient(x => new VReader(Configuration["neural:vocabulary"]));

            // stores
            services.AddTransient<ITwitterCollectionsStore, TwitterCollectionsStore>();
            services.AddTransient<ITwitterSourcesStore, TwitterSourcesStore>();
            services.AddTransient<IUserSocialsStore, UserSocialsStore>();
            services.AddTransient<INeuralStore, NeuralStore>();
            services.AddTransient<ITrainSetStore, TrainSetStore>();

            // managers
            services.AddTransient<TrainSetManager>();
            services.AddTransient<UserManager, UserManager>();
            services.AddTransient<TwitterManager, TwitterManager>();
            services.AddTransient<ICollectionsManager, CollectionsManager>();
            services.AddTransient<NeuralManager>();

            // services
            services.AddSingleton<IHostedService, CollectionHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("Client");

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.  
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LR Twitter neural");
            });

            app.UseMvc(x =>
            {
                x.MapRoute(
                    name: "Default",
                    template: "api/{controller=Home}/{action=Index}/{id?}"
                );
            });


        }

        public void OnShutdown()
        {

        }

        public void StartServices()
        {

        }

        public void ContextSeed(ApiDbContext context)
        {
            try
            {
                var userRole = context.Roles.FirstOrDefault(x => x.Name == "User");
                if (userRole == null)
                {
                    context.Roles.Add(new IdentityRole("User"));
                }

                var adminRole = context.Roles.FirstOrDefault(x => x.Name == "Admin");
                if (adminRole == null)
                {
                    context.Roles.Add(new IdentityRole("Admin"));
                }

                context.SaveChanges();
            }
            catch
            {

            }

        }
    }
}
