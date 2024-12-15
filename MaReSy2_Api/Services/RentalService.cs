using System.Diagnostics;
using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.RentalDTO;
using MaReSy2_Api.Models.DTO.SingleProductDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MaReSy2_Api.Services
{
    public class RentalService : IRentalService
    {
        private readonly MaReSyDbContext _context;
        private readonly IUserManagementService _userManagementService;
        private readonly ISetService _setService;
        private readonly IProductService _productService;
        private readonly ISingleProductService _singleProductService;
        public RentalService(MaReSyDbContext context, IUserManagementService userManagementService, IProductService productService, ISetService setService, ISingleProductService singleProductService)
        {
            _context = context;
            _userManagementService = userManagementService;
            _productService = productService;
            _setService = setService;
            _singleProductService = singleProductService;
        }

        public async Task<List<IdentityResult>> AddNewRentalAsync(CreateRentalDTO rental)
        {
            List<IdentityResult> errors = new List<IdentityResult>();

            //wenn setId == null und (productId == null und productamount == null) == null, dann fehler
            if (rental.setId == null && (rental.productId == null || rental.productAmount == null))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Entweder ein Set oder ein Produkt in Kombination mit einer Productamount muss gegeben sein." }));
                return errors;
            }

            //setId und (productId, productamount) dürfen nicht gleichzeitig != null sein
            if (rental.setId != null && (rental.productId != null || rental.productAmount != null))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Es dürfen nicht gleichzeitig ein Set und ein Produkt gegeben sein." }));
                return errors;
            }

            // Wenn es sich um ein Produkt handelt, darf ProductAmount niemals null oder <= 0 sein
            if (rental.productId != null && (rental.productAmount == null || rental.productAmount <= 0))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Bei einem Produkt darf die Productamount niemals null oder <= 0 sein!" }));
            }

            bool datesGiven = rental.fromDate == DateOnly.MaxValue && rental.endDate == DateOnly.MaxValue;

            if (datesGiven)
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Jede Reservierung braucht ein Start- und Enddatum" }));

            }

            else if (!datesGiven && (rental.fromDate > rental.endDate || rental.endDate < rental.fromDate))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Das Startdatum darf nicht nach dem Enddatum liegen bzw. Das Enddatum darf nicht vor dem Startdatum liegen" }));
            }


            //user muss existieren
            var userExists = await _userManagementService.FindUserAsync(rental.userId);

            if (userExists == null)
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Der User mit der angegebenen UserId existiert nicht!" }));

            }

            //das set muss es geben und darf nicht inaktiv sein, wenn es sich um ein set handelt
            if (rental.setId != null)
            {
                var setExists = await _setService.GetSetByIdAsync((int)rental.setId);

                if (setExists == null)
                {
                    errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Das Set mit der angegebenen SetId existiert nicht!" }));
                }

                else if (setExists != null && setExists.Setactive == false)
                {
                    errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Ein inaktives Set kann nicht reserviert werden!" }));
                }

                if (errors.Any()) return errors;
            }

            //das product muss es geben, wenn es sich um ein produkt handelt
            if (rental.productId != null)
            {
                var productExists = await _productService.GetProductByIdAsync((int)rental.productId);

                if (productExists == null)
                {
                    errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Das Proudkt mit der angegebenen ProductId existiert nicht!" }));
                }

                else if (productExists != null && productExists.Productactive == false)
                {
                    errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Ein inaktives Produkt kann nicht reserviert werden!" }));
                }
            }

            var result = await checkAvailabilityAndMakeRental(rental);

            return errors;
        }

        private async Task<List<IdentityResult>> checkAvailabilityAndMakeRental(CreateRentalDTO rental)
        {
            List<IdentityResult> errors = new List<IdentityResult>();

            if (rental.setId != null)
            {
                var set = await _context.Sets.Where(set => set.SetId == rental.setId).FirstAsync();


                    errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Da das Set ein inaktives Produkt enthält, kann es nicht reserviert werden" }));

                var setProducts = _productService.GetProductsForSet((int) rental.setId);

                foreach (var product in await setProducts.ConfigureAwait(false))
                {
                    Debug.WriteLine("Produkt: " + product.product.Productname + "(ID:" + product.product.ProductId + ")");
                    Debug.WriteLine("Amount: " + product.productAmount);
                }

                var rentable = await _singleProductService.GetRentableSingleproducts(22);

                return errors;

                        //var availableAmount = singleProducts.Count - rentedSingleProducts.Count;

                        //var productWithAmount = productsInSet.Where(x => x.Product.ProductId == product.Product.ProductId).First();

                        //if (productWithAmount.Amount < availableAmount)
                        //{
                        //    errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Vom Produkt {productWithAmount.Product.Productname} (ID: {productWithAmount.Product.ProductId}) gibt es nicht mehr genug Einheiten!" }));

                        //    return errors;
                        //}
             }
            return errors;

                }


                //next step: get the singleproducts of the product and check for rentals
                //



                //and if there is an uncompleted rental (noch nicht vorbei, wurde noch nicht zurückgebracht)
                //oder wenn abgelehnt auch frei
                //schauen welche singleproducts in den uncompleted rentals sind und diese für neue rentals ausschließen
                //schauen ob genügend singleproducts von diesem product da sind, wenn ja rental durchführen

        public async Task<IEnumerable<RentalDTO>> GetAllRentalsAsync()
        {
            var rentals = _context.Rentals
                .Include(rental => rental.User)
                .Include(rental => rental.Set)
                .Include(rental => rental.RentalFreigabeUserNavigation)
                .Include(rental => rental.RentalAblehnungUserNavigation)
                .Include(rental => rental.RentalAuslieferungUserNavigation)
                .Include(rental => rental.RentalZurückgabeUserNavigation)
                .Include(rental => rental.StatusNavigation)
                .Include(rental => rental.SingleProducts)
                    .ThenInclude(singpleproduct => singpleproduct.Product);




            var allRentals = await rentals.Select(rental => new RentalDTO
            {
                rentalId = rental.RentalId,

                user = new RentalUserDTO
                {
                    userId = rental.User.UserId,
                    username = rental.User.Username
                },
                setId = rental.Set != null ? rental.Set.SetId : null,
                setname = rental.Set != null ? rental.Set.Setname : null,

                rentalStart = DateOnly.FromDateTime(rental.RentalStart),
                rentalEnd = DateOnly.FromDateTime(rental.RentalEnd),

                rentalAnforderung = rental.RentalAnforderung,

                rentalFreigabe = rental.RentalFreigabe ?? null,
                rentalFreigabeUser = rental.RentalFreigabeUserNavigation != null ? new RentalUserDTO
                {
                    userId = rental.RentalFreigabeUserNavigation.UserId,
                    username = rental.RentalFreigabeUserNavigation.Username,
                } : null,

                rentalAblehnung = rental.RentalAblehnung ?? null,
                rentalAblehnungUser = rental.RentalAblehnungUserNavigation != null ? new RentalUserDTO
                {
                    userId = rental.RentalAblehnungUserNavigation.UserId,
                    username = rental.RentalAblehnungUserNavigation.Username,
                } : null,

                rentalAuslieferung = rental.RentalAuslieferung ?? null,
                rentalAuslieferungUser = rental.RentalAuslieferungUserNavigation != null ? new RentalUserDTO
                {
                    userId = rental.RentalAuslieferungUserNavigation.UserId,
                    username = rental.RentalAuslieferungUserNavigation.Username,
                } : null,

                rentalZuereck = rental.RentalZurückgabe ?? null,
                rentalZuereckUser = rental.RentalZurückgabeUserNavigation != null ? new RentalUserDTO
                {
                    userId = rental.RentalZurückgabeUserNavigation.UserId,
                    username = rental.RentalZurückgabeUserNavigation.Username,
                } : null,

                rentalStornierung = rental.RentalStornierung ?? null,

                status = rental.StatusNavigation.Bezeichnung,

                rentalNote = rental.RentalNote ?? null,

                singleProducts = rental.SingleProducts.Select(singleproduct => new RentalSingleProductDTO
                {
                    singleProductId = singleproduct.ProductId,
                    singleProductName = singleproduct.SingleProductName,
                    singleProductNumber = singleproduct.SingleProductNumber,
                    singleProductCategory = singleproduct.Product.Productname
                }).ToList(),

            }).ToListAsync();

            return allRentals;
        }

        public async Task<IEnumerable<RentalDTO>> GetAllUserRentalsAsync(int userId)
        {
            var rentals = _context.Rentals
                .Include(rental => rental.User)
                .Include(rental => rental.Set)
                .Include(rental => rental.RentalFreigabeUserNavigation)
                .Include(rental => rental.RentalAblehnungUserNavigation)
                .Include(rental => rental.RentalAuslieferungUserNavigation)
                .Include(rental => rental.RentalZurückgabeUserNavigation)
                .Include(rental => rental.StatusNavigation)
                .Include(rental => rental.SingleProducts)
                    .ThenInclude(singpleproduct => singpleproduct.Product)
                .Where(rental => rental.UserId == userId);





            var allRentals = await rentals.Select(rental => new RentalDTO
            {
                rentalId = rental.RentalId,

                user = new RentalUserDTO
                {
                    userId = rental.User.UserId,
                    username = rental.User.Username
                },
                setId = rental.Set != null ? rental.Set.SetId : null,
                setname = rental.Set != null ? rental.Set.Setname : null,

                rentalStart = DateOnly.FromDateTime(rental.RentalStart),
                rentalEnd = DateOnly.FromDateTime(rental.RentalEnd),

                rentalAnforderung = rental.RentalAnforderung,

                rentalFreigabe = rental.RentalFreigabe ?? null,
                rentalFreigabeUser = rental.RentalFreigabeUserNavigation != null ? new RentalUserDTO
                {
                    userId = rental.RentalFreigabeUserNavigation.UserId,
                    username = rental.RentalFreigabeUserNavigation.Username,
                } : null,

                rentalAblehnung = rental.RentalAblehnung ?? null,
                rentalAblehnungUser = rental.RentalAblehnungUserNavigation != null ? new RentalUserDTO
                {
                    userId = rental.RentalAblehnungUserNavigation.UserId,
                    username = rental.RentalAblehnungUserNavigation.Username,
                } : null,

                rentalAuslieferung = rental.RentalAuslieferung ?? null,
                rentalAuslieferungUser = rental.RentalAuslieferungUserNavigation != null ? new RentalUserDTO
                {
                    userId = rental.RentalAuslieferungUserNavigation.UserId,
                    username = rental.RentalAuslieferungUserNavigation.Username,
                } : null,

                rentalZuereck = rental.RentalZurückgabe ?? null,
                rentalZuereckUser = rental.RentalZurückgabeUserNavigation != null ? new RentalUserDTO
                {
                    userId = rental.RentalZurückgabeUserNavigation.UserId,
                    username = rental.RentalZurückgabeUserNavigation.Username,
                } : null,

                rentalStornierung = rental.RentalStornierung ?? null,

                status = rental.StatusNavigation.Bezeichnung,

                rentalNote = rental.RentalNote ?? null,

                singleProducts = rental.SingleProducts.Select(singleproduct => new RentalSingleProductDTO
                {
                    singleProductId = singleproduct.ProductId,
                    singleProductName = singleproduct.SingleProductName,
                    singleProductNumber = singleproduct.SingleProductNumber,
                    singleProductCategory = singleproduct.Product.Productname
                }).ToList(),

            }).ToListAsync();

            return allRentals;

        }




    }
}
