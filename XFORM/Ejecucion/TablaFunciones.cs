using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFORM.Ejecucion
{
    class TablaFunciones
    {

        Dictionary<String,Funcion> t;
        public TablaFunciones()
        {
            t = new Dictionary<String,Funcion>();
        }

        public void insertar(String key, Funcion funcion,String clase){
            t.Add(key.ToLower(), funcion);
            Console.WriteLine("Se inserto un nuevo metodo/funcion/constr : "+key+" de la clase : "+clase); 
        }

        public Funcion retornaFuncion(String nombre)
        {
            if (existeFuncion(nombre))
            {
                return t[nombre.ToLower()];
            }
            else {
                return null;
            }
            
        }


        public Boolean existeFuncion(String key)
        { // esta comprobacion hacerla en la analizador para hacer la validacion ahi 
            return t.Keys.Contains(key.ToLower());
        }
    }
}
