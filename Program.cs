using BootstrapBlazor.Components;
using BootstrapBlazorApp2.Server.Components;
using BootstrapBlazorApp2.Server.Data;
using BootstrapBlazorApp2.Server.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.SignalR;
// Importações adicionais para Entity Framework
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddBootstrapBlazor();

// 增加 Pdf 导出服务
builder.Services.AddBootstrapBlazorTableExportService();

// 增加 Html2Pdf 服务
builder.Services.AddBootstrapBlazorHtml2PdfService();

// 增加 SignalR 服务数据传输大小限制配置
builder.Services.Configure<HubOptions>(option => option.MaximumReceiveMessageSize = null);

// ===== CONFIGURAÇÃO ENTITY FRAMEWORK CORE IN-MEMORY =====
// Adicionar DbContext com provedor In-Memory
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase(databaseName: "BlazorInMemoryDb");

    // Configurações adicionais para desenvolvimento
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Registrar serviços relacionados ao banco de dados
builder.Services.AddScoped<IDatabaseService, DatabaseService>();

// Registrar serviços HTTP com timeout configurável
builder.Services.AddHttpClient("TesouroNacional", client =>
{
    client.Timeout = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("TesouroNacional:TimeoutSeconds", 30));
    client.DefaultRequestHeaders.Add("User-Agent", "BootstrapBlazorApp2/1.0");
});

// Registrar serviço do Tesouro Nacional
builder.Services.AddScoped<ITesouroNacionalService, TesouroNacionalService>();

builder.Services.AddLocalization();

// Add multi-language support configuration information
builder.Services.AddRequestLocalization<IOptionsMonitor<BootstrapBlazorOptions>>((localizerOption, blazorOption) =>
{
    localizerOption.DefaultRequestCulture = new RequestCulture("en-US");
    blazorOption.OnChange(op => Invoke(op));

    Invoke(blazorOption.CurrentValue);

    void Invoke(BootstrapBlazorOptions option)
    {
        var supportedCultures = option.GetSupportedCultures();
        localizerOption.SupportedCultures = supportedCultures;
        localizerOption.SupportedUICultures = supportedCultures;
    }
});

var app = builder.Build();

// ===== INICIALIZAÇÃO DO BANCO DE DADOS =====
// Garantir que o banco de dados seja criado e populado na inicialização
await InitializeDatabaseAsync(app);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

await app.RunAsync();

// ===== MÉTODOS AUXILIARES =====
static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Garantir que o banco seja criado
        await context.Database.EnsureCreatedAsync();

        // Verificar se já existem dados
        var tiposCount = await context.TiposPessoa.CountAsync();

        if (tiposCount == 0)
        {
            logger.LogInformation("Banco de dados vazio. Executando seeding...");
            // O seeding já está configurado no OnModelCreating do DbContext
            await context.SaveChangesAsync();
        }

        logger.LogInformation("Banco de dados inicializado com sucesso. Total de tipos: {TiposCount}",
            await context.TiposPessoa.CountAsync());
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao inicializar o banco de dados");
        throw new InvalidOperationException("Falha na inicialização do banco de dados", ex);
    }
}
