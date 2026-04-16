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
        private ContextMenuStrip _menu;
        private ListView _listView;
        private Func<string> _obtenerRutaActual;
        private Action _notificarCambio;

        // Constructor que inyecta dependencias
        public GestorMenuContextual(ListView listView, Func<string> obtenerRutaActual, Action notificarCambio)
        {
            _listView = listView;
            _obtenerRutaActual = obtenerRutaActual;
            _notificarCambio = notificarCambio; // Guardamos la referencia

            ConstruirMenu();
            _listView.MouseClick += ListView_MouseClick;
        }

        private void ConstruirMenu()
        {
            _menu = new ContextMenuStrip();
            _menu.BackColor = Color.FromArgb(250, 245, 255);
            _menu.ShowImageMargin = false;
            _menu.Font = new Font("Segoe UI", 9.5F);

            // --- OPCIONES BÁSICAS ---
            _menu.Items.Add("Abrir", null, (s, e) => EjecutarAccion(Abrir));
            // 👇 Añadimos de vuelta Ejecutar como Administrador
            _menu.Items.Add("Ejecutar como Administrador", null, (s, e) => EjecutarAccion(EjecutarComoAdmin));
            _menu.Items.Add(new ToolStripSeparator());

            // --- SEGURIDAD LEXORA ---
            var menuSeguridad = new ToolStripMenuItem("🛡️ Seguridad Lexora");
            menuSeguridad.ForeColor = Color.DarkSlateBlue;

            menuSeguridad.DropDownItems.Add("Cifrar/Descifrar (AES-256)", null, async (s, e) => await EjecutarAccionAsync(CifrarDescifrar));
            menuSeguridad.DropDownItems.Add("Bloquear/Desbloquear Bóveda", null, (s, e) => EjecutarAccion(AlternarBoveda));
            menuSeguridad.DropDownItems.Add("Ocultación Fuerte (Nivel OS)", null, (s, e) => EjecutarAccion(AlternarOcultacion));
            menuSeguridad.DropDownItems.Add("Escanear Malware en VirusTotal", null, (s, e) => EjecutarAccion(EscanearMalware));
            menuSeguridad.DropDownItems.Add(new ToolStripSeparator());

            var itemWipeDoD = new ToolStripMenuItem("🔥 Wipe DoD 5220.22-M (3 Pasadas)");
            itemWipeDoD.ForeColor = Color.Red;
            itemWipeDoD.Click += async (s, e) => await EjecutarAccionAsync(DestruccionSeguraDoD);
            menuSeguridad.DropDownItems.Add(itemWipeDoD);

            _menu.Items.Add(menuSeguridad);
            _menu.Items.Add(new ToolStripSeparator());

            // --- EDICIÓN Y PORTAPAPELES ---
            _menu.Items.Add("Copiar", null, (s, e) => EjecutarAccion(Copiar));
            // 👇 Añadimos de vuelta Copiar Ruta
            _menu.Items.Add("Copiar Ruta de Acceso", null, (s, e) => EjecutarAccion(CopiarRuta));
            _menu.Items.Add("Renombrar", null, (s, e) => EjecutarAccion(Renombrar, false));
            _menu.Items.Add("Eliminar", null, (s, e) => EjecutarAccion(Eliminar));
            _menu.Items.Add(new ToolStripSeparator());

            // --- IA Y PROPIEDADES ---
            var itemIA = new ToolStripMenuItem("✨ Analizar con Lexora IA");
            itemIA.ForeColor = Color.FromArgb(142, 108, 253);
            itemIA.Click += (s, e) => EjecutarAccion(AnalizarConIA);
            _menu.Items.Add(itemIA);

            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add("Propiedades", null, (s, e) => EjecutarAccion(MostrarPropiedades));
        }

        private void ListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTest = _listView.HitTest(e.Location);
                if (hitTest.Item != null)
                {
                    hitTest.Item.Selected = true; // Asegurar selección
                    _menu.Show(_listView, e.Location);
                }
            }
        }

        // --- ENRUTADOR 1: SÍNCRONO (Para acciones normales y rápidas) ---
        private void EjecutarAccion(Action<string, bool> accion, bool refrescarDespues = true)
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Lexora Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- ENRUTADOR 2: ASÍNCRONO (Para operaciones pesadas como el Wipe) ---
        private async Task EjecutarAccionAsync(Func<string, bool, Task> accion, bool refrescarDespues = true)
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error crítico: {ex.Message}", "Lexora Security", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- IMPLEMENTACIÓN DE LÓGICAS ---

        private void Abrir(string ruta, bool esCarpeta)
        {
            if (esCarpeta)
            {
                Process.Start("explorer.exe", $"\"{ruta}\"");
            }
            else
            {
                Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
            }
        }

        private void EjecutarComoAdmin(string ruta, bool esCarpeta)
        {
            if (esCarpeta) { MessageBox.Show("Solo aplicable a archivos ejecutables/scripts."); return; }
            Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true, Verb = "runas" });
        }

        private void Copiar(string ruta, bool esCarpeta)
        {
            StringCollection rutasList = new StringCollection { ruta };
            Clipboard.SetFileDropList(rutasList);
        }

        private void CopiarRuta(string ruta, bool esCarpeta)
        {
            Clipboard.SetText(ruta);
        }

        private void Eliminar(string ruta, bool esCarpeta)
        {
            if (MessageBox.Show($"¿Estás seguro de enviar '{Path.GetFileName(ruta)}' a la papelera?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ArchivosUtil.EnviarAPapelera(ruta);
            }
        }

        private void Renombrar(string ruta, bool esCarpeta)
        {
            _listView.LabelEdit = true;
            _listView.SelectedItems[0].BeginEdit();
        }

        private void MostrarPropiedades(string ruta, bool esCarpeta)
        {
            ArchivosUtil.MostrarPropiedadesWindows(ruta);
        }

        // --- FUNCIONES EXCLUSIVAS DE LEXORA ---

        private async Task DestruccionSegura(string ruta, bool esCarpeta)
        {
            if (esCarpeta) { MessageBox.Show("Solo archivos individuales."); return; }

            if (MessageBox.Show("¿Confirmar destrucción irreversible?", "⚠️ Alerta", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                // Ejecutamos en un hilo secundario para no bloquear la UI
                await Task.Run(() =>
                {
                    byte[] buffer = new byte[64 * 1024]; // Buffer de 64KB para mayor velocidad
                    using (FileStream fs = new FileStream(ruta, FileMode.Open, FileAccess.Write, FileShare.None))
                    {
                        long totalBytes = fs.Length;
                        long escritos = 0;
                        while (escritos < totalBytes)
                        {
                            int aEscribir = (int)Math.Min(buffer.Length, totalBytes - escritos);
                            fs.Write(buffer, 0, aEscribir);
                            escritos += aEscribir;
                        }
                        fs.Flush();
                    }
                    File.Delete(ruta);
                });

                MessageBox.Show("Archivo triturado con éxito.", "Lexora", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AnalizarConIA(string ruta, bool esCarpeta)
        {
            MessageBox.Show($"Llamando al motor IA para analizar: {Path.GetFileName(ruta)}...\n(Implementación pendiente en el módulo de IA)", "Lexora AI", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // --- NUEVOS ENRUTADORES DE SEGURIDAD ---

        private async Task CifrarDescifrar(string ruta, bool esCarpeta)
        {
            if (esCarpeta) { MessageBox.Show("Por ahora, el cifrado es solo para archivos."); return; }

            // Un simple InputBox improvisado usando forms de .NET para pedir la clave
            string promptText = ruta.EndsWith(".lxr") ? "Introduce contraseña para DESCIFRAR:" : "Introduce contraseña para CIFRAR:";
            string password = Microsoft.VisualBasic.Interaction.InputBox(promptText, "Seguridad Lexora", "");

            if (string.IsNullOrEmpty(password)) return;

            await Task.Run(() =>
            {
                if (ruta.EndsWith(".lxr"))
                    SeguridadAvanzadaUtil.DescifrarArchivo(ruta, password);
                else
                    SeguridadAvanzadaUtil.CifrarArchivo(ruta, password);
            });

            MessageBox.Show("Operación criptográfica completada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AlternarBoveda(string ruta, bool esCarpeta)
        {
            if (!esCarpeta) { MessageBox.Show("La bóveda solo se aplica a carpetas."); return; }
            SeguridadAvanzadaUtil.AlternarBloqueoCarpeta(ruta);
            MessageBox.Show("Permisos de la bóveda actualizados. Si la bloqueaste, Windows denegará el acceso.", "Bóveda Lexora", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AlternarOcultacion(string ruta, bool esCarpeta)
        {
            SeguridadAvanzadaUtil.AlternarOcultacionFuerte(ruta);
        }

        private void EscanearMalware(string ruta, bool esCarpeta)
        {
            if (esCarpeta) return;
            SeguridadAvanzadaUtil.EscanearEnVirusTotal(ruta);
        }

        private async Task DestruccionSeguraDoD(string ruta, bool esCarpeta)
        {
            if (esCarpeta) { MessageBox.Show("Solo archivos individuales."); return; }

            if (MessageBox.Show("ESTA ACCIÓN ES TOTALMENTE IRREVERSIBLE. Se aplicará el estándar militar DoD 5220.22-M (3 pasadas). ¿Continuar?", "⚠️ Destrucción Militar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                await SeguridadAvanzadaUtil.DestruccionDoD(ruta);
                MessageBox.Show("Archivo aniquilado con éxito. Es irrecuperable.", "Lexora Security", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }
}