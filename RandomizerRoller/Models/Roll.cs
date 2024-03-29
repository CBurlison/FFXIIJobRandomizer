﻿using System;
using System.Collections.Generic;

namespace RandomizerRoller.Models
{
    [Serializable]
    public class Roll
    {
        public bool Unique { get; set; }
        public bool Weapons { get; set; }
        public bool RandomCharacters { get; set; }
        public bool Classic { get; set; }
        public bool MainOnly { get; set; }
        public List<Character> Characters { get; set; }
        public string SaveLoc { get; set; }

        public Roll()
        {
            Unique = false;
            Weapons = false;
            Classic = false;
            MainOnly = false;
            RandomCharacters = false;

            SaveLoc = string.Empty;
            Characters = new List<Character>() { new Character(), new Character(), new Character() };
        }

        public Roll(bool unique, bool weapons,
            bool classic, bool mainOnly, bool randomCharacters)
        {
            Unique = unique;
            Weapons = weapons;
            Classic = classic;
            MainOnly = mainOnly;
            RandomCharacters = randomCharacters;

            SaveLoc = string.Empty;

            Characters = new List<Character>() { new Character(), new Character(), new Character() };
        }
    }
}
