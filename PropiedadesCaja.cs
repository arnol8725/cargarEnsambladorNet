using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapCajaLite
{
    public class PropiedadesCaja
    {

        public bool Esdivisa { get; set; }

        public short TipoDivisa { get; set; }

        public decimal lcurSaldoCajero { get; set; }

        public string UsrCaja { get; set; }

        public int NumeroTransaccion { get; set; }

        public short TipoOperacion { get; set; }

        public string Referencia { get; set; }

        public decimal Importe_Cheque { get; set; }

        public decimal Importe_Total { get; set; }

        public decimal Importe_Efectivo { get; set; }

        public int liNegocio { get; set; }

        public string WS { get; set; }

        public int lsNotienda { get; set; }

        public decimal liImpTotalVta { get; set; }

        public decimal gnImporte { get; set; }

        public int lsTipoVta { get; set; }

        public int gnPresupuesto { get; set; }

        public bool Anular_Efectivo { get; set; }

        public bool Anular_Cheques { get; set; }

        public int plTranNoAnula { get; set; }
    }
}
