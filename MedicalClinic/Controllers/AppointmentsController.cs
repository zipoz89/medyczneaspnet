using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalClinic.Data;
using MedicalClinic.Models;
using Microsoft.AspNetCore.Identity;

namespace MedicalClinic.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AppointmentsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Book()
        {
            var users = await _userManager.Users.ToListAsync();
            var doctors = new List<ApplicationUser>();
            foreach (ApplicationUser user in users)
            {
                var roles = await GetUserRoles(user);

                if (roles.Contains("Doctor"))
                {
                    doctors.Add(user);
                }
            }
            return View(doctors);
        }


        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        private string GetDoctorFullnameById(string doctorId)
        {
            string fullName = "";

            fullName += GetUserById(doctorId).Result.FirstName + " " + GetUserById(doctorId).Result.LastName;

            return fullName;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        private async Task<ApplicationUser> GetUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var appointmentListWithDoctorName = new List<Tuple<String, Appointment>>();
            var appointmentList = await _context.Appointment.ToListAsync();
            for (int i = 0; i < appointmentList.Count; i++)
            {
                if (appointmentList[i].PatientId == GetCurrentUser().Result.Id && !appointmentList[i].WasHeld)
                {
                    Tuple<String, Appointment> appointment = new Tuple<String, Appointment>("DR. " + GetDoctorFullnameById(appointmentList[i].DoctorId), appointmentList[i]);
                    appointmentListWithDoctorName.Add(appointment);
                }
            }

            return View(appointmentListWithDoctorName);
        }

        // GET: held Appointments
        public async Task<IActionResult> HeldVisits()
        {
            var appointmentListWithDoctorName = new List<Tuple<String, Appointment>>();
            var appointmentList = await _context.Appointment.ToListAsync();
            for (int i = 0; i < appointmentList.Count; i++)
            {
                if (appointmentList[i].PatientId == GetCurrentUser().Result.Id && appointmentList[i].WasHeld)
                {
                    Tuple<String, Appointment> appointment = new Tuple<String, Appointment>("DR. " + GetDoctorFullnameById(appointmentList[i].DoctorId), appointmentList[i]);
                    appointmentListWithDoctorName.Add(appointment);
                }
            }

            return View(appointmentListWithDoctorName);
        }

        // GET: Appointments
        public async Task<IActionResult> Doctor()
        {
            var appointmentListWithPatientName = new List<Tuple<String, Appointment>>();
            var appointmentList = await _context.Appointment.ToListAsync();
            for (int i = 0; i < appointmentList.Count; i++)
            {
                if (appointmentList[i].DoctorId == GetCurrentUser().Result.Id && !appointmentList[i].WasHeld)
                {
                    Tuple<String, Appointment> appointment = new Tuple<String, Appointment>(GetDoctorFullnameById(appointmentList[i].PatientId), appointmentList[i]);
                    appointmentListWithPatientName.Add(appointment);
                }
            }

            return View(appointmentListWithPatientName);
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }


            Tuple<String, Appointment> detailsWithDoctorName = new Tuple<string, Appointment>(GetDoctorFullnameById(appointment.DoctorId), appointment);
            return View(detailsWithDoctorName);
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> VisitNote(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }


            Tuple<String, Appointment> detailsWithDoctorName = new Tuple<string, Appointment>(GetDoctorFullnameById(appointment.DoctorId), appointment);
            return View(detailsWithDoctorName);
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create(string doctorId)
        {
            var users = await _userManager.Users.ToListAsync();

            foreach (var item in users)
            {
                if (doctorId == item.Id)
                {
                    ViewBag.doctorId = doctorId;
                    ViewBag.doctorName = item.FirstName + " " + item.LastName;
                    ViewBag.doctorPhoto = item.ProfilePicture;
                }
            }
            ViewBag.DateError = 0;
            Appointment model = new Appointment();
            model.DoctorId = doctorId;
            return View(model);
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DoctorId,PatientId,Reason,Date")] Appointment appointment, string doctorId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            appointment.PatientId = user.Id;
            appointment.DoctorId = doctorId;
            appointment.WasHeld = false;
            appointment.Note = "";
            var dateValid = await CheckIfDateIsValidAsync(doctorId, appointment.Date);
            if (dateValid!= 0)
            {
                Appointment model = new Appointment();
                model.DoctorId = doctorId;
                var users = await _userManager.Users.ToListAsync();
                foreach (var item in users)
                {
                    if (doctorId == item.Id)
                    {
                        ViewBag.doctorId = doctorId;
                        ViewBag.doctorName = item.FirstName + " " + item.LastName;
                        ViewBag.doctorPhoto = item.ProfilePicture;
                        ViewBag.DateError = dateValid;
                    }
                }
                return View(model);
            }
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appointment);
        }

        private async Task<int> CheckIfDateIsValidAsync(string doctorId, DateTime date)
        {
            System.Diagnostics.Debug.WriteLine(date.Hour + " " +date.Minute);
            if (date.CompareTo(DateTime.Today) <= 0)
            {
                System.Diagnostics.Debug.WriteLine("later than today");
                return 1;
            }

            if (date.Hour < 9 || date.Hour > 17) 
            {
                System.Diagnostics.Debug.WriteLine("clinc is closed");
                return 2;
            }
            if (date.Minute != 0 && date.Minute != 30) 
            {
                System.Diagnostics.Debug.WriteLine("must be 0 or 30");
                return 3;
            }

            var appointmentList = await _context.Appointment.ToListAsync();
            for (int i = 0; i < appointmentList.Count; i++)
            {
                if (appointmentList[i].DoctorId == doctorId)
                {
                    if((date.CompareTo(appointmentList[i].Date) >= 0) && (date.CompareTo(appointmentList[i].Date.AddMinutes(30)) <= 0))
                        return 4;
                }
            }


            return 0;
        }





        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DoctorId,PatientId,Reason,Date")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        public async Task<IActionResult> DeleteDoctor(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Manage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }
            var patient = await GetUserById(appointment.PatientId);
            ViewBag.PatientAvatar = patient.ProfilePicture;

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointment.FindAsync(id);
            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("DeleteDoctor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDoctorConfirmed(int id)
        {
            var appointment = await _context.Appointment.FindAsync(id);
            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Doctor));
        }

        [HttpPost, ActionName("Manage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageConfirmed(int id,string note)
        {
            var appointment = await _context.Appointment.FindAsync(id);
            appointment.WasHeld = true;
            appointment.Note = note;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Doctor));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointment.Any(e => e.Id == id);
        }
    }
}
