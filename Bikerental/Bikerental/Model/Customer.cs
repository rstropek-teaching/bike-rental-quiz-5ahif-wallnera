using System;
using System.ComponentModel.DataAnnotations;

namespace Bikerental.Model
{
    public class Customer
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [MaxLength(50)]
        public String FirstName { get; set; }

        [Required]
        [MaxLength(75)]
        public String LastName { get; set; }

        // https://stackoverflow.com/questions/5252979/assign-format-of-datetime-with-data-annotations
        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [MaxLength(10)]
        public String HouseNumber { get; set; }

        [Required]
        [MaxLength(10)]
        public String ZipCode { get; set; }

        [Required]
        [MaxLength(75)]
        public String Town { get; set; }
    }

    public enum Gender
    {
        male, female, unkown
    }
}