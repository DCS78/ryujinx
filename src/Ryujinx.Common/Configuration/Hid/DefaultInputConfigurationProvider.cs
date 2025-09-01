using System;
using Ryujinx.Common.Configuration.Hid.Controller;
using Ryujinx.Common.Configuration.Hid.Controller.Motion;
using Ryujinx.Common.Configuration.Hid.Keyboard;
using ConfigGamepadInputId = Ryujinx.Common.Configuration.Hid.Controller.GamepadInputId;
using ConfigStickInputId = Ryujinx.Common.Configuration.Hid.Controller.StickInputId;

namespace Ryujinx.Common.Configuration.Hid
{
    /// <summary>
    /// Provides default input configurations for keyboard and controller devices
    /// </summary>
    public static class DefaultInputConfigurationProvider
    {
        /// <summary>
        /// Creates a default keyboard input configuration
        /// </summary>
        /// <param name="id">Device ID</param>
        /// <param name="name">Device name</param>
        /// <param name="playerIndex">Player index</param>
        /// <param name="controllerType">Controller type (defaults to ProController)</param>
        /// <returns>Default keyboard input configuration</returns>
        public static StandardKeyboardInputConfig CreateDefaultKeyboardConfig(string id, string name, PlayerIndex playerIndex, ControllerType controllerType = ControllerType.ProController)
        {
            return new StandardKeyboardInputConfig
            {
                Version = InputConfig.CurrentVersion,
                Backend = InputBackendType.WindowKeyboard,
                Id = id,
                Name = name,
                ControllerType = ControllerType.ProController,
                PlayerIndex = playerIndex,
                LeftJoycon = new LeftJoyconCommonConfig<Key>
                {
                    DpadUp = Key.Up,
                    DpadDown = Key.Down,
                    DpadLeft = Key.Left,
                    DpadRight = Key.Right,
                    ButtonMinus = Key.Minus,
                    ButtonL = Key.E,
                    ButtonZl = Key.Q,
                    ButtonSl = Key.Unbound,
                    ButtonSr = Key.Unbound,
                },
                LeftJoyconStick = new JoyconConfigKeyboardStick<Key>
                {
                    StickUp = Key.W,
                    StickDown = Key.S,
                    StickLeft = Key.A,
                    StickRight = Key.D,
                    StickButton = Key.F,
                },
                RightJoycon = new RightJoyconCommonConfig<Key>
                {
                    ButtonA = Key.Z,
                    ButtonB = Key.X,
                    ButtonX = Key.C,
                    ButtonY = Key.V,
                    ButtonPlus = Key.Plus,
                    ButtonR = Key.U,
                    ButtonZr = Key.O,
                    ButtonSl = Key.Unbound,
                    ButtonSr = Key.Unbound,
                },
                RightJoyconStick = new JoyconConfigKeyboardStick<Key>
                {
                    StickUp = Key.I,
                    StickDown = Key.K,
                    StickLeft = Key.J,
                    StickRight = Key.L,
                    StickButton = Key.H,
                },
            };
        }

        /// <summary>
        /// Creates a default controller input configuration
        /// </summary>
        /// <param name="id">Device ID</param>
        /// <param name="name">Device name</param>
        /// <param name="playerIndex">Player index</param>
        /// <param name="isNintendoStyle">Whether to use Nintendo-style button mapping</param>
        /// <returns>Default controller input configuration</returns>
        public static StandardControllerInputConfig CreateDefaultControllerConfig(string id, string name, PlayerIndex playerIndex, bool isNintendoStyle = false)
        {
            // Split the ID for controller configs
            string cleanId = id.Split(" ")[0];
            
            return new StandardControllerInputConfig
            {
                Version = InputConfig.CurrentVersion,
                Backend = InputBackendType.GamepadSDL2,
                Id = cleanId,
                Name = name,
                ControllerType = ControllerType.ProController,
                PlayerIndex = playerIndex,
                DeadzoneLeft = 0.1f,
                DeadzoneRight = 0.1f,
                RangeLeft = 1.0f,
                RangeRight = 1.0f,
                TriggerThreshold = 0.5f,
                LeftJoycon = new LeftJoyconCommonConfig<ConfigGamepadInputId>
                {
                    DpadUp = ConfigGamepadInputId.DpadUp,
                    DpadDown = ConfigGamepadInputId.DpadDown,
                    DpadLeft = ConfigGamepadInputId.DpadLeft,
                    DpadRight = ConfigGamepadInputId.DpadRight,
                    ButtonMinus = ConfigGamepadInputId.Minus,
                    ButtonL = ConfigGamepadInputId.LeftShoulder,
                    ButtonZl = ConfigGamepadInputId.LeftTrigger,
                    ButtonSl = ConfigGamepadInputId.Unbound,
                    ButtonSr = ConfigGamepadInputId.Unbound,
                },
                LeftJoyconStick = new JoyconConfigControllerStick<ConfigGamepadInputId, ConfigStickInputId>
                {
                    Joystick = ConfigStickInputId.Left,
                    StickButton = ConfigGamepadInputId.LeftStick,
                    InvertStickX = false,
                    InvertStickY = false,
                },
                RightJoycon = new RightJoyconCommonConfig<ConfigGamepadInputId>
                {
                    ButtonA = isNintendoStyle ? ConfigGamepadInputId.A : ConfigGamepadInputId.B,
                    ButtonB = isNintendoStyle ? ConfigGamepadInputId.B : ConfigGamepadInputId.A,
                    ButtonX = isNintendoStyle ? ConfigGamepadInputId.X : ConfigGamepadInputId.Y,
                    ButtonY = isNintendoStyle ? ConfigGamepadInputId.Y : ConfigGamepadInputId.X,
                    ButtonPlus = ConfigGamepadInputId.Plus,
                    ButtonR = ConfigGamepadInputId.RightShoulder,
                    ButtonZr = ConfigGamepadInputId.RightTrigger,
                    ButtonSl = ConfigGamepadInputId.Unbound,
                    ButtonSr = ConfigGamepadInputId.Unbound,
                },
                RightJoyconStick = new JoyconConfigControllerStick<ConfigGamepadInputId, ConfigStickInputId>
                {
                    Joystick = ConfigStickInputId.Right,
                    StickButton = ConfigGamepadInputId.RightStick,
                    InvertStickX = false,
                    InvertStickY = false,
                },
                Motion = new StandardMotionConfigController
                {
                    EnableMotion = true,
                    MotionBackend = MotionInputBackendType.GamepadDriver,
                    GyroDeadzone = 1,
                    Sensitivity = 100,
                },
                Rumble = new RumbleConfigController
                {
                    EnableRumble = false,
                    WeakRumble = 1f,
                    StrongRumble = 1f,
                },
                Led = new LedConfigController
                {
                    EnableLed = false,
                    TurnOffLed = false,
                    UseRainbow = false,
                    LedColor = 0xFFFFFFFF,
                }
            };
        }

        /// <summary>
        /// Gets the short name of a gamepad by removing SDL prefix and truncating if too long
        /// </summary>
        /// <param name="name">Full gamepad name</param>
        /// <param name="maxLength">Maximum length before truncation (default: 50)</param>
        /// <returns>Short gamepad name</returns>
        public static string GetShortGamepadName(string name, int maxLength = 50)
        {
            const string SdlGamepadNamePrefix = "SDL2 Gamepad ";
            const string Ellipsis = "...";
            
            // First remove SDL prefix if present
            string shortName = name;
            if (name.StartsWith(SdlGamepadNamePrefix))
            {
                shortName = name[SdlGamepadNamePrefix.Length..];
            }
            
            // Then truncate if too long
            if (shortName.Length > maxLength)
            {
                return $"{shortName.AsSpan(0, maxLength - Ellipsis.Length)}{Ellipsis}";
            }
            
            return shortName;
        }

        /// <summary>
        /// Determines if a controller uses Nintendo-style button mapping
        /// </summary>
        /// <param name="name">Controller name</param>
        /// <returns>True if Nintendo-style mapping should be used</returns>
        public static bool IsNintendoStyleController(string name)
        {
            return name.Contains("Nintendo");
        }
    }
} 
