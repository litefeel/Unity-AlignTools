using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace litefeel.AlignTools
{
    public class AlignToolsWindow : EditorWindow
    {

        private static object editorPath;

        // Creation of window
        [MenuItem("Window/Align Tools")]
        public static void Init()
        {
            AlignToolsWindow window = GetWindow<AlignToolsWindow>(false, "Align Tools", true);
            window.Show();
        }

        // Update the editor window when user changes something (mainly useful when selecting objects)
        void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (editorPath == null)
                editorPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));
            
            EditorGUILayout.BeginHorizontal();
            DrawButton("align_left", AlignTools.AlignLeft);
            DrawButton("align_center_h", AlignTools.AlignCenterH);
            DrawButton("align_right", AlignTools.AlignRight);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            DrawButton("align_top", AlignTools.AlignTop);
            DrawButton("align_center_v", AlignTools.AlignCenterV);
            DrawButton("align_bottom", AlignTools.AlignBottom);
            EditorGUILayout.EndHorizontal();

            DrawLine();
            EditorGUILayout.BeginHorizontal();
            DrawButton("distribution_h", AlignTools.DistributionHorizontal);
            DrawButton("distribution_v", AlignTools.DistributionVertical);
            EditorGUILayout.EndHorizontal();

            DrawLine();
            EditorGUILayout.BeginHorizontal();
            DrawButton("distribution_h", AlignTools.ExpandWidth);
            DrawButton("distribution_v", AlignTools.ExpandHeight);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            DrawButton("distribution_h", AlignTools.ShrinkWidth);
            DrawButton("distribution_v", AlignTools.ShrinkHeight);
            EditorGUILayout.EndHorizontal();


        private void DrawLine()
        {
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
        }

        private void DrawButton(string iconName, System.Action action)
        {
            Texture icon = LoadIcon(iconName);
            if (GUILayout.Button(icon, GUILayout.ExpandWidth(false)))
                action();
        }

        private Texture LoadIcon(string iconName)
        {
            string path = string.Format("{0}/Icons/{1}.png", editorPath, iconName);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }
}


