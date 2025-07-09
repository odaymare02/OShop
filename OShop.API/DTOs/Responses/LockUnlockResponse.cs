namespace OShop.API.DTOs.Responses
{
    public class LockUnlockResponse
    {
        public bool Success { get; set; }
       public string Message { get; set; }
        public bool? IsLocked { get; set; }
    }
}
