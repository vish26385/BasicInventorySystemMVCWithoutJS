using ALLINONEPROJECTWITHOUTJS.Models;

namespace ALLINONEPROJECTWITHOUTJS.DTOs
{
    public class PurchaseViewModel
    {
        public PurchaseMaster purchaseMaster { get; set; } = new PurchaseMaster();
        public List<PurchaseMaster> purchaseLists { get; set; } = new List<PurchaseMaster>();
        public List<ItemMaster> itemMasters { get; set; } = new List<ItemMaster>();
        public List<PartyMaster> partyMasters { get; set; } = new List<PartyMaster>();
    }
}
