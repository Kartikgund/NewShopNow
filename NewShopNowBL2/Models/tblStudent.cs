using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewShopNowBL2.Models
{
    public class tblStudent
    {
        public int Id { get; set; }

        [Required]
        public string StudentName { get; set; }

        [Required]
        public string Course { get; set; }

        [Required]
        public string College { get; set; }

    }
}
