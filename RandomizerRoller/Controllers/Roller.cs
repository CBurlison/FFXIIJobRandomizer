using RandomizerRoller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomizerRoller.Controllers
{
    public class Roller
    {
        public Roll Assignments { get; set; }

        private Job[] _jobs;
        private string[] _characterList;

        public Roller(bool unique, bool weapons,
            bool classic, bool mainOnly, bool randomCharacters)
        {
            Assignments = new Roll(unique, weapons, classic, mainOnly, randomCharacters);

            _characterList = BuildCharacterList();
            _jobs = BuildJobList();
        }

        public static Job[] BuildJobList()
        {
            return new[] {
                new Job("Knight", new[] { "Swords", "Greatswords" }),
                new Job("Monk", new[] { "Unarmed", "Poles" }),
                new Job("White Mage", new[] { "Rods" }),
                new Job("Black Mage", new[] { "Staves" }),
                new Job("Red Battlemage", new[] { "Maces" }),
                new Job("Shikari", new[] { "Daggers", "Ninja Swords" }),
                new Job("Uhlan", new[] { "Spears" }),
                new Job("Bushi", new[] { "Katana" }),
                new Job("Foebreaker", new[] { "Axes && Hammers", "Hand-bombs" }),
                new Job("Time Battlemage", new[] { "Crossbows" }),
                new Job("Machinist", new[] { "Guns", "Measures" }),
                new Job("Archer", new[] { "Bows" })
            };
        }

        public static string[] BuildCharacterList()
        {
            return new[] { "Vaan", "Balthier", "Fran", "Basch", "Ashe", "Penelo" };
        }

        public void Roll()
        {
            Assignments.Characters = new List<Character>();
            DateTime now = DateTime.Now;
            Random rand = new Random(now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second + now.Millisecond);

            RollJobs(rand);
            RollWeapons(rand);
            RollCharacters(rand);
        }

        private void RollCharacters(Random rand)
        {
            if (Assignments.RandomCharacters)
            {
                string newName = _characterList[rand.Next(_characterList.Length)];
                Assignments.Characters[0].Name = newName;

                for (int i = 1; i < Assignments.Characters.Count; i++)
                {
                    while (NameExists(newName))
                        newName = _characterList[rand.Next(_characterList.Length)];

                    Assignments.Characters[i].Name = newName;
                }
            }
        }

        private void RollJobs(Random rand)
        {
            int max = Assignments.Classic ? 6 : 12;

            for (var i = 0; i <= 2; i++)
            {
                Job main = null;

                while (main == null)
                {
                    Job rolled = _jobs[rand.Next(max)];

                    if ((Assignments.Unique && !CheckExists(rolled)) || !Assignments.Unique)
                    {
                        Character ch = new Character();
                        ch.Main = rolled;
                        Assignments.Characters.Add(ch);
                        main = rolled;
                    }
                }

                if (!Assignments.MainOnly && !Assignments.Classic)
                {
                    Job sub = null;

                    while (sub == null)
                    {
                        Job rolled = _jobs[rand.Next(max)];

                        if (Assignments.Characters[i].Main != rolled && ((Assignments.Unique && !CheckExists(rolled)) || !Assignments.Unique))
                        {
                            Assignments.Characters[i].Sub = rolled;
                            sub = rolled;
                        }
                    }
                }
                else
                {
                    Assignments.Characters[i].Sub = Job.None;
                }
            }
        }

        private bool CheckExists(Job rolled)
        {
            var jobs = new List<Job>(Assignments.Characters.Select(a => a.Main));

            if (!Assignments.MainOnly && !Assignments.Classic)
            {
                jobs.AddRange(Assignments.Characters.Select(a => a.Sub));
            }

            return jobs.Contains(rolled);
        }

        private void RollWeapons(Random rand)
        {
            if (Assignments.Weapons)
            {
                foreach (Character ch in Assignments.Characters)
                {
                    var weapons = new List<string>(ch.Main.Weapons);

                    if (!Assignments.MainOnly && !Assignments.Classic)
                    {
                        weapons.AddRange(ch.Sub.Weapons);
                    }

                    ch.Weapon = weapons[rand.Next(weapons.Count)];
                }
            }
        }

        private bool NameExists(string toCheck)
        {
            foreach (var c in Assignments.Characters)
                if (c.Name == toCheck)
                    return true;

            return false;
        }
    }
}
