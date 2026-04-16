using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Lexora.Core
{
    public class GestorMenuDiscos
    {
        private ContextMenuStrip _menuDisco;
        private Action<string> _navegarADisco; // Acción para que MainForm cambie de directorio

        public GestorMenuDiscos(Action<string> navegarADisco)
        {
            _navegarADisco = navegarADisco;
            ConstruirMenu();
        }

        private void ConstruirMenu()
        {
            _menuDisco = new ContextMenuStrip();
            _menuDisco.BackColor = Color.FromArgb(245, 245, 255);
            _menuDisco.ShowImageMargin = false;
            _menuDisco.Font = new Font("Segoe UI", 9.5F);

            _menuDisco.Items.Add("📂 Explorar Unidad", null, (s, e) => EjecutarAccion(Explorar));
            _menuDisco.Items.Add(new ToolStripSeparator());

            var itemDefender = new ToolStripMenuItem("🛡️ Escanear con Windows Defender");
            itemDefender.ForeColor = Color.DarkGreen;
            itemDefender.Click += (s, e) => EjecutarAccion(EscanearDefender);
            _menuDisco.Items.Add(itemDefender);

            var itemFormatear = new ToolStripMenuItem("⚠️ Formatear Volumen...");
            itemFormatear.ForeColor = Color.DarkRed;
            itemFormatear.Click += (s, e) => EjecutarAccion(FormatearUnidad);
            _menuDisco.Items.Add(itemFormatear);

            _menuDisco.Items.Add(new ToolStripSeparator());
            _menuDisco.Items.Add("ℹ️ Propiedades", null, (s, e) => EjecutarAccion(MostrarPropiedades));
        }

        // Método público para "enganchar" este menú a las tarjetas de disco
        public void AdjuntarATarjeta(Control tarjeta, string letraDisco)
        {
            // Asignamos el tag para saber a qué disco pertenece
            tarjeta.Tag = letraDisco;

            // Escuchamos el click derecho en la tarjeta
            tarjeta.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    _menuDisco.Tag = letraDisco; // Pasamos la letra al menú
                    _menuDisco.Show(tarjeta, e.Location);
                }
            };

            // También se lo añadimos a los controles hijos (iconos, labels) por si el usuario hace click ahí
            foreach (Control hijo in tarjeta.Controls)
            {
                hijo.MouseUp += (s, e) =>
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        _menuDisco.Tag = letraDisco;
                        _menuDisco.Show(hijo, e.Location);
                    }
                };
            }
        }

        private void EjecutarAccion(Action<string> accion)
        {
            if (_menuDisco.Tag != null)
            {
                string letra = _menuDisco.Tag.ToString();
                try { accion(letra); }
                catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Lexora Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        // --- LÓGICA DE HERRAMIENTAS ---

        private void Explorar(string letra)
        {
            _navegarADisco?.Invoke(letra);
        }

        private void MostrarPropiedades(string letra)
        {
            ArchivosUtil.MostrarPropiedadesWindows(letra);
        }

        private void EscanearDefender(string letra)
        {
            // Llama a la herramienta de línea de comandos de Windows Defender oculta en el sistema
            string pathDefender = @"C:\Program Files\Windows Defender\MpCmdRun.exe";
            if (File.Exists(pathDefender))
            {
                MessageBox.Show($"Se abrirá una consola para escanear '{letra}' en busca de Rootkits/Malware. Esto puede tardar varios minutos dependiendo del tamaño del volumen.", "Análisis de Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Process.Start(new ProcessStartInfo
                {
                    FileName = pathDefender,
                    Arguments = $"-Scan -ScanType 3 -File {letra}",
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("Windows Defender no parece estar instalado o accesible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatearUnidad(string letra)
        {
            // LLamada nativa a la ventana de formateo de Windows (Seguro, porque Windows previene formatear C:)
            Process.Start(new ProcessStartInfo
            {
                FileName = "rundll32.exe",
                Arguments = $"shell32.dll,SHFormatDrive",
                UseShellExecute = true
            });
            MessageBox.Show($"Se ha abierto la utilidad de formateo. Asegúrate de seleccionar la letra ({letra}) correcta.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}