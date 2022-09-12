using System.Drawing;
using System.Reflection;
using System.Timers;
using Modern.Forms;
using Timer = System.Timers.Timer;

public class NotificationForm : Form
{
    protected override Size DefaultSize => new (300, 100);
    
    private Timer _timer;

    public Action<NotificationForm> Hidden; 

    private Label _label1;
    private Label _label2;
    private int _delay;
    private int _duration;
    private Point _startPosition;
    public NotificationForm(int yPos, int delay = 2000, int animationDuration = 1000)
    {
        StartPosition = FormStartPosition.Manual;
        _startPosition = new Point(0, yPos * Size.Height);
        Location = _startPosition;
        TitleBar.Size = new Size(0, 0);
        _label1 = Controls.Add(new Label { Location = new Point(10, 40) });
        _label2 = Controls.Add(new Label { Location = new Point(10, 70) });
        Resizeable = false;
        var w = typeof(Form).GetField("window", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
        w.GetType().GetMethod("SetTopmost", BindingFlags.Instance | BindingFlags.Public).Invoke(w, new object[] { true });
        
        // SetTopMost(true);
        this.
        
        UseSystemDecorations = false;

        _delay = delay;
        _duration = animationDuration;

        // new Thread(AnimateThread).Start(animationDuration);
        // Animate(_delay, _duration);
    }

    public void Show(string head, string body, Form parent)
    {
        _label1.Text = head;
        _label2.Text = body;
        ShowDialog(parent);
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
        _timer = new Timer(INTERVAL);
        _timer.Elapsed += (sender, args) =>
        {
            Translate(new (spd, 0));
        };
        _timer.Enabled = true;
        _timer.Start();
        await Task.Delay(duration);
        _timer.Stop();

        _timer.Dispose();
        Hide();
        Hidden?.Invoke(this);
        // Close();
   }
    
    private void SetTimer(double delay)
    {
        // Create a timer with a two second interval.
        _timer = new Timer(delay);
        // Hook up the Elapsed event for the timer. 
        _timer.Elapsed += TimerOnElapsed;
        _timer.AutoReset = true;
        _timer.Enabled = true; 
        _timer.Start();
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        Translate(new (1, 0));
    }

    private void Translate(Point delta)
    {
        Location = new Point(Location.X + delta.X, Location.Y + delta.Y);
    }

    private int timePassed;
}
