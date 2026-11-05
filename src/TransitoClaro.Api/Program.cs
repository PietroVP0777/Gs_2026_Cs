using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TransitoClaro.Api.Data;
using TransitoClaro.Api.Domain;
using TransitoClaro.Api.Infra;
using TransitoClaro.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var conexao = builder.Configuration.GetConnectionString("Padrao") ?? "Data Source=transitoclaro.db";
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(conexao));

// Injeção de dependência: cada interface com sua implementação
builder.Services.AddSingleton<IRelogio, RelogioSistema>();
builder.Services.AddSingleton<IClassificadorStatus, ClassificadorPorRelatos>();
builder.Services.AddScoped<ILinhaService, LinhaService>();
builder.Services.AddScoped<IRelatoService, RelatoService>();
builder.Services.AddScoped<IStatusService, StatusService>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new() { Title = "TransitoClaro API", Version = "v1",
        Description = "CitizenTech: informação em tempo real sobre atrasos e lotação de metrô, trem e BRT, feita pelos próprios passageiros." });
    var xml = Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml");
    if (File.Exists(xml)) o.IncludeXmlComments(xml);
});

var app = builder.Build();

using (var escopo = app.Services.CreateScope())
{
    escopo.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

app.UseMiddleware<MiddlewareDeExcecoes>();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

app.Run();
