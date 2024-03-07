﻿using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{

    public class AddOrUpdateRequestStatusLog : Repository<Requeststatuslog>, IAddOrUpdateRequestStatusLog
    {
        private readonly HelloDocDbContext _context;
        public AddOrUpdateRequestStatusLog(HelloDocDbContext context) : base(context)
        {
            _context = context;
        }

        void IAddOrUpdateRequestStatusLog.AddOrUpdateRequestStatusLog(int requestid, int? APId, string cancelnote, int? transtophyid = null)
        {
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == requestid);
            if (request != null)
            {
                var data = new Requeststatuslog
                {
                    Requestid = requestid,
                    Status = request.Status,
                    Adminid = APId,
                    Notes = cancelnote,
                    Createddate = DateTime.Now
                };
                if (transtophyid != null)
                {
                    data.Transtophysicianid = transtophyid;
                };
                _context.Requeststatuslogs.Add(data);
                _context.SaveChanges();
            }

        }
    }

}