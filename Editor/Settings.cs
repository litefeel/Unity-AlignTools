using UnityEditor;
using UnityEngine;

namespace litefeel.AlignTools
{
    public enum OperatorMode : int
    {
        UGUI,
        World,
    }
    public enum DistributionOrder : int
    {
        Position,
        Hierarchy,
    }
    public static class Settings
    {
        private const string AdjustPositionByKeyboardKey = "litefeel.AlignTools.AdjustPositionByKeyboard";
        private const string ShowRulerKey = "litefeel.AlignTools.ShowRuler";
        private const string RulerLineColorKey = "litefeel.AlignTools.RulerLineColor";
        private const string OperatorModeKey = "litefeel.AlignTools.OperatorModeKey";
        private const string DistributionOrderKey = "litefeel.AlignTools.DistributionOrderKey";
        


        [InitializeOnLoadMethod]
        private static void Init()
        {
            _AdjustPositionByKeyboard = EditorPrefs.GetBool(AdjustPositionByKeyboardKey, false);
            _ShowRuler = EditorPrefs.GetBool(ShowRulerKey, false);

            var ruleLineColorStr = EditorPrefs.GetString(RulerLineColorKey, null);
            var ruleLineColor = Color.white;
            if (!ColorUtility.TryParseHtmlString(ruleLineColorStr, out ruleLineColor))
                ruleLineColor = Color.white;
            _RulerLineColor = ruleLineColor;
            _OperatorMode = (OperatorMode)EditorPrefs.GetInt(OperatorModeKey, (int)OperatorMode.UGUI);
            _DistributionOrder = (DistributionOrder)EditorPrefs.GetInt(DistributionOrderKey, (int)DistributionOrder.Position);
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

        private static Color _RulerLineColor;
        public static Color RulerLineColor
        {
            get { return _RulerLineColor; }
            set
            {
                if (value != _RulerLineColor)
                {
                    _RulerLineColor = value;
                    EditorPrefs.SetString(RulerLineColorKey, "#" + ColorUtility.ToHtmlStringRGBA(value));
                }
            }
        }

        private static OperatorMode _OperatorMode;

        public static OperatorMode OperatorMode
        {
            get { return _OperatorMode; }
            set
            {
                if (value != _OperatorMode)
                {
                    _OperatorMode = value;
                    EditorPrefs.SetInt(OperatorModeKey, (int)value);
                }
            }
        }
        public static int OperatorModeInt
        {
            get { return (int)OperatorMode; }
            set { OperatorMode = (OperatorMode)value; }
        }

        private static DistributionOrder _DistributionOrder;
        public static DistributionOrder DistributionOrder
        {
            get { return _DistributionOrder; }
            set
            {
                if (value != _DistributionOrder)
                {
                    _DistributionOrder = value;
                    EditorPrefs.SetInt(DistributionOrderKey, (int)value);
                }
            }
        }

    }
}


