using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Heartbeat
{
    public class Status
    {
        public string subId = "Sub1";

        public Stopwatch sw;
     
        public Status(ref Stopwatch sw1)
        {       
			sw = sw1;
			sw.Start();
        }

     
    }
}
