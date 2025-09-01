using SkiaSharp;

namespace Ryujinx.UI.Overlay
{
    /// <summary>
    /// Rectangle overlay element
    /// </summary>
    public class RectangleElement : OverlayElement
    {
        public SKColor BackgroundColor { get; set; } = SKColors.Transparent;
        public SKColor BorderColor { get; set; } = SKColors.Transparent;
        public float BorderWidth { get; set; } = 0;
        public float CornerRadius { get; set; } = 0;

        public RectangleElement()
        {
        }

        public RectangleElement(float x, float y, float width, float height, SKColor backgroundColor)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            BackgroundColor = backgroundColor;
        }

        public override void Render(SKCanvas canvas, float globalOpacity = 1.0f)
        {
            if (!IsVisible || Width <= 0 || Height <= 0)
                return;

            float effectiveOpacity = Opacity * globalOpacity;
            var bounds = new SKRect(X, Y, X + Width, Y + Height);

            // Draw background
            if (BackgroundColor.Alpha > 0)
            {
                using var backgroundPaint = new SKPaint
                {
                    Color = ApplyOpacity(BackgroundColor, effectiveOpacity),
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true
                };

                if (CornerRadius > 0)
                {
                    canvas.DrawRoundRect(bounds, CornerRadius, CornerRadius, backgroundPaint);
                }
                else
                {
                    canvas.DrawRect(bounds, backgroundPaint);
                }
            }

            // Draw border
            if (BorderWidth > 0 && BorderColor.Alpha > 0)
            {
                using var borderPaint = new SKPaint
                {
                    Color = ApplyOpacity(BorderColor, effectiveOpacity),
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = BorderWidth,
                    IsAntialias = true
                };

                if (CornerRadius > 0)
                {
                    canvas.DrawRoundRect(bounds, CornerRadius, CornerRadius, borderPaint);
                }
                else
                {
                    canvas.DrawRect(bounds, borderPaint);
                }
            }
        }
    }
} 
