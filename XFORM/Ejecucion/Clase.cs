using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFORM.Ejecucion
{
    class Clase
    {
        ParseTreeNode cuerpo;
        ParseTreeNode her;
        String nombre;
        String visibilidad;
        String padre;
        public Clase(String name, String vis,String papa, ParseTreeNode heren,ParseTreeNode body)
        {
            this.nombre = name;
            this.visibilidad = vis;
            this.cuerpo = body;
            this.her = heren;
            this.padre = papa;
        }

        public String Padre
        {
            get { return padre; }
            set { padre = value; }
        }

        public ParseTreeNode Her
        {
            get { return her; }
            set { her = value; }
        }

        public ParseTreeNode Cuerpo
        {
            get { return cuerpo; }
            set { cuerpo = value; }
        }

        

        public String Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
       

        public String Visibilidad
        {
            get { return visibilidad; }
            set { visibilidad = value; }
        }

        
    }
}
