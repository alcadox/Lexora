namespace Lexora
{
    partial class InicioSesion
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.PanelPrincipal = new System.Windows.Forms.Panel();
            this.btnIniciarSesion = new System.Windows.Forms.Button();
            this.lblCrearCuenta = new System.Windows.Forms.Label();
            this.lblPWolvidado = new System.Windows.Forms.Label();
            this.btnOmitir = new System.Windows.Forms.Button();
            this.pwUser = new System.Windows.Forms.TextBox();
            this.emailUser = new System.Windows.Forms.TextBox();
            this.lblContrasena = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.panelInformacion = new System.Windows.Forms.Panel();
            this.pictureBoxInfo = new System.Windows.Forms.PictureBox();
            this.labelTituloInformacion = new System.Windows.Forms.Label();
            this.panelSuperiorInformacion = new System.Windows.Forms.Panel();
            this.panelInferiorInformacion = new System.Windows.Forms.Panel();
            this.labelTextoInformacion = new System.Windows.Forms.Label();
            this.PanelPrincipal.SuspendLayout();
            this.panelInformacion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).BeginInit();
            this.panelSuperiorInformacion.SuspendLayout();
            this.panelInferiorInformacion.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelPrincipal
            // 
            this.PanelPrincipal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelPrincipal.BackColor = System.Drawing.Color.White;
            this.PanelPrincipal.Controls.Add(this.btnIniciarSesion);
            this.PanelPrincipal.Controls.Add(this.lblCrearCuenta);
            this.PanelPrincipal.Controls.Add(this.lblPWolvidado);
            this.PanelPrincipal.Controls.Add(this.btnOmitir);
            this.PanelPrincipal.Controls.Add(this.pwUser);
            this.PanelPrincipal.Controls.Add(this.emailUser);
            this.PanelPrincipal.Controls.Add(this.lblContrasena);
            this.PanelPrincipal.Controls.Add(this.lblEmail);
            this.PanelPrincipal.Location = new System.Drawing.Point(391, 95);
            this.PanelPrincipal.Name = "PanelPrincipal";
            this.PanelPrincipal.Size = new System.Drawing.Size(371, 419);
            this.PanelPrincipal.TabIndex = 0;
            // 
            // btnIniciarSesion
            // 
            this.btnIniciarSesion.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnIniciarSesion.Font = new System.Drawing.Font("Segoe UI Variable Display", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIniciarSesion.ForeColor = System.Drawing.Color.White;
            this.btnIniciarSesion.Location = new System.Drawing.Point(185, 313);
            this.btnIniciarSesion.Name = "btnIniciarSesion";
            this.btnIniciarSesion.Size = new System.Drawing.Size(100, 32);
            this.btnIniciarSesion.TabIndex = 8;
            this.btnIniciarSesion.Text = "Iniciar Sesión";
            this.btnIniciarSesion.UseVisualStyleBackColor = false;
            this.btnIniciarSesion.Click += new System.EventHandler(this.btnIniciarSesion_Click);
            // 
            // lblCrearCuenta
            // 
            this.lblCrearCuenta.AutoSize = true;
            this.lblCrearCuenta.Font = new System.Drawing.Font("Segoe UI Variable Display", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCrearCuenta.ForeColor = System.Drawing.Color.MediumPurple;
            this.lblCrearCuenta.Location = new System.Drawing.Point(62, 252);
            this.lblCrearCuenta.Name = "lblCrearCuenta";
            this.lblCrearCuenta.Size = new System.Drawing.Size(190, 16);
            this.lblCrearCuenta.TabIndex = 7;
            this.lblCrearCuenta.Text = "¿No tienes cuenta? Crear cuenta";
            this.lblCrearCuenta.Click += new System.EventHandler(this.lblCrearCuenta_Click);
            // 
            // lblPWolvidado
            // 
            this.lblPWolvidado.AutoSize = true;
            this.lblPWolvidado.Font = new System.Drawing.Font("Segoe UI Variable Display", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPWolvidado.ForeColor = System.Drawing.Color.MediumPurple;
            this.lblPWolvidado.Location = new System.Drawing.Point(62, 223);
            this.lblPWolvidado.Name = "lblPWolvidado";
            this.lblPWolvidado.Size = new System.Drawing.Size(170, 16);
            this.lblPWolvidado.TabIndex = 6;
            this.lblPWolvidado.Text = "¿Has olvidado tu contraseña?";
            this.lblPWolvidado.Click += new System.EventHandler(this.lblPWolvidado_Click);
            // 
            // btnOmitir
            // 
            this.btnOmitir.Font = new System.Drawing.Font("Segoe UI Variable Display", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOmitir.Location = new System.Drawing.Point(79, 313);
            this.btnOmitir.Name = "btnOmitir";
            this.btnOmitir.Size = new System.Drawing.Size(100, 32);
            this.btnOmitir.TabIndex = 4;
            this.btnOmitir.Text = "Omitir";
            this.btnOmitir.UseVisualStyleBackColor = true;
            this.btnOmitir.Click += new System.EventHandler(this.btnOmitir_Click);
            // 
            // pwUser
            // 
            this.pwUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pwUser.Location = new System.Drawing.Point(65, 170);
            this.pwUser.Name = "pwUser";
            this.pwUser.Size = new System.Drawing.Size(220, 22);
            this.pwUser.TabIndex = 3;
            // 
            // emailUser
            // 
            this.emailUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emailUser.Location = new System.Drawing.Point(65, 74);
            this.emailUser.Name = "emailUser";
            this.emailUser.Size = new System.Drawing.Size(220, 22);
            this.emailUser.TabIndex = 2;
            // 
            // lblContrasena
            // 
            this.lblContrasena.AutoSize = true;
            this.lblContrasena.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContrasena.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.lblContrasena.Location = new System.Drawing.Point(62, 142);
            this.lblContrasena.Name = "lblContrasena";
            this.lblContrasena.Size = new System.Drawing.Size(97, 21);
            this.lblContrasena.TabIndex = 1;
            this.lblContrasena.Text = "Contraseña";
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.lblEmail.Location = new System.Drawing.Point(62, 46);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(55, 21);
            this.lblEmail.TabIndex = 0;
            this.lblEmail.Text = "E-mail";
            // 
            // panelInformacion
            // 
            this.panelInformacion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(161)))), ((int)(((byte)(161)))));
            this.panelInformacion.Controls.Add(this.panelInferiorInformacion);
            this.panelInformacion.Controls.Add(this.panelSuperiorInformacion);
            this.panelInformacion.Location = new System.Drawing.Point(793, 235);
            this.panelInformacion.Name = "panelInformacion";
            this.panelInformacion.Size = new System.Drawing.Size(364, 111);
            this.panelInformacion.TabIndex = 1;
            this.panelInformacion.Visible = false;
            // 
            // pictureBoxInfo
            // 
            this.pictureBoxInfo.Image = global::Lexora.Properties.Resources.IconoInfoOficial;
            this.pictureBoxInfo.Location = new System.Drawing.Point(12, 10);
            this.pictureBoxInfo.Name = "pictureBoxInfo";
            this.pictureBoxInfo.Size = new System.Drawing.Size(22, 22);
            this.pictureBoxInfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxInfo.TabIndex = 0;
            this.pictureBoxInfo.TabStop = false;
            // 
            // labelTituloInformacion
            // 
            this.labelTituloInformacion.AutoSize = true;
            this.labelTituloInformacion.Font = new System.Drawing.Font("Segoe UI Variable Display", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTituloInformacion.Location = new System.Drawing.Point(38, 12);
            this.labelTituloInformacion.Name = "labelTituloInformacion";
            this.labelTituloInformacion.Size = new System.Drawing.Size(47, 20);
            this.labelTituloInformacion.TabIndex = 1;
            this.labelTituloInformacion.Text = "Aviso";
            // 
            // panelSuperiorInformacion
            // 
            this.panelSuperiorInformacion.Controls.Add(this.pictureBoxInfo);
            this.panelSuperiorInformacion.Controls.Add(this.labelTituloInformacion);
            this.panelSuperiorInformacion.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSuperiorInformacion.Location = new System.Drawing.Point(0, 0);
            this.panelSuperiorInformacion.Name = "panelSuperiorInformacion";
            this.panelSuperiorInformacion.Size = new System.Drawing.Size(364, 36);
            this.panelSuperiorInformacion.TabIndex = 2;
            // 
            // panelInferiorInformacion
            // 
            this.panelInferiorInformacion.Controls.Add(this.labelTextoInformacion);
            this.panelInferiorInformacion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelInferiorInformacion.Location = new System.Drawing.Point(0, 36);
            this.panelInferiorInformacion.Name = "panelInferiorInformacion";
            this.panelInferiorInformacion.Size = new System.Drawing.Size(364, 75);
            this.panelInferiorInformacion.TabIndex = 3;
            // 
            // labelTextoInformacion
            // 
            this.labelTextoInformacion.AutoSize = true;
            this.labelTextoInformacion.Font = new System.Drawing.Font("Segoe UI Variable Display Semib", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTextoInformacion.Location = new System.Drawing.Point(39, 5);
            this.labelTextoInformacion.Name = "labelTextoInformacion";
            this.labelTextoInformacion.Size = new System.Drawing.Size(37, 16);
            this.labelTextoInformacion.TabIndex = 0;
            this.labelTextoInformacion.Text = "label1";
            // 
            // InicioSesion
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.panelInformacion);
            this.Controls.Add(this.PanelPrincipal);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(1200, 700);
            this.Name = "InicioSesion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Iniciar Sesión - Lexora";
            this.PanelPrincipal.ResumeLayout(false);
            this.PanelPrincipal.PerformLayout();
            this.panelInformacion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).EndInit();
            this.panelSuperiorInformacion.ResumeLayout(false);
            this.panelSuperiorInformacion.PerformLayout();
            this.panelInferiorInformacion.ResumeLayout(false);
            this.panelInferiorInformacion.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PanelPrincipal;
        private System.Windows.Forms.TextBox pwUser;
        private System.Windows.Forms.TextBox emailUser;
        private System.Windows.Forms.Label lblContrasena;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblCrearCuenta;
        private System.Windows.Forms.Label lblPWolvidado;
        private System.Windows.Forms.Button btnOmitir;
        private System.Windows.Forms.Button btnIniciarSesion;
        private System.Windows.Forms.Panel panelInformacion;
        private System.Windows.Forms.PictureBox pictureBoxInfo;
        private System.Windows.Forms.Panel panelSuperiorInformacion;
        private System.Windows.Forms.Label labelTituloInformacion;
        private System.Windows.Forms.Panel panelInferiorInformacion;
        private System.Windows.Forms.Label labelTextoInformacion;
    }
}

