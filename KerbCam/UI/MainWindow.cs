using KerbCam.Core;
using KerbCam.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbCam.UI
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class MainWindow : WindowBase
    {

        private SimpleCamPathEditor pathEditor = null;
        private Vector2 pathListScroll = new Vector2();
        private WindowResizer resizer;
        private bool cameraControlsOpen = false;
        private ManualCameraControlGUI cameraGui;
        

        /// <summary>
        ///     Gets the current instance if started or returns null.
        /// </summary>
        public static MainWindow Instance { get; private set; }

        protected override String GetName()
        {
            return "Main Window";
        }

        public override void ToggleWindow()
        {
            if(Visible == true)
            {
                HideChildWindows();
            }
            base.ToggleWindow();
            
        }

        public override void HideWindow()
        {
            Visible = false;
            HideChildWindows();
        }

        protected void Awake()
        {
            try {
                if (Instance == null)
                {
                    Instance = this;
                    resizer = new WindowResizer( new Rect(50, 50, 250, 250), new Vector2(GetGuiMinHeight(), GetGuiMinWidth()));                    
                    cameraGui = new ManualCameraControlGUI();
                    StateHandler.keyBindings.ListenKeyUp(BoundKey.KEY_TOGGLE_WINDOW, ToggleWindow);
                }
            }
            catch (Exception ex)
            {
                DebugUtil.LogException(ex);
            }
        }

        protected void Update()
        {
            StateHandler.keyBindings.HandleEvent(Event.current);
        }

        protected void OnGUI()
        {
            if(!Visible)
            {
                return;
            }

            GUI.skin = HighLogic.Skin;
            resizer.Position = GUILayout.Window(
                GetInstanceID(), resizer.Position, DrawGUI,
                string.Format(
                    "KerbCam [v{0}]",
                    KerbCamGlobals.assembly.GetName().Version.ToString(2)),
                resizer.LayoutMinWidth(),
                resizer.LayoutMinHeight());
        }

        private void DrawGUI(int windowID)
        {
            try
            {
                if (StateHandler.SelectedPath != null)
                {
                    // A path is selected.
                    if (pathEditor == null || !pathEditor.IsForPath(StateHandler.SelectedPath))
                    {
                        // Selected path has changed.
                        pathEditor = StateHandler.SelectedPath.MakeEditor();
                    }
                }
                else {
                    // No path is selected.
                    if (pathEditor != null)
                    {
                        pathEditor = null;
                    }
                }

                float minHeight = GetGuiMinHeight();
                float minWidth = GetGuiMinWidth();
                if (cameraControlsOpen)
                {
                    minHeight += cameraGui.GetGuiMinHeight();
                    minWidth = Math.Max(minWidth, cameraGui.GetGuiMinWidth());
                }
                if (pathEditor != null)
                {
                    minHeight = Math.Max(minHeight, pathEditor.GetGuiMinHeight());
                    minWidth += pathEditor.GetGuiMinWidth();
                }
                resizer.MinHeight = minHeight;
                resizer.MinWidth = minWidth;
                
                GUILayout.BeginVertical(); // BEGIN outer container

                GUILayout.BeginHorizontal(); // BEGIN left/right panes

                GUILayout.BeginVertical(GUILayout.MaxWidth(140)); // BEGIN main controls

                if (GUILayout.Button("New simple path"))
                {
                    StateHandler.SelectedPath = StateHandler.NewPath();
                }

                DoPathList();

                if (GUILayout.Button("Relative to"))
                {
                    VesselSelectionWindow.Instance.ToggleWindow();
                }

                bool pressed = GUILayout.Button(
                    (cameraControlsOpen ? "\u25bd" : "\u25b9")
                    + " Camera controls",
                    WindowStyles.FoldButtonStyle);
                cameraControlsOpen = cameraControlsOpen ^ pressed;
                if (cameraControlsOpen)
                {
                    cameraGui.DrawGUI();
                }

                GUILayout.EndVertical(); // END main controls

                // Path editor lives in right-hand-frame.
                if (pathEditor != null)
                {
                    pathEditor.DoGUI();
                }
                
                GUILayout.EndHorizontal(); // END left/right panes
                
                GUILayout.BeginHorizontal(); // BEGIN lower controls

                if (GUILayout.Button("Save"))
                {
                    StateHandler.SavePaths();
                }
                
                if (GUILayout.Button("Load"))
                {
                    StateHandler.LoadPaths();
                }

                if (GUILayout.Button("Config\u2026"))
                {
                    ConfigWindow.Instance.ToggleWindow();
                }
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("?", WindowStyles.WindowButtonStyle))
                {
                    HelpWindow.Instance.ToggleWindow();
                }

                DrawCloseButton();
                
                resizer.HandleResize();

                GUILayout.EndHorizontal(); // END lower controls

                GUILayout.EndVertical(); // END outer container

                GUI.DragWindow(new Rect(0, 0, 10000, 25));
            }
            catch (Exception e)
            {
                DebugUtil.LogException(e);
            }
        }

        private void DoPathList()
        {
            var noExpandWidth = GUILayout.ExpandWidth(false);

            // Scroll list allowing selection of an existing path.
            pathListScroll = GUILayout.BeginScrollView(pathListScroll, false, true);
            for (int i = 0; i < StateHandler.paths.Count; i++)
            {
                GUILayout.BeginHorizontal(noExpandWidth); // BEGIN path widgets
                if (GUILayout.Button("X", WindowStyles.DeleteButtonStyle, noExpandWidth))
                {
                    StateHandler.RemovePathAt(i);
                    if (i >= StateHandler.paths.Count)
                    {
                        break;
                    }
                }

                {
                    var path = StateHandler.paths[i];
                    bool isSelected = path == StateHandler.SelectedPath;
                    bool doSelect = GUILayout.Toggle(path == StateHandler.SelectedPath, "", noExpandWidth);
                    if (isSelected != doSelect)
                    {
                        if (doSelect)
                        {
                            StateHandler.SelectedPath = path;
                        }
                        else {
                            StateHandler.SelectedPath = null;
                        }
                    }
                    GUILayout.Label(path.Name);
                }
                GUILayout.EndHorizontal(); // END path widgets
            }
            GUILayout.EndScrollView();
        }
    

        private void HideChildWindows()
        {
            if (VesselSelectionWindow.Instance != null)
            {
                VesselSelectionWindow.Instance.Visible = false;
            } else
            {
                DebugUtil.Log("VesselSelectionWindow is null");
            }

            if (HelpWindow.Instance != null)
            {
                HelpWindow.Instance.Visible = false;
            }
            else
            {
                DebugUtil.Log("HelpWindow is null");
            }

            if (ConfigWindow.Instance != null)
            {
                ConfigWindow.Instance.Visible = false;
            }
            else
            {
                DebugUtil.Log("ConfigWindow is null");
            }

        }

        private float GetGuiMinHeight()
        {
            return 200;
        }

        private float GetGuiMinWidth()
        {
            return 280;
        }


    }
}
