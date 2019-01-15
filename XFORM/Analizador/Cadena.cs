using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFORM.Analizador
{
    class Cadena
    {
        //Inicio de los No Terminales 
        #region
        public const string INICIO="INICIO";
        public const string IMPORT = "IMPORT";
        public const string CLASS = "CLASS";
        public const string CLASE = "CLASE";
        public const string VIS = "VIS";
        public const string VIS2 = "VIS2";
        public const string HER = "HER";
        public const string CUERPO = "CUERPO";
        public const string CUERPO2 = "CUERPO2";
        public const string CUE = "CUE";
        public const string CUE2 = "CUE2";
        public const string TIPO = "TIPO";
        public const string RES = "RES";
        public const string RES2 = "RES2";
        public const string RES3 = "RES3";
        public const string RES4 = "RES4";
        public const string DIM = "DIM";
        public const string DIM2 = "DIM2";
        public const string DIM3 = "DIM3";
        public const string MAT = "MAT";
        public const string L_EXP = "L_EXP";
        public const string L_PAR = "L_PAR";
        public const string AS2 = "AS2";
        public const string AS3 = "AS3";
        public const string L_AS = "L_AS";
        public const string L_ACC = "L_ACC";
        public const string ACC = "ACC";
        public const string VAL = "VAL";
        public const string VAL2 = "VAL2";
        public const string VAL3 = "VAL3";
        public const string LOG = "LOG";
        public const string REL = "REL";
        public const string OP_REL = "OP_REL";
        public const string ARIT = "ARIT";
        public const string CASOS = "CASOS";
        public const string DEF = "DEF";
        public const string VAR = "VAR";
        public const string CUE_F = "CUE_F";
        public const string CUE_P = "CUE_P";
        //================================================
        public const string IMPORTA = "IMPORTA";
        public const string SUPER = "SUPER";
        public const string DEC_MET = "DEC_MET";
        public const string DEC_FUN = "DEC_FUN";
        public const string DEC_VAR = "DEC_VAR";
        public const string DEC_VAR_2 = "DEC_VAR_2";
        public const string LLAMADA = "LLAMADA";
        public const string PRINCIPAL = "PRINCIPAL";
        public const string DEC_ASIGNA_VAR = "DEC_ASIGNA_VAR";
        public const string DEC_ASIGNA_MAT = "DEC_ASIGNA_MAT";
        public const string DEC_ASIGNA_MAT_2 = "DEC_ASIGNA_MAT_2";
        public const string ASIGNA = "ASIGNA";
        public const string ASIGNA_MAT = "ASIGNA_MAT";
        public const string CONSTRUCT = "CONSTRCUT";
        public const string ACC_OBJ= "ACC_OBJ";
        public const string ACC_PRE = "ACC_PRE";
        public const string MET_PREG = "MET_PREG";
        public const string OPCIONES = "OPCIONES";
        public const string ADD_OP = "ADD_OP";
        public const string SEARCH_OP = "SEARCH_OP";
        public const string GET_OP = "GET_OP";
        //
        public const string DECLA_SIN = "DECLA_SIN";
        public const string RETORNO = "RETORNO";
        public const string ROMPER = "ROMPER";
        public const string CONTINUAR = "CONTINUAR";
        public const string SI = "SI";
        public const string PARA = "PARA";
        public const string OP = "OP";
        public const string SELECCIONA = "SELECCIONA";
        public const string HASTA = "HASTA";
        public const string MIENTRAS = "MIENTRAS";
        public const string HACER = "HACER";
        public const string REPETIR = "REPETIR";
        public const string PREGUNTA = "PREGUNTA";
        public const string GRUPO = "GRUPO";
        public const string FORMULARIO = "FORMULARIO";
        public const string FORM = "FORM";
        public const string INSTANCIA = "INSTANCIA";
        public const string NUEVO = "NUEVO";
        public const string SISIMP = "SISIMP";
        // funciones nativas sin retorno
        public const string IMPRIMIR = "IMPRIMIR";
        public const string MENSAJE = "MENSAJE";
        public const string IMAGEN = "IMAGEN";
        // funciones nativas con retorno
        public const string CADENA = "CADENA";
        public const string SUBCAD = "SUBCAD";
        public const string POSCAD = "POSCAD";
        public const string BOOLEANO = "BOOLEANO";
        public const string ENTERO = "ENTERO";
        public const string TAM = "TAM";
        public const string RANDOM = "RANDOM";
        public const string MIN = "MIN";
        public const string MAX = "MAX";
        public const string POW = "POW";
        public const string LOG1 = "LOG1";
        public const string LOG10 = "LOG10";
        public const string ABS = "ABS";
        public const string SIN = "SIN";
        public const string COS = "COS"; // esta pendiente el uso de este no terminal
        public const string TAN = "TAN";
        public const string SQRT = "SQRT";
        public const string PI= "PI";
        public const string HOY = "HOY";
        public const string AHORA = "AHORA";
        public const string FECHA = "FECHA";
        public const string HORA = "HORA";
        public const string FECHAHORA = "FECHAHORA";
        #endregion
//====================================================================================================================================================

        // incio de los Terminales 
        #region
        //encabezado
        public const string Importar = "Importar";
        public const string Idxform = "Idxform";
        //tipos de datos
        public const string Id = "Id";
        public const string Cad = "Cadena";
        public const string Entero = "Entero";
        public const string Decimmal = "Decimal";
        public const string Fecha = "Fecha";
        public const string Hora = "Hora";
        public const string FechaHora = "FechaHora";
        public const string Respuesta = "Respuesta";
        public const string Booleano = "Booleano";
        public const string Vacio = "Vacio";
        public const string Nulo = "Nulo";
        public const string Objeto = "Objeto";
        public const string Matriz = "Matriz";
        //Cuerpo
        public const string Clase = "Clase";
        public const string Padre = "Padre";
        public const string Super = "Super";
        //visivilidad
        public const string Publico = "Publico";
        public const string Privado = "Privado";
        public const string Protegido = "Protegido";
        public const string Principal = "Principal";
        public const string Nuevo = "Nuevo";
        public const string Si = "Si";
        public const string Sino = "Sino";
        public const string Retorno = "Retorno";
        public const string Continuar = "Continuar";
        public const string Romper = "Romper";
        public const string Caso = "Caso";
        public const string Defecto = "Defecto";
        public const string Mientras = "Mientras";
        public const string Hacer = "Hacer";
        public const string Repetir = "Repetir";
        public const string Hasta = "Hasta";
        public const string Para = "Para";
        public const string Imprimir = "Imprimir";
        public const string Subcad = "Subcad";
        public const string Poscad = "Poscad";
        public const string Tam = "Tam";
        public const string Random = "Random";
        public const string Min = "Min";
        public const string Max = "Max";
        public const string Pow = "Pow";
        public const string Log = "Log";
        public const string Log10 = "Log10";
        public const string Abs = "Abs";
        public const string Sin = "Sin";
        public const string Cos = "Cos";
        public const string Tan = "Tan";
        public const string Sqrt = "Sqrt";
        public const string Pi = "Pi";
        public const string Hoy = "Hoy";
        public const string Ahora = "Ahora";
        public const string Mensaje = "Mensaje";
        public const string Mostrar = "Mostrar";
        public const string Calcular = "Calcular";
        public const string Opciones = "Opciones";
        public const string Insertar = "Insertar";
        public const string Obtener = "Obtener";
        public const string Buscar = "Buscar";
        public const string Formulario = "Formulario";
        public const string Grupo = "Grupo";
        public const string Todo = "Todo";
        public const string Pagina = "Pagina";
        public const string Cuadricula = "Cuadricula";
        public const string Pregunta = "Pregunta";

        #endregion
//====================================================================================================================================================
        //constantes de ejecucion


        public const String
            fun = "_func_",
            ambito_g = "Ambito_global",
            ambito_fun = "Ambito_funcion",
            ambito_if = "Ambito_if",
            ambito_for = "Ambito_for",
            ambito_while = "Ambito_while",
            ambito_select = "Ambito_select",
            ambito_else = "Ambito_else",
            ambito_ifelse = "Ambito_if_else",
            ambito_do = "Ambito_do",
            ambito_repeat = "Ambito_repeat";

        //tipos de retorno
        public const string error = "Error";
        //public const string rt_ = "Error";
    }
}
