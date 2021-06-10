namespace OnlineShopServerCore.Models.JsonModels
{
    public class JSONImage
    {
        public JSONImage()
        {
        }
        public JSONImage(ItemImage img)
        {
            id = img.Id;
            path = $"{Startup.baseUrl}/api/items/ItemImage/{img.Path}";
            isMain = img.IsMain;
        }
        public long id { get; set; }
        public string path { get; set; }
        public bool? isMain { get; set; }

        public static explicit operator JSONImage(ItemImage img) => new JSONImage(img);
    }
}