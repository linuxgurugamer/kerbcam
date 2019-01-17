using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KerbCam.UI;
using KerbCam.Camera;

namespace KerbCam.Core
{

    class StateHandler
    {
        public static KeyBindings<BoundKey> keyBindings;
        public static List<SimpleCamPath> paths;
        public static CameraController camControl;
        public static ManualCameraControl manCamControl;

        //public static bool stockToolbar = true;
        public static bool developerMode = false;

        private static SimpleCamPath selectedPath;
        private static int numCreatedPaths = 0;
        private static GameObject camControlObj;

       static StateHandler()
        {
            keyBindings = new KeyBindings<BoundKey>();

            keyBindings.AddBinding(BoundKey.KEY_TOGGLE_WINDOW,
                new KeyBind("toggle KerbCam window",
                    // Binding required if Toolbar isn't available.
                    /*!ToolbarManager.ToolbarAvailable*/ true,
                    KeyCode.F8));

            // Playback controls.
            keyBindings.AddBinding(BoundKey.KEY_PATH_TOGGLE_RUNNING,
                new KeyBind("play/stop selected path", false, KeyCode.Insert));
            keyBindings.AddBinding(BoundKey.KEY_PATH_TOGGLE_PAUSE,
                new KeyBind("pause selected path", false, KeyCode.Home));

            // Manual camera control keys.
            keyBindings.AddBinding(BoundKey.KEY_TRN_UP, new KeyBind("translate up"));
            keyBindings.AddBinding(BoundKey.KEY_TRN_FORWARD, new KeyBind("translate forward"));
            keyBindings.AddBinding(BoundKey.KEY_TRN_LEFT, new KeyBind("translate left"));
            keyBindings.AddBinding(BoundKey.KEY_TRN_RIGHT, new KeyBind("translate right"));
            keyBindings.AddBinding(BoundKey.KEY_TRN_DOWN, new KeyBind("translate down"));
            keyBindings.AddBinding(BoundKey.KEY_TRN_BACKWARD, new KeyBind("translate backward"));
            keyBindings.AddBinding(BoundKey.KEY_ROT_ROLL_LEFT, new KeyBind("roll left"));
            keyBindings.AddBinding(BoundKey.KEY_ROT_UP, new KeyBind("pan up"));
            keyBindings.AddBinding(BoundKey.KEY_ROT_ROLL_RIGHT, new KeyBind("roll right"));
            keyBindings.AddBinding(BoundKey.KEY_ROT_LEFT, new KeyBind("pan left"));
            keyBindings.AddBinding(BoundKey.KEY_ROT_RIGHT, new KeyBind("pan right"));
            keyBindings.AddBinding(BoundKey.KEY_ROT_DOWN, new KeyBind("pan down"));

            keyBindings.AddBinding(BoundKey.KEY_DEBUG, new KeyBind("log debug data (developer mode only)"));
            keyBindings.ListenKeyUp(BoundKey.KEY_DEBUG, HandleDebug);

            paths = new List<SimpleCamPath>();
            camControlObj = new GameObject("KerbCam.CameraController");
            UnityEngine.Object.DontDestroyOnLoad(camControlObj);
            camControl = camControlObj.AddComponent<CameraController>();
            manCamControl = ManualCameraControl.Create();

            LoadConfig();
            LoadPaths();

        }

        public static SimpleCamPath SelectedPath
        {
            get
            {
                return selectedPath;
            }
            set
            {
                if (selectedPath != null)
                {
                    selectedPath.Runner.StopRunning();
                    selectedPath.StopDrawing();
                    camControl.StopControlling();
                    selectedPath.Runner.enabled = false;
                }
                selectedPath = value;
                if (value != null)
                {
                    value.Runner.enabled = true;
                }
            }
        }
        private static void HandleDebug()
        {
            if (developerMode)
            {
                // Random bits of logging used by the developer to
                // work out whatever the heck he's doing.
                DebugUtil.LogCamerasTransformTree();
                DebugUtil.LogVessel(FlightGlobals.ActiveVessel);
            }
        }

        public static void LoadConfig()
        {
            ConfigNode config;
            try
            {
                config = ConfigNode.Load(KerbCamGlobals.AssemblyPath + "/kerbcam.cfg");
            }
            catch (NullReferenceException)
            {
                Debug.LogWarning("KerbCam encountered NRE while loading kerbcam.cfg - file corrupted?");
                return;
            }

            if (config == null)
            {
                Debug.LogWarning("KerbCam could not load its configuration. This is okay if one has not been saved yet.");
                return;
            }

            keyBindings.Load(config.GetNode("KEY_BINDINGS"));

            //ConfigUtil.Parse<bool>(config, "TOOLBAR_STOCK", out stockToolbar, true);
            ConfigUtil.Parse<bool>(config, "DEV_MODE", out developerMode, false);
        }

        public static void SaveConfig()
        {
            var config = new ConfigNode();

            keyBindings.Save(config.AddNode("KEY_BINDINGS"));

            //ConfigUtil.Write<bool>(config, "TOOLBAR_STOCK", stockToolbar);
            ConfigUtil.Write<bool>(config, "DEV_MODE", developerMode);

            if (!config.Save(KerbCamGlobals.AssemblyPath + "/kerbcam.cfg"))
            {
                Debug.LogError("Could not save to kerbcam.cfg");
            }
        }

        internal const string KERBCAM_PATHS = "GameData/KerbCam/PluginData/KerbCam-paths.cfg";
        public static void LoadPaths()
        {
            ConfigNode config;
            try
            {
                config = ConfigNode.Load(KERBCAM_PATHS);
            }
            catch (NullReferenceException)
            {
                Debug.LogWarning("KerbCam encountered NRE while loading kerbcam-paths.cfg - file corrupted?");
                return;
            }
            if (config == null)
            {
                Debug.LogWarning(
                    "KerbCam could not load paths. This is okay if " +
                    "they have not been saved yet.");
                return;
            }
            var newPaths = new List<SimpleCamPath>();
            foreach (var pathNode in config.GetNodes("PATH"))
            {
                var path = new SimpleCamPath();
                path.Load(pathNode);
                newPaths.Add(path);
            }
            paths = newPaths;
            SelectedPath = null;
        }

        public static void SavePaths()
        {
            var config = new ConfigNode();
            foreach (var path in paths)
            {
                path.Save(config.AddNode("PATH"));
            }
            if (!config.Save(KERBCAM_PATHS))
            {
                Debug.LogError("Could not save to kerbcam-paths.cfg");
            }
        }

        public static void RemovePathAt(int index)
        {
            var path = paths[index];
            if (path == selectedPath)
            {
                SelectedPath = null;
            }
            paths.RemoveAt(index);
            path.Destroy();
        }

        public static SimpleCamPath NewPath()
        {
            numCreatedPaths++;
            var newPath = new SimpleCamPath("Path #" + numCreatedPaths);
            paths.Add(newPath);
            return newPath;
        }

        public static void Stop()
        {
            camControl.StopControlling();
            if (SelectedPath != null)
            {
                SelectedPath.StopDrawing();
                SelectedPath = null;
            }
            MainWindow.Instance.HideWindow();
        }

    }
}
