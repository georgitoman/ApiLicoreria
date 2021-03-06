using ApiSeriesGeorge.Token;
using Licoreria.Data;
using Licoreria.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLicoreria
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
            String cnn = this.Configuration.GetConnectionString("sqllicoreria");
            services.AddTransient<IRepositoryLicoreria, RepositoryLicoreria>();
            services.AddDbContext<LicoreriaContext>(options => options.UseSqlServer(cnn));

            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc(name: "v1", new OpenApiInfo
                    {
                        Title = "Api Licoreria",
                        Version = "1.0",
                        Description = "Api Licoreria Azure"
                    });
                });

            HelperToken helper = new HelperToken(Configuration);
            services.AddAuthentication(helper.GetAuthOptions())
                .AddJwtBearer(helper.GetJwtBearerOptions());

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    options.SwaggerEndpoint(
                        url: "/swagger/v1/swagger.json", name: "Api v1");
                    options.RoutePrefix = "";
                });

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
