using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalClinic.Models
{
    public class Visit
    {
        public int Id { get; set; }
        public int PatiendId { get; set; }
        public int DoctorId { get; set; }

        public DateTime Date { get; set; }

        public int VisitTimeInMinutes { get; set; }

        public Visit()
        {

        }
    }
}
