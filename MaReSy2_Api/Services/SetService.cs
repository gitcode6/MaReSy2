using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.SetDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MaReSy2_Api.Services
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

        public async Task<List<IdentityResult>> AddNewSetAsync(CreateSetDTO set)
        {
            List<IdentityResult> errors = new List<IdentityResult>();

            if (string.IsNullOrWhiteSpace(set.Setname))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Der Setname ist erforderlich!" }));
            }

            if (!set.setProductAssignDTOs.IsNullOrEmpty())
            {
                foreach (var setitem in set.setProductAssignDTOs!)
                {
                    if (!await _productService.ProductExistsAsync(setitem.productId))
                    {
                        errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Das Produkt mit der ID {setitem.productId} gibt es nicht!" }));
                    }

                    if (!int.IsPositive(setitem.productAmount))
                    {
                        errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Das Produkt mit der ID {setitem.productId} muss eine positive Anzahl haben!" }));
                    }
                }
            }

            if (errors.Any())
            {
                return errors;
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

            errors.Add(IdentityResult.Success);
            return errors;
        }

        public async Task<IdentityResult> deleteSetAsync(int setId)
        {
            var set = await _context.Sets.FindAsync(setId);

            if (set == null)
            {
                return IdentityResult.Failed(new IdentityError() { Description = "Set wurde nicht gefunden!" });
            }

            set.Setactive = false;

            _context.Sets.Update(set);
            await _context.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public async Task<SetDTO?> GetSetByIdAsync(int setId)
        {
            var result = await SetExistsAsync(setId);

            if (result == false)
            {
                return null;
            }

            var setfromDb = await _context.Sets
                .Include(s => s.ProductsSets)
                .ThenInclude(ps => ps.Product)
                .FirstOrDefaultAsync(x => x.SetId == setId);

            SetDTO? set = null;


            var set_fromDb = await _context.Sets.FirstAsync(x => x.SetId == setId);

            set = new SetDTO
            {
                SetId = set_fromDb.SetId,
                Setname = set_fromDb.Setname,
                Setdescription = set_fromDb.Setdescription,
                Setactive = set_fromDb.Setactive,
                ProductimageLink = null,
                Products = setfromDb!.ProductsSets.Select(ps=> new AssignedProductDTO
                {
                    ProductId = ps.ProductId,
                    Productname = ps.Product.Productname,
                    Productdescription = ps.Product.Productdescription ?? null,
                    Productamount = ps.SingleProductAmount,
                    
                }).ToList(),
            };

            return set;
        }

        public async Task<IEnumerable<SetDTO>> GetSetsAsync()
        {
            var sets = await _context.Sets.ToListAsync();

            return sets.Select(set => new SetDTO
            {
                SetId = set.SetId,
                Setname = set.Setname,
                Setdescription = set.Setdescription ?? null,
                Setactive = set.Setactive,
            });
        }

        public async Task<bool> SetExistsAsync(int setId)
        {
            var set = await _context.Sets.FindAsync(setId);

            return set != null;
        }

        public async Task<List<IdentityResult>> UpdateSetAsync(UpdateSetDTO set, int setId)
        {
            List<IdentityResult> errors = new List<IdentityResult>();

            Set? setfromDb = await _context.Sets.Include(s => s.ProductsSets)
                .FirstOrDefaultAsync(set => set.SetId == setId);

            if (set == null)
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Das Produkt mit der ID {setId} gibt es nicht!" }));
                setfromDb = await _context.Sets.FirstAsync(set => set.SetId == setId);
                return errors;
            }


            if (!set.Setname.IsNullOrEmpty() && string.IsNullOrWhiteSpace(set.Setname))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Der Setname ist erforderlich!" }));
            }

            if (!set.Setdescription.IsNullOrEmpty() && string.IsNullOrWhiteSpace(set.Setdescription))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Die Setbeschreibung ist erforderlich!" }));
            }


            if (!set.setProductAssignDTOs.IsNullOrEmpty())
            {
                foreach (var setitem in set.setProductAssignDTOs!)
                {
                    if (!await _productService.ProductExistsAsync(setitem.productId))
                    {
                        errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Das Produkt mit der ID {setitem.productId} gibt es nicht!" }));
                    }

                    if (!int.IsPositive(setitem.productAmount))
                    {
                        errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Das Produkt mit der ID {setitem.productId} muss eine positive Anzahl haben!" }));
                    }
                }
            }

            if (errors.Any())
            {
                return errors;
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

            errors.Add(IdentityResult.Success);
            return errors;


        }
    }
}
