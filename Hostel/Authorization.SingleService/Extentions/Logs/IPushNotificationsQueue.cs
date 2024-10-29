using Suo.Autorization.SingleService.Core.Auth.Identity;

namespace Suo.Autorization.SingleService.Extentions.Logs
{
    public interface IPushNotificationsQueue
    {
        void Enqueue(LogMessageEntry message);

        Task<LogMessageEntry> DequeueAsync(CancellationToken cancellationToken);
    }
}
