using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calendar.Model;
using Calendar.Data;
using Calendar.Controllers;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Net;
using System.Threading;

namespace Calendar.BusinessLayer
{
    public class CalendarLogicLayer : ICalendarLogicLayer
    {
        private CalendarDBContext dbContext;

        public CalendarLogicLayer(CalendarDBContext _dbContext)
        {
            dbContext = _dbContext;

        }
        public async Task<CalendarListRespModel> CalendarList(CalendarListRequestModel reqModel)
        {
            var respModel = new CalendarListRespModel();
            if (reqModel.date == null)
            {
                reqModel.date = DateTime.Now.Date;
            }
            respModel.calendars = dbContext.tbCalendars.Where(a => a.FromDatetime >= reqModel.date && a.UserID == reqModel.UserID)
                                  .Select(a => new CalendarInfo()
                                  {
                                      ID = a.ID,
                                      Description = a.Description,
                                      Guest = a.Guest,
                                      Title = a.Title,
                                      FromDateTime = a.FromDatetime,
                                      ToDateTime = a.ToDatetime
                                  }).ToList();
            respModel.RespCode = Messages._000.Key;
            respModel.RespDescription = Messages._000.Value;
            return respModel;
        }

        public async Task<CalendarRespModel> CreateCalendar(CreateCalendarRequestModel reqModel)
        {
            var respModel = new CalendarRespModel();
            if (!checkValidFromTime(reqModel.FromDateTime))
            {
                respModel.RespCode = Messages._024.Key;
                respModel.RespDescription = Messages._024.Value;
                return respModel;
            }
            if (!checkCorrectDateRange(reqModel.FromDateTime, reqModel.ToDateTime))
            {
                respModel.RespCode = Messages._023.Key;
                respModel.RespDescription = Messages._023.Value;
                return respModel;
            }

            var exist = checkTimeFrameExist(reqModel.FromDateTime, reqModel.ToDateTime,reqModel.UserID);
            if (exist == null)
            {
                var calendar = new tbCalendar()
                {
                    Title = reqModel.Title,
                    Description = reqModel.Description,
                    Guest = reqModel.Guest,
                    FromDatetime = reqModel.FromDateTime,
                    ToDatetime = reqModel.ToDateTime,
                    UserID = reqModel.UserID
                };
                await dbContext.tbCalendars.AddAsync(calendar);
                await dbContext.SaveChangesAsync();
                respModel.RespCode = Messages._000.Key;
                respModel.RespDescription = Messages._000.Value;
                //SendEmail(calendar);
                var tomail = dbContext.tbUsers.Where(a => a.ID == reqModel.UserID).FirstOrDefault().Email;
                new Thread(() => SendEmail(calendar,tomail)).Start();
            }
            else
            {
                respModel.RespCode = Messages._021.Key;
                respModel.RespDescription = Messages._021.Value.Replace("{title}", exist.Title)
                    .Replace("{from}", exist.FromDatetime.ToString("dd/MM/yyyy HH:mm:ss"))
                    .Replace("{to}", exist.ToDatetime.ToString("dd/MM/yyyy HH:mm:ss"));
            }

            return respModel;
        }

        public async Task<CalendarRespModel> DeleteCalendar(DeleteCalendarRequestModel reqModel)
        {
            var respModel = new CalendarRespModel();
            var existing = getCalendar(reqModel.ID, reqModel.UserID);
            if (existing == null)
            {
                respModel.RespCode = Messages._006.Key;
                respModel.RespDescription = Messages._006.Value;
                return respModel;
            }
            else
            {
                dbContext.tbCalendars.Remove(existing);
                respModel.RespCode = Messages._000.Key;
                respModel.RespDescription = Messages._000.Value;
            }
            existing = preparingCancellationSchedule(existing);
            var tomail = dbContext.tbUsers.Where(a => a.ID == existing.UserID).FirstOrDefault().Email;
            dbContext.SaveChanges();
            new Thread(()=>SendEmail(existing,tomail)).Start();
            return respModel;
        }

        public async Task<CalendarRespModel> UpdateCalendar(UpdateCalendarRequestModel reqModel)
        {
            var respModel = new CalendarRespModel();
            var existing = getCalendar(reqModel.ID, reqModel.UserID);
           
            if (!checkValidFromTime(reqModel.FromDateTime))
            {
                respModel.RespCode = Messages._024.Key;
                respModel.RespDescription = Messages._024.Value;
                return respModel;
            }
            if (!checkCorrectDateRange(reqModel.FromDateTime, reqModel.ToDateTime))
            {
                respModel.RespCode = Messages._023.Key;
                respModel.RespDescription = Messages._023.Value;
                return respModel;
            }
            if (existing == null)
            {
                respModel.RespCode = Messages._006.Key;
                respModel.RespDescription = Messages._006.Value;
                return respModel;
            }
            else
            {
                var existTimeFrame = checkTimeFrameExist(reqModel.FromDateTime, reqModel.ToDateTime, reqModel.UserID,reqModel.ID);
                if (existTimeFrame == null)
                {
                    existing.Title = reqModel.Title;
                    existing.Description = reqModel.Description;
                    existing.Guest = reqModel.Guest;
                    existing.FromDatetime = reqModel.FromDateTime;
                    existing.ToDatetime = reqModel.ToDateTime;

                    respModel.RespCode = Messages._000.Key;
                    respModel.RespDescription = Messages._000.Value;
                }
                else
                {
                    respModel.RespCode = Messages._021.Key;
                    respModel.RespDescription = Messages._021.Value.Replace("{title}", existTimeFrame.Title)
                        .Replace("{from}", existTimeFrame.FromDatetime.ToString("dd/MM/yyyy HH:mm:ss"))
                        .Replace("{to}", existTimeFrame.ToDatetime.ToString("dd/MM/yyyy HH:mm:ss"));
                }
            }
            await dbContext.SaveChangesAsync();
            return respModel;
        }

        public async Task<CalendarRespModel> UpdateCalendarTitle(UpdateCalendarTitleRequestModel reqModel)
        {
            var respModel = new CalendarRespModel();
            var existing = getCalendar(reqModel.ID, reqModel.UserID);
            if (existing == null)
            {
                respModel.RespCode = Messages._006.Key;
                respModel.RespDescription = Messages._006.Value;
            }
            else
            {
                existing.Title = reqModel.Title;
                await dbContext.SaveChangesAsync();
                respModel.RespCode = Messages._000.Key;
                respModel.RespDescription = Messages._000.Value;
            }
            return respModel;
        }
        private bool checkValidFromTime(DateTime from)
        {
            return from >= DateTime.Now?true:false;
        }
        private bool checkCorrectDateRange(DateTime from, DateTime to)
        {
            return from >= to ? false : true;
        }
        private tbCalendar checkTimeFrameExist(DateTime from, DateTime to, int userID,int ID = 0)
        {
            Expression<Func<tbCalendar, bool>> calendar = x => true;
            if (ID > 0)
            {
                calendar = x => x.ID != ID; 
            }
            var resp = dbContext.tbCalendars.Where(a => a.UserID==userID && a.FromDatetime>=from.Date && ((from < a.FromDatetime && to > a.FromDatetime)
                            || (from < a.ToDatetime && to > a.ToDatetime)
                            || (a.FromDatetime < from && a.ToDatetime > to)
                            || (a.FromDatetime > from && a.ToDatetime < to))
                            ).Where(calendar).FirstOrDefault();

            return resp;

        }
        private tbCalendar getCalendar(int ID, int userID)
        {
            return dbContext.tbCalendars.Where(a => a.ID == ID && a.UserID == userID).FirstOrDefault();
        }

        private tbCalendar preparingCancellationSchedule(tbCalendar reqModel)
        {
            reqModel.Title="Cancellation of "+reqModel.Title;
            reqModel.Description = $"We are sorry to notify you that the event {reqModel.Title} at {reqModel.FromDatetime.ToString("MMMM dd, yyyy HH:mm")} has been cancelled";
            return reqModel;
        }

        private void SendEmail(tbCalendar reqModel,string tomail)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("calendartt381@gmail.com");
                var mailList = reqModel.Guest.Split(',');
                message.To.Add(tomail);
                for (int i = 0; i < mailList.Length; i++)
                {
                    message.CC.Add(new MailAddress(mailList[i]));
                }


                message.Subject = reqModel.Title;
                message.IsBodyHtml = false; 
                message.Body = reqModel.Description;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("calendartt381@gmail.com", "CalendarTT123");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception ex) {
            }
        }
    }
}
