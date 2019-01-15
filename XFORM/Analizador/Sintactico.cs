using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;// importamos las herramientas de irony
using Irony.Parsing;
using Irony.Ast;



namespace XFORM.Analizador
{
    class Sintactico: Grammar
    {
        //private List<Datos.Error> listaErrs;

        String nombreDoc;
        public Sintactico(String NomArchivo)
        {
            this.nombreDoc = NomArchivo;
        }
        public ParseTreeNode analizar(String entrada) {

            Gramatica gramatica = new Gramatica();
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser parser = new Parser(lenguaje);
            ParseTree arbol = parser.Parse(entrada);
            ParseTreeNode raiz=arbol.Root;

            if (raiz == null)
            {
                //Datos.Error error;

                for (int i = 0; i < arbol.ParserMessages.Count(); i++) {
                    String error = arbol.ParserMessages.ElementAt(i).Level.ToString() +
                                    "  " + arbol.ParserMessages.ElementAt(i).Message +
                                    " L: " + (arbol.ParserMessages.ElementAt(i).Location.Line + 1) +
                                    " C: " + (arbol.ParserMessages.ElementAt(i).Location.Column + 1) +
                                    " DOC: " + nombreDoc;
                    Form1.listaErrores.Add(error);
                    //==========Notificamos en consola
                    Console.WriteLine(arbol.ParserMessages.ElementAt(i).Level.ToString());
                    Console.Write("  "+arbol.ParserMessages.ElementAt(i).Message);
                    Console.Write(" L: " + (arbol.ParserMessages.ElementAt(i).Location.Line+1));
                    Console.Write(" C: " + (arbol.ParserMessages.ElementAt(i).Location.Column+1));
                    Console.WriteLine(" DOC: " + nombreDoc);
                }

                    return null; // la cadena es invalida no se logra el analisis 
            }
            else 
            {
               
                for (int i = 0; i < arbol.ParserMessages.Count(); i++)
                {
                    String error = arbol.ParserMessages.ElementAt(i).Level.ToString() +
                                    "  " + arbol.ParserMessages.ElementAt(i).Message +
                                    " L: " + (arbol.ParserMessages.ElementAt(i).Location.Line + 1) +
                                    " C: " + (arbol.ParserMessages.ElementAt(i).Location.Column + 1) +
                                    " DOC: " + nombreDoc;
                    Form1.listaErrores.Add(error);
                    //==========Notificamos en consola
                    Console.WriteLine(arbol.ParserMessages.ElementAt(i).Level.ToString());
                    Console.Write("  " + arbol.ParserMessages.ElementAt(i).Message);
                    Console.Write(" L: " + (arbol.ParserMessages.ElementAt(i).Location.Line + 1));
                    Console.Write(" C: " + (arbol.ParserMessages.ElementAt(i).Location.Column + 1));
                    Console.WriteLine(" DOC: " + nombreDoc);
                }
                return raiz; // la cadena es valida si se realizo el analisis 
            }


        }
    }
}
