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
              
                if (!item.Value) continue;  // Si el valor es false, no marcar
                
                int index = checkedListBoxTipoArchivo.Items.IndexOf(item.Key); // Buscar el índice del ítem en el CheckedListBo
                
                if (index >= 0) checkedListBoxTipoArchivo.SetItemChecked(index, true); // Marcar el ítem si se encuentra
            }

        // BLOQUE DE FILTRO DE FECHA
            //inicializo elfiltro de fecha
            monthCalendarFecha.Enabled = false;
            buttonAceptarFecha.Enabled = false;
            buttonLimpiarFecha.Enabled = false;
            lblInfoFecha.Text = "Selecciona un filtro de fecha a la izquierda.";


            // Si ya hay fechas guardadas, avisamos
            if (filtros.Fechas != null && filtros.Fechas.Count > 0)
            {
                lblInfoFecha.Text = "Hay filtros de fecha guardados. Selecciona uno para ver/editar.";
            }
        //

        //BLOQUE para precargar los metadatos por documetnos:
            checkBoxFiltroAutorDoc.Checked = filtros.FiltrarAutor;
            textBoxAutorDoc.Text = filtros.AutorDocumento ?? "";

            checkBoxFiltroAutorDoc.Checked = filtros.FiltrarTitulo;
            textBoxTituloDoc.Text = filtros.TituloDocumento ?? "";

            checkBoxFiltroAppQLoGenero.Checked = filtros.FiltrarAplicacion;
            textBoxAppQGeneroDoc.Text = filtros.AplicacionGeneradora ?? "";

            checkBoxFiltroCantPaginas.Checked = filtros.FiltrarPaginas;
            textBoxNumPag.Text = filtros.CantidadPaginas?.ToString() ?? "0";



            //en el caso de que no haya filtros de fecha guardados, se bloquean los checkboxes de fecha para que no confunda al usuario

            // ===== bloquear/desbloquear inputs según checkboxes (metadatos documentos) =====
            textBoxAutorDoc.Enabled = checkBoxFiltroAutorDoc.Checked;
            textBoxTituloDoc.Enabled = checkBoxFiltroTituloDoc.Checked;
            textBoxAppQGeneroDoc.Enabled = checkBoxFiltroAppQLoGenero.Checked;

            textBoxNumPag.Enabled = checkBoxFiltroCantPaginas.Checked;
            buttonSumarPagina.Enabled = checkBoxFiltroCantPaginas.Checked;
            buttonRestarPagina.Enabled = checkBoxFiltroCantPaginas.Checked;
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
            



            // ======== GUARDAR LOS METADATOS DOCUMENTOS ==========
            filtros.FiltrarAutor = checkBoxFiltroAutorDoc.Checked;
            filtros.AutorDocumento = (textBoxAutorDoc.Text ?? "").Trim();

            filtros.FiltrarTitulo = checkBoxFiltroTituloDoc.Checked;
            filtros.TituloDocumento = (textBoxTituloDoc.Text ?? "").Trim();

            filtros.FiltrarAplicacion = checkBoxFiltroAppQLoGenero.Checked;
            filtros.AplicacionGeneradora = (textBoxAppQGeneroDoc.Text ?? "").Trim();

            filtros.FiltrarPaginas = checkBoxFiltroCantPaginas.Checked;

            // parse seguro de páginas para que ni no es número, sea null
            if(filtros.FiltrarPaginas && int.TryParse(textBoxNumPag.Text, out int n) && n >= 0)
            {
                filtros.CantidadPaginas = n;
            }
            else
            {
                filtros.CantidadPaginas = null;
            }
        }






        //Para que los textfields estén desactivados hasta que se active su checkbox correspondiente,

        private void textBoxAutorDoc_TextChanged(object sender, EventArgs e){ botonAplicar.Enabled = true;}

        private void textBoxTituloDoc_TextChanged(object sender, EventArgs e){ botonAplicar.Enabled = true; }
        private void textBoxAppQGeneroDoc_TextChanged(object sender, EventArgs e){botonAplicar.Enabled = true;}


     //Para que los textfields estén desactivados hasta que se active su checkbox correspondiente,

        //autor
        private void checkBoxFiltroAutorDoc_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAutorDoc.Enabled = checkBoxFiltroAutorDoc.Checked;

            // opcional: si lo desmarcas, vacías el texto para evitar confusiones
            if (!checkBoxFiltroAutorDoc.Checked)
                textBoxAutorDoc.Text = "";

            botonAplicar.Enabled = true;
        }
        //título
        private void checkBoxFiltroTituloDoc_CheckedChanged(object sender, EventArgs e)
        {
            textBoxTituloDoc.Enabled = checkBoxFiltroTituloDoc.Checked;

            if (!checkBoxFiltroTituloDoc.Checked)
                textBoxTituloDoc.Text = "";

            botonAplicar.Enabled = true;
        }
        //app que lo generó
        private void checkBoxFiltroAppQLoGenero_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAppQGeneroDoc.Enabled = checkBoxFiltroAppQLoGenero.Checked;

            if (!checkBoxFiltroAppQLoGenero.Checked)
                textBoxAppQGeneroDoc.Text = "";

            botonAplicar.Enabled = true;
        }
        //cantidad de páginas
        private void checkBoxFiltroCantPaginas_CheckedChanged(object sender, EventArgs e)
        {
            bool activo = checkBoxFiltroCantPaginas.Checked;

            textBoxNumPag.Enabled = activo;
            buttonSumarPagina.Enabled = activo;
            buttonRestarPagina.Enabled = activo;

            if (!activo)
                textBoxNumPag.Text = "0"; // o "" si prefieres

            botonAplicar.Enabled = true;
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
            // Ya no se usa pq el flujo correcto va por ItemCheck
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

            // Guardar el rango de fechas
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
                string nombre = checkedListBoxTiposfecha.Items[e.Index].ToString();

                //CORRECTION DEL MÉTODO: si se desmarca se borra de la calse filtros 
                if (e.NewValue == CheckState.Unchecked)
                {
                    filtros.Fechas.Remove(nombre);
                    lblInfoFecha.Text = $"Quitado: {nombre}";
                    botonAplicar.Enabled = true;
                    return;
                }
                // Si marca -> abrir calendario (tu código actual)
                filtroFechaSeleccionado = nombre;
                checkedListBoxTiposfecha.Enabled = false;
                monthCalendarFecha.Enabled = true;
                buttonAceptarFecha.Enabled = true;
                buttonLimpiarFecha.Enabled = true;

                lblInfoFecha.Text = "Selecciona rango para: " + filtroFechaSeleccionado;

                if (filtros.Fechas.TryGetValue(filtroFechaSeleccionado, out var rango) &&
                    rango.Desde.HasValue && rango.Hasta.HasValue)
                {
                    monthCalendarFecha.SelectionRange = new SelectionRange(rango.Desde.Value, rango.Hasta.Value);
                }
            }));
        }

        

        private void buttonSumarPagina_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBoxNumPag.Text, out int n)) n = 0;
            n++;
            textBoxNumPag.Text = n.ToString();
            botonAplicar.Enabled = true;
        }

        private void buttonRestarPagina_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBoxNumPag.Text, out int n)) n = 0;
            if (n > 0) n--;
            textBoxNumPag.Text = n.ToString();
            botonAplicar.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e){}
        private void label1_Click_1(object sender, EventArgs e) {}

    }
}
