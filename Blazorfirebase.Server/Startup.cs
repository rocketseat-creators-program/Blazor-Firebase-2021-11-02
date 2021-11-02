using System;
using System.IO;
using Blazorfirebase.Server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Blazorfirebase.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.Combine(Environment.CurrentDirectory, Configuration.GetValue<string>("Firebase:PathJson")));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var tokenAuthority = string.Format("https://securetoken.google.com/{0}", Configuration.GetValue<string>("Firebase:ProjectId"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.Authority = tokenAuthority;
                    opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = tokenAuthority,
                        ValidateAudience = true,
                        ValidAudience = Configuration.GetValue<string>("Firebase:ProjectId"),
                        ValidateLifetime = true
                    };
                });

            services.Configure<FirebaseConfig>(Configuration.GetSection("Firebase"));

            services.AddHttpContextAccessor();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Blazorfirebase.Server", Version = "v1" });
            });

            services.Configure<ApiBehaviorOptions>(options => {
                options.SuppressModelStateInvalidFilter = true;
            });
            
            /*
             Faz alteração do CORS padrão para que aceite 
             requisições do nosso outro app, para simular como 
             se fossem dois servidores diferentes
            */
            services.AddCors(opt => opt.AddDefaultPolicy(builder => {
                // builder.WithOrigins("https://localhost:5001");
                // builder.WithOrigins("http://localhost:5000");
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blazorfirebase.Server v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            /*
             Ativa o CORS na inicialização do projeto
             OBS: é necessario deixar esta linha entre o 
             app.UseRouting() e o app.UseAuthorization()
            */
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
