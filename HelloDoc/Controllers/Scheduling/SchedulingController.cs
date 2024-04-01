using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Viewmodels;
using System.Collections;

namespace HelloDoc.Controllers.Scheduling
{
    public class SchedulingController : Controller
    {

        private readonly HelloDocDbContext _context;

        public SchedulingController(HelloDocDbContext context)
        {
            _context = context;
        }

        public IActionResult Scheduling()
        {
            SchedulingViewModel modal = new SchedulingViewModel();
            modal.regions = _context.Regions.ToList();
            return View(modal);
        }


        public IActionResult LoadSchedulingPartial(string PartialName, string date, int regionid)
        {
            var currentDate = DateTime.Parse(date);
            List<Physician> physician = _context.Physicianregions.Include(u => u.Physician).Where(u => u.Regionid == regionid).Select(u => u.Physician).ToList();
            if (regionid == 0)
            {
                physician = _context.Physicians.ToList();
            }

            switch (PartialName)
            {

                case "_DayWise":
                    DayWiseScheduling day = new DayWiseScheduling
                    {
                        date = currentDate,
                        physicians = physician,
                        shiftdetails = _context.Shiftdetails.Include(u => u.Shift).ToList()
                    };
                    return PartialView("_DayWise", day);

                case "_WeekWise":
                    WeekWiseScheduling week = new WeekWiseScheduling
                    {
                        date = currentDate,
                        physicians = physician
                    };
                    return PartialView("_WeekWise", week);

                case "_MonthWise":
                    MonthWiseScheduling month = new MonthWiseScheduling
                    {
                        date = currentDate,
                    };
                    return PartialView("_MonthWise", month);

                default:
                    return PartialView("_DayWise");
            }
        }

        public List<Physician> filterregion(string regionid)
        {
            List<Physician> physicians = _context.Physicianregions.Where(u => u.Regionid.ToString() == regionid).Select(y => y.Physician).ToList();
            return physicians;
        }

        public IActionResult AddShift(SchedulingViewModel model)
        {
            int adminid = (int)HttpContext.Session.GetInt32("AdminId");
            var admin = _context.Admins.FirstOrDefault(m => m.Adminid == adminid);
            Aspnetuser aspnetadmin = _context.Aspnetusers.FirstOrDefault(m => m.Id == admin.Aspnetuserid);
            var chk = Request.Form["repeatdays"].ToList();
            var shiftid = _context.Shifts.Where(u => u.Physicianid == model.providerid).Select(u => u.Shiftid).ToList();
            if (shiftid.Count() > 0)
            {
                foreach (var obj in shiftid)
                {
                    var shiftdetailchk = _context.Shiftdetails.Where(u => u.Shiftid == obj && u.Shiftdate == DateOnly.FromDateTime(model.shiftdate)).ToList();
                    if (shiftdetailchk.Count() > 0)
                    {
                        foreach (var item in shiftdetailchk)
                        {
                            if ((model.starttime >= item.Starttime && model.starttime <= item.Endtime) || (model.endtime >= item.Starttime && model.endtime <= item.Endtime))
                            {
                                TempData["error"] = "Shift is already assigned in this time";
                                return RedirectToAction("Scheduling");
                            }
                        }
                    }
                }
            }
            Shift shift = new Shift
            {
                Physicianid = model.providerid,
                Startdate = DateOnly.FromDateTime(model.shiftdate),
                Repeatupto = model.repeatcount,
                Createddate = DateTime.Now,
                Createdby = aspnetadmin.Id
            };
            foreach (var obj in chk)
            {
                shift.Weekdays += obj;
            }
            if (model.repeatcount > 0)
            {
                shift.Isrepeat = new BitArray(new[] { true });
            }
            else
            {
                shift.Isrepeat = new BitArray(new[] { false });
            }
            _context.Shifts.Add(shift);
            _context.SaveChanges();
            DateTime curdate = model.shiftdate;
            Shiftdetail shiftdetail = new Shiftdetail();
            shiftdetail.Shiftid = shift.Shiftid;
            shiftdetail.Shiftdate = DateOnly.FromDateTime(curdate);
            shiftdetail.Regionid = model.regionid;
            shiftdetail.Starttime = model.starttime;
            shiftdetail.Endtime = model.endtime;
            shiftdetail.Isdeleted = new BitArray(new[] { false });
            _context.Shiftdetails.Add(shiftdetail);
            _context.SaveChanges();

            var dayofweek = model.shiftdate.DayOfWeek.ToString();
            int valueforweek;
            if (dayofweek == "Sunday")
            {
                valueforweek = 0;
            }
            else if (dayofweek == "Monday")
            {
                valueforweek = 1;
            }
            else if (dayofweek == "Tuesday")
            {
                valueforweek = 2;
            }
            else if (dayofweek == "Wednesday")
            {
                valueforweek = 3;
            }
            else if (dayofweek == "Thursday")
            {
                valueforweek = 4;
            }
            else if (dayofweek == "Friday")
            {
                valueforweek = 5;
            }
            else
            {
                valueforweek = 6;
            }
            if (shift.Isrepeat == new BitArray(new[] { true }))
            {
                for (int j = 0; j < shift.Weekdays.Count(); j++)
                {
                    var z = shift.Weekdays;
                    var p = shift.Weekdays.ElementAt(j).ToString();
                    int ele = Int32.Parse(p);
                    int x;
                    if (valueforweek > ele)
                    {
                        x = 6 - valueforweek + 1 + ele;
                    }
                    else
                    {
                        x = ele - valueforweek;
                    }
                    if (x == 0)
                    {
                        x = 7;
                    }
                    DateTime newcurdate = model.shiftdate.AddDays(x);
                    for (int i = 0; i < model.repeatcount; i++)
                    {
                        Shiftdetail shiftdetailnew = new Shiftdetail
                        {
                            Shiftid = shift.Shiftid,
                            Shiftdate = DateOnly.FromDateTime(newcurdate),
                            Regionid = model.regionid,
                            Starttime = new DateTime(0, 0, 0, model.starttime.Hour, model.starttime.Minute, model.starttime.Second),
                            Endtime = new DateTime(0, 0, 0, model.endtime.Hour, model.endtime.Minute, model.endtime.Second),
                            Isdeleted = new BitArray(new[] { false })
                        };
                        _context.Shiftdetails.Add(shiftdetailnew);
                        _context.SaveChanges();
                        newcurdate = newcurdate.AddDays(7);
                    }
                }
            }
            return RedirectToAction("Scheduling");
        }
    }
}
