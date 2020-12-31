using Users.Business.Interfaces;
using Users.Business.Notifications;

namespace Users.Business.Services
{
    public abstract class BaseService
    {
        private readonly INotifier _notifier;

        protected BaseService(INotifier notifier)
        {
            _notifier = notifier;
        }

        protected void Notify(string message)
        {
            _notifier.Handle(new Notification(message));
        }
    }
}
