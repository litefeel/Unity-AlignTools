using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
    delegate float CalcSize(Vector3[] corners, out float minV, out float maxV);
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
        public static void DistributionGapHorizontal()
        {
            DistributionGapUI(CalcSizeH, ApplyValueCenterH);
        }
        public static void DistributionGapVertical()
        {
            DistributionGapUI(CalcSizeV, ApplyValueCenterV);
        }
        public static void ExpandWidth()
        {
            ExpandUI(CalcSizeH, ApplyValueSizeH, RectTransform.Axis.Horizontal);
        }
        public static void ExpandHeight()
        {
            ExpandUI(CalcSizeV, ApplyValueSizeV, RectTransform.Axis.Vertical);
        }
        public static void ShrinkWidth()
        {
            SharkUI(CalcSizeH, ApplyValueSizeH, RectTransform.Axis.Horizontal);
        }
        public static void ShrinkHeight()
        {
            SharkUI(CalcSizeV, ApplyValueSizeV, RectTransform.Axis.Vertical);
        }

        #region logic
        private static void AlignUI(CalcValueOne calcValue, ApplyValue applyValue)
        {
            var list = Utils.GetRectTransforms();
            if (list.Count < 1) return;

            float v = 0f;
            Vector3[] corners = new Vector3[4];
            if (list.Count == 1)
            {
                var parent = list[0].parent as RectTransform;
                if (parent == null) return;
                parent.GetWorldCorners(corners);
                calcValue(corners, true, ref v);
            }
            else
            {
                for (var i = 0; i < list.Count; i++)
                {
                    list[i].GetWorldCorners(corners);
                    calcValue(corners, 0 == i, ref v);
                }
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
            var list = Utils.GetRectTransforms();
            if (list.Count < 1) return;

            float minV = 0f, maxV = 0f;
            Vector3[] corners = new Vector3[4];
            if (list.Count == 1)
            {
                var parent = list[0].parent as RectTransform;
                if (parent == null) return;
                parent.GetWorldCorners(corners);
                calcValue(corners, true, ref minV, ref maxV);
            }
            else
            {
                for (var i = 0; i < list.Count; i++)
                {
                    list[i].GetWorldCorners(corners);
                    calcValue(corners, 0 == i, ref minV, ref maxV);
                }
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
            public float size;
        }

        private static void DistributionUI(CalcValueTwo calcValue, ApplyValue applyValue)
        {
            var list = Utils.GetRectTransforms();
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
            vlist.Sort((a, b) =>
            {
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

        private static void DistributionGapUI(CalcSize calcSize, ApplyValue applyValue)
        {
            var list = Utils.GetRectTransforms();
            if (list.Count < 3) return;

            var vlist = new List<Value>(list.Count);

            float minV = 0f, maxV = 0f;
            float sumSize = 0f;
            Vector3[] corners = new Vector3[4];
            for (var i = 0; i < list.Count; i++)
            {
                list[i].GetWorldCorners(corners);
                float _minV, _maxV;
                float size = calcSize(corners, out _minV, out _maxV);
                minV = 0 == i ? _minV : Mathf.Min(_minV, minV);
                maxV = 0 == i ? _maxV : Mathf.Max(_maxV, maxV);
                sumSize += size;
                vlist.Add(new Value
                {
                    rt = list[i],
                    v = (_minV + _maxV) / 2,
                    size = size,
                });
            };
            vlist.Sort((a, b) =>
            {
                if (a.v < b.v) return -1;
                else if (a.v > b.v) return 1;
                return 0;
            });

            float gap = (maxV - minV - sumSize) / (list.Count - 1);
            float curV = minV + vlist[0].size + gap;
            for (var i = 1; i < vlist.Count - 1; i++)
            {
                var rt = vlist[i].rt;
                var pos = applyValue(rt, curV + vlist[i].size / 2);
                curV += vlist[i].size + gap;
                Undo.RecordObject(rt, "Distribution UI By Gap");
                rt.anchoredPosition3D = pos;
            }
        }

        private static void ExpandUI(CalcSize calcSize, ApplyValue applyValue, RectTransform.Axis axis)
        {
            var list = Utils.GetRectTransforms();
            if (list.Count < 2) return;

            float size = 0f;
            Vector3[] corners = new Vector3[4];
            for (var i = 0; i < list.Count; i++)
            {
                list[i].GetWorldCorners(corners);
                float _minV, _maxV;
                size = Mathf.Max(size, calcSize(corners, out _minV, out _maxV));
            }
            foreach (var rt in list)
            {
                var v = applyValue(rt, size);
                Undo.RecordObject(rt, "Expand or Shark UI");
                rt.SetSizeWithCurrentAnchors(axis, v.x);
            }
        }
        private static void SharkUI(CalcSize calcSize, ApplyValue applyValue, RectTransform.Axis axis)
        {
            var list = Utils.GetRectTransforms();
            if (list.Count < 2) return;

            float size = 10000000f;
            Vector3[] corners = new Vector3[4];
            for (var i = 0; i < list.Count; i++)
            {
                list[i].GetWorldCorners(corners);
                float _minV, _maxV;
                size = Mathf.Min(size, calcSize(corners, out _minV, out _maxV));
            }
            foreach (var rt in list)
            {
                var v = applyValue(rt, size);
                Undo.RecordObject(rt, "Expand or Shark UI");
                rt.SetSizeWithCurrentAnchors(axis, v.x);
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

        #region calc size min and max
        private static float CalcSizeH(Vector3[] corners, out float minV, out float maxV)
        {
            minV = Mathf.Min(corners[0].x, corners[2].x);
            maxV = Mathf.Max(corners[0].x, corners[2].x);
            return maxV - minV;
        }
        private static float CalcSizeV(Vector3[] corners, out float minV, out float maxV)
        {
            minV = Mathf.Min(corners[0].y, corners[2].y);
            maxV = Mathf.Max(corners[0].y, corners[2].y);
            return maxV - minV;
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
        private static Vector3 ApplyValueSizeH(RectTransform rt, float v)
        {
            var interV = rt.InverseTransformVector(v, 0, 0);
            v = interV.x * rt.localScale.x;
            return new Vector3(v, v, v);
        }
        private static Vector3 ApplyValueSizeV(RectTransform rt, float v)
        {
            var interV = rt.InverseTransformVector(0, v, 0);
            v = interV.y * rt.localScale.y;
            return new Vector3(v, v, v);
        }
        #endregion

    }
}


