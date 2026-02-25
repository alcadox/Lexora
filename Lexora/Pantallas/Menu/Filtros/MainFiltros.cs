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

            // ===== ENGANCHE FORZADO DE EVENTOS (para que no dependa del Designer) =====
            checkBoxFiltroAutorDoc.CheckedChanged += checkBoxFiltroAutorDoc_CheckedChanged;
            checkBoxFiltroTituloDoc.CheckedChanged += checkBoxFiltroTituloDoc_CheckedChanged;
            checkBoxFiltroAppQLoGenero.CheckedChanged += checkBoxFiltroAppQLoGenero_CheckedChanged;
            checkBoxFiltroCantPaginas.CheckedChanged += checkBoxFiltroCantPaginas_CheckedChanged;

            textBoxAutorDoc.TextChanged += textBoxAutorDoc_TextChanged;
            textBoxTituloDoc.TextChanged += textBoxTituloDoc_TextChanged;
            textBoxAppQGeneroDoc.TextChanged += textBoxAppQGeneroDoc_TextChanged;
            textBoxNumPag.TextChanged += textBoxNumPag_TextChanged; // crea este método si no existe

            buttonSumarPagina.Click += buttonSumarPagina_Click;
            buttonRestarPagina.Click += buttonRestarPagina_Click;

            // === Eventos para Metadatos de Imágenes ===
            checkedListBoxFiltrosMetadatosImagenes.ItemCheck += CheckedListBoxFiltrosMetadatosImagenes_ItemCheck;

            buttonIzquierdaPixelesMetadatosImagenesAncho.Click += (s, e) => ModificarValorNumerico(textBoxResolucionImagenAnchura, -100);
            buttonDerechaPixelesMetadatosImagenesAncho.Click += (s, e) => ModificarValorNumerico(textBoxResolucionImagenAnchura, 100);

            buttonIzquierdaPixelesMetadatosImagenesAlto.Click += (s, e) => ModificarValorNumerico(textBoxResolucionImagenAltura, -100);
            buttonDerechaPixelesMetadatosImagenesAlto.Click += (s, e) => ModificarValorNumerico(textBoxResolucionImagenAltura, 100);

            buttonDisminuirLatitud.Click += (s, e) => ModificarValorDecimal(textBoxLatitud, -0.1);
            buttonAumentarLatitud.Click += (s, e) => ModificarValorDecimal(textBoxLatitud, 0.1);
            buttonDisminuirLongitud.Click += (s, e) => ModificarValorDecimal(textBoxLongitud, -0.1);
            buttonAumentarLongitud.Click += (s, e) => ModificarValorDecimal(textBoxLongitud, 0.1);

            CargarValoresMetadatosImagenes();

            // ================================================================

            cargarFiltros();
            ActualizarEstadoMetadatosUI(); // <- IMPORTANTÍSIMO para que arranque bien
            CargarSugerenciasModelos();
        }

        private void CargarSugerenciasModelos()
        {
            // --- TOP CÁMARAS (Más famosas y comunes) ---
            string[] camaras = {
                "Canon EOS R5", "Canon EOS 5D Mark IV", "Canon EOS 90D", "Canon PowerShot",
                "Nikon Z7 II", "Nikon D850", "Nikon D3500", "Nikon Coolpix",
                "Sony A7 III", "Sony A7R V", "Sony Alpha a6400", "Sony ZV-E10",
                "Fujifilm X-T4", "Fujifilm X100V", "Panasonic Lumix GH6",
                "Olympus OM-D", "Leica M11", "GoPro HERO12", "GoPro HERO11", "DJI Osmo Action"
            };

            // --- TOP MÓVILES (Los más usados que suelen aparecer en metadatos) ---
            // Nota: En metadatos suelen salir como "iPhone 13 Pro" o "SM-G991B" (Samsung)
            string[] moviles = {
                "iPhone 15 Pro Max", "iPhone 15", "iPhone 14", "iPhone 13", "iPhone 12", "iPhone 11", "iPhone SE",
                "Samsung Galaxy S24 Ultra", "Samsung Galaxy S23", "Samsung Galaxy S22", "Samsung Galaxy S21",
                "Samsung Galaxy A54", "Samsung Galaxy A34", "Samsung Galaxy Z Fold5",
                "Google Pixel 8 Pro", "Google Pixel 7", "Google Pixel 6a",
                "Xiaomi 13 Ultra", "Xiaomi Redmi Note 12", "Xiaomi POCO F5",
                "Huawei P50 Pro", "OPPO Reno10", "OnePlus 11", "Motorola Edge 40",
                "Realme GT", "Sony Xperia 1 V"
            };

            // Configurar ComboBox Cámara
            comboBoxModelosCamara.Items.Clear();
            comboBoxModelosCamara.Items.AddRange(camaras);
            comboBoxModelosCamara.DropDownStyle = ComboBoxStyle.DropDown; // PERMITE ESCRIBIR
            comboBoxModelosCamara.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBoxModelosCamara.AutoCompleteSource = AutoCompleteSource.ListItems;

            // Configurar ComboBox Móvil
            comboBoxModelosMoviles.Items.Clear();
            comboBoxModelosMoviles.Items.AddRange(moviles);
            comboBoxModelosMoviles.DropDownStyle = ComboBoxStyle.DropDown; // PERMITE ESCRIBIR
            comboBoxModelosMoviles.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBoxModelosMoviles.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void ActualizarEstadoMetadatosUI()
        {
            // autor
            textBoxAutorDoc.Enabled = checkBoxFiltroAutorDoc.Checked;

            // título
            textBoxTituloDoc.Enabled = checkBoxFiltroTituloDoc.Checked;

            // app
            textBoxAppQGeneroDoc.Enabled = checkBoxFiltroAppQLoGenero.Checked;

            // páginas
            bool activoPag = checkBoxFiltroCantPaginas.Checked;
            textBoxNumPag.Enabled = activoPag;
            buttonSumarPagina.Enabled = activoPag;
            buttonRestarPagina.Enabled = activoPag;
        }

        private void textBoxNumPag_TextChanged(object sender, EventArgs e)
        {
            botonAplicar.Enabled = true;
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
         // ===== precargar filtros metadatos documentos =====

            // autor
            checkBoxFiltroAutorDoc.Checked = filtros.FiltrarAutor;
            textBoxAutorDoc.Text = filtros.AutorDocumento ?? "";
            textBoxAutorDoc.Enabled = checkBoxFiltroAutorDoc.Checked;

            // título
            checkBoxFiltroTituloDoc.Checked = filtros.FiltrarTitulo;
            textBoxTituloDoc.Text = filtros.TituloDocumento ?? "";
            textBoxTituloDoc.Enabled = checkBoxFiltroTituloDoc.Checked;

            // app que lo generó
            checkBoxFiltroAppQLoGenero.Checked = filtros.FiltrarAplicacion;
            textBoxAppQGeneroDoc.Text = filtros.AplicacionGeneradora ?? "";
            textBoxAppQGeneroDoc.Enabled = checkBoxFiltroAppQLoGenero.Checked;

            // cantidad de páginas (+ y -)
            checkBoxFiltroCantPaginas.Checked = filtros.FiltrarPaginas;
            textBoxNumPag.Text = filtros.CantidadPaginas?.ToString() ?? "0";

            bool activoPag = checkBoxFiltroCantPaginas.Checked;
            textBoxNumPag.Enabled = activoPag;
            buttonSumarPagina.Enabled = activoPag;
            buttonRestarPagina.Enabled = activoPag;



            //en el caso de que no haya filtros de fecha guardados, se bloquean los checkboxes de fecha para que no confunda al usuario

            // ===== bloquear/desbloquear inputs según checkboxes (metadatos documentos) =====
            textBoxAutorDoc.Enabled = checkBoxFiltroAutorDoc.Checked;
            textBoxTituloDoc.Enabled = checkBoxFiltroTituloDoc.Checked;
            textBoxAppQGeneroDoc.Enabled = checkBoxFiltroAppQLoGenero.Checked;

            textBoxNumPag.Enabled = checkBoxFiltroCantPaginas.Checked;
            buttonSumarPagina.Enabled = checkBoxFiltroCantPaginas.Checked;
            buttonRestarPagina.Enabled = checkBoxFiltroCantPaginas.Checked;

            ActualizarEstadoMetadatosUI();

            // Precargar checklist seguridad
            foreach (var kv in filtros.Seguridad)
            {
                if (!kv.Value) continue;

                int idx = checkedListBoxSeguridad.Items.IndexOf(kv.Key);
                if (idx >= 0) checkedListBoxSeguridad.SetItemChecked(idx, true);
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


            // ====== SEGURIDAD ======
            filtros.Seguridad.Clear();
            foreach (var item in checkedListBoxSeguridad.CheckedItems)
            {
                string nombre = item.ToString();
                filtros.Seguridad[nombre] = true;
            }


            filtros.FormatearTipoArchivo(); // Formatear los tipos de archivo para que estén en el formato correcto

           

            // ======== GUARDAR LOS METADATOS DOCUMENTOS ==========
            filtros.FiltrarAutor = checkBoxFiltroAutorDoc.Checked;
            filtros.AutorDocumento = (textBoxAutorDoc.Text ?? "").Trim();

            filtros.FiltrarTitulo = checkBoxFiltroTituloDoc.Checked;
            filtros.TituloDocumento = (textBoxTituloDoc.Text ?? "").Trim();

            filtros.FiltrarAplicacion = checkBoxFiltroAppQLoGenero.Checked;
            filtros.AplicacionGeneradora = (textBoxAppQGeneroDoc.Text ?? "").Trim();

            filtros.FiltrarPaginas = checkBoxFiltroCantPaginas.Checked;

            // --- Guardar Filtros de Imágenes ---
            filtros.FiltrarResolucion = checkedListBoxFiltrosMetadatosImagenes.GetItemChecked(0); // 0 = Resolución
            if (filtros.FiltrarResolucion)
            {
                filtros.ResolucionAncho = int.TryParse(textBoxResolucionImagenAnchura.Text, out int ancho) ? ancho : (int?)null;
                filtros.ResolucionAlto = int.TryParse(textBoxResolucionImagenAltura.Text, out int alto) ? alto : (int?)null;
            }

            filtros.FiltrarFechaImagen = checkedListBoxFiltrosMetadatosImagenes.GetItemChecked(1); // 1 = Fecha
            if (filtros.FiltrarFechaImagen)
            {
                filtros.FechaImagenDesde = datePrincipioDeImagen.Value;
                filtros.FechaImagenHasta = dateFinDeImagen.Value;
            }

            filtros.FiltrarModelo = checkedListBoxFiltrosMetadatosImagenes.GetItemChecked(2); // 2 = Modelo
            if (filtros.FiltrarModelo)
            {
                filtros.ModeloCamara = comboBoxModelosCamara.Text;
                filtros.ModeloMovil = comboBoxModelosMoviles.Text;
            }

            filtros.FiltrarGPS = checkedListBoxFiltrosMetadatosImagenes.GetItemChecked(3); // 3 = GPS
            if (filtros.FiltrarGPS)
            {
                filtros.Latitud = double.TryParse(textBoxLatitud.Text, out double lat) ? lat : (double?)null;
                filtros.Longitud = double.TryParse(textBoxLongitud.Text, out double lon) ? lon : (double?)null;
            }

            // parse seguro de páginas para que ni no es número, sea null
            if (filtros.FiltrarPaginas && int.TryParse(textBoxNumPag.Text, out int n) && n >= 0)
            {
                filtros.CantidadPaginas = n;
            }
            else
            {
                filtros.CantidadPaginas = null;
            }

            // ====== SEGURIDAD ======
            filtros.Seguridad.Clear();

            foreach (var item in checkedListBoxSeguridad.CheckedItems)
            {
                string nombre = item.ToString();
                filtros.Seguridad[nombre] = true;
            }


            // Disparar el evento para notificar que los filtros han sido aplicados
            FiltrosAplicados?.Invoke(this, EventArgs.Empty);


        }

        //Para que los textfields estén desactivados hasta que se active su checkbox correspondiente,

        private void textBoxAutorDoc_TextChanged(object sender, EventArgs e){ botonAplicar.Enabled = true;}

        private void textBoxTituloDoc_TextChanged(object sender, EventArgs e){ botonAplicar.Enabled = true; }
        private void textBoxAppQGeneroDoc_TextChanged(object sender, EventArgs e){botonAplicar.Enabled = true;}


        //Para que los textfields estén desactivados hasta que se active su checkbox correspondiente,

        //autor
        private void checkBoxFiltroAutorDoc_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxFiltroAutorDoc.Checked) textBoxAutorDoc.Text = "";
            ActualizarEstadoMetadatosUI();
            botonAplicar.Enabled = true;
        }
        //título
        private void checkBoxFiltroTituloDoc_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxFiltroTituloDoc.Checked) textBoxTituloDoc.Text = "";
            ActualizarEstadoMetadatosUI();
            botonAplicar.Enabled = true;
        }
        //app que lo generó
        private void checkBoxFiltroAppQLoGenero_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxFiltroAppQLoGenero.Checked) textBoxAppQGeneroDoc.Text = "";
            ActualizarEstadoMetadatosUI();
            botonAplicar.Enabled = true;
        }
        //cantidad de páginas
        private void checkBoxFiltroCantPaginas_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxFiltroCantPaginas.Checked) textBoxNumPag.Text = "0";
            ActualizarEstadoMetadatosUI();
            botonAplicar.Enabled = true;
        }

        private void botonCerrar_Click(object sender, EventArgs e)
        {
            // si hay cambios pendientes, aplicarlos
            if (botonAplicar.Enabled)
                botonAplicar.PerformClick();
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

        private void checkedListBoxSeguridad_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            botonAplicar.Enabled = true;
        }



        private void textBox1_TextChanged(object sender, EventArgs e){}
        private void label1_Click_1(object sender, EventArgs e) {}

        private void buttonIzquierdaPixelesMetadatosImagenesAncho_Click(object sender, EventArgs e)
        {

        }

        private void buttonDerechaPixelesMetadatosImagenesAncho_Click(object sender, EventArgs e)
        {

        }

        private void buttonIzquierdaPixelesMetadatosImagenesAlto_Click(object sender, EventArgs e)
        {

        }

        private void buttonDerechaPixelesMetadatosImagenesAlto_Click(object sender, EventArgs e)
        {

        }

        private void buttonDisminuirLatitud_Click(object sender, EventArgs e)
        {

        }

        private void buttonAumentarLatitud_Click(object sender, EventArgs e)
        {

        }

        private void buttonDisminuirLongitud_Click(object sender, EventArgs e)
        {

        }

        private void buttonAumentarLongitud_Click(object sender, EventArgs e)
        {

        }

        private void CargarValoresMetadatosImagenes()
        {
            // Bloqueamos todo por defecto
            ActivarPanelResolucion(false);
            ActivarPanelFecha(false);
            ActivarPanelModelo(false);
            ActivarPanelGPS(false);

            // Cargamos si ya había filtros guardados (suponiendo el orden de los items en el CheckListBox)
            // Nota: Verifica que los índices 0, 1, 2, 3 correspondan a Resolución, Fecha, Modelo, GPS en tu diseño.
            if (filtros.FiltrarResolucion) { checkedListBoxFiltrosMetadatosImagenes.SetItemChecked(0, true); textBoxResolucionImagenAnchura.Text = filtros.ResolucionAncho?.ToString(); textBoxResolucionImagenAltura.Text = filtros.ResolucionAlto?.ToString(); }
            if (filtros.FiltrarFechaImagen) { checkedListBoxFiltrosMetadatosImagenes.SetItemChecked(1, true); if (filtros.FechaImagenDesde.HasValue) datePrincipioDeImagen.Value = filtros.FechaImagenDesde.Value; if (filtros.FechaImagenHasta.HasValue) dateFinDeImagen.Value = filtros.FechaImagenHasta.Value; }
            if (filtros.FiltrarModelo) { checkedListBoxFiltrosMetadatosImagenes.SetItemChecked(2, true); comboBoxModelosCamara.Text = filtros.ModeloCamara; comboBoxModelosMoviles.Text = filtros.ModeloMovil; }
            if (filtros.FiltrarGPS) { checkedListBoxFiltrosMetadatosImagenes.SetItemChecked(3, true); textBoxLatitud.Text = filtros.Latitud?.ToString(); textBoxLongitud.Text = filtros.Longitud?.ToString(); }
        }

        private void CheckedListBoxFiltrosMetadatosImagenes_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool estaMarcado = e.NewValue == CheckState.Checked;
            botonAplicar.Enabled = true;

            // Dependiendo de qué checkbox se marque, activamos su panel
            // Asumiendo: 0=Resolución, 1=Fecha, 2=Modelo, 3=GPS
            if (e.Index == 0) ActivarPanelResolucion(estaMarcado);
            else if (e.Index == 1) ActivarPanelFecha(estaMarcado);
            else if (e.Index == 2) ActivarPanelModelo(estaMarcado);
            else if (e.Index == 3) ActivarPanelGPS(estaMarcado);
        }

        // Métodos auxiliares para activar/desactivar y sumar/restar
        private void ActivarPanelResolucion(bool activo) { textBoxResolucionImagenAnchura.Enabled = activo; textBoxResolucionImagenAltura.Enabled = activo; buttonIzquierdaPixelesMetadatosImagenesAncho.Enabled = activo; buttonDerechaPixelesMetadatosImagenesAncho.Enabled = activo; buttonIzquierdaPixelesMetadatosImagenesAlto.Enabled = activo; buttonDerechaPixelesMetadatosImagenesAlto.Enabled = activo; }
        private void ActivarPanelFecha(bool activo) { datePrincipioDeImagen.Enabled = activo; dateFinDeImagen.Enabled = activo; }
        private void ActivarPanelModelo(bool activo) { comboBoxModelosCamara.Enabled = activo; comboBoxModelosMoviles.Enabled = activo; }
        private void ActivarPanelGPS(bool activo) { textBoxLatitud.Enabled = activo; textBoxLongitud.Enabled = activo; buttonDisminuirLatitud.Enabled = activo; buttonAumentarLatitud.Enabled = activo; buttonDisminuirLongitud.Enabled = activo; buttonAumentarLongitud.Enabled = activo; }

        private void ModificarValorNumerico(TextBox tb, int cantidad)
        {
            if (!int.TryParse(tb.Text, out int val)) val = 0;
            val += cantidad;
            if (val < 0) val = 0;
            tb.Text = val.ToString();
            botonAplicar.Enabled = true;
        }

        private void ModificarValorDecimal(TextBox tb, double cantidad)
        {
            if (!double.TryParse(tb.Text, out double val)) val = 0;
            val = Math.Round(val + cantidad, 4);
            tb.Text = val.ToString();
            botonAplicar.Enabled = true;
        }

        private void comboBoxModelosCamara_TextChanged(object sender, EventArgs e)
        {
            filtros.ModeloCamara = comboBoxModelosCamara.Text;
            botonAplicar.Enabled = true;
        }

        private void comboBoxModelosMoviles_TextChanged(object sender, EventArgs e)
        {
            filtros.ModeloMovil = comboBoxModelosMoviles.Text;
            botonAplicar.Enabled = true;
        }
    }
}
