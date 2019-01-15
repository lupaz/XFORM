using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace XFORM.Ejecucion
{

    class Parametro
    {
        private String nombre;
        private String tipo;

        public Parametro(String nombre, String tipo)
        {
            this.nombre = nombre;
            this.tipo = tipo;
        }

        public String Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        public String Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }
    }

    class Funcion
    {
        public String Nombre;
        public String Visibilidad;
        public String Tipo;
        public List<Parametro> Parametros;
        public ParseTreeNode Cuerpo;
        public String key; // aca vamos a generar la llave de la funcion para el polimorfismo.

        public Funcion(String visibilidad,String tipo, String nombre, ParseTreeNode cuerpo){
            this.Visibilidad = visibilidad.ToLower();
            this.Nombre= nombre.ToLower();
            this.Cuerpo= cuerpo;
            this.Tipo = tipo.ToLower();
            Parametros = new List<Parametro>();
        }

        public int numPars()
        {
            return Parametros.Count();
        }

        public void addParametro(String  nombre, String tipo) //Vamos a enviar lo parametros de esta forma tipo,nombre
        {
            Parametros.Add(new Parametro(nombre.ToLower(),tipo.ToLower()));
        }

        public Boolean exixtePar(String nombre){
            foreach (Parametro par in Parametros) {
                if(par.Nombre.Equals(nombre.ToLower())){
                    return true;
                }
            }
            return false;
        }


        public String generar_llave() {//compuesta por nombre_tipo_tipoPar1...tipoParn
            key = this.Nombre + "_";
            foreach (Parametro par in Parametros)
            {
                key += par.Tipo;
            }
            return key;
        }

    }
}
