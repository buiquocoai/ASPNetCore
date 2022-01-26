using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace Middleware
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
            services.AddRazorPages();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddTransient<FrontMiddleware>();

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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseMiddleware<FrontMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            //app.UseMiddleware<CheckAcessMiddleware>();
            app.UseCheckAccess();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapRazorPages();      


                // EndPoint(2) khi truy vấn đến /Testpost với phương thức post hoặc put
                endpoints.MapMethods("/Testpost", new string[] { "post", "put" }, async context =>
                {
                    await context.Response.WriteAsync("post/pust");
                });

                //  EndPoint(2) -  Middleware khi truy cập /Home với phương thức GET - nó làm Middleware cuối Pipeline
                endpoints.MapGet("/Home", async context =>
                {

                    int? count = context.Session.GetInt32("count");
                    count = (count != null) ? count + 1 : 1;
                    context.Session.SetInt32("count", count.Value);
                    await context.Response.WriteAsync($"Home page! {count}");

                });
            });

            // EndPoint(3)  app.Run tham số là hàm delegate tham số là HttpContex
            // - nó tạo điểm cuối của pipeline.
            app.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Page not found");
            });

        }
    }
}
