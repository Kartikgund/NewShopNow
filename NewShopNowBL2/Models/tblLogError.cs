namespace NewShopNowBL2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblLogError")]
    public partial class tblLogError
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public string ExceptionMsg { get; set; }

        [Required]
        public string ExceptionType { get; set; }

        [Required]
        public string ExceptionURL { get; set; }

        [Required]
        public string ExceptionSource { get; set; }

        public DateTime Logdate { get; set; }
    }
}
