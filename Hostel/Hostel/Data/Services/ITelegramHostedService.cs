//using Suo.Autorization.TelegramBot;

//namespace Suo.Admin.TelegramBot
//{
//    internal class ITelegramHostedService : BackgroundService
//    {
//        public IServiceScopeFactory _serviceScopeFactory;
//        public ITelegramHostedService(IServiceProvider serviceProvider)
//        {
//            _serviceProvider = serviceProvider;
//        }
//        public IServiceProvider _serviceProvider;

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            using (var scope = _serviceProvider.CreateScope())
//            {
//                var Bot = scope.ServiceProvider.GetRequiredService<TelegramBotNotification>();
//                await Bot.Start();
//            }
//        }

//        public async Task StartAsync(CancellationToken cancellationToken)
//        {

//        }

//        public Task StopAsync(CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}