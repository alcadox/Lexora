using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Shell;

namespace Lexora.Core
{
    public static class UIBuilder
    {
        // Constructor maestro de tarjetas de discos
        public static Siticone.Desktop.UI.WinForms.SiticonePanel ConstruirTarjetaDisco(DriveInfo disco, EventHandler accionClick)
        {
            Siticone.Desktop.UI.WinForms.SiticonePanel tarjeta = new Siticone.Desktop.UI.WinForms.SiticonePanel
            {
                Size = new Size(150, 75),
                BorderRadius = 8,
                FillColor = Color.FromArgb(245, 238, 255),
                BorderColor = Color.FromArgb(187, 167, 255),
                BorderThickness = 1,
                Margin = new Padding(0, 0, 0, 10),
                Cursor = Cursors.Hand,
                Tag = disco.RootDirectory.FullName
            };

            tarjeta.MouseEnter += (s, e) => tarjeta.FillColor = Color.FromArgb(228, 215, 255);
            tarjeta.MouseLeave += (s, e) => tarjeta.FillColor = Color.FromArgb(245, 238, 255);

            Image iconoDrive = null;
            try
            {
                var shellFile = ShellObject.FromParsingName(disco.Name);
                iconoDrive = shellFile.Thumbnail.MediumBitmap;
            }
            catch
            {
                iconoDrive = SystemIcons.WinLogo.ToBitmap();
            }

            PictureBox picIcono = new PictureBox
            {
                Image = iconoDrive,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(32, 32),
                Location = new Point(10, 10),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = disco.RootDirectory.FullName
            };

            string nombreMostrar = string.IsNullOrEmpty(disco.VolumeLabel) ? (disco.DriveType == DriveType.Removable ? "USB" : "Disco Local") : disco.VolumeLabel;
            Label lblNombre = new Label
            {
                Text = $"{nombreMostrar} ({disco.Name.Replace("\\", "")})",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                Location = new Point(48, 10),
                AutoSize = true,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = disco.RootDirectory.FullName
            };

            Siticone.Desktop.UI.WinForms.SiticoneProgressBar barraEspacio = new Siticone.Desktop.UI.WinForms.SiticoneProgressBar
            {
                Size = new Size(120, 6),
                Location = new Point(10, 48),
                BorderRadius = 3,
                FillColor = Color.FromArgb(225, 215, 240),
                ProgressColor = Color.FromArgb(142, 108, 253),
                ProgressColor2 = Color.FromArgb(173, 145, 255),
                Cursor = Cursors.Hand,
                Tag = disco.RootDirectory.FullName
            };

            Label lblEspacio = new Label
            {
                Font = new Font("Segoe UI", 7.5F, FontStyle.Regular),
                ForeColor = Color.DimGray,
                Location = new Point(7, 57),
                AutoSize = true,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = disco.RootDirectory.FullName
            };

            try
            {
                long total = disco.TotalSize;
                long libre = disco.TotalFreeSpace;
                long usado = total - libre;
                int porcentaje = (int)((usado * 100) / total);

                barraEspacio.Value = porcentaje;

                if (porcentaje > 90)
                {
                    barraEspacio.ProgressColor = Color.FromArgb(255, 60, 60);
                    barraEspacio.ProgressColor2 = Color.FromArgb(255, 100, 100);
                }

                lblEspacio.Text = $"{ArchivosUtil.FormatearTamaño(libre)} libres de {ArchivosUtil.FormatearTamaño(total)}";
            }
            catch
            {
                barraEspacio.Value = 0;
                lblEspacio.Text = "Espacio desconocido";
            }

            // Asignación unificada de eventos
            tarjeta.Click += accionClick;
            picIcono.Click += accionClick;
            lblNombre.Click += accionClick;
            barraEspacio.Click += accionClick;
            lblEspacio.Click += accionClick;

            tarjeta.Controls.Add(picIcono);
            tarjeta.Controls.Add(lblNombre);
            tarjeta.Controls.Add(barraEspacio);
            tarjeta.Controls.Add(lblEspacio);

            return tarjeta;
        }
    }
}