using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.RentalDTO;
using MaReSy2_Api.Models.DTO.SetDTO;
using MaReSy2_Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MaReSy2_Api.Controllers
{
    [Route("api/rentals")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly IRentalService _rentalService;
        private readonly MaReSyDbContext _maReSyDbContext;

        public RentalController(IRentalService rentalService, MaReSyDbContext maReSyDbContext)
        {
            _rentalService = rentalService;
            _maReSyDbContext = maReSyDbContext;
        }


        // GET: api/<RentalController>
        [HttpGet("")]
        public async Task<ActionResult<List<RentalDTO>>> Get()
        {
            var rentals = await _rentalService.GetAllRentalsAsync();

            return Ok(rentals);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<RentalDTO>>> Get(int userId)
        {
            var rentals = await _rentalService.GetAllUserRentalsAsync(userId);

            return Ok(rentals);
        }

        //// GET api/<ProductController>/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<SetDTO>> GetSetById(int id)
        //{
        //    var set = await _rentalService.GetSetByIdAsync(id);


        //    if (set == null)
        //    {
        //        var errors = IdentityResult.Failed(new IdentityError() { Description = "Set wurde nicht gefunden!" });
        //        return BadRequest(errors);
        //    }
        //    else
        //    {
        //        return Ok(set);
        //    }
        //}

        // POST api/<ProductController>
        [HttpPost("")]
        public async Task<ActionResult<ProductDTO>> CreateSet(CreateRentalDTO rental)
        {
            Validator.ValidateObject(rental, new ValidationContext(rental), validateAllProperties: true);

            if (ModelState.IsValid)
            {
                var result = await _rentalService.AddNewRentalAsync(rental);

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

            //}




            //[HttpPut("{id}")]
            //public async Task<IActionResult> updateSet(int id, UpdateSetDTO set)
            //{
            //    var result = await _rentalService.UpdateSetAsync(set, id);

            //    if (result.Contains(IdentityResult.Success))
            //    {
            //        return Ok(result);
            //    }

            //    else
            //    {
            //        return BadRequest(result);
            //    }
            //}

            //[HttpDelete("{id}")]
            //public async Task<IActionResult> deactivateProduct(int id)
            //{
            //    var result = await _rentalService.deleteSetAsync(id);

            //    if (result == IdentityResult.Success)
            //    {
            //        return Ok(result);
            //    }

            //    else
            //    {
            //        return BadRequest(result);
            //    }
            //}




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

            //[HttpDelete("{id}")]
            //public async Task<IActionResult> deactivateProduct(int id)
            //{
            //    var result = await _productService.deleteProductAsync(id);

            //    if (result == IdentityResult.Success)
            //    {
            //        return Ok(result);
            //    }

            //    else
            //    {
            //        return BadRequest(result);
            //    }
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

            ////[HttpPost("{id}/image")]
            ////public async Task<IActionResult> UploadImage(int id)
            ////{

            ////}



        }
    }
}

