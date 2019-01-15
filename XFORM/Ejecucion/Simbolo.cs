using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFORM.Ejecucion
{
    class Simbolo
    {

        public String Ambito;
        public String Nombre;
        public Object Valor;
        public String Tipo;
        private String tipoObjeto;
        public String Linea;
        public String Columna;
        private String visibilidad;
        public List<int> dimenciones;

       
        public Simbolo(String ambito,String nombre,Object valor, String tipo,String linea,String columna){
            this.Ambito =ambito;
            this.Nombre=nombre;
            this.Tipo=tipo;
            this.Valor=valor;
            this.Linea=linea;
            this.Columna =columna;
            visibilidad = "";
            tipoObjeto = "";
        }

        public String TipoObjeto
        {
            get { return tipoObjeto; }
            set { tipoObjeto = value; }
        }

        public String Visibilidad
        {
            get { return visibilidad; }
            set { visibilidad = value; }
        }

        public void iniciaDims(){
            dimenciones = new List<int>();
        }


    }
}
