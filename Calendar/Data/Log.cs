using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Calendar.Data
{
    [Table("tblog")]
    public class tbLog
    {
        public int ID { get; set; }
        public string RequestPath { get; set; }
        public string RequestData { get; set; }
        public string RespData { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UserID { get; set; }
    }
}
