using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calendar.Data;
using Calendar.Model;
using Calendar.Services;
using Microsoft.Extensions.Configuration;
using Calendar.BusinessLayer;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Calendar.Controllers
{
    public class Messages
    {
        public static KeyValuePair<string, string> _000 = new KeyValuePair<string, string>("000", "success");
        public static KeyValuePair<string, string> _004 = new KeyValuePair<string, string>("004", "Incorrect UserName or Password");
        public static KeyValuePair<string, string> _019 = new KeyValuePair<string, string>("119", "Something Went Wrong. Please try again later.");
        public static KeyValuePair<string, string> _021 = new KeyValuePair<string, string>("021", "There is conflict time frame with {title} between {from} and {to}.\n Please try considering to move the existing schedule or change the new schedule time frame");
        public static KeyValuePair<string, string> _023 = new KeyValuePair<string, string>("023", "FromDateTime cannot be equal or greater than ToDateTime");
        public static KeyValuePair<string, string> _024 = new KeyValuePair<string, string>("024", "FromDateTime cannot less than current time");
        public static KeyValuePair<string, string> _006 = new KeyValuePair<string, string>("006", "Sorry, the schedule does not exist.");
    }
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/Calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private CalendarDBContext dbContext;
        private iJWTAuthentication jwt;
        private IConfiguration configuration;
        private ICalendarLogicLayer logicLayer;
        private ILogService logService;
        public CalendarController(CalendarDBContext _dbContext,iJWTAuthentication _jwt,IConfiguration _configuration,ICalendarLogicLayer _calendarLogicLayer,ILogService _logService)
        {
            dbContext = _dbContext;
            jwt = _jwt;
            configuration = _configuration;
            logicLayer = _calendarLogicLayer;
            logService = _logService;
        }
        
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody]LoginRequestModel reqModel)
        {
            var respModel = new LoginRespModel();
            try {
                var user = dbContext.tbUsers.Where(a => a.Name == reqModel.name && a.Password == reqModel.password).FirstOrDefault();

                if (user != null)
                {
                    respModel.RespCode = Messages._000.Key;
                    respModel.RespDescription = Messages._000.Value;
                    respModel.UserID = user.ID;
                    respModel.Token = jwt.ValidateAndCreateJWT(user);
                }
                else
                {
                    respModel.RespCode = Messages._004.Key;
                    respModel.RespDescription = Messages._004.Value;
                }
            }
            catch(Exception ex)
            {
                respModel.RespCode = Messages._019.Key;
                respModel.RespDescription = Messages._019.Value;
            }
            finally
            {
                logService.Logging(reqModel, respModel, this.Request.GetDisplayUrl(), 0);
            }
            return Ok(respModel);
        }

        [HttpPost("CreateCalendar")]
        public async Task<IActionResult> CreateCalendar(CreateCalendarRequestModel reqModel)
        {
            var respModel = new CalendarRespModel();
            try
            {
                respModel = await logicLayer.CreateCalendar(reqModel);    
            }
            catch (Exception ex)
            {
                respModel.RespCode = Messages._019.Key;
                respModel.RespDescription = Messages._019.Value;
            }
            finally
            {
                logService.Logging(reqModel, respModel, this.Request.GetDisplayUrl(), reqModel.UserID);
            }
            return Ok(respModel);
        }

        [HttpPut("UpdateCalendar")]
        public async Task<IActionResult> UpdateCalendar(UpdateCalendarRequestModel reqModel)
        {
            var respModel = new CalendarRespModel();
            try
            {
                respModel = await logicLayer.UpdateCalendar(reqModel);
            }
            catch (Exception ex)
            {
                respModel.RespCode = Messages._019.Key;
                respModel.RespDescription = Messages._019.Value;
            }
            finally
            {
                logService.Logging(reqModel, respModel, this.Request.GetDisplayUrl(), reqModel.UserID);
            }
            return Ok(respModel);
        }

        [HttpPatch("UpdateTitle")]
        public async Task<IActionResult> UpdateTitle(UpdateCalendarTitleRequestModel reqModel)
        {
            var respModel = new CalendarRespModel();
            try
            {
                respModel = await logicLayer.UpdateCalendarTitle(reqModel);
            }
            catch (Exception ex)
            {
                respModel.RespCode = Messages._019.Key;
                respModel.RespDescription = Messages._019.Value;
            }
            finally
            {
                logService.Logging(reqModel, respModel, this.Request.GetDisplayUrl(), reqModel.UserID);
            }
            return Ok(respModel);
        }

        [HttpDelete("DeleteCalendar")]
        public async Task<IActionResult> DeleteTitle(DeleteCalendarRequestModel reqModel)
        {
            var respModel = new CalendarRespModel();
            try
            {
                respModel = await logicLayer.DeleteCalendar(reqModel);
            }
            catch (Exception ex)
            {
                respModel.RespCode = Messages._019.Key;
                respModel.RespDescription = Messages._019.Value;
            }
            finally
            {
                logService.Logging(reqModel, respModel, this.Request.GetDisplayUrl(), reqModel.UserID);
            }
            return Ok(respModel);
        }


        [HttpGet("GetCalendar")]
        public async Task<IActionResult> GetCalendar([FromBody]CalendarListRequestModel reqModel)
        {
            var respModel = new CalendarListRespModel();
            try
            {
                respModel = await logicLayer.CalendarList(reqModel);
            }
            catch (Exception ex)
            {
                respModel.RespCode = Messages._019.Key;
                respModel.RespDescription = Messages._019.Value;
            }
            finally
            {
                logService.Logging(reqModel, respModel, this.Request.GetDisplayUrl(), reqModel.UserID);
            }
            return Ok(respModel);
        }
    }
}
