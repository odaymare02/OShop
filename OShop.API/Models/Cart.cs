using Microsoft.EntityFrameworkCore;

namespace OShop.API.Models
{
    [PrimaryKey(nameof(ProductId),nameof(ApplicationUserId))]//this to make composite primary key rather than using fluentAPI
    public class Cart// this intermediate table between product and user
    {
        public int ProductId { get; set; }
        public Product product { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int Count { get; set; }
    }
}
