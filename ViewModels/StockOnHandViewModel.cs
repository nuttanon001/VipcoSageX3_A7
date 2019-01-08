using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class StockOnHandViewModel
    {
        /// <summary>
        /// ITMMASTER.ITMREF_0
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// ITMMASTER.ITMDES1_0
        /// </summary>
        public string ItemDesc { get; set; }
        /// <summary>
        /// ITMMASTER.STU_0
        /// </summary>
        public string Uom { get; set; }
        /// <summary>
        /// ITMMASTER.TCLCOD_0
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// TEXCLOB.TEXTE_0
        /// </summary>
        public string ItemDescFull { get; set; }
        /// <summary>
        /// ATEXTRA.TEXTE_0
        /// </summary>
        public string CategoryDesc { get; set; }
        /// <summary>
        /// ITMMVT.PHYSTO_0
        /// </summary>
        public double InternelStock { get; set; }
        /// <summary>
        /// LocationStock
        /// </summary>
        public string LocationStock { get; set; }
        /// <summary>
        /// ITMFACILIT.OFS_0
        /// </summary>
        public double OnOrder { get; set; }

        public string InternelStockString => string.Format("{0:#,##0}", this.InternelStock);
        public string OnOrderString => string.Format("{0:#,##0}", this.OnOrder);

        public ICollection<StockLocationViewModel> StockLocations { get; set; } = new List<StockLocationViewModel>();
    }
}
