using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediCareConnect.Data.Entities
{
    public class Medication
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string Dosage { get; set; }

        public string Frequency { get; set; }

        // Navigation property
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
