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
    public class VesselSelectionWindow : WindowBase
    {
        private Vector2 vesselListScroll = new Vector2();
        private WindowResizer resizer;

        /// <summary>
        ///     Gets the current instance if started or returns null.
        /// </summary>
        public static VesselSelectionWindow Instance { get; private set; }

        protected override String GetName()
        {
            return "VesselSelection Window";
        }

        protected void Awake()
        {
            try
            {
                if (Instance == null)
                {
                    Instance = this;
                }
                resizer = new WindowResizer(new Rect(50, 250, 250, 200), new Vector2(200, 170));
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
                "Choose vessel",
                resizer.LayoutMinWidth(),
                resizer.LayoutMinHeight());
        }

        private void DrawGUI(int windowID)
        {
            try
            {

                GUILayout.BeginVertical(); // BEGIN outer container

                Transform relTrn = StateHandler.camControl.RelativeTrn;

                if (GUILayout.Toggle(relTrn == null, "Active vessel"))
                {
                    relTrn = null;
                }
                GUILayout.BeginScrollView(vesselListScroll); // BEGIN vessel list scroller
                GUILayout.BeginVertical(); // BEGIN vessel list
                foreach (Vessel vessel in FlightGlobals.Vessels)
                {
                    bool isVesselSelected = object.ReferenceEquals(relTrn, vessel.transform);
                    if (vessel.loaded && GUILayout.Toggle(isVesselSelected, vessel.name))
                    {
                        relTrn = vessel.transform;
                    }
                }
                GUILayout.EndVertical(); // END vessel list
                GUILayout.EndScrollView(); // END vessel list scroller

                StateHandler.camControl.RelativeTrn = relTrn;

                GUILayout.BeginHorizontal(); // BEGIN lower controls
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
    }
}
