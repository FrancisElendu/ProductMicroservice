namespace Product.Domain.Entities
{
    public class Product : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public string Category { get; private set; }
        public string Sku { get; private set; }

        // Private constructor for EF Core
        private Product() { }

        public Product(string name, string description, decimal price,
                      int stockQuantity, string category, string sku)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Price = price > 0 ? price : throw new ArgumentException("Price must be greater than zero");
            StockQuantity = stockQuantity >= 0 ? stockQuantity : throw new ArgumentException("Stock quantity cannot be negative");
            Category = category;
            Sku = sku ?? throw new ArgumentNullException(nameof(sku));
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string description, decimal price,
                          string category, string sku)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Price = price > 0 ? price : throw new ArgumentException("Price must be greater than zero");
            Category = category;
            Sku = sku ?? throw new ArgumentNullException(nameof(sku));
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStock(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative");

            StockQuantity = quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsInStock() => StockQuantity > 0;
    }
}
