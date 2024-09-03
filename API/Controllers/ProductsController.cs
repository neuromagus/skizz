using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProductsController(IUnitOfWork unit) : BaseApiController
{
    [Cache(600)]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(
        [FromQuery] ProductSpecParams specParams)
    {
        var spec = new ProductSpecification(specParams);

        return await CreatePageResult(unit.Repository<Product>(), spec, specParams.PageIndex, specParams.PageSize);
    }

    [Cache(600)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await unit.Repository<Product>().GetByIdAsync(id);

        return product is null ? NotFound() : product;
    }

    [InvalidateCache("api/products|")]
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        unit.Repository<Product>().Add(product);

        return await unit.Complete() 
            ? CreatedAtAction("GetProduct", new {id = product.Id}, product)
            : BadRequest("Problem creating product");
    }

    [InvalidateCache("api/products|")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExist(id)) return BadRequest("Cannot update this product");

        unit.Repository<Product>().Update(product);

        return await unit.Complete()
            ? NoContent()
            : BadRequest("Problem updating product");
    }

    [InvalidateCache("api/products|")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await unit.Repository<Product>().GetByIdAsync(id);

        if (product is null) return NotFound();
        
        unit.Repository<Product>().Remove(product);

        return await unit.Complete()
            ? NoContent()
            : BadRequest("Problem deleting product");
    }

    [Cache(10800)]
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var spec = new BrandListSpecification();
        return Ok(await unit.Repository<Product>().ListAsync(spec));
    }

    [Cache(10800)]
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new TypeListSpecification();
        return Ok(await unit.Repository<Product>().ListAsync(spec));
    }

    private bool ProductExist(int id) => unit.Repository<Product>().Exists(id);
}
