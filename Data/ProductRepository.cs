using uploadphoto.Models;

namespace uploadphoto.Data
{
    public static class ProductRepository
    {
        private static readonly List<Product> _products = new List<Product>();
        private static int _nextId = 1;

        static ProductRepository()
        {
            // Seed some initial products
            _products.Add(new Product
            {
                Id = _nextId++,
                Name = "iPhone 15 Pro Max",
                Price = 29990000,
                ImageUrls = new List<string>()
            });
            _products.Add(new Product
            {
                Id = _nextId++,
                Name = "MacBook Air M3",
                Price = 32990000,
                ImageUrls = new List<string>()
            });
        }

        public static List<Product> GetAll()
        {
            return _products;
        }

        public static Product? GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public static void Add(Product product)
        {
            product.Id = _nextId++;
            _products.Add(product);
        }

        public static void Update(Product product)
        {
            var existing = GetById(product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Price = product.Price;
                existing.ImageUrls = product.ImageUrls;
            }
        }

        public static void Delete(int id)
        {
            var product = GetById(id);
            if (product != null)
            {
                _products.Remove(product);
            }
        }
    }
}
