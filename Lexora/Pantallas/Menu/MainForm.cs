using Lexora.Core; // Importación obligatoria de nuestra nueva arquitectura
using Lexora.Pantallas.Menu.Filtros;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Lexora
{
    public partial class MainForm : Form
    {
        string rutaActual = "";
        ClaseFiltros filtros = new ClaseFiltros();
        MotorFiltros motorFiltros; // Instancia de nuestro motor refactorizado
        string nombreUsuario = "";
        GestorMenuContextual menuContextual;

        private ImageList listaIconos;
        private FlowLayoutPanel panelDiscosDinamicos;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txtDireccion;

        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        public MainForm()
        {
            InitializeComponent();
            ConfigurarInterfaz();
        }

        public MainForm(string nombreUsuario)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
            ConfigurarInterfaz();
        }

        private void ConfigurarInterfaz()
        {
            // Inicializar Motores
            motorFiltros = new MotorFiltros(filtros);

            // INSTANCIAR EL GESTOR DE MENÚ
            menuContextual = new GestorMenuContextual(
                listViewArchivos,
                () => rutaActual,
                () => CargarCarpetas(rutaActual, txtBoxBuscador.Text) //refresh automático
            );

            // 3. SUSCRIPCIÓN AL EVENTO DE EDICIÓN 
            listViewArchivos.AfterLabelEdit += ListViewArchivos_AfterLabelEdit;

            AjustarColumnas();
            listViewArchivos.Resize += (s, e) => AjustarColumnas();

            listViewArchivos.OwnerDraw = true;
            listViewArchivos.DrawColumnHeader += ListView_DrawColumnHeader;
            listViewArchivos.DrawItem += ListView_DrawItem;
            listViewArchivos.DrawSubItem += ListView_DrawSubItem;

            typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(listViewArchivos, true, null);

            listaIconos = new ImageList();
            listaIconos.ColorDepth = ColorDepth.Depth32Bit;
            listaIconos.ImageSize = new Size(20, 20);
            listViewArchivos.SmallImageList = listaIconos;

            listaIconos.Images.Add("folder_default", Properties.Resources.carpeta_por_defecto_w11);
            listaIconos.Images.Add("folder_default_back", Properties.Resources.carpeta_back_por_defecto_w11);

            CargarDiscosDinamicos();
        }

        private void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (SolidBrush pincelFondo = new SolidBrush(Color.FromArgb(245, 235, 255)))
            using (SolidBrush pincelTexto = new SolidBrush(Color.Black))
            {
                e.Graphics.FillRectangle(pincelFondo, e.Bounds);
                using (StringFormat formato = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
                {
                    Rectangle limitesTexto = e.Bounds;
                    limitesTexto.X += 5;
                    e.Graphics.DrawString(e.Header.Text, new Font("Segoe UI", 10, FontStyle.Regular), pincelTexto, limitesTexto, formato);
                }
            }
        }

        private void ListView_DrawItem(object sender, DrawListViewItemEventArgs e) { e.DrawDefault = true; }
        private void ListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e) { e.DrawDefault = true; }

        private void AplicarFiltrosTipoArchivo() => CargarCarpetas(rutaActual);

        private void CargarCarpetas(string ruta, string textoBusqueda = "")
        {
            try
            {
                listViewArchivos.BeginUpdate();
                listViewArchivos.Items.Clear();

                string busqueda = (textoBusqueda ?? "").ToLower().Trim();

                if (Directory.GetParent(ruta) != null)
                {
                    ListViewItem volver = new ListViewItem("..");
                    volver.SubItems.Add("Carpeta");
                    volver.SubItems.Add("");
                    volver.SubItems.Add("");
                    volver.ImageKey = "folder_default_back";
                    listViewArchivos.Items.Add(volver);
                }

                var extensionesPermitidas = new HashSet<string>(filtros.TiposArchivo.Where(x => x.Value).Select(x => x.Key.ToLower()), StringComparer.OrdinalIgnoreCase);

                bool tieneFiltrosActivos = extensionesPermitidas.Count > 0;
                bool tieneFiltrosFechaActivos = filtros.Fechas != null && filtros.Fechas.Any(f => f.Value.Desde.HasValue && f.Value.Hasta.HasValue);
                bool tieneFiltrosMetadatosActivos = filtros.FiltrarAutor || filtros.FiltrarTitulo || filtros.FiltrarAplicacion || filtros.FiltrarPaginas;
                bool tieneFiltrosSeguridadActivos = filtros.Seguridad != null && filtros.Seguridad.Any(kv => kv.Value);
                bool tieneFiltrosMetadatosImagenesActivos = filtros.FiltrarResolucion || filtros.FiltrarFechaImagen || filtros.FiltrarModelo || filtros.FiltrarGPS;

                bool hayAlgunFiltroActivo = tieneFiltrosActivos || tieneFiltrosFechaActivos || tieneFiltrosMetadatosActivos || tieneFiltrosSeguridadActivos || tieneFiltrosMetadatosImagenesActivos;

                string[] carpetasSeguras = new string[0];
                try { carpetasSeguras = Directory.GetDirectories(ruta); } catch { }

                foreach (var carpeta in carpetasSeguras)
                {
                    try
                    {
                        DirectoryInfo info = new DirectoryInfo(carpeta);
                        if (info.Attributes.HasFlag(FileAttributes.ReparsePoint) && info.Attributes.HasFlag(FileAttributes.System)) continue;
                        if (!string.IsNullOrEmpty(busqueda) && !info.Name.ToLower().Contains(busqueda)) continue;

                        bool cumpleFechaCarpeta = motorFiltros.CumpleFiltrosFechaCarpeta(info);
                        if (hayAlgunFiltroActivo && !cumpleFechaCarpeta && !motorFiltros.CarpetaTieneArchivosQueCumplen(carpeta, extensionesPermitidas, tieneFiltrosActivos)) continue;

                        ListViewItem item = new ListViewItem(info.Name);
                        item.SubItems.Add("Carpeta");
                        item.SubItems.Add("");
                        item.SubItems.Add(info.CreationTime.ToString("dd/MM/yyyy HH:mm"));
                        item.ImageKey = "folder_default";
                        listViewArchivos.Items.Add(item);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (PathTooLongException) { }
                }

                string[] archivosSeguros = new string[0];
                try { archivosSeguros = Directory.GetFiles(ruta); } catch { }

                foreach (var archivo in archivosSeguros)
                {
                    try
                    {
                        FileInfo info = new FileInfo(archivo);
                        if (!string.IsNullOrEmpty(busqueda) && !info.Name.ToLower().Contains(busqueda)) continue;
                        if (!motorFiltros.CumpleFiltroTamano(archivo)) continue;
                        if (!motorFiltros.CumpleFiltroContenido(archivo)) continue;

                        if ((!tieneFiltrosActivos || extensionesPermitidas.Contains(info.Extension.ToLower())) &&
                            motorFiltros.CumpleFiltrosSeguridad(info) &&
                            motorFiltros.CumpleFiltrosFecha(info) &&
                            motorFiltros.CumpleFiltrosMetadatosDocumento(archivo) &&
                            motorFiltros.CumpleFiltrosMetadatosImagen(archivo))
                        {
                            ListViewItem item = new ListViewItem(info.Name);
                            item.SubItems.Add(info.Extension + " (Archivo)");
                            item.SubItems.Add(ArchivosUtil.FormatearTamaño(info.Length));
                            item.SubItems.Add(info.CreationTime.ToString("dd/MM/yyyy HH:mm"));

                            string ext = info.Extension.ToLowerInvariant();
                            string claveIcono = (ext == ".lnk" || ext == ".exe" || ext == ".ico") ? info.FullName : ext;

                            if (!string.IsNullOrEmpty(claveIcono) && !listaIconos.Images.ContainsKey(claveIcono))
                            {
                                try { var iconoExtraido = System.Drawing.Icon.ExtractAssociatedIcon(archivo); if (iconoExtraido != null) listaIconos.Images.Add(claveIcono, iconoExtraido); } catch { }
                            }

                            if (listaIconos.Images.ContainsKey(claveIcono)) item.ImageKey = claveIcono;
                            listViewArchivos.Items.Add(item);
                        }
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (PathTooLongException) { }
                }
                ActualizarBreadcrumbs();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Carpeta protegida por SYSTEM/TrustedInstaller.", "Acceso Restringido", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                var padre = Directory.GetParent(rutaActual);
                if (padre != null) { rutaActual = padre.FullName; CargarCarpetas(rutaActual); }
            }
            finally { listViewArchivos.EndUpdate(); }
        }

        private void listViewArchivos_DoubleClick(object sender, EventArgs e)
        {
            if (listViewArchivos.SelectedItems.Count == 0) return;

            ListViewItem itemSeleccionado = listViewArchivos.SelectedItems[0];
            string nombreSeleccion = itemSeleccionado.Text;

            // Caso especial: Subir de nivel (Carpeta padre)
            if (nombreSeleccion == "..")
            {
                rutaActual = Directory.GetParent(rutaActual).FullName;
                txtBoxBuscador.Text = "";
                CargarCarpetas(rutaActual);
                return;
            }

            string nuevaRuta = Path.Combine(rutaActual, nombreSeleccion);
            bool esCarpeta = itemSeleccionado.SubItems[1].Text == "Carpeta";

            if (esCarpeta)
            {
                // Si es carpeta, entramos en ella
                if (Directory.Exists(nuevaRuta))
                {
                    rutaActual = nuevaRuta;
                    txtBoxBuscador.Text = "";
                    CargarCarpetas(rutaActual);
                }
            }
            else
            {
                // SI ES UN ARCHIVO: Lo abrimos con la aplicación predeterminada del sistema
                try
                {
                    Process.Start(new ProcessStartInfo(nuevaRuta) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"No se pudo abrir el archivo: {ex.Message}", "Error en Lexora", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSesion_Click(object sender, EventArgs e)
        {
            Hide();
            using (InicioSesion ventanaLogin = new InicioSesion(true))
            {
                if (ventanaLogin.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(ventanaLogin.NombreUsuario))
                {
                    btnSesion.Text = "Bienvenido, " + ventanaLogin.NombreUsuario;
                    btnSesion.Enabled = false;
                }
                Show();
            }
        }

        private void btnFiltros_Click(object sender, EventArgs e)
        {
            MainFiltros ventanaFiltros = new MainFiltros(filtros);
            ventanaFiltros.FiltrosAplicados += (s, args) => AplicarFiltrosTipoArchivo();
            ventanaFiltros.ShowDialog();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit();
        }

        private void txtBoxBuscador_TextChanged(object sender, EventArgs e) => CargarCarpetas(rutaActual, txtBoxBuscador.Text);

        private void ActualizarBreadcrumbs()
        {
            if (txtDireccion == null)
            {
                pnlBreadcrumbs.Controls.Clear();
                pnlBreadcrumbs.Height = 32;
                pnlBreadcrumbs.Location = new Point(10, panelArchivos.ClientSize.Height - pnlBreadcrumbs.Height - 10);
                listViewArchivos.Height = pnlBreadcrumbs.Top - listViewArchivos.Top - 5;

                pnlBreadcrumbs.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                listViewArchivos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                txtDireccion = new Siticone.Desktop.UI.WinForms.SiticoneTextBox
                {
                    Width = pnlBreadcrumbs.Width - 5,
                    Height = 30,
                    BorderRadius = 4,
                    FillColor = Color.FromArgb(245, 238, 255),
                    BorderColor = Color.FromArgb(187, 167, 255),
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                    Margin = new Padding(0),
                    PlaceholderText = "Escribe o pega una ruta...",
                    Cursor = Cursors.IBeam
                };
                txtDireccion.HoverState.BorderColor = Color.FromArgb(142, 108, 253);
                txtDireccion.FocusedState.BorderColor = Color.FromArgb(142, 108, 253);
                txtDireccion.KeyDown += TxtDireccion_KeyDown;
                pnlBreadcrumbs.Controls.Add(txtDireccion);
            }
            txtDireccion.Text = rutaActual;
            txtDireccion.SelectionStart = txtDireccion.Text.Length;
        }

        private void TxtDireccion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string rutaEscrita = txtDireccion.Text.Trim();

                if (Directory.Exists(rutaEscrita)) { rutaActual = rutaEscrita; txtBoxBuscador.Text = ""; CargarCarpetas(rutaActual); }
                else
                {
                    MessageBox.Show("Ruta no válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDireccion.Text = rutaActual; txtDireccion.SelectionStart = txtDireccion.Text.Length;
                }
            }
        }

        private void AjustarColumnas()
        {
            if (listViewArchivos.Columns.Count >= 4)
            {
                int anchoOcupado = listViewArchivos.Columns[1].Width + listViewArchivos.Columns[2].Width + listViewArchivos.Columns[3].Width;
                int anchoLibre = listViewArchivos.ClientSize.Width - anchoOcupado - 20;
                if (anchoLibre > 100) listViewArchivos.Columns[0].Width = anchoLibre;
            }
        }

        private void CargarDiscosDinamicos()
        {
            if (panelDiscosDinamicos == null)
            {
                siticonePanel2.Controls.Clear();
                panelDiscosDinamicos = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, Padding = new Padding(12, 15, 0, 10) };
                siticonePanel2.Controls.Add(panelDiscosDinamicos);
            }

            panelDiscosDinamicos.SuspendLayout();
            panelDiscosDinamicos.Controls.Clear();

            foreach (DriveInfo disco in DriveInfo.GetDrives())
            {
                // Uso ultra limpio de nuestro nuevo componente UIBuilder
                if (disco.IsReady) panelDiscosDinamicos.Controls.Add(UIBuilder.ConstruirTarjetaDisco(disco, TarjetaDisco_Click));
            }
            panelDiscosDinamicos.ResumeLayout();

            if (string.IsNullOrEmpty(rutaActual))
            {
                var discoSistema = DriveInfo.GetDrives().FirstOrDefault(d => d.IsReady && d.RootDirectory.FullName == Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)));
                if (discoSistema != null) { rutaActual = discoSistema.RootDirectory.FullName; CargarCarpetas(rutaActual); }
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_DEVICECHANGE && ((int)m.WParam == DBT_DEVICEARRIVAL || (int)m.WParam == DBT_DEVICEREMOVECOMPLETE)) CargarDiscosDinamicos();
        }

        private void TarjetaDisco_Click(object sender, EventArgs e)
        {
            if (sender is Control control && control.Tag != null)
            {
                string rutaSeleccionada = control.Tag.ToString();
                if (rutaActual != rutaSeleccionada) { rutaActual = rutaSeleccionada; txtBoxBuscador.Text = ""; CargarCarpetas(rutaActual); }
            }
        }

        private void ListViewArchivos_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == null) return; // Si el usuario canceló la edición

            string nombreViejo = listViewArchivos.Items[e.Item].Text;
            string nombreNuevo = e.Label;
            string rutaVieja = Path.Combine(rutaActual, nombreViejo);
            string rutaNueva = Path.Combine(rutaActual, nombreNuevo);

            try
            {
                if (listViewArchivos.Items[e.Item].SubItems[1].Text == "Carpeta")
                    Directory.Move(rutaVieja, rutaNueva);
                else
                    File.Move(rutaVieja, rutaNueva);

                // Forzamos el refresco para que los iconos y filtros se reapliquen correctamente
                // Usamos BeginInvoke para asegurar que la edición del Label termine antes de recargar
                this.BeginInvoke(new Action(() => CargarCarpetas(rutaActual, txtBoxBuscador.Text)));

            }
            catch (Exception ex)
            {
                e.CancelEdit = true;
                MessageBox.Show($"No se pudo renombrar: {ex.Message}", "Error Lexora", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}