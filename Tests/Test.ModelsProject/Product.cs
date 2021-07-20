using System;

namespace Test.ModelsProject
{
    public class Product
    {
        public DateTime AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        public int ReorderAtItemsInStock { get; set; }
        public decimal RRP { get; set; }
    }
}