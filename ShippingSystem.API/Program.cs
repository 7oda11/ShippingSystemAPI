
using Microsoft.EntityFrameworkCore;
using ShippingSystem.API.Mapping;
using ShippingSystem.BL.Repositories;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using System;

namespace ShippingSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(op =>
            {
                op.AddPolicy("CORSPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:4200")  // Your Angular app URL
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
                });
            });
            builder.Services.AddDbContext<ShippingContext>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });
            #region // Registering Repositories and UnitOfWork
            builder.Services.AddScoped<ShippingContext>();
            builder.Services.AddScoped<ShippingSystem.Core.Interfaces.IUnitOfWork, ShippingSystem.BL.Repositories.UnitOfWork>();
            builder.Services.AddAutoMapper(typeof(MappConfig));
            #endregion

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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
