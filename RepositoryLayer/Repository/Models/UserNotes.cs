using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace RepositoryLayer.Repository.Models
{
    public partial class UserNotes
    {
        [Key]
        public int NoteId { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; }
        [Required]
        [StringLength(255)]
        public string Description { get; set; }
        [StringLength(255)]
        public string Color { get; set; }
        public bool? IsArchive { get; set; } = false;
        public bool? IsDeleted { get; set; } = false;
        [Column("PersonID")]
        public int PersonId { get; set; }

        [ForeignKey(nameof(PersonId))]
        [InverseProperty(nameof(Persons.Notes))]
        public virtual Persons Person { get; set; }
    }
}
