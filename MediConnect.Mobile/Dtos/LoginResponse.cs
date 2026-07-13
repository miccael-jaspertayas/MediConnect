using System;
using System.Collections.Generic;
using System.Text;

namespace MediConnect.Mobile.Dtos
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int UserID { get; set; }
        public int PatientID { get; set; }
    }
}
