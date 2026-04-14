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
            this.panelBuscador = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            this.btnFiltros = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            this.txtBoxBuscador = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            this.btnIA = new Siticone.Desktop.UI.WinForms.SiticoneCircleButton();
            this.btnSesion = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            this.panelIzquierdo = new System.Windows.Forms.Panel();
            this.siticonePanel2 = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            this.panelArchivos = new System.Windows.Forms.Panel();
            this.pnlBreadcrumbs = new System.Windows.Forms.FlowLayoutPanel();
            this.listViewArchivos = new System.Windows.Forms.ListView();
            this.columnaNombre = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnaTipo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnaTamaño = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnaCreacion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.panelTop.Size = new System.Drawing.Size(1184, 65);
            this.panelTop.TabIndex = 0;
            // 
            // panelBuscador
            // 
            this.panelBuscador.BackColor = System.Drawing.Color.Transparent;
            this.panelBuscador.BorderRadius = 4;
            this.panelBuscador.Controls.Add(this.btnFiltros);
            this.panelBuscador.Controls.Add(this.txtBoxBuscador);
            this.panelBuscador.Controls.Add(this.btnIA);
            this.panelBuscador.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(197)))), ((int)(((byte)(255)))));
            this.panelBuscador.Location = new System.Drawing.Point(198, 12);
            this.panelBuscador.Name = "panelBuscador";
            this.panelBuscador.Size = new System.Drawing.Size(974, 48);
            this.panelBuscador.TabIndex = 4;
            // 
            // btnFiltros
            // 
            this.btnFiltros.BackColor = System.Drawing.Color.Transparent;
            this.btnFiltros.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(33)))), ((int)(((byte)(173)))));
            this.btnFiltros.BorderRadius = 5;
            this.btnFiltros.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFiltros.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnFiltros.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnFiltros.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnFiltros.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnFiltros.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnFiltros.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(145)))), ((int)(((byte)(255)))));
            this.btnFiltros.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFiltros.ForeColor = System.Drawing.Color.White;
            this.btnFiltros.Image = global::Lexora.Properties.Resources.ico_filtros;
            this.btnFiltros.ImageAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.btnFiltros.ImageOffset = new System.Drawing.Point(3, 1);
            this.btnFiltros.Location = new System.Drawing.Point(871, 8);
            this.btnFiltros.Name = "btnFiltros";
            this.btnFiltros.Size = new System.Drawing.Size(96, 32);
            this.btnFiltros.TabIndex = 4;
            this.btnFiltros.Text = "Filtros";
            this.btnFiltros.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnFiltros.Click += new System.EventHandler(this.btnFiltros_Click);
            // 
            // txtBoxBuscador
            // 
            this.txtBoxBuscador.BorderRadius = 5;
            this.txtBoxBuscador.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtBoxBuscador.DefaultText = "";
            this.txtBoxBuscador.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtBoxBuscador.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtBoxBuscador.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtBoxBuscador.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtBoxBuscador.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtBoxBuscador.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxBuscador.ForeColor = System.Drawing.Color.Black;
            this.txtBoxBuscador.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtBoxBuscador.Location = new System.Drawing.Point(45, 8);
            this.txtBoxBuscador.Name = "txtBoxBuscador";
            this.txtBoxBuscador.PasswordChar = '\0';
            this.txtBoxBuscador.PlaceholderText = "Buscar archivos, documentos o imágenes...";
            this.txtBoxBuscador.SelectedText = "";
            this.txtBoxBuscador.Size = new System.Drawing.Size(820, 32);
            this.txtBoxBuscador.TabIndex = 2;
            this.txtBoxBuscador.TextChanged += new System.EventHandler(this.txtBoxBuscador_TextChanged);
            // 
            // btnIA
            // 
            this.btnIA.BackColor = System.Drawing.Color.Transparent;
            this.btnIA.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIA.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnIA.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnIA.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnIA.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnIA.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnIA.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnIA.ForeColor = System.Drawing.Color.White;
            this.btnIA.Image = global::Lexora.Properties.Resources.ico_ia_estrellas;
            this.btnIA.ImageOffset = new System.Drawing.Point(1, 0);
            this.btnIA.ImageSize = new System.Drawing.Size(25, 25);
            this.btnIA.Location = new System.Drawing.Point(6, 7);
            this.btnIA.Name = "btnIA";
            this.btnIA.ShadowDecoration.Mode = Siticone.Desktop.UI.WinForms.Enums.ShadowMode.Circle;
            this.btnIA.Size = new System.Drawing.Size(33, 33);
            this.btnIA.TabIndex = 4;
            // 
            // btnSesion
            // 
            this.btnSesion.BackColor = System.Drawing.Color.Transparent;
            this.btnSesion.BorderRadius = 10;
            this.btnSesion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSesion.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSesion.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSesion.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSesion.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSesion.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(108)))), ((int)(((byte)(253)))));
            this.btnSesion.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(145)))), ((int)(((byte)(255)))));
            this.btnSesion.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSesion.ForeColor = System.Drawing.Color.White;
            this.btnSesion.Image = global::Lexora.Properties.Resources.ico_usuario;
            this.btnSesion.ImageOffset = new System.Drawing.Point(-4, 0);
            this.btnSesion.ImageSize = new System.Drawing.Size(34, 34);
            this.btnSesion.Location = new System.Drawing.Point(12, 14);
            this.btnSesion.Name = "btnSesion";
            this.btnSesion.Size = new System.Drawing.Size(171, 43);
            this.btnSesion.TabIndex = 2;
            this.btnSesion.Text = "Iniciar Sesión";
            this.btnSesion.Click += new System.EventHandler(this.btnSesion_Click);
            // 
            // panelIzquierdo
            // 
            this.panelIzquierdo.BackColor = System.Drawing.Color.Transparent;
            this.panelIzquierdo.Controls.Add(this.siticonePanel2);
            this.panelIzquierdo.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelIzquierdo.Location = new System.Drawing.Point(0, 65);
            this.panelIzquierdo.Name = "panelIzquierdo";
            this.panelIzquierdo.Size = new System.Drawing.Size(188, 610);
            this.panelIzquierdo.TabIndex = 1;
            // 
            // siticonePanel2
            // 
            this.siticonePanel2.BackColor = System.Drawing.Color.Transparent;
            this.siticonePanel2.BorderRadius = 5;
            this.siticonePanel2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(167)))), ((int)(((byte)(255)))));
            this.siticonePanel2.Location = new System.Drawing.Point(12, 14);
            this.siticonePanel2.Name = "siticonePanel2";
            this.siticonePanel2.Size = new System.Drawing.Size(171, 564);
            this.siticonePanel2.TabIndex = 5;
            // 
            // panelArchivos
            // 
            this.panelArchivos.Controls.Add(this.pnlBreadcrumbs);
            this.panelArchivos.Controls.Add(this.listViewArchivos);
            this.panelArchivos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelArchivos.Location = new System.Drawing.Point(188, 65);
            this.panelArchivos.Name = "panelArchivos";
            this.panelArchivos.Size = new System.Drawing.Size(996, 610);
            this.panelArchivos.TabIndex = 2;
            // 
            // pnlBreadcrumbs
            // 
            this.pnlBreadcrumbs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlBreadcrumbs.Location = new System.Drawing.Point(7, 581);
            this.pnlBreadcrumbs.Name = "pnlBreadcrumbs";
            this.pnlBreadcrumbs.Size = new System.Drawing.Size(974, 26);
            this.pnlBreadcrumbs.TabIndex = 1;
            // 
            // listViewArchivos
            // 
            this.listViewArchivos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(247)))), ((int)(((byte)(255)))));
            this.listViewArchivos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewArchivos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnaNombre,
            this.columnaTipo,
            this.columnaTamaño,
            this.columnaCreacion});
            this.listViewArchivos.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewArchivos.FullRowSelect = true;
            this.listViewArchivos.HideSelection = false;
            this.listViewArchivos.Location = new System.Drawing.Point(10, 9);
            this.listViewArchivos.Name = "listViewArchivos";
            this.listViewArchivos.Size = new System.Drawing.Size(974, 569);
            this.listViewArchivos.TabIndex = 0;
            this.listViewArchivos.UseCompatibleStateImageBehavior = false;
            this.listViewArchivos.View = System.Windows.Forms.View.Details;
            this.listViewArchivos.DoubleClick += new System.EventHandler(this.listViewArchivos_DoubleClick);
            // 
            // columnaNombre
            // 
            this.columnaNombre.Text = "Nombre";
            this.columnaNombre.Width = 400;
            // 
            // columnaTipo
            // 
            this.columnaTipo.Text = "Tipo";
            this.columnaTipo.Width = 150;
            // 
            // columnaTamaño
            // 
            this.columnaTamaño.Text = "Tamaño";
            this.columnaTamaño.Width = 97;
            // 
            // columnaCreacion
            // 
            this.columnaCreacion.Text = "Fecha de Creación";
            this.columnaCreacion.Width = 240;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1184, 675);
            this.Controls.Add(this.panelArchivos);
            this.Controls.Add(this.panelIzquierdo);
            this.Controls.Add(this.panelTop);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(1200, 700);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lexora";
            this.panelTop.ResumeLayout(false);
            this.panelBuscador.ResumeLayout(false);
            this.panelIzquierdo.ResumeLayout(false);
            this.panelArchivos.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelIzquierdo;
        private System.Windows.Forms.Panel panelArchivos;
        private System.Windows.Forms.ListView listViewArchivos;
        private System.Windows.Forms.ColumnHeader columnaNombre;
        private System.Windows.Forms.ColumnHeader columnaTipo;
        private System.Windows.Forms.ColumnHeader columnaTamaño;
        private System.Windows.Forms.ColumnHeader columnaCreacion;
        private System.Windows.Forms.FlowLayoutPanel pnlBreadcrumbs;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnSesion;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelBuscador;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txtBoxBuscador;
        private Siticone.Desktop.UI.WinForms.SiticoneCircleButton btnIA;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnFiltros;
        private Siticone.Desktop.UI.WinForms.SiticonePanel siticonePanel2;
    }
}