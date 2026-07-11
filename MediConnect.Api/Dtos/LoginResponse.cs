namespace MediConnect.Api.Dtos
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int UserID { get; set; }
        public int PatientID { get; set; }
    }
}
