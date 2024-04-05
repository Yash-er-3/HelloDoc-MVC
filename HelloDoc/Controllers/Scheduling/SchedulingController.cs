using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Viewmodels;
using System;
using System.Collections;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Vonage.ProactiveConnect.Lists.SyncStatus;
using static Vonage.VonageUrls;

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


        public IActionResult LoadSchedulingPartial(string PartialName, string date, int regionid, int status)
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

                    };
                    if (regionid != 0 && status != 0)
                    {
                        day.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Regionid == regionid && m.Status == status).ToList();
                    }
                    else if (regionid != 0)
                    {
                        day.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Regionid == regionid).ToList();

                    }
                    else if (status != 0)
                    {
                        day.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Status == status).ToList();

                    }
                    else
                    {
                        day.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).ToList();
                    }
                    return PartialView("_DayWise", day);

                case "_WeekWise":
                    WeekWiseScheduling week = new WeekWiseScheduling
                    {
                        date = currentDate,
                        physicians = physician,

                    };

                    if (regionid != 0 && status != 0)
                    {
                        week.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Regionid == regionid && m.Status == status).ToList();
                    }
                    else if (regionid != 0)
                    {
                        week.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Regionid == regionid).ToList();

                    }
                    else if (status != 0)
                    {
                        week.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Status == status).ToList();

                    }
                    else
                    {
                        week.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).ToList();
                    }
                    return PartialView("_WeekWise", week);

                case "_MonthWise":
                    MonthWiseScheduling month = new MonthWiseScheduling
                    {
                        date = currentDate,
                        physicians = physician,
                    };
                    if (regionid != 0 && status != 0)
                    {
                        month.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Regionid == regionid && m.Status == status).ToList();
                    }
                    else if (regionid != 0)
                    {
                        month.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Regionid == regionid).ToList();

                    }
                    else if (status != 0)
                    {
                        month.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Status == status).ToList();

                    }
                    else
                    {
                        month.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).ToList();
                    }
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
            shiftdetail.Status = 1;
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
            if (shift.Isrepeat[0] == true)
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
                            Starttime = new DateTime(newcurdate.Year, newcurdate.Month, newcurdate.Day, model.starttime.Hour, model.starttime.Minute, model.starttime.Second),
                            Endtime = new DateTime(newcurdate.Year, newcurdate.Month, newcurdate.Day, model.endtime.Hour, model.endtime.Minute, model.endtime.Second),
                            Isdeleted = new BitArray(new[] { false }),
                            Status = 1
                        };
                        _context.Shiftdetails.Add(shiftdetailnew);
                        _context.SaveChanges();
                        newcurdate = newcurdate.AddDays(7);
                    }
                }
            }
            return RedirectToAction("Scheduling");
        }
        public SchedulingViewModel ViewShiftOpen(int shiftdetailid)
        {

            Shiftdetail shiftdata = _context.Shiftdetails.Include(x => x.Shift).FirstOrDefault(s => s.Shiftdetailid == shiftdetailid);

            SchedulingViewModel model = new SchedulingViewModel
            {
                regionname = _context.Regions.FirstOrDefault(r => r.Regionid == shiftdata.Regionid).Regionid.ToString(),
                physicianname = _context.Physicians.FirstOrDefault(p => p.Physicianid == shiftdata.Shift.Physicianid).Firstname + " "
                                + _context.Physicians.FirstOrDefault(p => p.Physicianid == shiftdata.Shift.Physicianid).Lastname,
                shiftdateviewshift = shiftdata.Shiftdate,
                starttime = shiftdata.Starttime,
                endtime = shiftdata.Endtime,
            };

            return model;

        }

        public void viewShiftEdit(SchedulingViewModel model)
        {

            var update = _context.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == model.shiftdetailsid);

            if (model.eventvalue == "return")
            {
                if (update.Status == 1)
                {
                    update.Status = 2;
                }

                else if (update.Status == 2)
                {
                    update.Status = 1;

                }

                _context.Shiftdetails.Update(update);

            }
            if (model.eventvalue == "save")
            {


                update.Starttime = model.starttime;
                update.Endtime = model.endtime;
                _context.Shiftdetails.Update(update);

            }
            if (model.eventvalue == "delete")
            {
                _context.Shiftdetails.Remove(update);

            }


            _context.SaveChanges();

            return;

        }


        public IActionResult RequestedShifts(string currentPartial, string filterDate, int regionid)
        {
            ShiftForReviewModel modal = new ShiftForReviewModel();
            modal.regions = _context.Regions.ToList();
            modal.physicians = _context.Physicians.ToList();

            DateOnly date = DateOnly.ParseExact(filterDate.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            List<Shiftdetail> shiftreviewdata = new List<Shiftdetail>();


            if (regionid != 0)
            {


                shiftreviewdata = _context.Shiftdetails.Include(s => s.Shift).Where(s => s.Shiftdate == date && s.Status == 1 && s.Regionid == regionid).ToList();

            }
            else
            {

                if (currentPartial == "_DayWise")
                {
                    shiftreviewdata = _context.Shiftdetails.Include(s => s.Shift).Where(s => s.Shiftdate == date && s.Status == 1).ToList();
                }
                else if (currentPartial == "_WeekWise")
                {
                    var prevsunday = date.AddDays(-(int)date.DayOfWeek);
                    var nextsunday = date.AddDays(7 - (int)date.DayOfWeek);

                    shiftreviewdata = _context.Shiftdetails.Include(s => s.Shift).Where(s => s.Shiftdate >= prevsunday && s.Shiftdate < nextsunday && s.Status == 1).ToList();
                }
                 else if (currentPartial == "_MonthWise")
                {
                    var monthStart = new DateOnly(date.Year, date.Month, 1);
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                    shiftreviewdata = _context.Shiftdetails.Include(s => s.Shift).Where(s => s.Shiftdate >= monthStart && s.Shiftdate <= monthEnd && s.Status == 1).ToList();
                }


            }
            modal.shiftreviewlist = shiftreviewdata;


            return View(modal);
        }

        public IActionResult ApproveSelectedShift(List<string> selectedshiftvalues)
        {

            return View();
        }
    }
}
