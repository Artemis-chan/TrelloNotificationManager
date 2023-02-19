using Modern.Forms;
using SkiaSharp;

namespace TrelloNotificationManager;

public class OverlayControl : Control
{
    private bool _hovering;
    private bool _mouseDown;
    
    public SKColor hoverColor;
    public SKColor downColor;
    public SKColor normalColor;

    public OverlayControl()
    {
        hoverColor = SKColors.LightPink.WithAlpha(0x80);
        downColor = SKColors.DeepPink.WithAlpha(0x30);
        normalColor = SKColors.Transparent;
        Style.BackgroundColor = normalColor;
    }
    
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        _hovering = true;
        Update();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hovering = false;
        Update();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _mouseDown = true;
        Update();
    }
    
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _mouseDown = false;
        Update();
    }

    protected void Update()
    {
        var col = _mouseDown ? downColor : _hovering ? hoverColor : normalColor;
        Style.BackgroundColor = col;
        Invalidate();
    }
}