using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFORM.Analizador;

namespace XFORM.Ejecucion
{
    class ListaClases
    {


        List<Clase> Clases;

        public ListaClases() {
            Clases = new List<Clase>();
        }

        public void insertar(Clase clas)
        {
            Clases.Add(clas);
            Console.WriteLine("Se inserto una nueva clase: " +clas.Nombre);
        }

        public Boolean existeClase(String Nombre)
        { // esta comprobacion hacerla en la analizador para hacer la validacion ahi 

            foreach (Clase clas in Clases)
            {
                if (clas.Nombre.ToLower().Equals(Nombre.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        public Clase retornaClasePrincipal() {
            foreach (Clase clas in Clases)
            {
                foreach (ParseTreeNode hijo in clas.Cuerpo.ChildNodes) {
                    if (hijo.Term.Name.Equals(Cadena.PRINCIPAL)) {
                        Console.WriteLine("Se retorno la clase principal -> "+clas.Nombre);
                        return clas;
                    }
                }
            }
            return null;
        }

        public Clase retornaClase(String nombre)
        {
            foreach (Clase clas in Clases)
            {
                if (clas.Nombre.ToLower().Equals(nombre.ToLower())) {
                    return clas;
                }
            }
            return null;
        }
    }
}
