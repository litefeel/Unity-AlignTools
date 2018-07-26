using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace litefeel.AlignTools
{
    public static class AlignToolsMenu
    {
        private const string KeyboardMenuPath = "Window/Align Tools/Adjust Position By Keyboard %#K";

        // Creation of window
        [MenuItem("Window/Align Tools/Align Tools")]
        private static void AlignToolsWindows()
        {
            AlignToolsWindow window = EditorWindow.GetWindow<AlignToolsWindow>(false, "Align Tools", true);
            window.Show();
            window.autoRepaintOnSceneChange = true;
        }

        [MenuItem(KeyboardMenuPath, true)]
        private static bool VaildToggleKeyboard()
        {
            Menu.SetChecked(KeyboardMenuPath, Settings.AdjustPositionByKeyboard);
            return true;
        }
        [MenuItem(KeyboardMenuPath)]
        private static void ToggleKeyboard()
        {
            Settings.AdjustPositionByKeyboard = !Menu.GetChecked(KeyboardMenuPath);
        }
    }
}


