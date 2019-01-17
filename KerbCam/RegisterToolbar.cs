namespace KerbCam.UI
{
    using KSP.Localization;
    using ToolbarControl_NS;
    using UnityEngine;

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        private void Start()
        {
            ToolbarControl.RegisterMod(KerbCamLauncherButton.MODID, KerbCamLauncherButton.MODNAME);
        }
    }
}
