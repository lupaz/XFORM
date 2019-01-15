using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFORM.Ejecucion
{
    class Pregunta
    {
        public String etiqueta;
        public String sugerir;
        public String nombre;
        public String respuesta;
        public String tipo;
        public String requeridoMsg;
        public String requerido;
        public ParseTreeNode cuerpo;
        public List<Parametro> Parametros;
        public Funcion funcion;

        public Pregunta(String nombre, ParseTreeNode Cuerpo) {
            this.nombre = nombre;
            this.cuerpo = Cuerpo;
            this.etiqueta = "";
            this.sugerir = "";
            this.respuesta = "";
            this.tipo = "";
            this.requeridoMsg = "";
            this.requerido = "";
            this.Parametros = new List<Parametro>();
        }

        public void addParametro(String nombre, String tipo) //Vamos a enviar lo parametros de esta forma tipo,nombre
        {
            Parametros.Add(new Parametro(nombre.ToLower(), tipo.ToLower()));
        }



    }
}
