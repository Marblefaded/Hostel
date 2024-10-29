using Suo.TelegramBotSeparated.Services.TelegramBotService;

namespace Suo.TelegramBotSeparated.Services

{
    internal class ITelegramHostedService : BackgroundService
    {
        public IServiceScopeFactory _serviceScopeFactory;
        public ITelegramHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IServiceProvider _serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scope = _serviceProvider.CreateScope();
            {
                var Bot = scope.ServiceProvider.GetRequiredService<TelegramBotSeparate>();
                await Bot.Start();
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

