using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace litefeel.AlignTools
{
    public class AlignToolsWindow : EditorWindow
    {

        private static object editorPath;

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
            DrawButton("align_left", AlignTools.AlignLeft, "Align Left");
            DrawButton("align_center_h", AlignTools.AlignCenterH, "Align Center by Horizontal");
            DrawButton("align_right", AlignTools.AlignRight, "Align Right");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            DrawButton("align_top", AlignTools.AlignTop, "Align Top");
            DrawButton("align_center_v", AlignTools.AlignCenterV, "Align Center by Vertical");
            DrawButton("align_bottom", AlignTools.AlignBottom, "Align Bottom");
            EditorGUILayout.EndHorizontal();

            DrawLine();
            EditorGUILayout.BeginHorizontal();
            DrawButton("distribution_h", AlignTools.DistributionGapHorizontal, "Distribute by Horizontal");
            DrawButton("distribution_v", AlignTools.DistributionGapVertical, "Distribute by Vertical");
            EditorGUILayout.EndHorizontal();

            DrawLine();
            EditorGUILayout.BeginHorizontal();
            DrawButton("expand_h", AlignTools.ExpandWidth, "Expand Size by Horizontal");
            DrawButton("expand_v", AlignTools.ExpandHeight, "Expand Size by Vertical");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            DrawButton("shrink_h", AlignTools.ShrinkWidth, "Shrink Size by Horizontal");
            DrawButton("shrink_v", AlignTools.ShrinkHeight, "Shrink Size by Vertical");
            EditorGUILayout.EndHorizontal();


            DrawLine();
            Settings.AdjustPositionByKeyboard = EditorGUILayout.ToggleLeft("Adjust Position By Keyboard", Settings.AdjustPositionByKeyboard);

            AdjustPosition.Execute();
        }

        private void DrawLine()
        {
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
        }

        private GUIContent btnContent;
        private void DrawButton(string iconName, System.Action action, string tooltip = null)
        {
            if (null == btnContent) btnContent = new GUIContent();
            btnContent.image = LoadIcon(iconName);
            btnContent.tooltip = tooltip;
            if (GUILayout.Button(btnContent, GUILayout.ExpandWidth(false)))
                action();
        }

        private Texture LoadIcon(string iconName)
        {
            var skinName = EditorGUIUtility.isProSkin ? "Dark" : "Light";
            string path = string.Format("{0}/Icons/{1}/{2}.png", editorPath, skinName, iconName);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private void OnEnable()
        {
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }

        private void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
        }

        private void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            AdjustPosition.Execute();
        }
        
        private void OnSceneGUI(SceneView sceneView)
        {
            AdjustPosition.Execute();
        }
        
    }
}


