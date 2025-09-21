using Ryujinx.Common.Logging;
using SkiaSharp;
using System;

namespace Ryujinx.UI.Overlay
{
    /// <summary>
    /// Image overlay element
    /// </summary>
    public class ImageElement : OverlayElement
    {
        private SKBitmap _bitmap;
        private byte[] _imageData;
        private string _imagePath;

        public SKFilterQuality FilterQuality { get; set; } = SKFilterQuality.Medium;
        public bool MaintainAspectRatio { get; set; } = true;

        public ImageElement()
        {
        }

        public ImageElement(float x, float y, float width, float height, byte[] imageData)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            SetImageData(imageData);
        }

        public ImageElement(float x, float y, float width, float height, string imagePath)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            SetImagePath(imagePath);
        }

        /// <summary>
        /// Set image from byte array
        /// </summary>
        public void SetImageData(byte[] imageData)
        {
            _imageData = imageData;
            _imagePath = null;
            LoadBitmap();
        }

        /// <summary>
        /// Set image from file path
        /// </summary>
        public void SetImagePath(string imagePath)
        {
            _imagePath = imagePath;
            _imageData = null;
            LoadBitmap();
        }

        /// <summary>
        /// Set image from existing SKBitmap
        /// </summary>
        public void SetBitmap(SKBitmap bitmap)
        {
            _bitmap?.Dispose();
            _bitmap = bitmap;
            _imageData = null;
            _imagePath = null;
        }

        private void LoadBitmap()
        {
            try
            {
                _bitmap?.Dispose();
                _bitmap = null;

                if (_imageData != null)
                {
                    _bitmap = SKBitmap.Decode(_imageData);
                }
                else if (!string.IsNullOrEmpty(_imagePath))
                {
                    _bitmap = SKBitmap.Decode(_imagePath);
                }
            }
            catch (Exception ex)
            {
                Logger.Error?.Print(LogClass.Gpu, $"Failed to load image: {ex.Message}");
                _bitmap = null;
            }
        }

        public override void Render(SKCanvas canvas, float globalOpacity = 1.0f)
        {
            if (!IsVisible || _bitmap == null || Width <= 0 || Height <= 0)
                return;

            float effectiveOpacity = Opacity * globalOpacity;

            using var paint = new SKPaint
            {
                FilterQuality = FilterQuality,
                Color = SKColors.White.WithAlpha((byte)(255 * effectiveOpacity))
            };

            var sourceRect = new SKRect(0, 0, _bitmap.Width, _bitmap.Height);
            var destRect = new SKRect(X, Y, X + Width, Y + Height);

            if (MaintainAspectRatio)
            {
                // Calculate aspect ratio preserving destination rectangle
                float sourceAspect = (float)_bitmap.Width / _bitmap.Height;
                float destAspect = Width / Height;

                if (sourceAspect > destAspect)
                {
                    // Source is wider, fit to width
                    float newHeight = Width / sourceAspect;
                    float yOffset = (Height - newHeight) / 2;
                    destRect = new SKRect(X, Y + yOffset, X + Width, Y + yOffset + newHeight);
                }
                else
                {
                    // Source is taller, fit to height
                    float newWidth = Height * sourceAspect;
                    float xOffset = (Width - newWidth) / 2;
                    destRect = new SKRect(X + xOffset, Y, X + xOffset + newWidth, Y + Height);
                }
            }

            canvas.DrawBitmap(_bitmap, sourceRect, destRect, paint);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _bitmap?.Dispose();
                _bitmap = null;
            }
            base.Dispose(disposing);
        }
    }
} 
