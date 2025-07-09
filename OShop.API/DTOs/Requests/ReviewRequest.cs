using OShop.API.Models;

namespace OShop.API.DTOs.Requests
{
    public class ReviewRequest
    {
        public string ?ApplicationUserId { get; set; }
        public int ProductId { get; set; }
        public int Rate { get; set; }
        public string ? Commnet { get; set; }
        public ICollection<ReviewImagesRequest>? ReviewImages { get; } = new List<ReviewImagesRequest>();
    }
}
