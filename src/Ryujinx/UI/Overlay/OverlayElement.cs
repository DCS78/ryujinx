using SkiaSharp;
using System;

namespace Ryujinx.UI.Overlay
{
    /// <summary>
    /// Base class for all overlay elements
    /// </summary>
    public abstract class OverlayElement : IDisposable
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public bool IsVisible { get; set; } = true;
        public float Opacity { get; set; } = 1.0f;
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Render this element to the canvas
        /// </summary>
        /// <param name="canvas">The canvas to draw on</param>
        /// <param name="globalOpacity">Global opacity multiplier</param>
        public abstract void Render(SKCanvas canvas, float globalOpacity = 1.0f);

        /// <summary>
        /// Check if a point is within this element's bounds
        /// </summary>
        public virtual bool Contains(float x, float y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
        }

        /// <summary>
        /// Get the bounds of this element
        /// </summary>
        public SKRect GetBounds()
        {
            return new SKRect(X, Y, X + Width, Y + Height);
        }

        /// <summary>
        /// Apply opacity to a color
        /// </summary>
        protected SKColor ApplyOpacity(SKColor color, float opacity)
        {
            return color.WithAlpha((byte)(color.Alpha * opacity));
        }

        /// <summary>
        /// Dispose of resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of resources
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
} 
