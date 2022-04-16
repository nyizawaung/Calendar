using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calendar.Model;

namespace Calendar.BusinessLayer
{
    public interface ICalendarLogicLayer
    {
        Task<CalendarRespModel> CreateCalendar(CreateCalendarRequestModel reqModel);
        Task<CalendarRespModel> UpdateCalendar(UpdateCalendarRequestModel reqModel);
        Task<CalendarRespModel> UpdateCalendarTitle(UpdateCalendarTitleRequestModel reqModel);
        Task<CalendarRespModel> DeleteCalendar(DeleteCalendarRequestModel reqModel);
        Task<CalendarListRespModel> CalendarList(CalendarListRequestModel reqModel);
    }
}
