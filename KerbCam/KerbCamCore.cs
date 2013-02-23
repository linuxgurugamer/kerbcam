using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.IO;


namespace KerbCam {

    // Class purely for the purpose for injecting the plugin.
    // Plugin startup taken from:
    // http://forum.kerbalspaceprogram.com/showthread.php/43027
    public class Bootstrap : KSP.Testing.UnitTest {
        public Bootstrap() {
            var gameObject = new GameObject("KerbCam", typeof(KerbCam));
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }
    }

    // Plugin behaviour class.
    public class KerbCam : MonoBehaviour {
        private bool isEnabled = false;
        private MainWindow mainWindow;
        private State state;

        // TODO: Custom keybindings.
        private Event KEY_PATH_TOGGLE_RUNNING = Event.KeyboardEvent(KeyCode.Home.ToString());
        private Event KEY_PATH_ADD_POINT = Event.KeyboardEvent(KeyCode.Insert.ToString());
        private Event KEY_PATH_TOGGLE_WINDOW = Event.KeyboardEvent(KeyCode.F8.ToString());

        // TODO: Remove this logging thing.
        private Event KEY_DEBUG_LOG = Event.KeyboardEvent(KeyCode.F7.ToString());

        public void OnLevelWasLoaded() {
            isEnabled = (FlightGlobals.fetch == null || FlightGlobals.ActiveVessel == null)
                && HighLogic.LoadedScene == GameScenes.FLIGHT;

            if (!isEnabled) {
                if (state != null) {
                    state.Stop();
                }
                if (mainWindow != null) {
                    mainWindow.HideWindow();
                }
            } else {
                if (state == null) {
                    state = new State();
                }
                if (mainWindow == null) {
                    mainWindow = new MainWindow(state);
                }
            }
        }

        public void Update() {
            if (!isEnabled)
                return;

            if (state.selectedPath != null)
                state.selectedPath.Update();
        }

        public void OnGUI() {
            if (!isEnabled)
                return;

            try {

                var ev = Event.current;

                if (state.selectedPath != null) {
                    // Events that require an active path.
                    if (ev.Equals(KEY_PATH_ADD_POINT)) {
                        state.selectedPath.AddKey(FlightCamera.fetch.transform);
                    } else if (ev.Equals(KEY_PATH_TOGGLE_RUNNING)) {
                        state.selectedPath.ToggleRunning(FlightCamera.fetch);
                    }
                }

                if (ev.Equals(KEY_DEBUG_LOG)) {
                    Transform trn = FlightCamera.fetch.transform;

                    int i = 0;
                    for (var t = trn; t != null; t = t.parent, i++) {
                        Debug.Log(string.Format(
                            "[{0}] position={1} rotation={2} localPosition={3} localRotation={4}" +
                            " right={5} up={6} forward={7} localScale={8} localEulerAngles={9}",
                            i, t.position, t.rotation, t.localPosition, t.localRotation,
                            t.right, t.up, t.forward, t.localScale, t.localEulerAngles));
                    }
                    /*Debug.Log("Cameras:");
                    foreach (var c in Camera.allCameras)
                    {
                        string suffix = (c == Camera.current ? "(current)" : "");
                        Debug.Log(string.Format(
                            "{0} name={1} tag={2} enabled={3} transform={4} depth={5} object id={6}",
                            suffix, c.name, c.tag, c.enabled, c.transform, c.depth, c.GetInstanceID()));
                    }*/
                } else if (ev.Equals(KEY_PATH_TOGGLE_WINDOW)) {
                    mainWindow.ToggleWindow();
                }
            } catch (Exception e) {
                Debug.LogError(e.ToString() + "\n" +  e.StackTrace);
            }
        }
    }

    class State {
        public SimpleCamPath selectedPath;
        public List<SimpleCamPath> paths = new List<SimpleCamPath>();
        public int numCreatedPaths = 0;

        public void Stop() {
            if (selectedPath != null)
                selectedPath.StopRunning();
        }
    }

    class MainWindow {
        private const int WINDOW_ID = 73469086; // xkcd/221 compliance.
        private bool isWindowOpen = false;
        private SimpleCamPathEditor pathEditor = null;
        private Rect windowPos;
        private State state;
        private Vector2 pathListScroll = new Vector2();

        public MainWindow(State state) {
            this.state = state;
            windowPos = new Rect(Screen.width / 2, Screen.height / 2, 200, 300);
        }

        public void ToggleWindow() {
            if (isWindowOpen) {
                HideWindow();
            } else {
                ShowWindow();
            }
        }

        public void ShowWindow() {
            isWindowOpen = true;
            RenderingManager.AddToPostDrawQueue(3, new Callback(DrawGUI));
            GUI.FocusWindow(WINDOW_ID);
        }

        public void HideWindow() {
            isWindowOpen = false;
            RenderingManager.RemoveFromPostDrawQueue(3, new Callback(DrawGUI));
        }

        private void DrawGUI() {
            GUI.skin = HighLogic.Skin;
            windowPos = GUILayout.Window(
                WINDOW_ID, windowPos, DoGUI, "KerbCam",
                GUILayout.MinWidth(200),
                GUILayout.MinHeight(300));
        }

        private void DoGUI(int windowID) {
            try {
                C.InitGUIConstants();

                if (state.selectedPath != null) {
                    if (pathEditor == null || !pathEditor.IsForPath(state.selectedPath)) {
                        pathEditor = state.selectedPath.MakeEditor();
                        windowPos.width = 500;
                    }
                } else {
                    pathEditor = null;
                    windowPos.width = 200;
                }

                GUILayout.BeginHorizontal(); // BEGIN left/right panes

                GUILayout.BeginVertical(); // BEGIN main controls

                if (GUILayout.Button("New simple path")) {
                    state.numCreatedPaths++;
                    state.selectedPath = new SimpleCamPath(
                        "Path #" + state.numCreatedPaths,
                        2);
                    state.paths.Add(state.selectedPath);
                }

                // Scroll list allowing selection of an existing path.
                pathListScroll = GUILayout.BeginScrollView(pathListScroll, false, true);
                foreach (var path in state.paths) {
                    if (GUILayout.Toggle(path == state.selectedPath, path.Name)) {
                        if (state.selectedPath != path) {
                            state.selectedPath.StopRunning();
                            state.selectedPath = path;
                        }
                    }
                }
                GUILayout.EndScrollView();

                GUILayout.EndVertical(); // END main controls

                // Path editor lives in right-hand-frame.
                if (pathEditor != null) {
                    pathEditor.DoGUI();
                }

                GUILayout.EndHorizontal(); // END left/right panes

                GUI.DragWindow(new Rect(0, 0, 10000, 20));
            } catch (Exception e) {
                Debug.LogError(e.ToString() + "\n" + e.StackTrace);
            }
        }
    }
}
