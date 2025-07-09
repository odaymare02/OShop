namespace OShop.API.Models
{
    public enum OrderStatus
    {
        Pending,
        Cancel,
        Approved,
        Shipped,
        Completed
    }
    public enum PaymentMethodType
    {
        Visa,Cash
    }
    public class Order
    {
        //order
        public int Id { get; set; }
        public OrderStatus OrderStatus{get;set;}
        public PaymentMethodType paymentMethod { get; set;}
        public DateTime OrderDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public decimal TotalPrice { get; set; }
        //this for stripe
        public string? SessionId { get; set; }//any payment process using strip the stripe give an session id to know who is this paymnet operation
        public string? TransactionId { get; set; }//this only for success payment process
        //this for carrier
        public string? Carrier { get; set; }
        public string ? TrackingNumber { get; set; }
        //this for relation
        public ApplicationUser ApplicationUser{ get; set;}
        public string ApplicationUserId { get; set;}

    }
}
