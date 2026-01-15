using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lexora.Pantallas.Menu.Filtros
{
    public partial class MainFiltros : Form
    {
        public MainFiltros()
        {
            InitializeComponent();
        }

        private void botonAplicar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void botonCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
