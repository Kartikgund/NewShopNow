using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewShopNowBL2.Models;

namespace NewShopNow2.ViewModel
{
    public class TransactionModel
    {
        public List<tblStock> lstProduct { get; set; }

        public List<tblTransactionItem> lstItems { get; set; }


        public tblTransaction objTransaction { get; set; }
        public tblTransactionItem objItem { get; set; }


        public tblCustomer objCustmore { get; set; }
        public SelectListItem[] BOProducts()
        {
            return new SelectListItem[3] { new SelectListItem() { Text = "Card", Value = "Card" },
                new SelectListItem() { Text = "Cash", Value = "Cash" },
                new SelectListItem() { Text = "UPI", Value = "UPI" } };
        }
    }
}