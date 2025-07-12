


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using ShippingSystem.API.Mapping;
using ShippingSystem.BL.Repositories;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using ShippingSystem.BL.Repositories;

using System;
using System.Text;
using ShippingSystem.Core.SeedData;
using ShippingSystem.API.Services;
using ShippingSystem.Core.Interfaces.Service;

namespace ShippingSystem.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(op =>
            {
                op.AddPolicy("CORSPolicy", builder =>
                {
                    builder.AllowAnyOrigin()  // Your Angular app URL
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                      
                });
            });

            builder.Services.AddDbContext<ShippingContext>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()

     .AddEntityFrameworkStores<ShippingContext>()
     .AddDefaultTokenProviders();

            #region // Registering Repositories and UnitOfWork
            builder.Services.AddScoped<ShippingContext>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(typeof(MappConfig));
            builder.Services.AddScoped<IVendorRepository, VendorRepository>();
            builder.Services.AddScoped<IDeliveryManRepository, DeliveryManRepository>();
            builder.Services.AddScoped<IOrderCancellationRepository, OrderCancellationRepository>();
            // Add to services section
            builder.Services.AddScoped<DeliveryManPerformanceService>();
            builder.Services.AddHttpClient(); // Add HttpClient factory

            // Register GPT service
            builder.Services.AddSingleton<IGPTChatService>(provider =>
                new GPTChatService(
                    builder.Configuration["OpenAIApiKey"],
                    provider.GetRequiredService<ILogger<GPTChatService>>(),
                    provider.GetRequiredService<IHttpClientFactory>().CreateClient()
                )
            );

            // Add logging
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            #endregion

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
            builder.Services.AddControllers();
            builder.Services.AddScoped<IStatusRepository, StatusRepository>();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();


         

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(op => op.SwaggerEndpoint("/openapi/v1.json", "v1"));
            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await IdentityDataInitializer.SeedAsync(userManager, roleManager);
            }

            app.UseHttpsRedirection();
            app.UseCors("CORSPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
           
            app.Run();
        }
    }
}
