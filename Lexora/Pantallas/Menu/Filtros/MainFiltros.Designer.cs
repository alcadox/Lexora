namespace Lexora.Pantallas.Menu.Filtros
{
    partial class MainFiltros
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageFiltroTipoArchivo = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.checkedListBoxTipoArchivo = new System.Windows.Forms.CheckedListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTituloFiltroTipoArchivo = new System.Windows.Forms.Label();
            this.tabPageFiltroTamano = new System.Windows.Forms.TabPage();
            this.tabPageFiltroFecha = new System.Windows.Forms.TabPage();
            this.panel7 = new System.Windows.Forms.Panel();
            this.buttonLimpiarFecha = new System.Windows.Forms.Button();
            this.buttonAceptarFecha = new System.Windows.Forms.Button();
            this.monthCalendarFecha = new System.Windows.Forms.MonthCalendar();
            this.panel5 = new System.Windows.Forms.Panel();
            this.checkedListBoxTiposfecha = new System.Windows.Forms.CheckedListBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblInfoFecha = new System.Windows.Forms.Label();
            this.labelTipoDeFecha = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.tabPageFiltroContenido = new System.Windows.Forms.TabPage();
            this.tabPageFiltroMetadatosDocumentos = new System.Windows.Forms.TabPage();
            this.tabPageFiltroMetadatosImagenes = new System.Windows.Forms.TabPage();
            this.tabPageFiltroSeguridad = new System.Windows.Forms.TabPage();
            this.tabPageFiltroEstructura = new System.Windows.Forms.TabPage();
            this.panelPrincipal = new System.Windows.Forms.Panel();
            this.botonAplicar = new System.Windows.Forms.Button();
            this.botonCerrar = new System.Windows.Forms.Button();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.labelTituloFiltroSeguridad = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPageFiltroTipoArchivo.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPageFiltroFecha.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabPageFiltroSeguridad.SuspendLayout();
            this.panelPrincipal.SuspendLayout();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageFiltroTipoArchivo);
            this.tabControl.Controls.Add(this.tabPageFiltroTamano);
            this.tabControl.Controls.Add(this.tabPageFiltroFecha);
            this.tabControl.Controls.Add(this.tabPageFiltroContenido);
            this.tabControl.Controls.Add(this.tabPageFiltroMetadatosDocumentos);
            this.tabControl.Controls.Add(this.tabPageFiltroMetadatosImagenes);
            this.tabControl.Controls.Add(this.tabPageFiltroSeguridad);
            this.tabControl.Controls.Add(this.tabPageFiltroEstructura);
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(758, 400);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageFiltroTipoArchivo
            // 
            this.tabPageFiltroTipoArchivo.Controls.Add(this.panel3);
            this.tabPageFiltroTipoArchivo.Controls.Add(this.panel2);
            this.tabPageFiltroTipoArchivo.Controls.Add(this.panel1);
            this.tabPageFiltroTipoArchivo.Location = new System.Drawing.Point(4, 25);
            this.tabPageFiltroTipoArchivo.Name = "tabPageFiltroTipoArchivo";
            this.tabPageFiltroTipoArchivo.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFiltroTipoArchivo.Size = new System.Drawing.Size(750, 371);
            this.tabPageFiltroTipoArchivo.TabIndex = 0;
            this.tabPageFiltroTipoArchivo.Text = "Tipo";
            this.tabPageFiltroTipoArchivo.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.checkedListBoxTipoArchivo);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(3, 55);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(373, 313);
            this.panel3.TabIndex = 4;
            // 
            // checkedListBoxTipoArchivo
            // 
            this.checkedListBoxTipoArchivo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.checkedListBoxTipoArchivo.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkedListBoxTipoArchivo.FormattingEnabled = true;
            this.checkedListBoxTipoArchivo.Items.AddRange(new object[] {
            "Documentos (pdf, docx, txt...)",
            "Imágenes (png, jpg, svg...)",
            "Vídeos (mp4, avi...)",
            "Audio (mp3, wav...)",
            "Comprimidos (zip, rar, 7z...)",
            "Instaladores (exe, msi...)",
            "Código (java, js, py...)",
            "Cifrados"});
            this.checkedListBoxTipoArchivo.Location = new System.Drawing.Point(8, 12);
            this.checkedListBoxTipoArchivo.Name = "checkedListBoxTipoArchivo";
            this.checkedListBoxTipoArchivo.Size = new System.Drawing.Size(358, 288);
            this.checkedListBoxTipoArchivo.TabIndex = 2;
            this.checkedListBoxTipoArchivo.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxTipoArchivo_ItemCheck);
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(375, 55);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(372, 313);
            this.panel2.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblTituloFiltroTipoArchivo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(744, 52);
            this.panel1.TabIndex = 2;
            // 
            // lblTituloFiltroTipoArchivo
            // 
            this.lblTituloFiltroTipoArchivo.AutoSize = true;
            this.lblTituloFiltroTipoArchivo.Font = new System.Drawing.Font("Segoe UI Variable Display", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloFiltroTipoArchivo.Location = new System.Drawing.Point(3, 11);
            this.lblTituloFiltroTipoArchivo.Name = "lblTituloFiltroTipoArchivo";
            this.lblTituloFiltroTipoArchivo.Size = new System.Drawing.Size(245, 27);
            this.lblTituloFiltroTipoArchivo.TabIndex = 0;
            this.lblTituloFiltroTipoArchivo.Text = "Filtros por tipo de archivo:";
            // 
            // tabPageFiltroTamano
            // 
            this.tabPageFiltroTamano.Location = new System.Drawing.Point(4, 25);
            this.tabPageFiltroTamano.Name = "tabPageFiltroTamano";
            this.tabPageFiltroTamano.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFiltroTamano.Size = new System.Drawing.Size(750, 371);
            this.tabPageFiltroTamano.TabIndex = 1;
            this.tabPageFiltroTamano.Text = "Tamaño";
            this.tabPageFiltroTamano.UseVisualStyleBackColor = true;
            // 
            // tabPageFiltroFecha
            // 
            this.tabPageFiltroFecha.Controls.Add(this.panel7);
            this.tabPageFiltroFecha.Controls.Add(this.panel5);
            this.tabPageFiltroFecha.Controls.Add(this.panel4);
            this.tabPageFiltroFecha.Location = new System.Drawing.Point(4, 25);
            this.tabPageFiltroFecha.Name = "tabPageFiltroFecha";
            this.tabPageFiltroFecha.Size = new System.Drawing.Size(750, 371);
            this.tabPageFiltroFecha.TabIndex = 2;
            this.tabPageFiltroFecha.Text = "Fecha";
            this.tabPageFiltroFecha.UseVisualStyleBackColor = true;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.buttonLimpiarFecha);
            this.panel7.Controls.Add(this.buttonAceptarFecha);
            this.panel7.Controls.Add(this.monthCalendarFecha);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(346, 61);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(404, 310);
            this.panel7.TabIndex = 0;
            this.panel7.Paint += new System.Windows.Forms.PaintEventHandler(this.panel7_Paint);
            // 
            // buttonLimpiarFecha
            // 
            this.buttonLimpiarFecha.Location = new System.Drawing.Point(284, 84);
            this.buttonLimpiarFecha.Name = "buttonLimpiarFecha";
            this.buttonLimpiarFecha.Size = new System.Drawing.Size(75, 23);
            this.buttonLimpiarFecha.TabIndex = 2;
            this.buttonLimpiarFecha.Text = "Borrar";
            this.buttonLimpiarFecha.UseVisualStyleBackColor = true;
            // 
            // buttonAceptarFecha
            // 
            this.buttonAceptarFecha.Location = new System.Drawing.Point(284, 55);
            this.buttonAceptarFecha.Name = "buttonAceptarFecha";
            this.buttonAceptarFecha.Size = new System.Drawing.Size(75, 23);
            this.buttonAceptarFecha.TabIndex = 1;
            this.buttonAceptarFecha.Text = "Aceptar";
            this.buttonAceptarFecha.UseVisualStyleBackColor = true;
            this.buttonAceptarFecha.Click += new System.EventHandler(this.buttonAceptarFecha_Click);
            // 
            // monthCalendarFecha
            // 
            this.monthCalendarFecha.Location = new System.Drawing.Point(80, 12);
            this.monthCalendarFecha.MaxSelectionCount = 365;
            this.monthCalendarFecha.Name = "monthCalendarFecha";
            this.monthCalendarFecha.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.checkedListBoxTiposfecha);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(0, 61);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(346, 310);
            this.panel5.TabIndex = 1;
            // 
            // checkedListBoxTiposfecha
            // 
            this.checkedListBoxTiposfecha.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.checkedListBoxTiposfecha.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkedListBoxTiposfecha.FormattingEnabled = true;
            this.checkedListBoxTiposfecha.Items.AddRange(new object[] {
            "Fec. de creación",
            "Fec. de última edición",
            "Fec. de último acceso",
            "Antigüedad del archivo",
            "Archivos creados",
            "Archivos modificado sen los últimos __ días"});
            this.checkedListBoxTiposfecha.Location = new System.Drawing.Point(8, 6);
            this.checkedListBoxTiposfecha.Name = "checkedListBoxTiposfecha";
            this.checkedListBoxTiposfecha.Size = new System.Drawing.Size(329, 288);
            this.checkedListBoxTiposfecha.TabIndex = 3;
            this.checkedListBoxTiposfecha.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxTiposfecha_ItemCheck);
            // 
            // panel4
            // 
            this.panel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.panel4.Controls.Add(this.lblInfoFecha);
            this.panel4.Controls.Add(this.labelTipoDeFecha);
            this.panel4.Controls.Add(this.panel6);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(750, 61);
            this.panel4.TabIndex = 0;
            // 
            // lblInfoFecha
            // 
            this.lblInfoFecha.AutoSize = true;
            this.lblInfoFecha.Location = new System.Drawing.Point(423, 28);
            this.lblInfoFecha.Name = "lblInfoFecha";
            this.lblInfoFecha.Size = new System.Drawing.Size(260, 16);
            this.lblInfoFecha.TabIndex = 3;
            this.lblInfoFecha.Text = "Selecciona un filtro de fecha a la izquierda.";
            // 
            // labelTipoDeFecha
            // 
            this.labelTipoDeFecha.AutoSize = true;
            this.labelTipoDeFecha.Font = new System.Drawing.Font("Segoe UI Variable Display", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTipoDeFecha.Location = new System.Drawing.Point(3, 17);
            this.labelTipoDeFecha.Name = "labelTipoDeFecha";
            this.labelTipoDeFecha.Size = new System.Drawing.Size(227, 27);
            this.labelTipoDeFecha.TabIndex = 3;
            this.labelTipoDeFecha.Text = "Filtros por tipo de fecha:";
            this.labelTipoDeFecha.Click += new System.EventHandler(this.label1_Click);
            // 
            // panel6
            // 
            this.panel6.Location = new System.Drawing.Point(343, 87);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(404, 281);
            this.panel6.TabIndex = 2;
            // 
            // tabPageFiltroContenido
            // 
            this.tabPageFiltroContenido.Location = new System.Drawing.Point(4, 25);
            this.tabPageFiltroContenido.Name = "tabPageFiltroContenido";
            this.tabPageFiltroContenido.Size = new System.Drawing.Size(750, 371);
            this.tabPageFiltroContenido.TabIndex = 3;
            this.tabPageFiltroContenido.Text = "Contenido";
            this.tabPageFiltroContenido.UseVisualStyleBackColor = true;
            // 
            // tabPageFiltroMetadatosDocumentos
            // 
            this.tabPageFiltroMetadatosDocumentos.Location = new System.Drawing.Point(4, 25);
            this.tabPageFiltroMetadatosDocumentos.Name = "tabPageFiltroMetadatosDocumentos";
            this.tabPageFiltroMetadatosDocumentos.Size = new System.Drawing.Size(750, 371);
            this.tabPageFiltroMetadatosDocumentos.TabIndex = 5;
            this.tabPageFiltroMetadatosDocumentos.Text = "Metadatos Documentos";
            this.tabPageFiltroMetadatosDocumentos.UseVisualStyleBackColor = true;
            // 
            // tabPageFiltroMetadatosImagenes
            // 
            this.tabPageFiltroMetadatosImagenes.Location = new System.Drawing.Point(4, 25);
            this.tabPageFiltroMetadatosImagenes.Name = "tabPageFiltroMetadatosImagenes";
            this.tabPageFiltroMetadatosImagenes.Size = new System.Drawing.Size(750, 371);
            this.tabPageFiltroMetadatosImagenes.TabIndex = 6;
            this.tabPageFiltroMetadatosImagenes.Text = "Metadatos Imágenes";
            this.tabPageFiltroMetadatosImagenes.UseVisualStyleBackColor = true;
            // 
            // tabPageFiltroSeguridad
            // 
            this.tabPageFiltroSeguridad.Controls.Add(this.panel10);
            this.tabPageFiltroSeguridad.Controls.Add(this.panel9);
            this.tabPageFiltroSeguridad.Controls.Add(this.panel8);
            this.tabPageFiltroSeguridad.Location = new System.Drawing.Point(4, 25);
            this.tabPageFiltroSeguridad.Name = "tabPageFiltroSeguridad";
            this.tabPageFiltroSeguridad.Size = new System.Drawing.Size(750, 371);
            this.tabPageFiltroSeguridad.TabIndex = 4;
            this.tabPageFiltroSeguridad.Text = "Seguridad";
            this.tabPageFiltroSeguridad.UseVisualStyleBackColor = true;
            // 
            // tabPageFiltroEstructura
            // 
            this.tabPageFiltroEstructura.Location = new System.Drawing.Point(4, 25);
            this.tabPageFiltroEstructura.Name = "tabPageFiltroEstructura";
            this.tabPageFiltroEstructura.Size = new System.Drawing.Size(750, 371);
            this.tabPageFiltroEstructura.TabIndex = 7;
            this.tabPageFiltroEstructura.Text = "Estructura";
            this.tabPageFiltroEstructura.UseVisualStyleBackColor = true;
            // 
            // panelPrincipal
            // 
            this.panelPrincipal.Controls.Add(this.botonAplicar);
            this.panelPrincipal.Controls.Add(this.botonCerrar);
            this.panelPrincipal.Controls.Add(this.tabControl);
            this.panelPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPrincipal.Location = new System.Drawing.Point(0, 0);
            this.panelPrincipal.Name = "panelPrincipal";
            this.panelPrincipal.Size = new System.Drawing.Size(782, 456);
            this.panelPrincipal.TabIndex = 1;
            // 
            // botonAplicar
            // 
            this.botonAplicar.Location = new System.Drawing.Point(683, 417);
            this.botonAplicar.Name = "botonAplicar";
            this.botonAplicar.Size = new System.Drawing.Size(83, 26);
            this.botonAplicar.TabIndex = 2;
            this.botonAplicar.Text = "Aplicar";
            this.botonAplicar.UseVisualStyleBackColor = true;
            this.botonAplicar.Click += new System.EventHandler(this.botonAplicar_Click);
            // 
            // botonCerrar
            // 
            this.botonCerrar.Location = new System.Drawing.Point(594, 417);
            this.botonCerrar.Name = "botonCerrar";
            this.botonCerrar.Size = new System.Drawing.Size(83, 26);
            this.botonCerrar.TabIndex = 1;
            this.botonCerrar.Text = "Cerrar";
            this.botonCerrar.UseVisualStyleBackColor = true;
            this.botonCerrar.Click += new System.EventHandler(this.botonCerrar_Click);
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.labelTituloFiltroSeguridad);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(750, 62);
            this.panel8.TabIndex = 0;
            // 
            // panel9
            // 
            this.panel9.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel9.Location = new System.Drawing.Point(0, 62);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(363, 309);
            this.panel9.TabIndex = 1;
            // 
            // panel10
            // 
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(363, 62);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(387, 309);
            this.panel10.TabIndex = 2;
            // 
            // labelTituloFiltroSeguridad
            // 
            this.labelTituloFiltroSeguridad.AutoSize = true;
            this.labelTituloFiltroSeguridad.Font = new System.Drawing.Font("Segoe UI Variable Display", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTituloFiltroSeguridad.Location = new System.Drawing.Point(3, 15);
            this.labelTituloFiltroSeguridad.Name = "labelTituloFiltroSeguridad";
            this.labelTituloFiltroSeguridad.Size = new System.Drawing.Size(304, 27);
            this.labelTituloFiltroSeguridad.TabIndex = 1;
            this.labelTituloFiltroSeguridad.Text = "Filtros por Seguridad del archivo:";
            this.labelTituloFiltroSeguridad.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // MainFiltros
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 456);
            this.Controls.Add(this.panelPrincipal);
            this.Name = "MainFiltros";
            this.Text = "Filtros";
            this.tabControl.ResumeLayout(false);
            this.tabPageFiltroTipoArchivo.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPageFiltroFecha.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.tabPageFiltroSeguridad.ResumeLayout(false);
            this.panelPrincipal.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageFiltroTipoArchivo;
        private System.Windows.Forms.TabPage tabPageFiltroTamano;
        private System.Windows.Forms.Panel panelPrincipal;
        private System.Windows.Forms.TabPage tabPageFiltroFecha;
        private System.Windows.Forms.TabPage tabPageFiltroContenido;
        private System.Windows.Forms.TabPage tabPageFiltroMetadatosDocumentos;
        private System.Windows.Forms.TabPage tabPageFiltroMetadatosImagenes;
        private System.Windows.Forms.TabPage tabPageFiltroSeguridad;
        private System.Windows.Forms.TabPage tabPageFiltroEstructura;
        private System.Windows.Forms.Button botonAplicar;
        private System.Windows.Forms.Button botonCerrar;
        private System.Windows.Forms.Label lblTituloFiltroTipoArchivo;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckedListBox checkedListBoxTipoArchivo;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label labelTipoDeFecha;
        private System.Windows.Forms.CheckedListBox checkedListBoxTiposfecha;
        private System.Windows.Forms.MonthCalendar monthCalendarFecha;
        private System.Windows.Forms.Button buttonAceptarFecha;
        private System.Windows.Forms.Button buttonLimpiarFecha;
        private System.Windows.Forms.Label lblInfoFecha;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Label labelTituloFiltroSeguridad;
    }
}