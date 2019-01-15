using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XFORM.Ejecucion
{

     class FormPregunta
    {
        public  string ShowCadenaSimple(string caption, string text,string sugerir,bool requerido,string requeridoMsg,bool sololec)
        {
            if (sugerir != "")
            {
                Form prompt = new Form();
                prompt.Width = 280;
                prompt.Height = 150;
                prompt.Text = caption;
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = text, AutoEllipsis = true };
                Label Sugerir = new Label() { Left = 16, Top = 40, Width = 440, Text = sugerir, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Italic), AutoEllipsis=true };
                TextBox textBox = new TextBox() { Left = 16, Top = 60, Width = 240, TabIndex = 0, TabStop = true, Enabled=sololec };
                Button confirmacion = new Button() { Text = "Responder", Left = 16, Width = 80, Top = 82, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => {
                    if (requerido) {
                        if (textBox.Text.Length == 0) {
                            MessageBox.Show("Requerido : "+requeridoMsg, "Pregunta Requerida",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }else prompt.Close(); 
                    }else prompt.Close();
                };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile) {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 82, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Sugerir);
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                textBox.Focus();
                return textBox.Text;
            }
            else {
                Form prompt = new Form();
                prompt.Width = 280;
                prompt.Height = 150;
                prompt.Text = caption;
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = text, AutoEllipsis = true };
                TextBox textBox = new TextBox() { Left = 16, Top = 40, Width = 240, TabIndex = 0, TabStop = true, Enabled=sololec };
                Button confirmacion = new Button() { Text = "Responder", Left = 16, Width = 80, Top = 82, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) =>
                {
                    if (requerido)
                    {
                        if (textBox.Text.Length == 0)
                        {
                            MessageBox.Show("Requerido : " + requeridoMsg, "Pregunta Requerida",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else prompt.Close();
                    }
                    else prompt.Close();
                };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 82, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                textBox.Focus();
                return textBox.Text;
            }
        }

        public  string ShowCadenaMulti(string titulo, string etiqueta, string sugerir, bool requerido,string requeridoMsg, string maximo, string minimo, string lineas,bool sololec)
        {
            if (sugerir != "")
            {
                int max = 70;
                int min = 0;
                int saltos = 0;
                bool multi = false;
                if (!maximo.ToLower().Equals("nulo"))
                {
                    max = Int32.Parse(maximo);
                }
                if (!minimo.ToLower().Equals("nulo"))
                {
                    min = Int32.Parse(minimo);
                }
                if (!lineas.ToLower().Equals("nulo"))
                {
                    multi=true;
                    saltos = Int32.Parse(lineas);
                }
                
                Form prompt = new Form();
                prompt.Width = 300;
                prompt.Height = 190;
                prompt.Text = titulo;
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                Label Sugerir = new Label() { Left = 16, Top = 40, Width = 440, Text = sugerir, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Italic), AutoEllipsis = true };
                TextBox textBox = new TextBox() { Left = 16, Top = 60, Width = 240, Height=50, TabIndex = 0, TabStop = true, ScrollBars = ScrollBars.Both, Multiline=multi , MaxLength=max,Enabled=sololec};
                Button confirmacion = new Button() { Text = "Responder", Left = 16, Width = 80, Top = 120, TabIndex = 1, TabStop = true};
                confirmacion.Click += (sender, e) => {

                    if (requerido)
                    {
                        if (textBox.Text.Length == 0)
                        {
                            MessageBox.Show("Requerido : " + requeridoMsg, "Pregunta Requerida",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else {
                            if (textBox.Text.Length >= min){
                                if(textBox.Lines.Count() >= saltos)
                                    prompt.Close();
                                else
                                    MessageBox.Show("Debe ingresar un minimo de " + lineas+ " saltos de linea.", "INGRESE EL MINIMO",MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                                MessageBox.Show("Debe ingresar un minimo de " + min + " caracteres.", "INGRESE EL MINIMO",MessageBoxButtons.OK, MessageBoxIcon.Error);                            
                        }
                    }
                    else {
                        if (textBox.Text.Length >= min)
                        {
                            if (textBox.Lines.Count() >= saltos)
                                prompt.Close();
                            else
                                MessageBox.Show("Debe ingresar un minimo de " + lineas + " saltos de linea.", "INGRESE EL MINIMO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                            MessageBox.Show("Debe ingresar un minimo de " + min + " caracteres.", "INGRESE EL MINIMO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }; 
                };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 120, TabIndex = 2, TabStop = true };
                    confirmacion.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Sugerir);
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                textBox.Focus();
                return textBox.Text;
            }
            else
            {
                int max = 100;
                int min = 10;
                int saltos = 0;
                bool multi = false;
                if (!maximo.ToLower().Equals("Nulo"))
                {
                    max = Int32.Parse(maximo);
                }
                if (!minimo.ToLower().Equals("Nulo"))
                {
                    min = Int32.Parse(minimo);
                }
                if (!lineas.ToLower().Equals("Nulo"))
                {
                    saltos = Int32.Parse(lineas);
                    multi = true;
                }

                Form prompt = new Form();
                prompt.Width = 300;
                prompt.Height = 190;
                prompt.Text = titulo;
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                TextBox textBox = new TextBox() { Left = 16, Top = 40, Width = 240, Height = 50, TabIndex = 0, TabStop = true, ScrollBars = ScrollBars.Both, Multiline = multi, MaxLength = max,Enabled = sololec };
                Button confirmacion = new Button() { Text = "Responder", Left = 16, Width = 80, Top = 120, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) =>
                {
                    if (requerido)
                    {
                        if (textBox.Text.Length == 0)
                        {
                            MessageBox.Show("Requerido : " + requeridoMsg, "Pregunta Requerida",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            if (textBox.Text.Length >= min)
                            {
                                if (textBox.Lines.Count() >= saltos)
                                    prompt.Close();
                                else
                                    MessageBox.Show("Debe ingresar un minimo de " + lineas + " saltos de linea.", "INGRESE EL MINIMO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                                MessageBox.Show("Debe ingresar un minimo de " + min + " caracteres.", "INGRESE EL MINIMO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            
                        }
                    }
                    else
                    {
                        if (textBox.Text.Length >= min)
                        {
                            if (textBox.Lines.Count() >= saltos)
                                prompt.Close();
                            else
                                MessageBox.Show("Debe ingresar un minimo de " + lineas + " saltos de linea.", "INGRESE EL MINIMO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                            MessageBox.Show("Debe ingresar un minimo de " + min + " caracteres.", "INGRESE EL MINIMO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        
                    };
                };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 120, TabIndex = 2, TabStop = true };
                    confirmacion.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                textBox.Focus();
                return textBox.Text;
            }
        }

        public string ShowNumerico(string titulo, string etiqueta,String sugerir, bool sololec)
        {
            Form prompt = new Form();
            prompt.Width = 380;
            prompt.Height = 250;
            prompt.Text = titulo;
            if (sugerir != "")
            {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                Label Sugerir = new Label() { Left = 16, Top = 40, Width = 440, Text = sugerir, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Italic) };
                NumericUpDown Numerico = new NumericUpDown() { Left = 16, Top = 60, TabIndex = 0, TabStop = true,Enabled =sololec};
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 82, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(Numerico);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 82, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Sugerir);
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                Numerico.Focus();
                return Numerico.Text;
            }
            else {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                NumericUpDown Numerico = new NumericUpDown() { Left = 16, Top = 40, TabIndex = 0, TabStop = true , Enabled = sololec};
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 72, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(Numerico);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 72, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                Numerico.Focus();
                return Numerico.Text;
            }
        }

        public  string ShowDecimal(string titulo, string etiqueta,String sugerir, bool sololec) { 
            Form prompt = new Form();
            prompt.Width = 380;
            prompt.Height = 250;
            prompt.Text = titulo;
            if (sugerir != "")
            {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                Label Sugerir = new Label() { Left = 16, Top = 40, Width = 440, Text = sugerir, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Italic), AutoEllipsis = true };
                NumericUpDown Numerico = new NumericUpDown() { Left = 16, Top = 60, TabIndex = 0, TabStop = true ,DecimalPlaces=2, ThousandsSeparator=true, Increment = 0.25M , Enabled = sololec};
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 82, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(Numerico);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 82, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Sugerir);
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                Numerico.Focus();
                return Numerico.Text;
            }
            else {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                NumericUpDown Numerico = new NumericUpDown() { Left = 16, Top = 40, TabIndex = 0, TabStop = true, DecimalPlaces = 2, ThousandsSeparator = true, Increment = 0.25M, Enabled = sololec };
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 72, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(Numerico);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 72, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                Numerico.Focus();
                return Numerico.Text;
            }
        }

        public  string ShowRango(string titulo, string etiqueta, String sugerir, int ini, int fin,bool sololec)
        {
            Form prompt = new Form();
            prompt.Width = 380;
            prompt.Height = 250;
            prompt.Text = titulo;
            if (sugerir != "")
            {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                Label Sugerir = new Label() { Left = 16, Top = 40, Width = 440, Text = sugerir, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Italic), AutoEllipsis = true };
                NumericUpDown Numerico = new NumericUpDown() { Left = 16, Top = 60, TabIndex = 0, TabStop = true, Minimum=ini, Maximum=fin, Enabled = sololec};
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 82, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(Numerico);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 82, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Sugerir);
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                Numerico.Focus();
                return Numerico.Text;
            }
            else
            {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis=true };
                NumericUpDown Numerico = new NumericUpDown() { Left = 16, Top = 40, TabIndex = 0, TabStop = true, Minimum = ini, Maximum = fin,Enabled=sololec };
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 72, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(Numerico);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 72, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                Numerico.Focus();
                return Numerico.Text;
            }
        }

        public  string ShowCondicion(string caption, string text, string sugerir, bool requerido, string requeridoMsg,bool sololec)
        {
            if (sugerir != "")
            {
                Form prompt = new Form();
                prompt.Width = 280;
                prompt.Height = 180;
                prompt.Text = caption;
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = text, AutoEllipsis = true };
                Label Sugerir = new Label() { Left = 16, Top = 40, Width = 440, Text = sugerir, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Italic), AutoEllipsis = true };
                RadioButton verdadero = new RadioButton() { Left = 16, Top = 60, Width = 100, TabIndex = 0, Text = "Verdadero",Enabled = sololec };
                RadioButton falso = new RadioButton() { Left=150, Top = 60, Width = 100, TabIndex = 1, Text = "Falso",Enabled=sololec };
                Button confirmacion = new Button() { Text = "Responder", Left = 16, Width = 80, Top = 90, TabIndex = 2, TabStop = true };
                confirmacion.Click += (sender, e) =>
                {
                    if (requerido)
                    {
                        if (!verdadero.Checked && !falso.Checked)
                        {
                            MessageBox.Show("Requerido : " + requeridoMsg, "Pregunta Requerida",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else prompt.Close();
                    }
                    else prompt.Close();
                };
                prompt.Controls.Add(verdadero);
                prompt.Controls.Add(falso);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 90, TabIndex = 3, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Sugerir);
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                verdadero.Focus();

                if (verdadero.Checked)
                {
                    return "verdadero";
                }
                else {
                    return "falso";
                }
            }
            else
            {
                Form prompt = new Form();
                prompt.Width = 280;
                prompt.Height = 180;
                prompt.Text = caption;
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = text, AutoEllipsis = true };
                RadioButton verdadero = new RadioButton() { Left = 16, Top = 60, Width = 100, TabIndex = 0, Text = "Verdadero",Enabled=sololec };
                RadioButton falso = new RadioButton() { Left = 150, Top = 60, Width = 100, TabIndex = 1, Text = "Falso",Enabled=sololec };
                Button confirmacion = new Button() { Text = "Responder", Left = 16, Width = 80, Top = 90, TabIndex = 2, TabStop = true };
                confirmacion.Click += (sender, e) =>
                {
                    if (requerido)
                    {
                        if (!verdadero.Checked && !falso.Checked)
                        {
                            MessageBox.Show("Requerido : " + requeridoMsg, "Pregunta Requerida",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else prompt.Close();
                    }
                    else prompt.Close();
                };
                prompt.Controls.Add(verdadero);
                prompt.Controls.Add(falso);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 90, TabIndex = 3, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                verdadero.Focus();
                if (verdadero.Checked)
                {
                    return "verdadero";
                }
                else
                {
                    return "falso";
                }
            }
        }

        public  string ShowTime(string titulo, string etiqueta, String sugerir, bool sololec)
        {
            Form prompt = new Form();
            prompt.Width = 380;
            prompt.Height = 250;
            prompt.Text = titulo;
            if (sugerir != "")
            {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                Label Sugerir = new Label() { Left = 16, Top = 40, Width = 440, Text = sugerir, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Italic), AutoEllipsis = true };
                DateTimePicker  time = new DateTimePicker() { Left = 16, Top = 60, TabIndex = 0, TabStop = true ,Format = DateTimePickerFormat.Time , ShowUpDown =true,Enabled = sololec};
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 82, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(time);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 82, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Sugerir);
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                time.Focus();
                return time.Value.ToString("HH:mm:ss");
            }
            else
            {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                DateTimePicker time = new DateTimePicker() { Left = 16, Top = 40, TabIndex = 0, TabStop = true, Format = DateTimePickerFormat.Time, ShowUpDown = true,Enabled = sololec };
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 72, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(time);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 72, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                time.Focus();
                return time.Value.ToString("HH:mm:ss");
            }
        }

        public  string ShowDate(string titulo, string etiqueta, String sugerir, bool sololec)
        {
            Form prompt = new Form();
            prompt.Width = 380;
            prompt.Height = 250;
            prompt.Text = titulo;
            if (sugerir != "")
            {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                Label Sugerir = new Label() { Left = 16, Top = 40, Width = 440, Text = sugerir, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Italic), AutoEllipsis = true };
                DateTimePicker date = new DateTimePicker() { Left = 16, Top = 60, TabIndex = 0, TabStop = true ,CustomFormat="dd/MM/yyyy",Format = DateTimePickerFormat.Custom,Enabled = sololec};
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 82, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(date);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 82, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Sugerir);
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                date.Focus();
                return date.Value.ToString("dd/MM/yyyy");
            }
            else
            {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                DateTimePicker time = new DateTimePicker() { Left = 16, Top = 40, TabIndex = 0, TabStop = true, CustomFormat="dd/MM/yyyy", Format = DateTimePickerFormat.Custom, Enabled=sololec };
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 72, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(time);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 72, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                time.Focus();
                return time.Value.ToString("dd/MM/yyyy");
            }
        }

        public string ShowDateTime(string titulo, string etiqueta, String sugerir,bool sololec)
        {
            Form prompt = new Form();
            prompt.Width = 380;
            prompt.Height = 250;
            prompt.Text = titulo;
            if (sugerir != "")
            {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                Label Sugerir = new Label() { Left = 16, Top = 40, Width = 440, Text = sugerir, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Italic), AutoEllipsis = true };
                DateTimePicker date = new DateTimePicker() { Left = 16, Top = 60, TabIndex = 0, TabStop = true, CustomFormat = "dd/MM/yyyy HH:mm:ss", Format = DateTimePickerFormat.Custom,Enabled = sololec };
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 82, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(date);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 82, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Sugerir);
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                date.Focus();
                return date.Value.ToString("dd/MM/yyyy HH:mm:ss");
            }
            else
            {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                DateTimePicker time = new DateTimePicker() { Left = 16, Top = 40, TabIndex = 0, TabStop = true, CustomFormat = "dd/MM/yyyy HH:mm:ss", Format = DateTimePickerFormat.Custom , Enabled =sololec};
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 72, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(time);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 72, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                time.Focus();
                return time.Value.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }
        public  string ShowMedia(string titulo, string etiqueta, String sugerir, String @ruta,bool autoplay)
        {
            Form prompt = new Form();
            prompt.Width = 480;
            prompt.Height = 350;
            prompt.Text = titulo;

            if (!System.IO.File.Exists(@ruta)) {
                ruta = @"C:\Users\Luis\Desktop\AST\gua.png";
            }

            if (sugerir != "")
            {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                Label Sugerir = new Label() { Left = 16, Top = 40, Width = 440, Text = sugerir, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Italic), AutoEllipsis = true };
                AxWMPLib.AxWindowsMediaPlayer media = new AxWMPLib.AxWindowsMediaPlayer() { Left = 16, Top = 60, Width = 240, Height=220,TabIndex=0 };
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 280, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => { prompt.Close(); media.Dispose();};
                prompt.Controls.Add(media);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 280, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Sugerir);
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.Show();
                media.Focus();
                if (autoplay)
                {
                    media.URL = ruta;
                    media.settings.autoStart = true;
                }
                else {
                    media.URL = ruta;
                    media.settings.autoStart = false;
                }
                return "correcto";
            }
            else
            {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                AxWMPLib.AxWindowsMediaPlayer media = new AxWMPLib.AxWindowsMediaPlayer() { Left = 16, Top = 40, Width = 240, Height = 220, TabIndex = 0 };
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 260, TabIndex = 1, TabStop = true };
                confirmacion.Click += (sender, e) => { prompt.Close(); media.Dispose(); };
                prompt.Controls.Add(media);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 260, TabIndex = 2, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.Show();
                media.Focus();
                if (autoplay)
                {
                    media.URL = ruta;
                    media.settings.autoStart = true;
                }
                else
                {
                    media.URL = ruta;
                    media.settings.autoStart = false;
                }
                return "correcto";
            }
        }

        public  string ShowNota(string titulo, string etiqueta)
        {
            Form prompt = new Form();
            prompt.Width = 380;
            prompt.Height = 150;
            prompt.Text = titulo;
            Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta , AutoEllipsis=true};
            Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 60, TabIndex = 0, TabStop = true };
            confirmacion.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmacion);
            if (EjecucionXform.inwhile)
            {
                Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 60, TabIndex = 1, TabStop = true };
                SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                prompt.Controls.Add(SalirCiclo);
            }
            prompt.Controls.Add(Etiqueta);
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.ShowDialog();
            confirmacion.Focus();
            return "correcto";
        }

        public  string ShowMediaUp(string titulo, string etiqueta, String sugerir, String filtro, bool sololec)
        {
            Form prompt = new Form();
            prompt.Width = 480;
            prompt.Height = 380;
            prompt.Text = titulo;
            String ruta="C:/Defecto/imgs";
            if (sugerir != "")
            {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = etiqueta, AutoEllipsis = true };
                Label Sugerir = new Label() { Left = 16, Top = 40, Width = 440, Text = sugerir, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Italic), AutoEllipsis = true };
                Button subir = new Button() { Text = "Subir", Left = 16, Top = 60, Width = 240, TabIndex = 0, TabStop = true,Enabled = sololec };
                AxWMPLib.AxWindowsMediaPlayer media = new AxWMPLib.AxWindowsMediaPlayer() { Left = 16, Top = 80, Width = 240, Height = 220, TabIndex = 1 };
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 300, TabIndex = 2, TabStop = true };
                confirmacion.Click += (sender, e) => { ruta = media.URL; media.Dispose(); prompt.Close(); };
                subir.Click += (sender, e) => {
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();
                    openFileDialog1.Title = "Subir Archivo";
                    String[] ext = filtro.Split(',');
                    String filt="Todos (*.*)|*.*";
                    int i = 1;
                    foreach (String item in ext)
                    {
                        if (i == 1)
                            filt = "Tipo_" + i + " (*" + item.Trim() + ")|*" + item.Trim();
                        else 
                            filt += "|Tipo_" + i + " (*" + item.Trim() + ")|*" + item.Trim();
                        i++;
                    }
                    openFileDialog1.Filter = filt;
                    openFileDialog1.FilterIndex = ext.Count()+1;

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        ruta = openFileDialog1.FileName;
                        media.URL = ruta;
                        media.settings.autoStart = true;
                    }
                };
                prompt.Controls.Add(subir);
                prompt.Controls.Add(media);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 300, TabIndex = 3, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Sugerir);
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.Show();
                media.Focus();
                return ruta;
            }
            else
            {
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 240, Text = etiqueta };
                Button subir = new Button() { Text = "Subir",Left = 16, Top = 40, Width = 240, TabIndex = 0, TabStop = true,Enabled = sololec };
                AxWMPLib.AxWindowsMediaPlayer media = new AxWMPLib.AxWindowsMediaPlayer() { Left = 16, Top = 60, Width = 240, Height = 220, TabIndex = 1 };
                Button confirmacion = new Button() { Text = "Aceptar", Left = 16, Width = 80, Top = 300, TabIndex = 2, TabStop = true };
                confirmacion.Click += (sender, e) => { ruta = media.URL; media.Dispose(); prompt.Close(); };
                subir.Click += (sender, e) =>
                {
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();
                    openFileDialog1.Title = "Subir Archivo";
                    String[] ext = filtro.Split(',');
                    String filt = "Todos (*.*)|*.*";
                    int i = 1;
                    foreach (String item in ext)
                    {
                        if (i == 1)
                            filt = "Tipo_" + i + " (*" + item.Trim() + ")|*" + item.Trim();
                        else
                            filt += "|Tipo_" + i + " (*" + item.Trim() + ")|*" + item.Trim();
                        i++;
                    }
                    openFileDialog1.Filter = filt;
                    openFileDialog1.FilterIndex = ext.Count() + 1;

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        ruta = openFileDialog1.FileName;
                        media.URL = ruta;
                        media.settings.autoStart = true;
                    }
                };
                prompt.Controls.Add(subir);
                prompt.Controls.Add(media);
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = 300, TabIndex = 3, TabStop = true };
                    confirmacion.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.Show();
                media.Focus();
                return ruta;
            }
        }

        public  string ShowUnica(string caption, string text, string sugerir, bool requerido, string requeridoMsg,Opciones ops,bool sololec)
        {
            if (sugerir != "")
            {
                Form prompt = new Form();
                prompt.Width = 280;
                prompt.Height = 200;
                prompt.Text = caption;
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = text, AutoEllipsis = true };
                Label Sugerir = new Label() { Left = 16, Top = 40, Width = 440, Text = sugerir, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Italic), AutoEllipsis = true };
                int top=60;
                int left=16;
                int val_reini = 0;
                int tabindex=0;
                List<RadioButton> lista_op = new List<RadioButton>();
                for (int i  = 0; i <ops.elementos.Count ; i ++)
                {
                    if (ops.elementos.ElementAt(i).Count > 2)
                    {
                        String nombre = ((retorno)ops.elementos.ElementAt(i).ElementAt(0)).valor.ToString();
                        String etiqueta = ((retorno)ops.elementos.ElementAt(i).ElementAt(1)).valor.ToString();
                        String ruta = ((retorno)ops.elementos.ElementAt(i).ElementAt(2)).valor.ToString();
                        RadioButton item = new RadioButton() { Name = nombre, Left = 16, Top = top, Width = 100, TabIndex = tabindex, Text = etiqueta, Enabled = sololec, AutoEllipsis = true };
                        left +=150; tabindex++; 
                        PictureBox img= new PictureBox(){ Left=left,Top =top , Width = 100 , Height = 100, TabIndex= tabindex,SizeMode =PictureBoxSizeMode.StretchImage};
                        if (System.IO.File.Exists(ruta))
                            img.Image = Image.FromFile(ruta);
                        else
                            img.Image = Image.FromFile(@"C:\Users\Luis\Desktop\AST\gua.png");
                        lista_op.Add(item);
                        prompt.Controls.Add(item);
                        prompt.Controls.Add(img);
                        top += 120;
                        left = 16;
                    }
                    else{
                        String nombre = ((retorno)ops.elementos.ElementAt(i).ElementAt(0)).valor.ToString();
                        String etiqueta = ((retorno)ops.elementos.ElementAt(i).ElementAt(1)).valor.ToString();
                        RadioButton item = new RadioButton() { Name = nombre, Left = left, Top = top, Width = 100, TabIndex = tabindex, Text = etiqueta, Enabled = sololec, AutoEllipsis = true };
                        tabindex++;
                        lista_op.Add(item);
                        prompt.Controls.Add(item);
                        val_reini++;
                        if (val_reini == 2)
                        {
                            top += 20;
                            left = 16;
                            val_reini = 0;
                        }
                        else {
                            left+=100;
                        }
                        if (i == ops.elementos.Count - 1) {
                            top += 40;
                        }
                    }
                }
                Button confirmacion = new Button() { Text = "Responder", Left = 16, Width = 80, Top = top, TabIndex = tabindex, TabStop = true };
                tabindex++;
                confirmacion.Click += (sender, e) =>
                {
                    if (requerido)
                    {
                        if (!comprobarSeleccion(lista_op))
                        {
                            MessageBox.Show("Requerido : " + requeridoMsg, "Pregunta Requerida",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else prompt.Close();
                    }
                    else prompt.Close();
                };
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = top, TabIndex = tabindex, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Sugerir);
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                confirmacion.Focus();
                return capturarRespues(lista_op);
            }
            else
            {
                Form prompt = new Form();
                prompt.Width = 280;
                prompt.Height = 180;
                prompt.Text = caption;
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = text, AutoEllipsis = true };
                int top = 40;
                int left = 16;
                int val_reini = 0;
                int tabindex = 0;
                List<RadioButton> lista_op = new List<RadioButton>();
                for (int i = 0; i < ops.elementos.Count; i++)
                {
                    if (ops.elementos.ElementAt(i).Count > 2)
                    {
                        String nombre = ((retorno)ops.elementos.ElementAt(i).ElementAt(0)).valor.ToString();
                        String etiqueta = ((retorno)ops.elementos.ElementAt(i).ElementAt(1)).valor.ToString();
                        String ruta = ((retorno)ops.elementos.ElementAt(i).ElementAt(2)).valor.ToString();
                        RadioButton item = new RadioButton() { Name = nombre, Left = 16, Top = top, Width = 100, TabIndex = tabindex, Text = etiqueta, Enabled = sololec, AutoEllipsis = true };
                        left += 150; tabindex++;
                        PictureBox img = new PictureBox() { Left = left, Top = top, Width = 100, Height = 100, TabIndex = tabindex, SizeMode = PictureBoxSizeMode.StretchImage };
                        if (System.IO.File.Exists(ruta))
                            img.Image = Image.FromFile(ruta);
                        else
                            img.Image = Image.FromFile(@"C:\Users\Luis\Desktop\AST\gua.png");
                        lista_op.Add(item);
                        prompt.Controls.Add(item);
                        prompt.Controls.Add(img);
                        top += 120;
                        left = 16;
                    }
                    else
                    {
                        String nombre = ((retorno)ops.elementos.ElementAt(i).ElementAt(0)).valor.ToString();
                        String etiqueta = ((retorno)ops.elementos.ElementAt(i).ElementAt(1)).valor.ToString();
                        RadioButton item = new RadioButton() { Name = nombre, Left = left, Top = top, Width = 100, TabIndex = tabindex, Text = etiqueta, Enabled = sololec, AutoEllipsis = true };
                        tabindex++;
                        lista_op.Add(item);
                        prompt.Controls.Add(item);
                        val_reini++;
                        if (val_reini == 2)
                        {
                            top += 20;
                            left = 16;
                            val_reini = 0;
                        }
                        else
                        {
                            left += 100;
                        }
                        if (i == ops.elementos.Count - 1)
                        {
                            top += 40;
                        }
                    }
                }
                Button confirmacion = new Button() { Text = "Responder", Left = 16, Width = 80, Top = top, TabIndex = tabindex, TabStop = true };
                tabindex++;
                confirmacion.Click += (sender, e) =>
                {
                    if (requerido)
                    {
                        if (!comprobarSeleccion(lista_op))
                        {
                            MessageBox.Show("Requerido : " + requeridoMsg, "Pregunta Requerida",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else prompt.Close();
                    }
                    else prompt.Close();
                };
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = top, TabIndex = tabindex, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                confirmacion.Focus();
                return capturarRespues(lista_op);
            }
        }

        public string ShowmMulti(string caption, string text, string sugerir, bool requerido, string requeridoMsg, Opciones ops,bool sololec)
        {
            if (sugerir != "")
            {
                Form prompt = new Form();
                prompt.Width = 280;
                prompt.Height = 200;
                prompt.Text = caption;
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = text, AutoEllipsis=true };
                Label Sugerir = new Label() { Left = 16, Top = 40, Width = 440, Text = sugerir, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Italic), AutoEllipsis = true };
                int top = 60;
                int left = 16;
                int val_reini = 0;
                int tabindex = 0;
                List<CheckBox> lista_op = new List<CheckBox>();
                for (int i = 0; i < ops.elementos.Count; i++)
                {
                    if (ops.elementos.ElementAt(i).Count > 2)
                    {
                        String nombre = ((retorno)ops.elementos.ElementAt(i).ElementAt(0)).valor.ToString();
                        String etiqueta = ((retorno)ops.elementos.ElementAt(i).ElementAt(1)).valor.ToString();
                        String ruta = ((retorno)ops.elementos.ElementAt(i).ElementAt(2)).valor.ToString();
                        CheckBox item = new CheckBox() { Name = nombre, Left = 16, Top = top, Width = 100, TabIndex = tabindex, Text = etiqueta, Enabled = sololec, AutoEllipsis = true };
                        left += 100; tabindex++;
                        PictureBox img = new PictureBox() { Left = left, Top = top, Width = 100, Height = 100, TabIndex = tabindex, SizeMode = PictureBoxSizeMode.StretchImage };
                        if (System.IO.File.Exists(ruta))
                            img.Image = Image.FromFile(ruta);
                        else
                            img.Image = Image.FromFile(@"C:\Users\Luis\Desktop\AST\gua.png");
                        lista_op.Add(item);
                        prompt.Controls.Add(item);
                        prompt.Controls.Add(img);
                        top += 120;
                        left = 16;
                    }
                    else
                    {
                        String nombre = ((retorno)ops.elementos.ElementAt(i).ElementAt(0)).valor.ToString();
                        String etiqueta = ((retorno)ops.elementos.ElementAt(i).ElementAt(1)).valor.ToString();
                        CheckBox item = new CheckBox() { Name = nombre, Left = left, Top = top, Width = 100, TabIndex = tabindex, Text = etiqueta, Enabled = sololec, AutoEllipsis = true };
                        tabindex++;
                        lista_op.Add(item);
                        prompt.Controls.Add(item);
                        val_reini++;
                        if (val_reini == 2)
                        {
                            top += 20;
                            left = 16;
                            val_reini = 0;
                        }
                        else
                        {
                            left += 100;
                        }
                        if (i == ops.elementos.Count - 1)
                        {
                            top += 40;
                        }
                    }
                }
                Button confirmacion = new Button() { Text = "Responder", Left = 16, Width = 80, Top = top, TabIndex = tabindex, TabStop = true };
                tabindex++;
                confirmacion.Click += (sender, e) =>
                {
                    if (requerido)
                    {
                        if (!comprobarSeleccion2(lista_op))
                        {
                            MessageBox.Show("Requerido : " + requeridoMsg, "Pregunta Requerida",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else prompt.Close();
                    }
                    else prompt.Close();
                };
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = top, TabIndex = tabindex , TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Sugerir);
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                confirmacion.Focus();
                return capturarRespues2(lista_op);
            }
            else
            {
                Form prompt = new Form();
                prompt.Width = 280;
                prompt.Height = 180;
                prompt.Text = caption;
                Label Etiqueta = new Label() { Left = 16, Top = 20, Width = 440, Text = text, AutoEllipsis = true };
                int top = 40;
                int left = 16;
                int val_reini = 0;
                int tabindex = 0;
                List<CheckBox> lista_op = new List<CheckBox>();
                for (int i = 0; i < ops.elementos.Count; i++)
                {
                    if (ops.elementos.ElementAt(i).Count > 2)
                    {
                        String nombre = ((retorno)ops.elementos.ElementAt(i).ElementAt(0)).valor.ToString();
                        String etiqueta = ((retorno)ops.elementos.ElementAt(i).ElementAt(1)).valor.ToString();
                        String ruta = ((retorno)ops.elementos.ElementAt(i).ElementAt(2)).valor.ToString();
                        CheckBox item = new CheckBox() { Name = nombre, Left = 16, Top = top, Width = 100, TabIndex = tabindex, Text = etiqueta, Enabled = sololec, AutoEllipsis = true };
                        left += 100; tabindex++;
                        PictureBox img = new PictureBox() { Left = left, Top = top, Width = 100, Height = 100, TabIndex = tabindex, SizeMode = PictureBoxSizeMode.StretchImage };
                        if (System.IO.File.Exists(ruta))
                            img.Image = Image.FromFile(ruta);
                        else
                            img.Image = Image.FromFile(@"C:\Users\Luis\Desktop\AST\gua.png");
                        lista_op.Add(item);
                        prompt.Controls.Add(item);
                        prompt.Controls.Add(img);
                        top += 120;
                        left = 16;
                    }
                    else
                    {
                        String nombre = ((retorno)ops.elementos.ElementAt(i).ElementAt(0)).valor.ToString();
                        String etiqueta = ((retorno)ops.elementos.ElementAt(i).ElementAt(1)).valor.ToString();
                        CheckBox item = new CheckBox() { Name = nombre, Left = left, Top = top, Width = 100, TabIndex = tabindex, Text = etiqueta, Enabled = sololec, AutoEllipsis = true };
                        tabindex++;
                        lista_op.Add(item);
                        prompt.Controls.Add(item);
                        val_reini++;
                        if (val_reini == 2)
                        {
                            top += 20;
                            left = 16;
                            val_reini = 0;
                        }
                        else
                        {
                            left += 100;
                        }
                        if (i == ops.elementos.Count - 1)
                        {
                            top += 40;
                        }
                    }
                }
                Button confirmacion = new Button() { Text = "Responder", Left = 16, Width = 80, Top = top, TabIndex = tabindex, TabStop = true };
                tabindex++;
                confirmacion.Click += (sender, e) =>
                {
                    if (requerido)
                    {
                        if (!comprobarSeleccion2(lista_op))
                        {
                            MessageBox.Show("Requerido : " + requeridoMsg, "Pregunta Requerida",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else prompt.Close();
                    }
                    else prompt.Close();
                };
                prompt.Controls.Add(confirmacion);
                if (EjecucionXform.inwhile)
                {
                    Button SalirCiclo = new Button() { Text = "Salir Ciclo", Left = 150, Width = 80, Top = top, TabIndex =  tabindex, TabStop = true };
                    SalirCiclo.Click += (sender, e) => { EjecucionXform.salwhile = true; prompt.Close(); };
                    prompt.Controls.Add(SalirCiclo);
                }
                prompt.Controls.Add(Etiqueta);
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.ShowDialog();
                confirmacion.Focus();
                return capturarRespues2(lista_op);
            }
        }

        private  bool comprobarSeleccion(List<RadioButton> lista){
            foreach (RadioButton rad in lista)
	        {
		        if(rad.Checked){
                    return true;
                }
	        }
            return false;
        }
        private  String capturarRespues(List<RadioButton> lista) {
            string resp = "";
            int elementos=0;
            for (int i = 0; i < lista.Count; i++)
            {
                if (lista.ElementAt(i).Checked) {
                    if (elementos == 0)
                    {
                        resp += lista.ElementAt(i).Name;
                    }
                    else
                    {
                        resp += "," + lista.ElementAt(i).Name;
                    }
                    elementos++;
                }
            }
            return resp;
        }

        private bool comprobarSeleccion2(List<CheckBox> lista)
        {
            foreach (CheckBox rad in lista)
            {
                if (rad.Checked)
                {
                    return true;
                }
            }
            return false;
        }


        private String capturarRespues2(List<CheckBox> lista)
        {
            string resp = "";
            int elementos = 0;
            for (int i = 0; i < lista.Count; i++)
            {
                if (lista.ElementAt(i).Checked)
                {
                    if (elementos == 0)
                    {
                        resp += lista.ElementAt(i).Name;
                    }
                    else
                    {
                        resp += "," + lista.ElementAt(i).Name;
                    }
                    elementos++;
                }
            }
            return resp;
        }
    }
}
