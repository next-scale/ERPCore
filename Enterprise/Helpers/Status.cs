using ERPCore.Enterprise.Models.Accounting.Enums;
using System;

namespace ERPCore.Enterprise.Helpers
{
    public static class Status
    {
        public static string StatusImage(object status)
        {
            if (status != null)
                return String.Format("<img src ='/Content/Icon/Status/{0}.png' /> ", status.ToString());
            else
                return " ";
        }


        public static string ItemTypeImageUrl(object status)
        {
            if (status != null)
                return String.Format("/Content/Icon/ItemTypes/{0}.png", status.ToString());
            else
                return " ";
        }

        public static string ItemTypeImage(object status)
        {
            if (status != null)
                return String.Format("<img src = '/Content/Icon/ItemTypes/{0}.png' height='15' />", status.ToString());
            else
                return " ";
        }




        public static string LedgerStatus(object isPostLedger)
        {
            if ((LedgerPostStatus)(isPostLedger ?? LedgerPostStatus.ReadyToPost) == LedgerPostStatus.Posted)
                return String.Format("<i class=\"fas fa-calculator text-muted\"></i>");
            else
                return String.Format("<i class=\"fas fa-calculator text-light\"></i>");
        }
    }
}