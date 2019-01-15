using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFORM.Ejecucion
{
    class TablaSimbolos
    {

        public String Nivel;
        public String Tipo;
        public Boolean retorno;
        public Boolean detener;
        public Boolean continuar;


        Dictionary<String,Simbolo> t;
        public TablaSimbolos(String nivel, String tipo, Boolean retorno, Boolean detener,Boolean continuar)
        {
            t = new Dictionary<String,Simbolo>();
            this.Tipo = tipo;
            this.Nivel = nivel;
            this.retorno = retorno;
            this.detener = detener;
            this.continuar = continuar;
        }

        public void insertar(String nombre, Simbolo simbolo,String clase){
            if (!existeSimbolo(nombre))
            {
                t.Add(nombre.ToLower(), simbolo);
                Console.WriteLine("Se inserto una nueva variable: " + nombre + " Nivel : " + Nivel + " Ambito : " + Tipo + " Clase: " + clase);
            }
        }

        public Simbolo retornaSimbolo(String nombre)
        {
            if (existeSimbolo(nombre))
            {
                return t[nombre.ToLower()];
            }
            else {
                return null;
            }
        }

        public Boolean existeSimbolo(String Nombre)
        {
            return t.Keys.Contains(Nombre.ToLower());
        }

        public void clear() {
            t.Clear();
        }

        public bool vacio() {
            if (t.Count == 0) {
                return true;
            }
            return false;
        }

        public Dictionary<String,Simbolo> getT()
        {
            return t;
        }
    }
}
