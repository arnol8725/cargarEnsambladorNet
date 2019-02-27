using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapCajaLite
{
    public class TipoPago
    {
        private int idTipoPago;
        private decimal total;

        public int IdTipoPago
        {
            get
            {
                return idTipoPago;
            }

            set
            {
                idTipoPago = value;
            }
        }

        public decimal Total
        {
            get
            {
                return total;
            }

            set
            {
                total = value;
            }
        }

        public TipoPago()
        {
            this.idTipoPago = 0;
            this.total = 0;
        }
    }
}
