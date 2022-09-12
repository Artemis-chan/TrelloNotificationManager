using System.ComponentModel;
using System.Drawing;
using Modern.Forms;
using Timer = System.Timers.Timer;
namespace TrelloNotificationManager;

public class NotificationListForm : Form
{
    const int MaxNotificationCount = 3;
    private readonly Queue<NotificationForm> _notifications = new ();
    private readonly Queue<(string, string)> _notificationQueue = new ();
    
    public NotificationListForm()
    {
        for (int i = 0; i < MaxNotificationCount; i++)
        {
            var notif = new NotificationForm(i)
            {
                Hidden = Notif_Deactivated,
            };
            notif.Closed += Notif_Closed;
            notif.Shown += Notif_Shown;
            _notifications.Enqueue(notif);
        }
    }

    public void ShowNotification(string title, string message)
    {
        if(_notifications.TryDequeue(out var notif))
        {
            notif.Show(title, message, this);
            return;
        }
        
        _notificationQueue.Enqueue((title, message));        
    }

    private void Notif_Closed(object? sender, EventArgs e)
    {
        Console.WriteLine("closed" + sender);
    }

    private void Notif_Shown(object? sender, EventArgs e)
    {
        Console.WriteLine("Shown" + sender);
    }

    private void Notif_Deactivated(NotificationForm sender)
    {
        if (_notificationQueue.TryDequeue(out var next))
        {
            sender.Show(next.Item1, next.Item2, this);
            return;
        }
        _notifications.Enqueue(sender);
    }
}