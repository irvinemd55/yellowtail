using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//separate database models in dbModels.cs file for quick access

namespace YellowTail.Models
{
//user model
    public class User 
    {
        //primary key attribute assigneed to user id listed as int
        [Key]
        public int userId {get; set;}
        //string variables listed as strings this includes email and password
        public string firstname {get; set;}
        public string lastname {get; set;}
        public string email {get; set;}
        public string password {get; set;}
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        //  [InverseProperty("participant")]
        public List<Participant> Participants {get; set;}
        //  [InverseProperty("activity")]
        public List<Activity> Activities {get; set;}

    }
    public class Activity{
        [Key]
        public int activityId {get; set;}
        public int userId {get; set;}
        [ForeignKey("userId")]
        public virtual User coordinator {get; set;}
        public string title {get; set;}
        public string description {get; set;}
        public string duration {get; set;}
        public DateTime datetime {get; set;}

        public List<Participant> Participants {get; set;}
    }
    public class Participant
    {
        [Key]
        public int participantId {get; set;}
        public int userId {get; set;}
        [ForeignKey("userId")]
        public User participant {get; set;}
        public int activityId {get; set;}
        [ForeignKey("activityId")]
        public Activity activity {get; set;}
    }
}