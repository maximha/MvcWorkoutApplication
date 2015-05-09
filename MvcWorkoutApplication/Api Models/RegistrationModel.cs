using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWorkoutApplication.Api_Models
{
    public class RegistrationModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
    }
}