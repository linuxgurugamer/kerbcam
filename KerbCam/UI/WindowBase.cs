using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbCam.UI
{
    public abstract class WindowBase : MonoBehaviour
    {

        private bool visible = false;

        /// <summary>
        /// Flag for window visibility
        /// </summary>
        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                visible = value;
            }
        }

        public virtual void ToggleWindow()
        {            
            bool before = visible;
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
