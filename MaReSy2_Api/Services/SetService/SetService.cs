using MaReSy2_Api.Extensions;
using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.SetDTO;
using MaReSy2_Api.Services.ProductService;
using Microsoft.AspNetCore.Identity;

namespace MaReSy2_Api.Services.SetService
{
    public class SetService : ISetService
    {
        private readonly MaReSyDbContext _context;
        private readonly IProductService _productService;

        public SetService(MaReSyDbContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        public async Task<APIResponse<string>> AddNewSetAsync(CreateSetDTO set)
        {
            var result = new APIResponse<string>();

            if (string.IsNullOrWhiteSpace(set.Setname))
            {
                result.Errors.Add(new ErrorDetail { Field = "Setname", Error = "Der Setname ist erforderlich!" });
            }

            if (!set.setProductAssignDTOs.IsNullOrEmpty())
            {
                foreach (var setitem in set.setProductAssignDTOs!)
                {
                    if (!await _productService.ProductExistsAsync(setitem.productId))
                    {
                        result.Errors.Add(new ErrorDetail { Field = "ProductId", Error = $"Das Produkt mit der ID {setitem.productId} gibt es nicht!" });
                    }

                    if (!int.IsPositive(setitem.productAmount))
                    {
                        result.Errors.Add(new ErrorDetail { Field = "ProductAmount", Error = $"Das Produkt mit der ID {setitem.productId} muss eine positive Anzahl haben!" });
                    }
                }
            }

            if (result.Errors.Any())
            {
                result.StatusCode = 400;
                return result;
            }

            Set createdSet = new Set
            {
                Setname = set.Setname,
                Setdescription = set.Setdescription,
                Setactive = set.Setactive,

            };

            await _context.Sets.AddAsync(createdSet);
            await _context.SaveChangesAsync();


            var productsSet = set.setProductAssignDTOs?.Select(x => new ProductsSet
            {
                ProductId = x.productId,
                SetId = createdSet.SetId,
                SingleProductAmount = x.productAmount
            }).ToList();

            if (productsSet?.Count > 0)
            {
                await _context.ProductsSets.AddRangeAsync(productsSet);
                await _context.SaveChangesAsync();
            }

            result.Data = "Set erfolgreich erstellt!";
            result.StatusCode = 200;
            return result;
        }

        public async Task<APIResponse<string>> deleteSetAsync(int setId)
        {
            var result = new APIResponse<string>();


            var set = await _context.Sets.FindAsync(setId);

            if (set == null)
            {
                result.Errors.Add(new ErrorDetail { Field = "SetId", Error = "Set wurde nicht gefunden!" });
                result.StatusCode = 404;
                return result;
            }

            set.Setactive = false;

            _context.Sets.Update(set);
            await _context.SaveChangesAsync();

            result.Data = "Set erfolgreich deaktiviert!";
            result.StatusCode = 200;
            return result;
        }

        public async Task<APIResponse<SetDTO>> GetSetByIdAsync(int setId)
        {
            var result = new APIResponse<SetDTO>();

            var response = await SetExistsAsync(setId);

            if (response == false)
            {
                result.Errors.Add(new ErrorDetail { Field = "SetId", Error = "Das Set wurde nicht gefunden!" });
                result.StatusCode = 404;
                return result;
            }

            var setfromDb = await _context.Sets
                .Include(s => s.ProductsSets)
                .ThenInclude(ps => ps.Product)
                .FirstOrDefaultAsync(x => x.SetId == setId);

            SetDTO? set = null;


            var set_fromDb = await _context.Sets.Where(x => x.SetId == setId).FirstOrDefaultAsync();

            if (set_fromDb != null)
            {
                result.Data = new SetDTO
                {
                    SetId = set_fromDb.SetId,
                    Setname = set_fromDb.Setname,
                    Setdescription = set_fromDb.Setdescription,
                    Setactive = set_fromDb.Setactive,
                    SetimageLink = set_fromDb.Setimage != null && set_fromDb.Setimage.Length != 0 ? $"/api/sets/{set_fromDb.SetId}/image" : null,
                    Products = setfromDb!.ProductsSets.Select(ps => new AssignedProductDTO
                    {
                        ProductId = ps.ProductId,
                        Productname = ps.Product.Productname,
                        Productdescription = ps.Product.Productdescription ?? null,
                        Productamount = ps.SingleProductAmount,

                    }).ToList(),
                };
                result.StatusCode = 200;
            }



            return result;
        }

        public async Task<APIResponse<IEnumerable<SetDTO>>> GetSetsAsync()
        {
            var result = new APIResponse<IEnumerable<SetDTO>>();

            var sets = await _context.Sets.ToListAsync();

            result.Data = sets.Select(set => new SetDTO
            {
                SetId = set.SetId,
                Setname = set.Setname,
                Setdescription = set.Setdescription ?? null,
                Setactive = set.Setactive,
                SetimageLink = set.Setimage != null && set.Setimage.Length != 0 ? $"/api/sets/{set.SetId}/image" : null,
            });

            result.StatusCode = 200;
            return result;
        }

        public async Task<bool> SetExistsAsync(int setId)
        {
            var set = await _context.Sets.FindAsync(setId);

            return set != null;
        }

        public async Task<APIResponse<string>> UpdateSetAsync(UpdateSetDTO set, int setId)
        {
            var result = new APIResponse<string>();

            Set? setfromDb = await _context.Sets.Include(s => s.ProductsSets)
                .FirstOrDefaultAsync(set => set.SetId == setId);

            if (setfromDb == null)
            {
                result.Errors.Add(new ErrorDetail { Field = "SetId", Error = "Das Set wurde nicht gefunden!" });
                result.StatusCode = 404;
                return result;
            }


            if (!set.Setname.IsNullOrEmpty() && string.IsNullOrWhiteSpace(set.Setname))
            {
                result.Errors.Add(new ErrorDetail { Field = "Setname", Error = "Der Setname ist erforderlich!" });
            }

            if (!set.Setdescription.IsNullOrEmpty() && string.IsNullOrWhiteSpace(set.Setdescription))
            {
                result.Errors.Add(new ErrorDetail { Field = "Setdescription", Error = $"Die Setbeschreibung ist erforderlich!" });

            }


            if (!set.setProductAssignDTOs.IsNullOrEmpty())
            {
                foreach (var setitem in set.setProductAssignDTOs!)
                {
                    if (!await _productService.ProductExistsAsync(setitem.productId))
                    {
                        result.Errors.Add(new ErrorDetail { Field = "ProductId", Error = $"Das Produkt mit der ID {setitem.productId} gibt es nicht!" });
                    }

                    if (!int.IsPositive(setitem.productAmount))
                    {
                        result.Errors.Add(new ErrorDetail { Field = "ProductAmount", Error = $"Das Produkt mit der ID {setitem.productId} muss eine positive Anzahl haben!" });
                    }

                    if (setitem.productAmount == 0)
                    {
                        result.Errors.Add(new ErrorDetail { Field = "ProductAmount", Error = $"Das Produkt mit der ID {setitem.productId} darf nicht 0 sein!" });
                    }
                }
            }

            if (result.Errors.Any())
            {
                result.StatusCode = 400;
                return result;
            }



            setfromDb!.Setname = set.Setname ?? setfromDb!.Setname;
            setfromDb!.Setdescription = set.Setdescription ?? setfromDb!.Setdescription;
            setfromDb!.Setactive = set.Setactive ?? setfromDb!.Setactive;

            if (set.setProductAssignDTOs?.Count > 0)
            {
                foreach (var dto in set.setProductAssignDTOs!)
                {
                    var existingAssignment = setfromDb.ProductsSets.FirstOrDefault(
                        ps => ps.ProductId == dto.productId
                        );


                    if (existingAssignment != null)
                    {
                        existingAssignment.SingleProductAmount = dto.productAmount;
                    }
                    else
                    {
                        setfromDb.ProductsSets.Add(new ProductsSet()
                        {
                            ProductId = dto.productId,
                            SetId = setfromDb.SetId,
                            SingleProductAmount = dto.productAmount,

                        });
                    }
                }
            }


            _context.Sets.Update(setfromDb);

            await _context.SaveChangesAsync();

            result.Data = "Set erfolgreich aktualisiert!";
            result.StatusCode = 200;
            return result;
        }


    }
}
