﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ALLINONEPROJECTWITHOUTJS.Models
{
    public class PurchaseDetail
    {
        public int Id { get; set; }
        [ForeignKey("PurchaseMaster")]
        public int PurchaseMasterId { get; set; }
        [ForeignKey("ItemMaster")]
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public ItemMaster ItemMaster { get; set; }
        public PurchaseMaster PurchaseMaster { get; set; }

        [NotMapped]
        public decimal Rate { get; set; }

        [NotMapped]
        public decimal Amount { get; set; }
    }
}
