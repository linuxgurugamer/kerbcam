// 
//     Copyright (C) 2015 CYBUTEK - https://github.com/CYBUTEK
//     Copyright (C) 2016 Cartman09 - https://github.com/cartman09 
//
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using KerbCam.Core;
using KSP.UI;
using KSP.UI.Screens;
using System;
using System.IO;
using UnityEngine;
using ToolbarControl_NS;

namespace KerbCam.UI
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class KerbCamLauncherButton : MonoBehaviour
    {
        ToolbarControl toolbarControl;

        internal const string MODID = "kerbcam";
        internal const string MODNAME = "KerbCam";

        const string ButtonLoc = "KerbCam/PluginData/Textures/appicon";

        bool lastVisible = false;

        /// <summary>
        ///     Enables the button if not already enabled.
        /// </summary>
        public void Enable()
        {
            if (toolbarControl != null && !toolbarControl.Enabled)
            {
                toolbarControl.Enabled = true;
            }

        }

        /// <summary>
        ///     Enables and sets the button to off.
        /// </summary>
        public void SetOff()
        {
            Enable();

            if (toolbarControl != null && toolbarControl.buttonActive) 
            {
                toolbarControl.SetFalse();
            }

        }

        /// <summary>
        ///     Enables and sets the button to on.
        /// </summary>
        public void SetOn()
        {
            Enable();

            if (toolbarControl != null && toolbarControl.buttonActive)
            {
                toolbarControl.SetTrue();
            }
        }

        protected virtual void Awake()
        {
            addButton();
        }

        /// <summary>
        /// Update event called every frame
        /// </summary>
        protected virtual void Update()
        {
            // TODO find out how to get the current GameGUI state
            if (MainWindow.Instance != null && MainWindow.Instance.GuiVisible)
            {
                // Set the launcher button state according to the Main window visibility (could have been de/actived by kb shortcut)
                if (MainWindow.Instance.Visible != lastVisible)
                {
                    lastVisible = MainWindow.Instance.Visible;

                    if (MainWindow.Instance.Visible)
                    {
                        SetOn();
                    }
                    else
                    {
                        SetOff();
                    }
                }
            }
        }

        protected virtual void OnDestroy()
        {
            if (this.toolbarControl != null)
            {
                this.toolbarControl.OnDestroy();
                UnityEngine.Object.Destroy(this.toolbarControl);
                this.toolbarControl = null;
            }
        }

        /// <summary>
        ///     Called on button being toggled off.
        /// </summary>
        protected void OnFalse()
        {
            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.HideWindow();
            }
        }

        /// <summary>
        ///     Called after the application launcher is ready and the button created.
        /// </summary>
        protected void OnTrue()
        {
            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.ShowWindow();
            }
        }

        private void addButton()
        {
            if (toolbarControl == null)
            {
                this.toolbarControl = this.gameObject.AddComponent<ToolbarControl>();
                this.toolbarControl﻿.AddToAllToolbars(
                    OnTrue, OnFalse,
                    //OnHover, OnHoverOut,
                    //LocalOnEnable, LocalOnDisable,
                    
                    ApplicationLauncher.AppScenes.SPACECENTER |
                        ApplicationLauncher.AppScenes.FLIGHT |
                        ApplicationLauncher.AppScenes.MAPVIEW |
                        ApplicationLauncher.AppScenes.VAB |
                        ApplicationLauncher.AppScenes.SPH |
                        ApplicationLauncher.AppScenes.TRACKSTATION,

                    MODID,
                    "kerbcamButton",
                    ButtonLoc + "-38",
                    ButtonLoc + "-24",
                    MODNAME);
            }
        }
    }
}
