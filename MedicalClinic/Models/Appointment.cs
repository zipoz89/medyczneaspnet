using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalClinic.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public string Reason { get; set; }
        public DateTime Date{ get; set; }

        public bool WasHeld { get; set; }
        public string Note { get; set; }
        public Appointment()
        {

        }
    }
}
