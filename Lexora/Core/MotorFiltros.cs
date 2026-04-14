using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAPICodePack.Shell;
using iText.Kernel.Pdf;
using Lexora.Pantallas.Menu.Filtros;

namespace Lexora.Core
{
    public class MotorFiltros
    {
        private readonly ClaseFiltros filtros;

        public MotorFiltros(ClaseFiltros filtrosActivos)
        {
            this.filtros = filtrosActivos;
        }

        public bool CarpetaTieneArchivosQueCumplen(string rutaCarpeta, HashSet<string> extensionesPermitidas, bool tieneFiltrosTipo)
        {
            try
            {
                foreach (var archivo in Directory.EnumerateFiles(rutaCarpeta))
                {
                    FileInfo info = new FileInfo(archivo);
                    string ext = info.Extension.ToLowerInvariant();

                    if (tieneFiltrosTipo && !extensionesPermitidas.Contains(ext)) continue;
                    if (!CumpleFiltrosFecha(info)) continue;
                    if (!CumpleFiltrosSeguridad(info)) continue;
                    if (!CumpleFiltrosMetadatosDocumento(archivo)) continue;
                    if (!CumpleFiltrosMetadatosImagen(archivo)) continue;

                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool CumpleFiltroTamano(string rutaArchivo)
        {
            if (!filtros.FiltrarTamano) return true;
            try
            {
                FileInfo info = new FileInfo(rutaArchivo);
                long tamanoBytes = info.Length;
                return tamanoBytes >= filtros.TamanoMin && tamanoBytes <= filtros.TamanoMax;
            }
            catch { return false; }
        }

        public bool CumpleFiltrosFecha(FileInfo info)
        {
            if (filtros.Fechas == null || filtros.Fechas.Count == 0) return true;

            foreach (var kv in filtros.Fechas)
            {
                string nombreFiltro = kv.Key ?? "";
                DateTime? desde = kv.Value.Desde?.Date;
                DateTime? hasta = kv.Value.Hasta?.Date;

                if (!desde.HasValue || !hasta.HasValue) continue;

                string key = (nombreFiltro ?? "").ToLower()
                        .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("ü", "u")
                        .Replace(".", "").Replace("  ", " ").Trim();

                DateTime valorAComparar;
                if (key.Contains("creacion") || key.Contains("archivos creados") || key.Contains("antiguedad"))
                    valorAComparar = info.CreationTime.Date;
                else if (key.Contains("fec de ultima edicion") || key.Contains("modificado"))
                    valorAComparar = info.LastWriteTime.Date;
                else if (key.Contains("fec de ultimo acceso") || key.Contains("ultimo acceso"))
                    valorAComparar = info.LastAccessTime.Date;
                else continue;

                if (valorAComparar < desde.Value || valorAComparar > hasta.Value) return false;
            }
            return true;
        }

        public bool CumpleFiltrosFechaCarpeta(DirectoryInfo info)
        {
            if (filtros.Fechas == null || filtros.Fechas.Count == 0) return true;

            foreach (var kv in filtros.Fechas)
            {
                string nombreFiltro = kv.Key ?? "";
                DateTime? desde = kv.Value.Desde?.Date;
                DateTime? hasta = kv.Value.Hasta?.Date;

                if (!desde.HasValue || !hasta.HasValue) continue;

                string key = (nombreFiltro ?? "").ToLower()
                    .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("ü", "u")
                    .Replace(".", "").Replace("  ", " ").Trim();

                DateTime valorAComparar;
                if (key.Contains("creacion") || key.Contains("archivos creados") || key.Contains("antiguedad"))
                    valorAComparar = info.CreationTime.Date;
                else if (key.Contains("fec de ultima edicion") || key.Contains("modificado"))
                    valorAComparar = info.LastWriteTime.Date;
                else if (key.Contains("fec de ultimo acceso") || key.Contains("ultimo acceso"))
                    valorAComparar = info.LastAccessTime.Date;
                else continue;

                if (valorAComparar < desde.Value || valorAComparar > hasta.Value) return false;
            }
            return true;
        }

        public bool CumpleFiltroContenido(string ruta)
        {
            if (!filtros.FiltrarContenido || string.IsNullOrEmpty(filtros.TextoContenido)) return true;

            try
            {
                string ext = Path.GetExtension(ruta).ToLowerInvariant();
                string busqueda = filtros.TextoContenido;
                var comparacion = filtros.IgnorarMayusculas ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

                string[] extTexto = { ".txt", ".cs", ".py", ".sql", ".json", ".xml", ".html", ".log", ".csv", ".ini", ".md" };
                if (extTexto.Contains(ext))
                {
                    foreach (var linea in File.ReadLines(ruta))
                    {
                        if (linea.IndexOf(busqueda, comparacion) >= 0) return true;
                    }
                    return false;
                }

                if (ext == ".pdf")
                {
                    using (PdfReader pdfReader = new PdfReader(ruta))
                    using (PdfDocument pdfDoc = new PdfDocument(pdfReader))
                    {
                        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                        {
                            string textoPagina = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));
                            if (textoPagina.IndexOf(busqueda, comparacion) >= 0) return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        public bool CumpleFiltrosSeguridad(FileInfo info)
        {
            if (filtros.Seguridad == null || filtros.Seguridad.Count == 0) return true;
            if (!filtros.Seguridad.Values.Any(v => v)) return true;

            var attr = info.Attributes;

            if (SeguridadActiva("Archivos bloqueados por el sistema (solo lectura)") && !attr.HasFlag(FileAttributes.ReadOnly)) return false;
            if (SeguridadActiva("Archivos ocultos") && !attr.HasFlag(FileAttributes.Hidden)) return false;
            if (SeguridadActiva("Archivos cifrados") && !attr.HasFlag(FileAttributes.Encrypted)) return false;

            if (SeguridadActiva("Archivos protegidos por contraseña"))
            {
                if (info.Extension.ToLowerInvariant() != ".pdf") return false;
                if (!PdfTienePassword(info.FullName)) return false;
            }

            return true;
        }

        private bool PdfTienePassword(string rutaPdf)
        {
            try
            {
                using (var reader = new PdfReader(rutaPdf))
                using (var pdf = new PdfDocument(reader)) { return false; }
            }
            catch (Exception ex)
            {
                string msg = (ex.Message ?? "").ToLowerInvariant();
                return msg.Contains("password") || msg.Contains("encrypted") || msg.Contains("encryption");
            }
        }

        private bool SeguridadActiva(string nombre)
        {
            if (filtros.Seguridad == null) return false;
            return filtros.Seguridad.TryGetValue(nombre, out bool act) && act;
        }

        public bool CumpleFiltrosMetadatosDocumento(string rutaArchivo)
        {
            try
            {
                bool hayFiltrosMeta = filtros.FiltrarAutor || filtros.FiltrarTitulo || filtros.FiltrarAplicacion || filtros.FiltrarPaginas;
                if (!hayFiltrosMeta) return true;

                string ext = (Path.GetExtension(rutaArchivo) ?? "").ToLowerInvariant();
                bool soportaMetadatosDocumento = ext == ".pdf" || ext == ".docx" || ext == ".doc" || ext == ".pptx" || ext == ".ppt" ||
                                                 ext == ".xlsx" || ext == ".xls" || ext == ".rtf" || ext == ".odt" || ext == ".ods" || ext == ".odp";

                if ((filtros.FiltrarAutor || filtros.FiltrarTitulo || filtros.FiltrarPaginas) && !soportaMetadatosDocumento) return false;

                string filtroAutor = (filtros.AutorDocumento ?? "").Trim();
                string filtroTitulo = (filtros.TituloDocumento ?? "").Trim();
                string filtroApp = (filtros.AplicacionGeneradora ?? "").Trim();
                int? filtroPag = filtros.CantidadPaginas;

                if (filtros.FiltrarAutor && string.IsNullOrWhiteSpace(filtroAutor)) return false;
                if (filtros.FiltrarTitulo && string.IsNullOrWhiteSpace(filtroTitulo)) return false;
                if (filtros.FiltrarAplicacion && string.IsNullOrWhiteSpace(filtroApp)) return false;
                if (filtros.FiltrarPaginas && !filtroPag.HasValue) return false;

                string autor = "", titulo = "", app = "";
                int? paginas = null;

                if (ext == ".pdf")
                {
                    try
                    {
                        using (var reader = new PdfReader(rutaArchivo))
                        using (var pdf = new PdfDocument(reader))
                        {
                            var info = pdf.GetDocumentInfo();
                            autor = info.GetAuthor() ?? "";
                            titulo = info.GetTitle() ?? "";
                            app = ((info.GetCreator() ?? "") + " " + (info.GetProducer() ?? "")).Trim();
                            paginas = pdf.GetNumberOfPages();
                        }
                    }
                    catch { }
                }
                else
                {
                    using (var shellFile = ShellFile.FromFilePath(rutaArchivo))
                    {
                        var autores = shellFile.Properties.System.Author.Value;
                        if (autores != null && autores.Length > 0) autor = string.Join(" ", autores);
                        titulo = shellFile.Properties.System.Title.Value ?? "";
                        app = ((shellFile.Properties.System.ApplicationName.Value ?? "") + " " + (shellFile.Properties.System.SoftwareUsed.Value ?? "")).Trim();
                        paginas = shellFile.Properties.System.Document.PageCount.Value;
                    }
                }

                if (filtros.FiltrarAutor && (string.IsNullOrWhiteSpace(autor) || autor.IndexOf(filtroAutor, StringComparison.OrdinalIgnoreCase) < 0)) return false;
                if (filtros.FiltrarTitulo && (string.IsNullOrWhiteSpace(titulo) || titulo.IndexOf(filtroTitulo, StringComparison.OrdinalIgnoreCase) < 0)) return false;
                if (filtros.FiltrarAplicacion && (string.IsNullOrWhiteSpace(app) || app.IndexOf(filtroApp, StringComparison.OrdinalIgnoreCase) < 0)) return false;
                if (filtros.FiltrarPaginas && (!paginas.HasValue || paginas.Value != filtroPag.Value)) return false;

                return true;
            }
            catch { return false; }
        }

        public bool CumpleFiltrosMetadatosImagen(string rutaArchivo)
        {
            try
            {
                bool hayFiltrosImg = filtros.FiltrarResolucion || filtros.FiltrarFechaImagen || filtros.FiltrarModelo || filtros.FiltrarGPS;
                if (!hayFiltrosImg) return true;

                string ext = (Path.GetExtension(rutaArchivo) ?? "").ToLowerInvariant();
                bool esImagen = ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".tiff" || ext == ".gif" || ext == ".webp";

                if (!esImagen) return false;

                using (var shellFile = ShellFile.FromFilePath(rutaArchivo))
                {
                    if (filtros.FiltrarResolucion)
                    {
                        var anchoProp = shellFile.Properties.System.Image.HorizontalSize.Value;
                        var altoProp = shellFile.Properties.System.Image.VerticalSize.Value;
                        if (anchoProp == null || altoProp == null) return false;
                        if (filtros.ResolucionAncho.HasValue && anchoProp.Value != (uint)filtros.ResolucionAncho.Value) return false;
                        if (filtros.ResolucionAlto.HasValue && altoProp.Value != (uint)filtros.ResolucionAlto.Value) return false;
                    }

                    if (filtros.FiltrarFechaImagen)
                    {
                        DateTime? fechaReferencia = shellFile.Properties.System.Photo.DateTaken.Value ?? shellFile.Properties.System.DateCreated.Value ?? shellFile.Properties.System.DateModified.Value;
                        if (!fechaReferencia.HasValue) return false;

                        DateTime fecha = fechaReferencia.Value.Date;
                        if (filtros.FechaImagenDesde.HasValue && fecha < filtros.FechaImagenDesde.Value.Date) return false;
                        if (filtros.FechaImagenHasta.HasValue && fecha > filtros.FechaImagenHasta.Value.Date) return false;
                    }

                    if (filtros.FiltrarModelo)
                    {
                        string modeloReal = shellFile.Properties.System.Photo.CameraModel.Value ?? "";
                        if (string.IsNullOrWhiteSpace(modeloReal)) return false;

                        bool tieneTextoCamara = !string.IsNullOrWhiteSpace(filtros.ModeloCamara);
                        bool tieneTextoMovil = !string.IsNullOrWhiteSpace(filtros.ModeloMovil);

                        if (tieneTextoCamara || tieneTextoMovil)
                        {
                            bool coincideCamara = tieneTextoCamara && modeloReal.IndexOf(filtros.ModeloCamara, StringComparison.OrdinalIgnoreCase) >= 0;
                            bool coincideMovil = tieneTextoMovil && modeloReal.IndexOf(filtros.ModeloMovil, StringComparison.OrdinalIgnoreCase) >= 0;
                            if (!coincideCamara && !coincideMovil) return false;
                        }
                    }

                    if (filtros.FiltrarGPS)
                    {
                        var propLat = shellFile.Properties.GetProperty("System.GPS.LatitudeDecimal");
                        var propLon = shellFile.Properties.GetProperty("System.GPS.LongitudeDecimal");
                        double? latVal = propLat?.ValueAsObject as double?;
                        double? lonVal = propLon?.ValueAsObject as double?;

                        if (latVal == null || lonVal == null) return false;
                        if (filtros.Latitud.HasValue && Math.Abs(latVal.Value - filtros.Latitud.Value) > 0.001) return false;
                        if (filtros.Longitud.HasValue && Math.Abs(lonVal.Value - filtros.Longitud.Value) > 0.001) return false;
                    }
                }
                return true;
            }
            catch { return false; }
        }
    }
}