using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokeFarm
{
    public partial class loadingForm : Form
    {
        public loadingForm()
        {
            InitializeComponent();
        }

        private void loadingProgress_TextChanged(object sender, EventArgs e)
        {
            string newText = loadingProgress.Text.Remove(loadingProgress.Text.Length - 1);
            int pbValue = Int32.Parse(newText);
            loadingProgressBar.Value = pbValue;
        }
    }
}
