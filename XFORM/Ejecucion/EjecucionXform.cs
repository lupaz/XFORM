using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using XFORM.Analizador;

namespace XFORM.Ejecucion
{

    class retorno
    {
        public Object valor;
        public String tipo;
        public Boolean retorna;
        public Boolean detener;
        public Boolean continua;
        public String Linea;
        public String Columna;
        private String tipoDato;
        public retorno(Object valor, String tipo, String linea, String columna)
        {
            this.valor = valor;
            this.tipo = tipo;
            this.Linea = linea;
            this.Columna = columna;
            this.detener = false;
            this.retorna = false;
            this.continua = false;
            this.tipoDato = "";
        }
        public String TipoDato
        {
            get { return tipoDato; }
            set { tipoDato = value; }
        }
    }

    class EjecucionXform  
    {
        public Stack<TablaSimbolos> pilaSimbols; // pila actual
        public TablaSimbolos Global; // TS global de la primera instancia
        public TablaFunciones Funciones; // TB funciones actual
        public TablaPreguntas Preguntas;
        public ListaClases Clases;
        public Formulario Formulario;
        TablaSimbolos cima; // TS que esta en la cima
        TablaSimbolos Padre;//cuando llamamos a un objeto necesitamos tener acceso a las variables del la clase donde fue instanciado.
        Stack<TablaSimbolos> temporal;
        retorno aux2; //para los retornos 
        private Stack<VFO> pilaVFO; // la pilita sirve para controlorar los objetos papu XD
        VFO vfoActual; // vfo acual
        Clase claseActual; //clase actual
        FormPregunta formpreg;
        int limite_it;
        public static bool inwhile = false;
        public static bool salwhile = false;
        public RichTextBox consola;
        

        public EjecucionXform(RichTextBox consola) {
            this.pilaVFO = new Stack<VFO>();
            TablaSimbolos global = new TablaSimbolos("Global", Cadena.ambito_g, true, false,false);
            VFO vfoInicial = new VFO(global);
            this.pilaVFO.Push(vfoInicial);
            this.vfoActual = pilaVFO.Peek();
            this.Global = vfoActual.global;
            this.Funciones = vfoActual.funciones;
            this.pilaSimbols = vfoActual.pilaSimbolos;
            this.cima = pilaSimbols.Peek();
            this.Clases = new ListaClases();
            this.Preguntas = new TablaPreguntas();
            this.limite_it = 25;
            this.Formulario = null;
            this.formpreg = new FormPregunta();
            this.consola = consola;
            this.temporal = new Stack<TablaSimbolos>();
        }

        public retorno ejecutar(ParseTreeNode nodo) {
            switch (nodo.Term.Name)
            {                   
                case Cadena.LLAMADA:
                    #region
                    String name5 = nodo.ChildNodes[0].Token.Value.ToString();
                    String line5 = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                    String column5 = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                    List<retorno> parametros = new List<retorno>();
                    if (nodo.ChildNodes[1].ChildNodes.Count > 0)
                    {
                        foreach (ParseTreeNode subHijo in nodo.ChildNodes[1].ChildNodes)
                        {
                            retorno ret = ejecutarEXP(subHijo);
                            if (ret.tipo.ToLower().Equals(Cadena.error) || ret.tipo.ToLower().Equals(Cadena.Nulo.ToLower()))
                            {
                                String error = "ERROR SEMANTICO: Parametros incorrectos en la llamada a la funcion -> " + name5 + " L: " + line5 + " C: " + column5 + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, "0", "0");
                            }
                            parametros.Add(ret);
                        }
                    }
                    //armamos la llave de la funcion
                    String pars = "";
                    String key;
                    foreach (retorno ret in parametros)
                    {
                        pars += ret.tipo.ToLower();
                    }
                    key = name5 + "_" + pars; //esta es la llave de la funcion
                    Funcion func=Funciones.retornaFuncion(key);
                    if(func!=null){
                        //aca estaba la creacion de la tabla
                        String ambito = cima.Nivel+Cadena.fun+name5;
                        TablaSimbolos tab =  new TablaSimbolos(ambito,Cadena.ambito_fun,true,false,false);
                        pilaSimbols.Push(tab); //agregamos la nueva tabla de simbolos a la pila
                        cima =tab; // la colocamos en la cima
                        // se esta llamando a una funcion con parametros
                        if(parametros.Count>0){
                            //agregamos los parametros a la tb
                            for (int i = 0; i <func.Parametros.Count; i++)
			                {
                                retorno reto=parametros.ElementAt(i);
			                    Simbolo sim=new Simbolo(ambito,func.Parametros.ElementAt(i).Nombre,reto.valor,reto.tipo,reto.Linea,reto.Columna);
                                sim.TipoObjeto = reto.TipoDato;
                                cima.insertar(func.Parametros.ElementAt(i).Nombre,sim,claseActual.Nombre); //insertamos a la tabla el parametro

			                }
                            foreach (ParseTreeNode hijo in func.Cuerpo.ChildNodes)
	                        {
		                            retorno reto2=ejecutar(hijo);
                                    if(reto2.retorna){
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima=pilaSimbols.Peek();
                                        if (func.Tipo.ToLower().Equals(Cadena.Vacio.ToLower()) && !reto2.tipo.Equals(Cadena.Vacio))
                                        {
                                            String error = "ERROR SEMANTICO: No puede retornar expresiones en un metododo -> VACIO" + " L: " + reto2.Linea + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, "0", "0");
                                        }
                                        return reto2;
                                    }
                                    if(reto2.detener){ //detener fuera de ciclos, error semantico =
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima=pilaSimbols.Peek();
                                        String error = "ERROR SEMANTICO: Sentencia romper invalida, fuera de ciclos -> ROMPER " + " L: " + reto2.Linea + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, "0", "0");
                                    }
                                    if(reto2.continua){
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima=pilaSimbols.Peek();
                                        String error = "ERROR SEMANTICO: Sentencia continuar invalida, fuera de ciclos -> CONTINUAR " + " L: " + reto2.Columna + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, "0", "0");
                                    }
	                        }
                            pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                            cima=pilaSimbols.Peek();
                        }
                        //es una llamada a una funcion sin parametros
                        else{
                            foreach (ParseTreeNode hijo in func.Cuerpo.ChildNodes)
	                        {
		                            retorno reto2=ejecutar(hijo);
                                    if(reto2.retorna){
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima = pilaSimbols.Peek();
                                        if (func.Tipo.ToLower().Equals(Cadena.Vacio.ToLower()) && !reto2.tipo.Equals(Cadena.Vacio))
                                        {
                                            String error = "ERROR SEMANTICO: No puede retornar expresiones en un metododo -> EXP" + " L: " + reto2.Linea + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, "0", "0");
                                        }
                                        return reto2;
                                    }
                                    if(reto2.detener){ //detener fuera de ciclos, error semantico =
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima=pilaSimbols.Peek();
                                        String error = "ERROR SEMANTICO: Sentencia romper invalida, fuera de ciclos -> ROMPER " + " L: " + reto2.Linea + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, "0", "0");
                                    }
                                    if(reto2.continua){
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima=pilaSimbols.Peek();
                                        String error = "ERROR SEMANTICO: Sentencia continuar invalida, fuera de ciclos -> CONTINUAR " + " L: " + reto2.Columna + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, "0", "0");
                                    }
	                        }
                            pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                            cima=pilaSimbols.Peek();
                        }
                    }else{
                        String error = "ERROR SEMANTICO: La funcion invocada, no se encuntra definida -> " + key + " L: " + line5 + " C: " + column5 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return new retorno("error", Cadena.error, "0", "0");
                    }
                    #endregion
                    break;
                case Cadena.DEC_VAR_2:
                    capturarDecAsigs(nodo);
                    break;
                case Cadena.DEC_ASIGNA_MAT_2:
                    capturarDecAsigs(nodo);
                    break;
                case Cadena.ASIGNA:
                    capturarDecAsigs(nodo);
                    break;
                case Cadena.ASIGNA_MAT:
                    capturarDecAsigs(nodo);
                    break;
                case Cadena.RETORNO:
                    #region
                    String line0= (nodo.ChildNodes[0].Token.Location.Line+1)+"";
                    String column0 =  (nodo.ChildNodes[0].Token.Location.Column+1)+"";
                    
                    if(nodo.ChildNodes[1].ChildNodes.Count>0){
                        retorno r6 = ejecutarEXP(nodo.ChildNodes[1].ChildNodes[0]);
                        if (!r6.tipo.Equals(Cadena.error))
                        {
                            retorno r0 = new retorno(r6.valor, r6.tipo, r6.Linea, r6.Columna);
                            r0.retorna = true;
                            return r0;
                            //aca debo imprimir en la consola de la app
                        }
                        else {
                            r6.retorna = true;
                            return r6;
                        }
                    }else{
                        retorno r0=new retorno("vacio",Cadena.Vacio,line0,column0);
                        r0.retorna=true;
                        return r0;
                    }
                    #endregion
                case Cadena.ROMPER:
                    #region
                    String line1 = (nodo.ChildNodes[0].Token.Location.Line+1)+"";
                    String column1 = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                    retorno r1 = new retorno(Cadena.Romper,Cadena.Romper,line1,column1);
                    r1.detener=true;
                    return  r1;
                    #endregion
                case Cadena.CONTINUAR:
                    #region
                    String line2 = (nodo.ChildNodes[0].Token.Location.Line+1)+"";
                    String column2 = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                    retorno r2 = new retorno(Cadena.Romper,Cadena.Romper,line2,column2);
                    r2.continua=true;
                    #endregion
                    break;
                case Cadena.SI:
                    #region
                    String line3= (nodo.ChildNodes[0].Token.Location.Line+1)+"";
                    String column3=(nodo.ChildNodes[0].Token.Location.Column+1)+"";
                    retorno r3 = ejecutarEXP(nodo.ChildNodes[1]);                
                    if(r3.tipo.ToLower().Equals(Cadena.Booleano.ToLower())){//se comprueba que sea un booleano
                        if(r3.valor.ToString().ToLower().Equals("verdadero") || r3.valor.ToString().ToLower().Equals("'verdadero'")){
                            TablaSimbolos tab = new TablaSimbolos(cima.Nivel,Cadena.ambito_if,cima.retorno,cima.detener,cima.continuar);
                            pilaSimbols.Push(tab);
                            cima=tab;
                            foreach(ParseTreeNode hijo in nodo.ChildNodes[2].ChildNodes) {
                                retorno reto3 = ejecutar(hijo);
                                // aca preguntamos por lo return y break;
                                if(reto3.retorna){
                                    if(cima.retorno){ //
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                        cima=pilaSimbols.Peek();
                                        return reto3;
                                    }else{ //error no permite retornos
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                        cima=pilaSimbols.Peek();
                                        String error = "ERROR SEMANTICO: La sentencia RETORNO no es valida en el ambito que la envuelve." + " L: " + reto3.Linea + " C: " + reto3.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, "0", "0");
                                    }
                                }
                                if(reto3.detener){
                                   if(cima.detener){//si permite detener, entoces vemos  si es el ultimo hoy hay mas tablas sobre el
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                        cima=pilaSimbols.Peek();
                                        return  reto3;
                                   }else{// no permite detener es un error
                                        String error = "ERROR SEMANTICO: La sentencia DETENER no es valida en el ambito que la envuelve." + " L: " + reto3.Linea + " C: " + reto3.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, "0", "0");
                                   }  
                                }
                                if(reto3.continua){
                                   if(cima.continuar){//si permite detener, entoces vemos  si es el ultimo hoy hay mas tablas sobre el                                        
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                        cima=pilaSimbols.Peek();
                                        return  reto3;
                                   }else{// no permite detener es un error
                                        String error = "ERROR SEMANTICO: La sentencia CONTINUAR no es valida en el ambito que la envuelve." + " L: " + reto3.Linea + " C: " + reto3.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, "0", "0");
                                   }  
                                }
                            }
                            pilaSimbols.Pop();
                            cima=pilaSimbols.Peek();
                        }else{ 
                            foreach(ParseTreeNode hijo in nodo.ChildNodes[3].ChildNodes){                                                                      
                                if(hijo.ChildNodes.Count>2){//es un SINO SI                                      
                                    String line=(hijo.ChildNodes[0].Token.Location.Line+1)+"";
                                    String column= (hijo.ChildNodes[0].Token.Location.Column+1)+""; 
                                    retorno ret=ejecutarEXP(hijo.ChildNodes[2]);
                                    if(ret.tipo.ToLower().Equals(Cadena.Booleano.ToLower())){
                                        if(ret.valor.ToString().ToLower().Equals("verdadero") || ret.valor.ToString().ToLower().Equals("'verdadero'")){
                                            TablaSimbolos tab = new TablaSimbolos(cima.Nivel,Cadena.ambito_ifelse,cima.retorno,cima.detener,cima.continuar);
                                                pilaSimbols.Push(tab);
                                                cima=tab;
                                                foreach(ParseTreeNode sub_hijo in hijo.ChildNodes[3].ChildNodes) {
                                                    retorno reto3 = ejecutar(sub_hijo);
                                                    // aca preguntamos por lo return y break;
                                                    if(reto3.retorna){
                                                        if(cima.retorno){ //
                                                            pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                                            cima=pilaSimbols.Peek();
                                                            return reto3;
                                                        }else{ //error no permite retornos
                                                            pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                                            cima=pilaSimbols.Peek();
                                                            String error = "ERROR SEMANTICO: La sentencia RETORNO no es valida en el ambito que la envuelve." + " L: " + reto3.Linea + " C: " + reto3.Columna + " Clase: " + claseActual.Nombre;
                                                            Form1.listaErrores.Add(error);
                                                            Console.WriteLine(error);
                                                            return new retorno("error", Cadena.error, "0", "0");
                                                        }
                                                    }
                                                    if(reto3.detener){
                                                       if(cima.detener){//si permite detener, entoces vemos  si es el ultimo hoy hay mas tablas sobre el
                                                            pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                                            cima=pilaSimbols.Peek();
                                                            return  reto3;
                                                       }else{// no permite detener es un error
                                                            String error = "ERROR SEMANTICO: La sentencia DETENER no es valida en el ambito que la envuelve." + " L: " + reto3.Linea + " C: " + reto3.Columna + " Clase: " + claseActual.Nombre;
                                                            Form1.listaErrores.Add(error);
                                                            Console.WriteLine(error);
                                                            return new retorno("error", Cadena.error, "0", "0");
                                                       }  
                                                    }
                                                    if(reto3.continua){
                                                       if(cima.continuar){//si permite detener, entoces vemos  si es el ultimo hoy hay mas tablas sobre el                                        
                                                            pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                                            cima=pilaSimbols.Peek();
                                                            return  reto3;
                                                       }else{// no permite detener es un error
                                                            String error = "ERROR SEMANTICO: La sentencia CONTINUAR no es valida en el ambito que la envuelve." + " L: " + reto3.Linea + " C: " + reto3.Columna + " Clase: " + claseActual.Nombre;
                                                            Form1.listaErrores.Add(error);
                                                            Console.WriteLine(error);
                                                            return new retorno("error", Cadena.error, "0", "0");
                                                       }  
                                                    }
                                                }
                                                pilaSimbols.Pop();
                                                cima=pilaSimbols.Peek();
                                                break;        
                                        }
                                    }                                       
                                    else if(ret.tipo.Equals(Cadena.error)){                                     
                                        return ret;                                        
                                    }                                   
                                    else{
                                        String error = "ERROR SEMANTICO: Se debe evaluar una expresion booleana en la condicion del SINO SI -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        break;
                                    }
                                }else{ //es un SINO                                        
                                    TablaSimbolos tab = new TablaSimbolos(cima.Nivel, Cadena.ambito_else, cima.retorno, cima.detener, cima.continuar);
                                    pilaSimbols.Push(tab);
                                    cima = tab;
                                    foreach (ParseTreeNode sub_hijo in hijo.ChildNodes[1].ChildNodes)
                                    {
                                        retorno reto3 = ejecutar(sub_hijo);
                                        // aca preguntamos por lo return y break;
                                        if (reto3.retorna)
                                        {
                                            if (cima.retorno)
                                            { //
                                                pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                                cima = pilaSimbols.Peek();
                                                return reto3;
                                            }
                                            else
                                            { //error no permite retornos
                                                pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                                cima = pilaSimbols.Peek();
                                                String error = "ERROR SEMANTICO: La sentencia RETORNO no es valida en el ambito que la envuelve." + " L: " + reto3.Linea + " C: " + reto3.Columna + " Clase: " + claseActual.Nombre;
                                                Form1.listaErrores.Add(error);
                                                Console.WriteLine(error);
                                                return new retorno("error", Cadena.error, "0", "0");
                                            }
                                        }
                                        if (reto3.detener)
                                        {
                                            if (cima.detener)
                                            {//si permite detener, entoces vemos  si es el ultimo hoy hay mas tablas sobre el
                                                pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                                cima = pilaSimbols.Peek();
                                                return reto3;
                                            }
                                            else
                                            {// no permite detener es un error
                                                pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                                cima = pilaSimbols.Peek();
                                                String error = "ERROR SEMANTICO: La sentencia DETENER no es valida en el ambito que la envuelve." + " L: " + reto3.Linea + " C: " + reto3.Columna + " Clase: " + claseActual.Nombre;
                                                Form1.listaErrores.Add(error);
                                                Console.WriteLine(error);
                                                return new retorno("error", Cadena.error, "0", "0");
                                            }
                                        }
                                        if (reto3.continua)
                                        {
                                            if (cima.continuar)
                                            {//si permite detener, entoces vemos  si es el ultimo hoy hay mas tablas sobre el                                        
                                                pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                                cima = pilaSimbols.Peek();
                                                return reto3;
                                            }
                                            else
                                            {// no permite detener es un error
                                                pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                                cima = pilaSimbols.Peek();
                                                String error = "ERROR SEMANTICO: La sentencia CONTINUAR no es valida en el ambito que la envuelve." + " L: " + reto3.Linea + " C: " + reto3.Columna + " Clase: " + claseActual.Nombre;
                                                Form1.listaErrores.Add(error);
                                                Console.WriteLine(error);
                                                return new retorno("error", Cadena.error, "0", "0");
                                            }
                                        }
                                    }
                                    pilaSimbols.Pop();
                                    cima = pilaSimbols.Peek();
                                    break;
                                }
                            }
                        }
                    }else if(r3.tipo.Equals(Cadena.error)){
                        return r3;
                    }else{
                        //no retorno una expresion booleana
                        String error = "ERROR SEMANTICO: Se debe evaluar una expresion booleana en la condicion del SI -> " + " L: " + line3 + " C: " + column3 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                    }
                    break;
                    #endregion
                case Cadena.SELECCIONA:
                    #region
                    String line4=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                    String column4=(nodo.ChildNodes[0].Token.Location.Column+1)+"";
                    retorno r4 = ejecutarEXP(nodo.ChildNodes[1]); //obtenemos la expresion
                    if(r4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || r4.tipo.ToLower().Equals(Cadena.Cad.ToLower()) || r4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())){
                        bool mismoTipo=true;
                        foreach (ParseTreeNode sub in nodo.ChildNodes[3].ChildNodes)
	                    {
		                    if(!r4.tipo.ToLower().Equals(sub.ChildNodes[0].Term.Name.ToLower())){
                                mismoTipo=false; 
                                line4= (sub.ChildNodes[0].Token.Location.Line+1)+"";
                                column4=(sub.ChildNodes[0].Token.Location.Column+1)+"";
                                break;
                            }
	                    }
                        if(mismoTipo){
                            TablaSimbolos tab1 = new TablaSimbolos(cima.Nivel,Cadena.ambito_select,cima.retorno,true,cima.continuar);
                            pilaSimbols.Push(tab1);
                            cima=tab1;
                            retorno r5;
                            foreach(ParseTreeNode hijo in nodo.ChildNodes[3].ChildNodes) {  
                                if(r4.valor.Equals(hijo.ChildNodes[0].Token.Value)){//buscamos el caso que haga mach
                                    //aca recorremos las sentencias del caso
                                    foreach(ParseTreeNode sent in hijo.ChildNodes[1].ChildNodes) {
                                        r5= ejecutar(sent);
                                        if(r5.retorna){
                                           pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                           cima=pilaSimbols.Peek();
                                           return r5;
                                        }
                                        if(r5.detener){
                                            pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                            cima=pilaSimbols.Peek();
                                            return r5;
                                        }
                                        if(r5.continua){
                                            if (cima.continuar)
                                            {//si permite detener, entoces vemos  si es el ultimo hoy hay mas tablas sobre el                                        
                                                pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                                cima = pilaSimbols.Peek();
                                                return r5;
                                            }
                                            else
                                            {// no permite detener es un error
                                                pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                                cima = pilaSimbols.Peek();
                                                String error = "ERROR SEMANTICO: La sentencia CONTINUAR no es valida en el ambito que la envuelve." + " L: " + r5.Linea + " C: " + r5.Columna + " Clase: " + claseActual.Nombre;
                                                Form1.listaErrores.Add(error);
                                                Console.WriteLine(error);
                                                return new retorno("error", Cadena.error, "0", "0");
                                            }
                                        }
                                    }// si no vino retornar, continuar o romper sigue hechando verga
                                }
                            }
                            //ningun caso hizo mach si existe defecto lo ejecutamos
                            if(nodo.ChildNodes[4].ChildNodes.Count>0){
                                foreach(ParseTreeNode sent in nodo.ChildNodes[4].ChildNodes[1].ChildNodes) {
                                    r5= ejecutar(sent);
                                    if(r5.retorna){
                                       pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                       cima=pilaSimbols.Peek();
                                       return r5;                                        
                                    }
                                    if(r5.detener){
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                        cima=pilaSimbols.Peek();
                                        return r5; //a no sigo con lo del if
                                    }
                                    if (r5.continua)
                                    {
                                        if (cima.continuar)
                                        {//si permite detener, entoces vemos  si es el ultimo hoy hay mas tablas sobre el                                        
                                            pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                            cima = pilaSimbols.Peek();
                                            return r5;
                                        }
                                        else
                                        {// no permite detener es un error
                                            pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                            cima = pilaSimbols.Peek();
                                            String error = "ERROR SEMANTICO: La sentencia CONTINUAR no es valida en el ambito que la envuelve." + " L: " + r5.Linea + " C: " + r5.Columna + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, "0", "0");
                                        }
                                    }                                        
                                }
                            }
                            pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                            cima=pilaSimbols.Peek();
                        }else{
                            String error = "ERROR SEMANTICO: Exixte un valor en los casos que no es del mismo tipo del CASO evaluado -> " + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                        }
                    }else if(r4.tipo.Equals(Cadena.error)){
                        return r4;
                    }else{//no retorno una expresion booleana    
                        String error = "ERROR SEMANTICO: Se debe evaluar una expresion booleana/numerica/decimal en la condicion del CASO -> " + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                    }
                    break;
                    #endregion
                case Cadena.MIENTRAS:
                    #region
                    //evaluamos la condicion
                    String line7=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                    String column7= (nodo.ChildNodes[0].Token.Location.Column+1)+"";
                    retorno r7=ejecutarEXP(nodo.ChildNodes[1]); 
                    if(r7.tipo.ToLower().Equals(Cadena.Booleano.ToLower())){//aca todo va bien
                        int numero_it=0;
                        inwhile = true;
                        while (true) {                                
                                if((r7.valor.ToString().ToLower().Equals("verdadero") || r7.valor.ToString().ToLower().Equals("'verdadero'")) && numero_it<limite_it){
                                    //agregamos la nueva tabla de simbolos
                                    TablaSimbolos tab1 = new TablaSimbolos(cima.Nivel,Cadena.ambito_while,cima.retorno,true,true);
                                    pilaSimbols.Push(tab1);
                                    cima=tab1;
                                    foreach(ParseTreeNode hijo in nodo.ChildNodes[2].ChildNodes){
                                        retorno reto3 = ejecutar(hijo);
                                        // aca preguntamos por lo return y break;
                                        if(reto3.retorna){
                                            pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                            cima=pilaSimbols.Peek();
                                            inwhile = false;
                                            return reto3;
                                        }
                                        if(reto3.detener){                                           
                                            pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                            cima=pilaSimbols.Peek();
                                            reto3.detener = false;
                                            inwhile = false;
                                            return reto3; //a no sigo con lo del if 
                                        }
                                        if (reto3.continua) {
                                            break;
                                        }
                                        if (salwhile) {
                                            pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                            cima = pilaSimbols.Peek();
                                            salwhile = false;
                                            inwhile = false;
                                            return reto3;
                                        }
                                    }
                                    //despues de evaluar las sentecnias sacamos la tabla 
                                    pilaSimbols.Pop();
                                    cima=pilaSimbols.Peek();
                                }else{
                                    break;
                                }
                                //evaluamos la condicion de nuevo
                                numero_it++;
                                r7 = ejecutarEXP(nodo.ChildNodes[1]); //aca revalidamos la condicion
                                if (salwhile) { salwhile = false; break; }    
                            }
                        inwhile = false;
                    }else if(r7.tipo.Equals(Cadena.error)){
                        return r7;
                    }else{ 
                        //no retorno una expresion booleana
                        String error = "ERROR SEMANTICO: Se debe evaluar una expresion booleana en la condicion del MIENTRAS -> " + " L: " + line7 + " C: " + column7 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                    }
                    break;
                    #endregion
                case Cadena.HACER:
                    #region
                    //evaluamos la condicion
                    String line8 = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                    String column8 = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                    retorno r8 = ejecutarEXP(nodo.ChildNodes[3]);
                    if (r8.tipo.ToLower().Equals(Cadena.Booleano.ToLower()))
                    {//aca todo va bien
                        int numero_it = 0;
                        while (true)
                        {
                            //agregamos la nueva tabla de simbolos
                            TablaSimbolos tab1 = new TablaSimbolos(cima.Nivel, Cadena.ambito_while, cima.retorno, true, true);
                            pilaSimbols.Push(tab1);
                            cima = tab1;
                            foreach (ParseTreeNode hijo in nodo.ChildNodes[1].ChildNodes)
                            {
                                retorno reto3 = ejecutar(hijo);
                                // aca preguntamos por lo return y break;
                                if (reto3.retorna)
                                {
                                    pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                    cima = pilaSimbols.Peek();
                                    return reto3;
                                }
                                if (reto3.detener)
                                {
                                    pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                    cima = pilaSimbols.Peek();
                                    reto3.detener = false;
                                    return reto3; //a no sigo con lo del if 
                                }
                                if (reto3.continua)
                                {
                                    break;
                                }
                            }
                            //despues de evaluar las sentecnias sacamos la tabla 
                            pilaSimbols.Pop();
                            cima = pilaSimbols.Peek();
                            numero_it++;
                            r8 = ejecutarEXP(nodo.ChildNodes[3]);
                            if (!(r8.valor.ToString().ToLower().Equals("verdadero") || r8.valor.ToString().ToLower().Equals("'verdadero'")) || !(numero_it < limite_it))
                            {
                                break;  
                            }
                        }
                    }
                    else if (r8.tipo.Equals(Cadena.error))
                    {
                        return r8;
                    }
                    else
                    {
                        //no retorno una expresion booleana
                        String error = "ERROR SEMANTICO: Se debe evaluar una expresion booleana en la condicion del HACER-MIENTRAS -> " + " L: " + line8 + " C: " + column8 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                    }
                    break;
                    #endregion
                case Cadena.REPETIR:
                    #region
                    //evaluamos la condicion
                    String line9 = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                    String column9 = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                    retorno r9 = ejecutarEXP(nodo.ChildNodes[3]);
                    if (r9.tipo.ToLower().Equals(Cadena.Booleano.ToLower()))
                    {//aca todo va bien
                        int numero_it = 0;
                        while (true)
                        {
                            //agregamos la nueva tabla de simbolos
                            TablaSimbolos tab1 = new TablaSimbolos(cima.Nivel, Cadena.ambito_while, cima.retorno, true, true);
                            pilaSimbols.Push(tab1);
                            cima = tab1;
                            foreach (ParseTreeNode hijo in nodo.ChildNodes[1].ChildNodes)
                            {
                                retorno reto3 = ejecutar(hijo);
                                // aca preguntamos por lo return y break;
                                if (reto3.retorna)
                                {
                                    pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                    cima = pilaSimbols.Peek();
                                    reto3.detener = false;
                                    return reto3;
                                }
                                if (reto3.detener)
                                {
                                    pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                    cima = pilaSimbols.Peek();
                                    return reto3; //a no sigo con lo del if 
                                }
                                if (reto3.continua)
                                {
                                    break;
                                }
                            }
                            //despues de evaluar las sentecnias sacamos la tabla 
                            pilaSimbols.Pop();
                            cima = pilaSimbols.Peek();
                            numero_it++;
                            r9 = ejecutarEXP(nodo.ChildNodes[3]);
                            if ((r9.valor.ToString().ToLower().Equals("verdadero") || r9.valor.ToString().ToLower().Equals("'verdadero'")) || !(numero_it < limite_it))
                            {
                                break;
                            }
                        }
                    }
                    else if (r9.tipo.Equals(Cadena.error))
                    {
                        return r9;
                    }
                    else
                    {
                        //no retorno una expresion booleana
                        String error = "ERROR SEMANTICO: Se debe evaluar una expresion booleana en la condicion del HACER-MIENTRAS -> " + " L: " + line9 + " C: " + column9 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                    }
                    break;
                    #endregion
                case Cadena.PARA:
                    #region
                    String line10 = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                    String column10 = (nodo.ChildNodes[0].Token.Location.Column + 1) + ""; 
                    bool declara=false;
                    if(nodo.ChildNodes[1].ChildNodes.Count<3){ //aca es unicamente una asignacion
                        String name = nodo.ChildNodes[1].ChildNodes[0].Token.Value.ToString();
                        String line= (nodo.ChildNodes[1].ChildNodes[0].Token.Location.Line+1)+"";
                        String column= (nodo.ChildNodes[1].ChildNodes[0].Token.Location.Column+1)+"";
                        Simbolo sim =existeVariable2(name);
                        if(sim!=null){
                            if(sim.Tipo.ToLower().Equals(Cadena.Entero.ToLower()) || sim.Tipo.ToLower().Equals(Cadena.Decimmal.ToLower())){
                                retorno ret=ejecutarEXP(nodo.ChildNodes[1].ChildNodes[1]);
                                if(comprobarTipo(sim.Tipo,ret)){
                                    sim.Valor=ret.valor;
                                }else{
                                    String error = "ERROR SEMANTICO: Incompatibiidad de tipos en la asigacion del PARA -> " +name+ " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    break;
                                }
                            }else{
                                String error = "ERROR SEMANTICO: La variable a asignar en el PARA no es de tipo numerico -> " +name+ " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                break;
                            }
                        }else{
                            String error = "ERROR SEMANTICO: La variable a asignar en el PARA no esta definida -> " +name+ " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            break;
                        }
                    }else{
                        String tipo= nodo.ChildNodes[1].ChildNodes[0].Token.Value.ToString();
                        String name= nodo.ChildNodes[1].ChildNodes[1].Token.Value.ToString();
                        String line= (nodo.ChildNodes[1].ChildNodes[1].Token.Location.Line+1)+"";
                        String column= (nodo.ChildNodes[1].ChildNodes[1].Token.Location.Column+1)+"";
                        Simbolo sim=existeVariable2(name);
                        if(sim==null){
                            retorno ret=ejecutarEXP(nodo.ChildNodes[1].ChildNodes[2]);
                            if(comprobarTipo(tipo,ret)){
                                // creo una tabla de simbolos temporal para almacenar la variable del for
                                TablaSimbolos tab1 = new TablaSimbolos(cima.Nivel, Cadena.ambito_for+"_tmp", cima.retorno, true, true);
                                pilaSimbols.Push(tab1);
                                cima = tab1; 
                                sim = new Simbolo(cima.Nivel, name, ret.valor, tipo, line, column);
                                cima.insertar(name,sim,claseActual.Nombre);
                                declara=true;
                            }else{
                                String error = "ERROR SEMANTICO: Incompatibiidad de tipos en la Dec/asigacion del PARA -> " +name+ " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                break;
                            }
                        }else{
                            String error = "ERROR SEMANTICO: La variable a definir en el PARA ya esta definida -> " +name+ " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            break;
                        }
                    }
                    // si llego a este punto procedemos a ejecutar el cuerpo
                    retorno r10 = ejecutarEXP(nodo.ChildNodes[2]);
                    //si se cumple prodecemos a crear el abito que dura este ciclo
                    if(r10.tipo.ToLower().Equals(Cadena.Booleano.ToLower())){//aca todo va bien
                        while (true) {                                
                            if(r10.valor.ToString().ToLower().Equals("verdadero") || r10.valor.ToString().ToLower().Equals("'verdadero'")){
                                TablaSimbolos tab1 = new TablaSimbolos(cima.Nivel,Cadena.ambito_for,cima.retorno,true,true);
                                pilaSimbols.Push(tab1);
                                cima=tab1;
                                //ejecutamos todas las sentencias internas 
                                foreach(ParseTreeNode  hijo in nodo.ChildNodes[4].ChildNodes) {
                                    retorno reto3 = ejecutar(hijo);
                                    // aca preguntamos por lo return y break;
                                    if(reto3.retorna){
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                        cima=pilaSimbols.Peek();
                                        if(declara){
                                            pilaSimbols.Pop();
                                            cima=pilaSimbols.Peek();   
                                        } 
                                        return reto3;
                                    }
                                    if(reto3.detener){                                           
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos del if que se inserto
                                        cima=pilaSimbols.Peek();
                                        if(declara){
                                            pilaSimbols.Pop();
                                            cima=pilaSimbols.Peek();   
                                        }
                                        return reto3; //a no sigo con lo del if 
                                    }
                                    if (reto3.continua){
                                        break;
                                    }
                                }
                                //sacamos la tabla del ambito que ya termino
                                pilaSimbols.Pop();
                                cima=pilaSimbols.Peek();
                            }else{
                                if(declara){
                                    pilaSimbols.Pop();
                                    cima=pilaSimbols.Peek();   
                                }
                                break;
                            }
                            //ejecutamos el amunto o decremento segun sea el caso.
                            if (ejecutarEXP(nodo.ChildNodes[3]).tipo.Equals(Cadena.error))
                            {//si es un error nos salimos
                                if (declara)
                                {
                                    pilaSimbols.Pop();
                                    cima = pilaSimbols.Peek();
                                }
                                break;
                            }
                            //re evaluamos la condicion
                            r10 = ejecutarEXP(nodo.ChildNodes[2]);
                        }
                    }else if(r10.tipo.Equals(Cadena.error)){
                        if(declara){
                            pilaSimbols.Pop();
                            cima=pilaSimbols.Peek();   
                        } 
                        return r10;
                    }else{//no retorno una expresion booleana
                        if(declara){
                            pilaSimbols.Pop();
                            cima=pilaSimbols.Peek();   
                        }
                        String error = "ERROR SEMANTICO: Se debe evaluar una expresion booleana en la condicion del PARA -> " + " L: " + line10 + " C: " + column10 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                    }
                    break;
                    #endregion
                case Cadena.IMPRIMIR:
                    #region
                    retorno r11 = ejecutarEXP(nodo.ChildNodes[1]);
                    if (!r11.tipo.Equals(Cadena.error)) { 
                        //aca debo imprimir en la consola del programa
                        Console.WriteLine("IMPRIMIR----> " + r11.valor.ToString());
                        notificar(r11.valor.ToString());
                    }
                    break;
                    #endregion
                case Cadena.MENSAJE:
                    #region
                    retorno r12 = ejecutarEXP(nodo.ChildNodes[1]);
                    if (!r12.tipo.Equals(Cadena.error)) { 
                        MessageBox.Show(r12.valor.ToString(), "MENSAJE",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    break;
                    #endregion
                case Cadena.ACC_OBJ:
                    #region
                    String name11 = nodo.ChildNodes[0].Token.Value.ToString();
                    String line11 = (nodo.ChildNodes[0].Token.Location.Line+1)+"";
                    String column11 = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                    Simbolo sim1 = existeVariable2(name11);
                    if (sim1 != null)
                    {
                        if (sim1.TipoObjeto.Equals(Cadena.Objeto))
                        {
                            //seteamos el nuevo entorno del objeto
                            this.pilaVFO.Push((VFO)sim1.Valor);
                            this.vfoActual = pilaVFO.Peek();
                            this.Global = vfoActual.global;
                            this.Funciones = vfoActual.funciones;
                            this.pilaSimbols = vfoActual.pilaSimbolos;
                            this.cima = pilaSimbols.Peek();
                            this.claseActual = ((VFO)sim1.Valor).clase;
                            int cambio_entorno=0;
                            retorno val_ret=new retorno("error",Cadena.error,"0101","0101");
                            int penultimo_hijo = nodo.ChildNodes[1].ChildNodes.Count - 2;
                            for (int i = 0; i < nodo.ChildNodes[1].ChildNodes.Count; i++)
                            {
                                ParseTreeNode hijo = nodo.ChildNodes[1].ChildNodes[i];
                                if (hijo.ChildNodes.Count == 1)//puede ser un variable u otro objeto
                                {
                                    String nombre = hijo.ChildNodes[0].Token.Value.ToString();
                                    String line = (hijo.ChildNodes[0].Token.Location.Line + 1) + "";
                                    String column = (hijo.ChildNodes[0].Token.Location.Column + 1) + "";
                                    Simbolo sim2 = existeVariable2(nombre);
                                    if (sim2 != null)
                                    {
                                        if (sim2.TipoObjeto.Equals(""))//es una variable
                                        {
                                            if (penultimo_hijo == i || penultimo_hijo == -1)
                                            {
                                                if (!sim2.Visibilidad.ToLower().Equals(Cadena.Privado.ToLower()))
                                                {
                                                    if (nodo.ChildNodes.Count > 2) {
                                                        retorno ret = ejecutarEXP(nodo.ChildNodes[2]);
                                                        if (comprobarTipo(sim2.Tipo, ret))
                                                        {
                                                            sim2.Valor = ret.valor;
                                                        }
                                                        else {
                                                            String error = "ERROR SEMANTICO: Incompatibilidad de tipos en la asignacion de la variable de OBJETO -> " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                                            Form1.listaErrores.Add(error);
                                                            Console.WriteLine(error);  
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    String error = "ERROR SEMANTICO: El atributo del objeto al que intenta acceder es PRIVADO ->  " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                                    Form1.listaErrores.Add(error);
                                                    Console.WriteLine(error);
                                                    val_ret = new retorno("error", Cadena.error, "0", "0");
                                                    break;
                                                }
                                            }
                                            else {
                                                String error = "ERROR SEMANTICO: El atributo del objeto al que intenta acceder no es un objeto ->  " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                                Form1.listaErrores.Add(error);
                                                Console.WriteLine(error);
                                                val_ret = new retorno("error", Cadena.error, "0", "0");
                                                break;
                                            }
                                        }
                                        //es un objeto papu XD
                                        else {
                                            //retorno el objeto 
                                            if (i == penultimo_hijo || penultimo_hijo == -1)
                                            {
                                                if (nodo.ChildNodes.Count > 2)
                                                {
                                                    retorno ret = ejecutarEXP(nodo.ChildNodes[2]);
                                                    if (comprobarTipo(sim2.Tipo, ret))
                                                    {
                                                        sim2.Valor = ret.valor;
                                                    }
                                                    else
                                                    {
                                                        String error = "ERROR SEMANTICO: Incompatibilidad de tipos en la asignacion del objeto de OBJETO -> " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                                        Form1.listaErrores.Add(error);
                                                        Console.WriteLine(error);
                                                    }
                                                }
                                            }
                                            //si no es el penultimo realizo el cambio de entorno 
                                            else {
                                                this.pilaVFO.Push((VFO)sim2.Valor);
                                                this.vfoActual = pilaVFO.Peek();
                                                this.Global = vfoActual.global;
                                                this.Funciones = vfoActual.funciones;
                                                this.pilaSimbols = vfoActual.pilaSimbolos;
                                                this.cima = pilaSimbols.Peek();
                                                this.claseActual = ((VFO)sim2.Valor).clase;
                                                cambio_entorno++;
                                            }
                                        }
                                    }

                                    else
                                    {
                                        String error = "ERROR SEMANTICO: El atributo del objeto al que intenta acceder no esta definido ->  " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        val_ret =new retorno("error", Cadena.error, "0", "0");
                                        break;
                                    }
                                }
                                //puede ser una matriz o una llamda a funcion
                                else{
                                    String nombre=hijo.ChildNodes[0].Token.Value.ToString();
                                    String line= (hijo.ChildNodes[0].Token.Location.Line+1)+"";
                                    String column = (hijo.ChildNodes[0].Token.Location.Column + 1) + "";
                                    pasarVarTemporal();
                                    if (hijo.ChildNodes[1].Term.Name.Equals(Cadena.L_EXP)) //si entra aca es una llamada a una funcion.
                                    {
                                        NonTerminal llamada = new NonTerminal(Cadena.LLAMADA);
                                        SourceSpan span = new SourceSpan();
                                        ParseTreeNode node = new ParseTreeNode(llamada, span);
                                        node.ChildNodes.Add(hijo.ChildNodes[0]);
                                        node.ChildNodes.Add(hijo.ChildNodes[1]);
                                        val_ret = ejecutar(node);
                                        if (val_ret.tipo.Equals(Cadena.error))
                                        {
                                            temporal.Pop();
                                            break;
                                        }
                                        //la funcion retorno una variable primitiva y no es el ultimo nivel de acceso al objeto
                                        if(validarPrimitivo(val_ret.tipo) && i<penultimo_hijo){
                                            String error = "ERROR SEMANTICO: El atributo del objeto al que intenta acceder no es un objeto ->  " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            val_ret = new retorno("error", Cadena.error, "0", "0");
                                            temporal.Pop();
                                            break;
                                        }                                    
                                        //retorno un objeto y hay un nivel mas de acceso a este objeto
                                        if (!validarPrimitivo(val_ret.tipo) && i < penultimo_hijo && !val_ret.tipo.Equals(Cadena.Nulo)) {
                                            this.pilaVFO.Push((VFO)val_ret.valor);
                                            this.vfoActual = pilaVFO.Peek();
                                            this.Global = vfoActual.global;
                                            this.Funciones = vfoActual.funciones;
                                            this.pilaSimbols = vfoActual.pilaSimbolos;
                                            this.cima = pilaSimbols.Peek();
                                            this.claseActual = ((VFO)val_ret.valor).clase;
                                            cambio_entorno++;
                                        }
                                    }
                                    //es el acceso a una matriz
                                    else {
                                        if (i == penultimo_hijo || penultimo_hijo == -1)
                                        {
                                            Simbolo tmp_sim = existeVariable2(nombre);
                                            if (tmp_sim != null)
                                            {
                                                if (tmp_sim.TipoObjeto.Equals(Cadena.Matriz))//validamos que sea de tipo matriz
                                                {
                                                    int dim2 = hijo.ChildNodes[1].ChildNodes.Count;
                                                    if (tmp_sim.dimenciones.Count == dim2) //validamos que sean de la misma dimension 
                                                    {
                                                        retorno ret = ejecutarEXP(nodo.ChildNodes[2]);
                                                        if (comprobarTipo(tmp_sim.Tipo, ret))//validamos que sea del mismo tipo
                                                        {
                                                            List<Object> indices = new List<object>();
                                                            capturarVals(indices, hijo.ChildNodes[1]);
                                                            if (validarIndices(indices, tmp_sim.dimenciones))
                                                            {
                                                                int indice = linealizar(indices, tmp_sim.dimenciones);
                                                                ((List<Object>)tmp_sim.Valor)[indice] = ret.valor;
                                                            }//lo errores los indico en el metodo validarIndices :)
                                                        }
                                                        else
                                                        { // incoptibilidad de tipos
                                                            String error = "ERROR SEMANTICO: Incopatibilidad de tipos en la asignacion de la matriz -> " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                                            Form1.listaErrores.Add(error);
                                                            Console.WriteLine(error);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        String error = "ERROR SEMANTICO: Las dimensiones de la matriz no coinciden -> " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                                        Form1.listaErrores.Add(error);
                                                        Console.WriteLine(error);
                                                    }
                                                }
                                                else
                                                {
                                                    String error = "ERROR SEMANTICO: La variable a asignar no es de tipo matriz -> " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                                    Form1.listaErrores.Add(error);
                                                    Console.WriteLine(error);
                                                }
                                            }
                                            else
                                            {
                                                String error = "ERROR SEMANTICO: La variable no ha sido definida -> " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                                Form1.listaErrores.Add(error);
                                                Console.WriteLine(error);
                                            }
                                        }
                                        else{
                                            NonTerminal llamada = new NonTerminal(Cadena.ARIT);
                                            SourceSpan span = new SourceSpan();
                                            ParseTreeNode node = new ParseTreeNode(llamada, span);
                                            node.ChildNodes.Add(hijo.ChildNodes[0]);
                                            node.ChildNodes.Add(hijo.ChildNodes[1]);
                                            val_ret = ejecutarEXP(node);
                                            if (val_ret.tipo.Equals(Cadena.error))
                                            {
                                                temporal.Pop();
                                                break;
                                            }
                                            //la funcion retorno una variable primitiva y no es el ultimo nivel de acceso al objeto
                                            if (validarPrimitivo(val_ret.tipo) && i < penultimo_hijo)
                                            {
                                                String error = "ERROR SEMANTICO: El atributo del objeto al que intenta acceder no es un objeto ->  " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                                Form1.listaErrores.Add(error);
                                                Console.WriteLine(error);
                                                val_ret = new retorno("error", Cadena.error, "0", "0");
                                                temporal.Pop();
                                                break;
                                            }
                                            //retorno un objeto y hay un nivel mas de acceso a este objeto
                                            if (!validarPrimitivo(val_ret.tipo) && i < penultimo_hijo && !val_ret.tipo.Equals(Cadena.Nulo))
                                            {
                                                this.pilaVFO.Push((VFO)val_ret.valor);
                                                this.vfoActual = pilaVFO.Peek();
                                                this.Global = vfoActual.global;
                                                this.Funciones = vfoActual.funciones;
                                                this.pilaSimbols = vfoActual.pilaSimbolos;
                                                this.cima = pilaSimbols.Peek();
                                                this.claseActual = ((VFO)val_ret.valor).clase;
                                                cambio_entorno++;
                                            } 
                                        }  
                                    }
                                    temporal.Pop();
                                }
                            }
                            //aca va ir el for que va sacar todos los entornos de la pila
                            for (int i = 0; i < cambio_entorno; i++)
                            {
                                this.pilaVFO.Pop();
                                this.vfoActual = pilaVFO.Peek();
                                this.Global = vfoActual.global;
                                this.Funciones = vfoActual.funciones;
                                this.pilaSimbols = vfoActual.pilaSimbolos;
                                this.cima = pilaSimbols.Peek();
                                this.claseActual = vfoActual.clase;
                            }
                            this.pilaVFO.Pop();
                            this.vfoActual = pilaVFO.Peek();
                            this.Global = vfoActual.global;
                            this.Funciones = vfoActual.funciones;
                            this.pilaSimbols = vfoActual.pilaSimbolos;
                            this.cima = pilaSimbols.Peek();
                            this.claseActual = vfoActual.clase;
                            break;
                        }
                        else {
                            String error = "ERROR SEMANTICO: La instancia a la que intenta acceder no es de tipo OBJETO ->  " + name11 + " L: " + line11 + " C: " + column11 + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                        }
                    }
                    else {
                        String error = "ERROR SEMANTICO: El objeto al que intenta acceder no esta definido ->  " + name11 + " L: " + line11 + " C: " + column11 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                    }
                    #endregion
                    break;
                case Cadena.OPCIONES:
                    #region
                    String name14 = nodo.ChildNodes[1].Token.Value.ToString();
                    String line14 = (nodo.ChildNodes[1].Token.Location.Line + 1) + "";
                    String column14 = (nodo.ChildNodes[1].Token.Location.Line + 1) + "";
                    Simbolo sim14 = existeVariable2(name14);
                    if (sim14 == null)
                    {
                        Opciones op = new Opciones();
                        sim14 = new Simbolo(cima.Nivel, name14, op, Cadena.Opciones, line14, column14);
                        sim14.TipoObjeto = Cadena.Opciones;
                        cima.insertar(name14, sim14, claseActual.Nombre);
                    }
                    else
                    {
                        String error = "ERROR SEMANTICO: La variable ya se encuentra definida -> " + name14 + " L: " + line14 + " C: " + column14 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                    }
                    break;
                    #endregion
                case Cadena.ADD_OP:
                    #region
                    String name15 = nodo.ChildNodes[0].Token.Value.ToString();
                    String line15 = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                    String column15 = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                    Simbolo sim15 = existeVariable2(name15);
                    if (sim15 != null)
                    {
                        if (sim15.TipoObjeto.Equals(Cadena.Opciones))
                        {
                            List<retorno> expresiones = new List<retorno>();
                            foreach (ParseTreeNode subHijo in nodo.ChildNodes[2].ChildNodes)
                            {
                                retorno ret = ejecutarEXP(subHijo);
                                if (ret.tipo.ToLower().Equals(Cadena.error))
                                {
                                    String error = "ERROR SEMANTICO: No se agrego el parametro a la lista de opciones  -> " + ret.valor.ToString() + " L: " + ret.Linea + " C: " + ret.Columna + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                }
                                expresiones.Add(ret);
                            }
                            ((Opciones)sim15.Valor).insertar(expresiones);
                        }
                        else
                        {
                            String error = "ERROR SEMANTICO: La varible done desea insertar no es de tipo OPCIONES -> " + name15 + " L: " + line15 + " C: " + column15 + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                        }
                    }
                    else
                    {
                        String error = "ERROR SEMANTICO: La lista de OPCIONES no ha sido definida -> " + name15 + " L: " + line15 + " C: " + column15 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                    }
                    break;
                    #endregion
                case Cadena.OP:
                    ejecutarEXP(nodo);
                    break;
                case Cadena.FORM:
                    #region
                    String name16 = nodo.ChildNodes[1].Token.Value.ToString();
                    String line16 = (nodo.ChildNodes[1].Token.Location.Line +1)+"";
                    String column16 = (nodo.ChildNodes[1].Token.Location.Column + 1) + "";
                    if (Formulario.nombre.ToLower().Equals(name16.ToLower()))
                    {
                        String ambito = cima.Nivel + Cadena.fun + "formulario";
                        TablaSimbolos tab = new TablaSimbolos(ambito, Cadena.ambito_fun, true, false, false);
                        pilaSimbols.Push(tab); //agregamos la nueva tabla de simbolos a la pila
                        cima = tab; // la colocamos en la cima
                        //aca ejecuta las sentncias del metodo principal
                        foreach (ParseTreeNode sub_hijo in Formulario.cuerpo.ChildNodes)
                        {
                            retorno reto2 = ejecutar(sub_hijo);
                            if (reto2.retorna)
                            {
                                if (!reto2.tipo.Equals(Cadena.Vacio))
                                {
                                    String error = "ERROR SEMANTICO: No puede retornar expresiones en el metodo PRINCIPAL -> EXP" + " L: " + reto2.Linea + " C: " + reto2.Columna;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                }
                                break;
                            }
                            if (reto2.detener)
                            { //detener fuera de ciclos, error semantico =
                                String error = "ERROR SEMANTICO: Sentencia romper invalida, fuera de ciclos -> ROMPER " + " L: " + reto2.Linea + " C: " + reto2.Columna;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                break;
                            }
                            if (reto2.continua)
                            {
                                String error = "ERROR SEMANTICO: Sentencia continuar invalida, fuera de ciclos -> CONTINUAR " + " L: " + reto2.Columna + " C: " + reto2.Columna;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                break;
                            }
                        }
                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                        cima = pilaSimbols.Peek();                        
                    }
                    else {
                        String error = "ERROR SEMANTICO: El FORMULARIO al que desea acceder no esta definido -> " + name16 + " L: " + line16 + " C: " + column16 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                    }
                    break;
                    #endregion
                case Cadena.ACC_PRE: //acceso a las preguntas
                    #region
                    //Vamos a acceder a la pregunta
                    String nombr = nodo.ChildNodes[0].Token.Value.ToString();
                    String line17 = (nodo.ChildNodes[0].Token.Location.Line +1)+"";
                    String column17 = (nodo.ChildNodes[0].Token.Location.Column+1)+"";
                    Pregunta pre = Preguntas.retornaPregunta(nombr);
                    if (pre != null)
                    {
                        //Capturamos los parametros de la pregunta
                        #region
                        List<retorno> parametros2 = new List<retorno>();
                        if (nodo.ChildNodes[1].ChildNodes.Count > 0)
                        {
                            foreach (ParseTreeNode subHijo in nodo.ChildNodes[1].ChildNodes)
                            {
                                retorno ret = ejecutarEXP(subHijo);
                                if (ret.tipo.ToLower().Equals(Cadena.error) || ret.tipo.ToLower().Equals(Cadena.Nulo))
                                {
                                    String error = "ERROR SEMANTICO: Parametros incorrectos en la llamada a la pregunta -> " + nombr + " L: " + line17 + " C: " + column17 + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno(0, "correcto", "101", "101");
                                }
                                parametros2.Add(ret);
                            }
                        }
                        #endregion
                        //aca estaba la creacion de la tabla
                        String ambito = cima.Nivel; //+ Cadena.fun + nombr; se lo queite para  que comparta ambito con la pregunta
                        TablaSimbolos tab = new TablaSimbolos(ambito, Cadena.ambito_fun, true, false, false);
                        pilaSimbols.Push(tab); //agregamos la nueva tabla de simbolos a la pila
                        cima = tab; // la colocamos en la cima
                        // se esta llamando a una funcion con parametros
                        if (parametros2.Count > 0)
                        {
                            //agregamos los parametros a la tb
                            for (int i = 0; i < pre.Parametros.Count; i++)
                            {
                                retorno reto = parametros2.ElementAt(i);
                                Simbolo sim = new Simbolo(ambito, pre.Parametros.ElementAt(i).Nombre, reto.valor, reto.tipo, reto.Linea, reto.Columna);
                                cima.insertar(pre.Parametros.ElementAt(i).Nombre, sim, claseActual.Nombre); //insertamos a la tabla el parametro

                            }
                            foreach (ParseTreeNode hijo in pre.cuerpo.ChildNodes)
                            {
                                retorno reto2 = new retorno(0, "correcto", "101", "101");
                                //capturamos el metodo de Respuesta/Calcular/Mostrar
                                #region
                                if (hijo.Term.Name.Equals(Cadena.MET_PREG))
                                {
                                    String namem = hijo.ChildNodes[1].Token.Value.ToString();
                                    String visibilidad = hijo.ChildNodes[0].Token.Value.ToString();
                                    //
                                    key = namem.ToLower();
                                    if (pre.funcion == null)
                                    {
                                        //metodo o funcion con parametros
                                        if (hijo.ChildNodes[2].ChildNodes.Count > 0)
                                        {
                                            List<String> param = new List<string>();
                                            //comprobamos que no tenga parametros repetidos
                                            foreach (ParseTreeNode subHijo in hijo.ChildNodes[3].ChildNodes)
                                            {
                                                if (!param.Contains(subHijo.ChildNodes[1].Token.Value.ToString()))
                                                {
                                                    param.Add(subHijo.ChildNodes[1].Token.Value.ToString());
                                                }
                                                else
                                                {//error nombre de parametro repetido
                                                    String error = "ERROR SEMANTICO: No se agrego el metodo le metodo -> " + namem + " a la PREGUNTA, por parametro repetido:  L: " + subHijo.ChildNodes[1].Token.Location.Line + " C: " + subHijo.ChildNodes[1].Token.Location.Column + " Clase: " + claseActual.Nombre;
                                                    Form1.listaErrores.Add(error);
                                                    Console.WriteLine(error);
                                                    return new retorno(0, "correcto", "101", "101");
                                                }
                                            }
                                            // no hay parametros repetidos, creamos la funcion
                                            Funcion fun = new Funcion(visibilidad, namem, namem, hijo.ChildNodes[3]);
                                            foreach (ParseTreeNode subHijo in hijo.ChildNodes[3].ChildNodes)
                                            {
                                                fun.addParametro(subHijo.ChildNodes[1].Token.Value.ToString(), subHijo.ChildNodes[0].Token.Value.ToString());
                                            }
                                            pre.funcion = fun;
                                        }
                                        else  //metodo o funcion sin parametros
                                        {
                                            pre.funcion = new Funcion(visibilidad, namem, namem, hijo.ChildNodes[3]);
                                        }
                                    }
                                    else
                                    {//error la funcion ya fue insertada
                                        //String error = "ERROR SEMANTICO: No se agrego funcion -> " + namem + " por que ya fue definida:  L: " + hijo.ChildNodes[1].Token.Location.Line + " C: " + hijo.ChildNodes[1].Token.Location.Column + " Clase: " + claseActual.Nombre;
                                        //Form1.listaErrores.Add(error);
                                        //Console.WriteLine(error);
                                    }
                                }
                                else
                                {
                                    ejecutar(hijo);
                                }
                                #endregion
                                //validamos el retorno,break,continue
                                #region
                                if (reto2.retorna)
                                {
                                    pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                    cima = pilaSimbols.Peek();
                                    if (!reto2.tipo.Equals(Cadena.Vacio) && !reto2.tipo.Equals(Cadena.error))
                                    {
                                        String error = "ERROR SEMANTICO: No puede retornar expresiones en un metododo -> VACIO" + " L: " + reto2.Linea + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno(0, "correcto", "101", "101");
                                    }
                                    return new retorno(0, "correcto", "101", "101");
                                }
                                if (reto2.detener)
                                { //detener fuera de ciclos, error semantico =
                                    pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                    cima = pilaSimbols.Peek();
                                    String error = "ERROR SEMANTICO: Sentencia romper invalida, fuera de ciclos -> ROMPER " + " L: " + reto2.Linea + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno(0, "correcto", "101", "101");
                                }
                                if (reto2.continua)
                                {
                                    pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                    cima = pilaSimbols.Peek();
                                    String error = "ERROR SEMANTICO: Sentencia continuar invalida, fuera de ciclos -> CONTINUAR " + " L: " + reto2.Columna + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno(0, "correcto", "101", "101");
                                }
                                #endregion
                            }
                            //si llego a aca todo va bien
                            //aca se debe saber que tipo de pregunta se va realizar, luego ejecutar el metodo de la pregunta
                            String TIPO = "";
                            String RESPUESTA = "";
                            if (nodo.ChildNodes[2].ChildNodes.Count > 1)
                            {//pude ser condicion, cadena,selecionar,rango,entero,decimal,fecha,hora,fechahora,mostar
                                #region
                                ParseTreeNode tipo = nodo.ChildNodes[2].ChildNodes[1];
                                ParseTreeNode metod = nodo.ChildNodes[2].ChildNodes[0];
                                Simbolo simo;
                                bool ejecuta = true; //si es multimedia no ejecuta
                                //aca obtenemos los valores por defecto de la pregunta, como etiqueta, sugerir, etc
                                #region
                                simo = existeVariable2("etiqueta");
                                String etiqueta = "";
                                if (simo != null)
                                {
                                    etiqueta = simo.Valor.ToString();
                                }
                                String sugerir = "";
                                simo = existeVariable2("sugerir");
                                if (simo != null)
                                {
                                    sugerir = simo.Valor.ToString();
                                }
                                bool requerido = false;
                                simo = existeVariable2("requerido");
                                if (simo != null)
                                {
                                    if (simo.Valor.ToString().ToLower().Equals("'verdadero'") || simo.Valor.ToString().ToLower().Equals("verdadero"))
                                        requerido = true;
                                }
                                String requeridoMsg = "";
                                simo = existeVariable2("requeridomsn");
                                if (simo != null)
                                {
                                    requeridoMsg = simo.Valor.ToString();
                                }
                                bool enable = true; // sololec
                                if (pre.funcion.Visibilidad.ToLower().Equals("privado"))
                                    enable = false;
                                simo = existeVariable2("respuesta");
                                if (simo != null)
                                {
                                    TIPO = simo.Tipo;
                                }
                                #endregion
                                //se mostraran los imputs para las respuestas
                                #region
                                switch (tipo.ChildNodes[0].Token.Value.ToString().ToLower())
                                {
                                    case "cadena":
                                        #region
                                        if (tipo.ChildNodes[1].ChildNodes.Count > 0)
                                        { //traer parametros
                                            List<retorno> paras = new List<retorno>();
                                            foreach (ParseTreeNode subHijo in tipo.ChildNodes[1].ChildNodes)
                                            {
                                                retorno ret = ejecutarEXP(subHijo);
                                                paras.Add(ret);
                                            }
                                            String cad_min = paras.ElementAt(0).valor.ToString();
                                            String cad_max = paras.ElementAt(1).valor.ToString();
                                            String num_fil = paras.ElementAt(2).valor.ToString();
                                            RESPUESTA = formpreg.ShowCadenaMulti("Preguta - Cadena Parametros", etiqueta, sugerir, requerido, requeridoMsg, cad_max, cad_min, num_fil, enable);
                                        }
                                        else
                                        {
                                            RESPUESTA = formpreg.ShowCadenaSimple("Pregunta - Cadena Simple", etiqueta, sugerir, requerido, requeridoMsg, enable);
                                        }
                                        break;
                                        #endregion
                                    case "seleccionar":
                                        #region
                                        retorno ops = ejecutarEXP(tipo.ChildNodes[1].ChildNodes[0]);
                                        if (ops.tipo.ToLower().Equals(Cadena.Opciones.ToLower())){                                                                                      
                                            RESPUESTA = formpreg.ShowmMulti("Pregunta - Selecion Multiple", etiqueta,sugerir, requerido, requeridoMsg, (Opciones)ops.valor, enable);
                                        }
                                        else
                                        {
                                            String error = "ERROR SEMANTICO: No existe la lista de opcionees para selecionar_1 -> " + ops.valor.ToString() + " L: " + ops.Linea + " C: " + ops.Columna + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                        }
                                        break;
                                        #endregion
                                    case "seleccionar_1":
                                        #region
                                        retorno ops2 = ejecutarEXP(tipo.ChildNodes[1].ChildNodes[0]);
                                        if (ops2.tipo.ToLower().Equals(Cadena.Opciones.ToLower())){                                                                                      
                                            RESPUESTA = formpreg.ShowUnica("Pregunta - Selecion Unica", etiqueta,sugerir, requerido, requeridoMsg, (Opciones)ops2.valor, enable);
                                        }
                                        else
                                        {
                                            String error = "ERROR SEMANTICO: No existe la lista de opcionees para selecionar_1 -> " + ops2.valor.ToString() + " L: " + ops2.Linea + " C: " + ops2.Columna + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                        }
                                        break;
                                        #endregion
                                    case "rango":
                                        #region
                                        if (tipo.ChildNodes[1].ChildNodes.Count > 0)
                                        { //traer parametros
                                            List<retorno> paras = new List<retorno>();
                                            foreach (ParseTreeNode subHijo in tipo.ChildNodes[1].ChildNodes)
                                            {
                                                retorno ret = ejecutarEXP(subHijo);
                                                paras.Add(ret);
                                            }
                                            int ini;
                                            int fin;
                                            if (paras.ElementAt(0).tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                ini = Int32.Parse(paras.ElementAt(0).valor.ToString());
                                            else
                                                ini = 0;

                                            if (paras.ElementAt(1).tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                fin = Int32.Parse(paras.ElementAt(1).valor.ToString());
                                            else
                                                fin = 20;
                                            RESPUESTA = formpreg.ShowRango("Pregunta - Rango", etiqueta, sugerir, ini, fin, enable);
                                        }
                                        break;
                                        #endregion
                                    case "condicion":
                                        RESPUESTA = formpreg.ShowCondicion("Pregunta - Condicion", etiqueta, sugerir, requerido, requeridoMsg, enable);
                                        break;
                                    case "entero":
                                        RESPUESTA = formpreg.ShowNumerico("Pregunta - Entero", etiqueta, sugerir, enable);
                                        break;
                                    case "decimal":
                                        RESPUESTA = formpreg.ShowDecimal("Pregunta - Decimal", etiqueta, sugerir, enable);
                                        break;
                                    case "fecha":
                                        RESPUESTA = formpreg.ShowDate("Pregunta - Fecha", etiqueta, sugerir, enable);
                                        break;
                                    case "hora":
                                        RESPUESTA = formpreg.ShowTime("Pregunta - Hora", etiqueta, sugerir, enable);
                                        break;
                                    case "fechahora":
                                        RESPUESTA = formpreg.ShowDateTime("Pregunta - FechaHora", etiqueta, sugerir, enable);
                                        break;
                                    default://podria ser imagen,video,audio
                                        ejecuta = false;
                                        foreach (ParseTreeNode sub_h in pre.funcion.Cuerpo.ChildNodes)
                                        {
                                            if (sub_h.Term.Name.Equals(Cadena.IMAGEN))
                                            {
                                                String @ruta = @sub_h.ChildNodes[1].Token.Value.ToString();
                                                bool auto = false;
                                                if (sub_h.ChildNodes[2].Token.Value.ToString().ToLower().Equals("verdadero") || sub_h.ChildNodes[2].Token.Value.ToString().ToLower().Equals("'verdadero'"))
                                                    auto = true;
                                                RESPUESTA = formpreg.ShowMedia("Ver - Multimedia", etiqueta, sugerir, ruta, auto);
                                            }
                                        }
                                        pre.etiqueta = etiqueta;
                                        pre.respuesta = RESPUESTA;
                                        pre.tipo = TIPO;
                                        break;
                                }
                                #endregion
                                //ejecutamos el metodo respuesta
                                #region
                                if (ejecuta)
                                {

                                    List<retorno> paras = new List<retorno>();
                                    foreach (ParseTreeNode subHijo in metod.ChildNodes[1].ChildNodes)
                                    {
                                        retorno ret = ejecutarEXP(subHijo);
                                        paras.Add(ret);
                                    }
                                    String ambit = cima.Nivel + Cadena.fun + "pregunta";
                                    TablaSimbolos tabs = new TablaSimbolos(ambito, Cadena.ambito_fun, true, false, false);
                                    pilaSimbols.Push(tabs); //agregamos la nueva tabla de simbolos a la pila
                                    cima = tabs; // la colocamos en la cima
                                    // se esta llamando a una funcion con parametros
                                    if (paras.Count > 0)
                                    {
                                        //agregamos los parametros a la tb
                                        for (int i = 0; i < pre.funcion.Parametros.Count; i++)
                                        {
                                            retorno reto = paras.ElementAt(i);
                                            Simbolo sim;
                                            if (pre.funcion.Parametros.ElementAt(i).Nombre.ToLower().Equals("param_1"))
                                            {
                                                sim = new Simbolo(ambit, pre.funcion.Parametros.ElementAt(i).Nombre, RESPUESTA, TIPO, reto.Linea, reto.Columna);
                                            }
                                            else
                                            {
                                                sim = new Simbolo(ambit, pre.funcion.Parametros.ElementAt(i).Nombre, reto.valor, reto.tipo, reto.Linea, reto.Columna);
                                            }
                                            cima.insertar(pre.funcion.Parametros.ElementAt(i).Nombre, sim, claseActual.Nombre); //insertamos a la tabla el parametro
                                        }
                                        foreach (ParseTreeNode hijo in pre.funcion.Cuerpo.ChildNodes)
                                        {
                                            retorno reto2 = ejecutar(hijo);
                                        }
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima = pilaSimbols.Peek();
                                    }
                                    //es una llamada a una funcion sin parametros
                                    else
                                    {
                                        foreach (ParseTreeNode hijo in pre.funcion.Cuerpo.ChildNodes)
                                        {
                                            retorno reto2 = ejecutar(hijo);
                                        }
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima = pilaSimbols.Peek();
                                    }

                                    //luego de ejecutar Vamos guardar la info
                                    simo = existeVariable2("etiqueta");
                                    if (simo != null)
                                    {
                                        pre.etiqueta = simo.Valor.ToString();
                                    }
                                    simo = existeVariable2("respuesta");
                                    if (simo != null)
                                    {
                                        if (simo.Valor != null)
                                            pre.respuesta = simo.Valor.ToString();
                                        else
                                            pre.respuesta = "nulo";
                                    }
                                    pre.tipo = TIPO;
                                }
                                #endregion
                                #endregion
                            }
                            //solo tiene 2 nivel, puede ser Fiichero,Nota,Calcular
                            else
                            {
                                #region
                                ParseTreeNode tipo = nodo.ChildNodes[2].ChildNodes[0];
                                Simbolo simo;
                                //aca obtenemos los valores por defecto de la pregunta, como etiqueta, sugerir, etc
                                #region
                                simo = existeVariable2("etiqueta");
                                String etiqueta = "";
                                if (simo != null)
                                {
                                    etiqueta = simo.Valor.ToString();
                                }
                                String sugerir = "";
                                simo = existeVariable2("sugerir");
                                if (simo != null)
                                {
                                    sugerir = simo.Valor.ToString();
                                }
                                bool enable = true; // sololec
                                if (pre.funcion.Visibilidad.ToLower().Equals("privado"))
                                    enable = false;
                                simo = existeVariable2("respuesta");
                                if (simo != null)
                                {
                                    TIPO = simo.Tipo;
                                }
                                #endregion
                                switch (tipo.ChildNodes[0].Token.Value.ToString().ToLower())
                                {
                                    case "nota":
                                        RESPUESTA = formpreg.ShowNota("Ver - Nota", etiqueta);
                                        pre.etiqueta = etiqueta;
                                        pre.respuesta = RESPUESTA;
                                        break;
                                    case "calcular":
                                        #region
                                        List<retorno> paras = new List<retorno>();
                                        foreach (ParseTreeNode subHijo in tipo.ChildNodes[1].ChildNodes)
                                        {
                                            retorno ret = ejecutarEXP(subHijo);
                                            paras.Add(ret);
                                        }
                                        String ambit = cima.Nivel + Cadena.fun + "calcular";
                                        TablaSimbolos tabs = new TablaSimbolos(ambito, Cadena.ambito_fun, true, false, false);
                                        pilaSimbols.Push(tabs); //agregamos la nueva tabla de simbolos a la pila
                                        cima = tabs; // la colocamos en la cima
                                        // se esta llamando a una funcion con parametros
                                        if (paras.Count > 0)
                                        {
                                            //agregamos los parametros a la tb
                                            for (int i = 0; i < pre.funcion.Parametros.Count; i++)
                                            {
                                                retorno reto = paras.ElementAt(i);
                                                Simbolo sim;
                                                sim = new Simbolo(ambit, pre.funcion.Parametros.ElementAt(i).Nombre, reto.valor, reto.tipo, reto.Linea, reto.Columna);
                                                cima.insertar(pre.funcion.Parametros.ElementAt(i).Nombre, sim, claseActual.Nombre); //insertamos a la tabla el parametro
                                            }
                                            foreach (ParseTreeNode hijo in pre.funcion.Cuerpo.ChildNodes)
                                            {
                                                retorno reto2 = ejecutar(hijo);
                                            }
                                            pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                            cima = pilaSimbols.Peek();
                                        }
                                        //es una llamada a una funcion sin parametros
                                        else
                                        {
                                            foreach (ParseTreeNode hijo in pre.funcion.Cuerpo.ChildNodes)
                                            {
                                                retorno reto2 = ejecutar(hijo);
                                            }
                                            pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                            cima = pilaSimbols.Peek();
                                        }

                                        //luego de ejecutar Vamos guardar la info
                                        simo = existeVariable2("etiqueta");
                                        if (simo != null)
                                        {
                                            pre.etiqueta = simo.Valor.ToString();
                                        }
                                        simo = existeVariable2("respuesta");
                                        if (simo != null)
                                        {
                                            if (simo.Valor != null)
                                                pre.respuesta = simo.Valor.ToString();
                                            else
                                                pre.respuesta = "nulo";
                                        }
                                        pre.tipo = TIPO;
                                        break;
                                        #endregion
                                    case "fichero":
                                        retorno retox = ejecutarEXP(tipo.ChildNodes[0].ChildNodes[1]);
                                        String filt = "";
                                        if (retox.tipo.ToLower().Equals(Cadena.Cad.ToLower()))
                                            filt = retox.valor.ToString();
                                        RESPUESTA = formpreg.ShowMediaUp("Subir - Multimedia", etiqueta, sugerir, filt, enable);
                                        pre.etiqueta = etiqueta;
                                        pre.respuesta = RESPUESTA;
                                        pre.tipo = TIPO;
                                        break;
                                }
                                #endregion
                            }
                            pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                            cima = pilaSimbols.Peek();
                        }
                        //es una llamada a una funcion sin parametros
                        else
                        {
                            foreach (ParseTreeNode hijo in pre.cuerpo.ChildNodes)
                            {
                                retorno reto2 = new retorno(0, "correcto", "101", "101");
                                //capturamos el metodo de Respuesta/Calcular/Mostrar
                                #region
                                if (hijo.Term.Name.Equals(Cadena.MET_PREG)) { 
                                    String namem =hijo.ChildNodes[1].Token.Value.ToString();
                                    String visibilidad = hijo.ChildNodes[0].Token.Value.ToString();
                                    //
                                    key = namem.ToLower();
                                    if (pre.funcion==null)
                                    {
                                        //metodo o funcion con parametros
                                        if (hijo.ChildNodes[2].ChildNodes.Count > 0)
                                        {
                                            List<String> param = new List<string>();
                                            //comprobamos que no tenga parametros repetidos
                                            foreach (ParseTreeNode subHijo in hijo.ChildNodes[2].ChildNodes)
                                            {
                                                if (!param.Contains(subHijo.ChildNodes[1].Token.Value.ToString()))
                                                {
                                                    param.Add(subHijo.ChildNodes[1].Token.Value.ToString());
                                                }
                                                else
                                                {//error nombre de parametro repetido
                                                    String error = "ERROR SEMANTICO: No se agrego el metodo le metodo -> " + namem+ " a la PREGUNTA, por parametro repetido:  L: " + subHijo.ChildNodes[1].Token.Location.Line + " C: " + subHijo.ChildNodes[1].Token.Location.Column + " Clase: "+claseActual.Nombre;
                                                    Form1.listaErrores.Add(error);
                                                    Console.WriteLine(error);
                                                    return new retorno(0, "correcto", "101", "101");
                                                }
                                            }
                                            // no hay parametros repetidos, creamos la funcion
                                            Funcion fun = new Funcion(visibilidad, namem, namem, hijo.ChildNodes[3]);
                                            foreach (ParseTreeNode subHijo in hijo.ChildNodes[2].ChildNodes)
                                            {
                                                fun.addParametro(subHijo.ChildNodes[1].Token.Value.ToString(), subHijo.ChildNodes[0].Token.Value.ToString());
                                            }
                                            pre.funcion = fun;
                                        }
                                        else  //metodo o funcion sin parametros
                                        {
                                            pre.funcion = new Funcion(visibilidad,namem,namem, hijo.ChildNodes[3]);                                            
                                        }
                                    }
                                    else
                                    {//error la funcion ya fue insertada
                                        //String error = "ERROR SEMANTICO: No se agrego funcion -> " + namem + " por que ya fue definida:  L: " + hijo.ChildNodes[1].Token.Location.Line + " C: " + hijo.ChildNodes[1].Token.Location.Column + " Clase: " + claseActual.Nombre;
                                        //Form1.listaErrores.Add(error);
                                        //Console.WriteLine(error);
                                    }
                                }else{
                                    ejecutar(hijo);
                                }
                                #endregion
                                //validamos el retorno /detener/continuar
                                #region
                                if (reto2.retorna)
                                {
                                    pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                    cima = pilaSimbols.Peek();
                                    if (!reto2.tipo.Equals(Cadena.Vacio) && !reto2.tipo.Equals(Cadena.error))
                                    {
                                        String error = "ERROR SEMANTICO: No puede retornar expresiones en un metododo -> EXP" + " L: " + reto2.Linea + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno(0, "correcto", "101", "101");
                                    }
                                    return reto2;
                                }
                                if (reto2.detener)
                                { //detener fuera de ciclos, error semantico =
                                    pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                    cima = pilaSimbols.Peek();
                                    String error = "ERROR SEMANTICO: Sentencia romper invalida, fuera de ciclos -> ROMPER " + " L: " + reto2.Linea + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno(0, "correcto", "101", "101");
                                }
                                if (reto2.continua)
                                {
                                    pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                    cima = pilaSimbols.Peek();
                                    String error = "ERROR SEMANTICO: Sentencia continuar invalida, fuera de ciclos -> CONTINUAR " + " L: " + reto2.Columna + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno(0, "correcto", "101", "101");
                                }
                                #endregion
                            }
                            //aca se debe saber que tipo de pregunta se va realizar, luego ejecutar el metodo de la pregunta
                            String TIPO="";
                            String RESPUESTA="";
                            if (nodo.ChildNodes[2].ChildNodes.Count > 1)
                            {//pude ser condicion, cadena,selecionar,rango,entero,decimal,fecha,hora,fechahora,mostar
                                #region
                                ParseTreeNode tipo=nodo.ChildNodes[2].ChildNodes[1];
                                ParseTreeNode metod=nodo.ChildNodes[2].ChildNodes[0];
                                Simbolo simo;
                                bool ejecuta = true; //si es multimedia no ejecuta
                                //aca obtenemos los valores por defecto de la pregunta, como etiqueta, sugerir, etc
                                #region
                                simo = existeVariable2("etiqueta");
                                String etiqueta = "";
                                if (simo != null){
                                    etiqueta = simo.Valor.ToString();
                                }                                                         
                                String sugerir="";
                                simo = existeVariable2("sugerir");
                                if (simo != null) {
                                    sugerir = simo.Valor.ToString();
                                }
                                bool requerido=false;
                                simo = existeVariable2("requerido");
                                if (simo != null) { 
                                    if(simo.Valor.ToString().ToLower().Equals("'verdadero'") || simo.Valor.ToString().ToLower().Equals("verdadero"))
                                        requerido=true;
                                }
                                String requeridoMsg="";
                                simo = existeVariable2("requeridomsn");
                                if(simo!=null){
                                    requeridoMsg=simo.Valor.ToString();
                                }
                                bool enable= true; // sololec
                                if(pre.funcion.Visibilidad.ToLower().Equals("privado"))
                                    enable=false;
                                simo = existeVariable2("respuesta");
                                if(simo!=null){
                                    TIPO = simo.Tipo;
                                }
                                #endregion 
                                //se mostraran los imputs para las respuestas
                                #region
                                switch (tipo.ChildNodes[0].Token.Value.ToString().ToLower())
                                {
                                    case "cadena":
                                        #region
                                        if (tipo.ChildNodes[1].ChildNodes.Count > 0)
                                        { //traer parametros
                                            List<retorno> paras = new List<retorno>();
                                            foreach (ParseTreeNode subHijo in tipo.ChildNodes[1].ChildNodes)
                                            {
                                                retorno ret = ejecutarEXP(subHijo);
                                                paras.Add(ret);
                                            }
                                            String cad_min="Nulo";
                                            if (!paras.ElementAt(0).tipo.Equals(Cadena.error))
                                                cad_min = paras.ElementAt(0).valor.ToString();
                                            String cad_max= "Nulo";
                                            if (!paras.ElementAt(1).tipo.Equals(Cadena.error))
                                                cad_max = paras.ElementAt(1).valor.ToString();
                                            String num_fil = "Nulo";
                                            if (!paras.ElementAt(2).tipo.Equals(Cadena.error))
                                                num_fil = paras.ElementAt(2).valor.ToString();
                                            RESPUESTA = formpreg.ShowCadenaMulti("Preguta - Cadena Parametros", etiqueta, sugerir, requerido, requeridoMsg, cad_max, cad_min, num_fil, enable);
                                        }
                                        else {
                                            RESPUESTA = formpreg.ShowCadenaSimple("Pregunta - Cadena Simple", etiqueta, sugerir, requerido, requeridoMsg, enable);   
                                        }
                                        break;
                                        #endregion
                                    case "seleccionar":
                                        #region
                                        retorno ops = ejecutarEXP(tipo.ChildNodes[1].ChildNodes[0]);
                                        if (ops.tipo.ToLower().Equals(Cadena.Opciones.ToLower())){                                                                                      
                                            RESPUESTA = formpreg.ShowmMulti("Pregunta - Selecion Mulitiple", etiqueta,sugerir, requerido, requeridoMsg, (Opciones)ops.valor, enable);
                                        }
                                        else
                                        {
                                            String error = "ERROR SEMANTICO: No existe la lista de opcionees para selecionar_1 -> " + ops.valor.ToString() + " L: " + ops.Linea + " C: " + ops.Columna + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                        }
                                        break;
                                        #endregion
                                    case "seleccionar_1":
                                        #region
                                        retorno ops2 = ejecutarEXP(tipo.ChildNodes[1].ChildNodes[0]);
                                        if (ops2.tipo.ToLower().Equals(Cadena.Opciones.ToLower())){                                                                                      
                                            RESPUESTA = formpreg.ShowUnica("Pregunta - Selecion Unica", etiqueta,sugerir, requerido, requeridoMsg, (Opciones)ops2.valor, enable);
                                        }
                                        else
                                        {
                                            String error = "ERROR SEMANTICO: No existe la lista de opcionees para selecionar_1 -> " + ops2.valor.ToString() + " L: " + ops2.Linea + " C: " + ops2.Columna + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                        }
                                        break;
                                        #endregion
                                    case "rango":
                                        #region
                                        if (tipo.ChildNodes[1].ChildNodes.Count > 0)
                                        { //traer parametros
                                            List<retorno> paras = new List<retorno>();
                                            foreach (ParseTreeNode subHijo in tipo.ChildNodes[1].ChildNodes)
                                            {
                                                retorno ret = ejecutarEXP(subHijo);
                                                paras.Add(ret);
                                            }
                                            int ini;
                                            int fin;
                                            if(paras.ElementAt(0).tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                ini = Int32.Parse(paras.ElementAt(0).valor.ToString());
                                            else
                                                ini =0;

                                            if(paras.ElementAt(1).tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                fin= Int32.Parse(paras.ElementAt(1).valor.ToString());
                                            else
                                                fin=20;
                                            RESPUESTA = formpreg.ShowRango("Pregunta - Rango", etiqueta, sugerir, ini, fin, enable);
                                        }
                                        break;
                                        #endregion
                                    case "condicion":
                                        RESPUESTA = formpreg.ShowCondicion("Pregunta - Condicion", etiqueta, sugerir, requerido, requeridoMsg, enable);
                                        break;
                                    case "entero":
                                        RESPUESTA = formpreg.ShowNumerico("Pregunta - Entero", etiqueta, sugerir, enable);
                                        break;
                                    case "decimal":
                                        RESPUESTA = formpreg.ShowDecimal("Pregunta - Decimal", etiqueta, sugerir, enable);
                                        break;
                                    case "fecha":
                                        RESPUESTA = formpreg.ShowDate("Pregunta - Fecha", etiqueta, sugerir, enable);
                                        break;
                                    case "hora":
                                        RESPUESTA = formpreg.ShowTime("Pregunta - Hora", etiqueta, sugerir, enable);
                                        break;
                                    case "fechahora":
                                        RESPUESTA = formpreg.ShowDateTime("Pregunta - FechaHora",etiqueta, sugerir,enable);
                                        break;
                                    default://podria ser imagen,video,audio
                                        ejecuta = false;
                                        foreach (ParseTreeNode sub_h in pre.funcion.Cuerpo.ChildNodes)
                                        {
                                            if (sub_h.Term.Name.Equals(Cadena.IMAGEN)){
                                                String @ruta = @sub_h.ChildNodes[1].Token.Value.ToString();
                                                bool auto = false;
                                                if (sub_h.ChildNodes[2].Token.Value.ToString().ToLower().Equals("verdadero") || sub_h.ChildNodes[2].Token.Value.ToString().ToLower().Equals("'verdadero'"))
                                                    auto = true;
                                                RESPUESTA = formpreg.ShowMedia("Ver - Multimedia", etiqueta, sugerir, @ruta, auto);
                                            }
                                        }
                                        pre.etiqueta = etiqueta;
                                        pre.respuesta = RESPUESTA;
                                        pre.tipo = TIPO;
                                        break;
                                }
                                #endregion
                                //ejecutamos el metodo respuesta
                                #region
                                if (ejecuta)
                                {
                                    
                                    List<retorno> paras = new List<retorno>();
                                    foreach (ParseTreeNode subHijo in metod.ChildNodes[1].ChildNodes)
                                    {
                                        retorno ret = ejecutarEXP(subHijo);
                                        paras.Add(ret);
                                    }                                       
                                    String ambit = cima.Nivel + Cadena.fun + "pregunta";
                                    TablaSimbolos tabs = new TablaSimbolos(ambito, Cadena.ambito_fun, true, false, false);
                                    pilaSimbols.Push(tabs); //agregamos la nueva tabla de simbolos a la pila
                                    cima = tabs; // la colocamos en la cima
                                    // se esta llamando a una funcion con parametros
                                    if (paras.Count > 0)
                                    {
                                        //agregamos los parametros a la tb
                                        for (int i = 0; i < pre.funcion.Parametros.Count; i++)
                                        {
                                            retorno reto = paras.ElementAt(i);
                                            Simbolo sim;
                                            if (pre.funcion.Parametros.ElementAt(i).Nombre.ToLower().Equals("param_1"))
                                            {
                                                sim = new Simbolo(ambit, pre.funcion.Parametros.ElementAt(i).Nombre, RESPUESTA, TIPO, reto.Linea, reto.Columna);
                                            }
                                            else
                                            {
                                                sim = new Simbolo(ambit, pre.funcion.Parametros.ElementAt(i).Nombre, reto.valor, reto.tipo, reto.Linea, reto.Columna);
                                            }
                                            cima.insertar(pre.funcion.Parametros.ElementAt(i).Nombre, sim, claseActual.Nombre); //insertamos a la tabla el parametro
                                        }
                                        foreach (ParseTreeNode hijo in pre.funcion.Cuerpo.ChildNodes)
                                        {
                                            retorno reto2 = ejecutar(hijo);
                                        }
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima = pilaSimbols.Peek();
                                    }
                                    //es una llamada a una funcion sin parametros
                                    else
                                    {
                                        foreach (ParseTreeNode hijo in pre.funcion.Cuerpo.ChildNodes)
                                        {
                                            retorno reto2 = ejecutar(hijo);
                                        }
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima = pilaSimbols.Peek();
                                    }

                                    //luego de ejecutar Vamos guardar la info
                                    simo = existeVariable2("etiqueta");
                                    if (simo != null)
                                    {
                                        pre.etiqueta = simo.Valor.ToString();
                                    }
                                    simo = existeVariable2("respuesta");
                                    if (simo != null)
                                    {
                                        if (simo.Valor != null)
                                            pre.respuesta = simo.Valor.ToString();
                                        else
                                            pre.respuesta = "nulo";
                                    }
                                    pre.tipo = TIPO;
                                }
                                #endregion
                                #endregion
                            }
                            //solo tiene 2 nivel, puede ser Fiichero,Nota,Calcular
                            else
                            {
                                #region
                                ParseTreeNode tipo = nodo.ChildNodes[2].ChildNodes[0];
                                Simbolo simo;
                                //aca obtenemos los valores por defecto de la pregunta, como etiqueta, sugerir, etc
                                #region
                                simo = existeVariable2("etiqueta");
                                String etiqueta = "";
                                if (simo != null)
                                {
                                    etiqueta = simo.Valor.ToString();
                                }
                                String sugerir = "";
                                simo = existeVariable2("sugerir");
                                if (simo != null)
                                {
                                    sugerir = simo.Valor.ToString();
                                }
                                bool enable = true; // sololec
                                if (pre.funcion.Visibilidad.ToLower().Equals("privado"))
                                    enable = false;
                                simo = existeVariable2("respuesta");
                                if (simo != null)
                                {
                                    TIPO = simo.Tipo;
                                }
                                #endregion 
                                switch (tipo.ChildNodes[0].Token.Value.ToString().ToLower())
                                {
                                    case "nota":
                                        RESPUESTA = formpreg.ShowNota("Ver - Nota", etiqueta);
                                        pre.etiqueta = etiqueta;
                                        pre.respuesta = RESPUESTA;
                                        break;
                                    case "calcular":
                                        #region
                                        List<retorno> paras = new List<retorno>();
                                        foreach (ParseTreeNode subHijo in tipo.ChildNodes[1].ChildNodes)
                                        {
                                            retorno ret = ejecutarEXP(subHijo);
                                            paras.Add(ret);
                                        }                                       
                                        String ambit = cima.Nivel + Cadena.fun + "calcular";
                                        TablaSimbolos tabs = new TablaSimbolos(ambito, Cadena.ambito_fun, true, false, false);
                                        pilaSimbols.Push(tabs); //agregamos la nueva tabla de simbolos a la pila
                                        cima = tabs; // la colocamos en la cima
                                        // se esta llamando a una funcion con parametros
                                        if (paras.Count > 0)
                                        {
                                            //agregamos los parametros a la tb
                                            for (int i = 0; i < pre.funcion.Parametros.Count; i++)
                                            {
                                                retorno reto = paras.ElementAt(i);
                                                Simbolo sim;
                                                sim = new Simbolo(ambit, pre.funcion.Parametros.ElementAt(i).Nombre, reto.valor, reto.tipo, reto.Linea, reto.Columna);                                               
                                                cima.insertar(pre.funcion.Parametros.ElementAt(i).Nombre, sim, claseActual.Nombre); //insertamos a la tabla el parametro
                                            }
                                            foreach (ParseTreeNode hijo in pre.funcion.Cuerpo.ChildNodes)
                                            {
                                                retorno reto2 = ejecutar(hijo);
                                            }
                                            pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                            cima = pilaSimbols.Peek();
                                        }
                                        //es una llamada a una funcion sin parametros
                                        else
                                        {
                                            foreach (ParseTreeNode hijo in pre.funcion.Cuerpo.ChildNodes)
                                            {
                                                retorno reto2 = ejecutar(hijo);
                                            }
                                            pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                            cima = pilaSimbols.Peek();
                                        }

                                        //luego de ejecutar Vamos guardar la info
                                        simo = existeVariable2("etiqueta");
                                        if (simo != null)
                                        {
                                            pre.etiqueta = simo.Valor.ToString();
                                        }
                                        simo = existeVariable2("respuesta");
                                        if (simo != null)
                                        {
                                            if (simo.Valor!=null)
                                                pre.respuesta = simo.Valor.ToString();
                                            else
                                                pre.respuesta = "nulo";
                                        }
                                        pre.tipo = TIPO;
                                        break;
                                        #endregion
                                    case "fichero":
                                        retorno retox=ejecutarEXP(tipo.ChildNodes[1].ChildNodes[0]);
                                        String filt="";
                                        if(retox.tipo.ToLower().Equals(Cadena.Cad.ToLower()))
                                            filt=retox.valor.ToString();
                                        RESPUESTA =formpreg.ShowMediaUp("Subir - Multimedia",etiqueta,sugerir,filt,enable);
                                        pre.etiqueta = etiqueta;
                                        pre.respuesta = RESPUESTA;
                                        pre.tipo=TIPO;
                                        break;
                                }
                                #endregion
                            }
                            pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                            cima = pilaSimbols.Peek();
                        }
                    }
                    else {
                        String error = "ERROR SEMANTICO: La PREGUNTA a la que desea acceder no esta definida -> " + nombr + " L: " + line17 + " C: " + column17 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                    }
                    break;
                    #endregion
                case Cadena.SUPER:
                    #region
                    String line19 = (nodo.ChildNodes[0].Token.Location.Line +1)+"";
                    String column19 = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                    if (claseActual.Her.ChildNodes.Count > 1)
                    {
                        if (! vfoActual.super_agregados)
                        {
                            for (int i = 1; i < claseActual.Her.ChildNodes.Count; i++)
                            {
                                agregarConstructor2(claseActual.Her.ChildNodes[i]);
                            }
                            vfoActual.super_agregados = true;
                        }
                        NonTerminal llamada = new NonTerminal(Cadena.LLAMADA);
                        SourceSpan span = new SourceSpan();
                        ParseTreeNode node = new ParseTreeNode(llamada, span);
                        node.ChildNodes.Add(claseActual.Her.ChildNodes[1].ChildNodes[0]);
                        node.ChildNodes.Add(nodo.ChildNodes[1]);
                        ejecutar(node);
                    }
                    else {
                        String error = "ERROR SEMANTICO: no se encontro nigun constructor en la clase padre -> SUPER" + " L: " + line19 + " C: " + column19 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                    }
                    break;
                    #endregion

            }
            return new retorno(0,"correcto","101","101");//retorno cuando es de tipo vacio
        }

        public retorno ejecutarEXP(ParseTreeNode nodo) {
            switch (nodo.Term.Name) { 
                case Cadena.LOG:
                    #region
                    switch (nodo.ChildNodes.Count) { 
                        case 4: // cuando es un SI_SINO simplificado
                            String line = (nodo.ChildNodes[1].Token.Location.Line+1)+"";
                            String column = (nodo.ChildNodes[1].Token.Location.Line + 1) + "";
                            retorno ret =ejecutarEXP(nodo.ChildNodes[0]);
                            if (ret.tipo.ToLower().Equals(Cadena.Booleano.ToLower()))
                            {
                                if (ret.valor.ToString().ToLower().Equals("verdadero") || ret.valor.ToString().ToLower().Equals("'verdadero'"))
                                {
                                    return ejecutarEXP(nodo.ChildNodes[2]);
                                }else{
                                    return ejecutarEXP(nodo.ChildNodes[3]);
                                }
                            }
                            else {
                                String error = "ERROR SEMANTICO: La condicion del SI-SINO simple no retorno un booleano -> COND " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line,column); 
                            }
                        case 3: // Puede ser un OR o AND
                            String line2 = (nodo.ChildNodes[1].Token.Location.Line+1)+"";
                            String column2 = (nodo.ChildNodes[1].Token.Location.Line + 1) + "";
                            retorno ret2 = ejecutarEXP(nodo.ChildNodes[0]);
                            retorno ret22 = ejecutarEXP(nodo.ChildNodes[2]);
                            if (ret2.tipo.ToLower().Equals(Cadena.Booleano.ToLower()) && ret22.tipo.ToLower().Equals(Cadena.Booleano.ToLower()))
                            {
                                switch (nodo.ChildNodes[1].Token.Value.ToString())
                                {
                                    case "||":
                                        if (ret2.valor.ToString().ToLower().Equals("verdadero") || ret2.valor.ToString().ToLower().Equals("'verdadero'") || ret22.valor.ToString().ToLower().Equals("verdadero") || ret22.valor.ToString().ToLower().Equals("'verdadero'"))
                                        {
                                            return new retorno("verdadero", Cadena.Booleano, line2, column2);
                                        }
                                        else
                                        {
                                            return new retorno("falso", Cadena.Booleano, line2, column2);
                                        }
                                    case "&&":
                                        if ( (ret2.valor.ToString().ToLower().Equals("verdadero") || ret2.valor.ToString().ToLower().Equals("'verdadero'") ) && (ret22.valor.ToString().ToLower().Equals("verdadero") || ret22.valor.ToString().ToLower().Equals("'verdadero'")))
                                        {
                                            return new retorno("verdadero", Cadena.Booleano, line2, column2);
                                        }
                                        else
                                        {
                                            return new retorno("falso", Cadena.Booleano, line2, column2);
                                        }
                                }
                            }
                            else {
                                String error = "ERROR SEMANTICO: Una de las expresiones a evaluar en la condicion logica no es de tipo booleano -> EXP " + " L: " + line2 + " C: " + column2 + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line2, column2);
                            }
                            break;
                        case 1:
                            return ejecutarEXP(nodo.ChildNodes[0]);
                    }
                    #endregion
                    break;
                case Cadena.REL:
                    #region
                    switch (nodo.ChildNodes.Count) { 
                        case 3:
                            String line3 = (nodo.ChildNodes[1].Token.Location.Line+1)+"";
                            String column3 = (nodo.ChildNodes[1].Token.Location.Column +1) +"";
                            retorno ret3 = ejecutarEXP(nodo.ChildNodes[0]);
                            retorno ret33 = ejecutarEXP(nodo.ChildNodes[2]);

                            if ( ((ret3.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret3.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) &&
                                (ret33.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret33.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())))
                            )
                            {
                                    Double num1 = Double.Parse(ret3.valor.ToString());
                                    Double num2 = Double.Parse(ret33.valor.ToString());
                                    switch (nodo.ChildNodes[1].Token.Value.ToString())
                                    {
                                        case "==":
                                            if (num1 == num2)  
                                                return new retorno("verdadero", Cadena.Booleano, line3, column3);
                                            else
                                                return new retorno("falso", Cadena.Booleano, line3, column3);
                                        case "!=":
                                            if (num1 != num2)
                                                return new retorno("verdadero", Cadena.Booleano, line3, column3);
                                            else
                                                return new retorno("falso", Cadena.Booleano, line3, column3);
                                        case ">":
                                            if (num1 > num2)
                                                return new retorno("verdadero", Cadena.Booleano, line3, column3);
                                            else
                                                return new retorno("falso", Cadena.Booleano, line3, column3);
                                        case "<":
                                            if (num1 < num2)
                                                return new retorno("verdadero", Cadena.Booleano, line3, column3);
                                            else
                                                return new retorno("falso", Cadena.Booleano, line3, column3);
                                        case ">=":
                                            if (num1 >= num2)
                                                return new retorno("verdadero", Cadena.Booleano, line3, column3);
                                            else
                                                return new retorno("falso", Cadena.Booleano, line3, column3);
                                        case "<=":
                                            if (num1 <= num2)
                                                return new retorno("verdadero", Cadena.Booleano, line3, column3);
                                            else
                                                return new retorno("falso", Cadena.Booleano, line3, column3);
                                    }
                            }else if((ret3.tipo.ToLower().Equals(Cadena.Fecha.ToLower()) && ret33.tipo.ToLower().Equals(Cadena.Fecha.ToLower())) ||
                                    (ret3.tipo.ToLower().Equals(Cadena.Hora.ToLower()) && ret33.tipo.ToLower().Equals(Cadena.Hora.ToLower())) ||
                                    (ret3.tipo.ToLower().Equals(Cadena.FechaHora.ToLower()) && ret33.tipo.ToLower().Equals(Cadena.FechaHora.ToLower())) ||
                                    (ret3.tipo.ToLower().Equals(Cadena.Booleano.ToLower()) && ret33.tipo.ToLower().Equals(Cadena.Booleano.ToLower())) ||
                                    (ret3.tipo.ToLower().Equals(Cadena.Cad.ToLower()) && ret33.tipo.ToLower().Equals(Cadena.Cad.ToLower())) 
                                )
                            {
                                switch (nodo.ChildNodes[1].Token.Value.ToString())
                                {
                                    case "==":
                                        if (ret3.valor.ToString().Equals(ret33.valor.ToString()))
                                        {
                                            return new retorno("verdadero", Cadena.Booleano, line3, column3);
                                        }
                                        else
                                        {
                                            return new retorno("falso", Cadena.Booleano, line3, column3);
                                        }
                                    case "!=":
                                        if (!ret3.valor.ToString().Equals(ret33.valor.ToString()))
                                        {
                                            return new retorno("verdadero", Cadena.Booleano, line3, column3);
                                        }
                                        else
                                        {
                                            return new retorno("falso", Cadena.Booleano, line3, column3);
                                        }
                                    default:
                                        String error = "ERROR SEMANTICO: Operador invalido para comparar datos del mismo tipo -> " + nodo.ChildNodes[1].Token.Value.ToString() + " L: " + line3 + " C: " + column3 + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, line3, column3);
                                    // error
                                }
                                
                            }
                            else if (ret3.tipo.ToLower().Equals(Cadena.Nulo.ToLower()) || ret33.tipo.ToLower().Equals(Cadena.Nulo.ToLower()) &&
                                (!ret3.tipo.ToLower().Equals(Cadena.error.ToLower()) && !ret33.tipo.ToLower().Equals(Cadena.error.ToLower())) )
                            {
                                switch (nodo.ChildNodes[1].Token.Value.ToString())
                                {
                                    case "==":
                                        if (ret3.tipo.ToLower().Equals(Cadena.Nulo.ToLower()) && ret33.tipo.ToLower().Equals(Cadena.Nulo.ToLower()))
                                            return new retorno("verdadero", Cadena.Booleano, line3, column3);

                                        if (ret3.tipo.ToLower().Equals(Cadena.Nulo.ToLower()))
                                        {
                                            if (ret33.valor.ToString().ToLower().Equals(Cadena.Nulo.ToLower())) 
                                                return new retorno("verdadero", Cadena.Booleano, line3, column3);
                                            else 
                                                return new retorno("falso", Cadena.Booleano, line3, column3);   
                                        }
                                        else {
                                            if (ret3.valor.ToString().ToLower().Equals(Cadena.Nulo.ToLower())) 
                                                return new retorno("verdadero", Cadena.Booleano, line3, column3);
                                            else
                                                return new retorno("falso", Cadena.Booleano, line3, column3);
                                        }
                                    case "!=":
                                        if (ret3.tipo.ToLower().Equals(Cadena.Nulo.ToLower()) && ret33.tipo.ToLower().Equals(Cadena.Nulo.ToLower()))
                                            return new retorno("falso", Cadena.Booleano, line3, column3);

                                        if (ret3.tipo.ToLower().Equals(Cadena.Nulo.ToLower()))
                                        {
                                            if (!ret33.valor.ToString().ToLower().Equals(Cadena.Nulo.ToLower()))
                                                return new retorno("verdadero", Cadena.Booleano, line3, column3);
                                            else
                                                return new retorno("falso", Cadena.Booleano, line3, column3);
                                        }
                                        else { 
                                            if(!ret3.valor.ToString().ToLower().Equals(Cadena.Nulo.ToLower()))
                                                return new retorno("verdadero", Cadena.Booleano, line3, column3);
                                            else
                                                return new retorno("falso", Cadena.Booleano, line3, column3);
                                        }
                                    default:
                                        String error = "ERROR SEMANTICO: Operador invalido para comparar datos con NULO -> " + nodo.ChildNodes[1].Token.Value.ToString() + " L: " + line3 + " C: " + column3 + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, line3, column3);
                                    // error
                                }
                            }
                            else
                            {
                                String error = "ERROR SEMANTICO: Expresiones no validas en la operacion relacional REL -> " + nodo.ChildNodes[1].Token.Value.ToString() + " L: " + line3 + " C: " + column3 + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line3, column3);
                            }
                            break;
                        case 1:
                           return ejecutarEXP(nodo.ChildNodes[0]);
                    }

                    #endregion
                    break;
                case Cadena.ARIT:
                    #region
                    switch (nodo.ChildNodes.Count)
	                {
                        case 3:
                            #region
                            String line4 = (nodo.ChildNodes[1].Token.Location.Line+1)+"";
                            String column4 = (nodo.ChildNodes[1].Token.Location.Line + 1) + "";
                            retorno ret4 = ejecutarEXP(nodo.ChildNodes[0]);
                            retorno ret44 = ejecutarEXP(nodo.ChildNodes[2]);
                            if ( ! ret4.tipo.Equals(Cadena.error) && !ret44.tipo.Equals(Cadena.error))
                            {
                                switch (nodo.ChildNodes[1].Token.Value.ToString())
                                {
//======================================SUMA==================================================================>
                                    case "+":
                                        #region
                                        //si amnbos son boleanos hacemos el or
                                        if (ret4.tipo.ToLower().Equals(Cadena.Booleano.ToLower()) && ret44.tipo.ToLower().Equals(Cadena.Booleano.ToLower()))
                                        {
                                            if (ret4.valor.ToString().ToLower().Equals("'verdadero'") || ret4.tipo.ToLower().Equals("verdadero") ||
                                                ret44.valor.ToString().ToLower().Equals("'verdadero'") || ret44.tipo.ToLower().Equals("verdadero"))
                                            {
                                                return new retorno("verdadero", Cadena.Booleano, line4, column4);
                                            }
                                            else
                                            {
                                                return new retorno("falso", Cadena.Booleano, line4, column4);
                                            }
                                        }
                                        //validamos que sean entero con entero
                                        else if (ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) && ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                        {
                                            int num1=Int32.Parse(ret4.valor.ToString());
                                            int num2 = Int32.Parse(ret44.valor.ToString());
                                            num1 += num2;
                                            return new retorno(num1, Cadena.Entero, line4, column4);
                                        }
                                        //validamos que sea decimal con decimal
                                        else if (ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower()) && ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower()))
                                        {
                                            double num1= Double.Parse(ret4.valor.ToString());
                                            double num2=Double.Parse(ret44.valor.ToString());
                                            num1 += num2;
                                            return new retorno(num1, Cadena.Decimmal, line4, column4);
                                        }
                                        //validamos que sea decimal/numero o viceversa
                                        else if ((ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) &&
                                            (ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())))
                                        {
                                            double num1 = Double.Parse(ret4.valor.ToString());
                                            double num2 = Double.Parse(ret44.valor.ToString());
                                            num1 += num2;
                                            return new retorno(num1, Cadena.Decimmal, line4, column4);
                                        }
                                        //validamos que al menos uno sea cadena para concatenar
                                        else if (ret4.valor.ToString().ToLower().Equals(Cadena.Cad.ToLower()) || ret4.tipo.ToLower().Equals(Cadena.Cad.ToLower()))
                                        {
                                            String cad = ret4.valor.ToString() + ret44.valor.ToString();
                                            return new retorno(cad, Cadena.Cad, line4, column4);
                                        }
                                        //validamos que al menos uno sea un numerico y un bool 
                                        else if (((ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) && ret44.tipo.ToLower().Equals(Cadena.Booleano.ToLower())) ||
                                                    ((ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) && ret4.tipo.ToLower().Equals(Cadena.Booleano.ToLower())))
                                        {
                                            if (ret4.tipo.ToLower().Equals(Cadena.Booleano.ToLower()))
                                            {
                                                int num = 0;
                                                if (ret4.valor.ToString().ToLower().Equals("verdadero") || ret4.valor.ToString().ToLower().Equals("'verdadero'"))
                                                    num = 1;
                                                if (ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                {
                                                    int num2 = Int32.Parse(ret44.valor.ToString());
                                                    num += num2;
                                                    return new retorno(num, Cadena.Entero, line4, column4);
                                                }
                                                else
                                                {
                                                    double num1 = Double.Parse(num.ToString());
                                                    double num2 = Double.Parse(ret44.valor.ToString());
                                                    num1 += num2;
                                                    return new retorno(num1, Cadena.Decimmal, line4, column4);
                                                }
                                            }
                                            else
                                            {
                                                int num2 = 0;
                                                if (ret44.valor.ToString().ToLower().Equals("verdadero") || ret44.valor.ToString().ToLower().Equals("'verdadero'"))
                                                    num2 = 1;
                                                if (ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                {
                                                    int num = Int32.Parse(ret4.valor.ToString());
                                                    num += num2;
                                                    return new retorno(num, Cadena.Entero, line4, column4);
                                                }
                                                else
                                                {
                                                    double num1 = Double.Parse(ret4.valor.ToString());
                                                    double num22 = Double.Parse(num2.ToString());
                                                    num1 += num22;
                                                    return new retorno(num1, Cadena.Decimmal, line4, column4);
                                                }
                                            }
                                        }        
                                        else
                                        //aca es un error, no se pueden operara dichos tipos
                                        {
                                            String error = "ERROR SEMANTICO: Expresiones no validas en la operacion Aritmetica -> '+' " + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, line4, column4);
                                        }
                                        #endregion
//======================================RESTA=================================================================>
                                    case "-":
                                        #region
                                        //si amnbos son boleanos hacemos el or
                                        if (ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) && ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                        {
                                            int num1 = Int32.Parse(ret4.valor.ToString());
                                            int num2 = Int32.Parse(ret44.valor.ToString());
                                            num1 -= num2;
                                            return new retorno(num1, Cadena.Entero, line4, column4);
                                        }
                                        //validamos que sea decimal con decimal
                                        else if (ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower()) && ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower()))
                                        {
                                            double num1 = Double.Parse(ret4.valor.ToString());
                                            double num2 = Double.Parse(ret44.valor.ToString());
                                            num1 -= num2;
                                            return new retorno(num1, Cadena.Decimmal, line4, column4);
                                        }
                                        //validamos que sea decimal/numero o viceversa
                                        else if ((ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) &&
                                            (ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())))
                                        {
                                            double num1 = Double.Parse(ret4.valor.ToString());
                                            double num2 = Double.Parse(ret44.valor.ToString());
                                            num1 -= num2;
                                            return new retorno(num1, Cadena.Decimmal, line4, column4);
                                        }
                                        //validamos que al menos uno sea un numerico y un bool 
                                        else if ( ((ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) && ret44.tipo.ToLower().Equals(Cadena.Booleano.ToLower())) ||
                                                    ((ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) && ret4.tipo.ToLower().Equals(Cadena.Booleano.ToLower())))
                                        {
                                            if (ret4.tipo.ToLower().Equals(Cadena.Booleano.ToLower())) {
                                                int num = 0;
                                                if (ret4.valor.ToString().ToLower().Equals("verdadero") || ret4.valor.ToString().ToLower().Equals("'verdadero'"))
                                                    num = 1;
                                                if (ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                {
                                                    int num2 = Int32.Parse(ret44.valor.ToString());
                                                    num -= num2;
                                                    return new retorno(num, Cadena.Entero, line4, column4);
                                                }
                                                else {
                                                    double num1= Double.Parse(num.ToString());
                                                    double num2 = Double.Parse(ret44.valor.ToString());
                                                    num1 -= num2;
                                                    return new retorno(num1, Cadena.Decimmal, line4, column4);
                                                }
                                            }
                                            else{
                                                int num2 = 0;
                                                if (ret44.valor.ToString().ToLower().Equals("verdadero") || ret44.valor.ToString().ToLower().Equals("'verdadero'"))
                                                    num2 = 1;
                                                if (ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                {
                                                    int num = Int32.Parse(ret4.valor.ToString());
                                                    num -= num2;
                                                    return new retorno(num, Cadena.Entero, line4, column4);
                                                }
                                                else
                                                {
                                                    double num1 = Double.Parse(ret4.valor.ToString());
                                                    double num22 = Double.Parse(num2.ToString());
                                                    num1 -= num22;
                                                    return new retorno(num1, Cadena.Decimmal, line4, column4);
                                                }
                                            }
                                        }
                                        else
                                        //aca es un error, no se pueden operara dichos tipos
                                        {
                                            String error = "ERROR SEMANTICO: Expresiones no validas en la operacion Aritmetica -> '-' " + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, line4, column4);
                                        }
                                        #endregion
//======================================MULTI==================================================================>
                                    case "*":
                                        #region
                                        //si amnbos son boleanos hacemos el and
                                        if (ret4.tipo.ToLower().Equals(Cadena.Booleano.ToLower()) && ret44.tipo.ToLower().Equals(Cadena.Booleano.ToLower()))
                                        {
                                            if ( (ret4.valor.ToString().ToLower().Equals("'verdadero'") || ret4.tipo.ToLower().Equals("verdadero")) &&
                                                 (ret44.valor.ToString().ToLower().Equals("'verdadero'") || ret44.tipo.ToLower().Equals("verdadero")))
                                            {
                                                return new retorno("verdadero", Cadena.Booleano, line4, column4);
                                            }
                                            else
                                            {
                                                return new retorno("falso", Cadena.Booleano, line4, column4);
                                            }
                                        }
                                        //validamos que sean entero con entero
                                        else if (ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) && ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                        {
                                            int num1 = Int32.Parse(ret4.valor.ToString());
                                            int num2 = Int32.Parse(ret44.valor.ToString());
                                            num1 *= num2;
                                            return new retorno(num1, Cadena.Entero, line4, column4);
                                        }
                                        //validamos que sea decimal con decimal
                                        else if (ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower()) && ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower()))
                                        {
                                            double num1 = Double.Parse(ret4.valor.ToString());
                                            double num2 = Double.Parse(ret44.valor.ToString());
                                            num1 *= num2;
                                            return new retorno(num1, Cadena.Decimmal, line4, column4);
                                        }
                                        //validamos que sea decimal/numero o viceversa
                                        else if ((ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) &&
                                            (ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())))
                                        {
                                            double num1 = Double.Parse(ret4.valor.ToString());
                                            double num2 = Double.Parse(ret44.valor.ToString());
                                            num1 *= num2;
                                            return new retorno(num1, Cadena.Decimmal, line4, column4);
                                        }
                                        //validamos que al menos uno sea un numerico  y un bool
                                        else if (((ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) && ret44.tipo.ToLower().Equals(Cadena.Booleano.ToLower())) ||
                                                    ((ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) && ret4.tipo.ToLower().Equals(Cadena.Booleano.ToLower())))
                                        {
                                            if (ret4.tipo.ToLower().Equals(Cadena.Booleano.ToLower()))
                                            {
                                                int num = 0;
                                                if (ret4.valor.ToString().ToLower().Equals("verdadero") || ret4.valor.ToString().ToLower().Equals("'verdadero'"))
                                                    num = 1;
                                                if (ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                {
                                                    int num2 = Int32.Parse(ret44.valor.ToString());
                                                    num *= num2;
                                                    return new retorno(num, Cadena.Entero, line4, column4);
                                                }
                                                else
                                                {
                                                    double num1 = Double.Parse(num.ToString());
                                                    double num2 = Double.Parse(ret44.valor.ToString());
                                                    num1 *= num2;
                                                    return new retorno(num1, Cadena.Decimmal, line4, column4);
                                                }
                                            }
                                            else
                                            {
                                                int num2 = 0;
                                                if (ret44.valor.ToString().ToLower().Equals("verdadero") || ret44.valor.ToString().ToLower().Equals("'verdadero'"))
                                                    num2 = 1;
                                                if (ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                {
                                                    int num = Int32.Parse(ret4.valor.ToString());
                                                    num *= num2;
                                                    return new retorno(num, Cadena.Entero, line4, column4);
                                                }
                                                else
                                                {
                                                    double num1 = Double.Parse(ret4.valor.ToString());
                                                    double num22 = Double.Parse(num2.ToString());
                                                    num1 *= num22;
                                                    return new retorno(num1, Cadena.Decimmal, line4, column4);
                                                }
                                            }
                                        }
                                        else
                                        //aca es un error, no se pueden operara dichos tipos
                                        {
                                            String error = "ERROR SEMANTICO: Expresiones no validas en la operacion Aritmetica -> '*' " + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, line4, column4);
                                        }
                                    #endregion
//======================================DIVI==================================================================>
                                    case "/":
                                    #region
                                        //validamos que sean entero con entero
                                        if (ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) && ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                        {
                                            int num1 = Int32.Parse(ret4.valor.ToString());
                                            int num2 = Int32.Parse(ret44.valor.ToString());
                                            if (num2 != 0) {
                                                num1 /= num2;
                                                return new retorno(num1, Cadena.Entero, line4, column4);
                                            }
                                            String error = "ERROR SEMANTICO: No es posible dividir un cantidad sobre '0'" + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, line4, column4);
                                            
                                        }
                                        //validamos que sea decimal con decimal
                                        else if (ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower()) && ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower()))
                                        {
                                            double num1 = Double.Parse(ret4.valor.ToString());
                                            double num2 = Double.Parse(ret44.valor.ToString());
                                            if (num2 != 0)
                                            {
                                                num1 /= num2;
                                                return new retorno(num1, Cadena.Decimmal, line4, column4);
                                            }
                                            String error = "ERROR SEMANTICO: No es posible dividir un cantidad sobre '0'" + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, line4, column4);
                                        }
                                        //validamos que sea decimal/numero o viceversa
                                        else if ((ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) &&
                                            (ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())))
                                        {
                                            double num1 = Double.Parse(ret4.valor.ToString());
                                            double num2 = Double.Parse(ret44.valor.ToString());
                                            if (num2 != 0)
                                            {
                                                num1 /= num2;
                                                return new retorno(num1, Cadena.Decimmal, line4, column4);
                                            }
                                            String error = "ERROR SEMANTICO: No es posible dividir un cantidad sobre '0'" + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, line4, column4);
                                        }
                                        //validamos que al menos uno sea un numerico  y un bool
                                        else if (((ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) && ret44.tipo.ToLower().Equals(Cadena.Booleano.ToLower())) ||
                                                    ((ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) && ret4.tipo.ToLower().Equals(Cadena.Booleano.ToLower())))
                                        {
                                            if (ret4.tipo.ToLower().Equals(Cadena.Booleano.ToLower()))
                                            {
                                                int num = 0;
                                                if (ret4.valor.ToString().ToLower().Equals("verdadero") || ret4.valor.ToString().ToLower().Equals("'verdadero'"))
                                                    num = 1;
                                                if (ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                {
                                                    int num2 = Int32.Parse(ret44.valor.ToString());
                                                    if (num2 != 0)
                                                    {
                                                        num /= num2;
                                                        return new retorno(num, Cadena.Entero, line4, column4);
                                                    }
                                                    String error = "ERROR SEMANTICO: No es posible dividir un cantidad sobre '0'" + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                    Form1.listaErrores.Add(error);
                                                    Console.WriteLine(error);
                                                    return new retorno("error", Cadena.error, line4, column4);
                                                }
                                                else
                                                {
                                                    double num1 = Double.Parse(num.ToString());
                                                    double num2 = Double.Parse(ret44.valor.ToString());
                                                    if (num2 != 0)
                                                    {
                                                        num1 /= num2;
                                                        return new retorno(num1, Cadena.Decimmal, line4, column4);
                                                    }
                                                    String error = "ERROR SEMANTICO: No es posible dividir un cantidad sobre '0'" + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                    Form1.listaErrores.Add(error);
                                                    Console.WriteLine(error);
                                                    return new retorno("error", Cadena.error, line4, column4);
                                                }
                                            }
                                            else
                                            {
                                                int num2 = 0;
                                                if (ret44.valor.ToString().ToLower().Equals("verdadero") || ret44.valor.ToString().ToLower().Equals("'verdadero'"))
                                                    num2 = 1;
                                                if (ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                {
                                                    int num = Int32.Parse(ret4.valor.ToString());
                                                    if (num2 != 0)
                                                    {
                                                        num /= num2;
                                                        return new retorno(num, Cadena.Entero, line4, column4);
                                                    }
                                                    String error = "ERROR SEMANTICO: No es posible dividir un cantidad sobre '0'" + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                    Form1.listaErrores.Add(error);
                                                    Console.WriteLine(error);
                                                    return new retorno("error", Cadena.error, line4, column4); ;
                                                }
                                                else
                                                {
                                                    double num1 = Double.Parse(ret4.valor.ToString());
                                                    double num22 = Double.Parse(num2.ToString());
                                                    if (num22 != 0)
                                                    {
                                                        num1 /= num22;
                                                        return new retorno(num1, Cadena.Entero, line4, column4);
                                                    }
                                                    String error = "ERROR SEMANTICO: No es posible dividir un cantidad sobre '0'" + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                    Form1.listaErrores.Add(error);
                                                    Console.WriteLine(error);
                                                    return new retorno("error", Cadena.error, line4, column4);
                                                }
                                            }
                                        }
                                        else
                                        //aca es un error, no se pueden operara dichos tipos
                                        {
                                            String error = "ERROR SEMANTICO: Expresiones no validas en la operacion Aritmetica -> '+' " + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, line4, column4);
                                        }
                                    #endregion
//======================================MODULO==================================================================>
                                    case "%":
                                    #region
                                        //validamos que sean entero con entero
                                        if (ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) && ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                        {
                                            int num1 = Int32.Parse(ret4.valor.ToString());
                                            int num2 = Int32.Parse(ret44.valor.ToString());
                                            if (num2 != 0)
                                            {
                                                num1 %= num2;
                                                return new retorno(num1, Cadena.Entero, line4, column4);
                                            }
                                            String error = "ERROR SEMANTICO: No es posible aplicar modulo a un cantidad sobre '0'" + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, line4, column4);

                                        }
                                        //validamos que sea decimal con decimal
                                        else if (ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower()) && ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower()))
                                        {
                                            double num1 = Double.Parse(ret4.valor.ToString());
                                            double num2 = Double.Parse(ret44.valor.ToString());
                                            if (num2 != 0)
                                            {
                                                num1 %= num2;
                                                return new retorno(num1, Cadena.Decimmal, line4, column4);
                                            }
                                            String error = "ERROR SEMANTICO: No es posible aplicar modulo a un cantidad sobre '0'" + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, line4, column4);
                                        }
                                        //validamos que sea decimal/numero o viceversa
                                        else if ((ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) &&
                                            (ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())))
                                        {
                                            double num1 = Double.Parse(ret4.valor.ToString());
                                            double num2 = Double.Parse(ret44.valor.ToString());
                                            if (num2 != 0)
                                            {
                                                num1 %= num2;
                                                return new retorno(num1, Cadena.Decimmal, line4, column4);
                                            }
                                            String error = "ERROR SEMANTICO: No es posible aplicar modulo a un cantidad sobre '0'" + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, line4, column4);
                                        }
                                        //validamos que al menos uno sea un numerico  y un bool
                                        else if (((ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) && ret44.tipo.ToLower().Equals(Cadena.Booleano.ToLower())) ||
                                                    ((ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) && ret4.tipo.ToLower().Equals(Cadena.Booleano.ToLower())))
                                        {
                                            if (ret4.tipo.ToLower().Equals(Cadena.Booleano.ToLower()))
                                            {
                                                int num = 0;
                                                if (ret4.valor.ToString().ToLower().Equals("verdadero") || ret4.valor.ToString().ToLower().Equals("'verdadero'"))
                                                    num = 1;
                                                if (ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                {
                                                    int num2 = Int32.Parse(ret44.valor.ToString());
                                                    if (num2 != 0)
                                                    {
                                                        num %= num2;
                                                        return new retorno(num, Cadena.Entero, line4, column4);
                                                    }
                                                    String error = "ERROR SEMANTICO: No es posible aplicar modulo a un cantidad sobre '0'" + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                    Form1.listaErrores.Add(error);
                                                    Console.WriteLine(error);
                                                    return new retorno("error", Cadena.error, line4, column4);
                                                }
                                                else
                                                {
                                                    double num1 = Double.Parse(num.ToString());
                                                    double num2 = Double.Parse(ret44.valor.ToString());
                                                    if (num2 != 0)
                                                    {
                                                        num1 %= num2;
                                                        return new retorno(num1, Cadena.Decimmal, line4, column4);
                                                    }
                                                    String error = "ERROR SEMANTICO: No es posible aplicar modulo a un cantidad sobre '0'" + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                    Form1.listaErrores.Add(error);
                                                    Console.WriteLine(error);
                                                    return new retorno("error", Cadena.error, line4, column4);
                                                }
                                            }
                                            else
                                            {
                                                int num2 = 0;
                                                if (ret44.valor.ToString().ToLower().Equals("verdadero") || ret44.valor.ToString().ToLower().Equals("'verdadero'"))
                                                    num2 = 1;
                                                if (ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                {
                                                    int num = Int32.Parse(ret4.valor.ToString());
                                                    if (num2 != 0)
                                                    {
                                                        num %= num2;
                                                        return new retorno(num, Cadena.Entero, line4, column4);
                                                    }
                                                    String error = "ERROR SEMANTICO: No es posible aplicar modulo a un cantidad sobre '0'" + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                    Form1.listaErrores.Add(error);
                                                    Console.WriteLine(error);
                                                    return new retorno("error", Cadena.error, line4, column4); ;
                                                }
                                                else
                                                {
                                                    double num1 = Double.Parse(ret4.valor.ToString());
                                                    double num22 = Double.Parse(num2.ToString());
                                                    if (num22 != 0)
                                                    {
                                                        num1 %= num22;
                                                        return new retorno(num1, Cadena.Entero, line4, column4);
                                                    }
                                                    String error = "ERROR SEMANTICO: No es posible aplicar modulo a un cantidad sobre '0'" + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                    Form1.listaErrores.Add(error);
                                                    Console.WriteLine(error);
                                                    return new retorno("error", Cadena.error, line4, column4);
                                                }
                                            }
                                        }
                                        else
                                        //aca es un error, no se pueden operara dichos tipos
                                        {
                                            String error = "ERROR SEMANTICO: Expresiones no validas en la operacion Aritmetica -> '+' " + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, line4, column4);
                                        }
                                    #endregion
//======================================POT==================================================================>
                                    case "^":
                                    #region
                                        //validamos que sean entero con entero
                                        if (ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) && ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                        {
                                            int num1 = Int32.Parse(ret4.valor.ToString());
                                            int num2 = Int32.Parse(ret44.valor.ToString());
                                            try
                                            {
                                                num1 = (int)Math.Pow(num1, num2);
                                                return new retorno(num1, Cadena.Entero, line4, column4);
                                            }
                                            catch (Exception)
                                            {
                                                String error = "ERROR SEMANTICO: No se puede elevar la expresion por ser un resultado muy grande -> " + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                Form1.listaErrores.Add(error);
                                                Console.WriteLine(error);
                                                return new retorno("error", Cadena.error, line4, column4);
                                            }
                                            
                                        }
                                        //validamos que sea decimal con decimal
                                        else if (ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower()) && ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower()))
                                        {
                                            double num1 = Double.Parse(ret4.valor.ToString());
                                            double num2 = Double.Parse(ret44.valor.ToString());
                                            try
                                            {
                                                num1 = Math.Pow(num1, num2);
                                                return new retorno(num1, Cadena.Decimmal, line4, column4);
                                            }
                                            catch (Exception)
                                            {
                                                String error = "ERROR SEMANTICO: No se puede elevar la expresion por ser un resultado muy grande -> " + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                Form1.listaErrores.Add(error);
                                                Console.WriteLine(error);
                                                return new retorno("error", Cadena.error, line4, column4);
                                            }     
                                        }
                                        //validamos que sea decimal/numero o viceversa
                                        else if ((ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) &&
                                            (ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())))
                                        {
                                            double num1 = Double.Parse(ret4.valor.ToString());
                                            double num2 = Double.Parse(ret44.valor.ToString());
                                            try
                                            {
                                                num1 = Math.Pow(num1, num2);
                                                return new retorno(num1, Cadena.Decimmal, line4, column4);
                                            }
                                            catch (Exception)
                                            {
                                                String error = "ERROR SEMANTICO: No se puede elevar la expresion por ser un resultado muy grande -> " + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                Form1.listaErrores.Add(error);
                                                Console.WriteLine(error);
                                                return new retorno("error", Cadena.error, line4, column4);
                                            }
                                            
                                        }
                                        //validamos que al menos uno sea un numerico  y un bool
                                        else if (((ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret4.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) && ret44.tipo.ToLower().Equals(Cadena.Booleano.ToLower())) ||
                                                    ((ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret44.tipo.ToLower().Equals(Cadena.Decimmal.ToLower())) && ret4.tipo.ToLower().Equals(Cadena.Booleano.ToLower())))
                                        {
                                            if (ret4.tipo.ToLower().Equals(Cadena.Booleano.ToLower()))
                                            {
                                                int num = 0;
                                                if (ret4.valor.ToString().ToLower().Equals("verdadero") || ret4.valor.ToString().ToLower().Equals("'verdadero'"))
                                                    num = 1;
                                                if (ret44.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                {
                                                    int num2 = Int32.Parse(ret44.valor.ToString());
                                                    try
                                                    {
                                                        num = (int)Math.Pow(num, num2);
                                                        return new retorno(num, Cadena.Entero, line4, column4);
                                                    }
                                                    catch (Exception)
                                                    {
                                                        String error = "ERROR SEMANTICO: No se puede elevar la expresion por ser un resultado muy grande -> " + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                        Form1.listaErrores.Add(error);
                                                        Console.WriteLine(error);
                                                         return new retorno("error", Cadena.error, line4, column4);
                                                    }
                                                    
                                                }
                                                else
                                                {
                                                    double num1 = Double.Parse(num.ToString());
                                                    double num2 = Double.Parse(ret44.valor.ToString());
                                                    try
                                                    {
                                                        num1 = Math.Pow(num1, num2);
                                                        return new retorno(num1, Cadena.Decimmal, line4, column4);
                                                    }
                                                    catch (Exception)
                                                    {
                                                        String error = "ERROR SEMANTICO: No se puede elevar la expresion por ser un resultado muy grande -> " + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                        Form1.listaErrores.Add(error);
                                                        Console.WriteLine(error);
                                                        return new retorno("error", Cadena.error, line4, column4);
                                                    }
                                                    
                                                    
                                                }
                                            }
                                            else
                                            {
                                                int num2 = 0;
                                                if (ret44.valor.ToString().ToLower().Equals("verdadero") || ret44.valor.ToString().ToLower().Equals("'verdadero'"))
                                                    num2 = 1;
                                                if (ret4.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                                {
                                                    int num = Int32.Parse(ret4.valor.ToString());
                                                    try
                                                    {
                                                        num = (int)Math.Pow(num, num2);
                                                        return new retorno(num, Cadena.Entero, line4, column4);
                                                    }
                                                    catch (Exception)
                                                    {
                                                        String error = "ERROR SEMANTICO: No se puede elevar la expresion por ser un resultado muy grande -> " + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                        Form1.listaErrores.Add(error);
                                                        Console.WriteLine(error);
                                                        return new retorno("error", Cadena.error, line4, column4);
                                                    }  
                                                }
                                                else
                                                {
                                                    double num1 = Double.Parse(ret4.valor.ToString());
                                                    double num22 = Double.Parse(num2.ToString());
                                                    try
                                                    {
                                                        num1 *= num22;
                                                        return new retorno(num1, Cadena.Decimmal, line4, column4);
                                                    }
                                                    catch (Exception)
                                                    {
                                                        String error = "ERROR SEMANTICO: No se puede elevar la expresion por ser un resultado muy grande -> " + " L: " + line4 + " C: " + column4 + " Clase: " + claseActual.Nombre;
                                                        Form1.listaErrores.Add(error);
                                                        Console.WriteLine(error);
                                                        return new retorno("error", Cadena.error, line4, column4);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        //aca es un error, no se pueden operara dichos tipos
                                        {
                                            String error = "ERROR SEMANTICO: Expresiones no validas en la operacion Aritmetica -> '*' " + " L: " + line4 + " C: " + column4;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, line4, column4);
                                        }
                                    #endregion
                                }
                            }
                            else {
                                if (ret4.tipo.Equals(Cadena.error))
                                {
                                    return ret4;
                                }
                                else {
                                    return ret44;
                                }
                            }
                        #endregion
                            break;
                        case 2:
                            #region
                            if (nodo.ChildNodes[0].Token!=null){
                                //aca validamos el acceso a la matriz
                                if(nodo.ChildNodes[0].Term.Name.Equals(Cadena.Id)){
                                    String name = nodo.ChildNodes[0].Token.Value.ToString();
                                    String line = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                                    String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                                    Simbolo aux01 = existeVariable2(name);
                                    if (aux01 != null)
                                    {
                                        //comprobamos que sea de tipo matriz
                                        if (aux01.TipoObjeto.Equals(Cadena.Matriz))
                                        {
                                            List<Object> indices = new List<object>();
                                            capturarVals(indices, nodo.ChildNodes[1]);
                                            if (validarIndices(indices, aux01.dimenciones))
                                            {
                                                int indice = linealizar(indices, aux01.dimenciones);
                                                object valor = ((List<Object>)aux01.Valor).ElementAt(indice);// recuperamos el valor de la matriz
                                                return new retorno(valor, aux01.Tipo, line, column);
                                            }//lo errores los indico en el metodo validarIndices :)
                                            return new retorno("error", Cadena.error, line, column);
                                        }
                                        else
                                        {
                                            String error = "ERROR SEMANTICO: La variable no es de tipo matriz -> " + name + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, line, column);
                                        }
                                    }
                                    else
                                    {
                                        String error = "ERROR SEMANTICO: La variable no ha sido definida -> " + name + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, line, column);
                                    }
                                }
                                // es la negacion logica         
                                else if(nodo.ChildNodes[0].Term.Name.Equals("!")){
                                    retorno ret = ejecutarEXP(nodo.ChildNodes[1]);
                                    if (ret.tipo.ToLower().Equals(Cadena.Booleano.ToLower()))
                                    {
                                        if (ret.valor.ToString().ToLower().Equals("verdadero") || ret.valor.ToString().ToLower().Equals("'verdadero'"))
                                        {
                                            ret.valor = "falso";
                                            return ret;
                                        }
                                        else {
                                            ret.valor = "verdadero";
                                            return ret;
                                        }
                                    }
                                    else {
                                        String error = "ERROR SEMANTICO: La expresion a negar logicamente, no es de tipo booleano ->  " + ret.tipo + " L: " + ret.Linea + " C: " + ret.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, "0", "0");
                                    }
                                }
                                //es la negacion aritmetica
                                else{
                                    retorno ret = ejecutarEXP(nodo.ChildNodes[1]);
                                    if (ret.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret.tipo.ToLower().Equals(Cadena.Decimmal.ToLower()))
                                    {
                                        if (ret.tipo.ToLower().Equals(Cadena.Entero.ToLower()))//es un entero
                                        {
                                            int num = Int32.Parse(ret.valor.ToString());
                                            ret.valor = (num * -1);
                                            return ret;
                                        }
                                        else// es un decimal
                                        {
                                            double num = Double.Parse(ret.valor.ToString());
                                            ret.valor = (num * -1);
                                            return ret;
                                        }
                                    }
                                    else
                                    {
                                        String error = "ERROR SEMANTICO: La expresion a negar aritmeticamente, no es de tipo numerico ->  " + ret.tipo + " L: " + ret.Linea + " C: " + ret.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, "0", "0");
                                    }
                                }
                            }
                            // es una expresion
                            else {
                                retorno ret = ejecutarEXP(nodo.ChildNodes[0]);
                                if (ret.tipo.ToLower().Equals(Cadena.Entero.ToLower()) || ret.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                {
                                    switch (nodo.ChildNodes[1].Token.Value.ToString())
                                    {
                                        case "++":
                                            if (ret.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                            {
                                                int num = Int32.Parse(ret.valor.ToString()) + 1;
                                                ret.valor=num;
                                                return ret;
                                        
                                            }
                                            else
                                            {
                                                double num = Double.Parse(ret.valor.ToString()) + 1;
                                                ret.valor = num;
                                                return ret;
                                            }
                                        case "--":
                                            if (ret.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                            {
                                                int num = Int32.Parse(ret.valor.ToString()) - 1;
                                                ret.valor = num;
                                                return ret;
                                            }
                                            else
                                            {
                                                double num = Double.Parse(ret.valor.ToString()) - 1;
                                                ret.valor = num;
                                                return ret;
                                            }
                                    }
                                }
                                else
                                {
                                    String error = "ERROR SEMANTICO: La variable a inc/dec debe ser de tipo numerico ->  " + ret.tipo + " L: " + ret.Linea + " C: " + ret.Columna + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno("error", Cadena.error, "0", "0");
                                }
                            }                   
                            break;
                            #endregion
                        //========================Son las funciones especiales==============================
                        case 1:
                            #region
                            //aca son todos van todos los nodos terminales 
                            if (nodo.ChildNodes[0].Token != null)
                            {
                                switch (nodo.ChildNodes[0].Term.Name) {                               
                                    case Cadena.Id:
                                        Simbolo tmp00 = existeVariable(nodo.ChildNodes[0].Token.Value.ToString());
                                        if(tmp00!=null){ //aca no se por que no estaba retornando los objetos
                                            //if(tmp00.TipoObjeto.Equals("")){
                                                retorno rt = new retorno(tmp00.Valor,tmp00.Tipo,tmp00.Linea,tmp00.Columna);
                                                rt.tipo=retornarTipo(tmp00.Tipo);
                                                rt.TipoDato=tmp00.TipoObjeto;
                                                return rt;
                                            /*}else{
                                                String error = "ERROR SEMANTICO: La variable a acceder no es primitiva ->  " + tmp00.Nombre + " L: " + (nodo.ChildNodes[0].Token.Location.Line + 1) + " C: " + (nodo.ChildNodes[0].Token.Location.Column + 1) + " Clase: " + claseActual.Nombre;
                                                Form1.listaErrores.Add(error);
                                                Console.WriteLine(error);
                                                return new retorno("error", Cadena.error, "0", "0");
                                            } */ 
                                        }else{ // variable no existe
                                            String error = "ERROR SEMANTICO: La variable a acceder no esta definida ->  " + nodo.ChildNodes[0].Token.Value.ToString() + " L: " + (nodo.ChildNodes[0].Token.Location.Line + 1) + " C: " + (nodo.ChildNodes[0].Token.Location.Column + 1) + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, "0", "0");
                                        }
                                    case Cadena.Cad:
                                        return new retorno(nodo.ChildNodes[0].Token.Value, Cadena.Cad, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                                    case Cadena.Decimmal:
                                        return new retorno(nodo.ChildNodes[0].Token.Value, Cadena.Decimmal, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                                    case Cadena.Entero:
                                        return new retorno(nodo.ChildNodes[0].Token.Value, Cadena.Entero, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                                    case Cadena.Fecha:
                                        return new retorno(nodo.ChildNodes[0].Token.Value, Cadena.Fecha, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                                    case Cadena.Hora:
                                        return new retorno(nodo.ChildNodes[0].Token.Value, Cadena.Hora, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                                    case Cadena.FechaHora:
                                        return new retorno(nodo.ChildNodes[0].Token.Value, Cadena.FechaHora, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                                    case Cadena.Booleano:
                                        return new retorno(nodo.ChildNodes[0].Token.Value, Cadena.Booleano, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                                    case Cadena.Nulo:
                                        return new retorno(Cadena.Nulo, Cadena.Nulo, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                                }
                            }
                            //resto de nodos de funncion
                            else {
                                return ejecutarEXP(nodo.ChildNodes[0]);  
                            }
                            break;
                            #endregion
                    }
                    #endregion
                    break;
                //RESTO DE FUNCIONES
                #region
                case Cadena.OP://esto lo tengo que pasar a las instrucciones de cuerpo
                    #region
                    //es un id
                    if (nodo.ChildNodes[0].Token != null)
                    {
                        String name = nodo.ChildNodes[0].Token.Value.ToString();
                        String line = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1)+"";
                        Simbolo tmp01 = existeVariable2(name);
                        if (tmp01 != null)
                        {
                            if (tmp01.TipoObjeto.Equals(""))
                            {
                                if (tmp01.Tipo.ToLower().Equals(Cadena.Entero.ToLower()) || tmp01.Tipo.ToLower().Equals(Cadena.Decimmal.ToLower()))
                                {
                                    switch (nodo.ChildNodes[1].Token.Value.ToString())
                                    {
                                        case "++":
                                            if (tmp01.Tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                            {
                                                int num = Int32.Parse(tmp01.Valor.ToString()) + 1;
                                                tmp01.Valor = num;
                                                return new retorno(num, Cadena.Entero, line, column);
                                            }
                                            else
                                            {
                                                double num = Double.Parse(tmp01.Valor.ToString()) + 1;
                                                tmp01.Valor = num;
                                                return new retorno(num, Cadena.Decimmal, line, column);
                                            }
                                        case "--":
                                            if (tmp01.Tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                                            {
                                                int num = Int32.Parse(tmp01.Valor.ToString()) - 1;
                                                tmp01.Valor = num;
                                                return new retorno(num, Cadena.Entero, line, column);
                                            }
                                            else
                                            {
                                                double num = Double.Parse(tmp01.Valor.ToString()) - 1;
                                                tmp01.Valor = num;
                                                return new retorno(num, Cadena.Decimmal, line, column);
                                            }
                                    }
                                }
                                else
                                {
                                    String error = "ERROR SEMANTICO: La variable a inc/dec debe ser de tipo numerico ->  " + name + " L: " + line + " C: " + column;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno("error", Cadena.error, "0", "0");
                                }

                            }
                            else
                            {
                                String error = "ERROR SEMANTICO: La variable a acceder no es primitiva ->  " + name + " L: " + line + " C: " +column;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, "0", "0");
                            }
                        }
                        else
                        { // variable no existe
                            String error = "ERROR SEMANTICO: La variable a acceder no esta definida ->  " + name + " L: " + line + " C: " +column ;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, "0", "0");
                        }
                    }
                    #endregion
                    break;
                case Cadena.LLAMADA:
                    #region
                    retorno rut= ejecutar(nodo);
                    if (rut.tipo.Equals(Cadena.Vacio) || rut.tipo.Equals("correcto"))
                    {
                        String nombre = nodo.ChildNodes[0].Token.Value.ToString();
                        String line = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        String error = "ERROR SEMANTICO: Invoco un metodo de la clase, que no retorna nigun valor -> " + nombre+ " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return new retorno("error", Cadena.error, "0", "0");
                    }
                    return rut;
                    #endregion
                case Cadena.ACC_OBJ:
                    #region
                    String name9 = nodo.ChildNodes[0].Token.Value.ToString();
                    String line9 = (nodo.ChildNodes[0].Token.Location.Line+1)+"";
                    String column9 = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                    Simbolo sim1 = existeVariable(name9);
                    if (sim1 != null)
                    {
                        if (sim1.TipoObjeto.Equals(Cadena.Objeto))
                        {
                            //seteamos el nuevo entorno del objeto
                            this.pilaVFO.Push((VFO)sim1.Valor);
                            this.vfoActual = pilaVFO.Peek();
                            this.Global = vfoActual.global;
                            this.Funciones = vfoActual.funciones;
                            this.pilaSimbols = vfoActual.pilaSimbolos;
                            this.cima = pilaSimbols.Peek();
                            this.claseActual = ((VFO)sim1.Valor).clase;
                            int cambio_entorno=0;
                            retorno val_ret=new retorno("error",Cadena.error,"0101","0101");
                            int penultimo_hijo = nodo.ChildNodes[1].ChildNodes.Count - 2;
                            for (int i = 0; i < nodo.ChildNodes[1].ChildNodes.Count; i++)
                            {
                                ParseTreeNode hijo = nodo.ChildNodes[1].ChildNodes[i];
                                if (hijo.ChildNodes.Count == 1)//puede ser un variable u otro objeto
                                {
                                    String nombre = hijo.ChildNodes[0].Token.Value.ToString();
                                    String line = (hijo.ChildNodes[0].Token.Location.Line + 1) + "";
                                    String column = (hijo.ChildNodes[0].Token.Location.Column + 1) + "";
                                    Simbolo sim2 = existeVariable2(nombre);
                                    if (sim2 != null)
                                    {
                                        if (sim2.TipoObjeto.Equals(""))//es una variable
                                        {
                                            if (penultimo_hijo == i || penultimo_hijo == -1)
                                            {
                                                if (!sim2.Visibilidad.ToLower().Equals(Cadena.Privado.ToLower()))
                                                {
                                                    val_ret=new retorno(sim2.Valor, sim2.Tipo, line, column);
                                                }
                                                else
                                                {
                                                    String error = "ERROR SEMANTICO: El atributo del objeto al que intenta acceder es PRIVADO ->  " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                                    Form1.listaErrores.Add(error);
                                                    Console.WriteLine(error);
                                                    val_ret = new retorno("error", Cadena.error, "0", "0");
                                                    break;
                                                }
                                            }
                                            else {
                                                String error = "ERROR SEMANTICO: El atributo del objeto al que intenta acceder no es un objeto ->  " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                                Form1.listaErrores.Add(error);
                                                Console.WriteLine(error);
                                                val_ret = new retorno("error", Cadena.error, "0", "0");
                                                break;
                                            }
                                        }
                                        //es un objeto papu XD
                                        else {
                                            //retorno el objeto 
                                            if (i == penultimo_hijo || penultimo_hijo == -1)
                                            {
                                                val_ret = new retorno(sim2.Valor, sim2.Tipo, line9, column9);
                                            }
                                            //si no es el penultimo realizo el cambio de entorno 
                                            else {
                                                this.pilaVFO.Push((VFO)sim2.Valor);
                                                this.vfoActual = pilaVFO.Peek();
                                                this.Global = vfoActual.global;
                                                this.Funciones = vfoActual.funciones;
                                                this.pilaSimbols = vfoActual.pilaSimbolos;
                                                this.cima = pilaSimbols.Peek();
                                                this.claseActual = ((VFO)sim2.Valor).clase;
                                                cambio_entorno++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        String error = "ERROR SEMANTICO: El atributo del objeto al que intenta acceder no esta definido ->  " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        val_ret =new retorno("error", Cadena.error, "0", "0");
                                        break;
                                    }
                                }
                                //puede ser una matriz o una llamda a funcion
                                else{
                                    String nombre=hijo.ChildNodes[0].Token.Value.ToString();
                                    String line= (hijo.ChildNodes[0].Token.Location.Line+1)+"";
                                    String column = (hijo.ChildNodes[0].Token.Location.Column + 1) + "";
                                    pasarVarTemporal();
                                    if (hijo.ChildNodes[1].Term.Name.Equals(Cadena.L_EXP)) //si entra aca es una llamada a una funcion.
                                    {
                                        NonTerminal llamada = new NonTerminal(Cadena.LLAMADA);
                                        SourceSpan span = new SourceSpan();
                                        ParseTreeNode node = new ParseTreeNode(llamada, span);
                                        node.ChildNodes.Add(hijo.ChildNodes[0]);
                                        node.ChildNodes.Add(hijo.ChildNodes[1]);
                                        val_ret = ejecutar(node);
                                        if (val_ret.tipo.Equals(Cadena.error))
                                        {
                                            temporal.Pop();
                                            break;
                                        }
                                        if (val_ret.tipo.Equals(Cadena.Vacio) || val_ret.tipo.Equals("correcto")) {
                                            String error = "ERROR SEMANTICO: Accedio a un metodo del objeto que no retorna ningun valor ->  " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            val_ret = new retorno("error", Cadena.error, "0", "0");
                                            temporal.Pop();
                                            break;
                                        }    
                                        //la funcion retorno una variable primitiva y no es el ultimo nivel de acceso al objeto
                                        if(validarPrimitivo(val_ret.tipo) && i<penultimo_hijo){
                                            String error = "ERROR SEMANTICO: El atributo del objeto al que intenta acceder no es un objeto ->  " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            val_ret = new retorno("error", Cadena.error, "0", "0");
                                            temporal.Pop();
                                            break;
                                        }                                    
                                        //retorno un objeto y hay un nivel mas de acceso a este objeto
                                        if (!validarPrimitivo(val_ret.tipo) && i < penultimo_hijo && !val_ret.tipo.Equals(Cadena.Nulo)) {
                                            this.pilaVFO.Push((VFO)val_ret.valor);
                                            this.vfoActual = pilaVFO.Peek();
                                            this.Global = vfoActual.global;
                                            this.Funciones = vfoActual.funciones;
                                            this.pilaSimbols = vfoActual.pilaSimbolos;
                                            this.cima = pilaSimbols.Peek();
                                            this.claseActual = ((VFO)val_ret.valor).clase;
                                            cambio_entorno++;
                                        }
                                    }
                                    //es el acceso a una matriz
                                    else {
                                        NonTerminal llamada = new NonTerminal(Cadena.ARIT);
                                        SourceSpan span = new SourceSpan();
                                        ParseTreeNode node = new ParseTreeNode(llamada, span);
                                        node.ChildNodes.Add(hijo.ChildNodes[0]);
                                        node.ChildNodes.Add(hijo.ChildNodes[1]);
                                        val_ret = ejecutarEXP(node);
                                        if (val_ret.tipo.Equals(Cadena.error))
                                        {
                                            temporal.Pop();
                                            break;
                                        }
                                        //la funcion retorno una variable primitiva y no es el ultimo nivel de acceso al objeto
                                        if (validarPrimitivo(val_ret.tipo) && i < penultimo_hijo)
                                        {
                                            String error = "ERROR SEMANTICO: El atributo del objeto al que intenta acceder no es un objeto ->  " + nombre + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            val_ret = new retorno("error", Cadena.error, "0", "0");
                                            temporal.Pop();
                                            break;
                                        }
                                        //retorno un objeto y hay un nivel mas de acceso a este objeto
                                        if (!validarPrimitivo(val_ret.tipo) && i < penultimo_hijo && !val_ret.tipo.Equals(Cadena.Nulo))
                                        {
                                            this.pilaVFO.Push((VFO)val_ret.valor);
                                            this.vfoActual = pilaVFO.Peek();
                                            this.Global = vfoActual.global;
                                            this.Funciones = vfoActual.funciones;
                                            this.pilaSimbols = vfoActual.pilaSimbolos;
                                            this.cima = pilaSimbols.Peek();
                                            this.claseActual = ((VFO)val_ret.valor).clase;
                                            cambio_entorno++;
                                        }
                                    }
                                    temporal.Pop();
                                }
                            }
                            //aca va ir el for que va sacar todos los entornos de la pila
                            for (int i = 0; i < cambio_entorno; i++)
                            {
                                this.pilaVFO.Pop();
                                this.vfoActual = pilaVFO.Peek();
                                this.Global = vfoActual.global;
                                this.Funciones = vfoActual.funciones;
                                this.pilaSimbols = vfoActual.pilaSimbolos;
                                this.cima = pilaSimbols.Peek();
                                this.claseActual = vfoActual.clase;
                            }
                            this.pilaVFO.Pop();
                            this.vfoActual = pilaVFO.Peek();
                            this.Global = vfoActual.global;
                            this.Funciones = vfoActual.funciones;
                            this.pilaSimbols = vfoActual.pilaSimbolos;
                            this.cima = pilaSimbols.Peek();
                            this.claseActual = vfoActual.clase;
                            return val_ret;
                        }
                        else {
                            String error = "ERROR SEMANTICO: La instancia a la que intenta acceder no es de tipo OBJETO ->  " + name9 + " L: " + line9 + " C: " + column9 + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, "0", "0");
                        }
                    }
                    else {
                        String error = "ERROR SEMANTICO: El objeto al que intenta acceder no esta definido ->  " + name9 + " L: " + line9 + " C: " + column9 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return new retorno("error", Cadena.error, "0", "0");
                    }
                    #endregion
                case Cadena.NUEVO:
                    #region
                    String tipo_ob = nodo.ChildNodes[1].Token.Value.ToString();
                    String line5 = (nodo.ChildNodes[1].Token.Location.Line+1)+"";
                    String column5 = (nodo.ChildNodes[1].Token.Location.Column + 1) + "";
                    if (Clases.existeClase(tipo_ob))
                    {
                        Clase clase =  Clases.retornaClase(tipo_ob);
                        //despues de capturar la clase vamos a crear su VFO
                        TablaSimbolos global_obj = new TablaSimbolos("Global", Cadena.ambito_g, false, false,false);
                        VFO vfo_obj = new VFO(global_obj);
                        vfo_obj.clase = clase;
                        //seteamos las variables globales sobre se trabaja la ejecucion
                        this.pilaVFO.Push(vfo_obj);
                        this.vfoActual = pilaVFO.Peek();
                        this.Global = vfoActual.global;
                        this.Funciones = vfoActual.funciones;
                        this.pilaSimbols = vfoActual.pilaSimbolos;
                        this.cima = pilaSimbols.Peek();                        
                        this.claseActual = clase;
                        //pasamos la variables a temporal
                        pasarVarTemporal();
                        // luego de poner el objeto en la cima para que todos los cambios o haga sobre sus atributos
                        if (!clase.Padre.Equals("null")) //vemos si tiene herencia 
                        {
                            herencia(clase.Padre, clase);
                        }
                        //capturamos sus metodos, funciones y constructores
                        capturarFunciones(clase.Cuerpo);
                        //capturamos sus declaraciones y asignaciones globales
                        foreach (ParseTreeNode hijo in clase.Cuerpo.ChildNodes)
                        {
                            capturarDecAsigs(hijo);
                        }
                        // Ahora vamos a ver si tiene un constructor con el tipo que se esta inicializando el objeto
                        List<retorno> parametros = parametros = new List<retorno>();
                        if (nodo.ChildNodes[2].ChildNodes.Count > 0)
                        {
                            foreach (ParseTreeNode subHijo in nodo.ChildNodes[2].ChildNodes)
                            {
                                retorno ret = ejecutarEXP(subHijo);
                                if (ret.tipo.ToLower().Equals(Cadena.error) || ret.tipo.ToLower().Equals(Cadena.Nulo))
                                {
                                    String error = "ERROR SEMANTICO: Parametros incorrectos en la llamada a construcotor -> " + tipo_ob + " L: " + line5 + " C: " + column5 + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    temporal.Pop();
                                    return new retorno("error", Cadena.error, "0", "0");
                                }
                                parametros.Add(ret);
                            }
                        }
                        String pars = "";
                        String key;
                        foreach (retorno ret in parametros)
                        {
                            pars += ret.tipo.ToLower();
                        }
                        key = tipo_ob + "_" + pars; //esta es la llave de la funcion
                        Funcion func = Funciones.retornaFuncion(key);
                        if (func != null)
                        {
                            //aca estaba la creacion de la tabla
                            String ambito = cima.Nivel + Cadena.fun + tipo_ob;
                            TablaSimbolos tab = new TablaSimbolos(ambito, Cadena.ambito_fun, true, false, false);
                            pilaSimbols.Push(tab); //agregamos la nueva tabla de simbolos a la pila
                            cima = tab; // la colocamos en la cima
                            // se esta llamando a un constructor con parametros
                            if (parametros.Count > 0)
                            {
                                //agregamos los parametros a la tb
                                for (int i = 0; i < func.Parametros.Count; i++)
                                {
                                    retorno reto = parametros.ElementAt(i);
                                    Simbolo sim = new Simbolo(ambito, func.Parametros.ElementAt(i).Nombre, reto.valor, reto.tipo, reto.Linea, reto.Columna);
                                    cima.insertar(func.Parametros.ElementAt(i).Nombre, sim,claseActual.Nombre); //insertamos a la tabla el parametro

                                }
                                foreach (ParseTreeNode hijo in func.Cuerpo.ChildNodes)
                                {
                                    retorno reto2 = ejecutar(hijo);
                                    if (reto2.retorna)
                                    {
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima = pilaSimbols.Peek();
                                        if (func.Tipo.ToLower().Equals(Cadena.Vacio.ToLower()) && !reto2.tipo.Equals(Cadena.Vacio))
                                        {
                                            String error = "ERROR SEMANTICO: No puede retornar expresiones en un metododo -> EXP" + " L: " + reto2.Linea + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, "0", "0");
                                        }
                                        return reto2;
                                    }
                                    if (reto2.detener)
                                    { //detener fuera de ciclos, error semantico =
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima = pilaSimbols.Peek();
                                        String error = "ERROR SEMANTICO: Sentencia romper invalida, fuera de ciclos -> ROMPER " + " L: " + reto2.Linea + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, "0", "0");
                                    }
                                    if (reto2.continua)
                                    {
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima = pilaSimbols.Peek();
                                        String error = "ERROR SEMANTICO: Sentencia continuar invalida, fuera de ciclos -> CONTINUAR " + " L: " + reto2.Columna + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, "0", "0");
                                    }
                                }
                                pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                cima = pilaSimbols.Peek();
                            }
                            //es una llamada a un constructor sin parametros
                            else
                            {
                                foreach (ParseTreeNode hijo in func.Cuerpo.ChildNodes)
                                {
                                    retorno reto2 = ejecutar(hijo);
                                    if (reto2.retorna)
                                    {
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima = pilaSimbols.Peek();
                                        if (func.Tipo.ToLower().Equals(Cadena.Vacio.ToLower()) && !reto2.tipo.Equals(Cadena.Vacio))
                                        {
                                            String error = "ERROR SEMANTICO: No puede retornar expresiones en un metododo -> EXP" + " L: " + reto2.Linea + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                            return new retorno("error", Cadena.error, "0", "0");
                                        }
                                        return reto2;
                                    }
                                    if (reto2.detener)
                                    { //detener fuera de ciclos, error semantico =
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima = pilaSimbols.Peek();
                                        String error = "ERROR SEMANTICO: Sentencia romper invalida, fuera de ciclos -> ROMPER " + " L: " + reto2.Linea + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, "0", "0");
                                    }
                                    if (reto2.continua)
                                    {
                                        pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                        cima = pilaSimbols.Peek();
                                        String error = "ERROR SEMANTICO: Sentencia continuar invalida, fuera de ciclos -> CONTINUAR " + " L: " + reto2.Columna + " C: " + reto2.Columna + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, "0", "0");
                                    }
                                }
                                pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                cima = pilaSimbols.Peek();
                            }
                        }
                        // no se encontro nigun constructor de la clase        
                        else
                        {
                            String aviso = "AVISO: No se ejecutó ningun constructor al instanciar el objeto de la clase -> " + clase.Nombre + " L: " + line5 + " C: " + column5 + " Clase: " + claseActual.Nombre;
                            //Form1.listaErrores.Add(aviso);
                            Console.WriteLine(aviso);
                        }
                        //Despues de haber ejecutado el constructor(si existe alguno) vamos a regresar todo a la normalidad
                        temporal.Pop();
                        this.pilaVFO.Pop();
                        this.vfoActual = pilaVFO.Peek();
                        this.Global = vfoActual.global;
                        this.Funciones = vfoActual.funciones;
                        this.pilaSimbols = vfoActual.pilaSimbolos;
                        this.cima = pilaSimbols.Peek();
                        this.claseActual = vfoActual.clase;
                        //retornamos la instancia del objeto, para agregarlos a la tb de la clase donde fue instanciado
                        return new retorno(vfo_obj, tipo_ob, line5, column5);
                    }
                    //la clase del objeto es inaccesible
                    else 
                    {
                        String error = "ERROR SEMANTICO: El tipo de objeto a declar es inaccesible -> " + tipo_ob + " L: " + line5 + " C: " + column5 + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return new retorno("error", Cadena.error, "0", "0");
                    }
                    #endregion
                case Cadena.CADENA:
                    #region
                    retorno ret00 = ejecutarEXP(nodo.ChildNodes[1]);
                    if (!ret00.tipo.Equals(Cadena.error)) {
                        String line=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        if (!ret00.tipo.Equals(Cadena.Nulo))
                        {
                            if (ret00.tipo.Equals(Cadena.Entero) || ret00.tipo.Equals(Cadena.Decimmal) || ret00.tipo.Equals(Cadena.Cad) || ret00.tipo.Equals(Cadena.Booleano) ||
                                ret00.tipo.Equals(Cadena.FechaHora) || ret00.tipo.Equals(Cadena.Hora) || ret00.tipo.Equals(Cadena.Fecha))
                            {
                                ret00.tipo = Cadena.Cad;
                                return ret00;
                            }
                            else {
                                String error = "ERROR SEMANTICO: No se puede convertir -> " + ret00.tipo + " a CADENA" + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line, column);
                            }
                        }
                        else {
                            String error = "ERROR SEMANTICO: No se puede convertir NULO a CADENA -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, line, column);
                        }
                    }
                    return ret00;
                    #endregion
                case Cadena.SUBCAD:
                    #region
                    retorno re1 = ejecutarEXP(nodo.ChildNodes[1]);
                    retorno ret1 = ejecutarEXP(nodo.ChildNodes[2]);
                    retorno ret11 = ejecutarEXP(nodo.ChildNodes[3]);
                    if (!ret1.tipo.Equals(Cadena.error) && !ret11.Equals(Cadena.error)) {
                        String line = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        if (ret1.tipo.ToLower().Equals(Cadena.Entero.ToLower()) && ret11.tipo.ToLower().Equals(Cadena.Entero.ToLower()))
                        {
                            int inf = Int32.Parse(ret1.valor.ToString());
                            int sup = Int32.Parse(ret11.valor.ToString());
                            if(!re1.tipo.Equals(Cadena.Cad)){
                                String error = "ERROR SEMANTICO: Debe evaluar una cadena como primer parametro en SUBCAD  -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line, column);
                            }
                            String cad=re1.valor.ToString();
                            if (inf > -1 && sup < cad.Length)
                            {
                                int cant=sup-inf+1;
                                cad = cad.Substring(inf, cant);
                                return new retorno(cad, Cadena.Cad, line, column);
                            }
                            //error en los indices de la sub cadena
                            else {
                                String error = "ERROR SEMANTICO: Los indices de SUBCAD no son validos -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line, column);
                            }
                        }
                        else {
                            String error = "ERROR SEMANTICO: Los dos indices de SUBCAD deben ser numericos -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, line, column);
                        }
                    }
                    if (ret1.tipo.Equals(Cadena.error))
                        return ret1;
                    else
                        return ret11;
                    #endregion
                case Cadena.POSCAD:
                    #region
                    retorno re2 = ejecutarEXP(nodo.ChildNodes[1]);
                    retorno ret02 = ejecutarEXP(nodo.ChildNodes[2]);
                    if (!ret02.tipo.Equals(Cadena.error))
                    {
                        String line = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        if (ret02.tipo.Equals(Cadena.Entero))
                        {
                            int inf = Int32.Parse(ret02.valor.ToString());
                            if (!re2.tipo.Equals(Cadena.Cad)) { 
                                String error = "ERROR SEMANTICO: Debe evaluar una cadena en la funcion POSCAD -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line, column);
                            }
                            String cad = re2.valor.ToString();
                            if (inf > -1 && inf < cad.Length)
                            {
                                cad = cad.Substring(inf, 1);
                                return new retorno(cad, Cadena.Cad, line, column);
                            }
                            //error en los indices de la sub cadena
                            else
                            {
                                String error = "ERROR SEMANTICO: Los indices de SUBCAD no son validos -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line, column);
                            }
                        }
                        else
                        {
                            String error = "ERROR SEMANTICO: Los dos indices de SUBCAD deben ser numericos -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, line, column);
                        }
                    }
                    return ret02;
                    #endregion
                case Cadena.BOOLEANO:
                    #region
                    aux2 = ejecutarEXP(nodo.ChildNodes[1]);
                    if (!aux2.tipo.Equals(Cadena.error)) {
                        String line=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        if (!aux2.tipo.Equals(Cadena.Nulo))
                        {

                            if (aux2.tipo.Equals(Cadena.Fecha) || aux2.tipo.Equals(Cadena.Hora) || aux2.tipo.Equals(Cadena.FechaHora))
                            {
                                String error = "ERROR SEMANTICO: No se puede convertir -> "+ aux2.tipo +" a BOOLEANO"+" L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line, column);                               
                            }
                            
                            switch (aux2.tipo) { 
                                case Cadena.Entero:
                                    int num = Int32.Parse(aux2.valor.ToString());
                                    if (num > 0)
                                        return new retorno("verdadero", Cadena.Booleano, line, column);
                                    else 
                                        return new retorno("falso", Cadena.Booleano, line, column);
                                case Cadena.Decimmal:
                                    double num2 = Double.Parse(aux2.valor.ToString());
                                    if (num2 > 0)
                                        return new retorno("verdadero", Cadena.Booleano, line, column);
                                    else
                                        return new retorno("falso", Cadena.Booleano, line, column);
                                case Cadena.Cad:
                                    String cad = aux2.valor.ToString();
                                    if (cad.Length > 0)
                                        return new retorno("verdadero", Cadena.Booleano, line, column);
                                    else
                                        return new retorno("falso", Cadena.Booleano, line, column);
                                default:
                                    if (aux2.valor != null)
                                        return new retorno("verdadero", Cadena.Booleano, line, column);
                                    else
                                        return new retorno("falso", Cadena.Booleano, line, column);
                            }
                        }
                        else {
                            String error = "ERROR SEMANTICO: No se puede convertir NULO a BOOLEANO -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, line, column);
                        }
                    }
                    return aux2;
                    #endregion;
                case Cadena.ENTERO:
                    #region
                    aux2 = ejecutarEXP(nodo.ChildNodes[1]);
                    if (!aux2.tipo.Equals(Cadena.error)) {
                        String line=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        if (!aux2.tipo.Equals(Cadena.Nulo))
                        {
                            if (aux2.tipo.Equals(Cadena.Fecha) || aux2.tipo.Equals(Cadena.Hora) || aux2.tipo.Equals(Cadena.FechaHora))
                            {
                                int dias = convertiraDias(aux2.valor.ToString(), aux2.tipo);
                                return new retorno(dias, Cadena.Entero, line, column);
                            }
                            
                            switch (aux2.tipo) { 
                                case Cadena.Entero:
                                    return aux2;
                                case Cadena.Decimmal:
                                    double num = Double.Parse(aux2.valor.ToString());
                                    num=Math.Round(num, 0);
                                    return new retorno((int)num, Cadena.Entero, line, column);
                                case Cadena.Cad:
                                    String cad = aux2.valor.ToString();
                                    return new retorno(calcularASCII(cad), Cadena.Entero, line, column);
                                case Cadena.Booleano:
                                    if (aux2.valor.ToString().ToLower().Equals("verdadero") || aux2.valor.ToString().ToLower().Equals("'verdadero'"))
                                        return new retorno(1, Cadena.Entero, line, column);
                                    else
                                        return new retorno(0, Cadena.Entero, line, column);
                                default:
                                   String error = "ERROR SEMANTICO: No se puede convertir -> "+ aux2.tipo +" a ENTERO"+" L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno("error", Cadena.error, line, column);
                            }
                        }
                        else {
                            String error = "ERROR SEMANTICO: No se puede convertir NULO a Entero -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, line, column);
                        }
                    }
                    return aux2;
                    #endregion
                case Cadena.TAM:
                    #region
                    aux2 = ejecutarEXP(nodo.ChildNodes[1]);
                    if (!aux2.tipo.Equals(Cadena.error)) {
                        String line=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        if (!aux2.tipo.Equals(Cadena.Nulo))
                        {
                            if (aux2.tipo.Equals(Cadena.Fecha) || aux2.tipo.Equals(Cadena.Hora) || aux2.tipo.Equals(Cadena.FechaHora))
                            {
                                int dias = convertiraDias(aux2.valor.ToString(), aux2.tipo);
                                return new retorno(dias, Cadena.Entero, line, column);
                            }
                            
                            switch (aux2.tipo) { 
                                case Cadena.Entero:
                                    return aux2;
                                case Cadena.Decimmal:
                                    double num = Double.Parse(aux2.valor.ToString());
                                    num=Math.Round(num, 0);
                                    return new retorno((int)num, Cadena.Entero, line, column);
                                case Cadena.Cad:
                                    String cad = aux2.valor.ToString();
                                    return new retorno(cad.Length, Cadena.Entero, line, column);
                                case Cadena.Booleano:
                                    if (aux2.valor.ToString().ToLower().Equals("verdadero") || aux2.valor.ToString().ToLower().Equals("'verdadero'"))
                                        return new retorno(1, Cadena.Entero, line, column);
                                    else
                                        return new retorno(0, Cadena.Entero, line, column);
                                default:
                                   String error = "ERROR SEMANTICO: No se puede convertir -> "+ aux2.tipo +" a ENTERO"+" L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno("error", Cadena.error, line, column);
                            }
                        }
                        else {
                            String error = "ERROR SEMANTICO: No se puede convertir NULO a Entero -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, line, column);
                        }
                    }
                    return aux2;
                    #endregion    
                case Cadena.RANDOM:
                    #region
                    //viene una lista de expresiones
                    if (nodo.ChildNodes[1].ChildNodes.Count > 0)
                    {
                        String line = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        List<retorno> parametros = parametros = new List<retorno>();
                        foreach (ParseTreeNode subHijo in nodo.ChildNodes[1].ChildNodes)
                        {
                            retorno ret = ejecutarEXP(subHijo);
                            if (ret.tipo.Equals(Cadena.Entero) || ret.tipo.Equals(Cadena.Decimmal) || ret.tipo.Equals(Cadena.Cad))
                                parametros.Add(ret);
                            else {
                                String error = "ERROR SEMANTICO: Parametros incorrectos en la funcion RANDOM -> " + ret.tipo + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, "0", "0");
                            }
                        }

                        Random rd = new Random();
                        return parametros.ElementAt(rd.Next(parametros.Count));
                    }
                    //viene sin lista de parametros
                    else {
                        String line=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        Random rd = new Random();
                        return new retorno(rd.Next(2), Cadena.Entero, line, column);
                    }
                    #endregion
                case Cadena.MIN:
                    #region
                    //viene una lista de expresiones
                    if (nodo.ChildNodes[1].ChildNodes.Count > 0)
                    {
                        String line = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        List<retorno> parametros = parametros = new List<retorno>();
                        retorno min=null;
                        double val_min=0;
                        double val_aux=0;
                        foreach (ParseTreeNode subHijo in nodo.ChildNodes[1].ChildNodes)
                        {
                            retorno ret = ejecutarEXP(subHijo);
                            if (ret.tipo.Equals(Cadena.Entero) || ret.tipo.Equals(Cadena.Decimmal) || ret.tipo.Equals(Cadena.Cad)){
                                if (min == null)
                                {
                                    if(ret.tipo.Equals(Cadena.Entero)){
                                        int val = Int32.Parse(ret.valor.ToString());
                                        val_min=Double.Parse(val.ToString());
                                    }else if(ret.tipo.Equals(Cadena.Decimmal)){
                                        val_min = Double.Parse(ret.valor.ToString());
                                    }else{
                                        int val =calcularASCII(ret.valor.ToString());
                                        val_min=Double.Parse(val.ToString());
                                    }
                                    min = ret;
                                }
                                else {
                                    if (ret.tipo.Equals(Cadena.Entero))
                                    {
                                        int val = Int32.Parse(ret.valor.ToString());
                                        val_aux = Double.Parse(val.ToString());
                                        if (val_aux < val_min) {
                                            val_min = val_aux;
                                            min = ret;
                                        }
                                    }
                                    else if (ret.tipo.Equals(Cadena.Decimmal))
                                    {
                                        val_aux = Double.Parse(ret.valor.ToString());
                                        if (val_aux < val_min){
                                            val_min = val_aux;
                                            min = ret;
                                        }
                                    }
                                    else
                                    {
                                        int val = calcularASCII(ret.valor.ToString());
                                        val_aux = Double.Parse(val.ToString());
                                        if (val_aux < val_min)
                                        {
                                            val_min = val_aux;
                                            min = ret;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                String error = "ERROR SEMANTICO: Parametros incorrectos en la funcion RANDOM -> " + ret.tipo + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, "0", "0");
                            }
                        }
                        //retornamos el minimo
                        return min;
                    }
                    //viene sin lista de parametros
                    else
                    {
                        String line = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        String error = "ERROR SEMANTICO: Debe agregar al menod un parametro en la funcion  MIN"  + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return new retorno("error", Cadena.error, "0", "0");
                    }
                    #endregion
                case Cadena.MAX:
                    #region
                    //viene una lista de expresiones
                    if (nodo.ChildNodes[1].ChildNodes.Count > 0)
                    {
                        String line = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        List<retorno> parametros = parametros = new List<retorno>();
                        retorno max = null;
                        double val_max = 0;
                        double val_aux = 0;
                        foreach (ParseTreeNode subHijo in nodo.ChildNodes[1].ChildNodes)
                        {
                            retorno ret = ejecutarEXP(subHijo);
                            if (ret.tipo.Equals(Cadena.Entero) || ret.tipo.Equals(Cadena.Decimmal) || ret.tipo.Equals(Cadena.Cad))
                            {
                                if (max == null)
                                {
                                    if (ret.tipo.Equals(Cadena.Entero))
                                    {
                                        int val = Int32.Parse(ret.valor.ToString());
                                        val_max = Double.Parse(val.ToString());
                                    }
                                    else if (ret.tipo.Equals(Cadena.Decimmal))
                                    {
                                        val_max = Double.Parse(ret.valor.ToString());
                                    }
                                    else
                                    {
                                        int val = calcularASCII(ret.valor.ToString());
                                        val_max = Double.Parse(val.ToString());
                                    }
                                    max = ret;
                                }
                                else
                                {
                                    if (ret.tipo.Equals(Cadena.Entero))
                                    {
                                        int val = Int32.Parse(ret.valor.ToString());
                                        val_aux = Double.Parse(val.ToString());
                                        if (val_aux > val_max)
                                        {
                                            val_max = val_aux;
                                            max = ret;
                                        }
                                    }
                                    else if (ret.tipo.Equals(Cadena.Decimmal))
                                    {
                                        val_aux = Double.Parse(ret.valor.ToString());
                                        if (val_aux > val_max)
                                        {
                                            val_max = val_aux;
                                            max = ret;
                                        }
                                    }
                                    else
                                    {
                                        int val = calcularASCII(ret.valor.ToString());
                                        val_aux = Double.Parse(val.ToString());
                                        if (val_aux > val_max)
                                        {
                                            val_max = val_aux;
                                            max = ret;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                String error = "ERROR SEMANTICO: Parametros incorrectos en la funcion MAX -> " + ret.tipo + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, "0", "0");
                            }
                        }
                        //retornamos el minimo
                        return max;
                    }
                    //viene sin lista de parametros
                    else
                    {
                        String line = (nodo.ChildNodes[0].Token.Location.Line + 1) + "";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        String error = "ERROR SEMANTICO: Debe agregar al menod un parametro en la funcion  MAX" + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return new retorno("error", Cadena.error, "0", "0");
                    }
                    #endregion
                case Cadena.POW:
                    #region
                    aux2 = ejecutarEXP(nodo.ChildNodes[1]);
                    if (!aux2.tipo.Equals(Cadena.error)) {
                        String line=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        if (!aux2.tipo.Equals(Cadena.Nulo))
                        {
                            if (aux2.tipo.Equals(Cadena.Entero) || aux2.tipo.Equals(Cadena.Decimmal))
                            {
                                try 
	                            {	
                                    double pot=Double.Parse(nodo.ChildNodes[2].Token.Value.ToString());
                                    double num1 = Double.Parse(aux2.valor.ToString());
                                    num1 = Math.Pow(num1, pot);
                                    return new retorno(Math.Round(num1, 2), Cadena.Decimmal, line, column);
	                            }
	                            catch (Exception)
	                            {
                                    String error = "ERROR SEMANTICO: No se puede elevar la expresion por ser un resultado muy grande -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno("error", Cadena.error, line, column);
	                            }  
                            }else{
                                String error = "ERROR SEMANTICO: No se puede elevar un expresion de tipo -> " + aux2.tipo  + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line, column);
                            }                         
                        }
                        else {
                            String error = "ERROR SEMANTICO: No se puede elevar el tipo NULO" + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, line, column);
                        }
                    }
                    return aux2;
                    #endregion
                case Cadena.LOG1:
                    #region
                     aux2 = ejecutarEXP(nodo.ChildNodes[1]);
                    if (!aux2.tipo.Equals(Cadena.error)) {
                        String line=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        if (!aux2.tipo.Equals(Cadena.Nulo))
                        {
                            if (aux2.tipo.Equals(Cadena.Entero) || aux2.tipo.Equals(Cadena.Decimmal))
                            {
                                try 
	                            {	
                                    double log=Double.Parse(aux2.valor.ToString());
                                    if (log > 0)
                                    {
                                        log = Math.Log(log);
                                        return new retorno(Math.Round(log, 2), Cadena.Decimmal, line, column);
                                    }
                                    else {
                                        String error = "ERROR SEMANTICO: No se puede sacar el logartimo a 0 o un numero negatio -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, line, column);
                                    }
                                    
	                            }
	                            catch (Exception)
	                            {
                                    String error = "ERROR SEMANTICO: No se puede sacar el logartimo de la EXP por desborde -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno("error", Cadena.error, line, column);
	                            }  
                            }else{
                                String error = "ERROR SEMANTICO: No se puede operar logartimo con una expresion de tipo -> " + aux2.tipo  + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line, column);
                            }                         
                        }
                        else {
                            String error = "ERROR SEMANTICO: No se puede operar logartimo con una expresion de tipo NULO" + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, line, column);
                        }
                    }
                    return aux2;
                    #endregion
                case Cadena.LOG10:
                    #region
                    aux2 = ejecutarEXP(nodo.ChildNodes[1]);
                    if (!aux2.tipo.Equals(Cadena.error)) {
                        String line=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        if (!aux2.tipo.Equals(Cadena.Nulo))
                        {
                            if (aux2.tipo.Equals(Cadena.Entero) || aux2.tipo.Equals(Cadena.Decimmal))
                            {
                                try 
	                            {	
                                    double log=Double.Parse(aux2.valor.ToString());
                                    if (log > 0)
                                    {
                                        log = Math.Log10(log);
                                        return new retorno(Math.Round(log,2), Cadena.Decimmal, line, column);
                                    }
                                    else {
                                        String error = "ERROR SEMANTICO: No se puede sacar el logartimo_base_10 a 0 o un numero negatio -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, line, column);
                                    }
                                    
	                            }
	                            catch (Exception)
	                            {
                                    String error = "ERROR SEMANTICO: No se puede sacar el logartimo_base_10 de la EXP por desborde -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno("error", Cadena.error, line, column);
	                            }  
                            }else{
                                String error = "ERROR SEMANTICO: No se puede sacar el logartimo_base_10 a una expresion de tipo -> " + aux2.tipo + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line, column);
                            }                         
                        }
                        else {
                            String error = "ERROR SEMANTICO: No se puede sacar el logartimo_base_10 a  NULO" + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, line, column);
                        }
                    }
                    return aux2;
                    #endregion
                case Cadena.ABS:
                    #region
                     aux2 = ejecutarEXP(nodo.ChildNodes[1]);
                    if (!aux2.tipo.Equals(Cadena.error)) {
                        String line=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        if (!aux2.tipo.Equals(Cadena.Nulo))
                        {
                            if (aux2.tipo.Equals(Cadena.Entero) || aux2.tipo.Equals(Cadena.Decimmal))
                            {
                                if (aux2.tipo.Equals(Cadena.Entero))
                                {
                                    int num1 = Int32.Parse(aux2.valor.ToString());
                                    num1 = Math.Abs(num1);
                                    return new retorno(num1, Cadena.Entero, line, column);
                                }
                                else {
                                    double num1 = Double.Parse(aux2.valor.ToString());
                                    num1 = Math.Abs(num1);
                                    return new retorno(num1, Cadena.Decimmal, line, column);
                                }  
                            }else{
                                String error = "ERROR SEMANTICO: No se puede sacar el valor ABS a una expresion de tipo -> " + aux2.tipo + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line, column);
                            }                         
                        }
                        else {
                            String error = "ERROR SEMANTICO: No se puede sacar el valor ABS a  NULO" + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, line, column);
                        }
                    }
                    return aux2;
                    #endregion
                case Cadena.SIN:
                    #region
                    aux2 = ejecutarEXP(nodo.ChildNodes[1]);
                    if (!aux2.tipo.Equals(Cadena.error)) {
                        String line=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        if (!aux2.tipo.Equals(Cadena.Nulo))
                        {
                            if (aux2.tipo.Equals(Cadena.Entero) || aux2.tipo.Equals(Cadena.Decimmal))
                            {
                                try 
	                            {	
                                    double val=Double.Parse(aux2.valor.ToString());
                                    val = Math.Sin(val);
                                    return new retorno(Math.Round(val,2), Cadena.Decimmal, line, column);
	                            }
	                            catch (Exception)
	                            {
                                    String error = "ERROR SEMANTICO: No se puede sacar el SENO de la EXP por desborde -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno("error", Cadena.error, line, column);
	                            }  
                            }else{
                                String error = "ERROR SEMANTICO: No se puede sacar el SENO a una expresion de tipo -> " + aux2.tipo + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line, column);
                            }                         
                        }
                        else {
                            String error = "ERROR SEMANTICO: No se puede sacar el SENO a  NULO" + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, line, column);
                        }
                    }
                    return aux2;
                    #endregion
                case Cadena.COS:
                    #region
                    aux2 = ejecutarEXP(nodo.ChildNodes[1]);
                    if (!aux2.tipo.Equals(Cadena.error)) {
                        String line=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        if (!aux2.tipo.Equals(Cadena.Nulo))
                        {
                            if (aux2.tipo.Equals(Cadena.Entero) || aux2.tipo.Equals(Cadena.Decimmal))
                            {
                                try 
	                            {	
                                    double val=Double.Parse(aux2.valor.ToString());
                                    val = Math.Cos(val);

                                    return new retorno(Math.Round(val, 2), Cadena.Decimmal, line, column);
	                            }
	                            catch (Exception)
	                            {
                                    String error = "ERROR SEMANTICO: No se puede sacar el COSENO de la EXP por desborde -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno("error", Cadena.error, line, column);
	                            }  
                            }else{
                                String error = "ERROR SEMANTICO: No se puede sacar el COSENO a una expresion de tipo -> " + aux2.tipo + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line, column);
                            }                         
                        }
                        else {
                            String error = "ERROR SEMANTICO: No se puede sacar el COSENO a  NULO" + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, line, column);
                        }
                    }
                    return aux2;
                    #endregion
                case Cadena.TAN:
                    #region
                    aux2 = ejecutarEXP(nodo.ChildNodes[1]);
                    if (!aux2.tipo.Equals(Cadena.error)) {
                        String line=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        if (!aux2.tipo.Equals(Cadena.Nulo))
                        {
                            if (aux2.tipo.Equals(Cadena.Entero) || aux2.tipo.Equals(Cadena.Decimmal))
                            {
                                try 
	                            {	
                                    double val=Double.Parse(aux2.valor.ToString());
                                    val = Math.Tan(val);
                                    return new retorno(Math.Round(val,2), Cadena.Decimmal, line, column);
	                            }
	                            catch (Exception)
	                            {
                                    String error = "ERROR SEMANTICO: No se puede sacar el TAN de la EXP por desborde -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno("error", Cadena.error, line, column);
	                            }  
                            }else{
                                String error = "ERROR SEMANTICO: No se puede sacar el TAN a una expresion de tipo -> " + aux2.tipo + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line, column);
                            }                         
                        }
                        else {
                            String error = "ERROR SEMANTICO: No se puede sacar el TAN a  NULO" + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, line, column);
                        }
                    }
                    return aux2;
                    #endregion
                case Cadena.SQRT:
                     #region
                    aux2 = ejecutarEXP(nodo.ChildNodes[1]);
                    if (!aux2.tipo.Equals(Cadena.error)) {
                        String line=(nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String column = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                        if (!aux2.tipo.Equals(Cadena.Nulo))
                        {
                            if (aux2.tipo.Equals(Cadena.Entero) || aux2.tipo.Equals(Cadena.Decimmal))
                            {
                                try 
	                            {	
                                    double val=Double.Parse(aux2.valor.ToString());
                                    if (val > -1)
                                    {
                                        val = Math.Sqrt(val);
                                        return new retorno(Math.Round(val,2), Cadena.Decimmal, line, column);
                                    }
                                    else {
                                        String error = "ERROR SEMANTICO: No se puede sacar SQRT a un numero negatio -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                        return new retorno("error", Cadena.error, line, column);
                                    }
                                    
	                            }
	                            catch (Exception)
	                            {
                                    String error = "ERROR SEMANTICO: No se puede sacar SQRT de la EXP por desborde -> " + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return new retorno("error", Cadena.error, line, column);
	                            }  
                            }else{
                                String error = "ERROR SEMANTICO: No se puede sacar SQRT a una expresion de tipo -> " + aux2.tipo + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, line, column);
                            }                         
                        }
                        else {
                            String error = "ERROR SEMANTICO: No se puede sacar el SQRT a  NULO" + " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, line, column);
                        }
                    }
                    return aux2;
                    #endregion
                case Cadena.PI:
                    return new retorno(Math.Round(Math.PI, 4), Cadena.Decimmal, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                case Cadena.HOY:
                    return new retorno(DateTime.Now.ToString("dd/MM/yyyy"), Cadena.Fecha, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                case Cadena.AHORA:
                    return new retorno(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Cadena.FechaHora, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                case Cadena.FECHA:
                    #region
                    String cad01 = nodo.ChildNodes[1].Token.Value.ToString();
                    if (Regex.IsMatch(cad01, @"^(0[1-9]|1[0-9]|2[0-9]|3[0-1])[/](0[1-9]|1[0-2])[/](\d{4})"))
                    {
                        return new retorno(cad01, Cadena.Fecha, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                    }
                    else
                    {
                        String error = "ERROR SEMANTICO: La cadena no tiene un formato de FECHA Valido -> " +cad01+ " L: " + (nodo.ChildNodes[0].Token.Location.Line + 1) + " C: " + (nodo.ChildNodes[0].Token.Location.Column + 1) + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return new retorno("error", Cadena.error, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                    }
                    #endregion
                case Cadena.HORA:
                    #region
                    String cad02 = nodo.ChildNodes[1].Token.Value.ToString();
                     if (Regex.IsMatch(cad02, @"^(0[0-9]|1[0-9]|2[0-3])[:]([0-5][0-9]|60)[:]([0-5][0-9]|60)"))
                    {
                        return new retorno(cad02, Cadena.Hora, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                    }
                    else
                    {
                        String error = "ERROR SEMANTICO: La cadena no tiene un formato de HORA Valido -> " +cad02+ " L: " + (nodo.ChildNodes[0].Token.Location.Line + 1) + " C: " + (nodo.ChildNodes[0].Token.Location.Column + 1) + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return new retorno("error", Cadena.error, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                    }
                    #endregion
                case Cadena.FECHAHORA:
                     #region
                    String cad03 = nodo.ChildNodes[1].Token.Value.ToString();
                    if (Regex.IsMatch(cad03, @"^(0[1-9]|1[0-9]|2[0-9]|3[0-1])[/](0[1-9]|1[0-2])[/](\d{4})[ ](0[0-9]|1[0-9]|2[0-3])[:]([0-5][0-9]|60)[:]([0-5][0-9]|60)"))
                    {
                        return new retorno(cad03, Cadena.FechaHora, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                    }
                    else
                    {
                        String error = "ERROR SEMANTICO: La cadena no tiene un formato de FECHAHORA Valido -> " +cad03+ " L: " + (nodo.ChildNodes[0].Token.Location.Line + 1) + " C: " + (nodo.ChildNodes[0].Token.Location.Column + 1) + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return new retorno("error", Cadena.error, (nodo.ChildNodes[0].Token.Location.Line + 1) + "", (nodo.ChildNodes[0].Token.Location.Column + 1) + "");
                    }
                #endregion
                case Cadena.GET_OP: // esto es sobre las opciones
                    #region
                    Simbolo tmp05 = existeVariable2(nodo.ChildNodes[0].Token.Value.ToString());
                     if(tmp05!=null){
                        if(tmp05.TipoObjeto.Equals(Cadena.Opciones)){
                            Opciones op = (Opciones)tmp05.Valor;
                            int ind_elem= Int32.Parse(nodo.ChildNodes[2].Token.Value.ToString());
                            int ind_val= Int32.Parse(nodo.ChildNodes[3].Token.Value.ToString());
                            aux2 = op.obtner(ind_elem, ind_val);
                            if (aux2 != null)
                            {
                                return aux2;
                            }   
                            else {
                                String error = "ERROR SEMANTICO: Los indices de acceso a la  lisa de OPCIONES son incorrectos ->  " + tmp05.Nombre + " L: " + (nodo.ChildNodes[0].Token.Location.Line + 1) + " C: " + (nodo.ChildNodes[0].Token.Location.Column + 1) + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, "0", "0");
                            }
                        }else{
                            String error = "ERROR SEMANTICO: La lista a acceder no es de tipo OPCIONES ->  " + tmp05.Nombre + " L: " + (nodo.ChildNodes[0].Token.Location.Line + 1) + " C: " + (nodo.ChildNodes[0].Token.Location.Column + 1) + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, "0", "0");
                        }  
                     }else{ // variable no existe
                        String error = "ERROR SEMANTICO: La lista de opciones a acceder no esta definida ->  " + nodo.ChildNodes[0].Token.Value.ToString() + " L: " + (nodo.ChildNodes[0].Token.Location.Line + 1) + " C: " + (nodo.ChildNodes[0].Token.Location.Column + 1) + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return new retorno("error", Cadena.error, "0", "0");
                     }
                    #endregion
                case Cadena.SEARCH_OP: //esto es sobre las opciones
                    #region
                    Simbolo tmp06 = existeVariable2(nodo.ChildNodes[0].Token.Value.ToString());
                     if(tmp06!=null){
                        if(tmp06.TipoObjeto.Equals(Cadena.Opciones)){
                            Opciones op = (Opciones)tmp06.Valor;
                            retorno ret = ejecutarEXP(nodo.ChildNodes[2]);
                            int ind_val= Int32.Parse(nodo.ChildNodes[3].Token.Value.ToString());
                            aux2 = op.buscar(ret, ind_val);
                            if (aux2 != null)
                            {
                                return aux2;
                            }   
                            else {
                                String error = "ERROR SEMANTICO: Los valores de busqueda en la  lista de OPCIONES son incorrectos ->  " + tmp06.Nombre + " L: " + (nodo.ChildNodes[0].Token.Location.Line + 1) + " C: " + (nodo.ChildNodes[0].Token.Location.Column + 1) + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                                return new retorno("error", Cadena.error, "0", "0");
                            }
                        }else{
                            String error = "ERROR SEMANTICO: La lista a acceder no es de tipo OPCIONES ->  " + tmp06.Nombre + " L: " + (nodo.ChildNodes[0].Token.Location.Line + 1) + " C: " + (nodo.ChildNodes[0].Token.Location.Column + 1) + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return new retorno("error", Cadena.error, "0", "0");
                        }  
                     }else{ // variable no existe
                        String error = "ERROR SEMANTICO: La lista de opciones a acceder no esta definida ->  " + nodo.ChildNodes[0].Token.Value.ToString() + " L: " + (nodo.ChildNodes[0].Token.Location.Line + 1) + " C: " + (nodo.ChildNodes[0].Token.Location.Column + 1) + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return new retorno("error", Cadena.error, "0", "0");
                     }
                     #endregion            
                case Cadena.ACC_PRE:
                     #region
                    String preg = nodo.ChildNodes[0].Token.Value.ToString();
                    String linep = (nodo.ChildNodes[0].Token.Location.Line+1)+"";
                    String columnp = (nodo.ChildNodes[0].Token.Location.Column + 1) + "";
                    Pregunta pre = Preguntas.retornaPregunta(preg);
                    if (pre!=null)
                    {
                        return new retorno(pre.respuesta, pre.tipo, linep, columnp);
                    }
                    else {
                        String error = "ERROR SEMANTICO: La  PREGUNTA a acceder no esta definida ->  " +preg+ " L: " + linep + " C: " +columnp + " Clase: " + claseActual.Nombre;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return new retorno("error", Cadena.error, "0", "0");
                    }
                     #endregion
                #endregion
            }
            return new retorno(1,"entero","01001","01001");
        }
        public void capturarDecAsigs(ParseTreeNode nodo) {//aca ya recibe el nodo unico 
            if (nodo != null) {
                switch (nodo.Term.Name)
                {
//=============================DECLARA Y ASIGNA MAT GLOABAL=================================================================================
                    case Cadena.DEC_ASIGNA_MAT:
                        #region
                        String tipo1 = nodo.ChildNodes[0].Token.Value.ToString();
                        String vis = "publico";
                        if (nodo.ChildNodes[1].ChildNodes.Count > 0) {
                            vis = nodo.ChildNodes[1].ChildNodes[0].Token.Value.ToString(); 
                        }
                        String name = nodo.ChildNodes[2].Token.Value.ToString();
                        String line = (nodo.ChildNodes[2].Token.Location.Line+1)+"";
                        String colum = (nodo.ChildNodes[2].Token.Location.Column-3)+"";
                        int dims1 = nodo.ChildNodes[3].ChildNodes.Count;
                        Simbolo tmp00 = existeVariable4(name); // acá se valida que no exista la variable
                        if (tmp00 == null)
                        {
                            //aca vamos a saber si solo se declara o se asigna la matriz de una 
                            if (nodo.ChildNodes[4].ChildNodes.Count > 1)
                            {
                                String tip = nodo.ChildNodes[4].ChildNodes[1].Token.Value.ToString();
                                if (tip.ToLower().Equals(tipo1.ToLower()))
                                {
                                    int dim2 = nodo.ChildNodes[4].ChildNodes[2].ChildNodes.Count;
                                    if (dims1 == dim2)
                                    {
                                        List<Object> valores = new List<object>();
                                        Simbolo sim = new Simbolo(cima.Nivel, name, valores, tipo1, line, colum);
                                        sim.Visibilidad = vis;
                                        sim.TipoObjeto = Cadena.Matriz;
                                        sim.iniciaDims();
                                        int tamMat = 1;
                                        foreach (ParseTreeNode hijo in nodo.ChildNodes[4].ChildNodes[2].ChildNodes)
                                        {
                                            sim.dimenciones.Add(Int32.Parse(hijo.Token.Value.ToString()));
                                            tamMat = tamMat * Int32.Parse(hijo.Token.Value.ToString());
                                        }
                                        for (int i = 0; i < tamMat; i++)
                                        {
                                            valores.Add(0);
                                        }
                                        cima.insertar(name, sim,claseActual.Nombre);
                                    }
                                    else
                                    {
                                        String error = "ERROR SEMANTICO: Dimensiones incorrestas en la dec de la matriz -> " + name + " L: " + line + " C: " + colum + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                    }
                                }
                                else
                                {//error no son del mismo tipo
                                    String error = "ERROR SEMANTICO: Incopatibilidad de tipos en la dec de la matriz -> " + name + " L: " + line + " C: " + colum + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                }
                            }
                            else//aca es con la asignacion con valores incluidos 
                            {
                                List<Object> tmp_val = new List<object>();
                                List<int> tmp_dim = new List<int>();
                                capturarDims(tmp_dim, nodo.ChildNodes[4].ChildNodes[0]);
                                capturarVals(tmp_val, nodo.ChildNodes[4].ChildNodes[0]);
                                if (validarTipos(tmp_val))
                                {
                                    String tip = ((retorno)tmp_val[0]).tipo;
                                    if (tipo1.ToLower().Equals(tip.ToLower())) {
                                        if (dims1 == tmp_dim.Count)
                                        {                                           
                                            List<Object> valores = new List<object>();
                                            Simbolo sim = new Simbolo(cima.Nivel, name, valores, tipo1, line, colum);
                                            sim.Visibilidad = vis;
                                            sim.TipoObjeto = Cadena.Matriz;
                                            sim.iniciaDims();
                                            for (int i = 0; i < tmp_val.Count; i++)
                                            {
                                                valores.Add(((retorno)tmp_val[i]).valor);
                                            }
                                            for (int i = 0; i < tmp_dim.Count; i++)
                                            {
                                                sim.dimenciones.Add(tmp_dim.ElementAt(i));
                                            }
                                            cima.insertar(name, sim,claseActual.Nombre);
                                        }
                                        else {
                                            String error = "ERROR SEMANTICO: Dimensiones incorrestas en la dec/asig de la matriz -> " + name + " L: " + line + " C: " + colum + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                        }
                                    }
                                    else
                                    {
                                        String error = "ERROR SEMANTICO: Incopatibilidad de tipos en la dec/asig de la matriz -> " + name + " L: " + line + " C: " + colum + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                    }
                                }
                                else {
                                    String error = "ERROR SEMANTICO: Incopatibilidad de tipos en valores a asignar a la matriz -> " + name + " L: " + line + " C: " + colum + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                }
                            }
                        }
                        else {
                            String error = "ERROR SEMANTICO: La variable ya se encuentra definida -> " + name + " L: " + line + " C: " + colum + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                        } 
                        #endregion
                        break;
//=============================DECLARA Y ASIGNA VAR GLOABAL=================================================================================
                    case Cadena.DEC_ASIGNA_VAR:
                        #region
                        String name2 = nodo.ChildNodes[2].Token.Value.ToString();
                        String line2 = (nodo.ChildNodes[2].Token.Location.Line+1)+"";
                        String colum2 = (nodo.ChildNodes[2].Token.Location.Column-3)+"";
                        String visibilidad = "publico";
                        if (nodo.ChildNodes[1].ChildNodes.Count > 0)
                        {
                            visibilidad = nodo.ChildNodes[1].ChildNodes[0].Token.Value.ToString();
                        }
                        Simbolo tmp06 = existeVariable4(name2);
                        if (tmp06 == null)
                        {//la variable no esxiste la puedo crear      
                            retorno ret = ejecutarEXP(nodo.ChildNodes[3]);
                            Simbolo tmp07;
                            if (!ret.tipo.Equals(Cadena.error))
                            {
                                String tipo = nodo.ChildNodes[0].Token.Value.ToString();
                                if (nodo.ChildNodes[0].Term.Name.Equals(Cadena.Id))
                                {
                                    if (Clases.existeClase(tipo))
                                    {
                                        if (comprobarTipo(tipo, ret))
                                        {
                                            tmp07 = new Simbolo(cima.Nivel, name2, ret.valor, tipo, line2, colum2);
                                            tmp07.TipoObjeto = Cadena.Objeto;
                                            tmp07.Visibilidad = visibilidad;
                                            cima.insertar(name2, tmp07, claseActual.Nombre);
                                        }
                                        else
                                        {
                                            String error = "ERROR SEMANTICO: Incompatibilidad de tipos en la asignacion de la variable  -> " + name2 + " L: " + line2 + " C: " + colum2 + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                        }
                                    }
                                    else
                                    {
                                        String error = "ERROR SEMANTICO: El tipo de objeto a declar es inaccesible -> " + name2 + " L: " + line2 + " C: " + colum2 + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                    }
                                }
                                else
                                {
                                    if (comprobarTipo(tipo, ret))
                                    {
                                        tmp07 = new Simbolo(cima.Nivel, name2, ret.valor, tipo, line2, colum2);
                                        tmp07.Visibilidad = visibilidad;
                                        cima.insertar(name2, tmp07, claseActual.Nombre);
                                    }
                                    else
                                    {
                                        String error = "ERROR SEMANTICO: Incompatibilidad de tipos en la asignacion de la variable  -> " + name2 + " L: " + line2 + " C: " + colum2 + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                    }
                                }      
                            }
                        }
                        else
                        {
                            String error = "ERROR SEMANTICO: La variable ya se encuentra definida -> " + name2 + " L: " + line2 + " C: " + colum2 + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                        }
                        #endregion
                        break;
//=============================ASIGNA MAT GLOBAL/LOCAL======================================================================================
                    case Cadena.ASIGNA_MAT:
                        #region
                        String name3 = nodo.ChildNodes[0].Token.Value.ToString();
                        String line3 = (nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String colum3 = (nodo.ChildNodes[0].Token.Location.Column+1)+"";
                        Simbolo tmp_sim = existeVariable2(name3);
                        if (tmp_sim != null)
                        {
                            if (tmp_sim.TipoObjeto.Equals(Cadena.Matriz))//validamos que sea de tipo matriz
                            {
                                int dim2=nodo.ChildNodes[1].ChildNodes.Count;
                                if (tmp_sim.dimenciones.Count == dim2) //validamos que sean de la misma dimension 
                                {

                                    retorno ret = ejecutarEXP(nodo.ChildNodes[2]);
                                    if (comprobarTipo(tmp_sim.Tipo,ret))//validamos que sea del mismo tipo
                                    {
                                        List<Object> indices = new List<object>();
                                        capturarVals(indices, nodo.ChildNodes[1]);
                                        if (validarIndices(indices, tmp_sim.dimenciones))
                                        {
                                            int indice = linealizar(indices, tmp_sim.dimenciones);
                                            ((List<Object>)tmp_sim.Valor)[indice] = ret.valor;
                                        }//lo errores los indico en el metodo validarIndices :)
                                    }
                                    else { // incoptibilidad de tipos
                                        String error = "ERROR SEMANTICO: Incopatibilidad de tipos en la asignacion de la matriz -> " + name3 + " L: " + line3 + " C: " + colum3 + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                    }
                                }
                                else {
                                    String error = "ERROR SEMANTICO: Las dimensiones de la matriz no coinciden -> " + name3 + " L: " + line3 + " C: " + colum3 + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                }
                            }
                            else {
                                String error = "ERROR SEMANTICO: La variable a asignar no es de tipo matriz -> " + name3 + " L: " + line3 + " C: " + colum3 + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                            }
                        }
                        else {
                            String error = "ERROR SEMANTICO: La variable no ha sido definida -> " + name3 + " L: " + line3 + " C: " + colum3 + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);   
                        }
                        #endregion
                        break;
//=============================DECLARA VAR GLOABAL =========================================================================================
                    case Cadena.DEC_VAR:
                        #region
                        String name4 = nodo.ChildNodes[2].Token.Value.ToString();
                        String line4 = (nodo.ChildNodes[2].Token.Location.Line+1)+"";
                        String colum4 = (nodo.ChildNodes[2].Token.Location.Column+1)+"";
                        String visibilidad4 = "publico";
                        if (nodo.ChildNodes[1].ChildNodes.Count > 0)
                        {
                            visibilidad4 = nodo.ChildNodes[1].ChildNodes[0].Token.Value.ToString();
                        }
                        Simbolo tmp02 = existeVariable4(name4);
                        if (tmp02 == null)
                        {//la variable no esxiste la puedo crear
                            if (nodo.ChildNodes[0].Term.Name.Equals(Cadena.Id))
                            {
                                String tipo=nodo.ChildNodes[0].Token.Value.ToString();
                                if (Clases.existeClase(tipo))
                                {
                                    tmp02 = new Simbolo(cima.Nivel, name4, Cadena.Nulo, tipo, line4, colum4);
                                    tmp02.TipoObjeto = Cadena.Objeto;
                                    tmp02.Visibilidad = visibilidad4;
                                    cima.insertar(name4, tmp02, claseActual.Nombre);
                                }
                                else {
                                    String error = "ERROR SEMANTICO: El tipo de objeto a declar es inaccesible -> " + name4 + " L: " + line4 + " C: " + colum4 + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                }
                            }
                            else
                            {
                                String tipo = nodo.ChildNodes[0].Token.Value.ToString();
                                tmp02 = new Simbolo(cima.Nivel, name4, Cadena.Nulo, tipo, line4, colum4);
                                tmp02.Visibilidad = visibilidad4;
                                cima.insertar(name4, tmp02, claseActual.Nombre);
                            }  
                        }
                        else
                        {
                            String error = "ERROR SEMANTICO: La variable ya se encuentra definida -> " + name4 + " L: " + line4 + " C: " + colum4 + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                        }
                        break;
                        #endregion
//=============================DECLARA VARIABLES CON/SIN ASIGNACION LOCAL===================================================================                    
                    case Cadena.DEC_VAR_2:
                        #region
                        String name5 = nodo.ChildNodes[1].Token.Value.ToString();
                        String line5 = (nodo.ChildNodes[1].Token.Location.Line+1)+"";
                        String colum5 = (nodo.ChildNodes[1].Token.Location.Column-3)+"";
                        Simbolo tmp03=existeVariable3(name5);
                        if(tmp03==null){//la variable no esxiste la puedo crear
                            if (nodo.ChildNodes.Count > 2) //es declaracion con asignacion/ hay que comprobar tipos XD
                            {
                                retorno ret = ejecutarEXP(nodo.ChildNodes[2]);
                                if (!ret.tipo.Equals(Cadena.error))
                                {

                                    String tipo = nodo.ChildNodes[0].Token.Value.ToString();
                                    if (nodo.ChildNodes[0].Term.Name.Equals(Cadena.Id))
                                    {
                                        if (Clases.existeClase(tipo))
                                        {
                                            if (comprobarTipo(tipo, ret))
                                            {
                                                tmp03 = new Simbolo(cima.Nivel, name5, ret.valor, nodo.ChildNodes[0].Token.Value.ToString(), line5, colum5);
                                                tmp03.TipoObjeto = Cadena.Objeto;
                                                cima.insertar(name5, tmp03,claseActual.Nombre);
                                            }
                                            else
                                            {
                                                String error = "ERROR SEMANTICO: Incompatibilidad de tipos en la asignacion de la variable  -> " + name5 + " L: " + line5 + " C: " + colum5 + " Clase: " + claseActual.Nombre;
                                                Form1.listaErrores.Add(error);
                                                Console.WriteLine(error);
                                            }
                                        }
                                        else
                                        {
                                            String error = "ERROR SEMANTICO: El tipo de objeto a declar es inaccesible -> " + name5 + " L: " + line5 + " C: " + colum5+ " Clase: "+claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                        }
                                    }
                                    else
                                    {
                                        if (comprobarTipo(tipo, ret))
                                        {
                                            tmp03 = new Simbolo(cima.Nivel, name5, ret.valor, tipo, line5, colum5);
                                            cima.insertar(name5, tmp03,claseActual.Nombre);
                                        }
                                        else
                                        {
                                            String error = "ERROR SEMANTICO: Incompatibilidad de tipos en la dec/asignacion de la variable  -> " + name5 + " L: " + line5 + " C: " + colum5 + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                        }
                                    }
                                }
                            }
                            else // es solo declaracion 
                            {
                                String tipo = nodo.ChildNodes[0].Token.Value.ToString();
                                if (nodo.ChildNodes[0].Term.Name.Equals(Cadena.Id))
                                {
                                    if (Clases.existeClase(tipo))
                                    {
                                        tmp03 = new Simbolo(cima.Nivel, name5, Cadena.Nulo, tipo, line5, colum5);
                                        tmp03.TipoObjeto = Cadena.Objeto;
                                        cima.insertar(name5, tmp03, claseActual.Nombre);
                                    }
                                    else
                                    {
                                        String error = "ERROR SEMANTICO: El tipo de objeto a declar es inaccesible -> " + name5 + " L: " + line5 + " C: " + colum5 + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                    }
                                }
                                else
                                {
                                    
                                    tmp03 = new Simbolo(cima.Nivel, name5, Cadena.Nulo, tipo, line5, colum5);
                                    cima.insertar(name5, tmp03, claseActual.Nombre);
                                }   
                            }               
                        }else{
                            String error = "ERROR SEMANTICO: La variable ya se encuentra definida -> " + name5 + " L: " + line5 + " C: " + colum5 + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                        }
                        break;
                    #endregion
//=============================DECLARA MAT CON/SIN ASIGNACION LOCAL=========================================================================
                    case Cadena.DEC_ASIGNA_MAT_2:
                        #region
                        String tipo6 = nodo.ChildNodes[0].Token.Value.ToString();
                        String name6 = nodo.ChildNodes[1].Token.Value.ToString();
                        String line6 = (nodo.ChildNodes[1].Token.Location.Line+1)+"";
                        String colum6 = (nodo.ChildNodes[1].Token.Location.Column-3)+"";
                        int dims6 = nodo.ChildNodes[2].ChildNodes.Count;
                        Simbolo tmp04 = existeVariable3(name6); // acá se valida que no exista la variable
                        if (tmp04 == null)
                        {
                            //aca vamos a saber si solo se declara o se asigna la matriz de una 
                            if (nodo.ChildNodes[3].ChildNodes.Count > 1)
                            {
                                String tip = nodo.ChildNodes[3].ChildNodes[1].Token.Value.ToString();
                                if (tip.ToLower().Equals(tipo6.ToLower()))
                                {
                                    int dim2 = nodo.ChildNodes[3].ChildNodes[2].ChildNodes.Count;
                                    if (dims6 == dim2)
                                    {
                                        List<Object> valores = new List<object>();
                                        Simbolo sim = new Simbolo(cima.Nivel, name6, valores, tipo6, line6, colum6);
                                        sim.TipoObjeto = Cadena.Matriz;
                                        sim.iniciaDims();
                                        int tamMat = 1;
                                        foreach (ParseTreeNode hijo in nodo.ChildNodes[3].ChildNodes[2].ChildNodes)
                                        {
                                            sim.dimenciones.Add(Int32.Parse(hijo.Token.Value.ToString()));
                                            tamMat = tamMat * Int32.Parse(hijo.Token.Value.ToString());
                                        }
                                        for (int i = 0; i < tamMat; i++)
                                        {
                                            valores.Add(0);
                                        }
                                        cima.insertar(name6, sim,claseActual.Nombre);
                                    }
                                    else
                                    {
                                        String error = "ERROR SEMANTICO: Dimensiones incorrestas en la dec de la matriz -> " + name6 + " L: " + line6 + " C: " + colum6 + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                    }
                                }
                                else
                                {//error no son del mismo tipo
                                    String error = "ERROR SEMANTICO: Incopatibilidad de tipos en la dec de la matriz -> " + name6 + " L: " + line6 + " C: " + colum6 + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                }
                            }
                            else//aca es con la asignacion con valores incluidos 
                            {
                                List<Object> tmp_val = new List<object>();
                                List<int> tmp_dim = new List<int>();
                                capturarDims(tmp_dim, nodo.ChildNodes[3].ChildNodes[0]);
                                capturarVals(tmp_val, nodo.ChildNodes[3].ChildNodes[0]);
                                if (validarTipos(tmp_val))
                                {
                                    String tip = ((retorno)tmp_val[0]).tipo;
                                    if (tipo6.ToLower().Equals(tip.ToLower())) {
                                        if (dims6 == tmp_dim.Count)
                                        {                                           
                                            List<Object> valores = new List<object>();
                                            Simbolo sim = new Simbolo(cima.Nivel, name6, valores, tipo6, line6, colum6);
                                            sim.TipoObjeto = Cadena.Matriz;
                                            sim.iniciaDims();
                                            for (int i = 0; i < tmp_val.Count; i++)
                                            {
                                                valores.Add(((retorno)tmp_val[i]).valor);
                                            }
                                            for (int i = 0; i < tmp_dim.Count; i++)
                                            {
                                                sim.dimenciones.Add(tmp_dim.ElementAt(i));
                                            }
                                            cima.insertar(name6, sim,claseActual.Nombre);
                                        }
                                        else {
                                            String error = "ERROR SEMANTICO: Dimensiones incorrestas en la dec/asig de la matriz -> " + name6 + " L: " + line6 + " C: " + colum6 + " Clase: " + claseActual.Nombre;
                                            Form1.listaErrores.Add(error);
                                            Console.WriteLine(error);
                                        }
                                    }
                                    else
                                    {
                                        String error = "ERROR SEMANTICO: Incopatibilidad de tipos en la dec/asig de la matriz -> " + name6 + " L: " + line6 + " C: " + colum6 + " Clase: " + claseActual.Nombre;
                                        Form1.listaErrores.Add(error);
                                        Console.WriteLine(error);
                                    }
                                }
                                else {
                                    String error = "ERROR SEMANTICO: Incopatibilidad de tipos en valores a asignar a la matriz -> " + name6 + " L: " + line6 + " C: " + colum6 + " Clase: " + claseActual.Nombre;
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                }
                            }
                        }
                        else {
                            String error = "ERROR SEMANTICO: La variable ya se encuentra definida -> " + name6 + " L: " + line6 + " C: " + colum6 + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                        }
                        #endregion
                        break;
//=============================ASIGNA VARIABLE GLOABAL/LOCAL ===============================================================================
                    case Cadena.ASIGNA:
                        #region
                        String name7 = nodo.ChildNodes[0].Token.Value.ToString();
                        String line7 = (nodo.ChildNodes[0].Token.Location.Line+1)+"";
                        String colum7 = (nodo.ChildNodes[0].Token.Location.Column+1)+"";
                        Simbolo tmp05 = existeVariable2(name7);
                        if(tmp05!=null){
                            retorno ret= ejecutarEXP(nodo.ChildNodes[1]);// vamos a traer el valor de la expresion
                            if (comprobarTipo(tmp05.Tipo, ret) && !(tmp05.TipoObjeto.Equals(Cadena.Matriz)))
                            {
                                tmp05.Valor = ret.valor;
                                //Console.WriteLine("Asignacion de variable:  "+tmp05.Nombre+" realizada.");
                            /*}else if(comprobarTipo(tmp05.TipoObjeto,ret) && !(tmp05.TipoObjeto.Equals(Cadena.Matriz))){
                                tmp05.Valor = ret.valor;
                                //Console.WriteLine("Asignacion de variable:  "+tmp05.Nombre+" realizada.");*/
                            }else{//error incompatibilidad de tipos 
                                String error = "ERROR SEMANTICO: Incompatibilidad de tipos en la asignacion de la variable  -> " + name7 + " L: " + line7 + " C: " + colum7 + " Clase: " + claseActual.Nombre;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);  
                            }                            
                        }else{//error, variable no declarada
                            String error = "ERROR SEMANTICO: La variable no ha sido definida -> " + name7 + " L: " + line7 + " C: " + colum7 + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                        }
                        #endregion
                        break;
                    default:
                        break;
                }    
            }
        }

        public void capturarFunciones(ParseTreeNode nodo){
            foreach (ParseTreeNode hijo in nodo.ChildNodes)
            {
                switch (hijo.Term.Name){          
                    case Cadena.DEC_FUN:
                        agregarFuncion(hijo);
                        break;
                    case Cadena.DEC_MET:
                        agregarFuncion(hijo);
                        break;
                    case Cadena.CONSTRUCT:
                        agregarConstructor(hijo);
                        break;
                    case Cadena.PREGUNTA:
                        agregarPregunta(hijo);
                        break;
                    case Cadena.FORMULARIO:
                        agregarFormulario(hijo);
                        break;
                    case Cadena.GRUPO:
                        agregarGrupo(hijo);
                        break;
                }
            }
        }

        public void capturarClases(ParseTreeNode nodo) {
            if (nodo != null) {
                foreach (ParseTreeNode hijo in nodo.ChildNodes[1].ChildNodes)
                {
                    String nomClase = hijo.ChildNodes[0].Token.Value.ToString();
                    if (!Clases.existeClase(nomClase))
                    {
                        String vis = "publico";
                        if (hijo.ChildNodes[1].ChildNodes.Count > 0)
                        {
                            vis = hijo.ChildNodes[1].ChildNodes[0].Token.Value.ToString();
                        }
                        String padre = "null";
                        if (hijo.ChildNodes[2].ChildNodes.Count > 0)
                        {
                            padre = hijo.ChildNodes[2].ChildNodes[0].Token.Value.ToString();
                        }
                        ParseTreeNode her = hijo.ChildNodes[2];
                        ParseTreeNode cuerpo = hijo.ChildNodes[3];
                        //Añadimos la clase a la lista
                        Clase clas = new Clase(nomClase, vis, padre, her, cuerpo);
                        Clases.insertar(clas);
                    }
                    else
                    {
                        Console.WriteLine("Se omitio la clase -> " + nomClase + " por estar duplicada.");
                        String error = "ERROR SEMANTICO: Se omitio la clase -> " + nomClase + " por estar duplicada.";
                        Form1.listaErrores.Add(error);
                    }
                }
            }
        }

        public void capturarClaseImport(ParseTreeNode nodo) {
            if (nodo != null) {
                foreach (ParseTreeNode hijo in nodo.ChildNodes[0].ChildNodes)
                {
                    String nombreArch = hijo.Token.Value.ToString();
                    String ruta = Form1.ruta_genesis + @"\" + nombreArch;
                    if (System.IO.File.Exists(ruta))
                    {
                        Sintactico analisis = new Sintactico(nombreArch);
                        System.IO.StreamReader sr = new System.IO.StreamReader(ruta);
                        ParseTreeNode raiz = analisis.analizar(sr.ReadToEnd());
                        sr.Close();

                        if (raiz != null)
                        {
                            capturarClases(raiz);
                            capturarClaseImport(raiz);
                        }
                        else
                        {
                            Console.WriteLine("Se econtraron errores en el archivo -> " + nombreArch + "; No se importo.");
                        }
                    }
                    else
                    {
                        String error = "ERROR SEMANTICO: No se pudo importar el archivo -> " + nombreArch + "; No econtrado.";
                        Form1.listaErrores.Add(error);
                    }
                }
            } 
        }

        public void capturarClasePrincipal() {
            Clase principal = Clases.retornaClasePrincipal();
            vfoActual.clase = principal;
            claseActual = principal; // seteamos la clase actual, por aqueo de la herencia
            if (principal != null) 
            {
                if (!principal.Padre.Equals("null")) {
                    herencia(principal.Padre,principal);
                }
                //aca puedo enviar el nodo cuerpo para que empeice la ejecucion de Principal
                capturarFunciones(principal.Cuerpo);
                foreach (ParseTreeNode hijo in principal.Cuerpo.ChildNodes)
                {
                    capturarDecAsigs(hijo);
                }
                ejecutarPrincipal(principal.Cuerpo);
            }
            else
            {
                String error = "ERROR SEMANTICO: No se encontro el metdo PRINCIPAL en las clases analizadas.";
                Form1.listaErrores.Add(error);
                Console.WriteLine(error);
            }
        }

        public void herencia(String padre,Clase hija) {

            Clase clasPadre = Clases.retornaClase(padre);

            if (clasPadre != null)
            {
                heredar(clasPadre, hija);
            }
            else // la clase no esta en los archivos analiados por lo que la buscamos en el archivo, externo
            {
                String ruta = Form1.ruta_genesis + @"\" + padre+".xform";
                if (System.IO.File.Exists(ruta))
                {
                    Sintactico analisis = new Sintactico(padre+".xform");
                    System.IO.StreamReader sr = new System.IO.StreamReader(ruta);
                    ParseTreeNode raiz = analisis.analizar(sr.ReadToEnd());
                    sr.Close();
                    if (raiz != null)
                    {
                        Boolean encontrado = false ;
                        foreach (ParseTreeNode hijo in raiz.ChildNodes[1].ChildNodes)
                        {
                            String nomClase = hijo.ChildNodes[0].Token.Value.ToString();
                            if (nomClase.ToLower().Equals(padre.ToLower()))
                            {
                                if(!Clases.existeClase(padre)){
                                    String vis = "publico";
                                    if (hijo.ChildNodes[1].ChildNodes.Count > 0)
                                    {
                                        vis = hijo.ChildNodes[1].ChildNodes[0].Token.Value.ToString();
                                    }
                                    String padre2 = "null";
                                    if (hijo.ChildNodes[2].ChildNodes.Count > 0)
                                    {
                                        padre2 = hijo.ChildNodes[1].ChildNodes[0].Token.Value.ToString();
                                    }
                                    ParseTreeNode her = hijo.ChildNodes[2];
                                    ParseTreeNode cuerpo = hijo.ChildNodes[3];
                                    //Añadimos la clase a la lista
                                    Clase clas = new Clase(nomClase, vis, padre2,her,cuerpo);
                                    Clases.insertar(clas);
                                    capturarClaseImport(raiz);
                                    heredar(clas, hija);
                                    encontrado = true;
                                    break;
                                }else{//crep que el codigo del else esta de más, pero lo dejare XS
                                    Clase clas = Clases.retornaClase(padre);
                                    capturarClaseImport(raiz);
                                    heredar(clas, hija);
                                    encontrado = true;
                                }
                            }
                        }
                        if (!encontrado) {// no se realizo la herencia
                            String error = "ERROR SEMANTICO: No se pudo heredar  la clase -> " + padre + "; No econtrada.";
                            Form1.listaErrores.Add(error);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Se econtraron errores en el archivo -> " + padre+ ".xform; No se importo.");
                    }
                }
                else
                {
                    String error = "ERROR SEMANTICO: No se pudo importar el archivo -> " + padre+ ".xform; No econtrado.";
                    Form1.listaErrores.Add(error);
                }
            }

        }

        public void heredar(Clase padre, Clase hija) {

            foreach (ParseTreeNode nodo in padre.Cuerpo.ChildNodes)
            {
                switch (nodo.Term.Name)
                {

                    case Cadena.CONSTRUCT:
                        hija.Her.ChildNodes.Add(nodo);
                        break;
                    case Cadena.DEC_FUN:
                        if (nodo.ChildNodes[0].ChildNodes.Count > 0)
                        {
                            String vis = nodo.ChildNodes[0].ChildNodes[0].Token.Value.ToString();
                            if (!vis.ToLower().Equals("privado")) {
                                hija.Cuerpo.ChildNodes.Add(nodo);
                            }
                        }
                        else {
                            hija.Cuerpo.ChildNodes.Add(nodo);
                        }
                        break;
                    case Cadena.DEC_MET:
                        if (nodo.ChildNodes[0].ChildNodes.Count > 0)
                        {
                            String vis = nodo.ChildNodes[0].ChildNodes[0].Token.Value.ToString();
                            if (!vis.ToLower().Equals("privado"))
                            {
                                hija.Cuerpo.ChildNodes.Add(nodo);
                            }
                        }
                        else
                        {
                            hija.Cuerpo.ChildNodes.Add(nodo);
                        }
                        break;
                    case Cadena.DEC_ASIGNA_MAT:
                        if (nodo.ChildNodes[1].ChildNodes.Count > 0)
                        {
                            String vis = nodo.ChildNodes[1].ChildNodes[0].Token.Value.ToString();
                            if (!vis.ToLower().Equals("privado"))
                            {
                                hija.Cuerpo.ChildNodes.Add(nodo);
                            }
                        }
                        else
                        {
                            hija.Cuerpo.ChildNodes.Add(nodo);
                        }
                        break;
                    case Cadena.DEC_ASIGNA_VAR:
                        if (nodo.ChildNodes[1].ChildNodes.Count > 0)
                        {
                            String vis = nodo.ChildNodes[1].ChildNodes[0].Token.Value.ToString();
                            if (!vis.ToLower().Equals("privado"))
                            {
                                hija.Cuerpo.ChildNodes.Add(nodo);
                            }
                        }
                        else
                        {
                            hija.Cuerpo.ChildNodes.Add(nodo);
                        }
                        break;
                    case Cadena.ASIGNA_MAT:
                        hija.Cuerpo.ChildNodes.Add(nodo);
                        break;
                    case Cadena.ASIGNA:
                        hija.Cuerpo.ChildNodes.Add(nodo);
                        break;
                    case Cadena.DEC_VAR:
                        if (nodo.ChildNodes[1].ChildNodes.Count > 0)
                        {
                            String vis = nodo.ChildNodes[1].ChildNodes[0].Token.Value.ToString();
                            if (!vis.ToLower().Equals("privado"))
                            {
                                hija.Cuerpo.ChildNodes.Add(nodo);
                            }
                        }
                        else
                        {
                            hija.Cuerpo.ChildNodes.Add(nodo);
                        }
                        break;
                    default:
                        break;
                }
            }
            //ParseTreeNode Constructores = new ParseTreeNode();
        
        }

        private void agregarFuncion(ParseTreeNode hijo) {
            String visibilidad;
            String tipo;
            String nombre;
            String parametros;
            String key;

            visibilidad = "publico";
            if (hijo.ChildNodes[0].ChildNodes.Count > 0)
            {
                visibilidad = hijo.ChildNodes[0].ChildNodes[0].Token.Value.ToString();
            }
            //sacamos el tipo
            tipo = hijo.ChildNodes[1].Token.Value.ToString();
            //sacamos el nombre 
            nombre = hijo.ChildNodes[2].Token.Value.ToString();
            //sacamos los parametros
            parametros = "";
            if (hijo.ChildNodes[3].ChildNodes.Count > 0)
            {
                String tipoPar;
                foreach (ParseTreeNode subHijo in hijo.ChildNodes[3].ChildNodes)
                {
                    tipoPar = subHijo.ChildNodes[0].Token.Value.ToString();
                    parametros += tipoPar.ToLower();
                }
            }
            //armamos la llave de la funcion
            key = nombre.ToLower() + "_" + parametros;
            if (!Funciones.existeFuncion(key))
            {
                //metodo o funcion con parametros
                if (hijo.ChildNodes[3].ChildNodes.Count > 0)
                {
                    List<String> pars = new List<string>();
                    //comprobamos que no tenga parametros repetidos
                    foreach (ParseTreeNode subHijo in hijo.ChildNodes[3].ChildNodes)
                    {
                        if (!pars.Contains(subHijo.ChildNodes[1].Token.Value.ToString()))
                        {
                            pars.Add(subHijo.ChildNodes[1].Token.Value.ToString());
                        }
                        else
                        {//error nombre de parametro repetido
                            String error = "ERROR SEMANTICO: No se agrego funcion -> " + nombre + " por parametro repetido:  L: " + subHijo.ChildNodes[1].Token.Location.Line + " C: " + subHijo.ChildNodes[1].Token.Location.Column + " Clase: "+claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return;
                            //break;
                        }
                    }
                    // no hay parametros repetidos, creamos la funcion
                    Funcion fun = new Funcion(visibilidad, tipo, nombre, hijo.ChildNodes[4]);
                    foreach (ParseTreeNode subHijo in hijo.ChildNodes[3].ChildNodes)
                    {
                        fun.addParametro(subHijo.ChildNodes[1].Token.Value.ToString(), subHijo.ChildNodes[0].Token.Value.ToString());
                    }
                    Funciones.insertar(key, fun,claseActual.Nombre);
                }
                else  //metodo o funcion sin parametros
                {
                    Funcion fun = new Funcion(visibilidad, tipo, nombre, hijo.ChildNodes[4]);
                    Funciones.insertar(key, fun,claseActual.Nombre);
                }
            }
            else
            {//error la funcion ya fue insertada
                String error = "ERROR SEMANTICO: No se agrego funcion -> " + nombre + " por que ya fue definida:  L: " + hijo.ChildNodes[1].Token.Location.Line + " C: " + hijo.ChildNodes[1].Token.Location.Column + " Clase: " + claseActual.Nombre;
                Form1.listaErrores.Add(error);
                Console.WriteLine(error);
            }
        }

        private void agregarConstructor(ParseTreeNode hijo)
        {
            String visibilidad="publico";
            String tipo="constructor";
            String nombre;
            String line;
            String column;
            String parametros="";
            String key;
            //cuando viene la palabra publico en el constructors
            if(hijo.ChildNodes.Count>3){
                nombre=hijo.ChildNodes[1].Token.Value.ToString();
                line = (hijo.ChildNodes[1].Token.Location.Line+1)+"";
                column= (hijo.ChildNodes[1].Token.Location.Column+1)+"";
                String tipoPar;
                foreach (ParseTreeNode subHijo in hijo.ChildNodes[2].ChildNodes)
                {
                    tipoPar = subHijo.ChildNodes[0].Token.Value.ToString();
                    parametros += tipoPar.ToLower();
                }
                if (nombre.ToLower().Equals(claseActual.Nombre.ToLower()))
                {
                    //armamos la llave de la funcion
                    key = nombre.ToLower() + "_" + parametros;
                    if (!Funciones.existeFuncion(key))
                    {
                        //metodo o funcion con parametros
                        if (hijo.ChildNodes[2].ChildNodes.Count > 0)
                        {
                            List<String> pars = new List<string>();
                            //comprobamos que no tenga parametros repetidos
                            foreach (ParseTreeNode subHijo in hijo.ChildNodes[2].ChildNodes)
                            {
                                if (!pars.Contains(subHijo.ChildNodes[1].Token.Value.ToString()))
                                {
                                    pars.Add(subHijo.ChildNodes[1].Token.Value.ToString());
                                }
                                else
                                {//error nombre de parametro repetido
                                    String error = "ERROR SEMANTICO: No se agrego constructor de la clase -> " + claseActual.Nombre + " por parametro repetido: ->" + subHijo.ChildNodes[1].Token.Value.ToString() + " L: " + (subHijo.ChildNodes[1].Token.Location.Line+1) + " C: " + (subHijo.ChildNodes[1].Token.Location.Column+1);
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return;
                                    //break;
                                }
                            }
                            // no hay parametros repetidos, creamos el constructor
                            Funcion fun = new Funcion(visibilidad, tipo, nombre, hijo.ChildNodes[3]);
                            foreach (ParseTreeNode subHijo in hijo.ChildNodes[2].ChildNodes)
                            {
                                fun.addParametro(subHijo.ChildNodes[1].Token.Value.ToString(), subHijo.ChildNodes[0].Token.Value.ToString());
                            }
                            Funciones.insertar(key, fun,claseActual.Nombre);
                        }
                        else  //constructor sin parametros
                        {
                            Funcion fun = new Funcion(visibilidad, tipo, nombre, hijo.ChildNodes[3]);
                            Funciones.insertar(key, fun,claseActual.Nombre);
                        }
                    }
                    else
                    {//error el constructor ya se inserto
                        String error = "ERROR SEMANTICO: No se agrego el constructor de la clase -> " + claseActual.Nombre +" por que ya fue definido:  L: " + line + " C: " + column;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return;
                    }
                }
                //el constructor no tiene el mismo nombre que la clase
                else {
                    String error = "ERROR SEMANTICO: No se agrego el construcotor de la clase -> " + claseActual.Nombre + ", no tiene el mismo nombre que la clase:  L: " + line + " C: " +column ;
                    Form1.listaErrores.Add(error);
                    Console.WriteLine(error);
                    return;
                }
            //cunado el constructor no trae la palabra publico
            }else{
                nombre=hijo.ChildNodes[0].Token.Value.ToString();
                line = (hijo.ChildNodes[0].Token.Location.Line+1)+"";
                column= (hijo.ChildNodes[0].Token.Location.Column+1)+"";
                String tipoPar;
                foreach (ParseTreeNode subHijo in hijo.ChildNodes[1].ChildNodes)
                {
                    tipoPar = subHijo.ChildNodes[0].Token.Value.ToString();
                    parametros += tipoPar.ToLower();
                }
                if (nombre.ToLower().Equals(claseActual.Nombre.ToLower()))
                {
                    //armamos la llave de la funcion
                    key = nombre.ToLower() + "_" + parametros;
                    if (!Funciones.existeFuncion(key))
                    {
                        //metodo o funcion con parametros
                        if (hijo.ChildNodes[1].ChildNodes.Count > 0)
                        {
                            List<String> pars = new List<string>();
                            //comprobamos que no tenga parametros repetidos
                            foreach (ParseTreeNode subHijo in hijo.ChildNodes[1].ChildNodes)
                            {
                                if (!pars.Contains(subHijo.ChildNodes[1].Token.Value.ToString()))
                                {
                                    pars.Add(subHijo.ChildNodes[1].Token.Value.ToString());
                                }
                                else
                                {//error nombre de parametro repetido
                                    String error = "ERROR SEMANTICO: No se agrego constructor de la clase -> " + claseActual.Nombre + " por parametro repetido: ->" + subHijo.ChildNodes[1].Token.Value.ToString() + " L: " + (subHijo.ChildNodes[1].Token.Location.Line + 1) + " C: " + (subHijo.ChildNodes[1].Token.Location.Column + 1);
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return;
                                    //break;
                                }
                            }
                            // no hay parametros repetidos, creamos la funcion
                            Funcion fun = new Funcion(visibilidad, tipo, nombre, hijo.ChildNodes[2]);
                            foreach (ParseTreeNode subHijo in hijo.ChildNodes[1].ChildNodes)
                            {
                                fun.addParametro(subHijo.ChildNodes[1].Token.Value.ToString(), subHijo.ChildNodes[0].Token.Value.ToString());
                            }
                            Funciones.insertar(key, fun,claseActual.Nombre);
                        }
                        else  //constructor sin parametros
                        {
                            Funcion fun = new Funcion(visibilidad, tipo, nombre, hijo.ChildNodes[2]);
                            Funciones.insertar(key, fun,claseActual.Nombre);
                        }
                    }
                    else
                    {//error el constructor ya se inserto
                        String error = "ERROR SEMANTICO: No se agrego el constructor de la clase -> " + claseActual.Nombre + " por que ya fue definido:  L: " + line + " C: " + column;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return;
                    }
                }
                //el constructor no tiene el mismo nombre que la clase
                else
                {
                    String error = "ERROR SEMANTICO: No se agrego el construcotor de la clase -> " + claseActual.Nombre + ", no tiene el mismo nombre que la clase:  L: " + line + " C: " + column;
                    Form1.listaErrores.Add(error);
                    Console.WriteLine(error);
                    return;
                }
            }
        }

        private void agregarConstructor2(ParseTreeNode hijo)
        {
            String visibilidad = "publico";
            String tipo = "constructor";
            String nombre;
            String line;
            String column;
            String parametros = "";
            String key;
            //cuando viene la palabra publico en el constructors
            if (hijo.ChildNodes.Count > 3)
            {
                nombre = hijo.ChildNodes[1].Token.Value.ToString();
                line = (hijo.ChildNodes[1].Token.Location.Line + 1) + "";
                column = (hijo.ChildNodes[1].Token.Location.Column + 1) + "";
                String tipoPar;
                foreach (ParseTreeNode subHijo in hijo.ChildNodes[2].ChildNodes)
                {
                    tipoPar = subHijo.ChildNodes[0].Token.Value.ToString();
                    parametros += tipoPar.ToLower();
                }

                    //armamos la llave de la funcion
                    key = nombre.ToLower() + "_" + parametros;
                    if (!Funciones.existeFuncion(key))
                    {
                        //metodo o funcion con parametros
                        if (hijo.ChildNodes[2].ChildNodes.Count > 0)
                        {
                            List<String> pars = new List<string>();
                            //comprobamos que no tenga parametros repetidos
                            foreach (ParseTreeNode subHijo in hijo.ChildNodes[2].ChildNodes)
                            {
                                if (!pars.Contains(subHijo.ChildNodes[1].Token.Value.ToString()))
                                {
                                    pars.Add(subHijo.ChildNodes[1].Token.Value.ToString());
                                }
                                else
                                {//error nombre de parametro repetido
                                    String error = "ERROR SEMANTICO: No se agrego constructor de la clase Padre  -> SUPER"  + " por parametro repetido: ->" + subHijo.ChildNodes[1].Token.Value.ToString() + " L: " + (subHijo.ChildNodes[1].Token.Location.Line + 1) + " C: " + (subHijo.ChildNodes[1].Token.Location.Column + 1);
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return;
                                    //break;
                                }
                            }
                            // no hay parametros repetidos, creamos el constructor
                            Funcion fun = new Funcion(visibilidad, tipo, nombre, hijo.ChildNodes[3]);
                            foreach (ParseTreeNode subHijo in hijo.ChildNodes[2].ChildNodes)
                            {
                                fun.addParametro(subHijo.ChildNodes[1].Token.Value.ToString(), subHijo.ChildNodes[0].Token.Value.ToString());
                            }
                            Funciones.insertar(key, fun, claseActual.Nombre);
                        }
                        else  //constructor sin parametros
                        {
                            Funcion fun = new Funcion(visibilidad, tipo, nombre, hijo.ChildNodes[3]);
                            Funciones.insertar(key, fun, claseActual.Nombre);
                        }
                    }
                    else
                    {//error el constructor ya se inserto
                        String error = "ERROR SEMANTICO: No se agrego el constructor de la clase padre -> SUPER"  + " por que ya fue definido:  L: " + line + " C: " + column;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return;
                    }
            }
            else
            {
                nombre = hijo.ChildNodes[0].Token.Value.ToString();
                line = (hijo.ChildNodes[0].Token.Location.Line + 1) + "";
                column = (hijo.ChildNodes[0].Token.Location.Column + 1) + "";
                String tipoPar;
                foreach (ParseTreeNode subHijo in hijo.ChildNodes[1].ChildNodes)
                {
                    tipoPar = subHijo.ChildNodes[0].Token.Value.ToString();
                    parametros += tipoPar.ToLower();
                }

                    //armamos la llave de la funcion
                    key = nombre.ToLower() + "_" + parametros;
                    if (!Funciones.existeFuncion(key))
                    {
                        //metodo o funcion con parametros
                        if (hijo.ChildNodes[1].ChildNodes.Count > 0)
                        {
                            List<String> pars = new List<string>();
                            //comprobamos que no tenga parametros repetidos
                            foreach (ParseTreeNode subHijo in hijo.ChildNodes[1].ChildNodes)
                            {
                                if (!pars.Contains(subHijo.ChildNodes[1].Token.Value.ToString()))
                                {
                                    pars.Add(subHijo.ChildNodes[1].Token.Value.ToString());
                                }
                                else
                                {//error nombre de parametro repetido
                                    String error = "ERROR SEMANTICO: No se agrego constructor de la clase -> " + claseActual.Nombre + " por parametro repetido: ->" + subHijo.ChildNodes[1].Token.Value.ToString() + " L: " + (subHijo.ChildNodes[1].Token.Location.Line + 1) + " C: " + (subHijo.ChildNodes[1].Token.Location.Column + 1);
                                    Form1.listaErrores.Add(error);
                                    Console.WriteLine(error);
                                    return;
                                    //break;
                                }
                            }
                            // no hay parametros repetidos, creamos la funcion
                            Funcion fun = new Funcion(visibilidad, tipo, nombre, hijo.ChildNodes[2]);
                            foreach (ParseTreeNode subHijo in hijo.ChildNodes[1].ChildNodes)
                            {
                                fun.addParametro(subHijo.ChildNodes[1].Token.Value.ToString(), subHijo.ChildNodes[0].Token.Value.ToString());
                            }
                            Funciones.insertar(key, fun, claseActual.Nombre);
                        }
                        else  //constructor sin parametros
                        {
                            Funcion fun = new Funcion(visibilidad, tipo, nombre, hijo.ChildNodes[2]);
                            Funciones.insertar(key, fun, claseActual.Nombre);
                        }
                    }
                    else
                    {//error el constructor ya se inserto
                        String error = "ERROR SEMANTICO: No se agrego el constructor de la clase -> " + claseActual.Nombre + " por que ya fue definido:  L: " + line + " C: " + column;
                        Form1.listaErrores.Add(error);
                        Console.WriteLine(error);
                        return;
                    }
            }
        }

        private void agregarPregunta(ParseTreeNode hijo)
        {
            String nombre;
            String key;
            //sacamos el tipo
            //sacamos el nombre 
            nombre = hijo.ChildNodes[1].Token.Value.ToString();
            //armamos la llave de la funcion
            key = nombre.ToLower();
            if (!Preguntas.existePregunta(key))
            {
                //metodo o funcion con parametros
                if (hijo.ChildNodes[2].ChildNodes.Count > 0)
                {
                    List<String> pars = new List<string>();
                    //comprobamos que no tenga parametros repetidos
                    foreach (ParseTreeNode subHijo in hijo.ChildNodes[2].ChildNodes)
                    {
                        if (!pars.Contains(subHijo.ChildNodes[1].Token.Value.ToString()))
                        {
                            pars.Add(subHijo.ChildNodes[1].Token.Value.ToString());
                        }
                        else
                        {//error nombre de parametro repetido
                            String error = "ERROR SEMANTICO: No se agrego la PREGUNTA -> " + nombre + " por parametro repetido:  L: " + subHijo.ChildNodes[1].Token.Location.Line + " C: " + subHijo.ChildNodes[1].Token.Location.Column + " Clase: " + claseActual.Nombre;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return;
                            //break;
                        }
                    }
                    // no hay parametros repetidos, creamos la funcion
                    Pregunta preg = new Pregunta(key, hijo.ChildNodes[3]);
                    foreach (ParseTreeNode subHijo in hijo.ChildNodes[2].ChildNodes)
                    {
                        preg.addParametro(subHijo.ChildNodes[1].Token.Value.ToString(), subHijo.ChildNodes[0].Token.Value.ToString());
                    }
                    Preguntas.insertar(key, preg, claseActual.Nombre);
                }
                else  //metodo o funcion sin parametros
                {
                    Pregunta preg = new Pregunta(key, hijo.ChildNodes[3]);
                    Preguntas.insertar(key, preg, claseActual.Nombre);
                }
            }
            else
            {//error la funcion ya fue insertada
                String error = "ERROR SEMANTICO: No se agrego la PREGUNTA -> " + nombre + " por que ya fue definida:  L: " + hijo.ChildNodes[1].Token.Location.Line + " C: " + hijo.ChildNodes[1].Token.Location.Column + " Clase: " + claseActual.Nombre;
                Form1.listaErrores.Add(error);
                Console.WriteLine(error);
            }   
        }

        private void agregarFormulario(ParseTreeNode hijo) {
            String nombre = hijo.ChildNodes[1].Token.Value.ToString();
            String line = (hijo.ChildNodes[1].Token.Location.Line+1)+"";
            String column = (hijo.ChildNodes[1].Token.Location.Column + 1) + ""; 
            if (Formulario == null)
            {
                Formulario = new Formulario(nombre, hijo.ChildNodes[2]);
            }
            else {
                String error = "ERROR SEMANTICO: Ya se encuntra definido en formulario -> " +nombre+ " L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                Form1.listaErrores.Add(error);
                Console.WriteLine(error);
            }
        }

        private void agregarGrupo(ParseTreeNode hijo) {            
            String tipo;
            String nombre;
            String parametros;
            String key;
            //sacamos el tipo
            tipo = "grupo";
            //sacamos el nombre 
            nombre = hijo.ChildNodes[1].Token.Value.ToString();
            String line= (hijo.ChildNodes[1].Token.Location.Line+1)+"";
            String column = (hijo.ChildNodes[1].Token.Location.Column + 1) + "";
            //sacamos los parametros
            parametros = "";
            //armamos la llave de la funcion
            key = nombre.ToLower() + "_" + parametros;
            if (!Funciones.existeFuncion(key))
            {
                Funcion fun = new Funcion(Cadena.Publico, tipo, nombre, hijo.ChildNodes[2]);
                Funciones.insertar(key, fun, claseActual.Nombre);                    
            }
            else
            {//error la funcion ya fue insertada
                String error = "ERROR SEMANTICO: No se agrego el grupo -> " + nombre + " por que ya fue definido:  L: " + line + " C: " + column + " Clase: " + claseActual.Nombre;
                Form1.listaErrores.Add(error);
                Console.WriteLine(error);
            }        
        }
        public void ejecutarPrincipal(ParseTreeNode cuerpo){
            Clase principal = Clases.retornaClasePrincipal();
            foreach (ParseTreeNode hijo in cuerpo.ChildNodes)
            {
                if (hijo.Term.Name.Equals(Cadena.PRINCIPAL))
                {
                    String ambito = cima.Nivel + Cadena.fun + "principal";
                    TablaSimbolos tab = new TablaSimbolos(ambito, Cadena.ambito_fun, true, false, false);
                    pilaSimbols.Push(tab); //agregamos la nueva tabla de simbolos a la pila
                    cima = tab; // la colocamos en la cima
                    //aca ejecuta las sentncias del metodo principal
                    foreach (ParseTreeNode sub_hijo in hijo.ChildNodes[1].ChildNodes)
                    {
                        retorno reto2 = ejecutar(sub_hijo);                     
                        if (reto2.retorna)
                        {
                            if (!reto2.tipo.Equals(Cadena.Vacio))
                            {
                                pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                                cima = pilaSimbols.Peek();
                                String error = "ERROR SEMANTICO: No puede retornar expresiones en el metodo PRINCIPAL -> EXP" + " L: " + reto2.Linea + " C: " + reto2.Columna;
                                Form1.listaErrores.Add(error);
                                Console.WriteLine(error);
                            }

                            return;
                        }
                        if (reto2.detener)
                        { //detener fuera de ciclos, error semantico =
                            pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                            cima = pilaSimbols.Peek();
                            String error = "ERROR SEMANTICO: Sentencia romper invalida, fuera de ciclos -> ROMPER " + " L: " + reto2.Linea + " C: " + reto2.Columna;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return;
                        }
                        if (reto2.continua)
                        {
                            pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                            cima = pilaSimbols.Peek();
                            String error = "ERROR SEMANTICO: Sentencia continuar invalida, fuera de ciclos -> CONTINUAR " + " L: " + reto2.Columna + " C: " + reto2.Columna;
                            Form1.listaErrores.Add(error);
                            Console.WriteLine(error);
                            return;
                        }
                    }
                    pilaSimbols.Pop(); //sacamos la tabla de simbolos que se inserto
                    cima = pilaSimbols.Peek();
                    return;
                }
            }

        }

        private Simbolo existeVariable(String id) //la pila aca se comporta haciendo push al lemeto 0 y no al ultimo
        { //metodo para obtener el valor de las variables cuando las llama una expresion por ende busca tanto en la gloabl como en las locales

            if (cima.Nivel.Equals("Global"))
            {
                if (cima.existeSimbolo(id))
                    return Global.retornaSimbolo(id);

                if (temporal.Count>0) {
                    if (temporal.Peek().existeSimbolo(id))
                        return Padre.retornaSimbolo(id);
                }    
            }
            else
            {
                for (int i = 0; i < pilaSimbols.Count; i++)//y no llega hasata la global
                {
                    if (pilaSimbols.ElementAt(i).existeSimbolo(id))
                        return pilaSimbols.ElementAt(i).retornaSimbolo(id);
                }

                if (temporal.Count > 0)
                {
                    if (temporal.Peek().existeSimbolo(id))
                        return Padre.retornaSimbolo(id);
                }
            }

            return null;
        }

        private Simbolo existeVariable2(String id)
        { // para acceso a variables ->  locales y globales
            String nombre = cima.Nivel;
            for (int j = 0; j < pilaSimbols.Count - 1; j++)
            {
                if (pilaSimbols.ElementAt(j).Nivel.Equals(nombre))
                {
                    if (pilaSimbols.ElementAt(j).existeSimbolo(id))
                    {
                        return pilaSimbols.ElementAt(j).retornaSimbolo(id);
                    }
                }
            }
            return Global.retornaSimbolo(id);
        }

        private Simbolo existeVariable3(String id) //declara local
        { //metodo para obtener el valor de las variables
            String nombre = cima.Nivel;
            for (int j = 0; j < pilaSimbols.Count - 1; j++)
            {
                if (pilaSimbols.ElementAt(j).Nivel.Equals(nombre))
                {
                    if (pilaSimbols.ElementAt(j).existeSimbolo(id))
                    {
                        return pilaSimbols.ElementAt(j).retornaSimbolo(id);
                    }
                }
            }
            return null;
        }

        private Simbolo existeVariable4(String id) //declara o asigna gloabal
        { //metodo para obtener el valor de las variables
            return Global.retornaSimbolo(id);
        }

        private bool comprobarTipo(String tipoPadre, retorno ret) {

            switch (tipoPadre.ToLower())
            {
                case "entero":
                    if (ret.tipo.ToLower().Equals("entero")) {
                        return true;
                    }
                    return false;
                case "decimal":
                     if (ret.tipo.ToLower().Equals("decimal")) {
                        return true;
                    }
                    return false;
                case "cadena":
                     if (ret.tipo.ToLower().Equals("cadena")) {
                        return true;
                    }
                    return false;
                case "booleano":
                     if (ret.tipo.ToLower().Equals("booleano")) {
                        return true;
                    }
                    return false;
                case "fecha":
                     if (ret.tipo.ToLower().Equals("fecha")) {
                        return true;
                    }
                    return false;
                case "hora":
                    if (ret.tipo.ToLower().Equals("hora")) {
                        return true;
                    }
                    return false;
                case "fechahora":
                     if (ret.tipo.ToLower().Equals("fechahora")) {
                        return true;
                    }
                    return false;
                default://tipo objeto
                     if (ret.tipo.ToLower().Equals(tipoPadre.ToLower()) || ret.tipo.ToLower().Equals("nulo")) {
                        return true;
                    }
                    return false;
            }
        }

        private void capturarDims(List<int> dims, ParseTreeNode nodo){
            if (nodo.Term.Name.Equals(Cadena.L_EXP)) {
                dims.Add(nodo.ChildNodes.Count);
                return;
            }else{
                dims.Add(nodo.ChildNodes.Count);
                capturarDims(dims, nodo.ChildNodes[0]);
            }
        }

        private void capturarVals(List<object> vals, ParseTreeNode nodo) {
            switch (nodo.Term.Name) { 
                case Cadena.L_AS:
                    foreach (ParseTreeNode hijo in nodo.ChildNodes)
                    {
                        capturarVals(vals, hijo);
                    }
                    break;
                case Cadena.L_EXP:
                    foreach (ParseTreeNode hijo in nodo.ChildNodes)
                    {
                        capturarVals(vals, hijo);
                    }
                    break;
                case Cadena.DIM3:
                    foreach (ParseTreeNode hijo in nodo.ChildNodes)
                    {
                        capturarVals(vals, hijo);
                    }
                    break;
                case Cadena.LOG:
                    retorno ret = ejecutarEXP(nodo);
                    vals.Add(ret);
                    break;
            }

        }

        private bool validarTipos(List<Object> vals) {
            String tipo_base="";
            String tipo_sig;
            for (int i = 0; i < vals.Count; i++)
            {
                if (i == 0)
                {
                    retorno ret = (retorno)vals[i];
                    tipo_base = ret.tipo;
                }
                else {
                    retorno ret = (retorno)vals[i];
                    tipo_sig=ret.tipo;
                    if (!tipo_base.ToLower().Equals(tipo_sig.ToLower())) {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool validarIndices(List<object> indices, List<int> dims) {
            foreach (object item in indices)
            {
                retorno ret = (retorno)item;
                if (!ret.tipo.ToLower().Equals("entero")) {
                    String error = "ERROR SEMANTICO: El indice no es de tipo entero -> " + ret.valor.ToString() + " L: " + ret.Linea + " C: " + ret.Columna + " Clase: " + claseActual.Nombre;
                    Form1.listaErrores.Add(error);
                    Console.WriteLine(error);
                    return false;
                }
            }
            for (int i = 0; i < dims.Count; i++)
            {
                retorno ret = (retorno)indices.ElementAt(i);
                int indice = Int32.Parse(ret.valor.ToString());
                if ( !(indice > -1 && indice < dims.ElementAt(i))){
                    String error = "ERROR SEMANTICO: El indice esta fuera del rango -> " + ret.valor.ToString() + " L: " + ret.Linea + " C: " + ret.Columna + " Clase: " + claseActual.Nombre;
                    Form1.listaErrores.Add(error);
                    Console.WriteLine(error);
                    return false;
                }
            }
            return true;
        }

        private int linealizar(List<object> indices, List<int> dims) {
            int indice = 0;
            for (int i = 0; i < dims.Count; i++)
            {
                retorno ret = (retorno)indices.ElementAt(i);
                if (i == 0)
                {
                    indice = Int32.Parse(ret.valor.ToString());
                }
                else {
                    indice = (indice * dims.ElementAt(i)) + Int32.Parse(ret.valor.ToString());
                }
            }
            return indice;
        }

        private int convertiraDias(String fechaHora, String tipo) {
            int inicial = 730001; //es el equivalente en dias de la fecha '01/01/2000 00:00:00'

            fechaHora = fechaHora.Replace("'","");
            switch (tipo)
            {
                case Cadena.Hora:
                    String[] val = fechaHora.Split(':');
                    int horas = Int32.Parse(val[0]);
                    if (horas > 12)
                        return inicial+1;
                    else
                        return inicial+0;
                case Cadena.Fecha:
                    String[] val2 = fechaHora.Split('/');
                    int dias = Int32.Parse(val2[0]);
                    int diasMes = (Int32.Parse(val2[1]) - 1) * 30;
                    int diasAno = Int32.Parse(val2[2])*365;
                    return (dias + diasMes + diasAno) - inicial;
                case Cadena.FechaHora:
                    String[] val3 = fechaHora.Split(' ');
                    String[] val4 = val3[1].Split(':');
                    String[] val5 = val3[0].Split('/');
                    //sumamos hora y fecha
                    int diasHora =0;
                    if (Int32.Parse(val4[0]) > 12)
                    {
                        diasHora = 1;
                    }
                    int dias2 = Int32.Parse(val5[0]);
                    int dias2Mes = (Int32.Parse(val5[1]) - 1) * 30;
                    int dias2Ano = Int32.Parse(val5[2]) * 365;
                    return (diasHora+dias2 + dias2Mes + dias2Ano) - inicial;
            }
            return 0;
        }

        private int calcularASCII(String cadena) {
            int val = 0;
            for (int i = 0; i < cadena.Length; i++)
            {
                val += cadena.ElementAt(i);
            }
            return val;
        }

        private bool validarPrimitivo(String tipo) { 
            if(tipo.ToLower().Equals(Cadena.Entero) || tipo.ToLower().Equals(Cadena.Cad) || tipo.ToLower().Equals(Cadena.Decimmal) || tipo.ToLower().Equals(Cadena.Booleano) ||
                tipo.ToLower().Equals(Cadena.Hora) || tipo.ToLower().Equals(Cadena.FechaHora) || tipo.ToLower().Equals(Cadena.Fecha)){
                    return true;
                }
            return false;
        }

        private bool permiteDetner()
        {
            if (cima.Tipo.Equals(Cadena.ambito_for) || cima.Tipo.Equals(Cadena.ambito_while) || cima.Tipo.Equals(Cadena.ambito_select) ||
                cima.Tipo.Equals(Cadena.ambito_do) || cima.Tipo.Equals(Cadena.ambito_repeat))
            {
                return true;
            }
            return false;
        }

        private bool permiteContinuar()
        {
            if (cima.Tipo.Equals(Cadena.ambito_for) || cima.Tipo.Equals(Cadena.ambito_while)|| cima.Tipo.Equals(Cadena.ambito_do) || cima.Tipo.Equals(Cadena.ambito_repeat))
            {
                return true;
            }
            return false;
        }

        private void notificar(string mensaje)
        {
            consola.AppendText(">>: " + mensaje + "\n");
        }

        private void pasarVarTemporal() { 
            //este metodo se encargar de pasar las variables de la clase padre a una tabla tempora cuando se llame a una funcion de un objeto o una matriz

            Stack<TablaSimbolos> tmp = pilaVFO.ElementAt(1).pilaSimbolos;
            Padre = new TablaSimbolos("temporal", "objeto", false, false, false);
            foreach (TablaSimbolos item in tmp)
            {
                foreach(KeyValuePair<String,Simbolo> sim in item.getT())
                    {
                        Padre.insertar(sim.Key, sim.Value, "Temporal Objeto");
                    }
            }
            temporal.Push(Padre);
        }

        private string retornarTipo(string tipo) {
            switch (tipo.ToLower())
            {
                case "entero":  
                        return Cadena.Entero;
                case "decimal":
                   
                        return Cadena.Decimmal;
                case "cadena":
                   
                    return  Cadena.Cad;
                case "booleano":

                    return Cadena.Booleano;
                case "fecha":
                  
                    return  Cadena.Fecha;
                case "hora":
                   
                    return Cadena.Hora;
                case "fechahora":
                    return  Cadena.FechaHora;
                default://tipo objeto
                    return tipo;
            }
        }
    }
}
