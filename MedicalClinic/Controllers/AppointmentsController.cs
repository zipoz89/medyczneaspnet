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

        public async Task<IActionResult> BookDoctor(string userId)
        {
            var users = await _userManager.Users.ToListAsync();

            foreach (var item in users)
            {
                if (userId == item.Id)
                {
                    ViewBag.doctorName = item.FirstName + " " + item.LastName;
                    ViewBag.doctorPhoto = item.ProfilePicture;
                }
            }
            Appointment model = new Appointment();
            model.DoctorId = userId;
            return View(model);
        }

        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Appointment.ToListAsync());
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

            return View(appointment);
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
            Appointment model = new Appointment();
            model.DoctorId = doctorId;
            return View(model);
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DoctorId,PatientId,Reason,Date")] Appointment appointment,string doctorId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            appointment.PatientId = user.Id;
            appointment.DoctorId = doctorId;
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appointment);
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

        private bool AppointmentExists(int id)
        {
            return _context.Appointment.Any(e => e.Id == id);
        }
    }
}
