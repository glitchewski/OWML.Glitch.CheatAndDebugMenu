using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace Glitch.AltDebugMenu
{
    public static class DebugKeys
    {
        // Mod specific keys
        public static Key ShowModGui = Key.Backquote;

        // Debug Keys
        public static Key HideGui = Key.F1;
        public static Key SuitUp = Key.F2;
        public static Key Invincibility = Key.F7;
        public static Key LearnLaunchCodes = Key.F3;
        public static Key ToggleClipping = Key.F11;
        public static Key RefillResources = Key.F4;
        public static Key DarkenStranger = Key.Numpad9;
        public static Key LightenUp = Key.Numpad8;
        public static Key FastForward = Key.Equals;

        // Advanced Warp
        public static Key AdvancedWarpExecute = Key.Backslash;
        public static Key AdvancedWarpPrevious = Key.LeftBracket;
        public static Key AdvancedWarpNext = Key.RightBracket;

        // Memory Warp
        public static Key WarpPoint1 = Key.Numpad1;
        public static Key WarpPoint2 = Key.Numpad2;
        public static Key WarpPoint3 = Key.Numpad3;
        public static Key WarpPointSetModifier = Key.Numpad0;

        // Basic Warp
        public static Key WarpComet = Key.Digit1;
        public static Key WarpHourglassTwins = Key.Digit2;
        public static Key WarpTimberHearth = Key.Digit3;
        public static Key WarpBrittleHollow = Key.Digit4;
        public static Key WarpGiantsDeep = Key.Digit5;
        public static Key WarpDarkBramble = Key.Digit6;
        public static Key WarpQuantumMoon = Key.Digit7;
        public static Key WarpMoon = Key.Digit8;
        public static Key WarpStranger = Key.Digit9;
        public static Key WarpShip = Key.Digit0;
        public static Key WarpModifierA = Key.P;
        public static Key WarpModifierB = Key.Minus;
    }
}
