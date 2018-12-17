
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class WeddingSchedule
    {
        [Key]
        public int WeddingId {get;set;}

            [Required]
            [MinLength(2, ErrorMessage="Wedder One should be more than 2 characters!!")]       
        public string WedderOne {get;set;}

            [Required]
            [MinLength(2, ErrorMessage="Wedder Two should be more than 2 characters!!")]
        public string WedderTwo {get;set;}

            [Required]
        public DateTime Date {get;set;}

            [Required]
        public string WeddingAddress {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;

        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public int UserId {get;set;}
        public List<Guest> guest {get;set;}

    }
}