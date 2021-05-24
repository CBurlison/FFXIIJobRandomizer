using RandomizerRoller.Controllers;
using RandomizerUI.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
            toolTip.SetToolTip(chkClassic, "Rolls Main jobs that existed in Final Fantasy 1. No Sub jobs can be used.");
            toolTip.SetToolTip(chkMainOnly, "Rolls only Main jobs.");
            toolTip.SetToolTip(chkCharacters, "Rolls character assignments.");
        }

        void btnRoll_Click(object sender, EventArgs e)
        {
            ClearAll();
            Roller roller = new Roller(chkUnique.Checked, chkWeapon.Checked, chkClassic.Checked, chkMainOnly.Checked, chkCharacters.Checked);
            roller.Roll();

            var labels = GetControlsOfType<Label>();
            var pictures = GetControlsOfType<PictureBox>();

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

                PictureBox imgCharacter = pictures.First(a => a.Tag != null && a.Tag.ToString() == $"Char{i + 1}");
                if (roller.Assignments[i].Name != "Any")
                    imgCharacter.BackgroundImage = (Image)Resources.ResourceManager.GetObject(roller.Assignments[i].Name);

                PictureBox imgMain = pictures.First(a => a.Tag != null && a.Tag.ToString() == $"Main{i + 1}");
                if (roller.Assignments[i].Main.Name != "None")
                    imgMain.BackgroundImage = (Image)Resources.ResourceManager.GetObject(roller.Assignments[i].Main.Name);

                PictureBox imgSub = pictures.First(a => a.Tag != null && a.Tag.ToString() == $"Sub{i + 1}");
                if (roller.Assignments[i].Sub.Name != "None")
                    imgSub.BackgroundImage = (Image)Resources.ResourceManager.GetObject(roller.Assignments[i].Sub.Name);

                PictureBox imgWeapon = pictures.First(a => a.Tag != null && a.Tag.ToString() == $"Weapon{i + 1}");
                if (roller.Assignments[i].Weapon != "Any")
                    imgWeapon.BackgroundImage = (Image)Resources.ResourceManager.GetObject(roller.Assignments[i].Weapon);
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

            var images = GetControlsOfType<PictureBox>();
            var imagesToClear = images.Where(a => a.Tag != null &&
            (a.Tag.ToString().StartsWith("Main") || a.Tag.ToString().StartsWith("Sub")
            || a.Tag.ToString().StartsWith("Weapon") || a.Tag.ToString().StartsWith("Char")));

            foreach (PictureBox img in imagesToClear)
                img.BackgroundImage = null;
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
        private void weaponsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons");
        }

        private void jobsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/License_Board#List_of_jobs");
        }
        #endregion >>> Menu Strip Events
    }
}