namespace OShop.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public ICollection<Product> products { get; } = new List<Product>();//when we use this only to assign navigation proparity and don't need to iteration on data use icollection
    }
}
