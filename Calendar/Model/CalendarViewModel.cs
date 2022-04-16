using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Calendar.Model
{
    public class CalendarViewModel
    {
    }

    public class LoginRequestModel
    {
        public string name { get; set; }
        public string password { get; set; }
    }
    public class LoginRespModel
    {
        public int UserID { get; set; }
        public string Token { get; set; }
        public string RespCode { get; set; }
        public string RespDescription { get; set; }

    }

    public class CalendarRespModel
    {
        public string RespCode { get; set; }
        public string RespDescription { get; set; }
    }
    public class CalendarListRespModel
    {
        public string RespCode { get; set; }
        public string RespDescription { get; set; }
        public List<CalendarInfo> calendars { get; set; }
    }
    public class CalendarInfo
    {
        public int ID { get; set; }
        public string Guest { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime FromDateTime { get; set;}
        public DateTime ToDateTime { get; set; }
    }
    public class CreateCalendarRequestModel
    {
        [Required]
        public int UserID { get; set; }
     
        public string Guest { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime FromDateTime { get; set; }
        [Required]
        public DateTime ToDateTime { get; set; }
    }
    public class UpdateCalendarRequestModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int UserID { get; set; }
     
        public string Guest { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime FromDateTime { get; set; }
        [Required]
        public DateTime ToDateTime { get; set; }
    }
    public class UpdateCalendarTitleRequestModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public string Title { get; set; }
    }
    public class DeleteCalendarRequestModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int UserID { get; set; }
    }
    public class CalendarListRequestModel
    {
        [Required]
        public int UserID { get; set; }
        public DateTime? date { get; set; } = DateTime.Now.Date;
    }
}
