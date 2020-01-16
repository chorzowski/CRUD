using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiNetCore.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(250, ErrorMessage = "Headline can't be more than 250 characters")]
        public string Headline { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "Review text can't be more than 500 characters")]
        public string ReviewText { get; set; }

        [Required]
        [Range(1,5, ErrorMessage = "Rating must be between 1 to 5 stars")]
        public int Rating { get; set; }
        public virtual Reviewer Reviewer { get; set; }
        public virtual Book Book { get; set; }
    }
}
