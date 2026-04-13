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
using System.Drawing;

namespace Lexora
{
    public partial class MainForm : Form
    {
        string rutaActual = "";

        // instancia de la clase filtros
        ClaseFiltros filtros = new ClaseFiltros();
        string nombreUsuario = "";

        // Motor de iconos
        private ImageList listaIconos;

        public MainForm()
        {

            InitializeComponent();

            // INYECCIÓN DE OPTIMIZACIÓN LEXORA: Evita el parpadeo al cargar miles de archivos
            typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(listViewArchivos, true, null);

            //Inicializar la lista de iconos
            listaIconos = new ImageList();
            listaIconos.ColorDepth = ColorDepth.Depth32Bit; // Para que se vean en HD, no pixelados
            listaIconos.ImageSize = new Size(16, 16); // Tamaño clásico de Windows
            listViewArchivos.SmallImageList = listaIconos; // Enlazar al ListView

            // REGISTRO ÚNICO: Añadimos el icono de carpeta a la lista con una clave fija
            listaIconos.Images.Add("folder_default", Properties.Resources.carpeta_por_defecto_w11);
            listaIconos.Images.Add("folder_default_back", Properties.Resources.carpeta_back_por_defecto_w11);


            CargarVolumenPrincipal();
        }

        public MainForm(string nombreUsuario)
        {
            InitializeComponent();

            // INYECCIÓN DE OPTIMIZACIÓN LEXORA: Evita el parpadeo al cargar miles de archivos
            typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(listViewArchivos, true, null);

            // Inicializar la lista de iconos
            listaIconos = new ImageList();
            listaIconos.ColorDepth = ColorDepth.Depth32Bit; // Para que se vean en HD, no pixelados
            listaIconos.ImageSize = new Size(16, 16); // Tamaño clásico de Windows
            listViewArchivos.SmallImageList = listaIconos; // Enlazar al ListView

            // REGISTRO ÚNICO: Añadimos el icono de carpeta a la lista con una clave fija
            listaIconos.Images.Add("folder_default", Properties.Resources.carpeta_por_defecto_w11);
            listaIconos.Images.Add("folder_default_back", Properties.Resources.carpeta_back_por_defecto_w11);


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
        private void CargarCarpetas(string ruta, string textoBusqueda = "")
        {
            try
            {
                listViewArchivos.BeginUpdate();
                listViewArchivos.Items.Clear();

                // 1. Normalizamos la búsqueda
                string busqueda = (textoBusqueda ?? "").ToLower().Trim();

                // 2. Botón para volver atrás 
                // Solo lo mostramos si no es el directorio raíz
                if (Directory.GetParent(ruta) != null)
                {
                    ListViewItem volver = new ListViewItem("..");
                    volver.SubItems.Add("Carpeta");
                    volver.SubItems.Add("");
                    volver.SubItems.Add("");

                    volver.ImageKey = "folder_default_back";

                    listViewArchivos.Items.Add(volver);
                }

                // 3. Preparar filtros (HashSet para rendimiento O(1))
                var extensionesPermitidas = new HashSet<string>(
                    filtros.TiposArchivo.Where(x => x.Value).Select(x => x.Key.ToLower()),
                    StringComparer.OrdinalIgnoreCase
                );

                bool tieneFiltrosActivos = extensionesPermitidas.Count > 0;
                bool tieneFiltrosFechaActivos = filtros.Fechas != null && filtros.Fechas.Any(f => f.Value.Desde.HasValue && f.Value.Hasta.HasValue);
                bool tieneFiltrosMetadatosActivos = filtros.FiltrarAutor || filtros.FiltrarTitulo || filtros.FiltrarAplicacion || filtros.FiltrarPaginas;
                bool tieneFiltrosSeguridadActivos = filtros.Seguridad != null && filtros.Seguridad.Any(kv => kv.Value);
                bool tieneFiltrosMetadatosImagenesActivos = filtros.FiltrarResolucion || filtros.FiltrarFechaImagen || filtros.FiltrarModelo || filtros.FiltrarGPS;

                bool hayAlgúnFiltroActivo = tieneFiltrosActivos || tieneFiltrosFechaActivos || tieneFiltrosMetadatosActivos || tieneFiltrosSeguridadActivos || tieneFiltrosMetadatosImagenesActivos;

                // 4. Cargar CARPETAS (Optimizado con EnumerateDirectories)
                foreach (var carpeta in Directory.EnumerateDirectories(ruta))
                {
                    try
                    {
                        DirectoryInfo info = new DirectoryInfo(carpeta);

                        // FILTRO DE BÚSQUEDA POR NOMBRE
                        if (!string.IsNullOrEmpty(busqueda) && !info.Name.ToLower().Contains(busqueda))
                            continue;

                        // FILTROS DE CONTENIDO/FECHA
                        bool cumpleFechaCarpeta = CumpleFiltrosFechaCarpeta(info);
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

                        // ASIGNACIÓN DEL ICONO:
                        item.ImageKey = "folder_default";
                        
                        listViewArchivos.Items.Add(item);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // SOLUCIÓN BOMBA 1: Si esta subcarpeta no tiene permisos, 
                        // simplemente la saltamos y el bucle sigue vivo.
                        continue;
                    }
                    catch (PathTooLongException)
                    {
                        // Si la ruta supera el límite de Windows, la saltamos.
                        continue;
                    }
                }

                // 5. Cargar ARCHIVOS (Optimizado con EnumerateFiles)
                foreach (var archivo in Directory.EnumerateFiles(ruta))
                {
                    try
                    {
                        FileInfo info = new FileInfo(archivo);

                        // FILTRO DE BÚSQUEDA POR NOMBRE
                        if (!string.IsNullOrEmpty(busqueda) && !info.Name.ToLower().Contains(busqueda))
                            continue;

                        // FILTRO DE TAMAÑO
                        if (!CumpleFiltroTamano(archivo))
                            continue;

                        // FILTRO DE CONTENIDO
                        if (!CumpleFiltroContenido(archivo)) continue;

                        // FILTROS TÉCNICOS (Tipo, Fecha, Metadatos...)
                        bool cumpleSeguridad = CumpleFiltrosSeguridad(info);
                        bool cumpleTipo = !tieneFiltrosActivos || extensionesPermitidas.Contains(info.Extension.ToLower());
                        bool cumpleFecha = CumpleFiltrosFecha(info);
                        bool cumpleMetadatos = CumpleFiltrosMetadatosDocumento(archivo);
                        bool cumpleMetadatosImagen = CumpleFiltrosMetadatosImagen(archivo);

                        if (cumpleTipo && cumpleFecha && cumpleMetadatos && cumpleSeguridad && cumpleMetadatosImagen)
                        {
                            ListViewItem item = new ListViewItem(info.Name);
                            item.SubItems.Add(info.Extension + " (Archivo)");
                            item.SubItems.Add(FormatearTamaño(info.Length));
                            item.SubItems.Add(info.CreationTime.ToString("dd/MM/yyyy HH:mm"));

                            // ==========================================
                            // INYECCIÓN DE ICONOS REALES
                            // ==========================================
                            string ext = info.Extension.ToLowerInvariant();

                            // 1. Por defecto, la "llave" para guardar el icono es la extensión (ej. ".pdf")
                            string claveIcono = ext;

                            // 2. EXCEPCIÓNSi es un acceso directo o un ejecutable, 
                            // usamos la ruta completa del archivo para que no se sobreescriban entre ellos.
                            if (ext == ".lnk" || ext == ".exe" || ext == ".ico")
                            {
                                claveIcono = info.FullName;
                            }

                            // Si la clave no está en la memoria caché, extraemos el icono del archivo físico
                            if (!string.IsNullOrEmpty(claveIcono) && !listaIconos.Images.ContainsKey(claveIcono))
                            {
                                try
                                {
                                    // Extrae el icono oficial registrado en Windows
                                    System.Drawing.Icon iconoExtraido = System.Drawing.Icon.ExtractAssociatedIcon(archivo);
                                    if (iconoExtraido != null)
                                    {
                                        listaIconos.Images.Add(claveIcono, iconoExtraido);
                                    }
                                }
                                catch
                                {
                                    // Si falla al extraer (archivos corruptos), simplemente no tendrá icono
                                }
                            }

                            // Asignar el icono al elemento visual
                            if (listaIconos.Images.ContainsKey(claveIcono))
                            {
                                item.ImageKey = claveIcono;
                            }


                            listViewArchivos.Items.Add(item);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Si un archivo específico está bloqueado por el sistema, se salta.
                        continue;
                    }
                    catch (PathTooLongException)
                    {
                        continue;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Este catch externo SÓLO se dispara si la "ruta" principal (padre) 
                // no tiene permisos en absoluto (ej: intentar entrar en C:\Windows\Prefetch).
                MessageBox.Show("No tienes permisos de administrador para leer el contenido de esta carpeta principal.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al cargar la carpeta: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                listViewArchivos.EndUpdate();
            }
        }

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
                // Usar EnumerateFiles para no saturar la RAM
                foreach (var archivo in Directory.EnumerateFiles(rutaCarpeta))
                {
                    FileInfo info = new FileInfo(archivo);
                    string ext = info.Extension.ToLowerInvariant();

                    // Ordenamos de los filtros más RÁPIDOS a los más LENTOS.
                    // Si un archivo no pasa un filtro rápido, saltamos al siguiente archivo al instante.

                    // 1 Filtro de Tipo 
                    bool cumpleTipo = !tieneFiltrosTipo || extensionesPermitidas.Contains(ext);
                    if (!cumpleTipo) continue;

                    // 2 Filtro de Fecha (solo lee atributos de Windows)
                    bool cumpleFecha = CumpleFiltrosFecha(info);
                    if (!cumpleFecha) continue;

                    // 3. Filtro de Seguridad 
                    bool cumpleSeguridad = CumpleFiltrosSeguridad(info);
                    if (!cumpleSeguridad) continue;

                    // 4 Filtro de Metadatos de Documento (LENTO - Abre el archivo con iText7/Shell)
                    // El programa SOLO llega aquí si los 3 filtros anteriores pasaron la prueba.
                    bool cumpleMetadatos = CumpleFiltrosMetadatosDocumento(archivo);
                    if (!cumpleMetadatos) continue;

                    // 5 Filtro de Metadatos de Imagen (LENTO - Lee EXIF)
                    bool cumpleMetadatosImagen = CumpleFiltrosMetadatosImagen(archivo);
                    if (!cumpleMetadatosImagen) continue;

                    // Si el código sobrevive a todos los IFs, significa que este archivo cumple todo.
                    // Devolvemos true inmediatamente y evitamos leer el resto de la carpeta.
                    return true;
                }
            }
            catch
            {
                // Por si no hay permisos o falla algo, la carpeta simplemente se oculta
            }
            return false;
        }

        private bool CumpleFiltroTamano(string rutaArchivo)
        {
            if (!filtros.FiltrarTamano) return true; // Si el filtro no está activo, pasa la prueba

            try
            {
                FileInfo info = new FileInfo(rutaArchivo);
                long tamanoBytes = info.Length;

                // Comprobar si está dentro del rango definido en ClaseFiltros
                return tamanoBytes >= filtros.TamanoMin && tamanoBytes <= filtros.TamanoMax;
            }
            catch
            {
                return false; // Si hay error al leer el archivo (permisos, etc), no lo mostramos
            }
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

        // Cumple Filtros Contenido
        private bool CumpleFiltroContenido(string ruta)
        {
            // Si el filtro no está activado o el campo de búsqueda está vacío, 
            // dejamos pasar el archivo (devolvemos true)
            if (!filtros.FiltrarContenido || string.IsNullOrEmpty(filtros.TextoContenido)) return true;

            try
            {
                string ext = Path.GetExtension(ruta).ToLowerInvariant(); // Usamos ToLowerInvariant por seguridad con idiomas
                string busqueda = filtros.TextoContenido;

                // Configuramos si la búsqueda ignora o no las mayúsculas según el CheckBox
                var comparacion = filtros.IgnorarMayusculas ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

                // ARCHIVOS DE TEXTO PLANO O CÓDIGO
                string[] extTexto = { ".txt", ".cs", ".py", ".sql", ".json", ".xml", ".html", ".log", ".csv", ".ini", ".md" }; // Añadidos .ini y .md que son comunes
                if (extTexto.Contains(ext))
                {
                    // Leemos el archivo línea por línea (Lazy Evaluation).
                    // Consumo de RAM ultra bajo
                    foreach (var linea in File.ReadLines(ruta))
                    {
                        // Verificamos si la palabra clave existe dentro de esta línea concreta
                        if (linea.IndexOf(busqueda, comparacion) >= 0)
                        {
                            return true; // Encontrado. Salimos al instante y el archivo se cierra.
                        }
                    }
                    // Si termina el bucle, la palabra no estaba en todo el archivo.
                    return false;
                }

                // ARCHIVOS PDF (Usando la librería iText7)
                if (ext == ".pdf")
                {
                    // Abrimos el PDF en modo lectura
                    using (PdfReader pdfReader = new PdfReader(ruta))
                    using (PdfDocument pdfDoc = new PdfDocument(pdfReader))
                    {
                        // Recorremos el PDF página por página
                        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                        {
                            // Extraemos el texto de la página actual
                            string textoPagina = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));

                            // Si encontramos la palabra en esta página, ya no hace falta leer el resto
                            if (textoPagina.IndexOf(busqueda, comparacion) >= 0) return true;
                        }
                    }
                }
            }
            catch
            {
                // Si el archivo está abierto por otro programa (ej. Word usándolo) o no tenemos permisos, 
                // lo ignoramos para que la aplicación no se detenga. Es el comportamiento correcto aquí.
            }

            // Si llegamos aquí es porque el archivo no contenía el texto o no es de un tipo soportado
            return false;
        }

        // ===================== CUMPLE FILTROS DE SEGURIDAD =====================
        private bool CumpleFiltrosSeguridad(FileInfo info)
        {
            // Si no hay diccionario o no hay nada marcado => no filtrar
            if (filtros.Seguridad == null || filtros.Seguridad.Count == 0)
                return true;

            // Si NO hay ninguna entrada true => no filtrar
            if (!filtros.Seguridad.Values.Any(v => v))
                return true;

            var attr = info.Attributes;

            // AND entre filtros activos:
            if (SeguridadActiva("Archivos bloqueados por el sistema (solo lectura)"))
            {
                if (!attr.HasFlag(FileAttributes.ReadOnly))
                    return false;
            }

            if (SeguridadActiva("Archivos ocultos"))
            {
                if (!attr.HasFlag(FileAttributes.Hidden))
                    return false;
            }

            if (SeguridadActiva("Archivos cifrados"))
            {
                if (!attr.HasFlag(FileAttributes.Encrypted))
                    return false;
            }

            if (SeguridadActiva("Archivos protegidos por contraseña"))
            {
                // Si solo lo soportas para PDF:
                if (info.Extension.ToLowerInvariant() != ".pdf")
                    return false;

                if (!PdfTienePassword(info.FullName))
                    return false;
            }

            return true;
        }
        //===================== FIN CUMPLE FILTROS DE SEGURIDAD =====================
        private bool PdfTienePassword(string rutaPdf)
        {
            try
            {
                using (var reader = new iText.Kernel.Pdf.PdfReader(rutaPdf))
                using (var pdf = new iText.Kernel.Pdf.PdfDocument(reader))
                {
                    return false; // se abrió => no requiere password
                }
            }
            catch (Exception ex)
            {
                // iText lanza excepciones distintas según versión.
                // Esto cubre la mayoría de casos cuando está encriptado/requiere password.
                string msg = (ex.Message ?? "").ToLowerInvariant();
                if (msg.Contains("password") || msg.Contains("encrypted") || msg.Contains("encryption"))
                    return true;

                return false;
            }
        }
        private void listViewArchivos_DoubleClick(object sender, EventArgs e)
        {
            if (listViewArchivos.SelectedItems.Count == 0) return;

            string seleccion = listViewArchivos.SelectedItems[0].Text;

            if (seleccion == "..")
            {
                rutaActual = Directory.GetParent(rutaActual).FullName;
                txtBoxBuscador.Text = ""; // LIMPIAMOS AQUÍ
                CargarCarpetas(rutaActual);
                return;
            }

            string nuevaRuta = Path.Combine(rutaActual, seleccion);
            if (Directory.Exists(nuevaRuta))
            {
                rutaActual = nuevaRuta;
                txtBoxBuscador.Text = ""; // LIMPIAMOS AQUÍ
                CargarCarpetas(rutaActual);
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

        // ===================== CUMPLE FILTROS DE METADATOS DE IMÁGENES =====================
        private bool CumpleFiltrosMetadatosImagen(string rutaArchivo)
        {
            try
            {
                // 1) Si no hay filtros de imágenes activos, cumple siempre
                bool hayFiltrosImg =
                    filtros.FiltrarResolucion ||
                    filtros.FiltrarFechaImagen ||
                    filtros.FiltrarModelo ||
                    filtros.FiltrarGPS;

                if (!hayFiltrosImg)
                    return true;

                string ext = (Path.GetExtension(rutaArchivo) ?? "").ToLowerInvariant();

                // 2) Extensiones soportadas como imagen
                bool esImagen = ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".tiff" || ext == ".gif" || ext == ".webp";

                if (hayFiltrosImg && !esImagen)
                    return false;

                // 3) Leer propiedades usando ShellFile
                using (var shellFile = ShellFile.FromFilePath(rutaArchivo))
                {
                    if (filtros.FiltrarResolucion)
                    {
                        var anchoProp = shellFile.Properties.System.Image.HorizontalSize.Value;
                        var altoProp = shellFile.Properties.System.Image.VerticalSize.Value;

                        if (anchoProp == null || altoProp == null) return false;

                        if (filtros.ResolucionAncho.HasValue && anchoProp.Value != (uint)filtros.ResolucionAncho.Value) return false;
                        if (filtros.ResolucionAlto.HasValue && altoProp.Value != (uint)filtros.ResolucionAlto.Value) return false;
                    }

                    if (filtros.FiltrarFechaImagen)
                    {
                        // 1. Intentamos obtener la fecha de toma (EXIF)
                        DateTime? fechaReferencia = shellFile.Properties.System.Photo.DateTaken.Value;

                        // 2. Si no hay fecha de toma, probamos con la fecha de creación del archivo
                        if (!fechaReferencia.HasValue)
                        {
                            fechaReferencia = shellFile.Properties.System.DateCreated.Value;
                        }

                        // 3. Si aún así no hay (muy raro), probamos con la de modificación
                        if (!fechaReferencia.HasValue)
                        {
                            fechaReferencia = shellFile.Properties.System.DateModified.Value;
                        }

                        // Si después de todo no tenemos ninguna fecha, no puede pasar el filtro
                        if (!fechaReferencia.HasValue) return false;

                        DateTime fecha = fechaReferencia.Value.Date;

                        // Comprobamos si está en el rango
                        if (filtros.FechaImagenDesde.HasValue && fecha < filtros.FechaImagenDesde.Value.Date)
                            return false;

                        if (filtros.FechaImagenHasta.HasValue && fecha > filtros.FechaImagenHasta.Value.Date)
                            return false;
                    }

                    if (filtros.FiltrarModelo)
                    {
                        string modeloReal = shellFile.Properties.System.Photo.CameraModel.Value ?? "";

                        // 1. Si la imagen no tiene metadato de modelo, queda descartada (el filtro pide que tenga modelo)
                        if (string.IsNullOrWhiteSpace(modeloReal)) return false;

                        bool tieneTextoCamara = !string.IsNullOrWhiteSpace(filtros.ModeloCamara);
                        bool tieneTextoMovil = !string.IsNullOrWhiteSpace(filtros.ModeloMovil);

                        // 2. Si el usuario escribió algo en los cuadros de búsqueda
                        if (tieneTextoCamara || tieneTextoMovil)
                        {
                            bool coincideCamara = false;
                            bool coincideMovil = false;

                            if (tieneTextoCamara)
                                coincideCamara = modeloReal.IndexOf(filtros.ModeloCamara, StringComparison.OrdinalIgnoreCase) >= 0;

                            if (tieneTextoMovil)
                                coincideMovil = modeloReal.IndexOf(filtros.ModeloMovil, StringComparison.OrdinalIgnoreCase) >= 0;

                            // Si no coincide con ninguna de las búsquedas del usuario, se oculta
                            if (!coincideCamara && !coincideMovil) return false;
                        }

                        // 3. Si tieneTextoCamara y tieneTextoMovil son falsos (están vacíos), 
                        // la imagen pasa el filtro simplemente por tener metadato (que es lo que ya hace el punto 1).
                    }

                    if (filtros.FiltrarGPS)
                    {
                        // Accedemos mediante el "Canonical Name" como string para saltarnos la falta de definición en la clase estática
                        var propLat = shellFile.Properties.GetProperty("System.GPS.LatitudeDecimal");
                        var propLon = shellFile.Properties.GetProperty("System.GPS.LongitudeDecimal");

                        // Obtenemos los valores como object y los convertimos a double?
                        double? latVal = propLat?.ValueAsObject as double?;
                        double? lonVal = propLon?.ValueAsObject as double?;

                        if (latVal == null || lonVal == null) return false;

                        // Comparación con margen de error
                        if (filtros.Latitud.HasValue && Math.Abs(latVal.Value - filtros.Latitud.Value) > 0.001) return false;
                        if (filtros.Longitud.HasValue && Math.Abs(lonVal.Value - filtros.Longitud.Value) > 0.001) return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        //===================== FIN CUMPLE FILTROS DE METADATOS DE IMÁGENES =====================
        private bool SeguridadActiva(string nombre)
        {
            if (filtros.Seguridad == null) return false;
            return filtros.Seguridad.TryGetValue(nombre, out bool act) && act;
        }
        // Cerrar completamente la aplicación
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit();
        }

        private void txtBoxBuscador_TextChanged(object sender, EventArgs e)
        {
            // Llamamos a CargarCarpetas pasando el texto actual del buscador.
            // Esto filtrará la vista actual basándose en el nombre y los filtros de ClaseFiltros.
            CargarCarpetas(rutaActual, txtBoxBuscador.Text);
        }
    }
}