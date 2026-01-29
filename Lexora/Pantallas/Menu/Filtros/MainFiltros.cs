using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Lexora.Pantallas.Menu.Filtros
{
    public partial class MainFiltros : Form
    {

        public event EventHandler FiltrosAplicados; // evento para notificar que se aplicaron los filtros

        ClaseFiltros filtros;

        public MainFiltros(ClaseFiltros filtrosTraidos)
        {
            InitializeComponent();
            filtros = filtrosTraidos;
            cargarFiltros();
        }

        // ========== CARGAR FILTROS ==========
        private void cargarFiltros()
        {
            botonAplicar.Enabled = false; // Desactivar botón aplicar al cargar filtros

            // Recorrer el diccionario de TiposArchivoSinFormatear y marcar los ítems correspondientes en el CheckedListBox
            foreach (var item in filtros.TiposArchivoSinFormatear)
            {
                // Si el valor es false, no marcar
                if (!item.Value) continue;

                // Buscar el índice del ítem en el CheckedListBox
                int index = checkedListBoxTipoArchivo.Items.IndexOf(item.Key);

                // Marcar el ítem si se encuentra
                if (index >= 0) checkedListBoxTipoArchivo.SetItemChecked(index, true);
            }
        }

        // ========== GUARDAR FILTROS ==========
        private void botonAplicar_Click(object sender, EventArgs e)
        {
            botonAplicar.Enabled = false; // Desactivar botón aplicar al guardar filtros

            /* ========== APLICAR FILTROS TIPO ARCHIVO ==========
            / 1. Limpiar los filtros actuales para evitar acumular filtros antiguos
            / 2. Agregar el tipo de archivo seleccionado al diccionario de filtros
            / se guarda: < "documento" , true > por ejemplo
            */

            filtros.TiposArchivo.Clear(); // 1. Limpiar los filtros actuales (limpia el diccionario)
            filtros.TiposArchivoSinFormatear.Clear(); // Limpiar también el diccionario sin formatear

            foreach (var item in checkedListBoxTipoArchivo.CheckedItems)
            {
                // 2. Agregar el tipo de archivo seleccionado al diccionario de filtros
                // se guarda: < "documento" , true > por ejemplo
                string nombreFiltro = item.ToString();

                // Guardamos en ambos para mantener consistencia
                filtros.TiposArchivo[nombreFiltro] = true;
                filtros.TiposArchivoSinFormatear[nombreFiltro] = true;
            }

            filtros.FormatearTipoArchivo(); // Formatear los tipos de archivo para que estén en el formato correcto

            // Disparar el evento para notificar que los filtros han sido aplicados
            FiltrosAplicados?.Invoke(this, EventArgs.Empty);
        }

        private void botonCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkedListBoxTipoArchivo_ItemCheck(object sender, ItemCheckEventArgs e)
        {

            if (botonAplicar.Enabled == false)
            {
                botonAplicar.Enabled = true; // Activar botón aplicar al cambiar cualquier ítem
            }
        }
    }
}
