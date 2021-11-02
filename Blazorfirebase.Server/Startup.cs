using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Blazorfirebase.Server", Version = "v1" });
            });
            /*
             Faz alteração do CORS padrão para que aceite 
             requisições do nosso outro app, para simular como 
             se fossem dois servidores diferentes
            */
            services.AddCors(opt => opt.AddDefaultPolicy(builder => {
                builder.WithOrigins("https://localhost:5001");
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
