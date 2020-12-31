using System.Collections.Generic;
using Users.Business.Notifications;

namespace Users.Business.Interfaces
{
    public interface INotifier
    {
        bool HasNotification();

        List<Notification> GetNotifications();

        void Handle(Notification notification);
    }
}
