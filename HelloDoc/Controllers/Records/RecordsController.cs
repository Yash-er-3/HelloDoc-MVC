using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Viewmodels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections;
using Services.Implementation;

namespace HelloDoc.Controllers.Records
{
    public class RecordsController : Controller
    {
        private readonly IunitOfWork _unit;
        public RecordsController(IunitOfWork unit, IRecordRepository Record)
        {
            _unit = unit;
        }
        public IActionResult SearchRecords()
        {
            return View();
        }

        public IActionResult SearchRecordsFilter(
            int reqstatus,
            string patientname,
            int requesttype,
            string fromdateofservice,
            string todateofservice,
            string physicianname,
            string email,
            string phonenumber
            )
        {
            List<RecordViewModel> records = new List<RecordViewModel>();

            records = _unit.records.GetSearchRecordData(reqstatus, patientname, requesttype, fromdateofservice, todateofservice, physicianname, email, phonenumber);



            return PartialView("_SearchRecordsTable", records);
        }


        public string DownloadSearchRecord(
             int reqstatus,
            string patientname,
            int requesttype,
            string fromdateofservice,
            string todateofservice,
            string physicianname,
            string email,
            string phonenumber)
        {

            List<RecordViewModel> records = new List<RecordViewModel>();

            records = _unit.records.GetSearchRecordData(reqstatus, patientname, requesttype, fromdateofservice, todateofservice, physicianname, email, phonenumber);

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet1");

            // Create header row
            IRow headerRow = sheet1.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("Patient Name");
            headerRow.CreateCell(1).SetCellValue("Requestor");
            headerRow.CreateCell(2).SetCellValue("Date Of Service");
            headerRow.CreateCell(3).SetCellValue("Close Case Date");
            headerRow.CreateCell(4).SetCellValue("Email");
            headerRow.CreateCell(5).SetCellValue("Phone Number");
            headerRow.CreateCell(6).SetCellValue("Address");
            headerRow.CreateCell(7).SetCellValue("Zip");
            headerRow.CreateCell(8).SetCellValue("Request Status");
            headerRow.CreateCell(9).SetCellValue("Physician");
            headerRow.CreateCell(10).SetCellValue("Physician Note");
            headerRow.CreateCell(11).SetCellValue("Cancelled By Provider Note");
            headerRow.CreateCell(12).SetCellValue("Admin Note");
            headerRow.CreateCell(13).SetCellValue("Patient Note");
            // Add more headers here...

            // Add data rows
            for (int i = 0; i < records.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);  // Start from the second row

                row.CreateCell(0).SetCellValue(records[i].PatientName);
                row.CreateCell(1).SetCellValue(records[i].Requestor);
                row.CreateCell(2).SetCellValue(records[i].DateOfService);
                row.CreateCell(3).SetCellValue(records[i].CloseCaseDate);
                row.CreateCell(4).SetCellValue(records[i].Email);
                row.CreateCell(5).SetCellValue(records[i].PhoneNumber);
                row.CreateCell(6).SetCellValue(records[i].Address);
                row.CreateCell(7).SetCellValue(records[i].Zip);
                row.CreateCell(8).SetCellValue(records[i].RequestStatus);
                row.CreateCell(9).SetCellValue(records[i].Physician);
                row.CreateCell(10).SetCellValue(records[i].PhysicianNote);
                row.CreateCell(11).SetCellValue(records[i].CancelProviderNote);
                row.CreateCell(12).SetCellValue(records[i].AdminNote);
                row.CreateCell(13).SetCellValue(records[i].PatientNote);
                // Add more cells here...
            }

            var date = DateTime.Now.ToString("hh : mm") + "Search Records";


            using (var stream = new MemoryStream())
            {
                workbook.Write(stream);
                var content = stream.ToArray();

                return Convert.ToBase64String(content);
            }
        }

        public IActionResult DeleteRecords(int requestid)
        {

            _unit.records.DeleteSearchRecord(requestid);

            List<RecordViewModel> records = new List<RecordViewModel>();

            records = _unit.records.GetSearchRecordData(0, null, 0, null, null, null, null, null); ;

            return PartialView("_SearchRecordsTable", records);
        }

        //block History

        public IActionResult BlockHistory()
        {
            return View();
        }

        public IActionResult BlockHistoryFilter(string name, string date, string email, string phonenumber)
        {
            List<RecordViewModel> blockhistorydata = new List<RecordViewModel>();

            blockhistorydata = _unit.records.GetBlockHistoryFilterData(name, date, email, phonenumber);

            return PartialView("_BlockHistoryTable", blockhistorydata);

        }

        public IActionResult UnblockbtnBlockHistory(int requestid)
        {
            int adminid = (int)HttpContext.Session.GetInt32("AdminId");

            int yesorno = _unit.records.UnblockBlockHistory(requestid, adminid);

            if (yesorno != 1)
            {
                TempData["success"] = "Some Error Occured";
            }

            List<RecordViewModel> blockhistorydata = new List<RecordViewModel>();

            blockhistorydata = _unit.records.GetBlockHistoryFilterData(null, null, null, null);

            return PartialView("_BlockHistoryTable", blockhistorydata);
        }

      

        public IActionResult PatientHistory()
        {
            return View();
        }
        public IActionResult GetPatientGistoryTable(string firstname, string lastname, string email, int phone)
        {
            PatientHistoryViewModel viewModel = new PatientHistoryViewModel();
            viewModel.UserList = _unit.records.GetUserList(firstname, lastname, email, phone);
            return PartialView("_PatientHistoryTable", viewModel);
        }
        public IActionResult PatientRecord(int id)
        {
            List<PatientRecordViewModel> records = new List<PatientRecordViewModel>();
            records = _unit.records.GetPatientRecordData(id);
            return View(records);
        }
        public IActionResult EmailLog()
        {
            var model = _unit.records.GetAspNetRoleList();
            return View(model);
        }
        [HttpGet]
        //public IActionResult GetEmailLogTable(int Role, string ReceiverName, string Email, string CreatedDate, string SentData)
        public IActionResult GetEmailLogTable(int Role, string ReceiverName, string Email, DateTime CreatedDate, DateTime SentDate)
        {
            var model = _unit.records.GetEmailLogs(Role, ReceiverName, Email, CreatedDate, SentDate);
            return PartialView("_EmailLogTable", model);
        }
        public IActionResult SMSLog()
        {
            var model = _unit.records.GetAspNetRoleList();
            return View(model);
        }
        [HttpGet]
        //public IActionResult GetEmailLogTable(int Role, string ReceiverName, string Email, string CreatedDate, string SentData)
        public IActionResult GetSMSLogTable(int Role, string ReceiverName, string Email, DateTime CreatedDate, DateTime SentDate)
        {
            var model = _unit.records.GetSMSLogs(Role, ReceiverName, Email, CreatedDate, SentDate);
            return PartialView("_SMSLogTable", model);
        }

    }
}
