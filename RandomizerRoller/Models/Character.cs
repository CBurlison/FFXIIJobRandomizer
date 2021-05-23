using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerRoller.Models
{
    public class Character
    {
        public Job Main { get; set; } = null;
        public Job Sub { get; set; } = null;
        public string Weapon { get; set; } = "Any";
        public string Name { get; set; } = "Any";
    }
}
