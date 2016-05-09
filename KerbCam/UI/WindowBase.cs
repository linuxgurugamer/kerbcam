using System;
using UnityEngine;

namespace KerbCam.UI
{
    public abstract class WindowBase : MonoBehaviour
    {

        private bool visible = false;
        private bool guiVisible = true;

        /// <summary>
        /// Flag for window visibility
        /// </summary>
        public bool Visible
        {
            get
            {
                return visible && guiVisible;
            }

            set
            {
                visible = value;
            }
        }

        public bool GuiVisible
        {
            get
            {
                return guiVisible;
            }
        }

        public virtual void ToggleWindow()
        {            
            visible = !visible;
        }

        public virtual void HideWindow()
        {
            visible = false;
        }

        public virtual void ShowWindow()
        {
            visible = true;
        }

        public void onShowGameGUI()
        {
            guiVisible = true;
        }

        public void onHideGameGUI()
        {
            guiVisible = false;
        }

        protected virtual void Start()
        {
            GameEvents.onHideUI.Add(onHideGameGUI);
            GameEvents.onShowUI.Add(onShowGameGUI);
        }

        protected void DrawCloseButton()
        {
            if (GUILayout.Button(WindowStyles.ChrTimes, WindowStyles.WindowButtonStyle))
            {
                HideWindow();
            }
        }

        protected abstract String GetName();

    }
}
