using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Timers;
using Modern.Forms;
using SkiaSharp;
using ContentAlignment = Modern.Forms.ContentAlignment;
using Timer = System.Timers.Timer;

public class NotificationForm : Form
{
    protected override Size DefaultSize => new (300, 100);
    
    public Action<NotificationForm>? Hidden; 

    private readonly Label _label1;
    private readonly Label _label2;
    private readonly Control _clickCheck;
    private readonly int _delay;
    private readonly int _duration;
    private readonly Point _startPosition;

    private string? _link = "";
    
    public NotificationForm(int yPos, int delay = 5000, int animationDuration = 1000)
    {
        StartPosition = FormStartPosition.Manual;
        _startPosition = new Point(0, yPos * Size.Height);
        Location = _startPosition;
        TitleBar.Visible = false;
        Resizeable = false;
        // TitleBar.Size = new Size(0, 0);
        
        var width = Size.Width - 10;
        var heightA = Size.Height / 3;
        _label1 = Controls.Add(new Label
        {
            Location = new Point(5, 5),
            Height = heightA - 5,
            Width = width,
            Multiline = true,
            TextAlign = ContentAlignment.TopLeft,
        });
        _label2 = Controls.Add(new Label
        {
            Location = new Point(5, _label1.Height + 10),
            Height = heightA * 2 - 10,
            Width = width,
            Multiline = true,
            TextAlign = ContentAlignment.TopLeft,
        });

        _clickCheck = Controls.Add(new Control()
        {
            Width = Size.Width,
            Height = Size.Height,
            Location = Point.Empty,
        });
        _clickCheck.Style.BackgroundColor = SKColors.Transparent;
        _clickCheck.Click += ClickCheckOnClick;
        
        // _label1.Style.BackgroundColor = SKColor.FromHsv(100, 100, 100);
        // _label2.Style.BackgroundColor = SKColor.FromHsv(100, 100, 100);
        
        // var w = typeof(Form).GetField("window", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(this);
        // w?.GetType().GetMethod("SetTopmost", BindingFlags.Instance | BindingFlags.Public)?.Invoke(w, new object[] { true });
        
        SetTopmost(true);
        ShowTaskbarIcon(false);

        _delay = delay;
        _duration = animationDuration;
    }

    private void ClickCheckOnClick(object? sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
                if(string.IsNullOrWhiteSpace(_link)) break;
                Process.Start(new ProcessStartInfo(_link) { UseShellExecute = true });
                break;
            case MouseButtons.Right:
                Program.NotificationList.Show();
                break;
        }

        EndNotification();
    }

    private void EndNotification()
    {
        Hide();
        Hidden?.Invoke(this);
    }

    public void Show(string head, string body, string? link = null)
    {
        _label1.Text = head;
        _label2.Text = body;
        _link = link;
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
