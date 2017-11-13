using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YellowTail.Models
{
   
    public class Dashboard
    {
        public List<Activity> Activities {get; set;}
  
        public User User {get; set;}
        public User coordinator {get; set;}
    }
    public class ShowActivity
    {
        public Activity Activity {get; set;}
        public User User {get; set;}
    }
   
    public class ViewActivity
    {
        
        [Required]
        public string Title {get; set;}
        [Required]
        public string Description {get; set;}
        [Required]
        public string Duration {get; set;}
        //datetype required with future date method validation added
        [Required]
        [FutureDate]
        [DataType(DataType.DateTime)]
        public DateTime DateTime {get; set;}
            
    }
    //future date validator overrides value depending on context
    //obj value passed through with validation context
    //datetime date variable is assigned the datime value being passed through
    //return successful validation or a negative result if date is less than the current date
    //this prevents dates from being set in the past
    public class FutureDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime date = (DateTime)value;
            return date < DateTime.Now ? new ValidationResult("Date must be in the future.") : ValidationResult.Success;
        }
    }
}