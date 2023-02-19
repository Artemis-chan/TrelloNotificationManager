using System.ComponentModel;
using System.Drawing;
using Modern.Forms;
using SkiaSharp;
using Timer = System.Timers.Timer;

namespace TrelloNotificationManager;

public class NotificationListForm : Form
{
    const int MaxNotificationCount = 7;
    private readonly Queue<NotificationForm> _notifications = new ();
    private readonly Queue<NotifData> _notificationQueue = new ();
    private readonly ScrollableControl _scroll;

    private int _yPos = 30;
    
    public NotificationListForm()
    {
        Resizeable = false;
        
        Style.BackgroundColor = SKColors.DarkSlateGray;
        
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

        _scroll = Controls.Add(new ScrollableControl
        {
            Size = Size,
            Location = Point.Empty,
            AutoScroll = true
        });
        
        _scroll.MouseWheel += OnScroll;

        // AddPanel("Test", "test body");
    }

    private void OnScroll(object? _, MouseEventArgs e)
    {
        DoScroll(e.Delta.Y);
    }

    private void DoScroll(int delta)
    {
        var p = _scroll.VerticalScrollProperties;
        p.Value = Math.Clamp(p.Value - delta * 20, p.Minimum, p.Maximum);
    }

    public void ShowNotification(string title, string message, LinkData? link)
    {
        AddPanel(title, message, link);
        
        if(_notifications.TryDequeue(out var notif))
        {
            notif.Show(title, message, link);
            return;
        }

        
        _notificationQueue.Enqueue(new NotifData(title, message, link));        
    }

    private void AddPanel(string title, string message, LinkData? link = null)
    {
        var size = new Size(Size.Width - 40, 100);
        
        var p = new NotificationControl(size, title, message, link);
        
        p.Location = new Point(15, _yPos += 10);
        _yPos += p.Size.Height;
        p.MouseWheel += OnScroll;

        _scroll.Controls.Add(p);
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
            sender.Show(next.Title, next.Message, next.Link);
            return;
        }
        _notifications.Enqueue(sender);
    }
}

public struct NotifData
{
    public string Title;
    public string Message;
    public LinkData? Link;
    
    public NotifData(string title, string message, LinkData? link)
    {
        Title = title;
        Message = message;
        Link = link;
    }
}

public class LinkData
{
    public string Link;
    public string? Exe;

    private LinkData()
    { }
    
    public static LinkData? New(string? link, string? exe = null)
    {
        return link is null ? null : new LinkData
        {
            Link = link,
            Exe = exe
        };
    }
}