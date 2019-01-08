using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class StockMovementViewModel
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
        /// TEXCLOB.TEXTE_0
        /// </summary>
        public string ItemDescFull { get; set; }
        /// <summary>
        /// ITMMASTER.TCLCOD_0
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// ATEXTRA.TEXTE_0
        /// </summary>
        public string CategoryDesc { get; set; }
        /// <summary>
        /// STOJOU.PRIVAL_0
        /// </summary>
        public double? UnitPrice { get; set; }
        /// <summary>
        /// STOJOU.AMTVAL_0
        /// </summary>
        public double? Amount { get; set; }
 
        public string Uom { get; set; }

        public ICollection<StockMovement2ViewModel> StockMovement2s { get; set; } = new List<StockMovement2ViewModel>();
    }
}
