using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFORM.Graficar
{
    class DibujaArbol
    {

        private static int index;
        private static string cadenaDot;


        public DibujaArbol() {
            index = 0;
        }

        public string generarDot(ParseTreeNode raiz) {
            cadenaDot = "digraph G {\n";
            cadenaDot+="nodo"+index.ToString()+"[label=\""+rendeer(raiz.ToString())+"\"];\n";
            index += 1;
            recorreAST("nodo0", raiz);
            cadenaDot += "}";
            index = 0;
            return cadenaDot;
        }

        private void recorreAST(String nombreNodo, ParseTreeNode nodo) {
            foreach (ParseTreeNode hijo in nodo.ChildNodes) {
                string nomHijo = "nodo" + index.ToString();
                cadenaDot += nomHijo + "[label=\"" + rendeer(hijo.ToString())+ "\"];\n";
                cadenaDot+= nombreNodo+"->"+nomHijo+";\n";
                index += 1;
                recorreAST(nomHijo, hijo);
            }
        }

        public void generarImg(ParseTreeNode raiz,String nombreImg) {
            if (raiz != null)
            {
                string tmp = generarDot(raiz);
                string[] cad = { tmp }; //C:\Users\Luis\Desktop\AST
                System.IO.File.WriteAllLines(@"C:\Users\Luis\Desktop\AST\"+nombreImg+".dot", cad);
                ejecutarCmd("dot -Tpng " + "\"" + @"C:\Users\Luis\Desktop\AST\"+nombreImg+".dot" + "\" " + "-o" + " \"" + @"C:\Users\Luis\Desktop\AST\"+nombreImg+".png" + "\"");
                //Image imagen= Image.FromFile(@"C:\Users\Luis\Documents\Visual Studio 2013\Projects\PracticaIrony\PracticaIrony\ImgsAST\ast.png");
                Console.WriteLine("Imagen creada correctamente.");
            }
            else {
                Console.WriteLine("No se genero la imagen.");
            }
             
        }

        private static string rendeer(String cadena) {
            cadena = cadena.Replace("\\", "\\\\");
            cadena = cadena.Replace("\"", "\\\"");
            return cadena;
        }

        static void ejecutarCmd(string comando)
        {
            
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = "cmd";
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.CreateNoWindow = false;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            proc.StandardInput.WriteLine(comando);
            proc.StandardInput.Flush();
            proc.StandardInput.Close();
            proc.Close(); 
        }


    }
}
