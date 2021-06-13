using UnityEditor;
using UnityEngine;

namespace litefeel.AlignTools
{
    public class AdjustPosition
    {
        internal static void Execute()
        {
            if (!Settings.AdjustPositionByKeyboard) return;

            //Debug.Log("DoKeyboard");
            var evt = Event.current;
            if (null == evt || evt.type != EventType.KeyDown) return;

            int ox = 0, oy = 0;
            switch (evt.keyCode)
            {
                case KeyCode.LeftArrow: ox = -1; break;
                case KeyCode.RightArrow: ox = 1; break;
                case KeyCode.UpArrow: oy = 1; break;
                case KeyCode.DownArrow: oy = -1; break;
            }
            if (ox != 0 || oy != 0)
            {
                evt.Use();

                var offset2 = new Vector2(ox, oy);
                var offset3 = new Vector3(ox, oy, 0);
                // shift change z for 3d
                if(evt.shift)
                    offset3 = new Vector3(0, 0, ox + oy);
                if (evt.control)
                {
                    offset2 *= 10;
                    offset3 *= 10;
                }

                foreach(var trans in Utils.GetTransforms())
                {
                    Undo.RecordObject(trans, "AdjustPosition");
                    if (trans is RectTransform)
                        ((RectTransform)trans).anchoredPosition += offset2;
                    else
                        trans.localPosition += offset3;
                }
            }
        }
    }
}


