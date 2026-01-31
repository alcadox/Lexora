using Lexora.Pantallas.Menu.Filtros;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Lexora
{
    public partial class MainForm : Form
    {
        string rutaActual = "";

        // instancia de la clase filtros
        ClaseFiltros filtros = new ClaseFiltros();
        string nombreUsuario = "";

        public MainForm()
        {
            InitializeComponent();
            CargarVolumenPrincipal();
        }

        public MainForm(string nombreUsuario)
        {
            InitializeComponent();
            CargarVolumenPrincipal();
            this.nombreUsuario = nombreUsuario;
        }


        private void CargarVolumenPrincipal()
        {

            if (!string.IsNullOrEmpty(nombreUsuario))
            {
                btnSesion.Text = "Bienvenido, " + nombreUsuario;
                btnSesion.Enabled = false;
            }
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

            string texto = string.Join("\n", filtros.TiposArchivo.Select(f => $"{f.Key}: {f.Value}"));
            MessageBox.Show(texto);

            CargarCarpetas(rutaActual);
        }

        /* ANTERIOR MÉTODO SIN FILTROS, CONSERVADO POR SI SE NECESITA
         
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
        */

        // Formatear tamaño de bytes a KB, MB, GB
        // funciona de la siguiente manera:
        // si es menor a 1024 bytes → muestra en bytes
        // si es menor a 1024 KB → muestra en KB
        // si es menor a 1024 MB → muestra en MB
        // si es mayor o igual a 1024 MB → muestra en GB
        // Ejemplo: 2048 bytes → 2.0 KB
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
            Hide();
            using (InicioSesion ventanaLogin = new InicioSesion(true))
            {
                var resultado = ventanaLogin.ShowDialog();

                if (resultado == DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(ventanaLogin.NombreUsuario))
                    {
                        btnSesion.Text = "Bienvenido, " + ventanaLogin.NombreUsuario;
                        btnSesion.Enabled = false;
                    }
                    Show();
                }
                else
                {
                    Show();
                }
            }
        }

        // cuando pulse el botón de filtros se abrirá una nueva ventana de filtros
        private void btnFiltros_Click(object sender, EventArgs e)
        {
            MainFiltros ventanaFiltros = new MainFiltros(filtros);

            // Suscribirse al evento para actualizar la lista al aplicar filtros
            ventanaFiltros.FiltrosAplicados += (s, args) =>
            {
                // Llamar al método para aplicar los filtros seleccionados
                AplicarFiltrosTipoArchivo();
            };

            ventanaFiltros.ShowDialog();
        }

        // ========================= MÉTODO PARA APLICAR FILTROS DE TIPO DE ARCHIVO A TODO EL DISCO =========================
        private void AplicarFiltrosTipoArchivo()
        {
            // Simplemente refrescamos la vista actual. 
            // La lógica de filtrado reside dentro de CargarCarpetas 
            // para que siempre que se navegue o se filtre, se respete la selección.
            CargarCarpetas(rutaActual);
        }
        private void CargarCarpetas(string ruta)
        {
            try
            {
                listViewArchivos.BeginUpdate(); // Optimización visual: evita parpadeo
                listViewArchivos.Items.Clear();

                // 1. Botón para volver atrás (Siempre visible)
                if (Directory.GetParent(ruta) != null)
                {
                    ListViewItem volver = new ListViewItem("..");
                    volver.SubItems.Add("Carpeta");
                    volver.SubItems.Add("");
                    volver.SubItems.Add("");
                    listViewArchivos.Items.Add(volver);
                }

                // 2. Obtener las extensiones permitidas desde el diccionario ya formateado
                // Usamos un HashSet para que la búsqueda sea O(1) en lugar de O(n)
                var extensionesPermitidas = new HashSet<string>(
                    filtros.TiposArchivo.Where(x => x.Value).Select(x => x.Key.ToLower()),
                    StringComparer.OrdinalIgnoreCase
                );

                bool tieneFiltrosActivos = extensionesPermitidas.Count > 0;

                // 3. Cargar CARPETAS
                foreach (var carpeta in Directory.GetDirectories(ruta))
                {
                    DirectoryInfo info = new DirectoryInfo(carpeta);

                    // Lógica especial: Si hay filtros de "Comprimidos" (ej. .zip), 
                    // las carpetas se siguen mostrando porque podrías querer navegar a buscar archivos dentro.
                    // Si quieres ocultar carpetas cuando hay filtros, podrías añadir un if(!tieneFiltrosActivos).

                    ListViewItem item = new ListViewItem(info.Name);
                    item.SubItems.Add("Carpeta");
                    item.SubItems.Add("");
                    item.SubItems.Add(info.CreationTime.ToString("dd/MM/yyyy HH:mm"));
                    listViewArchivos.Items.Add(item);
                }

                // 4. Cargar ARCHIVOS (Aquí aplicamos el filtro real)
                foreach (var archivo in Directory.GetFiles(ruta))
                {
                    FileInfo info = new FileInfo(archivo);
                    string ext = info.Extension.ToLower();

                    // Si no hay filtros, mostramos todo. Si hay filtros, solo lo que coincida.
                    if (!tieneFiltrosActivos || extensionesPermitidas.Contains(ext))
                    {
                        ListViewItem item = new ListViewItem(info.Name);
                        item.SubItems.Add(info.Extension + " (Archivo)");
                        item.SubItems.Add(FormatearTamaño(info.Length));
                        item.SubItems.Add(info.CreationTime.ToString("dd/MM/yyyy HH:mm"));
                        listViewArchivos.Items.Add(item);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("No tienes permisos para acceder a esta carpeta.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando: " + ex.Message);
            }
            finally
            {
                listViewArchivos.EndUpdate();
            }
        }

    }
}