using System.Data;
using System.Data.SQLite;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace web_api_praktikum.Model.DTO.Sqlite;

public class ProductDTOSQLite : ProductDTO
{
      private readonly DB db;
      public ProductDTOSQLite(DB db)
      {
            this.db = db;
      }
      public bool Delete(long id)
      {
            if (GetOne(id) == null)
            {
                  return false;
            }
            SQLiteCommand command = (SQLiteCommand)db.CreateCommand();
            command.CommandText = "DELETE FROM products WHERE id = @id;";
            command.Parameters.AddWithValue("@id", id);
            db.Execute(command);
            return true;
      }

      public ProductM? FromDataRow(DataRow dr)
      {
            long Id = Convert.ToInt64(dr["id"]);
            string? Name = dr["name"].ToString();
            if (Name == null)
            {
                  return null;
            }
            int Price = Convert.ToInt32(dr["price"]);
            int Stock = Convert.ToInt32(dr["stock"]);
            return new ProductM(Name, Price, Stock) { Id = Id };
      }

      public List<ProductM>? Get(ProductGetFilter? filter = null)
      {
            SQLiteCommand command = (SQLiteCommand)db.CreateCommand();
            command.CommandText = "SELECT * FROM products";
            List<string> where = new();
            if (filter?.Name != null)
            {
                  where.Add("name = @name");
                  command.Parameters.AddWithValue("@name", filter?.Name);
            }
            if (filter?.PriceMin != null)
            {
                  where.Add("price >= @pricemin");
                  command.Parameters.AddWithValue("@pricemin", filter?.PriceMin);
            }
            if (filter?.PriceMax != null)
            {
                  where.Add("price <= @pricemax");
                  command.Parameters.AddWithValue("@pricemax", filter?.PriceMax);
            }
            if (filter?.StockMin != null)
            {
                  where.Add("stock >= @stockmin");
                  command.Parameters.AddWithValue("@stockmin", filter?.StockMin);
            }
            if (filter?.StockMax != null)
            {
                  where.Add("stock >= @stockmax");
                  command.Parameters.AddWithValue("@stockmax", filter?.StockMax);
            }
            if (where.Count > 0)
            {
                  command.CommandText += " WHERE " + string.Join(" AND ", where) + ";";
            }
            DataTable dt = db.GetMany(command);
            List<ProductM> products = new List<ProductM>();
            foreach (DataRow dr in dt.Rows)
            {
                  ProductM? product = FromDataRow(dr);
                  if (product == null)
                  {
                        throw new Exception("Invalid data in database");
                  }
                  products.Add(product);
            }
            return products.Count > 0 ? products : null;
      }

      public ProductM? GetOne(long id)
      {
            SQLiteCommand command = (SQLiteCommand)db.CreateCommand();
            command.CommandText = "SELECT * FROM products WHERE id = @id;";
            command.Parameters.AddWithValue("@id", id);
            var row = db.GetOne(command);
            return row == null ? null : FromDataRow(row);
      }

      public bool Save(ProductM product)
      {
            var productId = product.Id;
            if (productId != null)
            {
                  if (GetOne(productId != null ? (long)productId : -1) == null)
                  {
                        return false;
                  }
                  SQLiteCommand command = (SQLiteCommand)db.CreateCommand();
                  command.CommandText = "UPDATE products SET name=@name, price=@price, stock=@stock WHERE id=@id;";
                  command.Parameters.AddWithValue("@name", product.Name);
                  command.Parameters.AddWithValue("@price", product.Price);
                  command.Parameters.AddWithValue("@stock", product.Stock);
                  command.Parameters.AddWithValue("@id", product.Id);
                  db.Execute(command);
                  return true;
            }
            SQLiteCommand commandCreate = (SQLiteCommand)db.CreateCommand();
            commandCreate.CommandText = "INSERT INTO products (name, price, stock) VALUES (@name, @price, @stock); select last_insert_rowid();";
            commandCreate.Parameters.AddWithValue("@name", product.Name);
            commandCreate.Parameters.AddWithValue("@price", product.Price);
            commandCreate.Parameters.AddWithValue("@stock", product.Stock);
            product.Id = db.ExecuteAndId(commandCreate);
            if (product.Id == null)
            {
                  return false;
            }
            return true;
      }

      public bool VerifyNameUnique(string name, ModelStateDictionary modelState)
      {
            var products = Get(new ProductGetFilter { Name = name });
            if (products != null)
            {
                  modelState.AddModelError("name", "Name already exists");
                  return false;
            }
            return true;
      }
}
