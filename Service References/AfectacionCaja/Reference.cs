﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CAPCajaLite.AfectacionCaja {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ObjetoAfectacionCaja", Namespace="http://schemas.datacontract.org/2004/07/CAPAfectaCajaService")]
    [System.SerializableAttribute()]
    public partial class ObjetoAfectacionCaja : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AplicacionField;
        
        private int ConceptoField;
        
        private int DivisaField;
        
        private string EmpleadoField;
        
        private decimal ImporteField;
        
        private decimal ImporteEfectivoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int PedidoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int PresupuestoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ReferenciaField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SesionField;
        
        private int TipoAfectacionField;
        
        private int TipoMovimientoField;
        
        private int TopField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int TopEgresoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CAPCajaLite.AfectacionCaja.TopsAfectacion[] TopsAfectacionField;
        
        private int TransaccionField;
        
        private string WsField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Aplicacion {
            get {
                return this.AplicacionField;
            }
            set {
                if ((object.ReferenceEquals(this.AplicacionField, value) != true)) {
                    this.AplicacionField = value;
                    this.RaisePropertyChanged("Aplicacion");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Concepto {
            get {
                return this.ConceptoField;
            }
            set {
                if ((this.ConceptoField.Equals(value) != true)) {
                    this.ConceptoField = value;
                    this.RaisePropertyChanged("Concepto");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Divisa {
            get {
                return this.DivisaField;
            }
            set {
                if ((this.DivisaField.Equals(value) != true)) {
                    this.DivisaField = value;
                    this.RaisePropertyChanged("Divisa");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Empleado {
            get {
                return this.EmpleadoField;
            }
            set {
                if ((object.ReferenceEquals(this.EmpleadoField, value) != true)) {
                    this.EmpleadoField = value;
                    this.RaisePropertyChanged("Empleado");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public decimal Importe {
            get {
                return this.ImporteField;
            }
            set {
                if ((this.ImporteField.Equals(value) != true)) {
                    this.ImporteField = value;
                    this.RaisePropertyChanged("Importe");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public decimal ImporteEfectivo {
            get {
                return this.ImporteEfectivoField;
            }
            set {
                if ((this.ImporteEfectivoField.Equals(value) != true)) {
                    this.ImporteEfectivoField = value;
                    this.RaisePropertyChanged("ImporteEfectivo");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Pedido {
            get {
                return this.PedidoField;
            }
            set {
                if ((this.PedidoField.Equals(value) != true)) {
                    this.PedidoField = value;
                    this.RaisePropertyChanged("Pedido");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Presupuesto {
            get {
                return this.PresupuestoField;
            }
            set {
                if ((this.PresupuestoField.Equals(value) != true)) {
                    this.PresupuestoField = value;
                    this.RaisePropertyChanged("Presupuesto");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Referencia {
            get {
                return this.ReferenciaField;
            }
            set {
                if ((object.ReferenceEquals(this.ReferenciaField, value) != true)) {
                    this.ReferenciaField = value;
                    this.RaisePropertyChanged("Referencia");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Sesion {
            get {
                return this.SesionField;
            }
            set {
                if ((object.ReferenceEquals(this.SesionField, value) != true)) {
                    this.SesionField = value;
                    this.RaisePropertyChanged("Sesion");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int TipoAfectacion {
            get {
                return this.TipoAfectacionField;
            }
            set {
                if ((this.TipoAfectacionField.Equals(value) != true)) {
                    this.TipoAfectacionField = value;
                    this.RaisePropertyChanged("TipoAfectacion");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int TipoMovimiento {
            get {
                return this.TipoMovimientoField;
            }
            set {
                if ((this.TipoMovimientoField.Equals(value) != true)) {
                    this.TipoMovimientoField = value;
                    this.RaisePropertyChanged("TipoMovimiento");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Top {
            get {
                return this.TopField;
            }
            set {
                if ((this.TopField.Equals(value) != true)) {
                    this.TopField = value;
                    this.RaisePropertyChanged("Top");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TopEgreso {
            get {
                return this.TopEgresoField;
            }
            set {
                if ((this.TopEgresoField.Equals(value) != true)) {
                    this.TopEgresoField = value;
                    this.RaisePropertyChanged("TopEgreso");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CAPCajaLite.AfectacionCaja.TopsAfectacion[] TopsAfectacion {
            get {
                return this.TopsAfectacionField;
            }
            set {
                if ((object.ReferenceEquals(this.TopsAfectacionField, value) != true)) {
                    this.TopsAfectacionField = value;
                    this.RaisePropertyChanged("TopsAfectacion");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Transaccion {
            get {
                return this.TransaccionField;
            }
            set {
                if ((this.TransaccionField.Equals(value) != true)) {
                    this.TransaccionField = value;
                    this.RaisePropertyChanged("Transaccion");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Ws {
            get {
                return this.WsField;
            }
            set {
                if ((object.ReferenceEquals(this.WsField, value) != true)) {
                    this.WsField = value;
                    this.RaisePropertyChanged("Ws");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TopsAfectacion", Namespace="http://schemas.datacontract.org/2004/07/CAPAfectaCajaService")]
    [System.SerializableAttribute()]
    public partial class TopsAfectacion : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string ReferenciaField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CAPCajaLite.AfectacionCaja.TipoPago[] TiposPagoField;
        
        private short TopField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Referencia {
            get {
                return this.ReferenciaField;
            }
            set {
                if ((object.ReferenceEquals(this.ReferenciaField, value) != true)) {
                    this.ReferenciaField = value;
                    this.RaisePropertyChanged("Referencia");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CAPCajaLite.AfectacionCaja.TipoPago[] TiposPago {
            get {
                return this.TiposPagoField;
            }
            set {
                if ((object.ReferenceEquals(this.TiposPagoField, value) != true)) {
                    this.TiposPagoField = value;
                    this.RaisePropertyChanged("TiposPago");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public short Top {
            get {
                return this.TopField;
            }
            set {
                if ((this.TopField.Equals(value) != true)) {
                    this.TopField = value;
                    this.RaisePropertyChanged("Top");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TipoPago", Namespace="http://schemas.datacontract.org/2004/07/CAPAfectaCajaService")]
    [System.SerializableAttribute()]
    public partial class TipoPago : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private short IdTipoPagoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal MontoField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public short IdTipoPago {
            get {
                return this.IdTipoPagoField;
            }
            set {
                if ((this.IdTipoPagoField.Equals(value) != true)) {
                    this.IdTipoPagoField = value;
                    this.RaisePropertyChanged("IdTipoPago");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal Monto {
            get {
                return this.MontoField;
            }
            set {
                if ((this.MontoField.Equals(value) != true)) {
                    this.MontoField = value;
                    this.RaisePropertyChanged("Monto");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="RespuestaAfectacionCaja", Namespace="http://schemas.datacontract.org/2004/07/CAPAfectaCajaService")]
    [System.SerializableAttribute()]
    public partial class RespuestaAfectacionCaja : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DescripcionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private short NoErrorField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Descripcion {
            get {
                return this.DescripcionField;
            }
            set {
                if ((object.ReferenceEquals(this.DescripcionField, value) != true)) {
                    this.DescripcionField = value;
                    this.RaisePropertyChanged("Descripcion");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public short NoError {
            get {
                return this.NoErrorField;
            }
            set {
                if ((this.NoErrorField.Equals(value) != true)) {
                    this.NoErrorField = value;
                    this.RaisePropertyChanged("NoError");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ListaObjetosCaja", Namespace="http://schemas.datacontract.org/2004/07/CAPAfectaCajaService")]
    [System.SerializableAttribute()]
    public partial class ListaObjetosCaja : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private CAPCajaLite.AfectacionCaja.ObjetoAfectacionCaja[] ObjsAfecCajaField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public CAPCajaLite.AfectacionCaja.ObjetoAfectacionCaja[] ObjsAfecCaja {
            get {
                return this.ObjsAfecCajaField;
            }
            set {
                if ((object.ReferenceEquals(this.ObjsAfecCajaField, value) != true)) {
                    this.ObjsAfecCajaField = value;
                    this.RaisePropertyChanged("ObjsAfecCaja");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AfectacionCaja.ICAPAfectacionCaja")]
    public interface ICAPAfectacionCaja {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICAPAfectacionCaja/AfectarCaja", ReplyAction="http://tempuri.org/ICAPAfectacionCaja/AfectarCajaResponse")]
        CAPCajaLite.AfectacionCaja.RespuestaAfectacionCaja AfectarCaja(CAPCajaLite.AfectacionCaja.ObjetoAfectacionCaja objetoCaja);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICAPAfectacionCaja/AfectarCajaMixto", ReplyAction="http://tempuri.org/ICAPAfectacionCaja/AfectarCajaMixtoResponse")]
        CAPCajaLite.AfectacionCaja.RespuestaAfectacionCaja[] AfectarCajaMixto(CAPCajaLite.AfectacionCaja.ListaObjetosCaja objetosCaja);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICAPAfectacionCajaChannel : CAPCajaLite.AfectacionCaja.ICAPAfectacionCaja, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CAPAfectacionCajaClient : System.ServiceModel.ClientBase<CAPCajaLite.AfectacionCaja.ICAPAfectacionCaja>, CAPCajaLite.AfectacionCaja.ICAPAfectacionCaja {
        
        public CAPAfectacionCajaClient() {
        }
        
        public CAPAfectacionCajaClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CAPAfectacionCajaClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CAPAfectacionCajaClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CAPAfectacionCajaClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public CAPCajaLite.AfectacionCaja.RespuestaAfectacionCaja AfectarCaja(CAPCajaLite.AfectacionCaja.ObjetoAfectacionCaja objetoCaja) {
            return base.Channel.AfectarCaja(objetoCaja);
        }
        
        public CAPCajaLite.AfectacionCaja.RespuestaAfectacionCaja[] AfectarCajaMixto(CAPCajaLite.AfectacionCaja.ListaObjetosCaja objetosCaja) {
            return base.Channel.AfectarCajaMixto(objetosCaja);
        }
    }
}
