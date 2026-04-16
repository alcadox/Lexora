using System;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;

namespace Lexora.Core
{
    public class GestorDragAndDrop
    {
        private ListView _listView;
        private Func<string> _obtenerRutaActual;
        private Action _notificarCambio;

        public GestorDragAndDrop(ListView listView, Func<string> obtenerRutaActual, Action notificarCambio)
        {
            _listView = listView;
            _obtenerRutaActual = obtenerRutaActual;
            _notificarCambio = notificarCambio;

            // Habilitar Drag & Drop en el control
            _listView.AllowDrop = true;

            // Suscribir eventos
            _listView.ItemDrag += ListView_ItemDrag;
            _listView.DragEnter += ListView_DragEnter;
            _listView.DragDrop += ListView_DragDrop;
        }

        // 1. CUANDO EL USUARIO ARRASTRA UN ARCHIVO DESDE LEXORA HACIA AFUERA
        private void ListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (_listView.SelectedItems.Count == 0) return;

            string[] archivos = new string[_listView.SelectedItems.Count];
            for (int i = 0; i < _listView.SelectedItems.Count; i++)
            {
                archivos[i] = Path.Combine(_obtenerRutaActual(), _listView.SelectedItems[i].Text);
            }

            // Inicia la operación de arrastre hacia el sistema (Windows Explorer, etc)
            _listView.DoDragDrop(new DataObject(DataFormats.FileDrop, archivos), DragDropEffects.Copy | DragDropEffects.Move);
            GestorLogs.Registrar("MOVER_ARCHIVO", $"Archivos arrastrados desde '{_obtenerRutaActual()}' hacia el sistema.");
        }

        // 2. CUANDO EL USUARIO MANTIENE ARCHIVOS SOBRE LA VENTANA DE LEXORA
        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            // Solo permitimos si lo que arrastran son archivos físicos
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Si pulsa CTRL es Copy, sino Move (comportamiento estándar de Windows)
                e.Effect = (e.KeyState & 8) == 8 ? DragDropEffects.Copy : DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        // 3. CUANDO EL USUARIO SUELTA LOS ARCHIVOS EN LEXORA
        private async void ListView_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            string[] archivosSoltados = (string[])e.Data.GetData(DataFormats.FileDrop);
            bool esCopia = (e.Effect == DragDropEffects.Copy);

            // Detectar si soltó SOBRE una carpeta específica o en el fondo vacío
            Point puntoCliente = _listView.PointToClient(new Point(e.X, e.Y));
            ListViewItem itemDestino = _listView.GetItemAt(puntoCliente.X, puntoCliente.Y);

            string rutaDestino = _obtenerRutaActual();

            if (itemDestino != null && itemDestino.SubItems[1].Text == "Carpeta")
            {
                // Soltó dentro de una subcarpeta mostrada en Lexora
                rutaDestino = Path.Combine(rutaDestino, itemDestino.Text);
            }

            // Usamos Task.Run para no congelar la app si arrastran 5GB de archivos
            await Task.Run(() =>
            {
                foreach (string rutaOrigen in archivosSoltados)
                {
                    string nombre = Path.GetFileName(rutaOrigen);
                    string destinoFinal = Path.Combine(rutaDestino, nombre);

                    // Evitar arrastrar sobre sí mismo
                    if (rutaOrigen.Equals(destinoFinal, StringComparison.OrdinalIgnoreCase)) continue;

                    try
                    {
                        if (Directory.Exists(rutaOrigen)) // Es carpeta
                        {
                            if (esCopia)
                                Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(rutaOrigen, destinoFinal, Microsoft.VisualBasic.FileIO.UIOption.AllDialogs);
                            else
                                Directory.Move(rutaOrigen, destinoFinal);

                            GestorLogs.Registrar("MOVER_CARPETA", $"Carpeta desplazada de '{rutaOrigen}' hacia '{rutaDestino}'.");
                        }
                        else if (File.Exists(rutaOrigen)) // Es archivo
                        {
                            if (esCopia)
                                File.Copy(rutaOrigen, destinoFinal, false);
                            else
                                File.Move(rutaOrigen, destinoFinal);

                            GestorLogs.Registrar("MOVER_ARCHIVO", $"Archivo desplazada de '{rutaOrigen}' hacia '{rutaDestino}'.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Evitamos MessageBox aquí dentro del Task para no bloquear
                        GestorLogs.Registrar("ERROR_MOVIMIENTO", $"Error al mover/copiar de '{rutaOrigen}' a '{rutaDestino}': {ex.Message}");
                    }
                }
            });

            _notificarCambio?.Invoke();
        }
    }
}