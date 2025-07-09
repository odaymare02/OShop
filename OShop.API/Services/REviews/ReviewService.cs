using OShop.API.Data;
using OShop.API.Models;
using OShop.API.Services.IService;

namespace OShop.API.Services.REviews
{
    public class ReviewService:Service<Review>,IReviewService
    {
        private readonly ApplicationDbContext contex;

        public ReviewService(ApplicationDbContext contex):base(contex)
        {
            this.contex = contex;
        }
    }
}
