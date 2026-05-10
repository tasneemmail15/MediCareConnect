using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MediCareConnect.Data;
using MediCareConnect.Models;

namespace MediCareConnect.Controllers
{
    public class PatientsController : Controller
    {
        private readonly MediCareContext _context;

        public PatientsController(MediCareContext context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index(string searchString)
        {
            var patients = from p in _context.Patients
                           .Include(p => p.Appointments)
                               .ThenInclude(a => a.Doctor) // Include Doctor for Appointments
                           .Include(p => p.Prescriptions)
                               .ThenInclude(pr => pr.Medication) // Include Medication for Prescriptions
                           .Include(p => p.Diagnoses)
                           select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                patients = patients.Where(p => p.Name.Contains(searchString));
            }

            return View(await patients.ToListAsync());
        }


        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Medication)
                .Include(p => p.Diagnoses)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            PopulateRelatedData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Name,PhoneNumber,Email")] Patient patient,
            int[] selectedDoctors,
            DateTime[] appointmentDates,
            int[] selectedMedications,
            string[] prescriptionInstructions,
            DateTime[] prescriptionStartDates,
            DateTime[] prescriptionEndDates,
            int[] selectedDiagnoses)
        {
            if (ModelState.IsValid)
            {
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                // Handle Appointments
                if (selectedDoctors != null && appointmentDates != null)
                {
                    for (int i = 0; i < selectedDoctors.Length; i++)
                    {
                        if (i < appointmentDates.Length)
                        {
                            var appointment = new Appointment
                            {
                                PatientId = patient.Id,
                                DoctorId = selectedDoctors[i],
                                AppointmentDate = appointmentDates[i]
                            };
                            _context.Appointments.Add(appointment);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                // Handle Diagnoses
                var diagnosisIds = new List<int>();
                if (selectedDiagnoses != null && selectedDoctors != null && selectedDoctors.Any())
                {
                    foreach (var diagnosisId in selectedDiagnoses)
                    {
                        var condition = _context.Diagnoses.Find(diagnosisId)?.Condition;
                        if (condition != null)
                        {
                            var diagnosis = new Diagnosis
                            {
                                PatientId = patient.Id,
                                Condition = condition,
                                DiagnosisDate = DateTime.Now,
                                DoctorId = selectedDoctors.FirstOrDefault()
                            };

                            if (_context.Doctors.Any(d => d.Id == diagnosis.DoctorId))
                            {
                                _context.Diagnoses.Add(diagnosis);
                                await _context.SaveChangesAsync();
                                diagnosisIds.Add(diagnosis.Id);
                            }
                        }
                    }
                }

                // Handle Prescriptions
                if (selectedMedications != null && prescriptionInstructions != null && prescriptionStartDates != null && prescriptionEndDates != null && diagnosisIds.Any())
                {
                    for (int i = 0; i < selectedMedications.Length; i++)
                    {
                        if (i < prescriptionInstructions.Length && i < prescriptionStartDates.Length && i < prescriptionEndDates.Length)
                        {
                            var prescription = new Prescription
                            {
                                PatientId = patient.Id,
                                MedicationId = selectedMedications[i],
                                Instructions = prescriptionInstructions[i],
                                PrescriptionDate = prescriptionStartDates[i],  // Set StartDate
                                EndDate = prescriptionEndDates[i],      // Set EndDate
                                DiagnosisId = diagnosisIds.FirstOrDefault(),
                                DoctorId = selectedDoctors.FirstOrDefault()
                            };

                            if (_context.Medications.Any(m => m.Id == prescription.MedicationId) &&
                                _context.Doctors.Any(d => d.Id == prescription.DoctorId))
                            {
                                _context.Prescriptions.Add(prescription);
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateRelatedData();
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Medication)
                .Include(p => p.Diagnoses)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            PopulateRelatedData();
            return View(patient);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,PhoneNumber,Email")] Patient patient,
            int[] selectedDoctors,
            DateTime[] appointmentDates,
            int[] selectedMedications,
            string[] prescriptionInstructions,
            DateTime[] prescriptionStartDates,
            DateTime[] prescriptionEndDates,
            int[] selectedDiagnoses)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();

                    // Update Appointments
                    var existingAppointments = _context.Appointments.Where(a => a.PatientId == id).ToList();
                    _context.Appointments.RemoveRange(existingAppointments);

                    if (selectedDoctors != null && appointmentDates != null)
                    {
                        for (int i = 0; i < selectedDoctors.Length; i++)
                        {
                            if (i < appointmentDates.Length)
                            {
                                var appointment = new Appointment
                                {
                                    PatientId = patient.Id,
                                    DoctorId = selectedDoctors[i],
                                    AppointmentDate = appointmentDates[i]
                                };
                                _context.Appointments.Add(appointment);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    // Update Diagnoses
                    var existingDiagnoses = _context.Diagnoses.Where(d => d.PatientId == id).ToList();

                    // Remove related prescriptions first
                    var diagnosisIdsToRemove = existingDiagnoses.Select(d => d.Id).ToList();
                    var relatedPrescriptions = _context.Prescriptions.Where(p => diagnosisIdsToRemove.Contains(p.DiagnosisId)).ToList();
                    _context.Prescriptions.RemoveRange(relatedPrescriptions);

                    _context.Diagnoses.RemoveRange(existingDiagnoses);

                    var diagnosisIds = new List<int>();
                    if (selectedDiagnoses != null && selectedDoctors != null && selectedDoctors.Any())
                    {
                        foreach (var diagnosisId in selectedDiagnoses)
                        {
                            var condition = _context.Diagnoses.Find(diagnosisId)?.Condition;
                            if (condition != null)
                            {
                                var diagnosis = new Diagnosis
                                {
                                    PatientId = patient.Id,
                                    Condition = condition,
                                    DiagnosisDate = DateTime.Now,
                                    DoctorId = selectedDoctors.FirstOrDefault()
                                };

                                if (_context.Doctors.Any(d => d.Id == diagnosis.DoctorId))
                                {
                                    _context.Diagnoses.Add(diagnosis);
                                    await _context.SaveChangesAsync();
                                    diagnosisIds.Add(diagnosis.Id);
                                }
                            }
                        }
                    }

                    // Update Prescriptions
                    var existingPrescriptions = _context.Prescriptions.Where(p => p.PatientId == id).ToList();
                    _context.Prescriptions.RemoveRange(existingPrescriptions);

                    if (selectedMedications != null && prescriptionInstructions != null && prescriptionStartDates != null && prescriptionEndDates != null && diagnosisIds.Any())
                    {
                        for (int i = 0; i < selectedMedications.Length; i++)
                        {
                            if (i < prescriptionInstructions.Length && i < prescriptionStartDates.Length && i < prescriptionEndDates.Length)
                            {
                                var prescription = new Prescription
                                {
                                    PatientId = patient.Id,
                                    MedicationId = selectedMedications[i],
                                    Instructions = prescriptionInstructions[i],
                                    PrescriptionDate = prescriptionStartDates[i], // Use PrescriptionDate as StartDate
                                    EndDate = prescriptionEndDates[i],            // Set EndDate
                                    DiagnosisId = diagnosisIds.FirstOrDefault(),
                                    DoctorId = selectedDoctors.FirstOrDefault()
                                };

                                if (_context.Medications.Any(m => m.Id == prescription.MedicationId) &&
                                    _context.Doctors.Any(d => d.Id == prescription.DoctorId))
                                {
                                    _context.Prescriptions.Add(prescription);
                                }
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
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

            PopulateRelatedData();
            return View(patient);
        }

        // Helper method to check if a patient exists
        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient != null)
            {
                // Remove related diagnoses first
                var relatedDiagnoses = _context.Diagnoses.Where(d => d.PatientId == id).ToList();
                _context.Diagnoses.RemoveRange(relatedDiagnoses);

                // Remove related appointments
                var relatedAppointments = _context.Appointments.Where(a => a.PatientId == id).ToList();
                _context.Appointments.RemoveRange(relatedAppointments);

                // Remove related prescriptions
                var relatedPrescriptions = _context.Prescriptions.Where(p => p.PatientId == id).ToList();
                _context.Prescriptions.RemoveRange(relatedPrescriptions);

                // Remove the patient
                _context.Patients.Remove(patient);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateRelatedData()
        {
            ViewData["Doctors"] = new SelectList(_context.Doctors, "Id", "Name");
            ViewData["Medications"] = new SelectList(_context.Medications, "Id", "Name");
            ViewData["Diagnoses"] = new SelectList(_context.Diagnoses, "Id", "Condition");
            // Add more related data as needed
        }
    }
}
