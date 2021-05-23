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
        public bool Classic { get; set; }
        public bool MainOnly { get; set; }
        public List<Character> Assignments { get; set; }

        private List<Job> _jobs = new List<Job>();

        public Roller (bool unique, bool weapons, bool all, bool classic, bool mainOnly)
        {
            Unique = unique;
            Weapons = weapons;
            All = all;
            Classic = classic;
            MainOnly = mainOnly;

            _jobs.Add(new Job("Knight", new[] { "Swords", "Greatswords" }));
            _jobs.Add(new Job("Monk", new[] { "Unarmed", "Poles" }));
            _jobs.Add(new Job("White Mage", new[] { "Rods" }));
            _jobs.Add(new Job("Black Mage", new[] { "Staves" }));
            _jobs.Add(new Job("Red Battlemage", new[] { "Maces" }));
            _jobs.Add(new Job("Shikari", new[] { "Daggers", "Ninja Swords" }));
            _jobs.Add(new Job("Uhlan", new[] { "Spears" }));
            _jobs.Add(new Job("Bushi", new[] { "Katanas" }));
            _jobs.Add(new Job("Foebreaker", new[] { "Axes & Hammers", "Hand-bombs" }));
            _jobs.Add(new Job("Time Battlemage", new[] { "Crossbows" }));
            _jobs.Add(new Job("Machinest", new[] { "Guns", "Measures" }));
            _jobs.Add(new Job("Archer", new[] { "Bows" }));
        }

        public void Roll()
        {
            Assignments = new List<Character>();
            DateTime now = DateTime.Now;
            Random rand = new Random(now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second + now.Millisecond);
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

            if (Weapons)
            {
                foreach (Character ch in Assignments)
                {
                    ch.Weapon = RollWeapon(rand, ch.Main, ch.Sub);
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

        private string RollWeapon(Random rand, Job main, Job sub)
        {
            var weapons = new List<string>(main.Weapons);

            if (!MainOnly && !Classic)
            {
                weapons.AddRange(sub.Weapons);
            }

            return weapons[rand.Next(weapons.Count)];
        }
    }
}
