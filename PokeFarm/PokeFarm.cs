using Newtonsoft.Json;
using PokeFarm.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokeFarm
{
    public partial class PokeFarm : Form
    {
        private const string baseURL = "https://pokeapi.co/api/v2/";

        public PokeFarm()
        {
            InitializeComponent();
        }

        private PokemonInfo LoadJson()
        {
            using (StreamReader r = new StreamReader(@"JSON/PokemonSrc.json"))
            {
                string json = r.ReadToEnd();
                PokemonInfo pkmInfo = JsonConvert.DeserializeObject<PokemonInfo>(json);
                /*foreach (var pkm in pkmInfo.results)
                {
                    MessageBox.Show("Pokemon: " + pkm.name);
                }*/
                return pkmInfo;
            }
        }

        private void DownloadJSON(string path, string fileName)
        {
            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                client.DownloadFile(baseURL + path.ToLower(), Directory.GetCurrentDirectory() + @"\json\" + fileName + ".json");
            }
        }

        private void SalirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PokemonToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DownloadJSON("pokemon/", "PokemonSrc");
            PokemonInfo pkmInfo = LoadJson();

            foreach (var pkm in pkmInfo.results)
            {
                DownloadJSON("pokemon/" + pkm.name, pkm.name);
                statusLabel.Text = "Descargado " + pkm.name;
            }

            MessageBox.Show("Descarga Completa");
        }

        private void apiLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            //Pasarle los parametros para que lo haga aca
        }
    }
}
