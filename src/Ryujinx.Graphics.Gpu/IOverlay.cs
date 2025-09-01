using SkiaSharp;
using System;

namespace Ryujinx.Graphics.Gpu
{
    /// <summary>
    /// Interface for overlay functionality
    /// </summary>
    public interface IOverlay : IDisposable
    {
        /// <summary>
        /// Name of the overlay
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Whether the overlay is visible
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Opacity of the overlay (0.0 to 1.0)
        /// </summary>
        float Opacity { get; set; }

        /// <summary>
        /// X position of the overlay
        /// </summary>
        float X { get; set; }

        /// <summary>
        /// Y position of the overlay
        /// </summary>
        float Y { get; set; }

        /// <summary>
        /// Z-index for overlay ordering
        /// </summary>
        int ZIndex { get; set; }

        /// <summary>
        /// Update overlay (for animations)
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update</param>
        void Update(float deltaTime);

        /// <summary>
        /// Render this overlay
        /// </summary>
        /// <param name="canvas">The canvas to render to</param>
        void Render(SKCanvas canvas);
    }
} 
