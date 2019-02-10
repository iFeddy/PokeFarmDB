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
using System.Reflection;
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
            apiLoader.WorkerReportsProgress = true;
        }

        private PokemonInfo LoadJson(string folder, string file)
        {
            using (StreamReader r = new StreamReader(@"json\" + folder + @"\" + file + ".json"))
            {
                string json = r.ReadToEnd();
                PokemonInfo pkmInfo = JsonConvert.DeserializeObject<PokemonInfo>(json);
                return pkmInfo;
            }
        }

        private void DownloadJSON(string path, string folderName, string fileName)
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\json\" + folderName);

            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                client.DownloadFile(baseURL + path.ToLower(), Directory.GetCurrentDirectory() + @"\json\" + folderName + @"\" + fileName + ".json");
            }
        }

        private void SalirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PokemonToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            loadingForm lf = new loadingForm();
            apiLoader.RunWorkerAsync(1);
            lf.ShowDialog(this);
        }

        private void apiLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int argumento = (int)e.Argument;
            if(argumento == 1) // Pokemon
            {
                statusLabel.Text = "Descargando Listado de Pokemon de Servidores";

                DownloadJSON("pokemon/", "config", "pokemon"); //esto tiene que hacerse cuando inicia el programa

                PokemonInfo pkmInfo = LoadJson("config", "pokemon");

                int total = pkmInfo.count; //Para el Offset

                DownloadJSON("pokemon/?offset=0&limit=" + total, "config", "pokemon");
                pkmInfo = LoadJson("config", "pokemon");

                int i = 1;
                double porcentaje;
                statusLabel.Text = "Encontrados "+ pkmInfo.count.ToString() + " Pokemon. Iniciando Descarga...";

                foreach (var pkm in pkmInfo.results)
                {
                    porcentaje = ((double)i / total) * 100;
                    int showPorcentaje = Convert.ToInt32(porcentaje);
                    DownloadJSON("pokemon/" + pkm.name, "pokemon", pkm.name);
           
                    statusLabel.Text = "Descargado Pokemon: " + pkm.name.ToUpperInvariant() + " (" + pkm.url.ToString() + ")";
                    worker.ReportProgress(showPorcentaje);
                    i++;
                }                    
            }
            if(argumento == 2) //Pokedex
            {
                statusLabel.Text = "Descargando Pokedex de Servidores";

                DownloadJSON("pokemon-species/", "config", "pokemon-species"); //esto tiene que hacerse cuando inicia el programa
                PokemonInfo pkmInfo = LoadJson("config", "pokemon-species");

                int total = pkmInfo.count;

                DownloadJSON("pokemon-species/?offset=0&limit=" + total, "config", "pokemon-species");
                pkmInfo = LoadJson("config", "pokemon-species");

                int i = 1;
                double porcentaje;

                statusLabel.Text = "Encontradas " + pkmInfo.count.ToString() + " Entradas de Pokedex. Iniciando Descarga...";

                foreach (var pkm in pkmInfo.results)
                {
                    porcentaje = ((double)i / total) * 100;
                    int showPorcentaje = Convert.ToInt32(porcentaje);
                    DownloadJSON("pokemon-species/" + pkm.name, "pokemon-species", pkm.name);
                    statusLabel.Text = "Descargadando Entrada de Pokedex: " + pkm.name.ToUpperInvariant();
                    worker.ReportProgress(showPorcentaje);
                    i++;
                }
            }
            if (argumento == 3) //Types
            {
                statusLabel.Text = "Descargando Type de Servidores";

                DownloadJSON("type/", "config", "type");
                PokemonInfo pkmInfo = LoadJson("config", "type");

                int total = pkmInfo.count;     

                int i = 1;
                double porcentaje;

                statusLabel.Text = "Encontradas " + pkmInfo.count.ToString() + " Entradas de Type. Iniciando Descarga...";

                foreach (var type in pkmInfo.results)
                {
                    porcentaje = ((double)i / total) * 100;
                    int showPorcentaje = Convert.ToInt32(porcentaje);
                    DownloadJSON("type/" + type.name, "type", type.name);
                    statusLabel.Text = "Descargadando Type: " + type.name.ToUpperInvariant();
                    worker.ReportProgress(showPorcentaje);
                    i++;
                }
            }
        }

        private void apiLoader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var frm = Application.OpenForms.Cast<Form>().Where(x => x.Name == "loadingForm").FirstOrDefault();      
            frm.Controls["loadingProgress"].Text = (e.ProgressPercentage.ToString() + "%");            
        }

        private void apiLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var frm = Application.OpenForms.Cast<Form>().Where(x => x.Name == "loadingForm").FirstOrDefault();
            frm.Close();
            statusLabel.Text = "Listo";
        }

        private void abrirCarpetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Directory.GetCurrentDirectory() + @"\json\");
        }

        private void pokedexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadingForm lf = new loadingForm();
            apiLoader.RunWorkerAsync(2);
            lf.ShowDialog(this);
        }

        public static DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            System.Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        private void pokemonToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //Cargar el file json en Clase Pokemon
        }

        private void typeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadingForm lf = new loadingForm();
            apiLoader.RunWorkerAsync(3);
            lf.ShowDialog(this);
        }
    }
}
