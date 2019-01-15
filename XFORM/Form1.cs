using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XFORM.Analizador;
using XFORM.Graficar;
using XFORM.Ejecucion;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace XFORM
{

    public partial class Form1 : Form
    {

        public static List<String> listaErrores = new List<String>();
        public static String ruta_genesis = @"C:\Users\Luis\Desktop\AST"; //para los imports de las clases
        Sintactico analisis;
        DibujaArbol graf;
        int columna;
        int fila;
        int posicion;
        //ParseTreeNode raiz;
        public Form1()
        {
            InitializeComponent();
            fila = 0;
            columna = 0;
        }

        private void XFORM_Load(object sender, EventArgs e)
        {
            graf = new DibujaArbol();
            analisis = new Sintactico("traduccion.xform");
            treeView1.AfterSelect += treeView1_AfterSelect;
        }


        private void analizar_entrada() {
            string ruta = @"C:\Users\Luis\Desktop\AST\prueba_2.xform";
           // try {
                System.IO.StreamReader sr = new System.IO.StreamReader(ruta);
                analisis = new Sintactico("traduccion.xform");
                ParseTreeNode raiz=analisis.analizar(sr.ReadToEnd());
                sr.Close();
                //manipulamos la raiz
                EjecucionXform ejecutar = new EjecucionXform(consola);
                ejecutar.capturarClases(raiz);
                ejecutar.capturarClaseImport(raiz);
                ejecutar.capturarClasePrincipal();
                graf.generarImg(raiz, "prue_2");
                Console.WriteLine("Archivo Cargado correctamente.");
            //}catch(Exception e){
                //Console.WriteLine(e.Message);
            //}       
        }

        private void button1_Click(object sender, EventArgs e)
        {
            analizar_entrada();
        }

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string titulo = "Nuevo " + (tabs.TabCount + 1);
            TabPage tab = new TabPage(titulo);
            //Lineas lineas = new Lineas(timer1);
            //System.Windows.Forms.Panel pane=lineas.getPane();
            RichTextBox rich = new RichTextBox();
            rich.SetBounds(0, 0, tab1.Bounds.Width, tab1.Bounds.Height);
            //PictureBox pic = new PictureBox();
            //pic.SetBounds(0, 0, 30, tab1.Bounds.Height);
            //pane.Controls.Add(pic);
            tab.Controls.Add(rich);
            tabs.TabPages.Add(tab);
            notificar("Se abrio una nueva pestaña.");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            actualizarColyFil();
        }

        private void actualizarColyFil()
        {
            TabPage tab;
            RichTextBox rich;
            Point point = new Point(0, 0);
            tab = (TabPage)tabs.GetControl(tabs.SelectedIndex);
            rich = (RichTextBox)tab.GetChildAtPoint(point);
            posicion = rich.SelectionStart;
            fila = rich.GetLineFromCharIndex(posicion)+1;
            columna = posicion - rich.GetFirstCharIndexOfCurrentLine()+1;
            numFila.Text = Convert.ToString(fila);
            numCol.Text = Convert.ToString(columna);
        }


        private void notificar(string mensaje)
        {
            consola.AppendText(">>: " + mensaje + "\n");
        }


        private void cerrarPestañaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Se elimnara la pesteña actual?", "Cerrar Pestaña", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (tabs.TabCount > 1)
                {
                    tabs.TabPages.RemoveAt(tabs.SelectedIndex);
                }
                else
                {
                    notificar("No puede eliminar la ultima pestaña.");
                }

            }
        }

        private void cargar2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "XFORM (*.xform)|*.xform";
            openFileDialog1.Title = "Seleccione un archivo:";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ruta = openFileDialog1.FileName;
                ruta = ruta.Replace(openFileDialog1.SafeFileName, "");
                ruta_genesis=ruta; // seteo la ruta para cargar los archivo de importar
                System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog1.FileName);
                TabPage tab;
                RichTextBox rich;
                Point point = new Point(0, 0);
                tab = (TabPage)tabs.GetControl(tabs.SelectedIndex);
                tab.Text = openFileDialog1.SafeFileName;
                rich = (RichTextBox)tab.GetChildAtPoint(point);
                rich.Text = sr.ReadToEnd();
                sr.Close();
                notificar("Archivo Cargado correctamente.");
            }
            else
            {
                notificar("No se cargo ningun archivo.");
            }
        }

        private void cargarArchivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "XFORM (*.xform)|*.xform";
            openFileDialog1.Title = "Seleccione un archivo:";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ruta = openFileDialog1.FileName;
                ruta = ruta.Replace(openFileDialog1.SafeFileName, "");
                ruta_genesis = ruta; // seteo la ruta para cargar los archivo de importar
                System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog1.FileName);
                TabPage tab;
                RichTextBox rich;
                Point point = new Point(0, 0);
                tab = (TabPage)tabs.GetControl(tabs.SelectedIndex);
                tab.Text = openFileDialog1.SafeFileName;
                rich = (RichTextBox)tab.GetChildAtPoint(point);
                rich.Text = sr.ReadToEnd();
                sr.Close();
                notificar("Archivo Cargado correctamente.");
            }
            else
            {
                notificar("No se cargo ningun archivo.");
            }
        }

        private void reporteDeErroresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notificar("Tabla de Errores Creada.");
            Process.Start(@"C:\Users\Luis\Desktop\AST\errores\index.html");
        }

        private void generarRepErrores(){
            try
            {
                #region
                String ini = "<!DOCTYPE HTML>\n" +
                                "<html>\n" +
                                "	<head>\n" +
                                "		<title>Reporte/XFORM</title>\n" +
                                "		<meta charset=\"utf-8\" />\n" +
                                "		<meta name=\"viewport\" content=\"width=device-width, initial-scale=1, user-scalable=no\" />\n" +
                                "		<link rel=\"stylesheet\" href=\"assets/css/main.css\" />\n" +
                                "		<noscript><link rel=\"stylesheet\" href=\"assets/css/noscript.css\" /></noscript>\n" +
                                "	</head>\n" +
                                "	<body class=\"is-preload\">\n" +
                                "\n" +
                                "		<!-- Header -->\n" +
                                "			<section id=\"header\">\n" +
                                "				<header>\n" +
                                "					<h1>Reporete Erores</h1>\n" +
                                "					<p>XFORM</p>\n" +
                                "				</header>\n" +
                                "				<footer>\n" +
                                "					<a href=\"#banner\" class=\"button style2 scrolly-middle\">VER</a>\n" +
                                "				</footer>\n" +
                                "			</section>\n" +
                                "\n" +
                                "		<!-- Feature 1 -->\n" +
                                "			<section id=\"banner\">\n" +
                                "					<header>\n" +
                                "						<h3>Error de compilación</h3>\n" +
                                "					</header>\n" +
                                "					<div class=\"table-wrapper\">\n" +
                                "						<table>\n" +
                                "							<thead>\n" +
                                "								<tr>\n" +
                                "									<th>id</th>\n" +
                                "									<th>Descripción</th>\n" +
                                "								</tr>\n" +
                                "							</thead>\n" +
                                "							<tbody>";
                String fin = "</tbody>\n" +
                                    "						</table>\n" +
                                    "					</div>\n" +
                                    "				</section>\n" +
                                    "\n" +
                                    "		<section id=\"footer\">\n" +
                                    "			<ul class=\"icons\">\n" +
                                    "				<li><a href=\"#\" class=\"icon fa-twitter\"><span class=\"label\">Twitter</span></a></li>\n" +
                                    "				<li><a href=\"#\" class=\"icon fa-facebook\"><span class=\"label\">Facebook</span></a></li>\n" +
                                    "				<li><a href=\"#\" class=\"icon fa-google-plus\"><span class=\"label\">Google+</span></a></li>\n" +
                                    "				<li><a href=\"#\" class=\"icon fa-pinterest\"><span class=\"label\">Pinterest</span></a></li>\n" +
                                    "				<li><a href=\"#\" class=\"icon fa-dribbble\"><span class=\"label\">Dribbble</span></a></li>\n" +
                                    "				<li><a href=\"#\" class=\"icon fa-linkedin\"><span class=\"label\">LinkedIn</span></a></li>\n" +
                                    "			</ul>\n" +
                                    "			<div class=\"copyright\">\n" +
                                    "				<ul class=\"menu\">\n" +
                                    "					<li>&copy; Untitled. All rights reserved.</li><li>Design: <a href=\"http://html5up.net\">HTML5 UP</a></li>\n" +
                                    "				</ul>\n" +
                                    "			</div>\n" +
                                    "		</section>\n" +
                                    "\n" +
                                    "		<!-- Scripts -->\n" +
                                    "			<script src=\"assets/js/jquery.min.js\"></script>\n" +
                                    "			<script src=\"assets/js/jquery.scrolly.min.js\"></script>\n" +
                                    "			<script src=\"assets/js/jquery.poptrox.min.js\"></script>\n" +
                                    "			<script src=\"assets/js/browser.min.js\"></script>\n" +
                                    "			<script src=\"assets/js/breakpoints.min.js\"></script>\n" +
                                    "			<script src=\"assets/js/util.js\"></script>\n" +
                                    "			<script src=\"assets/js/main.js\"></script>\n" +
                                    "\n" +
                                    "	</body>\n" +
                                    "</html>";
                #endregion
                int i = 1;
                String medio="";
                foreach (String item in listaErrores)
                {
                    medio += "								<tr>\n" +
                             "									<td>"+i+"</td>\n" +
                             "									<td>"+item+"</td>\n" +
                             "								</tr>";
                    i++;
                }

                String todo = ini + medio + fin;
                string[] cad = { todo }; //C:\Users\Luis\Desktop\AST
                System.IO.File.WriteAllLines(@"C:\Users\Luis\Desktop\AST\errores\index.html", cad);

            }
            catch (Exception)
            {
                Console.WriteLine("No se pudo generar el reporte de errores.");
                notificar("No se pudo generar el reporte de errores.");
            }
        }

        private void generar_Click(object sender, EventArgs e)
        {
            TabPage tab;
            RichTextBox rich;
            Point point = new Point(0, 0);
            tab = (TabPage)tabs.GetControl(tabs.SelectedIndex);
            rich = (RichTextBox)tab.GetChildAtPoint(point);
            ParseTreeNode raiz = analisis.analizar(rich.Text);
            listaErrores.Clear();
            if (raiz == null)
            {
                notificar("Existen errores, revisar reporte.");
                generarRepErrores();
            }
            else
            {
                notificar("Analisis realizado correctamente, inica la ejecucion.");
                EjecucionXform ejecutar = new EjecucionXform(this.consola);
                ejecutar.capturarClases(raiz);
                ejecutar.capturarClaseImport(raiz);
                ejecutar.capturarClasePrincipal();
                //graf.generarImg(raiz, "principal");
                notificar("Finalizo la ejecucion");  
                if (listaErrores.Count > 0)
                {
                    notificar("Existen errores, revisar reporte.");
                    generarRepErrores();
                }

                //pedir un nombre y guardar el fomulario y sus respuestas
                guardarForm(ejecutar.Preguntas);
            }

        }

        private void guardarForm( TablaPreguntas pregs) {
            TabPage tab;
            RichTextBox rich;
            Point point = new Point(0, 0);
            tab = (TabPage)tabs.GetControl(tabs.SelectedIndex);
            rich = (RichTextBox)tab.GetChildAtPoint(point);

            String[] name = tab.Text.Split('.');
            string pathString = System.IO.Path.Combine(ruta_genesis, name[0]);
            try
            {
                System.IO.Directory.CreateDirectory(pathString);
                String respuestas = "";
                foreach (KeyValuePair<String, Pregunta> pre in pregs.getT())
                {
                    respuestas += "<===============================================================>\n";
                    respuestas += "Nombre Pregunta: " + pre.Key + "\n";
                    respuestas += "Pregunta: " + pre.Value.etiqueta + "\n";
                    respuestas += "Respuesta:" + pre.Value.respuesta + "\n";
                    respuestas += "<===============================================================>\n";

                }

                string[] cad = { respuestas }; //C:\Users\Luis\Desktop\AST
                String ruta = System.IO.Path.Combine(pathString, name[0]+".res");
                System.IO.File.WriteAllLines(ruta, cad);
                String []form = {rich.Text};
                ruta = System.IO.Path.Combine(pathString, name[0] + ".xform");
                System.IO.File.WriteAllLines(ruta, cad);
                notificar("Formulario Guardado Correctamente. :)");
                Console.WriteLine("Formulario Guardado Correctamente. :)");
            }
            catch (Exception)
            {
                Console.WriteLine("No se pudo almacenar el formulario que respondio. :(");
                notificar("No se pudo almacenar el formulario que respondio. :(");
            }   
        }

        private void ver_form_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            DirectoryInfo directoryInfo = new DirectoryInfo(ruta_genesis);  
            if (directoryInfo.Exists)
            {
                ConstruirDir(directoryInfo, treeView1.Nodes); 
            } 
        }


        private void ConstruirDir(DirectoryInfo directoryInfo, TreeNodeCollection nodos)
        {
            TreeNode curNode = nodos.Add(directoryInfo.Name);

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                curNode.Nodes.Add(file.FullName, file.Name);
            }
            foreach (DirectoryInfo subdir in directoryInfo.GetDirectories())
            {
                ConstruirDir(subdir, curNode.Nodes);
            }
        } 
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Name.EndsWith("res"))
            {
                string titulo = "Respuestas " + (tabs.TabCount + 1);
                TabPage tab = new TabPage(titulo);
                //Lineas lineas = new Lineas(timer1);
                //System.Windows.Forms.Panel pane=lineas.getPane();
                RichTextBox rich = new RichTextBox();
                rich.SetBounds(0, 0, tab1.Bounds.Width, tab1.Bounds.Height);
                //PictureBox pic = new PictureBox();
                //pic.SetBounds(0, 0, 30, tab1.Bounds.Height);
                //pane.Controls.Add(pic);
                tab.Controls.Add(rich);
                tabs.TabPages.Add(tab);
                StreamReader reader = new StreamReader(e.Node.Name);
                rich.Text=reader.ReadToEnd();
                reader.Close();
                notificar("Se abrio una nueva pestaña con respuestas.");
            }
        }

        private void verRespuestasToolStripMenuItem_Click(object sender, EventArgs e)
        {

        } 

    }
}
