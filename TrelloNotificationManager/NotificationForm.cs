using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Timers;
using Modern.Forms;
using SkiaSharp;
using ContentAlignment = Modern.Forms.ContentAlignment;
using Timer = System.Timers.Timer;

namespace TrelloNotificationManager;

public class NotificationForm : Form
{
    protected override Size DefaultSize => new (300, 100);
    
    public Action<NotificationForm>? Hidden; 

    private readonly int _delay;
    private readonly int _duration;
    private readonly Point _startPosition;
    private readonly NotificationControl _panel;
    
    public NotificationForm(int yPos, int delay = 5000, int animationDuration = 1000)
    {
        StartPosition = FormStartPosition.Manual;
        _startPosition = new Point(0, yPos * Size.Height);
        Location = _startPosition;
        TitleBar.Visible = false;
        Resizeable = false;
        // TitleBar.Size = new Size(0, 0);
        
        _panel = Controls.Add(new NotificationControl(Size - new Size(2, 2))
        {
            Location = Point.Empty,
        });
        _panel.Click += (_,_) => EndNotification();
        
        SetTopmost(true);
        ShowTaskbarIcon(false);

        _delay = delay;
        _duration = animationDuration;
    }
    
    private void EndNotification()
    {
        Hide();
        Hidden?.Invoke(this);
    }

    public void Show(string head, string body, LinkData? link = null)
    {
        _panel.UpdateText(head, body, link);
        Show();
        Invalidate();
        Program.PlayNotificationSound();
        Animate(_delay, _duration);
    }

    private const int INTERVAL = 5;
    private async void Animate(int delay, int duration)
    {
        Location = _startPosition;
        await Task.Delay(delay);
        var spd = duration / -Size.Width * INTERVAL;
        var _timer = new Timer(INTERVAL);
        _timer.Elapsed += (_, _) => Location = Location with { X = Location.X + spd };
        _timer.Enabled = true;
        _timer.Start();
        
        await Task.Delay(duration);
        _timer.Stop();

        _timer.Dispose();
        EndNotification();
    }
    
    
}
