using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RandomizerRoller.Models
{
    [Serializable]
    public class Job : IEquatable<Job>, IEqualityComparer<Job>
    {
        public static Job None { get; set; } = new Job("None", new string[0]);
        public string Name { get; set; }
        public string[] Weapons { get; set; }

        public Job(string name, string[] weapons)
        {
            Name = name;
            Weapons = weapons;
        }

        public bool Equals(Job other)
        {
            return other == null ? false : Name == other.Name;
        }

        public bool Equals(Job x, Job y)
        {
            if (x != null && y != null)
                return Name == x.Name && Name == y.Name;

            return false;
        }

        public int GetHashCode(Job obj)
        {
            return Name.GetHashCode();
        }
    }
}
