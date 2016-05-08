using KSP.UI;
using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbCam.UI
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class AppLauncherButton : MonoBehaviour
    {
        private static Texture s_IconTexture;
        private ApplicationLauncherButton m_Button;

        /// <summary>
        ///     Gets the wrapped application launcher button object.
        /// </summary>
        public ApplicationLauncherButton Button
        {
            get
            {
                return m_Button;
            }
        }

        /// <summary>
        ///     Gets or sets the toggle button state.
        /// </summary>
        public bool IsOn
        {
            get
            {
                return m_Button != null &&
                       m_Button.IsEnabled &&
                       m_Button.toggleButton.CurrentState == UIRadioButton.State.True;
            }
            set
            {
                if (m_Button == null)
                {
                    return;
                }

                if (value)
                {
                    SetOn();
                }
                else
                {
                    SetOff();
                }
            }
        }

        /// <summary>
        ///     Disables the button if not already disabled.
        /// </summary>
        public void Disable()
        {
            if (m_Button != null && m_Button.IsEnabled)
            {
                m_Button.Disable();
            }
        }

        /// <summary>
        ///     Enables the button if not already enabled.
        /// </summary>
        public void Enable()
        {
            if (m_Button != null && !m_Button.IsEnabled)
            {
                m_Button.Enable();
            }
        }

        /// <summary>
        ///     Gets the anchor position for pop-up content.
        /// </summary>
        public Vector3 GetAnchor()
        {
            if (m_Button == null)
            {
                return Vector3.zero;
            }

            Vector3 anchor = m_Button.GetAnchor();

            anchor.x -= 3.0f;

            return anchor;
        }

        /// <summary>
        ///     Enables and sets the button to off.
        /// </summary>
        public void SetOff()
        {
            Enable();

            if (m_Button != null && m_Button.toggleButton.CurrentState != UIRadioButton.State.False)
            {
                m_Button.SetFalse();
            }
        }

        /// <summary>
        ///     Enables and sets the button to on.
        /// </summary>
        public void SetOn()
        {
            Enable();

            if (m_Button != null && m_Button.toggleButton.CurrentState != UIRadioButton.State.True)
            {
                m_Button.SetTrue();
            }
        }

        protected virtual void Awake()
        {
            // cache icon texture
            if (s_IconTexture == null)
            {
                s_IconTexture = loadLauncherIcon();
            }

            // subscribe event listeners
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIApplicationLauncherReady);
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(OnGUIApplicationLauncherUnreadifying);
        }

        protected virtual void OnDestroy()
        {
            // unsubscribe event listeners
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIApplicationLauncherReady);
            GameEvents.onGUIApplicationLauncherUnreadifying.Remove(OnGUIApplicationLauncherUnreadifying);
        }

        /// <summary>
        ///     Called on button being disabled.
        /// </summary>
        protected virtual void OnDisable() { }

        /// <summary>
        ///     Called on button being enabled.
        /// </summary>
        protected virtual void OnEnable() { }

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

        /// <summary>
        ///     Called on mouse hovering.
        /// </summary>
        protected virtual void OnHover() { }

        /// <summary>
        ///     Called on mouse exiting hover.
        /// </summary>
        protected virtual void OnHoverOut() { }

        /// <summary>
        ///     Called on button being ready.
        /// </summary>
        protected virtual void OnReady() { }

        /// <summary>
        ///     Called after the application launcher is unreadified and the button removed.
        /// </summary>
        protected virtual void OnUnreadifying() { }

        private void OnGUIApplicationLauncherReady()
        {
            // create button
            if (ApplicationLauncher.Instance != null)
            {
                m_Button = ApplicationLauncher.Instance.AddModApplication(OnTrue, OnFalse, OnHover, OnHoverOut, OnEnable, OnDisable, ApplicationLauncher.AppScenes.ALWAYS, s_IconTexture);
            }

            OnReady();
        }

        private void OnGUIApplicationLauncherUnreadifying(GameScenes scene)
        {
            // remove button
            if (ApplicationLauncher.Instance != null && m_Button != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(m_Button);
            }

            OnUnreadifying();
        }

        private Texture loadLauncherIcon()
        {
            Texture2D texture = new Texture2D(24, 24, TextureFormat.ARGB32, false);
            texture.LoadImage((Byte[])File.ReadAllBytes(KerbCamGlobals.AssemblyPath + "/icon.png"));

            return texture;
        }

    }
}
