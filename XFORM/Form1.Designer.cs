namespace XFORM
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.archivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nuevoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cargarArchivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cerrarPestañaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formulariosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verFormulariosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verRespuestasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualTecnicoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualDeUsuarioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reporteDeErroresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ver_form = new System.Windows.Forms.Button();
            this.generar = new System.Windows.Forms.Button();
            this.cargar2 = new System.Windows.Forms.Button();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tab1 = new System.Windows.Forms.TabPage();
            this.rich = new System.Windows.Forms.RichTextBox();
            this.tabContro = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.consola = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numFila = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numCol = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tab1.SuspendLayout();
            this.tabContro.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archivoToolStripMenuItem,
            this.formulariosToolStripMenuItem,
            this.manualesToolStripMenuItem,
            this.reportesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1098, 27);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // archivoToolStripMenuItem
            // 
            this.archivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nuevoToolStripMenuItem,
            this.cargarArchivoToolStripMenuItem,
            this.cerrarPestañaToolStripMenuItem});
            this.archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
            this.archivoToolStripMenuItem.Size = new System.Drawing.Size(67, 23);
            this.archivoToolStripMenuItem.Text = "Archivo";
            // 
            // nuevoToolStripMenuItem
            // 
            this.nuevoToolStripMenuItem.Name = "nuevoToolStripMenuItem";
            this.nuevoToolStripMenuItem.Size = new System.Drawing.Size(171, 24);
            this.nuevoToolStripMenuItem.Text = "Nuevo";
            this.nuevoToolStripMenuItem.Click += new System.EventHandler(this.nuevoToolStripMenuItem_Click);
            // 
            // cargarArchivoToolStripMenuItem
            // 
            this.cargarArchivoToolStripMenuItem.Name = "cargarArchivoToolStripMenuItem";
            this.cargarArchivoToolStripMenuItem.Size = new System.Drawing.Size(171, 24);
            this.cargarArchivoToolStripMenuItem.Text = "Cargar Archivo";
            this.cargarArchivoToolStripMenuItem.Click += new System.EventHandler(this.cargarArchivoToolStripMenuItem_Click);
            // 
            // cerrarPestañaToolStripMenuItem
            // 
            this.cerrarPestañaToolStripMenuItem.Name = "cerrarPestañaToolStripMenuItem";
            this.cerrarPestañaToolStripMenuItem.Size = new System.Drawing.Size(171, 24);
            this.cerrarPestañaToolStripMenuItem.Text = "Cerrar Pestaña";
            this.cerrarPestañaToolStripMenuItem.Click += new System.EventHandler(this.cerrarPestañaToolStripMenuItem_Click);
            // 
            // formulariosToolStripMenuItem
            // 
            this.formulariosToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.verFormulariosToolStripMenuItem,
            this.verRespuestasToolStripMenuItem});
            this.formulariosToolStripMenuItem.Name = "formulariosToolStripMenuItem";
            this.formulariosToolStripMenuItem.Size = new System.Drawing.Size(93, 23);
            this.formulariosToolStripMenuItem.Text = "Formularios";
            // 
            // verFormulariosToolStripMenuItem
            // 
            this.verFormulariosToolStripMenuItem.Name = "verFormulariosToolStripMenuItem";
            this.verFormulariosToolStripMenuItem.Size = new System.Drawing.Size(174, 24);
            this.verFormulariosToolStripMenuItem.Text = "Ver formularios";
            // 
            // verRespuestasToolStripMenuItem
            // 
            this.verRespuestasToolStripMenuItem.Name = "verRespuestasToolStripMenuItem";
            this.verRespuestasToolStripMenuItem.Size = new System.Drawing.Size(174, 24);
            this.verRespuestasToolStripMenuItem.Text = "Ver respuestas";
            this.verRespuestasToolStripMenuItem.Click += new System.EventHandler(this.verRespuestasToolStripMenuItem_Click);
            // 
            // manualesToolStripMenuItem
            // 
            this.manualesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manualTecnicoToolStripMenuItem,
            this.manualDeUsuarioToolStripMenuItem});
            this.manualesToolStripMenuItem.Name = "manualesToolStripMenuItem";
            this.manualesToolStripMenuItem.Size = new System.Drawing.Size(80, 23);
            this.manualesToolStripMenuItem.Text = "Manuales";
            // 
            // manualTecnicoToolStripMenuItem
            // 
            this.manualTecnicoToolStripMenuItem.Name = "manualTecnicoToolStripMenuItem";
            this.manualTecnicoToolStripMenuItem.Size = new System.Drawing.Size(196, 24);
            this.manualTecnicoToolStripMenuItem.Text = "Manual Tecnico";
            // 
            // manualDeUsuarioToolStripMenuItem
            // 
            this.manualDeUsuarioToolStripMenuItem.Name = "manualDeUsuarioToolStripMenuItem";
            this.manualDeUsuarioToolStripMenuItem.Size = new System.Drawing.Size(196, 24);
            this.manualDeUsuarioToolStripMenuItem.Text = "Manual de Usuario";
            // 
            // reportesToolStripMenuItem
            // 
            this.reportesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reporteDeErroresToolStripMenuItem});
            this.reportesToolStripMenuItem.Name = "reportesToolStripMenuItem";
            this.reportesToolStripMenuItem.Size = new System.Drawing.Size(75, 23);
            this.reportesToolStripMenuItem.Text = "Reportes";
            // 
            // reporteDeErroresToolStripMenuItem
            // 
            this.reporteDeErroresToolStripMenuItem.Name = "reporteDeErroresToolStripMenuItem";
            this.reporteDeErroresToolStripMenuItem.Size = new System.Drawing.Size(194, 24);
            this.reporteDeErroresToolStripMenuItem.Text = "Reporte de Errores";
            this.reporteDeErroresToolStripMenuItem.Click += new System.EventHandler(this.reporteDeErroresToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ver_form);
            this.groupBox1.Controls.Add(this.generar);
            this.groupBox1.Controls.Add(this.cargar2);
            this.groupBox1.Location = new System.Drawing.Point(186, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(403, 100);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Acciones";
            // 
            // ver_form
            // 
            this.ver_form.Location = new System.Drawing.Point(273, 19);
            this.ver_form.Name = "ver_form";
            this.ver_form.Size = new System.Drawing.Size(84, 75);
            this.ver_form.TabIndex = 2;
            this.ver_form.Text = "Ver Form";
            this.ver_form.UseVisualStyleBackColor = true;
            this.ver_form.Click += new System.EventHandler(this.ver_form_Click);
            // 
            // generar
            // 
            this.generar.Location = new System.Drawing.Point(161, 19);
            this.generar.Name = "generar";
            this.generar.Size = new System.Drawing.Size(84, 75);
            this.generar.TabIndex = 1;
            this.generar.Text = "Generar";
            this.generar.UseVisualStyleBackColor = true;
            this.generar.Click += new System.EventHandler(this.generar_Click);
            // 
            // cargar2
            // 
            this.cargar2.Location = new System.Drawing.Point(53, 19);
            this.cargar2.Name = "cargar2";
            this.cargar2.Size = new System.Drawing.Size(84, 75);
            this.cargar2.TabIndex = 0;
            this.cargar2.Text = "Cargar";
            this.cargar2.UseVisualStyleBackColor = true;
            this.cargar2.Click += new System.EventHandler(this.cargar2_Click);
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tab1);
            this.tabs.Location = new System.Drawing.Point(12, 147);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(751, 376);
            this.tabs.TabIndex = 0;
            // 
            // tab1
            // 
            this.tab1.Controls.Add(this.rich);
            this.tab1.Location = new System.Drawing.Point(4, 22);
            this.tab1.Name = "tab1";
            this.tab1.Padding = new System.Windows.Forms.Padding(3);
            this.tab1.Size = new System.Drawing.Size(743, 350);
            this.tab1.TabIndex = 0;
            this.tab1.Text = "Nuevo 1";
            this.tab1.UseVisualStyleBackColor = true;
            // 
            // rich
            // 
            this.rich.Location = new System.Drawing.Point(0, 0);
            this.rich.Name = "rich";
            this.rich.Size = new System.Drawing.Size(747, 354);
            this.rich.TabIndex = 0;
            this.rich.Text = "";
            // 
            // tabContro
            // 
            this.tabContro.Controls.Add(this.tabPage2);
            this.tabContro.Location = new System.Drawing.Point(779, 35);
            this.tabContro.Name = "tabContro";
            this.tabContro.SelectedIndex = 0;
            this.tabContro.Size = new System.Drawing.Size(307, 488);
            this.tabContro.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.treeView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(299, 462);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Directorio";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(299, 462);
            this.treeView1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 526);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Consola de Salida";
            // 
            // consola
            // 
            this.consola.BackColor = System.Drawing.Color.Black;
            this.consola.ForeColor = System.Drawing.Color.White;
            this.consola.Location = new System.Drawing.Point(12, 544);
            this.consola.Name = "consola";
            this.consola.Size = new System.Drawing.Size(1070, 122);
            this.consola.TabIndex = 4;
            this.consola.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(907, 679);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Linea:";
            // 
            // numFila
            // 
            this.numFila.AutoSize = true;
            this.numFila.Location = new System.Drawing.Point(951, 679);
            this.numFila.Name = "numFila";
            this.numFila.Size = new System.Drawing.Size(14, 15);
            this.numFila.TabIndex = 6;
            this.numFila.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(996, 679);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "Columna:";
            // 
            // numCol
            // 
            this.numCol.AutoSize = true;
            this.numCol.Location = new System.Drawing.Point(1062, 679);
            this.numCol.Name = "numCol";
            this.numCol.Size = new System.Drawing.Size(14, 15);
            this.numCol.TabIndex = 8;
            this.numCol.Text = "0";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1098, 713);
            this.Controls.Add(this.numCol);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numFila);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.consola);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabContro);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "XFORM";
            this.Load += new System.EventHandler(this.XFORM_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.tab1.ResumeLayout(false);
            this.tabContro.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem archivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nuevoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cargarArchivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cerrarPestañaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formulariosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verFormulariosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verRespuestasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manualesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manualTecnicoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manualDeUsuarioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reporteDeErroresToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ver_form;
        private System.Windows.Forms.Button generar;
        private System.Windows.Forms.Button cargar2;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tab1;
        private System.Windows.Forms.TabControl tabContro;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox consola;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label numFila;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label numCol;
        private System.Windows.Forms.RichTextBox rich;
        private System.Windows.Forms.TreeView treeView1;
        public System.Windows.Forms.Timer timer1;
    }
}

