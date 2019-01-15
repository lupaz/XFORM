using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFORM.Ejecucion
{
    class VFO
    {

        // Contendra la pila de ejecucion y lista de funiones por cada clase
        public Stack<TablaSimbolos> pilaSimbolos;
        public TablaFunciones funciones;
        public TablaSimbolos global;
        public Clase clase;
        public bool super_agregados = false;

        public VFO( TablaSimbolos tab) {
            funciones = new TablaFunciones();
            pilaSimbolos = new Stack<TablaSimbolos>();
            global = tab;
            pilaSimbolos.Push(global);
        }

    }
}
