using KerbCam.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ClickThroughFix;

namespace KerbCam.UI
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class ConfigWindow : WindowBase
    {

        private WindowResizer resizer = new WindowResizer( new Rect(50, 255, 380, 240), new Vector2(380, 240));
        private Vector2 scroll = new Vector2();
        private KeyBind captureTarget;

        /// <summary>
        ///     Gets the current instance if started or returns null.
        /// </summary>
        public static ConfigWindow Instance { get ; private set; }

        protected override String GetName()
        {
            return "Config Window";
        }

        protected void Awake()
        {
            try
            {
                if (Instance == null)
                {
                    Instance = this;
                }
            }
            catch (Exception ex)
            {
                DebugUtil.LogException(ex);
            }
        }

        protected void OnGUI()
        {

            if (!Visible)
            {
                return;
            }

            GUI.skin = HighLogic.Skin;
            resizer.Position = ClickThruBlocker.GUILayoutWindow(
                GetInstanceID(), resizer.Position, DrawGUI,
                "KerbCam configuration",
                resizer.LayoutMinWidth(),
                resizer.LayoutMinHeight());
        }

        private void DrawGUI(int windowID)
        {
            try
            {

                GUILayout.BeginVertical(); // BEGIN outer container

                // BEGIN vertical scroll.
                scroll = GUILayout.BeginScrollView(scroll);

                foreach (var kb in StateHandler.keyBindings.Bindings())
                {
                    DoBinding(kb);
                }

                //KerbCamLauncherButton.Instance.ShowButton = StateHandler.stockToolbar = GUILayout.Toggle(StateHandler.stockToolbar, "Use Stock toolbar");

                StateHandler.developerMode = GUILayout.Toggle(StateHandler.developerMode,"Developer mode - enables experimental features.");

                GUILayout.EndScrollView();
                // END vertical scroll.

                GUILayout.BeginHorizontal(); // BEGIN lower controls
                if (GUILayout.Button("Save"))
                {
                    StateHandler.SaveConfig();
                }
                if (GUILayout.Button("Load"))
                {
                    StateHandler.LoadConfig();
                }
                GUILayout.FlexibleSpace();
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

        private void DoBinding(KeyBind kb)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(kb.description, GUILayout.Width(165));
            string label;
            if (IsCapturing() && kb == captureTarget)
            {
                label = "...";
            }
            else {
                label = kb.HumanBinding;
            }
            if (GUILayout.Button(label, GUILayout.Width(110)))
            {
                if (captureTarget == null)
                {
                    StartKeyCapture(kb);
                }
                else {
                    CancelKeyCapture();
                }
            }
            if (!kb.IsRequiredBound() && GUILayout.Button("clear", WindowStyles.DeleteButtonStyle))
            {
                CancelKeyCapture();
                kb.SetBinding(null);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void HandleCapturedKey(Event ev)
        {
            if (ev.keyCode == KeyCode.Escape)
            {
                CancelKeyCapture();
            }
            else {
                CompleteKeyCapture(ev);
            }
        }

        private bool IsCapturing()
        {
            return captureTarget != null;
        }

        private void StartKeyCapture(KeyBind kb)
        {
            CancelKeyCapture();
            StateHandler.keyBindings.captureAnyKey += HandleCapturedKey;
            captureTarget = kb;
        }

        private void CancelKeyCapture()
        {
            StateHandler.keyBindings.captureAnyKey -= HandleCapturedKey;
            captureTarget = null;
        }

        private void CompleteKeyCapture(Event ev)
        {
            captureTarget.SetBinding(ev);
            StateHandler.keyBindings.captureAnyKey -= HandleCapturedKey;
            captureTarget = null;
        }
    }
}
