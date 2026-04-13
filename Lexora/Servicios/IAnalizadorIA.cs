using System.Threading.Tasks;

namespace Lexora.Servicios
{
    // Interfaz que define el comportamiento de la IA en el futuro
    public interface IAnalizadorIA
    {
        // Propiedad para activar/desactivar la IA sobre la marcha
        bool Habilitado { get; set; }

        // Método que en el futuro analizará el archivo y devolverá etiquetas/resumen
        Task<string> ObtenerEtiquetaInteligenteAsync(string rutaArchivo);
    }
}