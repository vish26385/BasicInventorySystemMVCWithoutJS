using System.ComponentModel.DataAnnotations.Schema;

namespace ALLINONEPROJECTWITHOUTJS.Models
{
    public class PurchaseMaster
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        [ForeignKey("PartyMaster")]
        public int PartyId { get; set; }
        public PartyMaster PartyMaster { get; set; }
        public List<PurchaseDetail> purchaseDetails { get; set; } = new List<PurchaseDetail>();

        [NotMapped]
        public string? CalcSource { get; set; }
    }
}
