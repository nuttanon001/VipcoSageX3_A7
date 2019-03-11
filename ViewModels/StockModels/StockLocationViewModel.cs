using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class StockLocationViewModel
    {
        /// <summary>
        /// STOCK.LOC_0
        /// </summary>
        public string LocationCode { get; set; }
        /// <summary>
        /// STOCK.QTYPCU_0
        /// </summary>
        public double Quantity { get; set; }
        public string QuantityString => string.Format("{0:#,##0}", this.Quantity);
        /// <summary>
        /// STOCK.PCU_0
        /// </summary>
        public string Uom { get; set; }
        /// <summary>
        /// STOCK.PJT_0
        /// </summary>
        public string Project { get; set; }
        /// <summary>
        /// STOCK.LOT_0
        /// </summary>
        public string LotNo { get; set; }
        /// <summary>
        /// STOCK.BPSLOT_0
        /// </summary>
        public string HeatNo { get; set; }
        /// <summary>
        /// STOCK.PALNUM_0
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// STOLOT.SHLDAT
        /// </summary>
        public DateTime? ExpDate { get; set; }
        public string ExpDateString => this.ExpDate != null ? this.ExpDate.Value.ToString("dd/MM/yy") : "";

    }
}
