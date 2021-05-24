using RandomizerRoller.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SixJobFiesta
{
    public partial class RollerUI : Form
    {
        public RollerUI()
        {
            InitializeComponent();
            ClearAll();
            toolTip.SetToolTip(chkUnique, "Rolls jobs without any duplicates.");
            toolTip.SetToolTip(chkWeapon, "Rolls required weapon based on jobs assigned. You must use this type of weapon once they become available, regardless of acquisition method. The use of a wiki is suggested.");
            toolTip.SetToolTip(chkAll, "Rolls 6 job combos. Allows the use of all 6 characters and all jobs will be assigned.");
            toolTip.SetToolTip(chkClassic, "Rolls Main jobs that existed in Final Fantasy 1. No Sub jobs can be used.");
            toolTip.SetToolTip(chkMainOnly, "Rolls only Main jobs.");
            toolTip.SetToolTip(chkCharacters, "Rolls character assignments.");
        }

        void btnRoll_Click(object sender, EventArgs e)
        {
            ClearAll();
            Roller roller = new Roller(chkUnique.Checked, chkWeapon.Checked, chkAll.Checked, chkClassic.Checked, chkMainOnly.Checked, chkCharacters.Checked);
            roller.Roll();

            var labels = GetControlsOfType<Label>();

            for (int i = 0; i < roller.Assignments.Count; i++)
            {
                Label main = labels.First(a => a.Tag != null && a.Tag.ToString() == $"Main{i + 1}");
                main.Text = roller.Assignments[i].Main.Name;
                Label sub = labels.First(a => a.Tag != null && a.Tag.ToString() == $"Sub{i + 1}");
                sub.Text = roller.Assignments[i].Sub.Name;
                Label weapon = labels.First(a => a.Tag != null && a.Tag.ToString() == $"Weapon{i + 1}");
                weapon.Text = roller.Assignments[i].Weapon;
                Label character = labels.First(a => a.Tag != null && a.Tag.ToString() == $"Char{i + 1}");
                character.Text = roller.Assignments[i].Name;
            }
        }

        void ClearAll()
        {
            var labels = GetControlsOfType<Label>();
            var toClear = labels.Where(a => a.Tag != null && 
            (a.Tag.ToString().StartsWith("Main") || a.Tag.ToString().StartsWith("Sub") 
            || a.Tag.ToString().StartsWith("Weapon") || a.Tag.ToString().StartsWith("Char")));

            foreach (Label lbl in toClear)
                lbl.Text = string.Empty;
        }

        private IEnumerable<T> GetControlsOfType<T>()
        {
            return from field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   where typeof(Component).IsAssignableFrom(field.FieldType)
                   let component = (Component)field.GetValue(this)
                   where component != null && component is T
                   select (T)Convert.ChangeType(component, typeof(T));
        }

        #region >>> Menu Strip Events
        private void unarmedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Unequipped");
        }

        private void swordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Swords");
        }

        private void daggersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Daggers");
        }

        private void axesAndHammersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Axes_and_hammers");
        }

        private void macesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Maces");
        }

        private void measuresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Measures");
        }

        private void greatswordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Greatswords");
        }

        private void katanaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Katana");
        }

        private void ninjaSwordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Ninja_swords");
        }

        private void spearsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Spears");
        }

        private void polesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Poles");
        }

        private void rodsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Rods");
        }

        private void stavesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Staves");
        }

        private void bowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Bows");
        }

        private void crossbowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Crossbows");
        }

        private void gunsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Guns");
        }

        private void handbombsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons#Hand-bombs");
        }
        #endregion >>> Menu Strip Events
    }
}