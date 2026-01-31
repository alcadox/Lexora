using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexora.Pantallas.Menu.Filtros
{
    public class ClaseFiltros
    {
        // se guarda: < "documento" , true > por ejemplo
        public Dictionary<string, bool> TiposArchivo { get; set; }
        // guarda el diccionario sin formatear
        public Dictionary<string, bool> TiposArchivoSinFormatear { get; set; }
        public Dictionary<string, (DateTime? Desde, DateTime? Hasta)> Fechas { get; set; }


        public ClaseFiltros()
        {
            TiposArchivo = new Dictionary<string, bool>();
            TiposArchivoSinFormatear = new Dictionary<string, bool>();
            
            //filtros de tiempo
            Fechas = new Dictionary<string, (DateTime? Desde, DateTime? Hasta)>();

        }

        public void FormatearTipoArchivo()
        {
            // si el diccionario está vacio ignorar
            if (TiposArchivo.Count == 0) return;

            // recorrer el diccionario y transformarlo a un formato valido,
            // ya que se guarda estilo <Documentos (pdf, docx...) - true> y debe de ser < .docx - true>, <.pdf - true><.pptx - true> y así sucesivamente

            var tiposFormateados = new Dictionary<string, bool>();

            // funciona de la siguiente manera: recorre el diccionario y si contiene "Documento" / "Imagenes" / "Audio"
            // "Videos" / "Comprimidos" / "Instaladores" / "Codigo" / "Cifrado" agrega las extensiones correspondientes dependiendo del tipo
            // Ejemplo: si encuentra "Documentos" agrega .pdf, .docx, .doc, .pptx, .xlsx, .txt al nuevo diccionario con el mismo valor (true/false)
            foreach (var item in TiposArchivo)
            {
                // Normalizamos la llave para que no importe mayúsculas/minúsculas ni tildes básicas
                // Usamos ToLower() y reemplazos simples para las tildes más comunes
                string keyNormalizada = item.Key.ToLower()
                    .Replace("á", "a")
                    .Replace("é", "e")
                    .Replace("í", "i")
                    .Replace("ó", "o")
                    .Replace("ú", "u");

                if (keyNormalizada.Contains("documento"))
                {
                    tiposFormateados[".pdf"] = item.Value;
                    tiposFormateados[".docx"] = item.Value;
                    tiposFormateados[".doc"] = item.Value;
                    tiposFormateados[".pptx"] = item.Value;
                    tiposFormateados[".ppt"] = item.Value;
                    tiposFormateados[".xlsx"] = item.Value;
                    tiposFormateados[".xls"] = item.Value;
                    tiposFormateados[".csv"] = item.Value;
                    tiposFormateados[".txt"] = item.Value;
                    tiposFormateados[".rtf"] = item.Value;
                    tiposFormateados[".odt"] = item.Value;
                    tiposFormateados[".ods"] = item.Value;
                    tiposFormateados[".odp"] = item.Value;
                    tiposFormateados[".epub"] = item.Value;
                    tiposFormateados[".pages"] = item.Value;
                    tiposFormateados[".numbers"] = item.Value;
                    tiposFormateados[".key"] = item.Value;
                }
                else if (keyNormalizada.Contains("imagen"))
                {
                    tiposFormateados[".jpg"] = item.Value;
                    tiposFormateados[".jpeg"] = item.Value;
                    tiposFormateados[".png"] = item.Value;
                    tiposFormateados[".gif"] = item.Value;
                    tiposFormateados[".bmp"] = item.Value;
                    tiposFormateados[".svg"] = item.Value;
                    tiposFormateados[".webp"] = item.Value;
                    tiposFormateados[".tiff"] = item.Value;
                    tiposFormateados[".tif"] = item.Value;
                    tiposFormateados[".ico"] = item.Value;
                    tiposFormateados[".raw"] = item.Value;
                    tiposFormateados[".heic"] = item.Value;
                    tiposFormateados[".psd"] = item.Value;
                    tiposFormateados[".ai"] = item.Value;
                }
                else if (keyNormalizada.Contains("audio"))
                {
                    tiposFormateados[".mp3"] = item.Value;
                    tiposFormateados[".wav"] = item.Value;
                    tiposFormateados[".flac"] = item.Value;
                    tiposFormateados[".aac"] = item.Value;
                    tiposFormateados[".ogg"] = item.Value;
                    tiposFormateados[".m4a"] = item.Value;
                    tiposFormateados[".wma"] = item.Value;
                    tiposFormateados[".aiff"] = item.Value;
                    tiposFormateados[".mid"] = item.Value;
                    tiposFormateados[".opus"] = item.Value;
                }
                else if (keyNormalizada.Contains("video"))
                {
                    tiposFormateados[".mp4"] = item.Value;
                    tiposFormateados[".avi"] = item.Value;
                    tiposFormateados[".mkv"] = item.Value;
                    tiposFormateados[".mov"] = item.Value;
                    tiposFormateados[".wmv"] = item.Value;
                    tiposFormateados[".flv"] = item.Value;
                    tiposFormateados[".webm"] = item.Value;
                    tiposFormateados[".mpeg"] = item.Value;
                    tiposFormateados[".m4v"] = item.Value;
                    tiposFormateados[".3gp"] = item.Value;
                }
                else if (keyNormalizada.Contains("comprimido"))
                {
                    tiposFormateados[".zip"] = item.Value;
                    tiposFormateados[".rar"] = item.Value;
                    tiposFormateados[".7z"] = item.Value;
                    tiposFormateados[".tar"] = item.Value;
                    tiposFormateados[".gz"] = item.Value;
                    tiposFormateados[".iso"] = item.Value;
                    tiposFormateados[".bz2"] = item.Value;
                    tiposFormateados[".xz"] = item.Value;
                }
                else if (keyNormalizada.Contains("instalador") || keyNormalizada.Contains("ejecutable"))
                {
                    tiposFormateados[".exe"] = item.Value;
                    tiposFormateados[".msi"] = item.Value;
                    tiposFormateados[".bin"] = item.Value;
                    tiposFormateados[".apk"] = item.Value;
                    tiposFormateados[".dmg"] = item.Value;
                    tiposFormateados[".appx"] = item.Value;
                    tiposFormateados[".deb"] = item.Value;
                    tiposFormateados[".rpm"] = item.Value;
                    tiposFormateados[".sh"] = item.Value;
                    tiposFormateados[".bat"] = item.Value;
                }
                else if (keyNormalizada.Contains("codigo") || keyNormalizada.Contains("programacion"))
                {
                    tiposFormateados[".cs"] = item.Value;
                    tiposFormateados[".java"] = item.Value;
                    tiposFormateados[".py"] = item.Value;
                    tiposFormateados[".js"] = item.Value;
                    tiposFormateados[".ts"] = item.Value;
                    tiposFormateados[".html"] = item.Value;
                    tiposFormateados[".css"] = item.Value;
                    tiposFormateados[".cpp"] = item.Value;
                    tiposFormateados[".c"] = item.Value;
                    tiposFormateados[".php"] = item.Value;
                    tiposFormateados[".swift"] = item.Value;
                    tiposFormateados[".json"] = item.Value;
                    tiposFormateados[".xml"] = item.Value;
                    tiposFormateados[".sql"] = item.Value;
                    tiposFormateados[".go"] = item.Value;
                    tiposFormateados[".rb"] = item.Value;
                    tiposFormateados[".kt"] = item.Value;
                    tiposFormateados[".sh"] = item.Value;

                }
                else if (keyNormalizada.Contains("cifrado"))
                {
                    tiposFormateados[".enc"] = item.Value;
                    tiposFormateados[".crypt"] = item.Value;
                    tiposFormateados[".aes"] = item.Value;
                    tiposFormateados[".gpg"] = item.Value;
                    tiposFormateados[".key"] = item.Value;
                    tiposFormateados[".pfx"] = item.Value;
                }
            }

            // reemplazar el diccionario original con el formateado
            TiposArchivo.Clear();
            TiposArchivo = tiposFormateados;

        }
    }
}