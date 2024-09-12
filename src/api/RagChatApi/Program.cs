using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RagChatApi.Services;
using RagChatApi.Services.Cosmos;
using RagChatApi.Services.OpenAi;
using RagChatApi.Settings;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddOptions<CosmosSettings>().Configure<IConfiguration>((settings, config) =>
        {
            config.GetSection("Cosmos").Bind(settings);
        });

        services.AddOptions<OpenAiSettings>().Configure<IConfiguration>((settings, config) =>
        {
            config.GetSection("OpenAi").Bind(settings);
        });

        services.AddSingleton<CosmosContainerClientFactory>();
        services.AddSingleton<IArticleService, CosmosArticleService>();
        services.AddSingleton<IAssistantService, CosmosAssistantService>();
        services.AddSingleton<IEmbeddingService, OpenAiEmbeddingService>();
        services.AddSingleton<IChatService, OpenAiChatService>();
        services.AddSingleton<IStorageService, FakeStorageService>();
    })
    .Build();

host.Run();
