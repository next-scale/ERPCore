using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.DataBase;

namespace ERPCore.Enterprise.Repository.Items
{
    public class InventoryItems : ERPNodeDalRepository
    {

        public InventoryItems(Organization organization) : base(organization)
        {

        }

        public void UpdateStockAmount()
        {
            //erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            organization.Items.RemoveUnLinkCommercialItems();
            this.ResetStockAmount();

            var Items = erpNodeDBContext
              .CommercialItems
              .Where(Ti => Ti.Item.ItemType == Models.Items.Enums.ItemTypes.Inventory)
              .GroupBy(TransactionItem => new
              {
                  TransactionItem.ItemGuid,
                  TransactionItem.Commercial.TransactionType
              })
              .Select(go => new
              {
                  ItemGuid = go.Key.ItemGuid,
                  transactionType = go.Key.TransactionType,
                  Amount = go.Sum(i => i.Amount),
                  Item = go.FirstOrDefault().Item
              });


            foreach (var stockItem in Items)
            {
                if (stockItem.transactionType == Models.Accounting.Enums.TransactionTypes.Sale)
                    stockItem.Item.AmountSold = stockItem.Amount;

                else if (stockItem.transactionType == Models.Accounting.Enums.TransactionTypes.Purchase)
                    stockItem.Item.AmountPurchase = stockItem.Amount;
            }

            erpNodeDBContext.SaveChanges();
        }
        private void ResetStockAmount()
        {
            erpNodeDBContext
                 .Items
                 .Where(Ti => Ti.ItemType == Models.Items.Enums.ItemTypes.Inventory)
                 .ToList()
                 .ForEach(i =>
                 {
                     i.AmountPurchase = 0;
                     i.AmountSold = 0;
                 });

            erpNodeDBContext.SaveChanges();

        }

    }
}
