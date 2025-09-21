using SkiaSharp;
using System;

namespace Ryujinx.Graphics.Gpu
{
    /// <summary>
    /// Interface for overlay management functionality
    /// </summary>
    public interface IOverlayManager : IDisposable
    {

        // Calculate scaling factor based on resolution
        // Use 1080p (1920x1080) as reference resolution for overlay scaling
        public const float ReferenceWidth = 1920;
        public const float ReferenceHeight = 1080;

        /// <summary>
        /// Add an overlay to the manager
        /// </summary>
        /// <param name="overlay">The overlay to add</param>
        void AddOverlay(IOverlay overlay);

        /// <summary>
        /// Check if there are any visible overlays to render
        /// </summary>
        /// <returns>True if there are overlays to render, false otherwise</returns>
        bool HasVisibleOverlays { get; }

        /// <summary>
        /// Update all overlays (for animations)
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update</param>
        void Update(float deltaTime);

        /// <summary>
        /// Render all visible overlays
        /// </summary>
        /// <param name="canvas">The canvas to render to</param>
        void Render(SKCanvas canvas);
    }
} 
