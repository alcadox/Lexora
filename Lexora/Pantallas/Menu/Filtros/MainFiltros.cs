using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lexora.Pantallas.Menu.Filtros
{
    public partial class MainFiltros : Form
    {

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
            foreach (var item in filtros.TiposArchivo)
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
            /* ========== APLICAR FILTROS TIPO ARCHIVO ==========
            / 1. Limpiar los filtros actuales para evitar acumular filtros antiguos
            / 2. Agregar el tipo de archivo seleccionado al diccionario de filtros
            / se guarda: < "documento" , true > por ejemplo
            */

            filtros.TiposArchivo.Clear(); // 1. Limpiar los filtros actuales (limpia el diccionario)
            foreach (var item in checkedListBoxTipoArchivo.CheckedItems)
            {
                // 2. Agregar el tipo de archivo seleccionado al diccionario de filtros
                // se guarda: < "documento" , true > por ejemplo
                filtros.TiposArchivo[item.ToString()] = true;
            }


        }

        private void botonCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }



    }
}
