using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionDebugging
{
     public class DecodedVehicleSpecEnvelope
    {
        public int Count { get; set; }
        public string Message { get; set; }
        public DecodedVehicleSpec[] Results { get; set; }
        public string SearchCriteria { get; set; }
    }

    public class DecodedVehicleSpec
{
        public Spec[] Specs { get; set; }
    }

    public class Spec
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

}
