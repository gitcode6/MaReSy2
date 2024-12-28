using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.SingleProductDTO;
using MaReSy2_Api.Services.SingleProductService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MaReSy2_Api.Controllers
{
    [Route("api/singleproducts")]
    [ApiController]
    [Authorize]
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
        public async Task<ActionResult<APIResponse<IEnumerable<SingleProductDTO>>>> Get()
        {
            var result = await _singleProductService.GetSingleProductsAsync();

            return helperMethod.ToActionResult(result, this);
        }

        // POST api/<ProductController>
        [HttpPost("")]
        public async Task<ActionResult<APIResponse<string>>> CreateSingleProduct(CreateSingleProductDTO singleProduct)
        {
            //Validator.ValidateObject(singleProduct, new ValidationContext(singleProduct), validateAllProperties: true);

            //if (ModelState.IsValid)
            //{
                var result = await _singleProductService.AddNewSingleProduct(singleProduct);

            return helperMethod.ToActionResultBasic(result, this);
            //}

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<APIResponse<string>>> deactivateProduct(int id)
        {
            var result = await _singleProductService.deleteSingleProductAsync(id);

            return helperMethod.ToActionResultBasic(result, this);
        }

        //GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse<SingleProductDTO>>> GetProductById(int id)
        {
            var result = await _singleProductService.GetSingleProductAsync(id);

            return helperMethod.ToActionResult(result, this);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<APIResponse<string>>> updateProduct(int id, UpdateSingleProductDTO product)
        {
            var result = await _singleProductService.updateSingleProduct(id, product);

           return helperMethod.ToActionResultBasic(result, this);
        }
    }
}
