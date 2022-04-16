using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Calendar.Data
{
    [Table("tbcalendar")]
    public class tbCalendar
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Guest { get; set; }
        public DateTime FromDatetime { get; set; }
        public DateTime ToDatetime { get; set; }
    }
}
