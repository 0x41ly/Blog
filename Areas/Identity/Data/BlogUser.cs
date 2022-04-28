﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Blog.Areas.Identity.Data

{
    public class BlogUser : IdentityUser
    {
        [PersonalData]
        public String? AvatarPath { get; set; }

        [PersonalData]
        public bool? IsPremuim { get; set; }

        [PersonalData]
        public string FirstName { get; set; }
        
        [PersonalData]
        public string LastName { get; set; }
        [PersonalData]
        public string Gender { get; set; }

        [PersonalData]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd,hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? LastSeen { get; set; }





    }

}
