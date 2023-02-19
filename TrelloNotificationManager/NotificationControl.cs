using System.Diagnostics;
using System.Drawing;
using Modern.Forms;
using SkiaSharp;
using ContentAlignment = Modern.Forms.ContentAlignment;

namespace TrelloNotificationManager;

public class NotificationControl : Control
{
    
    private readonly Label _label1;
    private readonly Label _label2;
    private readonly OverlayControl _inputCatcher;

    private LinkData? _link;

    public NotificationControl(Size? size, string head = "", string body = "", LinkData? link = null)
    {
        // TitleBar.Size = new Size(0, 0);
        Style.BackgroundColor = SKColors.Pink;
        Style.Border.Color = new SKColor(0xFF, 0x90, 0xAE, 0xFF);
        Style.Border.Width = 3;

        Size = size ?? Size;

        var width = Size.Width;
        var heightA = Size.Height / 3;
        
        // Controls.Add(new Label()
        // {
        //     Text = "text",
        //     Width = Size.Width / 2,
        //     Height = Size.Height / 2,
        // });
        _label1 = Controls.Add(new Label
        {
            Text = head,
            Location = new Point(5, 5),
            Height = heightA - 5,
            Width = width,
            Multiline = true,
            TextAlign = ContentAlignment.TopLeft,
            Style = { BackgroundColor = SKColors.Transparent }
        });
        
        _label2 = Controls.Add(new Label
        {
            Text = body,
            Location = new Point(5, _label1.Height + 10),
            Height = heightA * 2 - 10,
            Width = width,
            Multiline = true,
            TextAlign = ContentAlignment.TopLeft,
            Style = { BackgroundColor = SKColors.Transparent }
        });

        _inputCatcher = Controls.Add(new OverlayControl
        {
            Width = Size.Width,
            Height = Size.Height,
            Location = Point.Empty,
            Style = { BackgroundColor = SKColors.Transparent }
        });
        
        _inputCatcher.Click += InputCatcherOnClick;
        _inputCatcher.MouseWheel += (_, e) => OnMouseWheel(e);
        
        _link = link;
    }

    public void UpdateText(string head, string body, LinkData? link = null)
    {
        _label1.Text = head;
        _label2.Text = body;
        _link = link;
    }

    private void InputCatcherOnClick(object? sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
                if(_link is null) break;
                if(string.IsNullOrWhiteSpace(_link.Exe))
                {
                    Process.Start(new ProcessStartInfo(_link.Link) { UseShellExecute = true });
                    break;
                }
                Process.Start(new ProcessStartInfo(_link.Exe, _link.Link) { UseShellExecute = true });
                break;
            
            case MouseButtons.Right:
                Program.NotificationList.Show();
                break;
        }
        base.OnClick(e);
    }
    
    private void InputCatcherMouseLeave(object? sender, EventArgs e)
    {
        Style.BackgroundColor = SKColors.Pink;
        Hide();
        Show();
    }

    private void InputCatcherMouseEnter(object? sender, MouseEventArgs e)
    {
        Style.BackgroundColor = SKColors.LightPink;
        Hide();
        Show();
    }
}