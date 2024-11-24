using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.SingleProductDTO;
using MaReSy2_Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MaReSy2_Api.Controllers
{
    [Route("api/singleproducts")]
    [ApiController]
    public class SingleProductController : ControllerBase
    {
        private readonly ISingleProductService _singleProductService;
        private readonly MaReSyDbContext _maReSyDbContext;

        public SingleProductController(MaReSyDbContext maReSyDbContext, ISingleProductService singleProductService)
        {
            _maReSyDbContext = maReSyDbContext;
            _singleProductService = singleProductService;

        }

        // GET: api/<ProductController>
        [HttpGet("")]
        public async Task<ActionResult<List<SingleProductDTO>>> Get()
        {
            var products = await _singleProductService.GetSingleProductsAsync();

            return Ok(products);
        }

        // POST api/<ProductController>
        [HttpPost("")]
        public async Task<ActionResult<ProductDTO>> CreateSingleProduct(CreateSingleProductDTO singleProduct)
        {
            Validator.ValidateObject(singleProduct, new ValidationContext(singleProduct), validateAllProperties: true);

            if (ModelState.IsValid)
            {
                var result = await _singleProductService.AddNewSingleProduct(singleProduct);

                if (result.Contains(IdentityResult.Success))
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }

            return BadRequest();




            //var (createdProduct, errors) = result;

            //if (errors != null && errors.Any())
            //{
            //    return BadRequest(new { Errors = errors });
            //}

            //return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProduct);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deactivateProduct(int id)
        {
            var result = await _singleProductService.deleteSingleProductAsync(id);

            if (result == IdentityResult.Success)
            {
                return Ok(result);
            }

            else
            {
                return BadRequest(result);
            }
        }

        //GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            var product = await _singleProductService.GetSingleProductAsync(id);

            if (product == null)
            {
                var errors = IdentityResult.Failed(new IdentityError() { Description = "Singleproduct wurde nicht gefunden!" });
                return BadRequest(errors);
            }
            else
            {
                return Ok(product);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateProduct(int id, UpdateSingleProductDTO product)
        {
            var result = await _singleProductService.updateSingleProduct(id, product);

            if (result.Contains(IdentityResult.Success))
            {
                return Ok(result);
            }

            else
            {
                return BadRequest(result);
            }
        }

        //// POST api/<ProductController>
        //[HttpPost("")]
        //public async Task<ActionResult<ProductDTO>> CreateProduct(CreateProductDTO product)
        //{
        //    Validator.ValidateObject(product, new ValidationContext(product), validateAllProperties: true);

        //    if (ModelState.IsValid)
        //    {
        //        var result = await _productService.AddNewProduct(product);

        //        if (result.Contains(IdentityResult.Success))
        //        {
        //            return Ok(result);
        //        }
        //        else
        //        {
        //            return BadRequest(result);
        //        }
        //    }

        //    return BadRequest();




        //    //var (createdProduct, errors) = result;

        //    //if (errors != null && errors.Any())
        //    //{
        //    //    return BadRequest(new { Errors = errors });
        //    //}

        //    //return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProduct);

        //}

        //[HttpGet("{id}/image")]
        //public async Task<IActionResult> GetProductImage(int id)
        //{
        //    var product = await _maReSyDbContext.Products.FindAsync(id);

        //    if (product == null)
        //    {
        //        return BadRequest(IdentityResult.Failed(new IdentityError() { Description = "Produkt wurde nicht gefunden!" })); 

        //            //NotFound("Produkt nicht gefunden");
        //    }

        //    if (product.Productimage == null || product.Productimage.Length == 0)
        //    {
        //        return BadRequest(IdentityResult.Failed(new IdentityError() { Description = $"Kein Produktbild für Produkt (ID: {product.ProductId}) vorhanden." }));

        //    }

        //    var imageType = "image/jpeg";

        //    return File(product.Productimage, imageType);
        //}



        //[HttpPut("{id}")]
        //public async Task<IActionResult> updateProduct(int id, UpdateProductDTO product)
        //{
        //    var result = await _productService.updateProduct(id, product);

        //    if(result.Contains(IdentityResult.Success))
        //    {
        //        return Ok(result);
        //    }

        //    else
        //    {
        //        return BadRequest(result);
        //    }
        //}

        //[HttpPost("{id}/image")]
        //public async Task<IActionResult> UploadImage(int id)
        //{

        //}



    }
}
