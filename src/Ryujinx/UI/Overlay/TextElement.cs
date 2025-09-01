using SkiaSharp;

namespace Ryujinx.UI.Overlay
{
    /// <summary>
    /// Text overlay element
    /// </summary>
    public class TextElement : OverlayElement
    {
        public string Text { get; set; } = string.Empty;
        public SKColor TextColor { get; set; } = SKColors.White;
        public float FontSize { get; set; } = 16;
        public string FontFamily { get; set; } = "Arial";
        public SKFontStyle FontStyle { get; set; } = SKFontStyle.Normal;
        public SKTextAlign TextAlign { get; set; } = SKTextAlign.Left;
        public bool IsAntialias { get; set; } = true;

        // Shadow properties
        public bool HasShadow { get; set; } = false;
        public SKColor ShadowColor { get; set; } = SKColors.Black;
        public float ShadowOffsetX { get; set; } = 1;
        public float ShadowOffsetY { get; set; } = 1;
        public float ShadowBlur { get; set; } = 0;

        public TextElement()
        {
        }

        public TextElement(float x, float y, string text, float fontSize = 16, SKColor? color = null)
        {
            X = x;
            Y = y;
            Text = text;
            FontSize = fontSize;
            TextColor = color ?? SKColors.White;
            
            // Auto-calculate width and height based on text
            UpdateDimensions();
        }

        public override void Render(SKCanvas canvas, float globalOpacity = 1.0f)
        {
            if (!IsVisible || string.IsNullOrEmpty(Text))
                return;

            float effectiveOpacity = Opacity * globalOpacity;

            using var typeface = SKTypeface.FromFamilyName(FontFamily, FontStyle);
            using var paint = new SKPaint
            {
                Color = ApplyOpacity(TextColor, effectiveOpacity),
                TextSize = FontSize,
                Typeface = typeface,
                TextAlign = TextAlign,
                IsAntialias = IsAntialias
            };

            float textX = X;
            float textY = Y + FontSize; // Baseline adjustment

            // Adjust X position based on alignment
            if (TextAlign == SKTextAlign.Center)
            {
                textX += Width / 2;
            }
            else if (TextAlign == SKTextAlign.Right)
            {
                textX += Width;
            }

            // Draw shadow if enabled
            if (HasShadow)
            {
                using var shadowPaint = new SKPaint
                {
                    Color = ApplyOpacity(ShadowColor, effectiveOpacity),
                    TextSize = FontSize,
                    Typeface = typeface,
                    TextAlign = TextAlign,
                    IsAntialias = IsAntialias
                };

                if (ShadowBlur > 0)
                {
                    shadowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, ShadowBlur);
                }

                canvas.DrawText(Text, textX + ShadowOffsetX, textY + ShadowOffsetY, shadowPaint);
            }

            // Draw main text
            canvas.DrawText(Text, textX, textY, paint);
        }

        /// <summary>
        /// Update width and height based on current text and font settings
        /// </summary>
        public void UpdateDimensions()
        {
            if (string.IsNullOrEmpty(Text))
            {
                Width = 0;
                Height = 0;
                return;
            }

            using var typeface = SKTypeface.FromFamilyName(FontFamily, FontStyle);
            using var paint = new SKPaint
            {
                TextSize = FontSize,
                Typeface = typeface
            };

            var bounds = new SKRect();
            paint.MeasureText(Text, ref bounds);
            
            Width = bounds.Width;
            Height = FontSize; // Use font size as height for consistency
        }
    }
} 
