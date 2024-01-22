using System;

namespace RandomizerRoller.Models
{
    [Serializable]
    public class Character
    {
        public Job Main { get; set; } = null;
        public Job Sub { get; set; } = null;
        public string Weapon { get; set; } = "Any";
        public string Name { get; set; } = "Any";
    }
}
