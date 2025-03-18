using Microsoft.AspNetCore.Identity;

namespace Server.Data
{
    public class ApiUser :  IdentityUser
    {
        public DateTime DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string QRCode { get; set; }
        public int Points { get; set; }
        public string ReferrerId { get; set; }
        public virtual ICollection<UserDevice> UserDevices { get; set; }
        public virtual ICollection<UserCourse> UserCourses { get; set; }


    }
    }

