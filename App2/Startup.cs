using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace App2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SSOContext>(option => option.UseSqlServer(Configuration.GetConnectionString("SSO")));

            services.AddControllers();

            // //获取Redis 连接字符串
            // var redisConnStr = Configuration.GetValue<string>("Redis:EndPoint");
            // var redis = ConnectionMultiplexer.Connect(redisConnStr); //建立Redis 连接

            services.AddDataProtection()
                .PersistKeysToDbContext<SSOContext>()  //把加密数据保存在数据库
                //.PersistKeysToFileSystem(new DirectoryInfo(@"C:\server\share\directory\"))  //把加密信息保存大文件夹
                //.PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys")
                .SetApplicationName("SSO"); //把所有子系统都设置为统一的应用名称

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.Cookie.Name = "TestCookie";
                //options.Cookie.Domain = ".91suke.com";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}