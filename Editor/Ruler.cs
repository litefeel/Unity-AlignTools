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
            get
            {
                if (0 == _myControlId)
                    _myControlId = GUIUtility.GetControlID(typeof(Ruler).GetHashCode(), FocusType.Passive);
                return _myControlId;
            }
        }

        private bool isDraging = false;

        private int _MouseOverLineIdx = -1;
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

            int idx;
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
                        else if (IsPointOverLines(out idx, mousePos))
                        {
                            GUIUtility.hotControl = MyControlId;
                            dragingLine = lines[idx];
                            cursor = dragingLine.isH ? MouseCursor.ResizeVertical : MouseCursor.ResizeHorizontal;
                            lines.RemoveAt(idx);
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
                        {
                            _MouseOverLineIdx = lines.Count;
                            lines.Add(dragingLine);
                        }
                    }
                    break;
                case EventType.MouseMove:
                    if (IsPointOverLines(out _MouseOverLineIdx, evt.mousePosition))
                    {
                        var line = lines[_MouseOverLineIdx];
                        cursor = line.isH ? MouseCursor.ResizeVertical : MouseCursor.ResizeHorizontal;
                    }
                    else
                    {
                        cursor = MouseCursor.Arrow;
                    }
                    break;
            }

            if (cursor != MouseCursor.Arrow)
                EditorGUIUtility.AddCursorRect(new Rect(Vector2.zero, size), cursor);

            sceneView.Repaint();
        }

        private void DrawLines()
        {
            if (isDraging)
                DrawLine(dragingLine, Color.white);
            for (var i = lines.Count - 1; i >= 0; --i)
                DrawLine(lines[i], i == _MouseOverLineIdx ? Color.red : Settings.RulerLineColor);
        }

        private void DrawLine(Line line, Color color)
        {
            var p = World2Gui(new Vector3(line.p, line.p));
            var start = line.isH ? new Vector2(0, p.y) : new Vector2(p.x, 0);
            var end = line.isH ? new Vector2(size.x, p.y) : new Vector2(p.x, size.y);
            var p1 = Gui2World(start);
            var p2 = Gui2World(end);



            Handles.color = color;
            Handles.DrawLine(p1, p2);
        }

        private bool IsPointOnRulerArea(Vector2 uiPos)
        {
            return uiPos.x < RULER_SIZE || uiPos.y < RULER_SIZE;
        }

        private bool IsPointOverLines(out int idx, Vector2 uiPos)
        {
            for (var i = lines.Count - 1; i >= 0; --i)
            {
                if (IsPointOverLine(lines[i], uiPos))
                {
                    idx = i;
                    return true;
                }
            }
            idx = -1;
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
