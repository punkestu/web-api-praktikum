using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web_api_praktikum.Model;

namespace web_api_praktikum.Controllers;
[ApiController]
[Route("[controller]")]

public class ProductsController : Controller
{
      private readonly ILogger<ProductsController> _logger;
      private readonly ProductDTO _productDTO;

      public ProductsController(ILogger<ProductsController> logger,
                             ProductDTO productDTO)
      {
            _logger = logger;
            _productDTO = productDTO;
      }
      [HttpGet("/products")]
      public IActionResult Get([FromQuery] string? name, int? priceMin, int? priceMax, int? stockMin, int? stockMax)
      {
            var filter = new ProductGetFilter
            {
                  Name = name,
                  PriceMin = priceMin,
                  PriceMax = priceMax,
                  StockMin = stockMin,
                  StockMax = stockMax,
            };
            var products = _productDTO.Get(filter);
            return Ok(products == null ? new ProductM[] { } : products);
      }
      [HttpGet("/products/{id}")]
      public IActionResult GetOne(long id)
      {
            var product = _productDTO.GetOne(id);
            if (product == null)
            {
                  return NotFound();
            }
            return Ok(product);
      }
      [HttpPost("/products"), Authorize]
      public IActionResult Create([FromBody] ProductRequest productRequest)
      {
            if (!_productDTO.VerifyNameUnique(productRequest.name, ModelState))
            {
                  return new InvalidInputResult(ModelState);
            }
            var product = new ProductM(productRequest.name, productRequest.price, productRequest.stock);
            if (!_productDTO.Save(product))
            {
                  return StatusCode(500, "Failed to create product");
            }
            return Ok("product created with id " + product.Id.ToString());
      }
      [HttpPut("/products/{id}"), Authorize]
      public IActionResult Update(long id, [FromBody] ProductRequest productRequest)
      {
            var product = _productDTO.GetOne(id);
            if (product == null)
            {
                  return NotFound();
            }
            if (product.Name != productRequest.name)
            {
                  if (!_productDTO.VerifyNameUnique(productRequest.name, ModelState))
                  {
                        return new InvalidInputResult(ModelState);
                  }
            }
            product.Name = productRequest.name;
            product.Price = productRequest.price;
            product.Stock = productRequest.stock;
            if (!_productDTO.Save(product))
            {
                  return StatusCode(500, "Failed to update product");
            }
            return Ok("product updated");
      }
      [HttpDelete("/products/{id}")]
      public IActionResult Delete(long id)
      {
            if(!_productDTO.Delete(id)){
                  return NotFound();
            }
            return Ok("product deleted");
      }
}