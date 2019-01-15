using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using XFORM.Analizador;

namespace XFORM.Analizador
{
    class Gramatica:Grammar
    {
        
        public Gramatica(): base(caseSensitive: false){
            //Expresiones regulares;
            #region
            var idXform = new RegexBasedTerminal(Cadena.Idxform,"[a-zA-Z]([a-zA-Z0-9_])*.xform");
            var id = new RegexBasedTerminal(Cadena.Id, "[a-zA-Z]([a-zA-Z]|_|[0-9])*");
            StringLiteral cadena = new StringLiteral(Cadena.Cad, "\"");
            var decimmal = new NumberLiteral(Cadena.Decimmal); 
            var entero = new RegexBasedTerminal(Cadena.Entero,"[0-9]+");
            var fecha = new RegexBasedTerminal(Cadena.Fecha, "'[0-9][0-9]/[0-9][0-9]/[0-9][0-9][0-9][0-9]'");
            var hora = new RegexBasedTerminal(Cadena.Hora,"'[0-9][0-9]:[0-9][0-9]:[0-9][0-9]'" );
            var fechahora = new RegexBasedTerminal(Cadena.FechaHora, "'[0-9][0-9]/[0-9][0-9]/[0-9][0-9][0-9][0-9] [0-9][0-9]:[0-9][0-9]:[0-9][0-9]'");
            var trus = new RegexBasedTerminal(Cadena.Booleano, "'Verdadero'|Verdadero");
            var fals = new RegexBasedTerminal(Cadena.Booleano, "'Falso'|Falso");
            //comentario
            CommentTerminal comentario1 = new CommentTerminal("com1", "$#", "#$");
            CommentTerminal comentario2 = new CommentTerminal("com2", "$$", "\n");
            base.NonGrammarTerminals.Add(comentario1);// esto hace que no sea tomado como un terminal 
            base.NonGrammarTerminals.Add(comentario2);
            #endregion
            // Termianles
            #region
            //encabezado
            var Importar = ToTerm(Cadena.Importar);            
            //tipos
            var Entero = ToTerm(Cadena.Entero);
            var Booleano = ToTerm(Cadena.Booleano);
            var Fecha = ToTerm(Cadena.Fecha);
            var Cad = ToTerm(Cadena.Cad);
            var Decimal = ToTerm(Cadena.Decimmal);
            var Hora = ToTerm(Cadena.Hora);
            var FechaHora = ToTerm(Cadena.FechaHora);
            //var Respuesta = ToTerm(Cadena.Respuesta);
            var Vacio = ToTerm(Cadena.Vacio);
            var Nulo = ToTerm(Cadena.Nulo);
            //cuerpo
            var Clase = ToTerm(Cadena.Clase);
            var Padre = ToTerm(Cadena.Padre);
            var Super = ToTerm(Cadena.Super);
            var Nuevo = ToTerm(Cadena.Nuevo);
            //visibilidad
            var Publico = ToTerm(Cadena.Publico);
            var Privado = ToTerm(Cadena.Privado);
            var Protegido = ToTerm(Cadena.Protegido);
            //
            var Principal = ToTerm(Cadena.Principal);
            var Si= ToTerm(Cadena.Si);
            var Sino = ToTerm(Cadena.Sino);
            var Retorno = ToTerm(Cadena.Retorno);
            var Caso = ToTerm(Cadena.Caso);
            var Defecto = ToTerm(Cadena.Defecto);
            var Para = ToTerm(Cadena.Para);
            var Continuar = ToTerm(Cadena.Continuar);
            var Romper = ToTerm(Cadena.Romper);
            var Mientras = ToTerm(Cadena.Mientras);
            var Hacer = ToTerm(Cadena.Hacer);
            var Hasta = ToTerm(Cadena.Hasta);
            var Repetir = ToTerm(Cadena.Repetir);
            ///funciones
            var Imprimir = ToTerm(Cadena.Imprimir);
            var Subcad = ToTerm(Cadena.Subcad);
            var Poscad = ToTerm(Cadena.Poscad);
            var Tam = ToTerm(Cadena.Tam);
            var Ramdom = ToTerm(Cadena.Random);
            var Min = ToTerm(Cadena.Min);
            var Max = ToTerm(Cadena.Max);
            var Pow = ToTerm(Cadena.Pow);
            var Log = ToTerm(Cadena.Log);
            var Log10 = ToTerm(Cadena.Log10);
            var Abs = ToTerm(Cadena.Abs);
            var Sin = ToTerm(Cadena.Sin);
            var Cos = ToTerm(Cadena.Cos);
            var Tan = ToTerm(Cadena.Tan);
            var Sqrt = ToTerm(Cadena.Sqrt);
            var Pi = ToTerm(Cadena.Pi);
            var Hoy = ToTerm(Cadena.Hoy);
            var Ahora = ToTerm(Cadena.Ahora);
            var Mensaje = ToTerm(Cadena.Mensaje);
            var Mostrar = ToTerm(Cadena.Mostrar);
            var Calcular = ToTerm(Cadena.Calcular);
            var Opciones = ToTerm(Cadena.Opciones);
            var Insertar = ToTerm(Cadena.Insertar);
            var Obtener = ToTerm(Cadena.Obtener);
            var Buscar = ToTerm(Cadena.Buscar);
            var Formulario = ToTerm(Cadena.Formulario);
            var Grupo = ToTerm(Cadena.Grupo);
            var Pagina = ToTerm(Cadena.Pagina);
            var Todo = ToTerm(Cadena.Todo);
            var Cuadricula = ToTerm(Cadena.Cuadricula);
            var Pregunta = ToTerm(Cadena.Pregunta);    
            #endregion
            // No Terminales
            #region
            NonTerminal INI = new NonTerminal(Cadena.INICIO),
            IMPORT = new NonTerminal(Cadena.IMPORT),
            CLASS = new NonTerminal(Cadena.CLASS),
            CLASE = new NonTerminal(Cadena.CLASE),
            VIS = new NonTerminal(Cadena.VIS),
            VIS2 = new NonTerminal(Cadena.VIS2),
            HER = new NonTerminal(Cadena.HER),
            CUERPO = new NonTerminal(Cadena.CUERPO),
            CUE = new NonTerminal(Cadena.CUE),
            TIPO = new NonTerminal(Cadena.TIPO),
            RES = new NonTerminal(Cadena.RES),
            RES2 = new NonTerminal(Cadena.RES2),
            RES3 = new NonTerminal(Cadena.RES3),
            RES4 = new NonTerminal(Cadena.RES4),
            DIM = new NonTerminal(Cadena.DIM),
            DIM2 = new NonTerminal(Cadena.DIM2),
            DIM3 = new NonTerminal(Cadena.DIM3),
            MAT = new NonTerminal(Cadena.MAT),
            L_EXP = new NonTerminal(Cadena.L_EXP),
            L_PAR = new NonTerminal(Cadena.L_PAR),
            CUERPO2 = new NonTerminal(Cadena.CUERPO2),
            CUE2 = new NonTerminal(Cadena.CUE2),
            AS2 = new NonTerminal(Cadena.AS2),
            AS3 = new NonTerminal(Cadena.AS3),
            L_AS = new NonTerminal(Cadena.L_AS),
            L_ACC = new NonTerminal(Cadena.L_ACC),
            ACC = new NonTerminal(Cadena.ACC),
            VAL = new NonTerminal(Cadena.VAL),
            VAL2 = new NonTerminal(Cadena.VAL2),
            VAL3 = new NonTerminal(Cadena.VAL3),
            LOG = new NonTerminal(Cadena.LOG),
            REL = new NonTerminal(Cadena.REL),
            OP_REL = new NonTerminal(Cadena.OP_REL),
            ARIT = new NonTerminal(Cadena.ARIT),
            CASOS = new NonTerminal(Cadena.CASOS),
            DEF = new NonTerminal(Cadena.DEF),
            VAR = new NonTerminal(Cadena.VAR),// -n
            CUE_F = new NonTerminal(Cadena.CUE_F),
            CUE_P = new NonTerminal(Cadena.CUE_P),
                //=====================================
            IMPORTA = new NonTerminal(Cadena.IMPORTA),
            SUPER = new NonTerminal(Cadena.SUPER),
            DEC_MET = new NonTerminal(Cadena.DEC_MET),
            DEC_FUN = new NonTerminal(Cadena.DEC_FUN),
            PRINCIPAL = new NonTerminal(Cadena.PRINCIPAL),
            LLAMADA = new NonTerminal(Cadena.LLAMADA),
            DEC_ASIGNA_VAR = new NonTerminal(Cadena.DEC_ASIGNA_VAR),
            DEC_VAR = new NonTerminal(Cadena.DEC_VAR),
            DEC_VAR_2 = new NonTerminal(Cadena.DEC_VAR_2),
            DEC_ASIGNA_MAT = new NonTerminal(Cadena.DEC_ASIGNA_MAT),
            DEC_ASIGNA_MAT_2 = new NonTerminal(Cadena.DEC_ASIGNA_MAT_2),
            ASIGNA = new NonTerminal(Cadena.ASIGNA),
            ASIGNA_MAT = new NonTerminal(Cadena.ASIGNA_MAT),
            CONSTRUC = new NonTerminal(Cadena.CONSTRUCT),
            ACC_OBJ = new NonTerminal(Cadena.ACC_OBJ),
            ACC_PRE = new NonTerminal(Cadena.ACC_PRE),
            MET_PREG = new NonTerminal(Cadena.MET_PREG),
            OPCIONES = new NonTerminal(Cadena.OPCIONES),
            ADD_OP = new NonTerminal(Cadena.ADD_OP),
            //Sentencias metodos y funciones
            DECLA_SIN = new NonTerminal(Cadena.DECLA_SIN),
            RETORNO = new NonTerminal(Cadena.RETORNO),
            ROMPER = new NonTerminal(Cadena.ROMPER),
            CONTINUAR = new NonTerminal(Cadena.CONTINUAR),
            SI = new NonTerminal(Cadena.SI),
            PARA = new NonTerminal(Cadena.PARA),
            OP = new NonTerminal(Cadena.OP),
            SELECCIONA = new NonTerminal(Cadena.SELECCIONA),
            MIENTRAS = new NonTerminal(Cadena.MIENTRAS),
            HACER = new NonTerminal(Cadena.HACER),
            REPETIR = new NonTerminal(Cadena.REPETIR),
            PREGUNTA = new NonTerminal(Cadena.PREGUNTA),
            GRUPO = new NonTerminal(Cadena.GRUPO),
            FORMULARIO = new NonTerminal(Cadena.FORMULARIO),
            FORM = new NonTerminal(Cadena.FORM),
            INSTANCIA = new NonTerminal(Cadena.INSTANCIA),
            NUEVO = new NonTerminal(Cadena.NUEVO),
            SISIMP = new NonTerminal(Cadena.SISIMP),
                //FUNCIONES NATIVAS SIN RETORNO 
            IMPRIMIR = new NonTerminal(Cadena.IMPRIMIR),
            MENSAJE = new NonTerminal(Cadena.MENSAJE),
            IMAGEN = new NonTerminal(Cadena.IMAGEN),
                //FUNCIONES NATIVAS CON RETORNO
            CADENA = new NonTerminal(Cadena.CADENA),
            SUBCAD = new NonTerminal(Cadena.SUBCAD),
            POSCAD = new NonTerminal(Cadena.POSCAD),
            BOOLEANO = new NonTerminal(Cadena.BOOLEANO),
            ENTERO = new NonTerminal(Cadena.ENTERO),
            TAM = new NonTerminal(Cadena.TAM),
            RANDOM = new NonTerminal(Cadena.RANDOM),
            MIN = new NonTerminal(Cadena.MIN),
            MAX = new NonTerminal(Cadena.MAX),
            POW = new NonTerminal(Cadena.POW),
            LOG1 = new NonTerminal(Cadena.LOG1),
            LOG10 = new NonTerminal(Cadena.LOG10),
            ABS = new NonTerminal(Cadena.ABS),
            SIN = new NonTerminal(Cadena.SIN),
            COS = new NonTerminal(Cadena.COS),
            TAN = new NonTerminal(Cadena.TAN),
            SQRT = new NonTerminal(Cadena.SQRT),
            PI = new NonTerminal(Cadena.PI),
            HOY = new NonTerminal(Cadena.HOY),
            AHORA = new NonTerminal(Cadena.AHORA),
            FECHA = new NonTerminal(Cadena.FECHA),
            HORA = new NonTerminal(Cadena.HORA),
            FECHAHORA = new NonTerminal(Cadena.FECHAHORA),
            GET_OP = new NonTerminal(Cadena.GET_OP),
            SEARCH_OP = new NonTerminal(Cadena.SEARCH_OP);
            //de transicion
            NonTerminal PAR = new NonTerminal("PAR"),
            DI = new NonTerminal("DI"),
            DI2 = new NonTerminal("DI2"),
            DI3 = new NonTerminal("DI3"),
            CAS = new NonTerminal("CAS"),
            PRE  = new NonTerminal("PRE"),
            TIPO2 = new NonTerminal("TIPO2");
            #endregion
            // Gramatica 
            #region
            INI.Rule = IMPORT + CLASS;
            // ENCABEZADO
            #region
            IMPORT.Rule = MakeStarRule(IMPORT, IMPORTA);
            IMPORTA.Rule = Importar + "(" + idXform + ")"+";";
            IMPORTA.ErrorRule = SyntaxError + ";";
            //=====================
            CLASS.Rule = MakePlusRule(CLASS, CLASE);
            CLASE.Rule = Clase + id + VIS + HER + "{" +CUERPO + "}";
            HER.Rule = Padre + id
                    |Empty;
            CLASE.ErrorRule = SyntaxError + "}";
            #endregion 
            // CUERPO
            #region
            CUERPO.Rule = MakePlusRule(CUERPO, CUE);

            CUE.Rule =
                  DEC_ASIGNA_VAR //DECLARACION Y ASIGNACION DE VARS GLOBALES
                | DEC_FUN // DECLARACION DE FUNCION
                | DEC_MET // DECLARACION DE METODO
                | DEC_ASIGNA_MAT //DECLARACION Y ASIGNACION DE MAT GLOBALES
                | CONSTRUC // CONSTRUCTOR
                | ASIGNA  //ASIGNACION DE VARIANLES GLOBALES
                | ASIGNA_MAT  //ASIGNACION DE MATRIZ
                | DEC_VAR //DECLARACION  DE VARS GLOBALES
                | PRINCIPAL //METODO PRINCIPAL
                | PREGUNTA
                | GRUPO 
                | FORMULARIO;

            CUE.ErrorRule = SyntaxError + "}";
            CUE.ErrorRule = SyntaxError + ";";

            
            PRINCIPAL.Rule = Principal + "(" + ")" + "{" + CUERPO2 + "}";
            DEC_ASIGNA_VAR.Rule = TIPO + VIS + id + "=" + LOG +";";
            DEC_FUN.Rule = VIS + TIPO + id + "(" + L_PAR + ")" + "{" + CUERPO2 + "}";
            DEC_MET.Rule = VIS + Vacio + id + "(" + L_PAR + ")" + "{" + CUERPO2 + "}";
            DEC_ASIGNA_MAT.Rule = TIPO + VIS + id + DIM + "=" + MAT + ";";
            CONSTRUC.Rule = //Publico + id + "(" + L_PAR + ")" + "{" + CUERPO2 + "}"
                            id + "(" + L_PAR + ")" + "{" + CUERPO2 + "}";
            ASIGNA_MAT.Rule = id + DIM3 + "=" + LOG + ";";
            ASIGNA.Rule = id + "=" + LOG +";" ;
            DEC_VAR.Rule = TIPO + VIS + id + ";";
            GRUPO.Rule = Grupo +id +"(" + ")" + "{" + CUERPO2 + "}";
            FORMULARIO.Rule = Formulario +id+ "(" + ")" + "{" + CUERPO2 + "}";
            PREGUNTA.Rule = Pregunta + id + "(" + L_PAR + ")" + "{" + CUERPO2 + "}";
            #endregion
            // RESTO DE REGLAS DE CUERPO
            #region
            L_EXP.Rule = MakeStarRule(L_EXP,ToTerm(","),LOG);

            VIS.Rule = Privado
                    | Publico
                    | Protegido
                    | Empty;

            TIPO.Rule = Cad
                    | Entero
                    | Booleano
                    | Decimal
                    | Fecha
                    | Hora
                    | FechaHora
                    //| Respuesta 
                    | id;

            L_PAR.Rule = MakeStarRule(L_PAR,ToTerm(",") ,PAR);
            PAR.Rule = TIPO + id;

            DIM.Rule = MakePlusRule(DIM, DI);
            DI.Rule = ToTerm("[") + ToTerm("]");

            MAT.Rule = Nuevo + TIPO + DIM2
                | AS2;

            DIM2.Rule = MakePlusRule(DIM2,DI2);
            DI2.Rule = ToTerm("[") + entero + ToTerm("]");

            AS2.Rule = "{" + AS3 + "}";

            AS3.Rule = L_AS
                    | L_EXP;

            L_AS.Rule = MakePlusRule(L_AS, ToTerm(","), AS2);

            DIM3.Rule = MakePlusRule(DIM3, DI3);
            DI3.Rule = ToTerm("[") + LOG + ToTerm("]");

            #endregion
            // INSTRUCCIONES o CUERPO2
            #region 
            CUERPO2.Rule = MakeStarRule(CUERPO2, CUE2); // 
            CUE2.Rule = SUPER 
                | DEC_ASIGNA_MAT_2 
                | ASIGNA_MAT
                | ASIGNA
                | DEC_VAR_2
                | RETORNO
                | SI
                | SELECCIONA
                | ROMPER
                | CONTINUAR
                | MIENTRAS
                | HACER
                | REPETIR
                | PARA
                | IMPRIMIR
                | IMAGEN
                | MENSAJE
                | LLAMADA + ";"
                | ACC_OBJ + ";"//ACCESO A OBJETOS
                | ACC_PRE + ";"//acceso a preguntas
                | OP + ";"
                | MET_PREG
                | OPCIONES
                | ADD_OP
                | FORM;

            SUPER.Rule = Super + "(" + L_EXP + ")" + ";";
            CUE2.ErrorRule = SyntaxError + "}";
            CUE2.ErrorRule = SyntaxError + ";";
            DEC_ASIGNA_MAT_2.Rule = TIPO + id + DIM + "=" + MAT + ";";
            DEC_VAR_2.Rule = TIPO + id + RES + ";";
            RETORNO.Rule = Retorno + VAL2 + ";";
            SI.Rule = Si + "("+ LOG + ")" + "{" + CUERPO2 + "}" + RES2 ;
            SELECCIONA.Rule = Caso + "(" + LOG + ")" + ToTerm("de") + "{" + CASOS + DEF + "}";
            MIENTRAS.Rule = Mientras + "(" + LOG + ")" + "{" + CUERPO2 + "}" ;
            HACER.Rule = Hacer + "{" + CUERPO2 + "}" + Mientras + "(" + LOG + ")" + ";" ;
            REPETIR.Rule = Repetir + "{" + CUERPO2 + "}" + Hasta + "(" + LOG + ")" + ";" ;
            PARA.Rule = Para + "(" + VAR + ";" + LOG + ";" + OP + ")" + "{" + CUERPO2 + "}" ;
            IMPRIMIR.Rule = Imprimir + "(" + LOG + ")" + ";";
            MENSAJE.Rule = Mensaje + "(" + LOG + ")" + ";";
            IMAGEN.Rule = id + "("+cadena + "," + VAL3 +")" + ";" ; ///funcion multimedia
            ACC_OBJ.Rule = id + "." + L_ACC + RES ;
            ACC_PRE.Rule = id +"("+L_EXP+")" +"." + L_ACC ;
            LLAMADA.Rule = id + ToTerm("(") + L_EXP + ToTerm(")");
            ROMPER.Rule = Romper + ";";
            CONTINUAR.Rule = Continuar + ";";
            MET_PREG.Rule = VIS2 + id + "(" + L_PAR + ")" + "{" + CUERPO2 + "}";
            OPCIONES.Rule = Opciones + id + "=" + Nuevo + Opciones+"(" + ")" + ";";
            ADD_OP.Rule = id + "." + Insertar + "(" + L_EXP + ")" + ";";
            FORM.Rule = Nuevo + id + "(" + ")" + "." + TIPO2 + ";"; 
            #endregion
            //RESTO DE REGLAS  CUERPO2
            #region 
            RES.Rule = "=" + LOG
                    | Empty ;

            VAL2.Rule = LOG
                    | Empty;

            RES2.Rule = MakeStarRule(RES2, RES3);

            RES3.Rule = Sino + Si + ToTerm("(") + LOG + ToTerm(")") + ToTerm("{") + CUERPO2 + ToTerm("}")
                    | Sino + "{" + CUERPO2 + "}";

            CASOS.Rule = MakeStarRule(CASOS, CAS);
            CAS.Rule = VAL + ToTerm(":") + ToTerm("{") + CUERPO2 + ToTerm("}");

            VAL.Rule = decimmal
                    | entero
                    | cadena ;

            DEF.Rule = Defecto +":"+ "{" + CUERPO2 + "}"
                    | Empty;

            VAR.Rule = Entero + id + "=" + LOG
                    |  Decimal + id + "=" + LOG
                    |  id + "=" + LOG ;

            OP.Rule = id + "++"
                    | id + "--";
                   

            VAL3.Rule = trus
                    | fals;

            L_ACC.Rule = MakePlusRule(L_ACC, ToTerm("."), ACC);

            ACC.Rule = id
                    | TIPO + "(" + L_EXP + ")"
                    | id + DIM3;

            VIS2.Rule = Privado
                    | Publico;

            TIPO2.Rule = Grupo
                | Pagina
                | Todo
                | Cuadricula;
            #endregion
            // REGLAS DE EXPRESIONES
            #region
            LOG.Rule = LOG + ToTerm("||","or") + REL
                | LOG + ToTerm("&&","and") + REL
                | LOG + ToTerm("?","cond") + LOG + ":" + LOG
                | REL;

            REL.Rule = ARIT + OP_REL + ARIT
                | ARIT;

            OP_REL.Rule = ToTerm("==")
                | ToTerm("!=")
                | ToTerm("<")
                | ToTerm(">")
                | ToTerm("<=")
                | ToTerm(">=");

            ARIT.Rule = ARIT + ToTerm("+") + ARIT
                | ARIT + ToTerm("-") + ARIT
                | ARIT + ToTerm("*") + ARIT
                | ARIT + ToTerm("/") + ARIT
                | ARIT + ToTerm("^") + ARIT
                | ARIT + ToTerm("%") + ARIT
                | ToTerm("-") + ARIT
                | ToTerm("!") + LOG
                | ToTerm("(") + LOG + ToTerm(")")
                | ToTerm("(") + LOG + ToTerm(")") + ToTerm("++")
                | ToTerm("(") + LOG + ToTerm(")") + ToTerm("--")
                | id + DIM3 
                | id 
                | cadena
                | decimmal
                | entero
                | fecha
                | hora
                | fechahora
                | fals
                | trus
                | Nulo
                | OP
                | LLAMADA
                | ACC_OBJ
                | ACC_PRE
                | NUEVO
                //| SISIMP
                //funciones especiales 
                | CADENA
                | SUBCAD
                | POSCAD
                | BOOLEANO
                | ENTERO
                | TAM
                | RANDOM
                | MIN
                | MAX
                | POW
                | LOG1
                | LOG10
                | ABS
                | SIN
                | COS
                | TAN
                | SQRT
                | PI
                | HOY
                | AHORA
                | FECHA
                | HORA
                | FECHAHORA
                | SEARCH_OP
                | GET_OP;

            NUEVO.Rule = Nuevo + id + "(" + L_EXP + ")";    
            CADENA.Rule = Cad + "(" + LOG + ")";
            SUBCAD.Rule = Subcad + "(" + LOG + "," + LOG + "," + LOG + ")";
            POSCAD.Rule = Poscad + "(" + LOG + "," + LOG + ")";
            BOOLEANO.Rule = Booleano + "(" + LOG + ")";
            ENTERO.Rule = Entero + "(" + LOG + ")";
            TAM.Rule = Tam + "(" + LOG + ")";
            RANDOM.Rule = Ramdom + "(" + L_EXP + ")";
            MIN.Rule = Min + "(" + L_EXP + ")";
            MAX.Rule = Max + "(" + L_EXP + ")";
            POW.Rule = Pow + "(" + LOG + "," + entero + ")";
            LOG1.Rule = Log + "(" + LOG + ")";
            LOG10.Rule = Log10 + "(" + LOG + ")";
            ABS.Rule = Abs + "(" + LOG + ")";
            SIN.Rule = Sin + "(" + LOG + ")";
            COS.Rule = Cos + "(" + LOG + ")";
            TAN.Rule = Tan + "(" + LOG + ")";
            SQRT.Rule = Sqrt + "(" + LOG + ")";
            PI.Rule = Pi + "(" + ")";
            HOY.Rule = Hoy + "(" + ")";
            AHORA.Rule = Ahora + "(" + ")";
            FECHA.Rule = Fecha + "(" + cadena + ")";
            HORA.Rule = Hora + "(" + cadena + ")";
            FECHAHORA.Rule = FechaHora + "(" + cadena + ")";
            GET_OP.Rule = id + "." + Obtener + "(" + entero + ")" + "[" + entero + "]";
            SEARCH_OP.Rule = id + "." + Buscar + "(" + LOG + ")" + "[" + entero + "]";
            #endregion

            #endregion

            // Definir Precedencia
            this.RegisterOperators(1, Associativity.Left, "||");
            this.RegisterOperators(2, Associativity.Left, "&&");
            this.RegisterOperators(3, Associativity.Neutral,"?");
            this.RegisterOperators(4, Associativity.Left, "==", "!=", ">", "<","<=", ">=");
            this.RegisterOperators(5, Associativity.Left, "+", "-");
            this.RegisterOperators(6, Associativity.Left, "*", "/","%");
            this.RegisterOperators(7, Associativity.Right, "^");
            this.RegisterOperators(8, Associativity.Right, "!");
            this.RegisterOperators(9, Associativity.Left, "(",")");

            //Reservadas
            MarkReservedWords(Cadena.Abs, Cadena.Ahora, Cadena.Booleano, Cadena.Buscar, Cadena.Cad, Cadena.Calcular, Cadena.Caso, Cadena.Clase);
            MarkReservedWords(Cadena.Continuar, Cadena.Cos,Cadena.Cuadricula, Cadena.Decimmal, Cadena.Defecto, Cadena.Entero, Cadena.Fecha, Cadena.FechaHora);
            MarkReservedWords(Cadena.Formulario, Cadena.Grupo,Cadena.Hacer, Cadena.Hasta, Cadena.Hora, Cadena.Hoy,Cadena.Importar);
            MarkReservedWords(Cadena.Imprimir, Cadena.Insertar, Cadena.Log, Cadena.Log10, Cadena.Max, Cadena.Mensaje,Cadena.Mientras);
            MarkReservedWords(Cadena.Min, Cadena.Mostrar, Cadena.Nuevo, Cadena.Nulo, Cadena.Obtener, Cadena.Opciones,Cadena.Padre,Cadena.Pagina);
            MarkReservedWords(Cadena.Para, Cadena.Pi, Cadena.Poscad, Cadena.Pow, Cadena.Pregunta, Cadena.Principal, Cadena.Privado);
            MarkReservedWords(Cadena.Protegido, Cadena.Publico, Cadena.Random, Cadena.Repetir, /*Cadena.Respuesta,*/ Cadena.Retorno, Cadena.Romper);
            MarkReservedWords(Cadena.Si, Cadena.Sin, Cadena.Sino, Cadena.Sqrt, Cadena.Subcad, Cadena.Super, Cadena.Tam,Cadena.Tan,Cadena.Todo,Cadena.Vacio);
            //Personalizacion del arbol
            MarkPunctuation("(",")","{","}",";","=",",",":",".","[","]");
            MarkPunctuation(Cadena.Importar, Cadena.Clase, Cadena.Padre);
            MarkTransient(VAL,CUE,CUE2,TIPO,TIPO2,OP_REL,RES,IMPORTA,AS2,AS3,DI3,DI2,VIS2,VAL3);

            // Definir la raiz
            this.Root = INI;


        }

    }
}
