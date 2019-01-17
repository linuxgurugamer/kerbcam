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
    public class HelpWindow : WindowBase
    {
        private WindowResizer resizer;
        private Vector2 helpScroll = new Vector2();
        private static string origHelpText = string.Join("", new string[]{
            "KerbCam is a basic utility to automatically move the flight",
            " camera along a given path.\n",
            "\n",
            "NOTE: at its current stage of development, it is very rough,",
            " potentially buggy, and feature incomplete. Use at your own risk.",
            " It is not inconceivable that this can crash your spacecraft or",
            " do other nasty things.\n",
            "\n",
            "Paths are saved when the save button is pressed.",
            "\n",
            "Keys: (changeable in Config window)\n",
            "{0}",
            "\n",
            "Create a new path, then add keys to it by positioning your view",
            " and add the key with the \"New key\" button. Existing points",
            " can be viewed with the \"View\" button or moved to the current",
            " view position with the \"Set\" button.\n",
            "\n",
            "If more flexible camera control is required, then press the",
            " \"Camera controls\" button to fold out the 6-degrees-of-freedom",
            " controls. The left hand controls control translation, and the",
            " right control orientation. The sliders above each control the",
            " rate of movement or orientation for fine or coarse control of",
            " the camera position and orientation.\n",
            "\n",
            "Source is hosted at https://github.com/cartman09/kerbcam under the",
            " BSD license.\n",
            "Original code hosted at https://github.com/huin/kerbcam ."}
        );
        private string helpText;

        /// <summary>
        ///     Gets the current instance if started or returns null.
        /// </summary>
        public static HelpWindow Instance { get; private set; }

        protected override String GetName()
        {
            return "Help Window";
        }

        protected void Awake()
        {
            try
            {
                if (Instance == null)
                {
                    Instance = this;
                }
                resizer = new WindowResizer(new Rect(330, 50, 300, 300), new Vector2(300, 150));
                StateHandler.keyBindings.anyChanged += UpdateHelpText;
                UpdateHelpText();
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
                "KerbCam Help",
                resizer.LayoutMinWidth(),
                resizer.LayoutMinHeight());
        }

        private void DrawGUI(int windowID)
        {
            try
            {
                GUILayout.BeginVertical(); // BEGIN outer container

                GUILayout.Label(string.Format(
                    "KerbCam [v{0}]", KerbCamGlobals.assembly.GetName().Version.ToString()));

                // BEGIN text scroller.
                helpScroll = GUILayout.BeginScrollView(helpScroll);
                GUILayout.TextArea(helpText);
                GUILayout.EndScrollView(); // END text scroller.

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                /*
                if (GUILayout.Button("Kerbcam on Spaceport", WindowStyles.LinkButtonStyle))
                {
                    Application.OpenURL("http://kerbalspaceport.com/kerbcam/");
                }*/
                if (GUILayout.Button("Report issue", WindowStyles.LinkButtonStyle))
                {
                    Application.OpenURL("https://github.com/cartman09/kerbcam/issues");
                }

                DrawCloseButton();

                resizer.HandleResize();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical(); // END outer container

                GUI.DragWindow(new Rect(0, 0, 10000, 25));
            }
            catch (Exception e)
            {
                DebugUtil.LogException(e);
            }
        }

        private void UpdateHelpText()
        {
            var fmtBindingParts = new List<string>();
            foreach (var kb in StateHandler.keyBindings.Bindings())
            {
                if (kb.IsBound())
                {
                    fmtBindingParts.Add(string.Format("* {0} [{1}]\n",
                        kb.description, kb.HumanBinding));
                }
            }
            string fmtBindings;
            if (fmtBindingParts.Count > 0)
            {
                fmtBindings = string.Join("", fmtBindingParts.ToArray());
            }
            else {
                fmtBindings = "<nothing bound>";
            }
            helpText = string.Format(origHelpText,
                fmtBindings);
        }

    }
}
