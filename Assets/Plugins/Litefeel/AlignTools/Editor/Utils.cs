using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace litefeel.AlignTools
{
    internal static class Utils
    {
        internal static List<RectTransform> GetRectTransforms()
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


