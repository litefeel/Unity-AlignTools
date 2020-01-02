using UnityEditor;


namespace litefeel.AlignTools
{
    public static class AlignToolsMenu
    {
        private const string KeyboardMenuPath = "Window/LiteFeel/Align Tools/Adjust Position By Keyboard %#K";
        private const string WindowMenuPath = "Window/LiteFeel/Align Tools/Align Tools";

        // Creation of window
        [MenuItem(WindowMenuPath)]
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


