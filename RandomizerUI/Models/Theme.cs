using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Theme theme = new Theme();
                theme.Background = SystemColors.ControlLightLight;
                theme.Text = SystemColors.ControlText;

                return theme;
            }
        }

        public static Theme Dark
        {
            get
            {
                Theme theme = new Theme();
                theme.Background = SystemColors.ControlText;
                theme.Text = SystemColors.ControlLightLight;

                return theme;
            }
        }
    }
}
