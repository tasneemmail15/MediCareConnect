using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MediCareConnect.Data;
using MediCareConnect.Models;

namespace MediCareConnect.Controllers
{
    public class PrescriptionsController : Controller
    {
        private readonly MediCareContext _context;

        public PrescriptionsController(MediCareContext context)
        {
            _context = context;
        }

        // GET: Prescriptions
        public async Task<IActionResult> Index()
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.Medication)
                .Include(p => p.Diagnosis)
                .ToListAsync();
            return View(prescriptions);
        }

        // GET: Prescriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.Medication)
                .Include(p => p.Diagnosis)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        // GET: Prescriptions/Create
        public IActionResult Create()
        {
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Name");
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Name");
            ViewData["MedicationId"] = new SelectList(_context.Medications, "Id", "Name");
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnoses, "Id", "Condition");
            return View();
        }

        // POST: Prescriptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PatientId,DoctorId,MedicationId,DiagnosisId,PrescriptionDate,EndDate,Instructions")] Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prescription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Name", prescription.PatientId);
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Name", prescription.DoctorId);
            ViewData["MedicationId"] = new SelectList(_context.Medications, "Id", "Name", prescription.MedicationId);
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnoses, "Id", "Condition", prescription.DiagnosisId);
            return View(prescription);
        }

        // GET: Prescriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Name", prescription.PatientId);
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Name", prescription.DoctorId);
            ViewData["MedicationId"] = new SelectList(_context.Medications, "Id", "Name", prescription.MedicationId);
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnoses, "Id", "Condition", prescription.DiagnosisId);
            return View(prescription);
        }

        // POST: Prescriptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientId,DoctorId,MedicationId,DiagnosisId,PrescriptionDate,EndDate,Instructions")] Prescription prescription)
        {
            if (id != prescription.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prescription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrescriptionExists(prescription.Id))
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
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Name", prescription.PatientId);
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Name", prescription.DoctorId);
            ViewData["MedicationId"] = new SelectList(_context.Medications, "Id", "Name", prescription.MedicationId);
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnoses, "Id", "Condition", prescription.DiagnosisId);
            return View(prescription);
        }

        // GET: Prescriptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.Medication)
                .Include(p => p.Diagnosis)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        // POST: Prescriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrescriptionExists(int id)
        {
            return _context.Prescriptions.Any(e => e.Id == id);
        }
    }
}