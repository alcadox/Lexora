using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Lexora.Pantallas.Menu.Filtros
{
    public partial class MainFiltros : Form
    {
        private string filtroFechaSeleccionado = null;

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

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        //=================FILTRO FECHA=====================

        // Evento para manejar el cambio de selección en el CheckedListBox de tipos de fecha
        private void checkedListBoxTiposfecha_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBoxTiposfecha.SelectedItem == null) return;

            filtroFechaSeleccionado = checkedListBoxTiposfecha.SelectedItem.ToString();
            lblInfoFecha.Text = "Seleccionando fecha para: " + filtroFechaSeleccionado;
        }

        // Evento para manejar el clic en el botón "Aceptar" para guardar el filtro de fecha
        private void btnAceptarFecha_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filtroFechaSeleccionado))
            {
                MessageBox.Show("Selecciona primero un tipo de filtro de fecha.");
                return;
            }

            DateTime desde = monthCalendarFecha.SelectionRange.Start.Date;
            DateTime hasta = monthCalendarFecha.SelectionRange.End.Date;

            // Aquí SOLO lo dejamos guardado “en memoria” por ahora:
            // (Luego lo conectamos con ClaseFiltros)
            MessageBox.Show($"Guardado para '{filtroFechaSeleccionado}': {desde:dd/MM/yyyy} - {hasta:dd/MM/yyyy}");

            botonAplicar.Enabled = true; // ya hay cambios para aplicar
        }

        //BOTÓN LIMPIAR FECHA
        private void btnLimpiarFecha_Click(object sender, EventArgs e)
        {
            filtroFechaSeleccionado = null;
            lblInfoFecha.Text = "Selecciona un filtro de fecha a la izquierda.";
            botonAplicar.Enabled = true;
        }






    }
}
