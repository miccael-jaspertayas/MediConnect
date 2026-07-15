using System;
using System.Collections.Generic;
using System.Text;

namespace MediConnect.Mobile.Dtos
{
    public class TriageRequest
    {
        public List<string> Symptoms { get; set; } = new();
    }
}
