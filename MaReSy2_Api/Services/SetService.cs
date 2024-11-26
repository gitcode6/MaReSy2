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

        public async Task<SetDTO?> GetSetByIdAsync(int setId)
        {
            var result = await SetExistsAsync(setId);

            SetDTO? set = null;

            if (result == true)
            {
                var set_fromDb = await _context.Sets.FirstAsync(x => x.SetId == setId);

                set = new SetDTO
                {
                    SetId = set_fromDb.SetId,
                    Setname = set_fromDb.Setname,
                    Setdescription = set_fromDb.Setdescription,
                    Setactive = set_fromDb.Setactive,
                    ProductimageLink = null,
                };
            }

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
    }
}
