using RandomizerRoller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomizerRoller.Controllers
{
    public class Roller
    {
        public bool Unique { get; set; }
        public bool Weapons { get; set; }
        public bool All { get; set; }
        public bool Characters { get; set; }
        public bool Classic { get; set; }
        public bool MainOnly { get; set; }
        public List<Character> Assignments { get; set; }

        private Job[] _jobs;
        private string[] _characterList = new[] { "Vaan", "Balthier", "Fran", "Basch", "Ashe", "Penelo" };

        public Roller(bool unique, bool weapons,
            bool all, bool classic, bool mainOnly, bool characters)
        {
            Unique = unique;
            Weapons = weapons;
            All = all;
            Classic = classic;
            MainOnly = mainOnly;
            Characters = characters;

            _jobs = new[] { 
                new Job("Knight", new[] { "Swords", "Greatswords" }),
                new Job("Monk", new[] { "Unarmed", "Poles" }),
                new Job("White Mage", new[] { "Rods" }),
                new Job("Black Mage", new[] { "Staves" }),
                new Job("Red Battlemage", new[] { "Maces" }),
                new Job("Shikari", new[] { "Daggers", "Ninja Swords" }),
                new Job("Uhlan", new[] { "Spears" }),
                new Job("Bushi", new[] { "Katanas" }),
                new Job("Foebreaker", new[] { "Axes & Hammers", "Hand-bombs" }),
                new Job("Time Battlemage", new[] { "Crossbows" }),
                new Job("Machinest", new[] { "Guns", "Measures" }),
                new Job("Archer", new[] { "Bows" }) 
            };
        }

        public void Roll()
        {
            Assignments = new List<Character>();
            DateTime now = DateTime.Now;
            Random rand = new Random(now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second + now.Millisecond);

            RollJobs(rand);
            RollWeapons(rand);
            RollCharacters(rand);
        }

        private void RollCharacters(Random rand)
        {
            if (All)
            {
                for (int i = 0; i < Assignments.Count; i++)
                {
                    Assignments[i].Name = _characterList[i];
                }
            }
            else if (Characters)
            {
                string newName = _characterList[rand.Next(_characterList.Length)];
                Assignments[0].Name = newName;

                for (int i = 1; i < Assignments.Count; i++)
                {
                    while (NameExists(newName))
                        newName = _characterList[rand.Next(_characterList.Length)];

                    Assignments[i].Name = newName;
                }
            }
        }

        private void RollJobs(Random rand)
        {
            int max = Classic ? 6 : 12;
            int characters = All ? 5 : 2;

            for (var i = 0; i <= characters; i++)
            {
                Job main = null;

                while (main == null)
                {
                    Job rolled = _jobs[rand.Next(max)];

                    if ((Unique && !CheckExists(rolled)) || !Unique)
                    {
                        Character ch = new Character();
                        ch.Main = rolled;
                        Assignments.Add(ch);
                        main = rolled;
                    }
                }

                if (!MainOnly && !Classic)
                {
                    Job sub = null;

                    while (sub == null)
                    {
                        Job rolled = _jobs[rand.Next(max)];

                        if (Assignments[i].Main != rolled && ((Unique && !CheckExists(rolled)) || !Unique))
                        {
                            Assignments[i].Sub = rolled;
                            sub = rolled;
                        }
                    }
                }
                else
                {
                    Assignments[i].Sub = Job.None;
                }
            }
        }

        private bool CheckExists(Job rolled)
        {
            var jobs = new List<Job>(Assignments.Select(a => a.Main));

            if (!MainOnly && !Classic)
            {
                jobs.AddRange(Assignments.Select(a => a.Sub));
            }

            return jobs.Contains(rolled);
        }

        private void RollWeapons(Random rand)
        {
            if (Weapons)
            {
                foreach (Character ch in Assignments)
                {
                    var weapons = new List<string>(ch.Main.Weapons);

                    if (!MainOnly && !Classic)
                    {
                        weapons.AddRange(ch.Sub.Weapons);
                    }

                    ch.Weapon = weapons[rand.Next(weapons.Count)];
                }
            }
        }

        private bool NameExists(string toCheck)
        {
            foreach (var c in Assignments)
                if (c.Name == toCheck)
                    return true;

            return false;
        }
    }
}
