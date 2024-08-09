using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts()
    {
         return Ok(await repository.GetProductsAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repository.GetProductByIdAsync(id);

        return product is null ? NotFound() : product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repository.AddProduct(product);

        return await repository.SaveChangesAsync() 
            ? CreatedAtAction("GetProduct", new {id = product.Id}, product)
            : BadRequest("Problem creating product");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExist(id)) return BadRequest("Cannot update this product");

        repository.UpdateProduct(product);

        return await repository.SaveChangesAsync()
            ? NoContent()
            : BadRequest("Problem updating product");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repository.GetProductByIdAsync(id);

        if (product is null) return NotFound();
        
        repository.DeleteProduct(product);

        return await repository.SaveChangesAsync()
            ? NoContent()
            : BadRequest("Problem deleting product");
    }

    private bool ProductExist(int id) => repository.ProductExists(id);
}
