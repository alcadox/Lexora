using Lexora.Core;
using Lexora.Pantallas.InicioSesion;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Lexora.Pantallas.Usuario
{
    public partial class PerfilUsuario : Form
    {
        private DatosUsuarioAuth usuario;
        private bool logsCargados = false; // Control para Lazy Loading de los logs

        public PerfilUsuario(DatosUsuarioAuth datosUsuario)
        {
            InitializeComponent();
            this.usuario = datosUsuario;

            CargarDatos();
            MostrarVista(panelVistaDatos);
        }

        private void CargarDatos()
        {
            txtNombre.Text = usuario.Nombre;
            txtEmail.Text = usuario.Email;
            lblPuntosIA.Text = $"{usuario.PuntosIA}/100";

            lblNombrePerfil.Text = usuario.Nombre;
            lblEmailPerfil.Text = usuario.Email;
        }

        // =================================================================
        // NAVEGACIÓN "SINGLE-PAGE" (DISEÑO POR PANELES)
        // =================================================================

        private void MostrarVista(Panel vistaSeleccionada)
        {
            // Ocultamos todo primero (comprobando que existan para que no dé error)
            if (panelVistaDatos != null) panelVistaDatos.Visible = false;
            if (panelVistaSeguridad != null) panelVistaSeguridad.Visible = false;
            if (panelVistaLogs != null) panelVistaLogs.Visible = false;

            // Mostramos la solicitada
            if (vistaSeleccionada != null)
            {
                vistaSeleccionada.Visible = true;
                vistaSeleccionada.BringToFront();
            }
        }

        // Eventos de los botones del menú lateral
        private void btnTabDatos_Click(object sender, EventArgs e)
        {
            MostrarVista(panelVistaDatos);
            lblTituloVista.Text = "Mis Datos Personales";
        }

        private void btnTabSeguridad_Click(object sender, EventArgs e)
        {
            MostrarVista(panelVistaSeguridad);
            lblTituloVista.Text = "Seguridad de la Cuenta";
        }

        private void btnTabLogs_Click(object sender, EventArgs e)
        {
            MostrarVista(panelVistaLogs);
            lblTituloVista.Text = "Registro de Actividad";
            if (!logsCargados) CargarLogsEnGrid(); // Carga perezosa
        }

        // BOTÓN GUARDAR CAMBIOS
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nuevoNombre = txtNombre.Text.Trim();
            string nuevoEmail = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(nuevoNombre) || string.IsNullOrEmpty(nuevoEmail))
            {
                MessageBox.Show("Los campos no pueden estar vacíos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (GestorDBAuth.ActualizarPerfil(usuario.IdUsuario, nuevoNombre, nuevoEmail))
                {
                    MessageBox.Show("Perfil actualizado con éxito.", "Lexora", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Como ahora es diseño "Single-Page", actualizamos los datos sin cerrar la ventana.
                    usuario.Nombre = nuevoNombre;
                    usuario.Email = nuevoEmail;
                    CargarDatos();

                    // Si prefieres que se cierre la ventana sí o sí, descomenta esto:
                    // this.DialogResult = DialogResult.OK; 
                    // this.Close();
                }
                else
                {
                    MessageBox.Show("Error al actualizar la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // BOTÓN CERRAR SESIÓN
        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            // Blindaje: Limpiamos las credenciales para que el Auto-Login no lo vuelva a meter directo
            Properties.Settings.Default.TokenSesion = "";
            Properties.Settings.Default.UsuarioRecordado = "";
            Properties.Settings.Default.Save();

            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

        // BOTÓN CAMBIAR CONTRASEÑA
        private void btnCambiarPassword_Click(object sender, EventArgs e)
        {
            string passActual = txtPassActual.Text;
            string passNueva = txtPassNueva.Text;
            string passConfirma = txtPassConfirmar.Text;

            // 1. Validaciones de campos vacíos o diferentes
            if (string.IsNullOrEmpty(passActual) || string.IsNullOrEmpty(passNueva) || string.IsNullOrEmpty(passConfirma))
            {
                MessageBox.Show("Por favor, completa todos los campos de contraseña.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (passNueva != passConfirma)
            {
                MessageBox.Show("La nueva contraseña y su confirmación no coinciden.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 2. CIBERSEGURIDAD: Verificar que la contraseña actual es realmente la suya
                string hashActual = SeguridadUtil.CalcularHashSHA256(passActual);
                if (hashActual != usuario.HashBD)
                {
                    MessageBox.Show("La contraseña actual es incorrecta.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Cortamos el proceso aquí si es un intruso
                }

                // 3. Generar y Enviar el Token
                string token = Guid.NewGuid().ToString().Substring(0, 7).ToUpper();
                CorreoUtil.EnviarTokenRecuperacion(usuario.Email, token);

                // Lo mostramos DESPUÉS de enviarlo, por si falla el envío que no mienta al usuario.
                MessageBox.Show("Se ha enviado un código de seguridad a tu correo para autorizar el cambio.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 4. Verificar el Token con el usuario
                using (FormVerificacion frm = new FormVerificacion(usuario.Email, token))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        // 5. EL PASO QUE FALTABA: Guardar en la Base de Datos
                        string hashNuevo = SeguridadUtil.CalcularHashSHA256(passNueva);

                        if (GestorDBAuth.ActualizarContrasena(usuario.Email, hashNuevo))
                        {
                            // Actualizamos en memoria por si acaso
                            usuario.HashBD = hashNuevo;

                            MessageBox.Show("Contraseña actualizada. Por seguridad, vuelve a iniciar sesión.", "Lexora", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Limpiamos tokens de auto-login para forzar la salida
                            Properties.Settings.Default.TokenSesion = "";
                            Properties.Settings.Default.UsuarioRecordado = "";
                            Properties.Settings.Default.Save();

                            this.DialogResult = DialogResult.Abort;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Error de conexión al intentar guardar la nueva contraseña.", "Error de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Si el correo falla (ej: sin internet), atrapamos el error sin que la app muera.
                MessageBox.Show("Error en el proceso: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // BOTÓN CERRAR / CANCELAR
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // =================================================================
        // REGISTRO DE ACTIVIDAD (LOGS)
        // =================================================================
        private void CargarLogsEnGrid()
        {
            try
            {
                
                DataTable dtMockup = new DataTable();
                dtMockup.Columns.Add("Fecha", typeof(string));
                dtMockup.Columns.Add("Acción", typeof(string));
                dtMockup.Columns.Add("Estado", typeof(string));

                dtMockup.Rows.Add(DateTime.Now.ToString("g"), "Inicio de Sesión", "Exitoso");
                dtMockup.Rows.Add(DateTime.Now.AddDays(-1).ToString("g"), "Consulta IA Realizada", "Completado");
                dtMockup.Rows.Add(DateTime.Now.AddDays(-2).ToString("g"), "Actualización de Perfil", "Exitoso");

                // Asigna los datos al DataGridView
                if (dgvLogsUsuario != null)
                {
                    dgvLogsUsuario.DataSource = dtMockup;
                    dgvLogsUsuario.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }

                logsCargados = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los logs: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}