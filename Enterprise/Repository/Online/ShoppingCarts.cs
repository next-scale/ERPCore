using ERPCore.Enterprise.Models.Items;
using System.Collections.Generic;
using System.Linq;
using System;
using ERPCore.Enterprise.Models.Online;

namespace ERPCore.Enterprise.Repository.Items
{
    public class ShoppingCarts : ERPNodeDalRepository
    {
        public ShoppingCarts(Organization organization) : base(organization)
        {

        }


        public List<ShoppingCartItem> AllCartItems
        {
            get { return erpNodeDBContext.ShoppingCartItems.ToList(); }
        }

        public List<ShoppingCartItem> GetCartItemsByProfile(string sessionId, Guid? profileGuid)
        {
            if (profileGuid == null)
                return erpNodeDBContext.ShoppingCartItems
                     .Where(cartItem => cartItem.SessionId == sessionId)
                     .ToList();
            else
                return erpNodeDBContext.ShoppingCartItems
                    .Where(cartItem => cartItem.SessionId == sessionId || cartItem.ProfileId == profileGuid)
                    .ToList();
        }

        public void AddCart(string sessionId, Guid? ProfileId, Guid ItemId, int Amount)
        {
            //Calculate Discount
            var item = erpNodeDBContext.Items.Find(ItemId);

            var newcartItem = new ShoppingCartItem()
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                ProfileId = ProfileId,
                ItemGuid = ItemId,
                UnitPrice = item.UnitPrice,
                Amount = Amount,
                CreateDate = DateTime.Today
            };

            erpNodeDBContext.ShoppingCartItems.Add(newcartItem);
            erpNodeDBContext.SaveChanges();

        }

        public Models.Estimations.SalesEstimate SubmitOrder(Guid profileGuid)
        {
            var orderItems = erpNodeDBContext
               .ShoppingCartItems
               .Where(items => items.ProfileId == profileGuid)
               .ToList();



            if (orderItems.Count > 0)
            {

                var salesEstimate = organization.SalesEstimates.Create(profileGuid, DateTime.Today);
                salesEstimate.Reference = "Online Sale";

                salesEstimate.Items = new HashSet<Models.Estimations.EstimateItem>();

                orderItems.ForEach(orderItem =>
                {
                    var item = erpNodeDBContext.Items.Find(orderItem.ItemGuid);
                    salesEstimate.AddItem(item, orderItem.Amount);
                });

                salesEstimate.Calculate();
                erpNodeDBContext.SaveChanges();


                erpNodeDBContext.ShoppingCartItems.RemoveRange(orderItems);
                erpNodeDBContext.SaveChanges();

                return salesEstimate;
            }
            return null;
        }



    }
}