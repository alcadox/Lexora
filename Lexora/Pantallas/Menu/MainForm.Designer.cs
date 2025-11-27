namespace Lexora
{
    partial class MainForm
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelBuscador = new System.Windows.Forms.Panel();
            this.txtBoxBuscador = new System.Windows.Forms.TextBox();
            this.btnIA = new System.Windows.Forms.Button();
            this.btnFiltros = new System.Windows.Forms.Button();
            this.btnSesion = new System.Windows.Forms.Button();
            this.panelIzquierdo = new System.Windows.Forms.Panel();
            this.panelIzquierdoArchivos = new System.Windows.Forms.Panel();
            this.panelArchivos = new System.Windows.Forms.Panel();
            this.listViewArchivos = new System.Windows.Forms.ListView();
            this.panelTop.SuspendLayout();
            this.panelBuscador.SuspendLayout();
            this.panelIzquierdo.SuspendLayout();
            this.panelArchivos.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.panelBuscador);
            this.panelTop.Controls.Add(this.btnSesion);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1184, 71);
            this.panelTop.TabIndex = 0;
            // 
            // panelBuscador
            // 
            this.panelBuscador.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(197)))), ((int)(((byte)(255)))));
            this.panelBuscador.Controls.Add(this.txtBoxBuscador);
            this.panelBuscador.Controls.Add(this.btnIA);
            this.panelBuscador.Controls.Add(this.btnFiltros);
            this.panelBuscador.Location = new System.Drawing.Point(209, 17);
            this.panelBuscador.Name = "panelBuscador";
            this.panelBuscador.Size = new System.Drawing.Size(947, 35);
            this.panelBuscador.TabIndex = 1;
            // 
            // txtBoxBuscador
            // 
            this.txtBoxBuscador.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxBuscador.Location = new System.Drawing.Point(50, 7);
            this.txtBoxBuscador.Name = "txtBoxBuscador";
            this.txtBoxBuscador.Size = new System.Drawing.Size(779, 22);
            this.txtBoxBuscador.TabIndex = 2;
            // 
            // btnIA
            // 
            this.btnIA.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnIA.ForeColor = System.Drawing.Color.White;
            this.btnIA.Location = new System.Drawing.Point(5, 6);
            this.btnIA.Name = "btnIA";
            this.btnIA.Size = new System.Drawing.Size(32, 23);
            this.btnIA.TabIndex = 1;
            this.btnIA.Text = "IA";
            this.btnIA.UseVisualStyleBackColor = false;
            // 
            // btnFiltros
            // 
            this.btnFiltros.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnFiltros.ForeColor = System.Drawing.Color.White;
            this.btnFiltros.Location = new System.Drawing.Point(838, 3);
            this.btnFiltros.Name = "btnFiltros";
            this.btnFiltros.Size = new System.Drawing.Size(102, 30);
            this.btnFiltros.TabIndex = 0;
            this.btnFiltros.Text = "Filtros";
            this.btnFiltros.UseVisualStyleBackColor = false;
            // 
            // btnSesion
            // 
            this.btnSesion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(108)))), ((int)(((byte)(253)))));
            this.btnSesion.ForeColor = System.Drawing.Color.White;
            this.btnSesion.Location = new System.Drawing.Point(12, 14);
            this.btnSesion.Name = "btnSesion";
            this.btnSesion.Size = new System.Drawing.Size(176, 40);
            this.btnSesion.TabIndex = 0;
            this.btnSesion.Text = "Iniciar Sesión";
            this.btnSesion.UseVisualStyleBackColor = false;
            // 
            // panelIzquierdo
            // 
            this.panelIzquierdo.BackColor = System.Drawing.Color.Transparent;
            this.panelIzquierdo.Controls.Add(this.panelIzquierdoArchivos);
            this.panelIzquierdo.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelIzquierdo.Location = new System.Drawing.Point(0, 71);
            this.panelIzquierdo.Name = "panelIzquierdo";
            this.panelIzquierdo.Size = new System.Drawing.Size(188, 590);
            this.panelIzquierdo.TabIndex = 1;
            // 
            // panelIzquierdoArchivos
            // 
            this.panelIzquierdoArchivos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(167)))), ((int)(((byte)(255)))));
            this.panelIzquierdoArchivos.Location = new System.Drawing.Point(12, 6);
            this.panelIzquierdoArchivos.Name = "panelIzquierdoArchivos";
            this.panelIzquierdoArchivos.Size = new System.Drawing.Size(167, 572);
            this.panelIzquierdoArchivos.TabIndex = 2;
            // 
            // panelArchivos
            // 
            this.panelArchivos.Controls.Add(this.listViewArchivos);
            this.panelArchivos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelArchivos.Location = new System.Drawing.Point(188, 71);
            this.panelArchivos.Name = "panelArchivos";
            this.panelArchivos.Size = new System.Drawing.Size(996, 590);
            this.panelArchivos.TabIndex = 2;
            // 
            // listViewArchivos
            // 
            this.listViewArchivos.HideSelection = false;
            this.listViewArchivos.Location = new System.Drawing.Point(21, 6);
            this.listViewArchivos.Name = "listViewArchivos";
            this.listViewArchivos.Size = new System.Drawing.Size(942, 572);
            this.listViewArchivos.TabIndex = 0;
            this.listViewArchivos.UseCompatibleStateImageBehavior = false;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.panelArchivos);
            this.Controls.Add(this.panelIzquierdo);
            this.Controls.Add(this.panelTop);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(1200, 700);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lexora";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panelTop.ResumeLayout(false);
            this.panelBuscador.ResumeLayout(false);
            this.panelBuscador.PerformLayout();
            this.panelIzquierdo.ResumeLayout(false);
            this.panelArchivos.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnSesion;
        private System.Windows.Forms.Panel panelBuscador;
        private System.Windows.Forms.Button btnFiltros;
        private System.Windows.Forms.Button btnIA;
        private System.Windows.Forms.TextBox txtBoxBuscador;
        private System.Windows.Forms.Panel panelIzquierdo;
        private System.Windows.Forms.Panel panelIzquierdoArchivos;
        private System.Windows.Forms.Panel panelArchivos;
        private System.Windows.Forms.ListView listViewArchivos;
    }
}