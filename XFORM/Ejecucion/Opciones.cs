using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFORM.Ejecucion
{
    class Opciones
    {
        public List<List<retorno>> elementos;

        public Opciones() {
            this.elementos = new List<List<retorno>>();
        }

        public void insertar(List<retorno> valores) {
            elementos.Add(valores);
        }


        public retorno obtner(int ind_elment, int ind_val) {
            //aca buscamos por indices
           if(ind_elment>-1 && ind_elment < elementos.Count){
               if (ind_val > -1 && ind_val < elementos[ind_elment].Count) {
                   return elementos[ind_elment].ElementAt(ind_val);
               }
               return null; 
           }
            return null;
        }

        public retorno buscar(retorno valor, int ind_val) {
            // aca buscamos por valor y el segundo por indice
            foreach (List<retorno> valores in elementos)
            {
                foreach (retorno ret in valores) {
                    if (ret.valor.Equals(valor.valor)) {
                        if (ind_val > 0 && ind_val < valores.Count) {
                            return valores.ElementAt(ind_val);
                        }
                        return null;
                    }
                }
            }
            return null;
        }


    }
}
