using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewShopNowBL2.Models;

namespace NewShopNow2.ViewModel
{
    public class UserAndStore
    {
        public tblUser objUser { get; set; }

        public List<tblStore> lstStore { get; set; }
    }
}