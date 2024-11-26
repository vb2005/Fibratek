using Fibratek;
using Fibratek.Data;
using Fibratek.Hardware;
using Fibratek.Models;
using Fibratek.UI;
using Fibratek.UI.Controls;

namespace Fibratek
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new SettingsForm().ShowDialog();    
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new MainForm().ShowDialog();
        }
    }
}