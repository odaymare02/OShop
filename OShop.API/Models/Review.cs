namespace OShop.API.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser {get;set;}
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Rate { get; set; }
        public string? Commnet { get; set; }
        public DateTime ReviewDate { get; set; }
        //can add advance thing like add image with review or reply of review
        //if i need to add image this attribuate gane multi image this mean multi value and need to seperate in annother modle call reviewimages
        public ICollection<ReviewImages> ReviewImages { get; } = new List<ReviewImages>();
    }
}
