using UnityEngine;

namespace KerbCam.UI
{
    public static class WindowStyles
    {
        public static string ChrTimes = "\u00d7";

        private static int WinButtonSize = 25;

        public static GUIStyle DeleteButtonStyle;
        public static GUIStyle DisabledButtonStyle;
        public static GUIStyle LinkButtonStyle;
        public static GUIStyle UnpaddedButtonStyle;
        public static GUIStyle WindowButtonStyle;
        public static GUIStyle FoldButtonStyle;

        static WindowStyles()
        {
            
            var activeTxt = MakeConstantTexture(new Color(1f, 1f, 1f, 0.3f));
            var litTxt = MakeConstantTexture(new Color(1f, 1f, 1f, 0.2f));
            var normalTxt = MakeConstantTexture(new Color(1f, 1f, 1f, 0.1f));
            var clearTxt = MakeConstantTexture(Color.clear);
            Color disabledTextColor = new Color(0.7f, 0.7f, 0.7f);
            //Color linkColor = new Color(0.8f, 0.8f, 1f, 1f);
            //Color border = new Color(1f, 1f, 1f, 0.7f);
            //LinkButtonStyle.normal.textColor = new Color(0f, 0f, 0.7f);

            GUISkin skin = HighLogic.Skin;

            DeleteButtonStyle = createButtonStyle(skin.button, null, null, null, null, Color.red, null, null, null, skin.button.alignment, false);
            DisabledButtonStyle = createButtonStyle(skin.button, null, null, null, null, disabledTextColor, null, null, null, skin.button.alignment, false);
            LinkButtonStyle = createButtonStyle(skin.button, null, null, null, null, Color.blue, null, null, null, skin.button.alignment, false);
                        
            UnpaddedButtonStyle = createButtonStyle(skin.button, null, null, null, null, skin.button.normal.textColor,
                null, new RectOffset(0, 0, 0, 0), new RectOffset(0, 0, 0, 0), skin.button.alignment, false);

            WindowButtonStyle = createButtonStyle(skin.button, activeTxt, litTxt, litTxt, normalTxt, skin.button.normal.textColor,
                new RectOffset(1, 1, 1, 1),
                new RectOffset(2, 2, 8, 2),
                new RectOffset(2, 2, 2, 2), TextAnchor.LowerCenter, true);

            FoldButtonStyle = createButtonStyle(skin.button, activeTxt, litTxt, litTxt, clearTxt, skin.button.normal.textColor, 
                new RectOffset(0, 0, 0, 0), null, null, TextAnchor.MiddleLeft, false);

        }

        private static GUIStyle createButtonStyle(GUIStyle btnStyle, Texture2D active, Texture2D focused, Texture2D hover, Texture2D normal, 
            Color color, RectOffset border, RectOffset margin, RectOffset padding, TextAnchor alignment, bool fixedSize)
        {
            GUIStyle style = new GUIStyle(btnStyle);

            if(active != null)
            {
                style.active.background = active;
            }

            if (focused != null)
            {
                style.focused.background = focused;
            }

            if (hover != null)
            {
                style.hover.background = hover;
            }

            if (normal != null)
            {
                style.normal.background = normal;
            }

            if(border != null)
            {
                style.border = border;
            }

            if (margin != null)
            {
                style.border = margin;
            }

            if (padding != null)
            {
                style.border = padding;
            }

            style.alignment = alignment;

            if (color != null)
            {
                style.active.textColor = color;
                style.focused.textColor = color;
                style.hover.textColor = color;
                style.normal.textColor = color;
            }

            if(fixedSize)
            {
                style.fixedHeight = WinButtonSize;
                style.fixedWidth = WinButtonSize;
            }

            return style;
        }

        private static Texture2D MakeConstantTexture(Color fill)
        {
            const int size = 32;
            Texture2D txt = new Texture2D(size, size);
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    txt.SetPixel(col, row, fill);
                }
            }
            txt.Apply();
            txt.Compress(false);
            return txt;
        }
    }
}
