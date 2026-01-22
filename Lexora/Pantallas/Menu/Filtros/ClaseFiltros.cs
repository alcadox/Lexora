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

        public ClaseFiltros()
        {
            TiposArchivo = new Dictionary<string, bool>();
        }
    }
}
