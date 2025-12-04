using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Lexora
{
    public partial class MainForm : Form
    {
        string rutaActual = "";

        public MainForm()
        {
            InitializeComponent();
            CargarVolumenPrincipal();
        }


        private void CargarVolumenPrincipal()
        {
            // Seleccionamos el volumen del sistema (donde está Windows)
            var drive = DriveInfo.GetDrives()
                                 .FirstOrDefault(d => d.IsReady && d.RootDirectory.FullName == Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)));

            if (drive != null)
            {
                // Nombre del volumen 
                string nombreVolumen = string.IsNullOrEmpty(drive.VolumeLabel) ? "Disco Local" : drive.VolumeLabel;

                // Botón muestra: NombreVolumen (C:)
                btnDiscoPrincipal.Text = $"{nombreVolumen} ({drive.Name})";

                // Guardamos ruta principal
                rutaActual = drive.RootDirectory.FullName;
            }
        }

        private void btnDiscoPrincipal_Click(object sender, EventArgs e)
        {
            CargarCarpetas(rutaActual);
        }

        private void CargarCarpetas(string ruta)
        {
            try
            {
                listViewArchivos.Items.Clear();

                // Botón para volver atrás
                if (Directory.GetParent(ruta) != null)
                {
                    ListViewItem volver = new ListViewItem("..");
                    volver.SubItems.Add("Carpeta");
                    volver.SubItems.Add("");
                    volver.SubItems.Add("");
                    listViewArchivos.Items.Add(volver);
                }

                // Cargar carpetas
                foreach (var carpeta in Directory.GetDirectories(ruta))
                {
                    DirectoryInfo info = new DirectoryInfo(carpeta);

                    ListViewItem item = new ListViewItem(info.Name);
                    item.SubItems.Add("Carpeta");  // TIPO
                    item.SubItems.Add("");         // Tamaño vacío (carpetas no tienen tamaño directo)
                    item.SubItems.Add(info.CreationTime.ToString("dd/MM/yyyy HH:mm"));

                    listViewArchivos.Items.Add(item);
                }

                // Cargar archivos
                foreach (var archivo in Directory.GetFiles(ruta))
                {
                    FileInfo info = new FileInfo(archivo);

                    ListViewItem item = new ListViewItem(info.Name);
                    item.SubItems.Add(info.Extension + " (Archivo)");
                    item.SubItems.Add(FormatearTamaño(info.Length));
                    item.SubItems.Add(info.CreationTime.ToString("dd/MM/yyyy HH:mm"));

                    listViewArchivos.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando: " + ex.Message);
            }
        }

        private string FormatearTamaño(long bytes)
        {
            if (bytes < 1024)
                return bytes + " B";

            double kb = bytes / 1024.0;
            if (kb < 1024)
                return kb.ToString("0.0") + " KB";

            double mb = kb / 1024.0;
            if (mb < 1024)
                return mb.ToString("0.0") + " MB";

            double gb = mb / 1024.0;
            return gb.ToString("0.0") + " GB";
        }


        private void listViewArchivos_DoubleClick(object sender, EventArgs e)
        {
            if (listViewArchivos.SelectedItems.Count == 0)
                return;

            string seleccion = listViewArchivos.SelectedItems[0].Text;

            // Si pulsa ".." → subir
            if (seleccion == "..")
            {
                rutaActual = Directory.GetParent(rutaActual).FullName;
                CargarCarpetas(rutaActual);
                return;
            }

            // Intentar entrar a una carpeta
            string nuevaRuta = Path.Combine(rutaActual, seleccion);

            if (Directory.Exists(nuevaRuta))
            {
                rutaActual = nuevaRuta;
                CargarCarpetas(rutaActual);
            }
            else
            {
                // Si es archivo, puedes abrirlo o ignorar
                MessageBox.Show("Es un archivo, no una carpeta.");
            }
        }

        private void btnSesion_Click(object sender, EventArgs e)
        {
            using (InicioSesion ventanaLogin = new InicioSesion())
            {
                var resultado = ventanaLogin.ShowDialog();

                if (resultado == DialogResult.OK && ventanaLogin.LoginCorrecto)
                {
                    string nombre = ventanaLogin.NombreUsuario;
                    btnSesion.Text = "Hola, " + nombre;
                }
            }
        }
    }
}