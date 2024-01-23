using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace web_api_praktikum.Model;

public class ProductM
{
      public long? Id { get; set; }
      public string Name { get; set; }
      public int Price { get; set; }
      public int Stock { get; set; }
      public ProductM(string Name, int Price, int Stock)
      {
            this.Name = Name;
            this.Price = Price;
            this.Stock = Stock;
      }
}

public struct ProductGetFilter
{
      public string? Name;
      public int? StockMin;
      public int? StockMax;
      public int? PriceMin;
      public int? PriceMax;
}

public interface ProductDTO
{
      public List<ProductM>? Get(ProductGetFilter? filter = null);
      public ProductM? GetOne(long id);
      public ProductM? FromDataRow(DataRow dr);
      public bool Save(ProductM product);
      public bool Delete(long id);
      public bool VerifyNameUnique(string name, ModelStateDictionary modelState);
}

public class ProductRequest
{
      [Required]
      public string name { get; set; }
      [Required]
      [Range(1, int.MaxValue)]
      public int price { get; set; }
      [Required]
      [Range(0, int.MaxValue)]
      public int stock { get; set; }
      public ProductRequest(string name, int price, int stock)
      {
            this.name = name;
            this.price = price;
            this.stock = stock;
      }
}