namespace OnlineShopServerCore.Models.JsonModels
{
    public class JSONStatus
    {
        public long id { get; set; }
        public string name { get; set; }

        public JSONStatus(OrderStatus os)
        {
            id = os.Id;
            name = os.Name;
        }
    }
}