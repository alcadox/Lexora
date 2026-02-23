using Lexora.Pantallas.Menu.Filtros;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
//DESCARGAR PAQUETE  Microsoft.WindowsAPICodePack-Shell
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using iText.Kernel.Pdf; //INSTALAR PAQUETE itext7


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


                //si hay filtros de fecha activos
                bool tieneFiltrosFechaActivos = filtros.Fechas != null && filtros.Fechas.Any(f => f.Value.Desde.HasValue && f.Value.Hasta.HasValue);
                bool tieneFiltrosMetadatosActivos =
                filtros.FiltrarAutor || filtros.FiltrarTitulo || filtros.FiltrarAplicacion || filtros.FiltrarPaginas;

                bool hayAlgúnFiltroActivo = tieneFiltrosActivos || tieneFiltrosFechaActivos || tieneFiltrosMetadatosActivos;



                // 3. Cargar CARPETAS
                foreach (var carpeta in Directory.GetDirectories(ruta))
                {
                    DirectoryInfo info = new DirectoryInfo(carpeta);

                    // NUEVO: la carpeta puede cumplir por SU fecha
                    bool cumpleFechaCarpeta = CumpleFiltrosFechaCarpeta(info);

                    // SI HAY FILTROS ACTIVOS:
                    // - mostramos la carpeta si cumple por su propia fecha
                    // - o si dentro hay archivos que cumplan (para poder navegar)
                    if (hayAlgúnFiltroActivo)
                    {
                        bool tieneArchivosCumplen = CarpetaTieneArchivosQueCumplen(carpeta, extensionesPermitidas, tieneFiltrosActivos);

                        if (!cumpleFechaCarpeta && !tieneArchivosCumplen)
                            continue;
                    }

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


                    //+ filtros de fecha Y metadatos documentos
                    bool cumpleTipo = !tieneFiltrosActivos || extensionesPermitidas.Contains(ext);
                    bool cumpleFecha = CumpleFiltrosFecha(info);
                    bool cumpleMetadatos = CumpleFiltrosMetadatosDocumento(archivo);

                    // si cumple todo, se muestra
                    if (cumpleTipo && cumpleFecha && cumpleMetadatos)
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


        //========================= MÉTODO PARA CUMPLIR FILTROS DE FECHA =========================
        private bool CarpetaTieneArchivosQueCumplen(string rutaCarpeta, HashSet<string> extensionesPermitidas, bool tieneFiltrosTipo)
        {
            try
            {
                foreach (var archivo in Directory.GetFiles(rutaCarpeta))
                {
                    FileInfo info = new FileInfo(archivo);
                    string ext = info.Extension.ToLower();

                    bool cumpleTipo = !tieneFiltrosTipo || extensionesPermitidas.Contains(ext);
                    bool cumpleFecha = CumpleFiltrosFecha(info);
                    bool cumpleMetadatos = CumpleFiltrosMetadatosDocumento(archivo);

                    if (cumpleTipo && cumpleFecha && cumpleMetadatos)
                        return true;
                }
            }
            catch
            {
                //por si no ay permisos o falla algo pero no se rompe 
            }
            return false;
        }

        private bool CumpleFiltrosFecha(FileInfo info)
        {
            // Si no hay filtros de fecha guardados, cumple siempre
            if (filtros.Fechas == null || filtros.Fechas.Count == 0)
                return true;

            foreach (var kv in filtros.Fechas)
            {
                string nombreFiltro = kv.Key ?? "";
                DateTime? desde = kv.Value.Desde?.Date;
                DateTime? hasta = kv.Value.Hasta?.Date;

                // Si por lo que sea está incompleto, lo ignoramos
                if (!desde.HasValue || !hasta.HasValue)
                    continue;

                // Normalizamos para comparar sin tildes / mayúsculas
                string key = (nombreFiltro ?? "")
                        .ToLower()
                        .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("ü", "u")
                        .Replace(".", "") 
                        .Replace("  ", " ")
                        .Trim();

                DateTime valorAComparar;

                // Mapeo de filtro → propiedad real del archivo
                if (key.Contains("creacion") || key.Contains("archivos creados"))
                {
                    valorAComparar = info.CreationTime.Date;
                }
                else if (key.Contains("fec de ultima edicion") || key.Contains("modificado"))
                {
                    valorAComparar = info.LastWriteTime.Date;
                }
                else if (key.Contains("fec de ultimo acceso") || key.Contains("ultimo acceso"))
                {
                    valorAComparar = info.LastAccessTime.Date;
                }
                else if (key.Contains("antiguedad"))
                {
                    // Por ahora lo tratamos como rango por FECHA DE CREACIÓN (lo más coherente con tu calendario)
                    valorAComparar = info.CreationTime.Date;
                }
                else
                {
                    // Si llega un filtro desconocido, no filtramos por él
                    continue;
                }

                // Regla: tiene que estar dentro del rango (AND entre filtros activos)
                if (valorAComparar < desde.Value || valorAComparar > hasta.Value)
                    return false;
            }

            return true;
        }

        //======================fin de filtros de fecha========================
        // ===================== CUMPLE FILTROS FECHA PARA CARPETAS =====================
        private bool CumpleFiltrosFechaCarpeta(DirectoryInfo info)
        {
            // Si no hay filtros de fecha guardados, cumple siempre
            if (filtros.Fechas == null || filtros.Fechas.Count == 0)
                return true;

            foreach (var kv in filtros.Fechas)
            {
                string nombreFiltro = kv.Key ?? "";
                DateTime? desde = kv.Value.Desde?.Date;
                DateTime? hasta = kv.Value.Hasta?.Date;

                // Si está incompleto, lo ignoramos
                if (!desde.HasValue || !hasta.HasValue)
                    continue;

                string key = (nombreFiltro ?? "")
                    .ToLower()
                    .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("ü", "u")
                    .Replace(".", "")
                    .Replace("  ", " ")
                    .Trim();

                DateTime valorAComparar;

                // Mapeo igual que en archivos, pero usando DirectoryInfo
                if (key.Contains("creacion") || key.Contains("archivos creados"))
                {
                    valorAComparar = info.CreationTime.Date;
                }
                else if (key.Contains("fec de ultima edicion") || key.Contains("modificado"))
                {
                    valorAComparar = info.LastWriteTime.Date;
                }
                else if (key.Contains("fec de ultimo acceso") || key.Contains("ultimo acceso"))
                {
                    valorAComparar = info.LastAccessTime.Date;
                }
                else if (key.Contains("antiguedad"))
                {
                    valorAComparar = info.CreationTime.Date;
                }
                else
                {
                    continue;
                }

                // AND entre filtros activos
                if (valorAComparar < desde.Value || valorAComparar > hasta.Value)
                    return false;
            }

            return true;
        }
        //========= FIN CUMPLE FILTROS FECHA PARA CARPETAS ============




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



        // necesitas: NuGet "itext7" y "Microsoft.WindowsAPICodePack-Shell"
        // usings:
        // using Microsoft.WindowsAPICodePack.Shell;
        // using iText.Kernel.Pdf;

        private bool CumpleFiltrosMetadatosDocumento(string rutaArchivo)
        {
            try
            {
                // 1) si no hay filtros de metadatos activos, cumple siempre
                bool hayFiltrosMeta =
                    filtros.FiltrarAutor ||
                    filtros.FiltrarTitulo ||
                    filtros.FiltrarAplicacion ||
                    filtros.FiltrarPaginas;

                if (!hayFiltrosMeta)
                    return true;

                string ext = (Path.GetExtension(rutaArchivo) ?? "").ToLowerInvariant();

                // 2) si pide autor/titulo/paginas, solo tiene sentido en documentos
                bool soportaMetadatosDocumento =
                    ext == ".pdf" || ext == ".docx" || ext == ".doc" || ext == ".pptx" || ext == ".ppt" ||
                    ext == ".xlsx" || ext == ".xls" || ext == ".rtf" || ext == ".odt" || ext == ".ods" || ext == ".odp";

                bool pideCamposSoloDocumento = filtros.FiltrarAutor || filtros.FiltrarTitulo || filtros.FiltrarPaginas;
                if (pideCamposSoloDocumento && !soportaMetadatosDocumento)
                    return false;

                // 3) normalizar filtros del usuario
                string filtroAutor = (filtros.AutorDocumento ?? "").Trim();
                string filtroTitulo = (filtros.TituloDocumento ?? "").Trim();
                string filtroApp = (filtros.AplicacionGeneradora ?? "").Trim();
                int? filtroPag = filtros.CantidadPaginas;

                // si están activos pero vacíos => no puede cumplir
                if (filtros.FiltrarAutor && string.IsNullOrWhiteSpace(filtroAutor)) return false;
                if (filtros.FiltrarTitulo && string.IsNullOrWhiteSpace(filtroTitulo)) return false;
                if (filtros.FiltrarAplicacion && string.IsNullOrWhiteSpace(filtroApp)) return false;
                if (filtros.FiltrarPaginas && !filtroPag.HasValue) return false;

                // 4) variables de metadatos leídos
                string autor = "";
                string titulo = "";
                string app = "";
                int? paginas = null;

                // 5) PDFs: leer metadatos reales con iText7 (Shell muchas veces viene vacío)
                if (ext == ".pdf")
                {
                    try
                    {
                        using (var reader = new PdfReader(rutaArchivo))
                        using (var pdf = new PdfDocument(reader))
                        {
                            var info = pdf.GetDocumentInfo();

                            autor = info.GetAuthor() ?? "";
                            titulo = info.GetTitle() ?? "";

                            // en PDF esto a veces viene como "Producer" o "Creator"
                            string creator = info.GetCreator() ?? "";
                            string producer = info.GetProducer() ?? "";
                            app = (creator + " " + producer).Trim();

                            // número de páginas real
                            paginas = pdf.GetNumberOfPages();
                        }
                    }
                    catch
                    {
                        // si falla leer el pdf, lo tratamos como sin metadatos
                        autor = "";
                        titulo = "";
                        app = "";
                        paginas = null;
                    }
                }
                else
                {
                    // 6) otros documentos: usar Windows Shell Properties
                    using (var shellFile = ShellFile.FromFilePath(rutaArchivo))
                    {
                        // autor (string[])
                        var autores = shellFile.Properties.System.Author.Value;
                        if (autores != null && autores.Length > 0)
                            autor = string.Join(" ", autores);

                        // título
                        titulo = shellFile.Properties.System.Title.Value ?? "";

                        // app / software usado
                        string app1 = shellFile.Properties.System.ApplicationName.Value ?? "";
                        string app2 = shellFile.Properties.System.SoftwareUsed.Value ?? "";
                        app = (app1 + " " + app2).Trim();

                        // páginas (si existe)
                        paginas = shellFile.Properties.System.Document.PageCount.Value;
                    }
                }

                // 7) aplicar filtros (AND)
                if (filtros.FiltrarAutor)
                {
                    if (string.IsNullOrWhiteSpace(autor) ||
                        autor.IndexOf(filtroAutor, StringComparison.OrdinalIgnoreCase) < 0)
                        return false;
                }

                if (filtros.FiltrarTitulo)
                {
                    if (string.IsNullOrWhiteSpace(titulo) ||
                        titulo.IndexOf(filtroTitulo, StringComparison.OrdinalIgnoreCase) < 0)
                        return false;
                }

                if (filtros.FiltrarAplicacion)
                {
                    if (string.IsNullOrWhiteSpace(app) ||
                        app.IndexOf(filtroApp, StringComparison.OrdinalIgnoreCase) < 0)
                        return false;
                }

                if (filtros.FiltrarPaginas)
                {
                    if (!paginas.HasValue) return false;

                    // por ahora: igualdad exacta
                    if (paginas.Value != filtroPag.Value)
                        return false;
                }

                return true;
            }
            catch
            {
                // permisos/archivos raros => no cumple
                return false;
            }
        }
    }
}