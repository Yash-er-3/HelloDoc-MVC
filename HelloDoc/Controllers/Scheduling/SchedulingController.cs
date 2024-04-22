using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Viewmodels;
using System.Collections;
using System.Globalization;

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
            if (HttpContext.Session.GetInt32("PhysicianId") != null)
            {
                var phyid = HttpContext.Session.GetInt32("PhysicianId");
                MonthWiseScheduling month = new MonthWiseScheduling
                {
                    date = currentDate,
                };
                //if (regionid != 0 && status != 0)
                //{
                //    var dataphy = _context.ShiftDetails.Include(u => u.Shift).Where(m => m.RegionId == regionid && m.Status == status && m.IsDeleted != new BitArray(new[] { true })).ToList();

                //    month.shiftdetails = ;
                //}
                //else if (regionid != 0)
                //{
                //    month.shiftdetails = _context.ShiftDetails.Include(u => u.Shift.PhysicianId).Where(m => m.IsDeleted != new BitArray(new[] { true }) && m.Shift.PhysicianId == phyid).ToList();

                //}
                if (status != 0)
                {
                    month.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Status == status && m.Isdeleted != new BitArray(new[] { true }) && m.Shift.Physicianid == phyid).ToList();

                }
                else
                {
                    month.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Isdeleted != new BitArray(new[] { true }) && m.Shift.Physicianid == phyid).ToList();
                }
                return PartialView("_MonthWise", month);
            }
            else
            {
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
        }

        public List<Physician> filterregion(string regionid)
        {
            List<Physician> physicians = _context.Physicianregions.Where(u => u.Regionid.ToString() == regionid).Select(y => y.Physician).ToList();
            return physicians;
        }

        public IActionResult AddShift(SchedulingViewModel model)
        {
            var idcheck = "";
            var adminid = HttpContext.Session.GetInt32("AdminId");
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");
            if (adminid != null)
            {
                var admin = _context.Admins.FirstOrDefault(m => m.Adminid == adminid);
                idcheck = admin.Aspnetuserid;
                //AspNetUser aspnetadmin = _context.AspNetUsers.FirstOrDefault(m => m.Id == admin.AspNetUserId);
            }
            else
            {
                var phy = _context.Physicians.FirstOrDefault(m => m.Physicianid == physicianid);
                idcheck = phy.Aspnetuserid;
            }
            Aspnetuser aspnetadmin = _context.Aspnetusers.FirstOrDefault(m => m.Id == idcheck);
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
                                if (adminid != null)
                                {
                                    return RedirectToAction("Scheduling");
                                }
                                else
                                {
                                    return RedirectToAction("MyScheduling", "ProviderSide");
                                }
                            }
                        }
                    }
                }
            }
            Shift shift = new Shift
            {
                Startdate = DateOnly.FromDateTime(model.shiftdate),
                Repeatupto = model.repeatcount,
                Createddate = DateTime.Now,
                Createdby = aspnetadmin.Id
            };
            if (adminid != null)
            {
                shift.Physicianid = model.providerid;
            }
            else
            {
                shift.Physicianid = (int)physicianid;
            }
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
            if (adminid != null)
            {
                return RedirectToAction("Scheduling");
            }
            else
            {
                return RedirectToAction("MyScheduling", "ProviderSide");
            }
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
                endtime = shiftdata.Endtime
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
                update.Isdeleted = new BitArray(new[] { true });
                _context.Shiftdetails.Update(update);

            }


            _context.SaveChanges();

            return;

        }

        public IActionResult RequestedShifts()
        {
            ShiftForReviewModel modal = new ShiftForReviewModel();
            modal.regions = _context.Regions.ToList();


            return View(modal);
        }

        public int RequestedShiftsCount(string currentPartial, string filterDate, int regionid)
        {
            ShiftForReviewModel modal = new ShiftForReviewModel();
            modal.regions = _context.Regions.ToList();
            modal.physicians = _context.Physicians.ToList();

            DateOnly date = DateOnly.ParseExact(filterDate.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            List<Shiftdetail> shiftreviewdata = new List<Shiftdetail>();





            if (currentPartial == "_DayWise")
            {
                shiftreviewdata = _context.Shiftdetails.Include(s => s.Shift).Where(s => s.Shiftdate == date && s.Status == 1 && s.Isdeleted != new BitArray(new[] { true })).ToList();

            }
            else if (currentPartial == "_WeekWise")
            {
                var prevsunday = date.AddDays(-(int)date.DayOfWeek);
                var nextsunday = date.AddDays(7 - (int)date.DayOfWeek);

                shiftreviewdata = _context.Shiftdetails.Include(s => s.Shift).Where(s => s.Shiftdate >= prevsunday && s.Shiftdate < nextsunday && s.Status == 1 && s.Isdeleted != new BitArray(new[] { true })).ToList();
            }
            else if (currentPartial == "_MonthWise")
            {
                var monthStart = new DateOnly(date.Year, date.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                shiftreviewdata = _context.Shiftdetails.Include(s => s.Shift).Where(s => s.Shiftdate >= monthStart && s.Shiftdate <= monthEnd && s.Status == 1 && s.Isdeleted != new BitArray(new[] { true })).ToList();
            }

            if (regionid != 0)
            {
                shiftreviewdata = shiftreviewdata.Where(x => x.Regionid == regionid).ToList();
            }

            modal.shiftreviewlist = shiftreviewdata;

            return shiftreviewdata.Count();
        }


        public IActionResult RequestedShiftsPagination(string currentPartial, string filterDate, int regionid, int page, int pageSize)
        {
            ShiftForReviewModel modal = new ShiftForReviewModel();
            modal.regions = _context.Regions.ToList();
            modal.physicians = _context.Physicians.ToList();

            DateOnly date = DateOnly.ParseExact(filterDate.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            List<Shiftdetail> shiftreviewdata = new List<Shiftdetail>();


            if (currentPartial == "_DayWise")
            {
                shiftreviewdata = _context.Shiftdetails.Include(s => s.Shift).Where(s => s.Shiftdate == date && s.Status == 1 && s.Isdeleted != new BitArray(new[] { true })).ToList();
            }
            else if (currentPartial == "_WeekWise")
            {
                var prevsunday = date.AddDays(-(int)date.DayOfWeek);
                var nextsunday = date.AddDays(7 - (int)date.DayOfWeek);

                shiftreviewdata = _context.Shiftdetails.Include(s => s.Shift).Where(s => s.Shiftdate >= prevsunday && s.Shiftdate < nextsunday && s.Status == 1 && s.Isdeleted != new BitArray(new[] { true })).ToList();
            }
            else if (currentPartial == "_MonthWise")
            {
                var monthStart = new DateOnly(date.Year, date.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                shiftreviewdata = _context.Shiftdetails.Include(s => s.Shift).Where(s => s.Shiftdate >= monthStart && s.Shiftdate <= monthEnd && s.Status == 1 && s.Isdeleted != new BitArray(new[] { true })).ToList();
            }

            if (regionid != 0)
            {
                shiftreviewdata = shiftreviewdata.Where(x => x.Regionid == regionid).ToList();

            }

            modal.shiftreviewlist = shiftreviewdata;

            var startIndex = (page - 1) * pageSize;

            var pagedata = shiftreviewdata.Skip(startIndex).Take(pageSize).ToList();

            modal.shiftreviewlist = pagedata;

            return PartialView("_RequestShiftTable", modal);
        }

        public IActionResult ApproveSelectedShift(List<string> selectedshiftvalues, string clickvalue)
        {
            if (clickvalue == "approve")
            {
                foreach (var shiftdetailid in selectedshiftvalues)
                {
                    Shiftdetail x = _context.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == int.Parse(shiftdetailid));
                    x.Status = 2;
                    _context.Shiftdetails.Update(x);

                }
            }


            else if (clickvalue == "delete")
            {
                foreach (var shiftdetailid in selectedshiftvalues)
                {
                    Shiftdetail x = _context.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == int.Parse(shiftdetailid));
                    x.Isdeleted = new BitArray(new[] { true });
                    _context.Shiftdetails.Update(x);

                }
            }


            _context.SaveChanges();

            return RedirectToAction("");
        }


        //provider on call

        public IActionResult ProviderOnCall(string PartialName, string date, int regionid, int status)
        {
            status = 2;
            ProviderOnCall model = new ProviderOnCall();
            DateOnly dateOnly;
            IEnumerable<Physician> physicianlist = new List<Physician>();
            if (regionid != 0)
            {
                physicianlist = _context.Physicians.Where(m => m.Regionid == regionid).ToList();
            }
            else
            {
                physicianlist = _context.Physicians.ToList();
            }
            if (PartialName == "_WeekWise")
            {
                DateTime dateTime = DateTime.Parse(date);
                dateOnly = DateOnly.FromDateTime(dateTime.AddDays(-(int)dateTime.DayOfWeek + (int)DayOfWeek.Sunday));
            }
            else if (PartialName == "_MonthWise")
            {
                dateOnly = DateOnly.FromDateTime(DateTime.Parse(date).AddDays(-DateTime.Parse(date).Day + 1));
            }
            else
            {
                dateOnly = DateOnly.ParseExact(date.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            if (PartialName == "_DayWise")
            {

                List<Shiftdetail> shiftdetaillist = new List<Shiftdetail>();
                if (status != 0 && regionid != 0)
                {
                    shiftdetaillist = _context.Shiftdetails.Include(s => s.Shift).Where(m => m.Status == status && m.Isdeleted != new BitArray(new[] { true }) && m.Regionid == regionid && m.Shiftdate == dateOnly).ToList();
                }
                else if (regionid != 0)
                {

                    shiftdetaillist = _context.Shiftdetails.Include(s => s.Shift).Where(m => m.Regionid == regionid && m.Isdeleted != new BitArray(new[] { true }) && m.Shiftdate == dateOnly).ToList();
                }
                else if (status != 0)
                {
                    shiftdetaillist = _context.Shiftdetails.Include(s => s.Shift).Where(m => m.Status == status && m.Isdeleted != new BitArray(new[] { true }) && m.Shiftdate == dateOnly).ToList();

                }
                else
                {
                    shiftdetaillist = _context.Shiftdetails.Include(s => s.Shift).Where(m => m.Shiftdate == dateOnly && m.Isdeleted != new BitArray(new[] { true })).ToList();
                }

                IEnumerable<Physician> ondutyphysician = new List<Physician>();
                foreach (var item in shiftdetaillist)
                {
                    var x = _context.Physicians.Where(m => m.Physicianid == item.Shift.Physicianid).ToList();
                    ondutyphysician = ondutyphysician.Concat(x);
                }
                model.offdutyphysicianlist = physicianlist.Except(ondutyphysician);
                model.ondutyphysicianlist = ondutyphysician.Distinct();
            }

            if (PartialName == "_WeekWise")
            {
                List<DateOnly> weekDates = new List<DateOnly>();
                for (int i = 0; i < 7; i++)
                {
                    weekDates.Add(dateOnly.AddDays(i));
                }
                List<Shiftdetail> weekShiftdetaillist = new List<Shiftdetail>();
                foreach (var weekDate in weekDates)
                {
                    List<Shiftdetail> tempShiftdetaillist = new List<Shiftdetail>();
                    if (status != 0 && regionid != 0)
                    {
                        tempShiftdetaillist = _context.Shiftdetails.Include(s => s.Shift).Where(m => m.Status == status && m.Isdeleted != new BitArray(new[] { true }) && m.Regionid == regionid && m.Shiftdate == weekDate).ToList();
                    }
                    else if (regionid != 0)
                    {
                        tempShiftdetaillist = _context.Shiftdetails.Include(s => s.Shift).Where(m => m.Regionid == regionid && m.Isdeleted != new BitArray(new[] { true }) && m.Shiftdate == weekDate).ToList();

                    }
                    else if (status != 0)
                    {
                        tempShiftdetaillist = _context.Shiftdetails.Include(s => s.Shift).Where(m => m.Status == status && m.Isdeleted != new BitArray(new[] { true }) && m.Shiftdate == weekDate).ToList();

                    }
                    else
                    {
                        tempShiftdetaillist = _context.Shiftdetails.Include(s => s.Shift).Where(m => m.Shiftdate == weekDate && m.Isdeleted != new BitArray(new[] { true })).ToList();
                    }
                    weekShiftdetaillist.AddRange(tempShiftdetaillist);
                }
                IEnumerable<Physician> weekondutyphysician = new List<Physician>();
                foreach (var item in weekShiftdetaillist)
                {
                    var x = _context.Physicians.Where(m => m.Physicianid == item.Shift.Physicianid).ToList();
                    weekondutyphysician = weekondutyphysician.Concat(x);
                }
                model.offdutyphysicianlist = physicianlist.Except(weekondutyphysician);
                model.ondutyphysicianlist = weekondutyphysician.Distinct();
            }
            if (PartialName == "_MonthWise")
            {
                List<DateOnly> monthDates = new List<DateOnly>();
                for (int i = 1; i <= DateTime.DaysInMonth(dateOnly.Year, dateOnly.Month); i++)
                {
                    monthDates.Add(dateOnly.AddDays(i - 1));
                }

                List<Shiftdetail> monthShiftdetaillist = new List<Shiftdetail>();
                foreach (var monthDate in monthDates)
                {
                    List<Shiftdetail> tempShiftdetaillist = new List<Shiftdetail>();
                    if (status != 0 && regionid != 0)
                    {
                        tempShiftdetaillist = _context.Shiftdetails.Include(s => s.Shift).Where(m => m.Status == status && m.Isdeleted != new BitArray(new[] { true }) && m.Regionid == regionid && m.Shiftdate == monthDate).ToList();
                    }
                    else if (regionid != 0)
                    {
                        tempShiftdetaillist = _context.Shiftdetails.Include(s => s.Shift).Where(m => m.Regionid == regionid && m.Isdeleted != new BitArray(new[] { true }) && m.Shiftdate == monthDate).ToList();

                    }
                    else if (status != 0)
                    {
                        tempShiftdetaillist = _context.Shiftdetails.Include(s => s.Shift).Where(m => m.Regionid == regionid && m.Isdeleted != new BitArray(new[] { true }) && m.Shiftdate == monthDate).ToList();

                    }
                    else
                    {
                        tempShiftdetaillist = _context.Shiftdetails.Include(s => s.Shift).Where(m => m.Shiftdate == monthDate && m.Isdeleted != new BitArray(new[] { true })).ToList();
                    }
                    monthShiftdetaillist.AddRange(tempShiftdetaillist);
                }

                IEnumerable<Physician> monthondutyphysician = new List<Physician>();
                foreach (var item in monthShiftdetaillist)
                {
                    var x = _context.Physicians.Where(m => m.Physicianid == item.Shift.Physicianid).ToList();
                    monthondutyphysician = monthondutyphysician.Concat(x);
                }
                model.offdutyphysicianlist = physicianlist.Except(monthondutyphysician);
                model.ondutyphysicianlist = monthondutyphysician.Distinct();
            }
            model.regions = _context.Regions.ToList();
            model.selectedRegionid = regionid;
            return View(model);
        }

    }
}
