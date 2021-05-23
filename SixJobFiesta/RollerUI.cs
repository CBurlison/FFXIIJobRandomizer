using RandomizerRoller.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            toolTip.SetToolTip(chkAll, "Rolls 6 job combos. Allows the use of all 6 characters. Must be used in order of character acquisition.");
            toolTip.SetToolTip(chkClassic, "Rolls Main jobs that existed in Final Fantasy 1. No Sub jobs can be used.");
            toolTip.SetToolTip(chkMainOnly, "Rolls only Main jobs.");
        }

        void btnRoll_Click(object sender, EventArgs e)
        {
            ClearAll();
            Roller roller = new Roller(chkUnique.Checked, chkWeapon.Checked, chkAll.Checked, chkClassic.Checked, chkMainOnly.Checked);
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
            }
        }

        void ClearAll()
        {
            var labels = GetControlsOfType<Label>();
            var toClear = labels.Where(a => a.Tag != null && 
            (a.Tag.ToString().StartsWith("Main") || a.Tag.ToString().StartsWith("Sub") || a.Tag.ToString().StartsWith("Weapon")));

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
    }
}
