using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace RepositoryLayer.Repository.Models
{
    public partial class Persons
    {
        public Persons()
        {
            Notes = new HashSet<UserNotes>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string LastName { get; set; }
        [StringLength(255)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(255)]
        public string Email { get; set; }
        [StringLength(255)]
        public string Password { get; set; }

        [InverseProperty("Person")]
        public virtual ICollection<UserNotes> Notes { get; set; }
    }
}
