using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFORM.Ejecucion
{
    class TablaPreguntas
    {
        Dictionary<String,Pregunta> t;
        public TablaPreguntas()
        {
            t = new Dictionary<String,Pregunta>();
        }

        public void insertar(String key, Pregunta pregunta,String clase){
            t.Add(key.ToLower(),pregunta);
            Console.WriteLine("Se inserto una nueva pregunta: "+key+" de la clase : "+clase); 
        }

        public Pregunta retornaPregunta(String nombre)
        {
            if (existePregunta(nombre))
            {
                return t[nombre.ToLower()];
            }
            else {
                return null;
            }  
        }


        public Boolean existePregunta(String key)
        { // esta comprobacion hacerla en la analizador para hacer la validacion ahi 
            return t.Keys.Contains(key.ToLower());
        }

        public Dictionary<String, Pregunta> getT()
        {
            return t;
        }
    }
}
