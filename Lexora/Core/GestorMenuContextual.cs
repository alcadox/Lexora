using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lexora.Core
{
    public class GestorMenuContextual
    {
        private ContextMenuStrip _menuItem;  // Menú cuando tocas un archivo
        private ContextMenuStrip _menuFondo; // Menú cuando tocas el fondo vacío
        private ListView _listView;
        private Func<string> _obtenerRutaActual;
        private Action _notificarCambio;

        public GestorMenuContextual(ListView listView, Func<string> obtenerRutaActual, Action notificarCambio)
        {
            _listView = listView;
            _obtenerRutaActual = obtenerRutaActual;
            _notificarCambio = notificarCambio;

            ConstruirMenuItem();
            ConstruirMenuFondo(); // Construimos el nuevo menú de fondo
            _listView.MouseUp += ListView_MouseUp;
        }

        // --- 1. MENÚ PARA ARCHIVOS/CARPETAS ---
        private void ConstruirMenuItem()
        {
            _menuItem = new ContextMenuStrip();
            _menuItem.BackColor = Color.FromArgb(250, 245, 255);
            _menuItem.ShowImageMargin = false;
            _menuItem.Font = new Font("Segoe UI", 9.5F);

            // Emojis añadidos a las opciones básicas
            _menuItem.Items.Add("📂 Abrir", null, (s, e) => EjecutarAccionItem(Abrir));
            _menuItem.Items.Add("🚀 Ejecutar como Administrador", null, (s, e) => EjecutarAccionItem(EjecutarComoAdmin));
            _menuItem.Items.Add(new ToolStripSeparator());

            var menuSeguridad = new ToolStripMenuItem("🛡️ Seguridad Lexora");
            menuSeguridad.ForeColor = Color.DarkSlateBlue;
            menuSeguridad.DropDownItems.Add("🔒 Cifrar/Descifrar (AES-256)", null, async (s, e) => await EjecutarAccionItemAsync(CifrarDescifrar));
            menuSeguridad.DropDownItems.Add("🛑 Bloquear/Desbloquear Bóveda", null, (s, e) => EjecutarAccionItem(AlternarBoveda));
            menuSeguridad.DropDownItems.Add("👻 Ocultación Fuerte (Nivel OS)", null, (s, e) => EjecutarAccionItem(AlternarOcultacion));
            menuSeguridad.DropDownItems.Add("🦠 Escanear Malware en VirusTotal", null, (s, e) => EjecutarAccionItem(EscanearMalware));
            var itemAniquilarMeta = new ToolStripMenuItem("🥷 Aniquilar Metadatos (Anti-Forense)");
            itemAniquilarMeta.ForeColor = Color.DarkOrange;
            itemAniquilarMeta.Click += async (s, e) => await EjecutarAccionItemAsync(AniquilarMetadatosApp);
            menuSeguridad.DropDownItems.Add(itemAniquilarMeta);
            menuSeguridad.DropDownItems.Add(new ToolStripSeparator());
            var itemWipeDoD = new ToolStripMenuItem("🔥 Wipe DoD 5220.22-M (3 Pasadas)");
            itemWipeDoD.ForeColor = Color.Red;
            itemWipeDoD.Click += async (s, e) => await EjecutarAccionItemAsync(DestruccionSeguraDoD);
            menuSeguridad.DropDownItems.Add(itemWipeDoD);

            _menuItem.Items.Add(menuSeguridad);
            _menuItem.Items.Add(new ToolStripSeparator());

            // Emojis añadidos a portapapeles y edición
            _menuItem.Items.Add("📋 Copiar", null, (s, e) => EjecutarAccionItem(Copiar));
            _menuItem.Items.Add("🔗 Copiar Ruta de Acceso", null, (s, e) => EjecutarAccionItem(CopiarRuta));
            _menuItem.Items.Add("✏️ Renombrar", null, (s, e) => EjecutarAccionItem(Renombrar, false));
            _menuItem.Items.Add("🗑️ Eliminar", null, (s, e) => EjecutarAccionItem(Eliminar));
            _menuItem.Items.Add(new ToolStripSeparator());

            var itemIA = new ToolStripMenuItem("✨ Analizar con Lexora IA");
            itemIA.ForeColor = Color.FromArgb(142, 108, 253);
            itemIA.Click += (s, e) => EjecutarAccionItem(AnalizarConIA);
            _menuItem.Items.Add(itemIA);

            _menuItem.Items.Add(new ToolStripSeparator());
            _menuItem.Items.Add("ℹ️ Propiedades", null, (s, e) => EjecutarAccionItem(MostrarPropiedades));
        }

        // --- 2. MENÚ PARA EL FONDO VACÍO ---
        private void ConstruirMenuFondo()
        {
            _menuFondo = new ContextMenuStrip();
            _menuFondo.BackColor = Color.FromArgb(245, 250, 255); // Tono ligeramente distinto para diferenciar
            _menuFondo.ShowImageMargin = false;
            _menuFondo.Font = new Font("Segoe UI", 9.5F);

            _menuFondo.Items.Add("🔄 Actualizar", null, (s, e) => _notificarCambio?.Invoke());
            _menuFondo.Items.Add(new ToolStripSeparator());

            // Submenú NUEVO
            var menuNuevo = new ToolStripMenuItem("Nuevo");
            menuNuevo.DropDownItems.Add("📁 Carpeta", null, (s, e) => CrearNuevo("Carpeta"));
            menuNuevo.DropDownItems.Add("📄 Archivo de Texto (.txt)", null, (s, e) => CrearNuevo("Texto"));
            _menuFondo.Items.Add(menuNuevo);

            _menuFondo.Items.Add("📋 Pegar", null, async (s, e) => await PegarDesdePortapapeles());
            _menuFondo.Items.Add(new ToolStripSeparator());

            _menuFondo.Items.Add("💻 Abrir Terminal Aquí", null, (s, e) => AbrirTerminal());
            _menuFondo.Items.Add("📂 Copiar Ruta Actual", null, (s, e) => Clipboard.SetText(_obtenerRutaActual()));
        }

        // --- DETECCIÓN DE CLICS INTELIGENTE ---
        private void ListView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTest = _listView.HitTest(e.Location);

                if (hitTest.Item != null)
                {
                    hitTest.Item.Selected = true;
                    _menuItem.Show(_listView, e.Location); // Menú de archivo
                }
                else
                {
                    _menuFondo.Show(_listView, e.Location); // Menú de fondo vacío
                }
            }
        }

        // --- LÓGICA DEL MENÚ DE FONDO ---

        private void CrearNuevo(string tipo)
        {
            string rutaActual = _obtenerRutaActual();
            string nombre = Microsoft.VisualBasic.Interaction.InputBox($"Introduce el nombre para el nuevo {tipo}:", "Nuevo", "Nuevo Elemento");

            if (string.IsNullOrWhiteSpace(nombre)) return;

            try
            {
                if (tipo == "Carpeta")
                {
                    Directory.CreateDirectory(Path.Combine(rutaActual, nombre));
                }
                else if (tipo == "Texto")
                {
                    string rutaArchivo = Path.Combine(rutaActual, nombre.EndsWith(".txt") ? nombre : nombre + ".txt");
                    File.Create(rutaArchivo).Close(); // Crea y libera el archivo
                }
                _notificarCambio?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear: {ex.Message}", "Error Lexora", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task PegarDesdePortapapeles()
        {
            if (!Clipboard.ContainsFileDropList()) return;

            string destino = _obtenerRutaActual();
            var archivos = Clipboard.GetFileDropList();

            await Task.Run(() =>
            {
                foreach (string rutaOrigen in archivos)
                {
                    string nombre = Path.GetFileName(rutaOrigen);
                    string rutaDestino = Path.Combine(destino, nombre);

                    try
                    {
                        if (Directory.Exists(rutaOrigen)) // Es carpeta
                        {
                            // Requiere copiar recursivamente (API de VB.NET es perfecta y segura para esto)
                            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(rutaOrigen, rutaDestino, Microsoft.VisualBasic.FileIO.UIOption.AllDialogs);
                        }
                        else if (File.Exists(rutaOrigen)) // Es archivo
                        {
                            File.Copy(rutaOrigen, rutaDestino, false);
                        }
                    }
                    catch (Exception ex) { LogError(ex.Message); }
                }
            });

            _notificarCambio?.Invoke();
        }

        private void AbrirTerminal()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                WorkingDirectory = _obtenerRutaActual(),
                UseShellExecute = true
            });
        }

        private void LogError(string msg) { /* Placeholder para tu futuro sistema de logs */ }

        // =======================================================================
        // LÓGICA DE LOS ENRUTADORES ITEM (SÍNCRONO Y ASÍNCRONO)
        // =======================================================================

        private void EjecutarAccionItem(Action<string, bool> accion, bool refrescarDespues = true)
        {
            if (_listView.SelectedItems.Count == 0) return;
            string nombreSeleccion = _listView.SelectedItems[0].Text;
            bool esCarpeta = _listView.SelectedItems[0].SubItems[1].Text == "Carpeta";
            string rutaCompleta = Path.Combine(_obtenerRutaActual(), nombreSeleccion);

            try
            {
                accion(rutaCompleta, esCarpeta);
                if (refrescarDespues) _notificarCambio?.Invoke();
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Lexora Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async Task EjecutarAccionItemAsync(Func<string, bool, Task> accion, bool refrescarDespues = true)
        {
            if (_listView.SelectedItems.Count == 0) return;
            string nombreSeleccion = _listView.SelectedItems[0].Text;
            bool esCarpeta = _listView.SelectedItems[0].SubItems[1].Text == "Carpeta";
            string rutaCompleta = Path.Combine(_obtenerRutaActual(), nombreSeleccion);

            try
            {
                await accion(rutaCompleta, esCarpeta);
                if (refrescarDespues) _notificarCambio?.Invoke();
            }
            catch (Exception ex) { MessageBox.Show($"Error crítico: {ex.Message}", "Lexora Security", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // métodos: Abrir, Copiar, Eliminar, Cifrar, etc.
        private void Abrir(string r, bool e) { if (e) Process.Start("explorer.exe", $"\"{r}\""); else Process.Start(new ProcessStartInfo(r) { UseShellExecute = true }); }
        private void EjecutarComoAdmin(string r, bool c) { if (c) return; Process.Start(new ProcessStartInfo(r) { UseShellExecute = true, Verb = "runas" }); }
        private void Copiar(string r, bool c) { Clipboard.SetFileDropList(new StringCollection { r }); }
        private void CopiarRuta(string r, bool c) { Clipboard.SetText(r); }
        private void Eliminar(string r, bool c) { if (MessageBox.Show("¿Enviar a papelera?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes) ArchivosUtil.EnviarAPapelera(r); }
        private void Renombrar(string r, bool c) { _listView.LabelEdit = true; _listView.SelectedItems[0].BeginEdit(); }
        private void MostrarPropiedades(string r, bool c) { ArchivosUtil.MostrarPropiedadesWindows(r); }

        // Métodos Seguridad
        private async Task CifrarDescifrar(string r, bool c)
        {
            if (c) return;
            string pwd = Microsoft.VisualBasic.Interaction.InputBox("Contraseña:", "Seguridad Lexora", "");
            if (string.IsNullOrEmpty(pwd)) return;
            await Task.Run(() => { if (r.EndsWith(".lxr")) SeguridadAvanzadaUtil.DescifrarArchivo(r, pwd); else SeguridadAvanzadaUtil.CifrarArchivo(r, pwd); });
            MessageBox.Show("Operación exitosa.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void AlternarBoveda(string r, bool c) { if (c) SeguridadAvanzadaUtil.AlternarBloqueoCarpeta(r); }
        private void AlternarOcultacion(string r, bool c) { SeguridadAvanzadaUtil.AlternarOcultacionFuerte(r); }
        private void EscanearMalware(string r, bool c) { if (!c) SeguridadAvanzadaUtil.EscanearEnVirusTotal(r); }
        private async Task DestruccionSeguraDoD(string r, bool c)
        {
            string advertencia = c
                ? "Vas a TRITURAR esta carpeta Y TODO SU CONTENIDO RECURSIVAMENTE. No quedará ni rastro en el disco duro."
                : "Vas a TRITURAR este archivo de forma irreversible.";

            if (MessageBox.Show($"DESTRUCCIÓN MILITAR DoD 5220.22-M\n\n{advertencia}\n\nEl sistema sobrescribirá los datos físicos 3 veces (Ceros, Unos y Datos Aleatorios) y destruirá los nombres originales.\n\nNi con software forense profesional se podrán recuperar.\n\n¿Estás absolutamente seguro?", "Aniquilación Total", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                await SeguridadAvanzadaUtil.DestruccionDoD(r, c);
                MessageBox.Show("Operación militar completada. Los datos han dejado de existir.", "Lexora Security", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void AnalizarConIA(string r, bool c) { MessageBox.Show("Pendiente IA", "Lexora"); }

        private async Task AniquilarMetadatosApp(string r, bool c)
        {
            string advertencia = c
                ? "Vas a aniquilar esta carpeta Y TODO SU CONTENIDO RECURSIVAMENTE (archivos y subcarpetas)."
                : "Vas a aniquilar este archivo.";

            if (MessageBox.Show($"ACCIÓN DESTRUCTIVA MASIVA\n\n{advertencia}\n\nCambiará los nombres a hashes ininteligibles, borrará datos de origen, inyectará ruido binario y falsificará las fechas a los años 90.\n\n¿Estás completamente seguro de proceder con la aniquilación total?", "Seguridad Anti-Forense", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                await SeguridadAvanzadaUtil.AniquilarMetadatos(r, c);
                MessageBox.Show("Rastros digitales eliminados. Los elementos ahora son 'fantasmas'.", "Lexora Security", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}