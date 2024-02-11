using Microsoft.EntityFrameworkCore;
using CHALLENGEIntuit_Back;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // Método para configurar los servicios de la aplicación
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<APIDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("MyConnectionString")));

        // Otros servicios necesarios...
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Cors
        services.AddCors(options =>
        {
            options.AddPolicy(name: "Policy",
                builder =>
                {
                    builder.WithOrigins("http://localhost:4200") // Ajusta el puerto según tu configuración de Angular
                           .AllowAnyOrigin() // Permitir cualquier origen
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });
    }

    // Método para configurar el pipeline de middleware HTTP
    public void Configure(IApplicationBuilder app)
    {
        // Configura el manejo de errores
        app.UseExceptionHandler("/error");

        // Configura el middleware para servir archivos estáticos
        app.UseStaticFiles();

        // CORS
        app.UseCors("Policy"); //usamos el mismo nombre de arriba

        // Configura el middleware de enrutamiento
        app.UseRouting();

        // Configura el middleware de autorización
        app.UseAuthorization();

        // Configura el middleware para manejar endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // Configura Swagger
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
    }
}
