using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            _menu.BackColor = Color.FromArgb(250, 245, 255); // Tonos Lexora
            _menu.ShowImageMargin = false;
            _menu.Font = new Font("Segoe UI", 9.5F);

            // 1. Opciones Básicas
            _menu.Items.Add("Abrir", null, (s, e) => EjecutarAccion(Abrir));
            _menu.Items.Add("Ejecutar como Administrador", null, (s, e) => EjecutarAccion(EjecutarComoAdmin));
            _menu.Items.Add(new ToolStripSeparator());

            // 2. Operaciones de Portapapeles
            _menu.Items.Add("Copiar", null, (s, e) => EjecutarAccion(Copiar));
            _menu.Items.Add("Copiar Ruta de Acceso", null, (s, e) => EjecutarAccion(CopiarRuta));
            _menu.Items.Add(new ToolStripSeparator());

            // 3. Modificaciones
            _menu.Items.Add("Renombrar", null, (s, e) => EjecutarAccion(Renombrar, false));
            _menu.Items.Add("Eliminar", null, (s, e) => EjecutarAccion(Eliminar));
            _menu.Items.Add(new ToolStripSeparator());

            // 4. OPCIONES EXCLUSIVAS DE LEXORA
            var itemIA = new ToolStripMenuItem("✨ Analizar con Lexora IA");
            itemIA.ForeColor = Color.FromArgb(142, 108, 253);
            itemIA.Click += (s, e) => EjecutarAccion(AnalizarConIA);
            _menu.Items.Add(itemIA);

            var itemWipe = new ToolStripMenuItem("🔥 Destrucción Segura (Wipe)");
            itemWipe.ForeColor = Color.Red;
            itemWipe.Click += (s, e) => EjecutarAccion(DestruccionSegura);
            _menu.Items.Add(itemWipe);

            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add("Propiedades", null, (s, e) => EjecutarAccion(MostrarPropiedades));
        }

        private void ListView_MouseClick(object sender, MouseEventArgs e)
        {
            // Cambia a MouseButtons.Left si estrictamente lo deseas en el clic izquierdo
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

        // --- ENRUTADOR DE ACCIONES CON MANEJO DE ERRORES CENTRALIZADO ---
        private void EjecutarAccion(Action<string, bool> accion, bool refrescarDespues = true)
        {
            if (_listView.SelectedItems.Count == 0) return;

            string nombreSeleccion = _listView.SelectedItems[0].Text;
            bool esCarpeta = _listView.SelectedItems[0].SubItems[1].Text == "Carpeta";
            string rutaCompleta = Path.Combine(_obtenerRutaActual(), nombreSeleccion);

            try
            {
                accion(rutaCompleta, esCarpeta);

                // SOLO refrescamos si la acción no es de edición inmediata (como renombrar)
                if (refrescarDespues)
                {
                    _notificarCambio?.Invoke();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Lexora Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- IMPLEMENTACIÓN DE LÓGICAS ---

        private void Abrir(string ruta, bool esCarpeta)
        {
            if (esCarpeta)
            {
                // Para carpetas, emulamos un doble click.
                // Lo ideal sería disparar un evento que MainForm escuche para actualizar la vista.
                Process.Start("explorer.exe", $"\"{ruta}\""); // Alternativa rápida
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
                // Uso de la API Nativa de Windows - Sin dependencias de Visual Basic
                ArchivosUtil.EnviarAPapelera(ruta);
            }
        }

        private void Renombrar(string ruta, bool esCarpeta)
        {
            // Activa la edición en línea del ListView
            _listView.LabelEdit = true;
            _listView.SelectedItems[0].BeginEdit();
        }

        private void MostrarPropiedades(string ruta, bool esCarpeta)
        {
            // Muestra el cuadro de diálogo nativo de propiedades de Windows
            ArchivosUtil.MostrarPropiedadesWindows(ruta);
        }

        // --- FUNCIONES EXCLUSIVAS DE LEXORA ---

        private void DestruccionSegura(string ruta, bool esCarpeta)
        {
            if (esCarpeta) { MessageBox.Show("Por seguridad, el Wipe solo se aplica a archivos individuales por ahora."); return; }

            if (MessageBox.Show("ESTA ACCIÓN ES IRREVERSIBLE. El archivo será sobrescrito con ceros para evitar su recuperación por forense digital. ¿Continuar?", "⚠️ Destrucción Segura", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                byte[] buffer = new byte[4096]; // Sobrescribir con 0s
                using (FileStream fs = new FileStream(ruta, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    for (long i = 0; i < fs.Length; i += buffer.Length)
                    {
                        fs.Write(buffer, 0, (int)Math.Min(buffer.Length, fs.Length - i));
                    }
                }
                File.Delete(ruta);
                MessageBox.Show("Archivo triturado exitosamente.", "Seguridad Lexora", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AnalizarConIA(string ruta, bool esCarpeta)
        {
            MessageBox.Show($"Llamando al motor IA para analizar: {Path.GetFileName(ruta)}...\n(Implementación pendiente en el módulo de IA)", "Lexora AI", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}