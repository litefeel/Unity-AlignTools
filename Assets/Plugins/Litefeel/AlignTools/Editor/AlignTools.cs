using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace litefeel.AlignTools
{
    enum AlignType
    {
        LEFT,
        TOP,
        RIGHT,
        BOTTOM
    }

    delegate void CalcValueOne(Vector3[] corners, bool isFirst, ref float v);
    delegate float CalcValueTwo(Vector3[] corners, bool isFirst, ref float minV, ref float maxV);
    delegate Vector3 ApplyValue(RectTransform rt, float v);

    public class AlignTools
    {
        public static void AlignLeft()
        {
            AlignUI(CalcValueLeft, ApplyValueLeft);
        }
        public static void AlignTop()
        {
            AlignUI(CalcValueTop, ApplyValueTop);
        }
        public static void AlignRight()
        {
            AlignUI(CalcValueRight, ApplyValueRight);
        }
        public static void AlignBottom()
        {
            AlignUI(CalcValueBottom, ApplyValueBottom);
        }
        public static void AlignCenterH()
        {
            AlignCenterUI(CalcValueCenterH, ApplyValueCenterH);
        }
        public static void AlignCenterV()
        {
            AlignCenterUI(CalcValueCenterV, ApplyValueCenterV);
        }
        public static void DistributionHorizontal()
        {
            DistributionUI(CalcValueCenterH, ApplyValueCenterH);
        }
        public static void DistributionVertical()
        {
            DistributionUI(CalcValueCenterV, ApplyValueCenterV);
        }

        #region logic
        private static void AlignUI(CalcValueOne calcValue, ApplyValue applyValue)
        {
            var list = GetRectTransforms();
            if (list.Count < 2) return;

            float v = 0f;
            Vector3[] corners = new Vector3[4];
            for (var i = 0; i < list.Count; i++)
            {
                list[i].GetWorldCorners(corners);
                calcValue(corners, 0 == i, ref v);
            }
            foreach (var rt in list)
            {
                var pos = applyValue(rt, v);
                Undo.RecordObject(rt, "Align UI");
                rt.anchoredPosition3D = pos;
            }
        }

        private static void AlignCenterUI(CalcValueTwo calcValue, ApplyValue applyValue)
        {
            var list = GetRectTransforms();
            if (list.Count < 2) return;

            float minV = 0f, maxV = 0f;
            Vector3[] corners = new Vector3[4];
            for (var i = 0; i < list.Count; i++)
            {
                list[i].GetWorldCorners(corners);
                calcValue(corners, 0 == i, ref minV, ref maxV);
            }
            float v = (minV + maxV) * 0.5f;
            foreach (var rt in list)
            {
                var pos = applyValue(rt, v);
                Undo.RecordObject(rt, "Align Center UI");
                rt.anchoredPosition3D = pos;
            }
        }

        struct Value
        {
            public RectTransform rt;
            public float v;
        }

        private static void DistributionUI(CalcValueTwo calcValue, ApplyValue applyValue)
        {
            var list = GetRectTransforms();
            if (list.Count < 3) return;

            var vlist = new List<Value>(list.Count);

            float minV = 0f, maxV = 0f;
            Vector3[] corners = new Vector3[4];
            for (var i = 0; i < list.Count; i++)
            {
                list[i].GetWorldCorners(corners);
                vlist.Add(new Value
                {
                    rt = list[i],
                    v = calcValue(corners, 0 == i, ref minV, ref maxV)
                });
            };
            vlist.Sort((a, b)=>{
                if (a.v < b.v) return -1;
                else if (a.v > b.v) return 1;
                return 0;
            });
            
            float gap = (maxV - minV) / (list.Count - 1);
            for (var i = 1; i < vlist.Count - 1; i++)
            {
                var rt = vlist[i].rt;
                var pos = applyValue(rt, minV + gap * i);
                Undo.RecordObject(rt, "Distribution UI");
                rt.anchoredPosition3D = pos;
            }
        }
        #endregion

        
        #region calc value left right top bottom
        private static void CalcValueLeft(Vector3[] corners, bool isFirst, ref float v)
        {
            if (isFirst)
                v = corners[0].x;
            else
                v = Mathf.Min(v, corners[0].x);
        }

        private static void CalcValueRight(Vector3[] corners, bool isFirst, ref float v)
        {
            if (isFirst)
                v = corners[2].x;
            else
                v = Mathf.Max(v, corners[2].x);
        }

        private static void CalcValueTop(Vector3[] corners, bool isFirst, ref float v)
        {
            if (isFirst)
                v = corners[1].y;
            else
                v = Mathf.Max(v, corners[1].y);
        }

        private static void CalcValueBottom(Vector3[] corners, bool isFirst, ref float v)
        {
            if (isFirst)
                v = corners[0].y;
            else
                v = Mathf.Min(v, corners[0].y);
        }
        #endregion

        #region calc value min and max
        // calc min and max via left
        private static float CalcValueHLeft(Vector3[] corners, bool isFirst, ref float minV, ref float maxV)
        {
            if (isFirst)
                minV = maxV = corners[0].x;
            else
            {
                minV = Mathf.Min(minV, corners[0].x);
                maxV = Mathf.Max(maxV, corners[0].x);
            }
            return corners[0].x;
        }
        private static float CalcValueHRight(Vector3[] corners, bool isFirst, ref float minV, ref float maxV)
        {
            if (isFirst)
                minV = maxV = corners[2].x;
            else
            {
                minV = Mathf.Min(minV, corners[2].x);
                maxV = Mathf.Max(maxV, corners[2].x);
            }
            return corners[2].x;
        }
        private static float CalcValueCenterH(Vector3[] corners, bool isFirst, ref float minV, ref float maxV)
        {
            var x = (corners[0].x + corners[2].x) * 0.5f;
            if (isFirst)
                minV = maxV = x;
            else
            {
                minV = Mathf.Min(minV, x);
                maxV = Mathf.Max(maxV, x);
            }
            return x;
        }
        private static float CalcValueTop(Vector3[] corners, bool isFirst, ref float minV, ref float maxV)
        {
            if (isFirst)
                minV = maxV = corners[1].y;
            else
            {
                minV = Mathf.Min(minV, corners[1].y);
                maxV = Mathf.Max(maxV, corners[1].y);
            }
            return corners[1].y;
        }
        private static float CalcValueBottom(Vector3[] corners, bool isFirst, ref float minV, ref float maxV)
        {
            if (isFirst)
                minV = maxV = corners[0].y;
            else
            {
                minV = Mathf.Min(minV, corners[0].y);
                maxV = Mathf.Max(maxV, corners[0].y);
            }
            return corners[0].y;
        }
        private static float CalcValueCenterV(Vector3[] corners, bool isFirst, ref float minV, ref float maxV)
        {
            var y = (corners[0].y + corners[2].y) * 0.5f;
            if (isFirst)
                minV = maxV = y;
            else
            {
                minV = Mathf.Min(minV, y);
                maxV = Mathf.Max(maxV, y);
            }
            return y;
        }
        #endregion

        #region applay value
        private static Vector3 ApplyValueLeft(RectTransform rt, float v)
        {
            var interPos = rt.InverseTransformPoint(v, 0, 0);
            var pos = rt.anchoredPosition3D;
            pos.x += interPos.x + rt.pivot.x * rt.rect.width;
            return pos;
        }

        private static Vector3 ApplyValueRight(RectTransform rt, float v)
        {
            var interPos = rt.InverseTransformPoint(v, 0, 0);
            var pos = rt.anchoredPosition3D;
            pos.x += interPos.x - (1f - rt.pivot.x) * rt.rect.width;
            return pos;
        }

        private static Vector3 ApplyValueTop(RectTransform rt, float v)
        {
            var interPos = rt.InverseTransformPoint(0, v, 0);
            var pos = rt.anchoredPosition3D;
            pos.y += interPos.y - (1 - rt.pivot.y) * rt.rect.height;
            return pos;
        }

        private static Vector3 ApplyValueBottom(RectTransform rt, float v)
        {
            var interPos = rt.InverseTransformPoint(0, v, 0);
            var pos = rt.anchoredPosition3D;
            pos.y += interPos.y + rt.pivot.y * rt.rect.height;
            return pos;
        }

        private static Vector3 ApplyValueCenterH(RectTransform rt, float v)
        {
            var interPos = rt.InverseTransformPoint(v, 0, 0);
            var pos = rt.anchoredPosition3D;
            pos.x += interPos.x + (rt.pivot.x - 0.5f) * rt.rect.width;
            return pos;
        }

        private static Vector3 ApplyValueCenterV(RectTransform rt, float v)
        {
            var interPos = rt.InverseTransformPoint(0, v, 0);
            var pos = rt.anchoredPosition3D;
            pos.y += interPos.y + (rt.pivot.y - 0.5f) * rt.rect.height;
            return pos;
        }
        #endregion
        
        private static List<RectTransform> GetRectTransforms()
        {
            var arr = Selection.transforms;
            var list = new List<RectTransform>();
            foreach (var trans in arr)
            {
                var rt = trans as RectTransform;
                if (!rt) continue;
                list.Add(rt);
            }
            return list;
        }
    }
}
