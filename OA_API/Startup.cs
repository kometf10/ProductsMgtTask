using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OA.DataAccess;
using OA_API.LocalServices.Localization;
using OA.Domin.Resources;
using OA.Services.Auth;
using OA_API.MiddleWares;
using OA_API.ActionFilters;
using System.IO;
using Microsoft.Extensions.FileProviders;
using OA_API.Extentions;
using OA.Domain.Settings;
using OA.Services.ProductsMgt;

namespace OA_API
{
    public class Startup
    {

        private readonly string AllowAllPolicy = "AllowAllPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region "Controller"
            services.AddControllers(options =>
                    {
                        //use custom validation filter attrubute 
                        options.Filters.Add(typeof(ValidateModelStateAttribute));
                    })
                    .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                        {
                            var assemblyName = new AssemblyName(typeof(CommonResources).GetTypeInfo().Assembly.FullName);
                            return factory.Create(nameof(CommonResources), assemblyName.Name);
                        };                            
                    })
                    .AddNewtonsoftJson(options =>
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    )
                    .ConfigureApiBehaviorOptions(options => {
                        //suppress dafault model state validation filter
                        options.SuppressModelStateInvalidFilter = true;
                    });

            services.AddHttpContextAccessor();

            #endregion

            #region "EF Core"

            services.AddEntityFrameworkSqlServer(); //This Service Adds IMemeoryCache With 1024 Size Limit
            //services.AddEntityFrameworkProxies();
            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), mig => mig.MigrationsAssembly("OA.DataAccess"));
                options.UseInternalServiceProvider(serviceProvider);
                //options.UseLazyLoadingProxies();
            }, ServiceLifetime.Transient);

            //services.AddDbContextPool<AppDbContext>((serviceProvider, options) => {
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), mig => mig.MigrationsAssembly("OA.DataAccess"));
            //    options.UseInternalServiceProvider(serviceProvider);
            //    options.UseLazyLoadingProxies();
            //});

            services.AddIdentity<User, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            #endregion

            #region "JWT Auth"

            var jwtSettings = new JwtSettings();
            Configuration.Bind(key: nameof(JwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key: Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true        
                };
            });

            services.AddPolicyBasedAuthorization();

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("UserManager", policy => policy.RequireClaim("Permission", "UserManager.Read", "UserManager.Create", "UserManager.Update", "UserManager.Delete"));
            //});

            #endregion

            #region "Open API"

            services.AddSwaggerGen(options => {
                var security = new OpenApiSecurityRequirement{
                        {
                            new OpenApiSecurityScheme{
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    };
                options.AddSecurityDefinition(name: "Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authiraization Header With Bearer token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                });
                options.AddSecurityRequirement(security);
            });
            #endregion

            #region "CORS"
            // Allow cross origin
            services.AddCors(o => o.AddPolicy(AllowAllPolicy, builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination");
            }));
            #endregion

            #region "Localization"

            //Configure Localization For MultiCulture Supppot
            //services.AddLocalization(options => options.ResourcesPath ="Resources");
            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCulture = new List<CultureInfo>() {
                    new CultureInfo("en-US"),
                    new CultureInfo("ar-SY")
                };
                options.SupportedCultures = supportedCulture;
                options.SupportedUICultures = supportedCulture;
                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");              
                //options.RequestCultureProviders = new[] { new RouteDataRequestCultureProvider {  RouteDataStringKey = "Culture", UIRouteDataStringKey = "UiCulture"  } };
            });
            #endregion

            #region "SignalR Notification"

            services.AddSignalR();

            #endregion

            #region "Register Services"

            services.AddScoped<IIdentityService, IdentityService>();

            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<ICatigoriesService, CatigoriesService>();

            #endregion

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"StaticFiles")),
                RequestPath = new PathString("/StaticFiles")
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(AllowAllPolicy);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = string.Empty;
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "OA_Template");
                c.EnableFilter();
                c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                c.DefaultModelExpandDepth(1);
            });

            //Enable Localization Middelware
            var localizationOption = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOption.Value);

            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
