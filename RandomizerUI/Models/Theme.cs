using System.Drawing;

namespace RandomizerUI.Models
{
    public class Theme
    {
        public Color Background { get; set; }
        public Color Text { get; set; }

        public static Theme Light
        {
            get
            {
                Theme theme = new Theme
                {
                    Background = SystemColors.ControlLightLight,
                    Text = SystemColors.ControlText
                };

                return theme;
            }
        }

        public static Theme Dark
        {
            get
            {
                Theme theme = new Theme
                {
                    Background = SystemColors.ControlText,
                    Text = SystemColors.ControlLightLight
                };

                return theme;
            }
        }
    }
}
