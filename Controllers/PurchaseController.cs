using ALLINONEPROJECTWITHOUTJS.Data;
using ALLINONEPROJECTWITHOUTJS.DTOs;
using ALLINONEPROJECTWITHOUTJS.Models;
using ALLINONEPROJECTWITHOUTJS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALLINONEPROJECTWITHOUTJS.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class PurchaseController : Controller
    {
        private readonly AppDbContext _context;
        private readonly string _connectionString;
        private readonly IPartyService _partyService;
        private readonly IItemService _itemService;
        public PurchaseController(AppDbContext context, IConfiguration configuration, IItemService itemService, IPartyService partyService)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("ConnectionString");
            _itemService = itemService;
            _partyService = partyService;
        }

        public IActionResult Purchase()
        {            
            var pm = new PurchaseViewModel
            {
                itemMasters = _itemService.GetAllItems(),
                partyMasters = _partyService.GetAllParties("Supplier"),
                
                purchaseLists = _context.PurchaseMaster
                            .Include(s => s.PartyMaster)
                            .Include(s => s.purchaseDetails)
                            .ThenInclude(d => d.ItemMaster)
                            .OrderByDescending(s => s.Id)
                            .ToList()
            };
            pm.purchaseMaster = new PurchaseMaster();
            pm.purchaseMaster.PurchaseDate = DateTime.Today;
            if (pm.purchaseMaster.purchaseDetails == null || pm.purchaseMaster.purchaseDetails.Count == 0)
            {
                pm.purchaseMaster.purchaseDetails.Add(new PurchaseDetail());
            }
            return View(pm);
        }

        [HttpPost]
        public IActionResult Purchase(PurchaseViewModel model)
        {
            model.itemMasters = _itemService.GetAllItems();
            model.partyMasters = _partyService.GetAllParties("Supplier");
            model.purchaseLists = _context.PurchaseMaster
                .Include(s => s.PartyMaster)
                .Include(s => s.purchaseDetails)
                .ThenInclude(d => d.ItemMaster)
                .OrderByDescending(s => s.Id)
                .ToList();           

            foreach (var detail in model.purchaseMaster.purchaseDetails)
            {
                if (detail.ItemId > 0)
                {
                    var selectedItem = model.itemMasters.FirstOrDefault(x => x.Id == detail.ItemId);
                    if (selectedItem != null)
                    {
                        detail.Rate = selectedItem.Price;
                        detail.Amount = detail.Qty * selectedItem.Price;
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult AddNewRow(PurchaseViewModel model)
        {
            model.itemMasters = _itemService.GetAllItems();
            model.partyMasters = _partyService.GetAllParties("Supplier");
            model.purchaseLists = _context.PurchaseMaster
                .Include(s => s.PartyMaster)
                .Include(s => s.purchaseDetails)
                .ThenInclude(d => d.ItemMaster)
                .OrderByDescending(s => s.Id)
                .ToList();
            model.purchaseMaster.purchaseDetails.Add(new PurchaseDetail());
            return View("Purchase", model);
        }

        [HttpPost]
        public IActionResult DeleteRow(PurchaseViewModel model)
        {
            model.itemMasters = _itemService.GetAllItems();
            model.partyMasters = _partyService.GetAllParties("Supplier");
            model.purchaseLists = _context.PurchaseMaster
                .Include(s => s.PartyMaster)
                .Include(s => s.purchaseDetails)
                .ThenInclude(d => d.ItemMaster)
                .OrderByDescending(s => s.Id)
                .ToList();
            if (model.purchaseMaster.purchaseDetails.Count > 1)
            {
                model.purchaseMaster.purchaseDetails.RemoveAt(model.purchaseMaster.purchaseDetails.Count - 1);
            }
            return View("Purchase", model);
        }

        [HttpPost]
        public IActionResult SavePurchase(PurchaseViewModel model)
        {
            try
            {
                int PurchaseMasterId = 0;
                using (var con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    using (var trn = con.BeginTransaction())
                    {
                        using var cmd = new SqlCommand("sp_insertPurchaseMaster", con, trn);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PurchaseDate", model.purchaseMaster.PurchaseDate);
                        cmd.Parameters.AddWithValue("@PartyId", model.purchaseMaster.PartyId);
                        cmd.Parameters.Add("@PurchaseMasterId", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        PurchaseMasterId = (int)cmd.Parameters["@PurchaseMasterId"].Value;

                        foreach (var item in model.purchaseMaster.purchaseDetails)
                        {
                            using var cmdDetail = new SqlCommand("sp_insertPurchaseDetail", con, trn);
                            cmdDetail.CommandType = System.Data.CommandType.StoredProcedure;
                            cmdDetail.Parameters.AddWithValue("@PurchaseMasterId", PurchaseMasterId);
                            cmdDetail.Parameters.AddWithValue("@ItemId", item.ItemId);
                            cmdDetail.Parameters.AddWithValue("@Qty", item.Qty);
                            cmdDetail.ExecuteNonQuery();
                        }

                        trn.Commit();
                    }
                }
                return RedirectToAction("Purchase");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        [HttpPost]
        public IActionResult EditPurchase(int id)
        {
            var master = _context.PurchaseMaster.Include(d => d.purchaseDetails).FirstOrDefault(x => x.Id == id);
            if (master == null)
                return RedirectToAction("Purchase");

            var model = new PurchaseViewModel
            {
                purchaseMaster = master,
                itemMasters = _itemService.GetAllItems(),
                partyMasters = _partyService.GetAllParties("Supplier"),
                purchaseLists = _context.PurchaseMaster
                .Include(s => s.PartyMaster)
                .Include(s => s.purchaseDetails)
                .ThenInclude(d => d.ItemMaster)
                .OrderByDescending(s => s.Id)
                .ToList()
            };

            foreach (var detail in model.purchaseMaster.purchaseDetails)
            {
                if (detail.ItemId > 0)
                {
                    var selectedItem = model.itemMasters.FirstOrDefault(x => x.Id == detail.ItemId);
                    if (selectedItem != null)
                    {
                        detail.Rate = selectedItem.Price;
                        detail.Amount = detail.Qty * selectedItem.Price;
                    }
                }
            }

            return View("Purchase", model);
        }

        [HttpPost]
        public IActionResult UpdatePurchase(PurchaseViewModel model)
        {
            if (model.purchaseMaster == null || model.purchaseMaster.Id <= 0)
                return RedirectToAction("Purchase");

            var existing = _context.PurchaseMaster.Include(d => d.purchaseDetails).FirstOrDefault(x => x.Id == model.purchaseMaster.Id);

            if (existing == null)
                return RedirectToAction("Purchase");

            existing.PurchaseDate = model.purchaseMaster.PurchaseDate;
            existing.PartyId = model.purchaseMaster.PartyId;

            var itemMaster = new ItemMaster();
            _context.PurchaseDetails.RemoveRange(existing.purchaseDetails);
            foreach (var detail in existing.purchaseDetails)
            {
                if (detail.ItemId > 0)
                {
                    itemMaster = _context.ItemMasters?.Find(detail.ItemId);
                    itemMaster.CurrentStock = itemMaster.CurrentStock - (decimal)detail.Qty;
                    _context.Entry(itemMaster).State = EntityState.Modified;
                }
            }
            _context.SaveChanges();

            if (model.purchaseMaster.purchaseDetails != null)
            {                
                foreach (var detail in model.purchaseMaster.purchaseDetails)
                {
                    if (detail.ItemId > 0)
                    {
                        _context.PurchaseDetails.Add(new PurchaseDetail { PurchaseMasterId = existing.Id, Qty = detail.Qty, ItemId = detail.ItemId });
                        itemMaster = _context.ItemMasters?.Find(detail.ItemId);
                        itemMaster.CurrentStock = itemMaster.CurrentStock + (decimal)detail.Qty;
                        _context.Entry(itemMaster).State = EntityState.Modified; 
                    }
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Purchase");
        }

        [HttpPost]
        public IActionResult DeletePurchase(int id)
        {
            var master = _context.PurchaseMaster.Include(d => d.purchaseDetails).FirstOrDefault(x => x.Id == id);
            if (master == null)
                return RedirectToAction("Purchase");

            _context.PurchaseDetails.RemoveRange(master.purchaseDetails);
            var itemMaster = new ItemMaster();
            foreach (var detail in master.purchaseDetails)
            {
                if (detail.ItemId > 0)
                {
                    itemMaster = _context.ItemMasters?.Find(detail.ItemId);
                    itemMaster.CurrentStock = itemMaster.CurrentStock - (decimal)detail.Qty;
                    _context.Entry(itemMaster).State = EntityState.Modified;
                }
            }
            _context.PurchaseMaster.Remove(master);
            _context.SaveChanges();

            return RedirectToAction("Purchase");
        }
    }
}
