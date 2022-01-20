using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalClinic.Models
{
    public class Visit
    {
        public string Id { get; set; }
        public string PatiendId { get; set; }
        public string DoctorId { get; set; }

        public DateTime Date { get; set; }

        public int VisitTimeInMinutes { get; set; }

        public string VisitReason { get; set; }

        public Visit()
        {

        }
    }
}
