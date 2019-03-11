using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class StockMovement2ViewModel
    {
        /// <summary>
        /// STOJOU.VCRNUM_0
        /// </summary>
        public string DocNo { get; set; }
        /// <summary>
        /// STOJOU.VCRTYP_0
        /// </summary>
        public string MovementType { get; set; }
        /// <summary>
        /// STOJOU.IPTDAT_0
        /// </summary>
        public DateTime? MovementDate { get; set; }
        public string MovementDateString => this.MovementDate?.ToString("dd/MM/yy") ?? "-";
        /// <summary>
        /// STOJOU.QTYPCU_0
        /// </summary>
        public double? QuantityIn { get; set; }
        public string QuantityInString => string.Format("{0:#,##0}", this.QuantityIn);
        /// <summary>
        /// STOJOU.QTYPCU_0
        /// </summary>
        public double? QuantityOut { get; set; }
        public string QuantityOutString => string.Format("{0:#,##0}", this.QuantityOut * -1);
        /// <summary>
        /// STOJOU.LOC_0
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// STOJOU.CCE_1
        /// </summary>
        public string Bom { get; set; }
        /// <summary>
        /// STOJOU.CCE_2
        /// </summary>
        public string Project { get; set; }
        /// <summary>
        /// STOJOU.CCE_3
        /// </summary>
        public string WorkGroup { get; set; }
        /// <summary>
        /// STOJOU.PCU_0
        /// </summary>
        public double? Weight { get; set; }
    }
}
