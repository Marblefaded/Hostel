using System.Collections.Concurrent;
using Suo.Autorization.SingleService.Core.Auth.Identity;

namespace Suo.Autorization.SingleService.Extentions.Logs
{
    public class PushNotificationsQueue : IPushNotificationsQueue
    {
        private readonly ConcurrentQueue<LogMessageEntry> _messages = new ConcurrentQueue<LogMessageEntry>();
        private readonly SemaphoreSlim _messageEnqueuedSignal = new SemaphoreSlim(0);

        public void Enqueue(LogMessageEntry message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _messages.Enqueue(message);

            _messageEnqueuedSignal.Release();
        }

        public async Task<LogMessageEntry> DequeueAsync(CancellationToken cancellationToken)
        {
            await _messageEnqueuedSignal.WaitAsync(cancellationToken);

            _messages.TryDequeue(out LogMessageEntry message);

            return message;
        }
    }
}
