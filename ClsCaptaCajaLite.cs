using System;
using System.Collections.Generic;
using System.Linq;
using AdnAdmonAplC71.Clases;
using CAPCajaLite.AfectacionCaja;
using System.Reflection;
using System.Diagnostics;
using System.Net;
using Banco.PD3.Persistence;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace CapCajaLite
{
    /// <summary>
    /// Definición de comportamiento de las funcionalidades para la interacción con CAJA64 BAZ
    /// </summary>
    public class ClsCaptaCajaLite : Kernell
    {
        #region Propiedades Privadas
        private short TipodeOperacion;
        private bool Esdivisa;
        private short TipoDivisa;
        private short IdTipoPago;
        private CtrAdmApl administradorAplicaciones;
        private string nt;
        private string respuestaCaja;
        private string ConcepMov;
        private Decimal litotchq;
        private string aplicacion;
        private readonly IDictionary<string, Assembly> agregados = new Dictionary<string, Assembly>();
        private bool afecta_Servicio_Componente;
        private TopsAfectacion[] topsAfectacion;
        private string sesion;
        private string RutaEnsamblados;
        private bool tipoAfectacion;
        private dynamic caja;
        private Assembly executingAssemblies = null;
        
        private bool tipoAfectacionReversoPagoCheque;
        private List<TipoPago> tiposPago;
        private int transaccionConfirmacion;
        private enum eTipoDivisa
        {
            eMXP = 1,
            eUSD = 2,
            eOLP = 3,
            eGTQ = 4,
            eCAD = 5,
            eGBP = 6,
            eEUR = 7,
            eC01 = 8
        }
        private enum TipoMovimiento
        {
            Ingreso = 1,
            Egreso = 2
        }
        #endregion

        #region Propiedades Públicas
        public int IdTransaccionADN;
        public int gnPresupuesto;
        public short RespuestaCajaNoError;
        public bool Anular_Efectivo;
        public bool Anular_Cheques;
        public int IdTransaccionAnularADN;
        public string Referencia;
        public string Divisa;
        public Decimal Importe;
        public Decimal ImporteCheque;
        public Decimal ImporteAlterno;
        public Decimal ImporteCompraVenta;
        public string RespuestaCaja
        {
            get { return respuestaCaja; }
            set { respuestaCaja = value; }
        }
        public string ConceptoMovto
        {
            get
            {
                return this.ConcepMov;
            }

            set
            {
                this.ConcepMov = value;
            }
        }
        public Decimal ImporteTotalCheques
        {
            private get
            {
                return this.litotchq;
            }
            set
            {
                this.litotchq = value;
            }
        }
        public short TOP
        {
            get { return TipodeOperacion; }
            set { TipodeOperacion = value; }
        }
        public short TipoPago
        {
            get { return this.IdTipoPago; }
            set { this.IdTipoPago = value; }
        }
        public string Aplicacion
        {
            get
            {
                return aplicacion;
            }

            set
            {
                aplicacion = value;
            }
        }
        public bool Afecta_Servicio_Componente
        {
            get
            {
                return afecta_Servicio_Componente;
            }

            set
            {
                afecta_Servicio_Componente = value;
            }
        }
        public TopsAfectacion[] TopsAfectacion
        {
            private get { return topsAfectacion; }
            set { topsAfectacion = value; }
        }
        public string Sesion
        {
            get
            {
                return sesion;
            }

            set
            {
                sesion = value;
            }
        }
        /// <summary>
        /// Tipo de Afectación detonado por el proceso de depósito de cheque mixto,
        /// se deposita efectivo al mismo tiempo que el cheque. Se debe enviar
        /// el tipo de afectación en 17 
        /// </summary>
        public bool TipoAfectacion
        {
            get
            {
                return tipoAfectacion;
            }

            set
            {
                tipoAfectacion = value;
            }
        }
        /// <summary>
        /// Tipo de Afectación solicitada para reverso de pago de cheque.
        /// </summary>
        public bool TipoAfectacionReversoPagoCheque
        {
            get
            {
                return tipoAfectacionReversoPagoCheque;
            }
            set
            {
                tipoAfectacionReversoPagoCheque = value;
            }
        }
        /// <summary>
        /// Arreglo de tipos de pago para el reverso de pago de cheque mixto. 
        /// Tipo de Afectación solicitada para reverso de pago de cheque.
        /// </summary>
        public List<TipoPago> TiposPago
        {
            get
            {
                return tiposPago;
            }
            set
            {
                tiposPago = value;
            }
        }
        /// <summary>
        /// Propiedad específica para reversos.
        /// </summary>
        public int TransaccionConfirmacion
        {
            get { return transaccionConfirmacion; }
            set { transaccionConfirmacion = value; }
        }
        #endregion

        #region Inicio_Instancia

        /// <summary>
        /// Constructor que llama a las funciones Crear e Iniciar
        /// </summary>
        /// <param name="Admin"></param>
        public ClsCaptaCajaLite(CtrAdmApl Admin)
        {
            this.administradorAplicaciones = Admin;
            this.Crear();
            this.Iniciar();
        }

        /// <summary>
        /// Funcioón Crear sobreescrita cone l 
        /// </summary>
        /// <returns></returns>
        public override bool Crear()
        {
            try
            {
                nt = string.Empty;
                if (!administradorAplicaciones.ctrAbd.Ejecutar("EXEC spConControl"))
                    throw new Exception("No fue posible conectar con el servidor para obtener la estación de trabajo.");
                if (administradorAplicaciones.ctrAbd.Registros() == 0)
                    throw new NullReferenceException("No existe la estación de trabajo correspondiente al servidor.");
                nt = string.Concat("NT", string.Format("{0:D4}", Convert.ToInt32(administradorAplicaciones.ctrAbd.Dato("fiNoTienda").ToString().Trim())));
                RutaEnsamblados = administradorAplicaciones.ctrParametro.Obtener(7285).ToString();
                administradorAplicaciones.ctrBitacora.MostrarEnTrace(string.Concat("RutaEnsamblados: ", RutaEnsamblados));
                try
                {
                    executingAssemblies = Assembly.LoadFrom(administradorAplicaciones.ctrParametro.Obtener(4189).ToString());
                }
                catch (Exception ex)
                {
                    Adm.ctrBitacora.GuardarEnLog(string.Format("Ocurrió un error al intentar cargar el componente {0}. Detalle: {1}", RutaEnsamblados, ex.ToString()));
                    throw;
                }
                afecta_Servicio_Componente = false; //Pasar a False cuando se vaya a producción.
                tipoAfectacion = false; //Para afectar por servicio  
                tipoAfectacionReversoPagoCheque = false; //Para afectaciones del reverso de pago de cheque mixto.
                return true;
            }
            catch (Exception ex)
            {
                administradorAplicaciones.ctrBitacora.GuardarEnLog(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Iniciar()
        {
            try
            {
                this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Iniciar");
                this.administradorAplicaciones.ctrBitacora.MostrarEnTrace("Iniciar()");
                Esdivisa = false;
                TipoDivisa = 0;
                this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Inicializa valores de importe");
                ImporteCheque = 0;
                Importe = 0;
                ImporteCompraVenta = 0;
                IdTransaccionAnularADN = 0;
                Sesion = string.Empty;
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Previamente se tienen que inicializar las siguientes variables:
        /// Divisa, TOP, Importe, ImporteCheque, ImporteAlterno,  
        /// ImporteCompraVenta, IdTransaccionADN, ConceptoMovto, 
        /// Referencia
        /// </summary>
        /// <returns></returns>
        public bool Egresar()
        {
            try
            {
                bool respuesta = false;
                ConfigurarDivisaDeCaja();
                decimal Importe_Total = Importe + ImporteCheque;               
                this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Ingresando a la afectación de caja con componente. (Egreso).");
                this.administradorAplicaciones.ctrBitacora.MostrarEnTrace("Ingresando a la afectación de caja con componente. (Egreso).");
                this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Iniciando instancia de AdmonLite");
                this.administradorAplicaciones.ctrBitacora.MostrarEnTrace("Iniciando instancia de AdmonLite");
                var admonLite = new AdnAdmonLite.Clases.CtrAdmApl();
                this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Ejecución de conexión con credenciales.");
                this.administradorAplicaciones.ctrBitacora.MostrarEnTrace("Ejecución de conexión con credenciales.");
                admonLite.Ejecutar(this.administradorAplicaciones.ctrSesion.IDUsuario, AdnAdmonLite.Clases.CtrAbd.eInterface.ePD3);
                admonLite.ctrBitacora.GuardarLog = true;
                this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Cargando en tiempo de ejecución componente de Caja.");
                this.administradorAplicaciones.ctrBitacora.MostrarEnTrace("Cargando en tiempo de ejecución componente de Caja.");

                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResuelveReferenciasEventHandler);

                var tipoCaja = executingAssemblies.GetType("Baz.Caja.Afectacion.Caja");
                this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Cargó componente de Caja se inicia instancia.");
                this.administradorAplicaciones.ctrBitacora.MostrarEnTrace("Cargó componente de Caja se inicia instancia.");
                caja = Activator.CreateInstance(tipoCaja);
                this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Instancia creada.");
                this.administradorAplicaciones.ctrBitacora.MostrarEnTrace("Instancia creada.");
                TransactionalManager transactionManager = admonLite.ctrAbd.GetTransManager();
                this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Se obtuvo la instancia del TransManager");
                this.administradorAplicaciones.ctrBitacora.MostrarEnTrace("Se obtuvo la instancia del TransManager");
                if (Importe_Total > 0)
                {
                    caja.tipoMovimiento = (int)TipoMovimiento.Egreso;//Egreso
                    caja.concepto = Convert.ToInt32(ConcepMov);
                    caja.Divisa = TipoDivisa;
                    caja.numeroEmpleado = administradorAplicaciones.ctrSesion.IDUsuario;
                    caja.importeTotalOperacion = Importe_Total;
                    caja.importeEfectivo = Convert.ToDecimal(Importe);
                    caja.pedido = 0;//1,
                    caja.presupuesto = 0;//gnPresupuesto,
                    caja.referencia = Referencia;
                    caja.tipoAfectacion = this.tipoAfectacionReversoPagoCheque ? 25 : 1;
                    caja.tipoOperacion = TipodeOperacion;
                    caja.transaccion = IdTransaccionADN;
                    caja.terminal = administradorAplicaciones.ctrSesion.IDEstacionTrabajo;
                    caja.conexion = transactionManager;

                    if (this.tipoAfectacionReversoPagoCheque)
                    {
                        dynamic listaCajaPTiposPago = null;
                        this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Instancia lista dynamic.");
                        this.administradorAplicaciones.ctrBitacora.MostrarEnTrace("Instancia lista dynamic.");
                        var tipoTiposPago = caja.TiposPago.GetType();
                        listaCajaPTiposPago = Activator.CreateInstance(tipoTiposPago);
                        var tipoInTiposPago = caja.TiposPago.GetType().GetGenericArguments();
                        var d = Activator.CreateInstance(tipoInTiposPago[0]);

                        this.administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Concat("Obtiene tipo de dato de la lista. ", tipoInTiposPago[0]));
                        this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(string.Concat("Obtiene tipo de dato de la lista. ", tipoInTiposPago[0]));

                        foreach (TipoPago tipoPago in tiposPago)
                        {
                            this.administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Concat("TipoPago: ", tipoPago.IdTipoPago, " ", tipoPago.Total));
                            this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(string.Concat("TipoPago: ", tipoPago.IdTipoPago, " ", tipoPago.Total));
                            var otjeto = Activator.CreateInstance(tipoInTiposPago[0]);
                            otjeto.IdTipoPago = tipoPago.IdTipoPago;
                            otjeto.Total = tipoPago.Total;
                            listaCajaPTiposPago.Add(otjeto);
                        }
                        this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Realiza reverso.");
                        this.administradorAplicaciones.ctrBitacora.MostrarEnTrace("Realiza reverso.");
                        this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Agrega los tipos de pago al arreglo de caja.");
                        this.administradorAplicaciones.ctrBitacora.MostrarEnTrace("Agrega los tipos de pago al arreglo de caja.");
                        caja.TiposPago = listaCajaPTiposPago;
                        this.administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Concat("Asigna sesion. ", this.sesion));
                        this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(string.Concat("Asigna sesion. ", this.sesion));
                        caja.Sesion = this.sesion;
                        this.administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Concat("Asigna aplicación. ", this.aplicacion));
                        this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(string.Concat("Asigna aplicación. ", this.aplicacion));
                        caja.Aplicacion = this.aplicacion;
                        caja.transaccionConfirmacion = this.transaccionConfirmacion;
                    }

                    if (this.tipoAfectacionReversoPagoCheque)
                    {
                        if (!caja.ReversaChequeCaja() || caja.NoError != 0)
                        {
                            transactionManager.RollBack();
                            RespuestaCajaNoError = (short)caja.NoError;
                            RespuestaCaja = caja.DescipcionError;
                            this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Ocurrió un error con la afectación en caja. Algo no anda bien.");
                            this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(string.Concat("El componente de caja responde con el error: ", caja.NoError, " desc: ", caja.DescipcionError));
                        }
                        else
                        {
                            transactionManager.Commit();
                            RespuestaCajaNoError = (short)caja.NoError;
                            RespuestaCaja = caja.DescipcionError;
                            this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Afectación exitosa.");
                            this.administradorAplicaciones.ctrBitacora.MostrarEnTrace("Afectación exitosa.");
                            respuesta = true;
                        }
                    }
                }
                else
                {
                    RespuestaCaja = "El importe debe ser mayor a 0.";
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void ConfigurarDivisaDeCaja()
        {
            if (!string.IsNullOrEmpty(Divisa))
            {
                switch (Divisa)
                {
                    case "MXP": // Divisa Local                
                        TipoDivisa = (short)eTipoDivisa.eMXP;
                        Esdivisa = false;
                        break;
                    case "USD": // Divisa Extranjera                
                        TipoDivisa = (short)eTipoDivisa.eUSD;
                        Esdivisa = true;
                        break;
                    case "OLP": //OnzaPlata                
                        TipoDivisa = (short)eTipoDivisa.eOLP;
                        Esdivisa = true;
                        break;
                    case "GTQ": //Quetzales                
                        TipoDivisa = (short)eTipoDivisa.eGTQ;
                        Esdivisa = true;
                        break;
                    case "CAD": //Dolar Canadiense                
                        TipoDivisa = (short)eTipoDivisa.eCAD;
                        Esdivisa = true;
                        break;
                    case "GBP": //Libras Esterlinas                
                        TipoDivisa = (short)eTipoDivisa.eGBP;
                        Esdivisa = true;
                        break;
                    case "EUR": //Euros                
                        TipoDivisa = (short)eTipoDivisa.eEUR;
                        Esdivisa = true;
                        break;
                    case "C01": //Centenarios                
                        TipoDivisa = (short)eTipoDivisa.eC01;
                        Esdivisa = true;
                        break;
                    default:
                        throw new ArgumentException("Indique una Divisa válida.");
                }
            }
            else
            {
                throw new ArgumentNullException("La propiedad " + nameof(Divisa) + " no puede ser nula.");
            }
        }

        /// <summary>
        /// Previamente se debe inicializar la propiedad Divisa. Devolverá un booleano al finalizar. 
        /// Depede del tiempo que tarde el cajero en usar el front web de Caja64 BAZ
        /// </summary>
        /// <param name="pdblImporte"> Importe por el cual se quiere verificar el fondeo</param>
        /// <returns></returns>
        public bool FondearCaja(Decimal pdblImporte)
        {
            try
            {
                bool fondeoCaja = false;

                if ((bool)this.administradorAplicaciones.ctrParametro.Obtener(4123))
                {
                    var navFireFoxPersonalizado = new Process();
                    short intentos = 0;
                    ResponseFondeo validaFondeo = ValidaFondeoDeCaja(pdblImporte);

                    while (intentos < 3)
                    {
                        switch (validaFondeo.NoError)
                        {
                            case 0:
                                fondeoCaja = true;
                                intentos = 3;
                                break;
                            case 1:
                                fondeoCaja = false;
                                break;
                            case 3:
                                fondeoCaja = false;
                                throw new Exception(String.Concat("El servicio de Fondeo de Caja devolvió un error: ", validaFondeo.Descripcion));
                        }

                        respuestaCaja = validaFondeo.Descripcion;

                        if (!fondeoCaja)
                        {
                            short platinumParamCustomBrowser = Convert.ToInt16(administradorAplicaciones.ctrParametro.Obtener(3925).ToString());
                            string customFirefoxBrowser = administradorAplicaciones.ctrParametro.Obtener(3926).ToString();
                            string url = string.Concat("http://", nt, ":9014/Caja/Fronts/Fondeo/Fondeo.html?", "divisa=", TipoDivisa, "&monto=", pdblImporte.ToString(), "&usuario=", administradorAplicaciones.ctrControl.Empleado.IdEmpleado, "&ws=", administradorAplicaciones.ctrSesion.IDEstacionTrabajo, "&esWeb=", false);

                            navFireFoxPersonalizado.StartInfo.FileName = customFirefoxBrowser;
                            navFireFoxPersonalizado.StartInfo.Arguments = string.Concat(administradorAplicaciones.ctrSesion.IDUsuario, ",", administradorAplicaciones.ctrSesion.Password, ",", url);//Definir páramero para determinar si se ejecuta en navegador o en aplicación de escritorio
                            navFireFoxPersonalizado.Start();

                            navFireFoxPersonalizado.WaitForExit();


                            if (navFireFoxPersonalizado.HasExited)
                            {
                                validaFondeo = ValidaFondeoDeCaja(pdblImporte);

                                if (validaFondeo.NoError != 0)
                                {
                                    intentos += 1;
                                }
                                else
                                {
                                    intentos = 3;
                                    fondeoCaja = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    fondeoCaja = true;
                }

                return fondeoCaja;
            }
            catch (Exception ex)
            {
                administradorAplicaciones.ctrBitacora.GuardarEnLog(ex.ToString());
                throw;
            }
        }

        private Assembly ResuelveReferenciasEventHandler(object sender, ResolveEventArgs args)
        {
            this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Ingreso al Evento de Resolucion de Referencias");
            AssemblyName[] referencias = null;
            string nombreEnsamblado = string.Empty;
            Assembly respuesta = null;
            this.administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Concat("Entro a resolver los ensamblados de: ", args.Name));
            this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(string.Concat("Carga ensamblado: ", RutaEnsamblados));
            //executingAssemblies = Assembly.LoadFrom(RutaEnsamblados);
            executingAssemblies = Assembly.GetExecutingAssembly(); 
            referencias = executingAssemblies.GetReferencedAssemblies();
            referencias.ToList().ForEach(i => this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(string.Concat("Obtuvo referencia: ", i.FullName)).ToString());
            this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Crea y recorre Array de Referencias");
            foreach (AssemblyName ensamblado in referencias)
            {
                //Compara los nombres
                if (ensamblado.FullName.Substring(0, ensamblado.FullName.IndexOf(",", StringComparison.Ordinal)) == args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.Ordinal)))
                {
                    nombreEnsamblado = args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.Ordinal)) + ".dll";
                    nombreEnsamblado = RutaEnsamblados.Substring(0, 33) + nombreEnsamblado;
                    break;
                }
            }

            //Carga el ensamblado encontrado
            if (!string.IsNullOrEmpty(nombreEnsamblado))
            {
                this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Encontro Referencia " + nombreEnsamblado);
                try
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(this.ResuelveReferenciasEventHandler);
                    this.administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Concat("Cargar ensamblado: ", nombreEnsamblado));
                    this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(string.Concat("Cargar ensamblado: ", nombreEnsamblado));
                    respuesta = Assembly.LoadFrom(nombreEnsamblado);
                }
                catch (Exception ex)
                {
                 //   return Respuesta;
                }
            }
            else
            {
                try
                {
                    this.administradorAplicaciones.ctrBitacora.GuardarEnLog("No Encontro Referencia busca por Nombre de Solicitud" + nombreEnsamblado);
                    if (args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.Ordinal)).ToUpper()
                       .Contains("CAJA."))
                    {
                        
                        nombreEnsamblado = RutaEnsamblados +
                                           args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.Ordinal)) +
                                           ".dll";
                        this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Busca Assembly : " + nombreEnsamblado);

                        //nombreEnsamblado = nombreEnsamblado.ToLower().Replace("xmlserializers.", "").Trim();

                        if (System.IO.File.Exists(nombreEnsamblado))

                        {
                            this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Verificamos que Encuentra ruta de Asembly : " + nombreEnsamblado);
                        }
                        else {
                            this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Problemas, no encuentra la Ruta de Asembly : " + nombreEnsamblado);
                        }

                        respuesta = Assembly.LoadFrom(nombreEnsamblado);
                        this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Encuentra y Carga Asembly : " + nombreEnsamblado);
                    }
                    //AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(this.ResuelveReferenciasEventHandler);
                    //nombreEnsamblado = RutaEnsamblados.Substring(0, 33) + args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.Ordinal)) + ".dll";
                    //this.administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Concat("Cargar ensamblado: ", nombreEnsamblado));
                    //this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(string.Concat("Cargar ensamblado: ", nombreEnsamblado));
                    //respuesta = Assembly.LoadFrom(nombreEnsamblado);

                }
                catch (Exception ex)
                {
                    this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Problemas en la Carga del Ensamblado : " + ex.Message); 

                }
            }

            return respuesta;
        }
        private bool CargaComponentesCaja()
        {
            try
            {
                AssemblyName[] referencias = null;
                string nombreEnsamblado = string.Empty;
                bool respuesta = false;
                executingAssemblies = Assembly.LoadFrom(RutaEnsamblados);
                referencias = executingAssemblies.GetReferencedAssemblies();
                AssemblyName[] currentReferencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
                foreach (AssemblyName ensamblado in referencias)
                {
                    try
                    {
                        respuesta = true;
                        foreach (AssemblyName current in currentReferencedAssemblies)
                        {
                            if (current.Name.Equals(ensamblado.Name))
                            {
                                respuesta = false;
                                break;
                            }
                        }

                        if (respuesta)
                        {
                            nombreEnsamblado = string.Concat(ensamblado.Name, ".dll");
                            nombreEnsamblado = RutaEnsamblados.Substring(0, 33) + nombreEnsamblado;
                            Assembly.LoadFrom(nombreEnsamblado);
                        }
                    }
                    catch
                    {

                    }
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Concat("Error al cargar los ensamblados de caja: ", (ex.InnerException != null) ? string.Concat(ex.Message, " Inner: ", ex.InnerException.Message) : ex.Message));
                throw;
            }
        }
        private Assembly VerificaEnsamblado(Object sender, ResolveEventArgs e)
        {
            Assembly resuelto;
            agregados.TryGetValue(e.Name, out resuelto);
            return resuelto;
        }
        #endregion

        #region Metodos Privados
        private string requestFondeoJSONstr(string usuario, Decimal monto, int divisa, string ws)
        {
            RequestFondeo request = new RequestFondeo(usuario, monto, divisa, ws);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            string _result = jss.Serialize(request);

            return _result;
        }

        #endregion



        private ResponseFondeo ValidaFondeoDeCaja(Decimal importe)
        {
            try
            {
                ConfigurarDivisaDeCaja();
                string url = String.Concat("http://", nt, ":9014/Caja/Servicios/FondeoAutomatico/FondeoAutomatico.svc/wsFondeoValidaSaldo"); //this.administradorAplicaciones.ctrParametro.Obtener(2886).ToString().Trim();

                HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
                req.KeepAlive = false;
                req.Method = "POST";
                req.AllowAutoRedirect = true;
                req.Timeout = 5000;
                req.PreAuthenticate = true;
                req.Credentials = CredentialCache.DefaultCredentials;

                string JSON = this.requestFondeoJSONstr(administradorAplicaciones.ctrControl.Empleado.IdEmpleado, importe, this.TipoDivisa, administradorAplicaciones.ctrSesion.IDEstacionTrabajo);

                byte[] buffer = Encoding.ASCII.GetBytes(JSON);
                req.ContentLength = buffer.Length;
                req.ContentType = "application/json;charset=UTF-8";
                Stream PostData = req.GetRequestStream();
                PostData.Write(buffer, 0, buffer.Length);
                PostData.Close();

                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

                Stream stream = resp.GetResponseStream();

                Encoding encoding = System.Text.Encoding.GetEncoding("utf-8");

                StreamReader streamReader = new StreamReader(stream, encoding);

                string result = streamReader.ReadToEnd();

                streamReader.Close();
                resp.Close();

                ResponseFondeo response = new ResponseFondeo();
                response = this.responseFondeoJSONstr(result);



                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private ResponseFondeo responseFondeoJSONstr(string response)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            ResponseFondeo _response = jss.Deserialize<ResponseFondeo>(response);

            return _response;
        }
    }

    internal class ResponseFondeo
    {
        public string Descripcion { get; set; }
        public string Exe { get; set; }
        public decimal MontoFondeo { get; set; }
        public int NoError { get; set; }

    }
    internal class RequestFondeo
    {
        public string Usuario { get; set; }
        public decimal Monto { get; set; }
        public int Divisa { get; set; }
        public string Ws { get; set; }

        public RequestFondeo(string usuario, Decimal monto, int divisa, string ws)
        {
            Divisa = divisa;
            Usuario = usuario;
            Ws = ws;
            Monto = monto;
        }

    }
}

//private bool esquemaAfectacion;
///// <summary>
///// 
///// </summary>
///// <returns></returns>
//public ObjetoRespuestaCajaCheque MostrarCapturaCheques(decimal ImpTotal, string Ref, short TMovto, int TOP, decimal Presupuesto, short TipoVenta, string IdSesion, int Concepto, string NombreApp, bool EsDeposito)
//{
//    try
//    {
//        var navFireFoxPersonalizado = new Process();
//        var respuestaOperacionCajaCheque = new ObjetoRespuestaCajaCheque();
//        short navPersoParamPlatinum = Convert.ToInt16(administradorAplicaciones.ctrParametro.Obtener(3925).ToString());
//        string strNavFireFoxPersonalizado = administradorAplicaciones.ctrParametro.Obtener(3926).ToString();
//        string urlCheques = string.Concat(administradorAplicaciones.ctrParametro.Obtener(4088).ToString(), administradorAplicaciones.ctrParametro.Obtener(4089).ToString());
//        string url = string.Concat(urlCheques.Replace("|SERVER|", nt),
//                                   EsDeposito ? "" : "ImpTotal=", EsDeposito ? "" : !string.IsNullOrEmpty(ImpTotal.ToString()) ? ImpTotal.ToString() : "0",
//                                   "&NoEmpleado=", this.administradorAplicaciones.ctrSesion.IDUsuario,
//                                   "&Ref=", !string.IsNullOrEmpty(Ref) ? Ref.Replace(" ", "%20") : "",
//                                   "&TMovto=", !string.IsNullOrEmpty(TMovto.ToString()) ? TMovto.ToString() : "1",
//                                   "&TOP=", !string.IsNullOrEmpty(TOP.ToString()) ? TOP.ToString() : "1",
//                                   "&WS=", this.administradorAplicaciones.ctrSesion.IDEstacionTrabajo,
//                                   "&Presupuesto=", !string.IsNullOrEmpty(Presupuesto.ToString()) ? Presupuesto.ToString() : "0",
//                                   "&TipoVenta=", !string.IsNullOrEmpty(TipoVenta.ToString()) ? TipoVenta.ToString() : "1",
//                                   "&IdSesion=", !string.IsNullOrEmpty(IdSesion) ? IdSesion : "",
//                                   "&Concepto=", !string.IsNullOrEmpty(Concepto.ToString()) ? Concepto.ToString() : "632",
//                                   "&NombreApp=", !string.IsNullOrEmpty(NombreApp.ToString()) ? NombreApp.ToString().Replace(" ", "%20") : string.Concat("Captación.Default"),
//                                   EsDeposito ? "&EsDeposito=" : "", EsDeposito ? "true" : ""
//                                  );
//        this.administradorAplicaciones.ctrBitacora.GuardarEnLog(url);
//        this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(url);
//        navFireFoxPersonalizado.StartInfo.FileName = strNavFireFoxPersonalizado;
//        navFireFoxPersonalizado.StartInfo.Arguments = string.Concat(administradorAplicaciones.ctrSesion.IDUsuario, ",", administradorAplicaciones.ctrSesion.Password, ",", url);
//        navFireFoxPersonalizado.Start();
//        navFireFoxPersonalizado.WaitForExit();
//        if (navFireFoxPersonalizado.HasExited)
//        {
//            return InterpretarRespuestaCheques(IdSesion);
//        }
//        return respuestaOperacionCajaCheque;
//    }
//    catch (Exception ex)
//    {
//        administradorAplicaciones.ctrBitacora.GuardarEnLog(ex.ToString());
//        throw;
//    }
//}

//private ObjetoRespuestaCajaCheque InterpretarRespuestaCheques(string idSesion)
//{
//    var datosCheque = new RespuestaOperacionCheque();
//    try
//    {
//        Retry.DoWithRetry(() =>
//        {
//            datosCheque = RecuperaOperacionesCheques(idSesion);
//            if (datosCheque.ListaDocumentos.Count == 0)
//                throw new SinDatosChequeException(string.Concat("No se encontraron registros de la sesión: ", idSesion));
//        }, 3, new TimeSpan(0, 0, 0, 1));
//    }
//    catch (Exception ex)
//    {
//        administradorAplicaciones.ctrBitacora.GuardarEnLog("Error al recuperar los cheques: " + ex.ToString());
//        throw;
//    }

//    var respuesta = new ObjetoRespuestaCajaCheque()
//    {
//        Aplicacion = datosCheque.Aplicacion,
//        EstatusExito = datosCheque.EstatusExito,
//        Mensaje = datosCheque.Mensaje,
//        ListaDocumentos = datosCheque.ListaDocumentos.Select(y => new CAPCajaLite.OperacionesCheques.Documento()
//        {
//            tipoPago = y.tipoPago,
//            bco = y.bco,
//            caveTran = y.caveTran,
//            codSeg = y.codSeg,
//            digInter = y.digInter,
//            importeDocumento = y.importeDocumento.ToString(),
//            numCta = y.numCta,
//            numero = y.numero,
//            numeroDocumento = y.numeroDocumento,
//            plaComp = y.plaComp,
//            preMarc = y.preMarc,
//            nombre = y.nombre,
//            apPat = y.apPat,
//            apMat = y.apMat,
//        }).ToArray()
//    };

//    if (!respuesta.EstatusExito)
//        throw new SinDatosChequeException(string.Concat("No se encontraron registros de la sesión: ", idSesion));
//    return respuesta;
//}

//public bool GuardaChequeDevuelto(string banda, string cuenta, decimal importe, string descripcion)
//{
//    try
//    {
//        string url = String.Concat("http://", nt, ":9014/Caja/Servicios/RechazoCheque/Cheques.svc/ChequesDevolucion/GuardaDevolucionCheque"); //this.administradorAplicaciones.ctrParametro.Obtener(2886).ToString().Trim();

//        HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
//        req.KeepAlive = false;
//        req.Method = "POST";
//        req.AllowAutoRedirect = true;
//        req.Timeout = 5000;
//        req.PreAuthenticate = true;
//        req.Credentials = CredentialCache.DefaultCredentials;

//        string JSON = this.ChequeDevueltoJSONstr(banda, cuenta, importe, descripcion);

//        byte[] buffer = Encoding.ASCII.GetBytes(JSON);
//        req.ContentLength = buffer.Length;
//        req.ContentType = "application/json;charset=UTF-8";
//        Stream PostData = req.GetRequestStream();
//        PostData.Write(buffer, 0, buffer.Length);
//        PostData.Close();

//        HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

//        Stream stream = resp.GetResponseStream();

//        Encoding encoding = System.Text.Encoding.GetEncoding("utf-8");

//        StreamReader streamReader = new StreamReader(stream, encoding);

//        string result = streamReader.ReadToEnd();

//        streamReader.Close();
//        resp.Close();

//        ResponseChequeDevuelto response = new ResponseChequeDevuelto();
//        response = this.responseChequeDevueltoJSONstr(result);

//        if (response.NoError != 0)
//        {

//            this.administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Concat("El gurado respondó con el error: ", response.Descripcion));
//            this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(string.Concat("El gurado respondó con el error: ", response.Descripcion));
//            throw new Exception(response.Descripcion);
//        }
//        else
//            return true;
//    }
//    catch (Exception ex)
//    {
//        throw;
//    }
//}

//public ObjetoRespuestaCajaCheque MostrarDevolucionCheques(string IdSesion, string Banda, string cuenta, decimal ImpTotal, string status, string NombreApp)
//{
//    try
//    {
//        this.administradorAplicaciones.ctrBitacora.GuardarLog = true;
//        Process navFireFoxPersonalizado = new Process();
//        ObjetoRespuestaCajaCheque respuestaOperacionCajaCheque = new ObjetoRespuestaCajaCheque();
//        short navPersoParamPlatinum = Convert.ToInt16(administradorAplicaciones.ctrParametro.Obtener(3925).ToString());
//        this.administradorAplicaciones.ctrBitacora.GuardarEnLog(navPersoParamPlatinum.ToString());
//        this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(navPersoParamPlatinum.ToString());
//        string strNavFireFoxPersonalizado = administradorAplicaciones.ctrParametro.Obtener(3926).ToString();
//        this.administradorAplicaciones.ctrBitacora.GuardarEnLog(strNavFireFoxPersonalizado.ToString());
//        this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(strNavFireFoxPersonalizado.ToString());
//        string urlDevolucionCheques = string.Concat(administradorAplicaciones.ctrParametro.Obtener(4090).ToString(), administradorAplicaciones.ctrParametro.Obtener(4091).ToString());
//        this.administradorAplicaciones.ctrBitacora.GuardarEnLog(urlDevolucionCheques.ToString());
//        this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(urlDevolucionCheques.ToString());
//        DateTime dateTime = DateTime.Now;
//        string url = string.Concat(urlDevolucionCheques.Replace("|SERVER|", nt),
//                                   "NoEmpleado=", this.administradorAplicaciones.ctrSesion.IDUsuario,
//                                   "&WS=", this.administradorAplicaciones.ctrSesion.IDEstacionTrabajo,
//                                   "&IdSesion=", !string.IsNullOrEmpty(IdSesion) ? IdSesion : string.Concat("Cfaf~pt9a_", dateTime.Day.ToString(), dateTime.Month.ToString(), dateTime.Year.ToString("yyyy"), dateTime.Hour.ToString("HH"), dateTime.Minute.ToString(), dateTime.Second.ToString(), this.administradorAplicaciones.ctrSesion.IDEstacionTrabajo),
//                                   "&Banda=", !string.IsNullOrEmpty(Banda) ? Banda : "",
//                                   "&Cuenta=", !string.IsNullOrEmpty(cuenta) ? cuenta : "1",
//                                   "&ImpTotal=", !string.IsNullOrEmpty(ImpTotal.ToString()) ? ImpTotal.ToString() : "1",
//                                   "&Status=", !string.IsNullOrEmpty(status) ? status : "0",
//                                   "&NombreApp=", !string.IsNullOrEmpty(NombreApp) ? NombreApp : "CapCajaLite.dll");

//        navFireFoxPersonalizado.StartInfo.FileName = strNavFireFoxPersonalizado;
//        this.administradorAplicaciones.ctrBitacora.GuardarEnLog(url);
//        //this.administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Concat(administradorAplicaciones.ctrSesion.IDUsuario, ",", administradorAplicaciones.ctrSesion.Password, ",", url));
//        this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(url);
//        navFireFoxPersonalizado.StartInfo.Arguments = string.Concat(administradorAplicaciones.ctrSesion.IDUsuario, ",", administradorAplicaciones.ctrSesion.Password, ",", url);
//        navFireFoxPersonalizado.Start();

//        navFireFoxPersonalizado.WaitForExit();

//        if (navFireFoxPersonalizado.HasExited)
//            return InterpretarRespuestaCheques(IdSesion);
//        return respuestaOperacionCajaCheque;
//    }
//    catch (Exception ex)
//    {
//        throw;
//    }
//}

///// <summary>
///// 
///// </summary>
///// <returns></returns>
//public bool ValidarAvisosCajero()
//{
//    try
//    {

//        this.administradorAplicaciones.ctrBitacora.GuardarEnLog("ValidarAvisosCajero()");
//        //this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Instanciando la clase AbdCaja");

//        this.administradorAplicaciones.ctrBitacora.GuardarEnLog("Validando los Avisos del Cajero tipo de pago " + this.IdTipoPago);

//        //this.administradorAplicaciones.ctrBitacora.GuardarEnLog("ValidarAvisosCajero()");
//        string lstrSql = string.Concat("Exec ", "spAhoConAvisosCajero ", "'", administradorAplicaciones.ctrControl.Empleado.IdEmpleado, "',", IdTipoPago.ToString());
//        this.administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Concat("Ejecutando Sql ", lstrSql));
//        DataTable dt = new DataTable();
//        if (!administradorAplicaciones.ctrAbd.Ejecutar(lstrSql))
//            return false;
//        if (administradorAplicaciones.ctrAbd.Registros() == 0)
//            return false;
//        string idRegistro = administradorAplicaciones.ctrAbd.Dato("Respuesta").ToString().Trim();
//        this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(administradorAplicaciones.ctrAbd.Dato("Respuesta").ToString().Trim());
//        return Convert.ToBoolean(Convert.ToInt16(idRegistro));
//    }
//    catch (Exception ex)
//    {
//        administradorAplicaciones.ctrBitacora.GuardarEnLog(ex.ToString());
//        throw;
//    }
//}

////public ValidaAccesoUsuario ValidaAcceso()
////{
////    try
////    {
////        AccesoClient solicitaAcceso = new AccesoClient();

////        ValidaAccesoUsuario validaAccesoUsuario = new ValidaAccesoUsuario();

////        Respuesta respuesta = solicitaAcceso.SolicitaAcceso(this.administradorAplicaciones.ctrSesion.IDUsuario, "", this.administradorAplicaciones.ctrControl.Terminal);

////        validaAccesoUsuario.AccesoConcedido = respuesta.AccesoConcedido;
////        validaAccesoUsuario.AccesoCobrar = respuesta.AccesoCobrar;
////        validaAccesoUsuario.Aplicacion = respuesta.Aplicacion;
////        validaAccesoUsuario.Mensaje = respuesta.Mensaje;
////        validaAccesoUsuario.MinutosRestantesRendicion = respuesta.MinutosRestantesRendicion;
////        validaAccesoUsuario.ParametrosAdicionales = respuesta.ParametrosAdicionales;
////        validaAccesoUsuario.ParametrosAplicacion = respuesta.ParametrosAplicacion;
////        validaAccesoUsuario.TipoAplicacion = respuesta.TipoAplicacion;

////        return validaAccesoUsuario;
////    }
////    catch (Exception ex)
////    {
////        throw new InvalidDataException(string.Concat("Ocurrió un error con la validación de acceso para el usuario: ", ex.Message, (ex.InnerException != null) ? string.Concat(" InnerException: ", ex.InnerException.Message) : ""));
////    }
////}

///// <summary>
///// Previamente se debe inicializar la propiedad Divisa. Devolverá un booleano al finalizar. 
///// Depede del tiempo que tarde el cajero en usar el front web de Caja64 BAZ
///// </summary>
///// <param name="pdblImporte"> Importe por el cual se quiere verificar el fondeo</param>
///// <returns></returns>
//public bool FondearCaja(Decimal pdblImporte)
//{
//    try
//    {
//        bool fondeoCaja = false;

//        if ((bool)this.administradorAplicaciones.ctrParametro.Obtener(4123))
//        {
//            var navFireFoxPersonalizado = new Process();
//            short intentos = 0;
//            ResponseFondeo validaFondeo = ValidaFondeoDeCaja(pdblImporte);

//            while (intentos < 3)
//            {
//                switch (validaFondeo.NoError)
//                {
//                    case 0:
//                        fondeoCaja = true;
//                        intentos = 3;
//                        break;
//                    case 1:
//                        fondeoCaja = false;
//                        break;
//                    case 3:
//                        fondeoCaja = false;
//                        throw new Exception(String.Concat("El servicio de Fondeo de Caja devolvió un error: ", validaFondeo.Descripcion));
//                }

//                respuestaCaja = validaFondeo.Descripcion;

//                if (!fondeoCaja)
//                {
//                    short platinumParamCustomBrowser = Convert.ToInt16(administradorAplicaciones.ctrParametro.Obtener(3925).ToString());
//                    string customFirefoxBrowser = administradorAplicaciones.ctrParametro.Obtener(3926).ToString();
//                    string url = string.Concat("http://", nt, ":9014/Caja/Fronts/Fondeo/Fondeo.html?", "divisa=", TipoDivisa, "&monto=", pdblImporte.ToString(), "&usuario=", administradorAplicaciones.ctrControl.Empleado.IdEmpleado, "&ws=", administradorAplicaciones.ctrSesion.IDEstacionTrabajo, "&esWeb=", false);

//                    navFireFoxPersonalizado.StartInfo.FileName = customFirefoxBrowser;
//                    navFireFoxPersonalizado.StartInfo.Arguments = string.Concat(administradorAplicaciones.ctrSesion.IDUsuario, ",", administradorAplicaciones.ctrSesion.Password, ",", url);//Definir páramero para determinar si se ejecuta en navegador o en aplicación de escritorio
//                    navFireFoxPersonalizado.Start();

//                    navFireFoxPersonalizado.WaitForExit();


//                    if (navFireFoxPersonalizado.HasExited)
//                    {
//                        validaFondeo = ValidaFondeoDeCaja(pdblImporte);

//                        if (validaFondeo.NoError != 0)
//                        {
//                            intentos += 1;
//                        }
//                        else
//                        {
//                            intentos = 3;
//                            fondeoCaja = true;
//                        }
//                    }
//                }
//            }
//        }
//        else
//        {
//            fondeoCaja = true;
//        }

//        return fondeoCaja;
//    }
//    catch (Exception ex)
//    {
//        administradorAplicaciones.ctrBitacora.GuardarEnLog(ex.ToString());
//        throw;
//    }
//}

///// <summary>
///// 
///// </summary>
///// <returns></returns>
////public bool RegistraImagenDevolucionChequesBAZ()
////{
////    try
////    {
////        //this.administradorAplicaciones.ctrBitacora.GuardarEnLog( "RegistraImagenDevolucionChequesBAZ";
////        //this.administradorAplicaciones.ctrBitacora.GuardarEnLog( "Ejecutando el método RegistraImagenDevolucionChequesBAZ de Caja";
////        ////System.Windows.Forms.Application.DoEvents();
////        //RegistraImagenDevolucionChequesBAZ = Caja.RegistraImagenDevolucionChequesBAZ;
////        throw new NotImplementedException("No se ha desarrollado la parte final de caja para cheques.");
////    }
////    catch (Exception ex)
////    {
////        //Err.Clear();
////        throw;
////    }
////}

///// <summary>
///// 
///// </summary>
///// <returns></returns>
//public bool ObtenDenominacion()
//{
//    try
//    {
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(" *** Inicia Configuración Obtener Denominaciones ***");
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".UsrCaja = " + administradorAplicaciones.ctrSesion.IDUsuario);
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".WS = " + administradorAplicaciones.ctrSesion.IDEstacionTrabajo);
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".TipoOperacion = " + TipodeOperacion);
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".ConcepMov = 0 - Fijo");
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".Referencia = - Fijo");
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".lsNotienda = " + administradorAplicaciones.ctrControl.IdSucursal);
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".liNegocio = 1 - Fijo");
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".Esdivisa = False - Fijo");
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".liImpTotalVta = " + string.Format("##############0.00", Importe));
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".Importe_Total = " + string.Format("##############0.00", Importe));
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".gnImporte = " + Importe);
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".Importe_Efectivo =" + string.Format("##############0.00", Importe));
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".lsTipoVta = 1  - Fijo - 1=Contado, 2=Credito");
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".gnPresupuesto = 0 - Fijo");
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(".NumeroTransthis.administradorAplicaciones.ctrBitacora.GuardarEnLog( 0");

//        this.ConcepMov = "0";

//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(" *** Termina Configuraciòn Obtener Denominaciones ***");
//        if (TipodeOperacion == 388 & IdTipoPago != 1)
//        {
//        }
//        else
//        {
//            administradorAplicaciones.ctrBitacora.MostrarEnTrace(" Invoca RegistraDenominaciones");

//        }

//        administradorAplicaciones.ctrBitacora.MostrarEnTrace(" Termina RegistraDenominaciones de manera correcta");

//        return true;
//    }
//    catch (Exception ex)
//    {
//        administradorAplicaciones.ctrBitacora.MostrarEnTrace("Error en función ObtenDenominacion");
//        administradorAplicaciones.ctrBitacora.GuardarEnLog(ex.ToString());
//        throw;
//    }
//}

///// <summary>
///// Especificar la propiedad: TipoPago para hacer la ejecución correctamente
///// Si es 'true' se encuentra topada la caja y no se podrán hacer afectaciones
///// </summary>
///// <returns></returns>
//public bool ValidarTopeCaja()
//{
//    try
//    {
//        if (this.IdTipoPago != 0)
//        {
//            //this.administradorAplicaciones.ctrBitacora.GuardarEnLog("ValidarTopesCaja()");

//            string lstrSql = string.Concat("Exec ", "PACJCCValidaTopeEmpleado ", "'", administradorAplicaciones.ctrControl.Empleado.IdEmpleado, "',", this.TipoDivisa, ",", IdTipoPago);
//            this.administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Concat("Ejecutando Sql ", lstrSql));

//            DataTable dt = new DataTable();
//            if (administradorAplicaciones.ctrAbd.Ejecutar(lstrSql))
//            {
//                if (administradorAplicaciones.ctrAbd.Registros() > 0)
//                {
//                    string idRegistro = administradorAplicaciones.ctrAbd.Dato("TopeExcedido").ToString().Trim();
//                    //this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(administradorAplicaciones.ctrAbd.Dato("TopeExcedido").ToString().Trim());
//                    return Convert.ToBoolean(idRegistro);
//                }
//                else
//                {
//                    throw new DataException("No se resolvió adecuadamente la respuesta del sp PACJCCValidaTopeEmpleado en la base de datos.");
//                }
//            }
//            else
//            {
//                throw new MissingFieldException("No fue posible ejecutar la consulta en la base de datos.");
//            }
//        }
//        else
//        {
//            throw new MissingFieldException("Se debe indicar el tipo de pago para validar el tope de la caja.");
//        }
//    }
//    catch (Exception ex)
//    {
//        throw;
//    }
//}

//public bool AvisarAGerente()
//{
//    try
//    {
//        if (this.Importe > 0)
//        {
//            string comandoSql = string.Concat("EXEC PACJCFMensajesValida 1,", administradorAplicaciones.ctrControl.Empleado.IdEmpleado, ",", this.Importe);
//            administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Concat("Validando: ", comandoSql));
//            if (administradorAplicaciones.ctrAbd.Ejecutar(comandoSql))
//            {
//                if (administradorAplicaciones.ctrAbd.Registros() > 0)
//                {
//                    //this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(administradorAplicaciones.ctrAbd.Registros().ToString());
//                    return true;
//                }
//                else
//                {
//                    throw new DataException("No se resolvió adecuadamente la respuesta del sp PACJCFMensajesValida en la base de datos.");
//                }
//            }
//            else
//            {
//                throw new MissingFieldException("No fue posible ejecutar la consulta en la base de datos.");
//            }
//        }
//        else
//        {
//            throw new MissingFieldException("El Importe debe ser mayor a 0 para validar los mensajes.");
//        }
//    }
//    catch (Exception ex)
//    {
//        throw;
//    }
//}


//public string ValidarBloqueoEmpleadosCaja()
//{
//    try
//    {
//        this.ConfigurarDivisaDeCaja();
//        if (this.TipoDivisa != 0)
//        {
//            if (this.IdTipoPago != 0)
//            {
//                string comandoSql = string.Concat("EXEC PACJCFLUConBloqueo ", this.TipoDivisa, ",", this.IdTipoPago, ",", administradorAplicaciones.ctrControl.Empleado.IdEmpleado);
//                administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Concat("Validando: ", comandoSql));
//                if (administradorAplicaciones.ctrAbd.Ejecutar(comandoSql))
//                {
//                    if (administradorAplicaciones.ctrAbd.Registros() > 0)
//                    {
//                        DataTable dt = (DataTable)administradorAplicaciones.ctrAbd.DataTable();
//                        this.respuestaCaja = administradorAplicaciones.ctrAbd.Dato("Mensaje").ToString().Trim();
//                        //this.administradorAplicaciones.ctrBitacora.MostrarEnTrace(administradorAplicaciones.ctrAbd.Dato("Mensaje").ToString().Trim());
//                        return administradorAplicaciones.ctrAbd.Dato("Respuesta").ToString().Trim();
//                    }
//                    else
//                    {
//                        throw new DataException("No se resolvió adecuadamente la respuesta del sp PACJCFMensajesValida en la base de datos.");
//                    }
//                }
//                else
//                {
//                    throw new MissingFieldException("No fue posible ejecutar la consulta en la base de datos.");
//                }
//            }
//            else
//            {
//                throw new MissingFieldException("Se debe indicar el tipo de pago para validar si el usuario se encuentra bloqueado.");
//            }
//        }
//        else
//        {
//            throw new MissingFieldException("Se debe indicar la divisa para validar si el usuario se encuentra bloqueado.");
//        }
//    }
//    catch (Exception ex)
//    {
//        throw;
//    }
//}
//#endregion

//#region Métodos Privados

//private string requestFondeoJSONstr(string usuario, Decimal monto, int divisa, string ws)
//{
//    RequestFondeo request = new RequestFondeo(usuario, monto, divisa, ws);
//    JavaScriptSerializer jss = new JavaScriptSerializer();
//    string _result = jss.Serialize(request);

//    return _result;
//}

//private string ChequeDevueltoJSONstr(string banda, string cuenta, decimal importe, string descripcion)
//{
//    ChequeDevuelto request = new ChequeDevuelto(banda, cuenta, importe, descripcion);
//    JavaScriptSerializer jss = new JavaScriptSerializer();
//    string _result = jss.Serialize(request);
//    return _result;
//}

//private ResponseFondeo responseFondeoJSONstr(string response)
//{
//    JavaScriptSerializer jss = new JavaScriptSerializer();
//    ResponseFondeo _response = jss.Deserialize<ResponseFondeo>(response);

//    return _response;
//}

//private ResponseChequeDevuelto responseChequeDevueltoJSONstr(string response)
//{
//    JavaScriptSerializer jss = new JavaScriptSerializer();
//    ResponseChequeDevuelto _response = jss.Deserialize<ResponseChequeDevuelto>(response);

//    return _response;
//}

//private ResponseFondeo ValidaFondeoDeCaja(Decimal importe)
//{
//    try
//    {
//        ConfigurarDivisaDeCaja();
//        string url = String.Concat("http://", nt, ":9014/Caja/Servicios/FondeoAutomatico/FondeoAutomatico.svc/wsFondeoValidaSaldo"); //this.administradorAplicaciones.ctrParametro.Obtener(2886).ToString().Trim();

//        HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
//        req.KeepAlive = false;
//        req.Method = "POST";
//        req.AllowAutoRedirect = true;
//        req.Timeout = 5000;
//        req.PreAuthenticate = true;
//        req.Credentials = CredentialCache.DefaultCredentials;

//        string JSON = this.requestFondeoJSONstr(administradorAplicaciones.ctrControl.Empleado.IdEmpleado, importe, this.TipoDivisa, administradorAplicaciones.ctrSesion.IDEstacionTrabajo);

//        byte[] buffer = Encoding.ASCII.GetBytes(JSON);
//        req.ContentLength = buffer.Length;
//        req.ContentType = "application/json;charset=UTF-8";
//        Stream PostData = req.GetRequestStream();
//        PostData.Write(buffer, 0, buffer.Length);
//        PostData.Close();

//        HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

//        Stream stream = resp.GetResponseStream();

//        Encoding encoding = System.Text.Encoding.GetEncoding("utf-8");

//        StreamReader streamReader = new StreamReader(stream, encoding);

//        string result = streamReader.ReadToEnd();

//        streamReader.Close();
//        resp.Close();

//        ResponseFondeo response = new ResponseFondeo();
//        response = this.responseFondeoJSONstr(result);



//        return response;
//    }
//    catch (Exception ex)
//    {
//        throw;
//    }
//}

//[Obsolete("Hombres trabajando por un mundo mejor, mejoras para cambiar de chueques a cheques", false)]
//private ObjetoRespuestaCajaCheque ValidaRespuestaOperacionesChueques(string IdSesion)
//{
//    try
//    {
//        CAPOperacionesChequesClient cheques = new CAPOperacionesChequesClient();

//        ObjetoRespuestaCajaCheque respuestaOperacionCheque = new ObjetoRespuestaCajaCheque();

//        respuestaOperacionCheque = cheques.ConsultaOperacionesCheques(IdSesion);

//        return respuestaOperacionCheque;
//    }
//    catch (Exception ex)
//    {
//        throw;
//    }
//}

//private RespuestaOperacionCheque RecuperaOperacionesCheques(string idSesion)
//{
//    var objetoRespuestacajaCheque = new RespuestaOperacionCheque();
//    try
//    {
//        if (string.IsNullOrWhiteSpace(idSesion))
//            throw new ArgumentNullException("El número de sesión no puede estar vacío. Informa uno para hacer la consulta.");
//        string queryConsultaCheques = string.Concat("EXEC spConCAPDatosCheques '", idSesion, "'");
//        if (!administradorAplicaciones.ctrAbd.Ejecutar(queryConsultaCheques))
//            throw new NullReferenceException("No fue posible ejecutar la consulta del guardado de los datos sobre la operación con cheques.");
//        if (administradorAplicaciones.ctrAbd.Registros() == 0)
//            throw new SinDatosChequeException(string.Concat("No se encontraron registros de la sesión: ", idSesion));
//        DataSet resultadoBusqueda = (DataSet)administradorAplicaciones.ctrAbd.DataSet();
//        if (resultadoBusqueda == null)
//            return objetoRespuestacajaCheque;
//        if (resultadoBusqueda.Tables.Count == 0)
//            return objetoRespuestacajaCheque;
//        administradorAplicaciones.ctrBitacora.GuardarEnLog("Obteniendo los valores de la operación con cheques.");
//        objetoRespuestacajaCheque.EstatusExito = (resultadoBusqueda.Tables[0].Rows[0]["fiEstatus"].ToString().Equals("1") ? true : false);
//        objetoRespuestacajaCheque.Aplicacion = resultadoBusqueda.Tables[0].Rows[0]["fcAplicacion"].ToString();
//        objetoRespuestacajaCheque.Mensaje = resultadoBusqueda.Tables[0].Rows[0]["fcMensaje"].ToString();
//        var doctos = new List<Documento>();
//        DataTable dtDoctos = resultadoBusqueda.Tables[1];
//        if (dtDoctos.Rows.Count > 0)
//        {
//            int max = Convert.ToInt32(dtDoctos.Compute("MAX(fiNumDoctoId)", ""));
//            administradorAplicaciones.ctrBitacora.GuardarEnLog(string.Format("Número de documentos de la sesión [{0}]: {1}", idSesion, max));
//            for (int i = 1; i <= Convert.ToInt32(max); i++)
//            {
//                Documento doc = new Documento();
//                List<DataRow> lstDr = dtDoctos.Select("fiNumDoctoId=" + i).ToList();//dtDoctos.AsEnumerable().Where(row => row.Field<int>("fiNumDoctoId") == i).ToList();

//                foreach (DataRow dr in lstDr)
//                {
//                    foreach (PropertyInfo drdetalle in doc.GetType().GetProperties().ToList())
//                    {
//                        if (dr.Field<string>("fcDescripcion").Equals(drdetalle.Name))
//                            drdetalle.SetValue(doc, Convert.ChangeType(dr["fcValor"].ToString(), drdetalle.PropertyType), null);
//                    }
//                }
//                doctos.Add(doc);
//            }
//            objetoRespuestacajaCheque.ListaDocumentos = doctos;
//        }
//        return objetoRespuestacajaCheque;
//    }
//    catch (Exception)
//    {
//        throw;
//    }
//}

//    #region Public classes
//    public class ValidaAccesoUsuario
//    {
//        private bool accesoConcedido;
//        private string mensaje;
//        private bool accesoCobrar;
//        private string aplicacion;
//        private int minutosRestantesRendicion;
//        private string parametrosAdicionales;
//        private string parametrosAplicacion;
//        private int tipoAplicacion;

//        public bool AccesoConcedido
//        {
//            get { return accesoConcedido; }
//            set { accesoConcedido = value; }
//        }

//        public string Mensaje
//        {
//            get { return mensaje; }
//            set { mensaje = value; }
//        }

//        public bool AccesoCobrar
//        {
//            get { return accesoCobrar; }
//            set { accesoCobrar = value; }
//        }

//        public string Aplicacion
//        {
//            get { return aplicacion; }
//            set { aplicacion = value; }
//        }

//        public int MinutosRestantesRendicion
//        {
//            get { return minutosRestantesRendicion; }
//            set { minutosRestantesRendicion = value; }
//        }

//        public string ParametrosAdicionales
//        {
//            get { return parametrosAdicionales; }
//            set { parametrosAdicionales = value; }
//        }

//        public string ParametrosAplicacion
//        {
//            get { return parametrosAplicacion; }
//            set { parametrosAplicacion = value; }
//        }

//        public int TipoAplicacion
//        {
//            get { return tipoAplicacion; }
//            set { tipoAplicacion = value; }
//        }
//    }

//    public class RespuestaOperacionCheque
//    {
//        private bool estatusExito;
//        private string mensaje;
//        private string aplicacion;
//        private List<Documento> listaDocumentos;

//        public bool EstatusExito
//        {
//            get { return estatusExito; }
//            set { estatusExito = value; }
//        }

//        public string Mensaje
//        {
//            get { return mensaje; }
//            set { mensaje = value; }
//        }

//        public string Aplicacion
//        {
//            get { return aplicacion; }
//            set { aplicacion = value; }
//        }

//        public List<Documento> ListaDocumentos
//        {
//            get { return listaDocumentos; }
//            set { listaDocumentos = value; }
//        }

//        public RespuestaOperacionCheque()
//        {
//            ListaDocumentos = new List<Documento>();
//        }
//    }

//    public class Documento
//    {
//        private string TipoPago;
//        private string Bco;
//        private string CaveTran;
//        private int CodSeg;
//        private short DigInter;
//        private decimal ImporteDocumento;
//        private string NumCta;
//        private string Numero;
//        private string NumeroDocumento;
//        private int PlaComp;
//        private int PreMarc;
//        private string Nombre;
//        private string ApPat;
//        private string ApMat;

//        public string tipoPago
//        {
//            get { return TipoPago; }
//            set { TipoPago = value; }
//        }

//        public string bco
//        {
//            get { return Bco; }
//            set { Bco = value; }
//        }

//        public string caveTran
//        {
//            get { return CaveTran; }
//            set { CaveTran = value; }
//        }

//        public int codSeg
//        {
//            get { return CodSeg; }
//            set { CodSeg = value; }
//        }

//        public short digInter
//        {
//            get { return DigInter; }
//            set { DigInter = value; }
//        }

//        public decimal importeDocumento
//        {
//            get { return ImporteDocumento; }
//            set { ImporteDocumento = value; }
//        }

//        public string numCta
//        {
//            get { return NumCta; }
//            set { NumCta = value; }
//        }

//        public string numero
//        {
//            get { return Numero; }
//            set { Numero = value; }
//        }

//        public string numeroDocumento
//        {
//            get { return NumeroDocumento; }
//            set { NumeroDocumento = value; }
//        }

//        public int plaComp
//        {
//            get { return PlaComp; }
//            set { PlaComp = value; }
//        }

//        public int preMarc
//        {
//            get { return PreMarc; }
//            set { PreMarc = value; }
//        }

//        public string nombre
//        {
//            get { return Nombre; }
//            set { Nombre = value; }
//        }

//        public string apPat
//        {
//            get { return ApPat; }
//            set { ApPat = value; }
//        }

//        public string apMat
//        {
//            get { return ApMat; }
//            set { ApMat = value; }
//        }
//    }
//    #endregion

//    #region Internal Classes
//    internal class RequestFondeo
//    {
//        public string Usuario { get; set; }
//        public decimal Monto { get; set; }
//        public int Divisa { get; set; }
//        public string Ws { get; set; }

//        public RequestFondeo(string usuario, Decimal monto, int divisa, string ws)
//        {
//            Divisa = divisa;
//            Usuario = usuario;
//            Ws = ws;
//            Monto = monto;
//        }

//    }

//    internal class ChequeDevuelto
//    {
//        public string BandaCheque { get; set; }
//        public string CuentaCheque { get; set; }
//        public decimal Importe { get; set; }
//        public string DescripcionStatus { get; set; }

//        public ChequeDevuelto(string bandaCheque, string cuentaCheque, decimal importe, string descripcionStatus)
//        {
//            this.BandaCheque = bandaCheque;
//            this.CuentaCheque = cuentaCheque;
//            this.Importe = importe;
//            this.DescripcionStatus = descripcionStatus;
//        }
//    }

//    internal class ResponseFondeo
//    {
//        public string Descripcion { get; set; }
//        public string Exe { get; set; }
//        public decimal MontoFondeo { get; set; }
//        public int NoError { get; set; }

//    }

//    internal class ResponseChequeDevuelto
//    {
//        public int NoError { get; set; }
//        public string Descripcion { get; set; }
//        public bool Respuesta { get; set; }
//    }

//    #endregion
//}
