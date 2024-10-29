using System.Diagnostics;
using Ams.Pm.Wasm.Core.Auth.Identity;
using Microsoft.EntityFrameworkCore;
using Suo.Autorization.SingleService.Core.Auth.Identity;

namespace Suo.Autorization.SingleService.Extentions.Logs
{
    public class PushNotificationsDequeuer : IHostedService, IDisposable
    {
        private Timer _timer;
        private Task _executingTask;
        private readonly IPushNotificationsQueue _messagesQueue;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        private readonly IServiceScopeFactory _scopeFactory;
        public PushNotificationsDequeuer(IPushNotificationsQueue messagesQueue, IServiceScopeFactory scopeFactory)
        {
            _messagesQueue = messagesQueue;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _timer = new Timer(ExecuteTask, null, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(-1));

            return Task.CompletedTask;
        }

        private void ExecuteTask(object state)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _executingTask = ExecuteTaskAsync(_stoppingCts.Token);
        }

        private async Task DequeueMessagesAsync(CancellationToken stoppingToken)
        {
            LogMessageEntry message;
            do
            {
                message = await _messagesQueue.DequeueAsync(stoppingToken);

                if (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var _context = scope.ServiceProvider.GetRequiredService<ApplicationAuthDbContext>();

                            var logEntry = new LogMessageEntry()
                            {
                                InsertDate = DateTime.Now,
                                ErrorMsg = message.ErrorMsg,
                                ErrorLevel = message.ErrorLevel,
                                UserData = message.UserData,
                                ErrorContext = message.ErrorContext,
                                BrowserInfo = message.BrowserInfo,
                                AppVersion = message.AppVersion
                            };

                            var itemNew = _context.LogMessageEntry.Add(logEntry).Entity;
                            _context.SaveChanges();

                            _context.Entry(logEntry).State = EntityState.Detached;
                            _context.SaveChanges();

                            //appRepo = new EFRepository<LogMessageEntry>(_context);


                            //appRepo.Create(new LogMessageEntry()
                            //{
                            //    InsertDate = DateTime.Now,
                            //    ErrorMsg = message.ErrorMsg,
                            //    ErrorLevel = message.ErrorLevel,
                            //    UserData = message.UserData,
                            //    ErrorContext = message.ErrorContext,
                            //    BrowserInfo = message.BrowserInfo,
                            //    AppVersion = message.AppVersion
                            //});
                        }
                    }
                    catch (Exception e)
                    {
                        if (!EventLog.SourceExists("CUO.Authorization.SingleService"))
                            EventLog.CreateEventSource("CUO.Authorization.SingleService", "Critical Log");
                        EventLog.WriteEntry("CUO.Authorization.SingleService", $"Exception - {e.Message}", EventLogEntryType.Error);

                    }

                }
            } while (message != null);
        }

        private async Task ExecuteTaskAsync(CancellationToken stoppingToken)
        {
            await DequeueMessagesAsync(stoppingToken);
            _timer.Change(TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(-1));
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
            _timer?.Dispose();
        }
    }
}
