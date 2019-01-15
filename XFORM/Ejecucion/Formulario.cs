using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFORM.Ejecucion
{
    class Formulario
    {
        public String nombre;
        public ParseTreeNode cuerpo;

        public Formulario(String nombre, ParseTreeNode cuerpo)
        {
            this.nombre = nombre;
            this.cuerpo = cuerpo;
        }
    }
}
