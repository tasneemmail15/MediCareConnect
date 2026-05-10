using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediCareConnect.Data.Entities
{
    public class Diagnosis
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        [Required]
        public DateTime DiagnosisDate { get; set; }

        [Required]
        public string? Condition { get; set; }

        public string? Notes { get; set; }

        // Relationship with Prescription
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
