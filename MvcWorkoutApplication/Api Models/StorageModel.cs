using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWorkoutApplication.Api_Models
{
    public class StorageModel
    {
        public string workoutName { get; set; }

        public string userName { get; set; }
        public bool inStorage { get; set; }
    }
}