using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RagChatApi.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddSingleton<IArticleService, FakeArticleService>();
        services.AddSingleton<IAssistantService, FakeAssistantService>();
        services.AddSingleton<IConversation, FakeConversation>();
    })
    .Build();

host.Run();
