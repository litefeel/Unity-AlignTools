using UnityEditor;
#if UNITY_2019_1_OR_NEWER
using UnityEditor.ShortcutManagement;
using UnityEngine;
#endif

namespace litefeel.AlignTools
{
    public static class AlignToolsMenu
    {
#if UNITY_2019_1_OR_NEWER
        private const string KeyboardMenuPath = "Align Tools/Adjust Position By Keyboard";
        private const string WindowMenuPath = "Window/LiteFeel/Align Tools";
#else
        private const string KeyboardMenuPath = "Window/LiteFeel/Align Tools/Adjust Position By Keyboard %#K";
        private const string WindowMenuPath = "Window/LiteFeel/Align Tools/Align Tools";
#endif
        // Creation of window
        [MenuItem(WindowMenuPath)]
        private static void AlignToolsWindows()
        {
            AlignToolsWindow window = EditorWindow.GetWindow<AlignToolsWindow>(false, "Align Tools", true);
            window.Show();
            window.autoRepaintOnSceneChange = true;
        }

#if UNITY_2019_1_OR_NEWER
        [ClutchShortcut(KeyboardMenuPath, KeyCode.K, ShortcutModifiers.Action | ShortcutModifiers.Shift)]
        private static void ToggleKeyboard(ShortcutArguments arg)
        {
            if (arg.stage == ShortcutStage.Begin)
                Settings.AdjustPositionByKeyboard = !Settings.AdjustPositionByKeyboard;
        }
#else
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
#endif
    }
}


