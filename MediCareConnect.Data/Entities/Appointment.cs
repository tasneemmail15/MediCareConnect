using System;
using System.ComponentModel.DataAnnotations;

namespace MediCareConnect.Data.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient?  Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public string? Notes { get; set; }
    }
}