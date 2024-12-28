using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.SetDTO;
using MaReSy2_Api.Services;
using MaReSy2_Api.Services.ImageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MaReSy2_Api.Controllers
{
    [Route("api/sets")]
    [ApiController]
    [Authorize]
    public class SetController : ControllerBase
    {
        private readonly ISetService _setService;
        private readonly IImageUploadService _imageUploadService;
        private readonly MaReSyDbContext _maReSyDbContext;

        public SetController(ISetService setService, IImageUploadService imageUploadService, MaReSyDbContext maReSyDbContext)
        {
            _setService = setService;
            _imageUploadService = imageUploadService;
            _maReSyDbContext = maReSyDbContext;
        }


        // GET: api/sets
        [HttpGet("")]
        public async Task<ActionResult<APIResponse<IEnumerable<SetDTO>>>> Get()
        {
            var result = await _setService.GetSetsAsync();

            return helperMethod.ToActionResult(result, this);
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse<SetDTO>>> GetSetById(int id)
        {
            var result = await _setService.GetSetByIdAsync(id);

            return helperMethod.ToActionResult(result, this);
        }

        // POST api/<ProductController>
        [HttpPost("")]
        public async Task<ActionResult<APIResponse<string>>> CreateSet(CreateSetDTO set)
        {
            //Validator.ValidateObject(set, new ValidationContext(set), validateAllProperties: true);

            //if (ModelState.IsValid)
            //{
            var result = await _setService.AddNewSetAsync(set);

            return helperMethod.ToActionResultBasic(result, this);
            //}


        }




        [HttpPut("{id}")]
        public async Task<ActionResult<APIResponse<string>>> updateSet(int id, UpdateSetDTO set)
        {
            var result = await _setService.UpdateSetAsync(set, id);
            return helperMethod.ToActionResultBasic(result, this);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<APIResponse<string>>> deactivateProduct(int id)
        {
            var result = await _setService.deleteSetAsync(id);

            return helperMethod.ToActionResultBasic(result, this);
        }

        [HttpPost("{id}/upload-image")]
        public async Task<ActionResult<APIResponse<string>>> UploadSetImage(int id, IFormFile image)
        {
            var result = new APIResponse<string>();

            try
            {
                var imageBytes = await _imageUploadService.ValidateAndProcessImageAsync(image);
                var set = await _maReSyDbContext.Sets.FindAsync(id);

                if (set == null)
                {
                    result.Errors.Add(new ErrorDetail
                    {
                        Field = "SetId",
                        Error = "Das Set wurde nicht gefunden."
                    });
                    result.StatusCode = 404;
                    return helperMethod.ToActionResult(result, this);
                }

                set.Setimage = imageBytes;
                _maReSyDbContext.Sets.Update(set);
                await _maReSyDbContext.SaveChangesAsync();


                result.Data = "Bild erfolgreich hochgeladen!";
                result.StatusCode = 200;
                return helperMethod.ToActionResultBasic(result, this);

            }
            catch (ArgumentException ex)
            {
                result.Errors.Add(new ErrorDetail
                {
                    Field = "Image",
                    Error = ex.Message
                });
                result.StatusCode = 400;
                return helperMethod.ToActionResult(result, this);
            }
            catch
            {
                result.Errors.Add(new ErrorDetail
                {
                    Field = "General",
                    Error = "Ein unerwarteter Fehler ist aufgetreten."
                });
                result.StatusCode = 500;
                return helperMethod.ToActionResult(result, this);
            }
        }

        [HttpGet("{id}/image")]
        public async Task<ActionResult<APIResponse<byte[]>>> GetSetImage(int id)
        {
            var result = new APIResponse<byte[]>();


            var set = await _maReSyDbContext.Sets.FindAsync(id);

            if (set == null)
            {
                result.Errors.Add(new ErrorDetail
                {
                    Field = "SetId",
                    Error = "Set wurde nicht gefunden!"
                });
                result.StatusCode = 404;
                return helperMethod.ToActionResult(result, this);
            }

            if (set.Setimage == null || set.Setimage.Length == 0)
            {
                result.Errors.Add(new ErrorDetail
                {
                    Field = "SetImage",
                    Error = $"Kein Setbild für Set (ID: {set.SetId}) vorhanden."
                });
                result.StatusCode = 400;
                return helperMethod.ToActionResult(result, this);

            }

            var imageType = "image/jpeg";

            return File(set.Setimage, imageType);
        }



    }
}

