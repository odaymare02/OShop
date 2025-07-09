namespace OShop.API.Models
{
    public class ReviewImages
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public int ReviewId { get; set; }
        public Review? Review { get; set; }
    }

}
