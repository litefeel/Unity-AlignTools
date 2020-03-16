using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace litefeel.AlignTools
{

    public class AlignToolsWorld
    {
        delegate void CalcValueOne(int axis, Vector3 position, bool isFirst, ref float v);
        delegate float CalcValueTwo(int axis, Vector3 position, bool isFirst, ref float minV, ref float maxV);
        delegate Vector3 ApplyValue(int axis, Transform rt, float v);

        public static void AlignToMin(int axis)
        {
            AlignUI(axis, CalcValueMin, ApplyValueOne);
        }
        public static void AlignToMax(int axis)
        {
            AlignUI(axis, CalcValueMax, ApplyValueOne);
        }

        public static void AlignToCenter(int axis)
        {
            AlignCenterUI(axis, CalcValue, ApplyValueOne);
        }

        public static void Distribution(int axis)
        {
            DistributionUI(axis, CalcValue, ApplyValueOne);
        }


        #region logic
        private static void AlignUI(int axis, CalcValueOne calcValue, ApplyValue applyValue)
        {
            var list = Utils.GetWorldTransforms();
            if (list.Count < 2) return;

            float v = 0f;
            for (var i = 0; i < list.Count; i++)
            {
                calcValue(axis, list[i].position, 0 == i, ref v);
            }

            foreach (var trans in list)
            {
                var pos = applyValue(axis, trans, v);
                Undo.RecordObject(trans, "Align UI");
                trans.position = pos;
            }
        }

        private static void AlignCenterUI(int axis, CalcValueTwo calcValue, ApplyValue applyValue)
        {
            var list = Utils.GetWorldTransforms();
            if (list.Count < 2) return;

            float minV = 0f, maxV = 0f;
            for (var i = 0; i < list.Count; i++)
            {
                calcValue(axis, list[i].position, 0 == i, ref minV, ref maxV);
            }

            float v = (minV + maxV) * 0.5f;
            foreach (var trans in list)
            {
                var pos = applyValue(axis, trans, v);
                Undo.RecordObject(trans, "Align Center UI");
                trans.position = pos;
            }
        }

        struct Value
        {
            public Transform trans;
            public float v;
            //public float size;
        }

        private static void DistributionUI(int axis, CalcValueTwo calcValue, ApplyValue applyValue)
        {
            var list = Utils.GetWorldTransforms();
            if (list.Count < 3) return;

            var vlist = new List<Value>(list.Count);

            float minV = 0f, maxV = 0f;
            for (var i = 0; i < list.Count; i++)
            {
                vlist.Add(new Value
                {
                    trans = list[i],
                    v = calcValue(axis, list[i].position, 0 == i, ref minV, ref maxV)
                });
            };

            switch (Settings.DistributionOrder)
            {
                case DistributionOrder.Position:
                    vlist.Sort(SortByPosition);
                    break;
                case DistributionOrder.Hierarchy:
                    vlist.Sort(SortByHierarchy);
                    break;
                case DistributionOrder.HierarchyFlipY:
                    vlist.Sort(SortByHierarchy);
                    vlist.Reverse();
                    break;
            }

            float gap = (maxV - minV) / (list.Count - 1);
            for (var i = 1; i < vlist.Count - 1; i++)
            {
                var trans = vlist[i].trans;
                var pos = applyValue(axis, trans, minV + gap * i);
                Undo.RecordObject(trans, "Distribution UI");
                trans.position = pos;
            }
        }
        #endregion


        #region calc value left right top bottom
        private static void CalcValueMin(int axis, Vector3 pos, bool isFirst, ref float v)
        {
            if (isFirst)
                v = pos[axis];
            else
                v = Mathf.Min(v, pos[axis]);
        }

        private static void CalcValueMax(int axis, Vector3 pos, bool isFirst, ref float v)
        {
            if (isFirst)
                v = pos[axis];
            else
                v = Mathf.Max(v, pos[axis]);
        }

        #endregion



        #region calc value min and max
        // calc min and max via left
        private static float CalcValue(int axis, Vector3 pos, bool isFirst, ref float minV, ref float maxV)
        {
            if (isFirst)
                minV = maxV = pos[axis];
            else
            {
                minV = Mathf.Min(minV, pos[axis]);
                maxV = Mathf.Max(maxV, pos[axis]);
            }
            return pos[axis];
        }
        #endregion

        #region applay value

        private static Vector3 ApplyValueOne(int axis, Transform trans, float v)
        {
            var pos = trans.position;
            pos[axis] = v;
            return pos;
        }
        #endregion


        private static int SortByPosition(Value a, Value b)
        {
            if (Mathf.Approximately(a.v, b.v)) return 0;
            if (a.v < b.v) return -1;
            else if (a.v > b.v) return 1;
            return 0;
        }

        private static Dictionary<Transform, int> s_Sets = new Dictionary<Transform, int>();
        private static int SortByHierarchy(Value a, Value b)
        {
            // 是否兄弟节点
            if (a.trans.parent == b.trans.parent)
                return a.trans.GetSiblingIndex() - b.trans.GetSiblingIndex();

            // 是否非跟节点
            var rootA = a.trans.root;
            var rootB = b.trans.root;
            if (rootA != rootB)
                return rootA.GetSiblingIndex() - rootB.GetSiblingIndex();

            s_Sets.Clear();
            int siblingIndx = -1;
            Transform transA = a.trans;
            do
            {
                s_Sets.Add(transA, siblingIndx);
                siblingIndx = transA.GetSiblingIndex();
                transA = transA.parent;
            } while (transA != null);


            Transform transB = b.trans;
            while (transB != null)
            {
                if (s_Sets.TryGetValue(transB.parent, out siblingIndx))
                {
                    s_Sets.Clear();
                    return siblingIndx - transB.GetSiblingIndex();
                }
                transB = transB.parent;
            }
            s_Sets.Clear();
            return 0;
        }

    }
}


