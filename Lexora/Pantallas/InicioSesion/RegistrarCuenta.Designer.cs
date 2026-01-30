namespace Lexora.Pantallas.InicioSesion
{
    partial class RegistrarCuenta
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PanelPrincipal = new System.Windows.Forms.Panel();
            this.btnCrearCuenta = new System.Windows.Forms.Button();
            this.lblIniciarSesion = new System.Windows.Forms.Label();
            this.textBoxContrasena = new System.Windows.Forms.TextBox();
            this.textBoxemailUser = new System.Windows.Forms.TextBox();
            this.lblContrasena = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.textBoxNombreUsuario = new System.Windows.Forms.TextBox();
            this.labeNombreUsuario = new System.Windows.Forms.Label();
            this.textBoxVerificarContrasena = new System.Windows.Forms.TextBox();
            this.labelVerificarContrasena = new System.Windows.Forms.Label();
            this.PanelPrincipal.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelPrincipal
            // 
            this.PanelPrincipal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelPrincipal.BackColor = System.Drawing.Color.White;
            this.PanelPrincipal.Controls.Add(this.textBoxVerificarContrasena);
            this.PanelPrincipal.Controls.Add(this.labelVerificarContrasena);
            this.PanelPrincipal.Controls.Add(this.textBoxNombreUsuario);
            this.PanelPrincipal.Controls.Add(this.labeNombreUsuario);
            this.PanelPrincipal.Controls.Add(this.btnCrearCuenta);
            this.PanelPrincipal.Controls.Add(this.lblIniciarSesion);
            this.PanelPrincipal.Controls.Add(this.textBoxContrasena);
            this.PanelPrincipal.Controls.Add(this.textBoxemailUser);
            this.PanelPrincipal.Controls.Add(this.lblContrasena);
            this.PanelPrincipal.Controls.Add(this.lblEmail);
            this.PanelPrincipal.Location = new System.Drawing.Point(413, 78);
            this.PanelPrincipal.Name = "PanelPrincipal";
            this.PanelPrincipal.Size = new System.Drawing.Size(359, 459);
            this.PanelPrincipal.TabIndex = 1;
            // 
            // btnCrearCuenta
            // 
            this.btnCrearCuenta.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnCrearCuenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCrearCuenta.ForeColor = System.Drawing.Color.White;
            this.btnCrearCuenta.Location = new System.Drawing.Point(132, 395);
            this.btnCrearCuenta.Name = "btnCrearCuenta";
            this.btnCrearCuenta.Size = new System.Drawing.Size(100, 32);
            this.btnCrearCuenta.TabIndex = 8;
            this.btnCrearCuenta.Text = "Crear Cuenta";
            this.btnCrearCuenta.UseVisualStyleBackColor = false;
            // 
            // lblIniciarSesion
            // 
            this.lblIniciarSesion.AutoSize = true;
            this.lblIniciarSesion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIniciarSesion.ForeColor = System.Drawing.Color.MediumPurple;
            this.lblIniciarSesion.Location = new System.Drawing.Point(62, 341);
            this.lblIniciarSesion.Name = "lblIniciarSesion";
            this.lblIniciarSesion.Size = new System.Drawing.Size(198, 13);
            this.lblIniciarSesion.TabIndex = 7;
            this.lblIniciarSesion.Text = "¿Ya tienes cuenta? Iniciar Sesión";
            this.lblIniciarSesion.Click += new System.EventHandler(this.lblIniciarSesion_Click);
            // 
            // textBoxContrasena
            // 
            this.textBoxContrasena.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxContrasena.Location = new System.Drawing.Point(65, 214);
            this.textBoxContrasena.Name = "textBoxContrasena";
            this.textBoxContrasena.Size = new System.Drawing.Size(220, 22);
            this.textBoxContrasena.TabIndex = 3;
            // 
            // textBoxemailUser
            // 
            this.textBoxemailUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxemailUser.Location = new System.Drawing.Point(65, 139);
            this.textBoxemailUser.Name = "textBoxemailUser";
            this.textBoxemailUser.Size = new System.Drawing.Size(220, 22);
            this.textBoxemailUser.TabIndex = 2;
            // 
            // lblContrasena
            // 
            this.lblContrasena.AutoSize = true;
            this.lblContrasena.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContrasena.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.lblContrasena.Location = new System.Drawing.Point(62, 187);
            this.lblContrasena.Name = "lblContrasena";
            this.lblContrasena.Size = new System.Drawing.Size(86, 16);
            this.lblContrasena.TabIndex = 1;
            this.lblContrasena.Text = "Contraseña";
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.lblEmail.Location = new System.Drawing.Point(62, 117);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(51, 16);
            this.lblEmail.TabIndex = 0;
            this.lblEmail.Text = "E-mail";
            // 
            // textBoxNombreUsuario
            // 
            this.textBoxNombreUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxNombreUsuario.Location = new System.Drawing.Point(65, 73);
            this.textBoxNombreUsuario.Name = "textBoxNombreUsuario";
            this.textBoxNombreUsuario.Size = new System.Drawing.Size(220, 22);
            this.textBoxNombreUsuario.TabIndex = 10;
            // 
            // labeNombreUsuario
            // 
            this.labeNombreUsuario.AutoSize = true;
            this.labeNombreUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labeNombreUsuario.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.labeNombreUsuario.Location = new System.Drawing.Point(62, 51);
            this.labeNombreUsuario.Name = "labeNombreUsuario";
            this.labeNombreUsuario.Size = new System.Drawing.Size(142, 16);
            this.labeNombreUsuario.TabIndex = 9;
            this.labeNombreUsuario.Text = "Nombre de Usuario";
            // 
            // textBoxVerificarContrasena
            // 
            this.textBoxVerificarContrasena.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxVerificarContrasena.Location = new System.Drawing.Point(65, 283);
            this.textBoxVerificarContrasena.Name = "textBoxVerificarContrasena";
            this.textBoxVerificarContrasena.Size = new System.Drawing.Size(220, 22);
            this.textBoxVerificarContrasena.TabIndex = 12;
            // 
            // labelVerificarContrasena
            // 
            this.labelVerificarContrasena.AutoSize = true;
            this.labelVerificarContrasena.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVerificarContrasena.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.labelVerificarContrasena.Location = new System.Drawing.Point(62, 256);
            this.labelVerificarContrasena.Name = "labelVerificarContrasena";
            this.labelVerificarContrasena.Size = new System.Drawing.Size(148, 16);
            this.labelVerificarContrasena.TabIndex = 11;
            this.labelVerificarContrasena.Text = "Verificar Contraseña";
            // 
            // RegistrarCuenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.PanelPrincipal);
            this.Name = "RegistrarCuenta";
            this.Text = "Registrar Cuenta - Lexora";
            this.PanelPrincipal.ResumeLayout(false);
            this.PanelPrincipal.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PanelPrincipal;
        private System.Windows.Forms.Button btnCrearCuenta;
        private System.Windows.Forms.Label lblIniciarSesion;
        private System.Windows.Forms.TextBox textBoxContrasena;
        private System.Windows.Forms.TextBox textBoxemailUser;
        private System.Windows.Forms.Label lblContrasena;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox textBoxVerificarContrasena;
        private System.Windows.Forms.Label labelVerificarContrasena;
        private System.Windows.Forms.TextBox textBoxNombreUsuario;
        private System.Windows.Forms.Label labeNombreUsuario;
    }
}