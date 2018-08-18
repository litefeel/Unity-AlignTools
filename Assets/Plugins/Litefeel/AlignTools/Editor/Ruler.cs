using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace litefeel.AlignTools
{
    struct Line
    {
        public bool isH;
        public float p;

        public Vector2 P1(Vector2 size)
        {
            return isH ? new Vector2(0, p) : new Vector2(p, 0);
        }
        public Vector2 P2(Vector2 size)
        {
            return isH ? new Vector2(size.x, p) : new Vector2(p, size.y);
        }
    }

    public class Ruler
    {

        private MouseCursor cursor = MouseCursor.Arrow;
        private Vector2 size;
        private Camera sceneCamera;
        const float RULER_SIZE = 20;
        private int MY_CONTROLE_HINT = typeof(Ruler).GetHashCode();
        private int MyControlId
        {
            get { return  GUIUtility.GetControlID(MY_CONTROLE_HINT, FocusType.Passive); }
        }

        private bool isDraging = false;

        private Line dragingLine;
        private List<Line> lines = new List<Line>();


        internal void OnSceneGUI(SceneView sceneView)
        {
            if (!Settings.ShowRuler) return;
            if (!sceneView.in2DMode) return;
            sceneCamera = sceneView.camera;

            size = sceneView.position.size;

            var evt = Event.current;

            Handles.BeginGUI();
            DrawTexture(new Vector2(size.x, RULER_SIZE));
            DrawTexture(new Vector2(RULER_SIZE, size.y));
            Handles.EndGUI();

            DrawLines();

            Line line;
            switch (evt.type)
            {
                case EventType.MouseDrag:
                    if(evt.button == 0 && GUIUtility.hotControl == MyControlId)
                    {
                        isDraging = true;
                        dragingLine.p = dragingLine.isH ? evt.mousePosition.y : evt.mousePosition.x;
                        evt.Use();
                    }
                    break;
                case EventType.MouseDown:
                    if (evt.button == 0)
                    {
                        if (IsPointOnRulerArea(evt.mousePosition))
                        {
                            dragingLine.isH = evt.mousePosition.x > evt.mousePosition.y;
                            dragingLine.p = 0;
                            GUIUtility.hotControl = MyControlId;
                            cursor = MouseCursor.Pan;
                            evt.Use();
                        }else if (IsPointOverLines(out line, evt.mousePosition))
                        {
                            GUIUtility.hotControl = MyControlId;
                            cursor = MouseCursor.Pan;
                            dragingLine = line;
                            lines.Remove(line);
                            isDraging = true;
                            evt.Use();
                        }
                    }
                    break;
                case EventType.MouseUp:
                    isDraging = false;
                    cursor = MouseCursor.Arrow;
                    if(!IsPointOnRulerArea(evt.mousePosition))
                        lines.Add(dragingLine);
                    break;
                case EventType.MouseMove:
                    if (IsPointOverLines(out line, evt.mousePosition))
                        cursor = MouseCursor.Pan;
                    else
                        cursor = MouseCursor.Arrow;
                    break;
            }
            
            if (cursor != MouseCursor.Arrow)
                EditorGUIUtility.AddCursorRect(new Rect(Vector2.zero, size), cursor);

            sceneView.Repaint();
        }

        private void DrawLines()
        {
            if(isDraging)
                DrawLine(dragingLine.P1(size), dragingLine.P2(size));
            foreach (var line in lines)
                DrawLine(line.P1(size), line.P2(size));
        }

        private void DrawLine(Vector2 start, Vector2 end)
        {
            var p1 = Gui2World(start, sceneCamera);
            var p2 = Gui2World(end, sceneCamera);
            Handles.DrawLine(p1, p2);
        }

        private bool IsPointOnRulerArea(Vector2 pos)
        {
            return pos.x < RULER_SIZE || pos.y < RULER_SIZE;
        }

        private bool IsPointOverLines(out Line line, Vector2 pos)
        {
            foreach(var l in lines)
            {
                if (IsPointOverLine(l, pos))
                {
                    line = l;
                    return true;
                }
            }
            line = new Line();
            return false;
        }

        private bool IsPointOverLine(Line line, Vector2 pos)
        {
            var p = line.isH ? pos.y : pos.x;
            return Mathf.Abs(p - line.p) < 3;
        }

        private void DrawTexture(Vector2 size)
        {
            var texture = Utils.LoadTexture("ruler");
            GUI.DrawTexture(new Rect(Vector2.zero, size), texture, ScaleMode.StretchToFill);
        }

        private Vector3 Gui2World(Vector2 uiPos, Camera camera)
        {
            uiPos.y = size.y-uiPos.y - 20;
            uiPos.x = Mathf.Clamp(uiPos.x, 0, size.x);
            uiPos.y = Mathf.Clamp(uiPos.y, 0, size.y);
            return camera.ScreenToWorldPoint(new Vector3(uiPos.x, uiPos.y, 500));
        }
    }
}
