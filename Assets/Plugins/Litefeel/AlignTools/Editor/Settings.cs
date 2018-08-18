using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace litefeel.AlignTools
{
    public static class Settings
    {
        private const string AdjustPositionByKeyboardKey = "litefeel.AlignTools.AdjustPositionByKeyboard";
        private const string ShowRulerKey = "litefeel.AlignTools.ShowRuler";


        [InitializeOnLoadMethod]
        private static void Init()
        {
            _AdjustPositionByKeyboard = EditorPrefs.GetBool(AdjustPositionByKeyboardKey, false);
            _ShowRuler = EditorPrefs.GetBool(ShowRulerKey, false);
        }

        private static bool _AdjustPositionByKeyboard;
        public static bool AdjustPositionByKeyboard
        {
            get { return _AdjustPositionByKeyboard; }
            set
            {
                if (value != _AdjustPositionByKeyboard)
                {
                    _AdjustPositionByKeyboard = value;
                    EditorPrefs.SetBool(AdjustPositionByKeyboardKey, value);
                }
            }
        }

        private static bool _ShowRuler;
        public static bool ShowRuler
        {
            get { return _ShowRuler; }
            set
            {
                if (value != _ShowRuler)
                {
                    _ShowRuler = value;
                    EditorPrefs.SetBool(ShowRulerKey, value);
                }
            }
        }

    }
}


