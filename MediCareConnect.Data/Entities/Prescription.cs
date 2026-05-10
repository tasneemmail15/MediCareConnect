using System;
using System.ComponentModel.DataAnnotations;

namespace MediCareConnect.Data.Entities
{
    public class Prescription
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        [Required]
        public int MedicationId { get; set; }
        public Medication? Medication { get; set; }

        [Required]
        public int DiagnosisId { get; set; }
        public Diagnosis? Diagnosis { get; set; }

        [Required]
        public DateTime PrescriptionDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Instructions { get; set; }
    }
}