using Fibratek.Data;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fibratek.UI
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = LocalSettings.Instance;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Внимание!", "Внесение изменений в работающую систему может потребовать перезагрузку программы. Продолжить?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                LocalSettings.Instance.Save();
                Close();
            }
        }
    }
}
