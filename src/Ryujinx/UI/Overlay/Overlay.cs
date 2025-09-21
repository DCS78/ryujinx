using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using Ryujinx.Graphics.Gpu;

namespace Ryujinx.UI.Overlay
{
    /// <summary>
    /// Base overlay class containing multiple elements
    /// </summary>
    public abstract class Overlay : IOverlay
    {
        private readonly List<OverlayElement> _elements = new();

        public string Name { get; set; } = string.Empty;
        public bool IsVisible { get; set; } = true;
        public float Opacity { get; set; } = 1.0f;
        public float X { get; set; }
        public float Y { get; set; }
        public int ZIndex { get; set; } = 0;

        public Overlay()
        {
        }

        public Overlay(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Add an element to this overlay
        /// </summary>
        public void AddElement(OverlayElement element)
        {
            _elements.Add(element);
        }

        /// <summary>
        /// Remove an element from this overlay
        /// </summary>
        public void RemoveElement(OverlayElement element)
        {
            _elements.Remove(element);
        }

        /// <summary>
        /// Get all elements
        /// </summary>
        public IReadOnlyList<OverlayElement> GetElements()
        {
            return _elements.AsReadOnly();
        }

        /// <summary>
        /// Find element by name
        /// </summary>
        public T FindElement<T>(string name) where T : OverlayElement
        {
            return _elements.OfType<T>().FirstOrDefault(e => e.Name == name);
        }

        /// <summary>
        /// Clear all elements
        /// </summary>
        public void Clear()
        {
            foreach (var element in _elements)
            {
                element.Dispose();
            }
            _elements.Clear();
        }

        /// <summary>
        /// Update overlay
        /// </summary>
        public abstract void Update(float deltaTime);

        /// <summary>
        /// Render this overlay
        /// </summary>
        public void Render(SKCanvas canvas)
        {
            if (!IsVisible || Opacity <= 0.0f)
                return;

            // Save canvas state
            canvas.Save();

            // Apply overlay position offset
            if (X != 0 || Y != 0)
            {
                canvas.Translate(X, Y);
            }

            // Render all elements
            foreach (var element in _elements)
            {
                if (element.IsVisible)
                {
                    element.Render(canvas, Opacity);
                }
            }

            // Restore canvas state
            canvas.Restore();
        }

        public void Dispose()
        {
            Clear();
        }
    }
} 
