using ALLINONEPROJECTWITHOUTJS.Models;

namespace ALLINONEPROJECTWITHOUTJS.DTOs
{
    public class SaleViewModel
    {
        public SalesMaster salesMaster { get; set; } = new SalesMaster();
        public List<SalesMaster> salesLists { get; set; } = new List<SalesMaster>();
        public List<ItemMaster> itemMasters { get; set; } = new List<ItemMaster>();
        public List<PartyMaster> partyMasters { get; set; } = new List<PartyMaster>();
    }
}
