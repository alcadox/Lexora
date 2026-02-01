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
        private bool bloqueandoChecksFecha = false;
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

            // Inicializar el filtro de fecha
            monthCalendarFecha.Enabled = false;
            buttonAceptarFecha.Enabled = false;
            buttonLimpiarFecha.Enabled = false;
            lblInfoFecha.Text = "Selecciona un filtro de fecha a la izquierda.";


            // Si ya hay fechas guardadas, avisamos (sin abrir el calendario)
            if (filtros.Fechas != null && filtros.Fechas.Count > 0)
            {
                lblInfoFecha.Text = "Hay filtros de fecha guardados. Selecciona uno para ver/editar.";
            }


        }

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

        //ANTERIOR MÉTODO DE SELECCIÓN DE FILTRO FECHA
        private void checkedListBoxTiposfecha_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ya no se usa. El flujo correcto va por ItemCheck.

            /*
            if (checkedListBoxTiposfecha.SelectedItem == null) return;

            // 1) Guardamos cuál filtro estamos editando
            filtroFechaSeleccionado = checkedListBoxTiposfecha.SelectedItem.ToString();

            // 2) Bloqueamos el checklist para que no cambien a otro mientras eligen fecha
            checkedListBoxTiposfecha.Enabled = false;

            // 3) Habilitamos calendario y botones
            monthCalendarFecha.Enabled = true;
            buttonAceptarFecha.Enabled = true;
            buttonLimpiarFecha.Enabled = true;

            lblInfoFecha.Text = "Selecciona rango para: " + filtroFechaSeleccionado;

            // 4) Si ya había fecha guardada para ese filtro, la precargamos en el calendario
            if (filtros.Fechas.TryGetValue(filtroFechaSeleccionado, out var rango) &&
                rango.Desde.HasValue && rango.Hasta.HasValue)
            {
                monthCalendarFecha.SelectionRange = new SelectionRange(rango.Desde.Value, rango.Hasta.Value);
            }*/
        }

        //BOTÓN ACEPTAR FECHA
        private void buttonAceptarFecha_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(filtroFechaSeleccionado))
            {
                MessageBox.Show("Selecciona primero un tipo de filtro de fecha.");
                return;
            }

            DateTime desde = monthCalendarFecha.SelectionRange.Start.Date;
            DateTime hasta = monthCalendarFecha.SelectionRange.End.Date;

            // Guardar el rango para ESE filtro
            filtros.Fechas[filtroFechaSeleccionado] = (desde, hasta);

            // Desbloquear UI para poder elegir otro filtro
            monthCalendarFecha.Enabled = false;
            buttonAceptarFecha.Enabled = false;
            buttonLimpiarFecha.Enabled = false;

            checkedListBoxTiposfecha.Enabled = true;

            // Info y habilitar aplicar
            lblInfoFecha.Text = $"Guardado: {filtroFechaSeleccionado} ({desde:dd/MM/yyyy} - {hasta:dd/MM/yyyy})";
            filtroFechaSeleccionado = null;

            botonAplicar.Enabled = true; 
        }

        //BOTÓN LIMPIAR FECHA
        private void buttonLimpiarFecha_Click(object sender, EventArgs e)
        {
            // Si estábamos editando un filtro, borramos su rango
            if (!string.IsNullOrEmpty(filtroFechaSeleccionado))
                filtros.Fechas.Remove(filtroFechaSeleccionado);

            // Reset UI
            monthCalendarFecha.Enabled = false;
            buttonAceptarFecha.Enabled = false;
            buttonLimpiarFecha.Enabled = false;

            checkedListBoxTiposfecha.ClearSelected();
            checkedListBoxTiposfecha.Enabled = true;

            lblInfoFecha.Text = "Selecciona un filtro de fecha a la izquierda.";
            filtroFechaSeleccionado = null;
            botonAplicar.Enabled = true;
        }

        //cuando marco o desmarco un ítem del checklist de tipos de fecha
        private void checkedListBoxTiposfecha_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                // Si lo están desmarcando, no abrimos el calendario
                if (e.NewValue != CheckState.Checked) return;

                filtroFechaSeleccionado = checkedListBoxTiposfecha.Items[e.Index].ToString();

                // Bloqueamos mientras editas ESTE filtro
                checkedListBoxTiposfecha.Enabled = false;

                monthCalendarFecha.Enabled = true;
                buttonAceptarFecha.Enabled = true;
                buttonLimpiarFecha.Enabled = true;

                lblInfoFecha.Text = "Selecciona rango para: " + filtroFechaSeleccionado;

                // Precargar rango si ya existe
                if (filtros.Fechas.TryGetValue(filtroFechaSeleccionado, out var rango) &&
                    rango.Desde.HasValue && rango.Hasta.HasValue)
                {
                    monthCalendarFecha.SelectionRange = new SelectionRange(rango.Desde.Value, rango.Hasta.Value);
                }
            }));
        }













    }
}
