using System;
using System.Collections.Generic;

namespace Test.ModelsProject
{
    public class Product
    {
        public DateTime AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
        public string Name { get; set; }
        public IEnumerable<Dictionary<ProductDetails, Dictionary<int, Tuple<int, string, string, DateTime, DateTime?>>>> ProductDetails { get; set; }
        public int ProductId { get; set; }
        public int ReorderAtItemsInStock { get; set; }
        public decimal RRP { get; set; }

        public decimal CostPrice { get; set; }
    }
}