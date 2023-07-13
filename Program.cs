using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Net;

namespace LearnSmartCoding.TaskManager.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                var configuration = builder.Configuration;

                builder.Services.AddApplicationInsightsTelemetry();

                Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .Enrich.FromLogContext()
                        .CreateBootstrapLogger();

                builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                .WriteTo.ApplicationInsights(
                        services.GetRequiredService<TelemetryConfiguration>(),
                        TelemetryConverter.Traces));

                // Add services to the container.
                builder.Services.AddControllersWithViews();
                builder.Services.AddHttpClient();
                builder.Services.AddApplicationInsightsTelemetry();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                // Enable Serilog exception logging
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;

                        Log.Error(exception, "Unhandled exception occurred. {ExceptionDetails}", exception?.ToString());

                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
                    });
                });


                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
       
    }
}