namespace OShop.API.DTOs.Responses
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int Rate { get; set; }
        public string Commnet { get; set; }
        public DateTime ReviewDate { get; set; }



    }
}
