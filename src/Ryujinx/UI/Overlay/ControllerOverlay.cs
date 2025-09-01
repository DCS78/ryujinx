using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using OriginalInputConfig = Ryujinx.Common.Configuration.Hid.InputConfig;
using OriginalPlayerIndex = Ryujinx.Common.Configuration.Hid.PlayerIndex;
using OriginalInputBackendType = Ryujinx.Common.Configuration.Hid.InputBackendType;
using Ryujinx.Ava.Common.Locale;
using Ryujinx.Graphics.Gpu;

namespace Ryujinx.UI.Overlay
{
    /// <summary>
    /// Controller overlay that shows controller bindings matching the original AXAML design
    /// </summary>
    public class ControllerOverlay : Overlay
    {
        private const float OverlayWidth = 400;
        private const float Padding = 24;
        private const float PlayerSpacing = 12;
        private const float PlayerRowHeight = 32;

        private const float TitleTextSize = 25;
        private const float PlayerTextSize = 22;

        private float _lifespan = 0f;

        public ControllerOverlay() : base("ControllerOverlay")
        {
            CreateBaseElements();
            X = IOverlayManager.ReferenceWidth - OverlayWidth - 20; // 20px margin from right
            Y = 50; // 50px margin from top
        }

        private void CreateBaseElements()
        {
            // Main background container
            var background = new RectangleElement(0, 0, OverlayWidth, 200, // Dynamic height will be set later
                new SKColor(0, 0, 0, 224)) // #E0000000
            {
                Name = "Background",
                CornerRadius = 12,
                BorderColor = new SKColor(255, 255, 255, 64), // #40FFFFFF
                BorderWidth = 1
            };
            AddElement(background);

            // Title text (will be updated with localized text)
            var titleText = new TextElement(Padding + 30, Padding, LocaleManager.Instance[LocaleKeys.ControllerOverlayTitle], TitleTextSize, SKColors.White)
            {
                Name = "TitleText",
                FontStyle = SKFontStyle.Bold
            };
            AddElement(titleText);
        }

        /// <summary>
        /// Show controller bindings with localized strings
        /// </summary>
        public void ShowControllerBindings(List<OriginalInputConfig> inputConfigs, int durationSeconds)
        {
            // Update title text
            var titleElement = FindElement<TextElement>("TitleText");
            if (titleElement != null)
            {
                titleElement.Text = LocaleManager.Instance[LocaleKeys.ControllerOverlayTitle];
            }

            // Reset lifespan and opacity
            _lifespan = durationSeconds;

            // Clear existing player bindings
            ClearPlayerBindings();

            // Group controllers by player index (support all players + handheld)
            var playerBindings = new Dictionary<OriginalPlayerIndex, List<OriginalInputConfig>>();

            foreach (var config in inputConfigs.Where(c => c.PlayerIndex <= OriginalPlayerIndex.Handheld))
            {
                if (!playerBindings.ContainsKey(config.PlayerIndex))
                {
                    playerBindings[config.PlayerIndex] = new List<OriginalInputConfig>();
                }
                playerBindings[config.PlayerIndex].Add(config);
            }

            float currentY = Padding + 40; // After title

            // Add player bindings to UI (support 8 players + handheld)
            var playerIndices = new[]
            {
                OriginalPlayerIndex.Player1, OriginalPlayerIndex.Player2, OriginalPlayerIndex.Player3, OriginalPlayerIndex.Player4,
                OriginalPlayerIndex.Player5, OriginalPlayerIndex.Player6, OriginalPlayerIndex.Player7, OriginalPlayerIndex.Player8,
                OriginalPlayerIndex.Handheld
            };

            for (int i = 0; i < playerIndices.Length; i++)
            {
                var playerIndex = playerIndices[i];
                float rowY = currentY + (i * (PlayerRowHeight + PlayerSpacing));

                // Player number with colored background (circular badge)
                var playerColor = GetPlayerColor(i);
                var playerBadge = new RectangleElement(Padding, rowY, 24, 20, playerColor)
                {
                    Name = $"PlayerBadge_{i}",
                    CornerRadius = 12
                };
                AddElement(playerBadge);

                // Player number text
                string playerLabel = playerIndex == OriginalPlayerIndex.Handheld ? "H" : $"P{(int)playerIndex + 1}";
                var playerLabelElement = new TextElement(Padding + 12, rowY + 2, playerLabel, PlayerTextSize, SKColors.White)
                {
                    Name = $"PlayerLabel_{i}",
                    FontStyle = SKFontStyle.Bold,
                    TextAlign = SKTextAlign.Center
                };
                AddElement(playerLabelElement);

                // Controller info
                if (playerBindings.ContainsKey(playerIndex))
                {
                    var controllers = playerBindings[playerIndex];
                    var controllerNames = GetUniqueControllerDisplayNames(controllers);

                    var controllerTextElement = new TextElement(Padding + 56, rowY + 2, string.Join(", ", controllerNames), PlayerTextSize, new SKColor(144, 238, 144)) // LightGreen
                    {
                        Name = $"ControllerText_{i}",
                        FontStyle = SKFontStyle.Bold
                    };
                    AddElement(controllerTextElement);
                }
                else
                {
                    var noControllerTextElement = new TextElement(Padding + 56, rowY + 2, LocaleManager.Instance[LocaleKeys.ControllerOverlayNoController], PlayerTextSize, new SKColor(128, 128, 128)) // Gray
                    {
                        Name = $"NoControllerText_{i}",
                        FontStyle = SKFontStyle.Italic
                    };
                    AddElement(noControllerTextElement);
                }
            }

            // Calculate total height and update background
            float totalHeight = Padding + 40 + (playerIndices.Length * (PlayerRowHeight + PlayerSpacing)) + Padding + 20;
            var background = FindElement<RectangleElement>("Background");
            if (background != null)
            {
                background.Height = totalHeight;
            }

            // Show the overlay (position will be set by Window class with actual dimensions)
            IsVisible = true;
        }

        private static SKColor GetPlayerColor(int playerIndex)
        {
            return playerIndex switch
            {
                0 => new SKColor(255, 92, 92),   // Red for Player 1
                1 => new SKColor(54, 162, 235),  // Blue for Player 2  
                2 => new SKColor(255, 206, 84),  // Yellow for Player 3
                3 => new SKColor(75, 192, 192),  // Green for Player 4
                4 => new SKColor(153, 102, 255), // Purple for Player 5
                5 => new SKColor(255, 159, 64),  // Orange for Player 6
                6 => new SKColor(199, 199, 199), // Light Gray for Player 7
                7 => new SKColor(83, 102, 255),  // Indigo for Player 8
                8 => new SKColor(255, 99, 132), // Pink for Handheld
                _ => new SKColor(128, 128, 128)  // Gray fallback
            };
        }

        private List<string> GetUniqueControllerDisplayNames(List<OriginalInputConfig> controllers)
        {
            var nameGroups = new Dictionary<string, List<int>>();
            var displayNames = new List<string>();

            // First pass: get base names and group them
            for (int i = 0; i < controllers.Count; i++)
            {
                string baseName = GetControllerDisplayName(controllers[i]);

                if (!nameGroups.ContainsKey(baseName))
                {
                    nameGroups[baseName] = new List<int>();
                }
                nameGroups[baseName].Add(i);
                displayNames.Add(baseName);
            }

            // Second pass: add numbering for duplicates
            foreach (var group in nameGroups.Where(g => g.Value.Count > 1))
            {
                for (int i = 0; i < group.Value.Count; i++)
                {
                    int index = group.Value[i];
                    displayNames[index] = $"{group.Key} #{i + 1}";
                }
            }

            return displayNames;
        }

        private string GetControllerDisplayName(OriginalInputConfig config)
        {
            if (string.IsNullOrEmpty(config.Name))
            {
                return config.Backend switch
                {
                    OriginalInputBackendType.WindowKeyboard => LocaleManager.Instance[LocaleKeys.ControllerOverlayKeyboard],
                    OriginalInputBackendType.GamepadSDL2 => LocaleManager.Instance[LocaleKeys.ControllerOverlayController],
                    _ => LocaleManager.Instance[LocaleKeys.ControllerOverlayUnknown]
                };
            }

            // Truncate long controller names from the middle
            string name = config.Name;
            if (name.Length > 25)
            {
                int keepLength = 22; // Total characters to keep (excluding "...")
                int leftLength = (keepLength + 1) / 2; // Round up for left side
                int rightLength = keepLength / 2; // Round down for right side

                name = name.Substring(0, leftLength) + "..." + name.Substring(name.Length - rightLength);
            }

            return name;
        }

        /// <summary>
        /// Clear all player bindings
        /// </summary>
        private void ClearPlayerBindings()
        {
            var elementsToRemove = new List<OverlayElement>();

            foreach (var element in GetElements())
            {
                if (element.Name.StartsWith("PlayerBadge_") ||
                    element.Name.StartsWith("PlayerLabel_") ||
                    element.Name.StartsWith("ControllerText_") ||
                    element.Name.StartsWith("NoControllerText_"))
                {
                    elementsToRemove.Add(element);
                }
            }

            foreach (var element in elementsToRemove)
            {
                RemoveElement(element);
                element.Dispose();
            }
        }

        /// <summary>
        /// Update overlay
        /// </summary>
        public override void Update(float deltaTime)
        {
            _lifespan -= deltaTime;

            if (_lifespan <= 0)
            {
                IsVisible = false;
                return;
            }

            if (_lifespan <= 0.5f)
            {
                // Fade out during the last 0.5 seconds
                Opacity = _lifespan / 0.5f;
            }
            else
            {
                Opacity = 1;
            }
        }
    }
}
