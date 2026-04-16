namespace Lexora.Pantallas.InicioSesion
{
    partial class FormVerificacion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVerificacion));
            this.labelIntroducirCodigo = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtCodigoUsuario = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonValidarCodigo = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelIntroducirCodigo
            // 
            this.labelIntroducirCodigo.AutoSize = true;
            this.labelIntroducirCodigo.Font = new System.Drawing.Font("Segoe UI Variable Display", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelIntroducirCodigo.Location = new System.Drawing.Point(21, 9);
            this.labelIntroducirCodigo.Name = "labelIntroducirCodigo";
            this.labelIntroducirCodigo.Size = new System.Drawing.Size(538, 20);
            this.labelIntroducirCodigo.TabIndex = 0;
            this.labelIntroducirCodigo.Text = "Introduce el código que se te ha enviado a tu correo electrónico para verificarlo" +
    ". ";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel1.Controls.Add(this.labelIntroducirCodigo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(585, 44);
            this.panel1.TabIndex = 1;
            // 
            // txtCodigoUsuario
            // 
            this.txtCodigoUsuario.BackColor = System.Drawing.Color.White;
            this.txtCodigoUsuario.Font = new System.Drawing.Font("Segoe UI Variable Display", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodigoUsuario.Location = new System.Drawing.Point(189, 19);
            this.txtCodigoUsuario.Name = "txtCodigoUsuario";
            this.txtCodigoUsuario.Size = new System.Drawing.Size(196, 27);
            this.txtCodigoUsuario.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel2.Controls.Add(this.buttonValidarCodigo);
            this.panel2.Controls.Add(this.txtCodigoUsuario);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 44);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(585, 123);
            this.panel2.TabIndex = 3;
            // 
            // buttonValidarCodigo
            // 
            this.buttonValidarCodigo.Font = new System.Drawing.Font("Segoe UI Variable Display", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonValidarCodigo.Location = new System.Drawing.Point(240, 63);
            this.buttonValidarCodigo.Name = "buttonValidarCodigo";
            this.buttonValidarCodigo.Size = new System.Drawing.Size(90, 30);
            this.buttonValidarCodigo.TabIndex = 3;
            this.buttonValidarCodigo.Text = "Validar";
            this.buttonValidarCodigo.UseVisualStyleBackColor = true;
            this.buttonValidarCodigo.Click += new System.EventHandler(this.buttonValidarCodigo_Click);
            // 
            // FormVerificacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(585, 167);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormVerificacion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Verificación de Código - Lexora";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelIntroducirCodigo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtCodigoUsuario;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonValidarCodigo;
    }
}