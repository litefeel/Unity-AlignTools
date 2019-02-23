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
    }

    public class Ruler
    {

        private MouseCursor cursor = MouseCursor.Arrow;
        private Vector2 size;
        const float RULER_SIZE = 20;
        private int _myControlId = 0;
        private int MyControlId
        {
            get {
                if (0 == _myControlId)
                    _myControlId = GUIUtility.GetControlID(typeof(Ruler).GetHashCode(), FocusType.Passive);
                return _myControlId;
            }
        }

        private bool isDraging = false;

        private Line dragingLine;
        private List<Line> lines = new List<Line>();


        internal void OnSceneGUI(SceneView sceneView)
        {
            if (!Settings.ShowRuler) return;
            if (!sceneView.in2DMode) return;

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
                    if (evt.button == 0 && GUIUtility.hotControl == MyControlId)
                    {
                        isDraging = true;
                        var p = Gui2World(evt.mousePosition);
                        dragingLine.p = dragingLine.isH ? p.y : p.x;
                        evt.Use();
                    }
                    break;
                case EventType.MouseDown:
                    if (evt.button == 0)
                    {
                        var mousePos = evt.mousePosition;
                        if (IsPointOnRulerArea(mousePos))
                        {
                            dragingLine.isH = evt.mousePosition.x > evt.mousePosition.y;
                            GUIUtility.hotControl = MyControlId;
                            cursor = dragingLine.isH ? MouseCursor.ResizeVertical : MouseCursor.ResizeHorizontal;
                            evt.Use();
                        }
                        else if (IsPointOverLines(out line, mousePos))
                        {
                            GUIUtility.hotControl = MyControlId;
                            dragingLine = line;
                            cursor = dragingLine.isH ? MouseCursor.ResizeVertical : MouseCursor.ResizeHorizontal;
                            lines.Remove(line);
                            isDraging = true;
                            evt.Use();
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (evt.button == 0 && GUIUtility.hotControl == MyControlId)
                    {
                        isDraging = false;
                        GUIUtility.hotControl = 0;
                        cursor = MouseCursor.Arrow;
                        if (!IsPointOnRulerArea(evt.mousePosition))
                            lines.Add(dragingLine);
                    }
                    break;
                case EventType.MouseMove:
                    if (IsPointOverLines(out line, evt.mousePosition))
                        cursor = line.isH ? MouseCursor.ResizeVertical : MouseCursor.ResizeHorizontal;
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
                DrawLine(dragingLine);
            foreach (var line in lines)
                DrawLine(line);
        }

        private void DrawLine(Line line)
        {
            var p = World2Gui(new Vector3(line.p, line.p));
            var start = line.isH ? new Vector2(0, p.y) : new Vector2(p.x, 0);
            var end = line.isH ? new Vector2(size.x, p.y) : new Vector2(p.x, size.y);
            var p1 = Gui2World(start);
            var p2 = Gui2World(end);
            Handles.color = Settings.RulerLineColor;
            Handles.DrawLine(p1, p2);
        }

        private bool IsPointOnRulerArea(Vector2 uiPos)
        {
            return uiPos.x < RULER_SIZE || uiPos.y < RULER_SIZE;
        }

        private bool IsPointOverLines(out Line line, Vector2 uiPos)
        {
            foreach(var l in lines)
            {
                if (IsPointOverLine(l, uiPos))
                {
                    line = l;
                    return true;
                }
            }
            line = new Line();
            return false;
        }

        private bool IsPointOverLine(Line line, Vector2 uiPos)
        {
            var p = World2Gui(new Vector3(line.p, line.p));
            if (line.isH)
                return Mathf.Abs(p.y - uiPos.y) < 3;
            else
                return Mathf.Abs(p.x - uiPos.x) < 3;
        }

        private void DrawTexture(Vector2 size)
        {
            var texture = Utils.LoadTexture("ruler");
            GUI.DrawTexture(new Rect(Vector2.zero, size), texture, ScaleMode.StretchToFill);
        }

        private Vector3 Gui2World(Vector2 uiPos)
        {
            var ray = HandleUtility.GUIPointToWorldRay(uiPos);
            var pos = ray.origin;
            pos.z = 500;
            return pos;
        }
        private Vector2 World2Gui(Vector3 wpos)
        {
            return HandleUtility.WorldToGUIPoint(wpos);
        }
    }
}
