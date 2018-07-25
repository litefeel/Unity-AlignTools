using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace litefeel.AlignTools
{
    public static class Settings
    {
        private const string AdjustPositionByKeyboardKey = "litefeel.AlignTools.AdjustPositionByKeyboard";


        [InitializeOnLoadMethod]
        private static void Init()
        {
            _AdjustPositionByKeyboard = EditorPrefs.GetBool(AdjustPositionByKeyboardKey, false);
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
        
    }
}


