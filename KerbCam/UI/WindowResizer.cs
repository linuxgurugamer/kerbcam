using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbCam.UI
{
    public class WindowResizer
    {
        private static GUIContent gcDrag = new GUIContent("\u25E2");

        private bool isResizing = false;
        private Rect resizeStart = new Rect();

        private Vector2 minSize;
        private Rect position;

        public WindowResizer(Rect windowRect, Vector2 minSize)
        {
            this.position = windowRect;
            this.minSize = minSize;
        }

        public Rect Position
        {
            get { return position; }
            set { position = value; }
        }

        public float Width
        {
            get { return position.width; }
            set { position.width = value; }
        }

        public float Height
        {
            get { return position.height; }
            set { position.height = value; }
        }

        public float MinWidth
        {
            get { return minSize.x; }
            set { minSize.x = value; }
        }

        public float MinHeight
        {
            get { return minSize.y; }
            set { minSize.y = value; }
        }

        // Helpers to return GUILayoutOptions for GUILayout.Window.

        public GUILayoutOption LayoutMinWidth()
        {
            return GUILayout.MinWidth(minSize.x);
        }

        public GUILayoutOption LayoutMinHeight()
        {
            return GUILayout.MinHeight(minSize.y);
        }

        // Originally from the following URL and modified:
        // http://answers.unity3d.com/questions/17676/guiwindow-resize-window.html
        public void HandleResize()
        {
            Vector2 mouse = GUIUtility.ScreenToGUIPoint(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));

            Rect r = GUILayoutUtility.GetRect(gcDrag, WindowStyles.WindowButtonStyle);

            if (Event.current.type == EventType.mouseDown && r.Contains(mouse))
            {
                isResizing = true;
                resizeStart = new Rect(mouse.x, mouse.y, position.width, position.height);
                //Event.current.Use();  // the GUI.Button below will eat the event, and this way it will show its active state
            }
            else if (Event.current.type == EventType.mouseUp && isResizing)
            {
                isResizing = false;
            }
            else if (!Input.GetMouseButton(0))
            {
                // if the mouse is over some other window we won't get an event, this just kind of circumvents that by checking the button state directly
                isResizing = false;
            }
            else if (isResizing)
            {
                position.width = Mathf.Max(minSize.x, resizeStart.width + (mouse.x - resizeStart.x));
                position.height = Mathf.Max(minSize.y, resizeStart.height + (mouse.y - resizeStart.y));
                position.xMax = Mathf.Min(Screen.width, position.xMax);  // modifying xMax affects width, not x
                position.yMax = Mathf.Min(Screen.height, position.yMax);  // modifying yMax affects height, not y
            }

            GUI.Button(r, gcDrag, WindowStyles.WindowButtonStyle);
        }
    }
}
