public class SearchResult
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Section { get; set; }
    public int? Year { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public string InventoryNumber { get; set; } // 🔹 Added Inventory Number
    public string Isbn { get; set; } // 🔹 Added ISBN
    public string PublishingHouse { get; set; }
}
