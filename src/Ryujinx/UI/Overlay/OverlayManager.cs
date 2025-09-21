using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using Ryujinx.Graphics.Gpu;
using System.Threading;

namespace Ryujinx.UI.Overlay
{
    /// <summary>
    /// Manages multiple overlays and handles rendering
    /// </summary>
    public class OverlayManager : IOverlayManager
    {
        private readonly List<IOverlay> _overlays = new();
        private readonly Lock _lock = new();

        /// <summary>
        /// Check if there are any visible overlays to render
        /// </summary>
        public bool HasVisibleOverlays
        {
            get
            {
                lock (_lock)
                {
                    return _overlays.Any(o => o.IsVisible && o.Opacity > 0.0f);
                }
            }
        }

        /// <summary>
        /// Add an overlay to the manager
        /// </summary>
        public void AddOverlay(IOverlay overlay)
        {
            lock (_lock)
            {
                _overlays.Add(overlay);
                SortOverlays();
            }
        }

        /// <summary>
        /// Remove an overlay from the manager
        /// </summary>
        public void RemoveOverlay(Overlay overlay)
        {
            lock (_lock)
            {
                _overlays.Remove(overlay);
            }
        }

        /// <summary>
        /// Remove overlay by name
        /// </summary>
        public void RemoveOverlay(string name)
        {
            lock (_lock)
            {
                var overlay = _overlays.FirstOrDefault(o => o.Name == name);
                if (overlay != null)
                {
                    _overlays.Remove(overlay);
                    overlay.Dispose();
                }
            }
        }

        /// <summary>
        /// Find overlay by name
        /// </summary>
        public IOverlay FindOverlay(string name)
        {
            lock (_lock)
            {
                return _overlays.FirstOrDefault(o => o.Name == name);
            }
        }

        /// <summary>
        /// Get all overlays
        /// </summary>
        public IReadOnlyList<IOverlay> GetOverlays()
        {
            lock (_lock)
            {
                return _overlays.AsReadOnly();
            }
        }

        /// <summary>
        /// Clear all overlays
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                foreach (var overlay in _overlays)
                {
                    overlay.Dispose();
                }
                _overlays.Clear();
            }
        }

        /// <summary>
        /// Update all overlays (for animations)
        /// </summary>
        public void Update(float deltaTime)
        {
            lock (_lock)
            {
                foreach (var overlay in _overlays.Where(o => o.IsVisible))
                {
                    overlay.Update(deltaTime);
                }
            }
        }

        /// <summary>
        /// Render all visible overlays
        /// </summary>
        public void Render(SKCanvas canvas)
        {
            lock (_lock)
            {
                foreach (var overlay in _overlays.Where(o => o.IsVisible && o.Opacity > 0.0f))
                {
                    overlay.Render(canvas);
                }
            }
        }

        /// <summary>
        /// Sort overlays by Z-index
        /// </summary>
        private void SortOverlays()
        {
            _overlays.Sort((a, b) => a.ZIndex.CompareTo(b.ZIndex));
        }

        /// <summary>
        /// Show overlay
        /// </summary>
        public void ShowOverlay(string name)
        {
            var overlay = FindOverlay(name);
            if (overlay != null)
            {
                overlay.IsVisible = true;
                overlay.Opacity = 1.0f;
            }
        }

        /// <summary>
        /// Hide overlay
        /// </summary>
        public void HideOverlay(string name)
        {
            var overlay = FindOverlay(name);
            if (overlay != null)
            {
                overlay.IsVisible = false;
                overlay.Opacity = 0.0f;
            }
        }

        public void Dispose()
        {
            Clear();
        }
    }
} 
