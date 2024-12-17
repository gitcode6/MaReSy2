using System.Diagnostics;
using MaReSy2_Api.Data.Models;
using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.RentalDTO;
using MaReSy2_Api.Models.DTO.SingleProductDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using NuGet.Packaging;
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

            if (rental.fromDate < DateOnly.FromDateTime(DateTime.Today))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Nachträglich kann keine Reservierung angelegt werden!" }));
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
                    errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Das Produkt mit der angegebenen ProductId existiert nicht!" }));
                }

                else if (productExists != null && productExists.Productactive == false)
                {
                    errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Ein inaktives Produkt kann nicht reserviert werden!" }));
                }
            }

            var result = await checkAvailabilityAndMakeRental(rental);

            if (result.Count > 0)
            {
                errors.AddRange(result);
            }

            return errors;
        }

        private async Task<List<IdentityResult>> checkAvailabilityAndMakeRental(CreateRentalDTO rental)
        {
            List<IdentityResult> errors = new List<IdentityResult>();
            List<SingleProduct> singleProductsForRental = new List<SingleProduct>();


            if (rental.setId != null)
            {
                var set = await _context.Sets.Where(set => set.SetId == rental.setId).FirstAsync();

                if (await _productService.SetContainsInactiveProduct(set.SetId))
                {
                    errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Da das Set ein inaktives Produkt enthält, kann es nicht reserviert werden" }));

                }

                if (errors.Any()) return errors;

                var setProducts = await _productService.GetProductsForSet((int)rental.setId);

                foreach (var product in setProducts)
                {
                    int neededAmount = product.productAmount;
                    Product needed_product = product.product;

                    var rentable = await _singleProductService.GetRentableSingleproducts(needed_product.ProductId);



                    if (!(rentable.Count >= neededAmount))
                    {
                        errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Es sind nicht mehr genug Einheiten von {needed_product.Productname} (ID: {needed_product.ProductId}) verfügbar, daher kann das Set ({set.Setname} (setId: {set.SetId})" }));
                    }

                    else
                    {
                        var neededSingleproducts = await _singleProductService.GetNeededSingleProducts(needed_product.ProductId, neededAmount);

                        singleProductsForRental.AddRange(neededSingleproducts);
                    }
                }

                Rental newRental = new Rental()
                {
                    UserId = rental.userId,
                    SetId = set.SetId,
                    RentalStart = rental.fromDate,
                    RentalEnd = rental.endDate,
                    RentalAnforderung = DateTime.Now,
                    Status = 1,
                    RentalNote = rental.rentalNote

                };

                newRental.SingleProducts.AddRange(singleProductsForRental);

                await _context.Rentals.AddAsync(newRental);
                await _context.SaveChangesAsync();


                errors.Add(IdentityResult.Success);

                return errors;

                //var availableAmount = singleProducts.Count - rentedSingleProducts.Count;

                //var productWithAmount = productsInSet.Where(x => x.Product.ProductId == product.Product.ProductId).First();

                //if (productWithAmount.Amount < availableAmount)
                //{
                //    errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Vom Produkt {productWithAmount.Product.Productname} (ID: {productWithAmount.Product.ProductId}) gibt es nicht mehr genug Einheiten!" }));

                //    return errors;
                //}
            }

            if (rental.productId != null || rental.productId != 0)
            {
                var needed_product = await _context.Products.Where(product => product.ProductId == rental.productId).FirstOrDefaultAsync();

                var rentable = await _singleProductService.GetRentableSingleproducts(needed_product!.ProductId);

                if (!(rentable.Count >= rental.productAmount))
                {

                    errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Es sind nicht mehr genug Einheiten von {needed_product.Productname} (ID: {needed_product.ProductId}) verfügbar, daher kann das Produkt derzeit nicht reserviert werden!" }));
                    return errors;

                }
                else
                {
                    singleProductsForRental.AddRange(await _singleProductService.GetNeededSingleProducts(needed_product.ProductId, (int)rental.productAmount));

                    Rental newRental = new Rental()
                    {
                        UserId = rental.userId,
                        RentalStart = rental.fromDate,
                        RentalEnd = rental.endDate,
                        RentalAnforderung = DateTime.Now,
                        Status = 1,
                        RentalNote = rental.rentalNote
                    };

                    newRental.SingleProducts.AddRange(singleProductsForRental);

                    await _context.AddAsync(newRental);
                    await _context.SaveChangesAsync();


                    errors.Add(IdentityResult.Success);


                }

            }
            return errors;

        }
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

                rentalStart = rental.RentalStart,
                rentalEnd = rental.RentalEnd,

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

        public async Task<RentalDTO?> GetRentalAsync(int rentalId, int userId)
        {
            var rental = await _context.Rentals
                .Include(rental => rental.User)
                .Include(rental => rental.Set)
                .Include(rental => rental.RentalFreigabeUserNavigation)
                .Include(rental => rental.RentalAblehnungUserNavigation)
                .Include(rental => rental.RentalAuslieferungUserNavigation)
                .Include(rental => rental.RentalZurückgabeUserNavigation)
                .Include(rental => rental.StatusNavigation)
                .Include(rental => rental.SingleProducts)
                    .ThenInclude(singpleproduct => singpleproduct.Product)
                .Where(rental => rental.RentalId == rentalId).FirstOrDefaultAsync();

            var user = await _context.Users
                .Include(users => users.Role)
                .Where(user => user.UserId == userId).FirstOrDefaultAsync();


            if (rental == null || user == null || (user.Role.Rolename.ToLower() != "admin"  && rental.UserId != userId))
            {
                return null;
            }


            var userRental = new RentalDTO
            {
                rentalId = rental.RentalId,

                user = new RentalUserDTO
                {
                    userId = rental.User.UserId,
                    username = rental.User.Username
                },
                setId = rental.Set != null ? rental.Set.SetId : null,
                setname = rental.Set != null ? rental.Set.Setname : null,

                rentalStart = rental.RentalStart,
                rentalEnd = rental.RentalEnd,

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

            };

            return userRental;
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

                rentalStart = rental.RentalStart,
                rentalEnd = rental.RentalEnd,

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

        public async Task<IEnumerable<IdentityResult>> UpdateRental(ActionDTO rentalAction)
        {
            List<IdentityResult> errors = new List<IdentityResult>();

            var rental = await _context.Rentals
                .Include(rental => rental.StatusNavigation)
                .Where(rental => rental.RentalId == rentalAction.rentalId).FirstOrDefaultAsync();

            var actionUser = await _context.Users.Where(user => user.UserId == rentalAction.actionUserId).FirstOrDefaultAsync();

            if (rental == null)
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Es gibt kein Rental mit der angegebenen Id" }));
                return errors;
            }

            if (actionUser == null)
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Der ActionUser mit der angegebenen Id existiert nicht" }));
            }



            switch (rentalAction.action)
            {
                case 1:
                    //ablehnen
                    //möglich wenn
                    /**
                     * angefordert
                     * freigegeben
                     */

                    if (rental.Status == 1 || rental.Status == 2)
                    {
                        rental.Status = 3;
                        rental.RentalFreigabe = DateTime.Now;
                        rental.RentalFreigabeUser = rentalAction.actionUserId;
                        _context.Rentals.Update(rental);
                        await _context.SaveChangesAsync();
                        errors.Add(IdentityResult.Success);
                    }
                    else
                    {
                        errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Das Rental befindet sich im Status {rental.StatusNavigation.Bezeichnung}, in diesem Status kann es nicht mehr abgelehnt werden!" }));
                    }
                    break;

                case 2:
                    //freigeben
                    //möglich wenn
                    /**
                     * angefordert
                     * abgelehnt
                     */

                    if (rental.Status == 1 || rental.Status == 3)
                    {
                        rental.Status = 2;
                        rental.RentalFreigabe = DateTime.Now;
                        rental.RentalFreigabeUser = rentalAction.actionUserId;
                        _context.Rentals.Update(rental);
                        await _context.SaveChangesAsync();
                        errors.Add(IdentityResult.Success);
                    }
                    else
                    {
                        errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Das Rental ({rental.RentalId}) befindet sich im Status {rental.StatusNavigation.Bezeichnung}, in diesem Status kann es nicht mehr freigegeben werden werden!" }));
                    }
                    break;


                case 3:
                    //ausliefern
                    //möglich wenn
                    /**
                     * freigegeben 
                     */
                    if (rental.Status == 2)
                    {
                        rental.Status = 4;
                        rental.RentalFreigabe = DateTime.Now;
                        rental.RentalFreigabeUser = rentalAction.actionUserId;
                        _context.Rentals.Update(rental);
                        await _context.SaveChangesAsync();
                        errors.Add(IdentityResult.Success);
                    }

                    else
                    {
                        errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Das Rental befindet sich im Status {rental.StatusNavigation.Bezeichnung}, in diesem Status kann es noch nicht/nicht mehr ausgeliefert werden!" }));
                    }

                    break;


                case 4:
                    //zurücknehmen
                    //möglich wenn
                    /**
                     * ausgeliefert
                     */

                    if (rental.Status == 4)
                    {
                        rental.Status = 5;
                        rental.RentalFreigabe = DateTime.Now;
                        rental.RentalFreigabeUser = rentalAction.actionUserId;
                        _context.Rentals.Update(rental);
                        await _context.SaveChangesAsync();
                        errors.Add(IdentityResult.Success);
                    }

                    else
                    {
                        errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Das Rental befindet sich im Status {rental.StatusNavigation.Bezeichnung}, in diesem Status kann es noch nicht/nicht mehr zurückgenommen werden!" }));
                    }


                    break;


                default:
                    errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Die angegebene Action gibt es nicht." }));

                    break;
            }

            return errors;

        }

        public async Task<List<IdentityResult>> userCancelRental(int rentalId, int userId)
        {
            List<IdentityResult> errors = new List<IdentityResult>();

            var rental = await _context.Rentals
                .Where(rental => rental.RentalId == rentalId).FirstOrDefaultAsync();

            var user = await _context.Users.Where(user=>user.UserId == userId).FirstOrDefaultAsync();


            if(rental == null)
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Das Rental mit der ID {rentalId} gibt es nicht!" }));
            }


            if(user == null)
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Den User mit der ID {userId} gibt es nicht!" }));
            }

            if(rental != null && user != null && rental.UserId != userId)
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = $"Die Rental befindet sich nicht im Besitz von {user.Username}, daher kann Sie nicht storniert werden!" }));
            }

            if (errors.Any()) return errors;

            rental!.RentalStornierung = DateTime.Now;
            rental!.Status = 6;

            _context.Rentals.Update(rental);
            await _context.SaveChangesAsync();
            errors.Add(IdentityResult.Success);

            return errors;






        }




        public async Task<bool> clearFreigabeFields(int rentalId)
        {
            var rental = await _context.Rentals.Where(rental => rental.RentalId == rentalId).FirstOrDefaultAsync();

            if (rental != null)
            {
                rental.RentalFreigabe = null;
                rental.RentalFreigabeUser = null;
                _context.Rentals.Update(rental);
                return Convert.ToBoolean(await _context.SaveChangesAsync());
            }

            return false;
        }

        public async Task<bool> clearAblehnungFields(int rentalId)
        {
            var rental = await _context.Rentals.Where(rental => rental.RentalId == rentalId).FirstOrDefaultAsync();

            if (rental != null)
            {
                rental.RentalAblehnung = null;
                rental.RentalAblehnungUser = null;
                _context.Rentals.Update(rental);
                return Convert.ToBoolean(await _context.SaveChangesAsync());
            }

            return false;
        }

        public async Task<bool> clearAuslieferungFields(int rentalId)
        {
            var rental = await _context.Rentals.Where(rental => rental.RentalId == rentalId).FirstOrDefaultAsync();

            if (rental != null)
            {
                rental.RentalAuslieferung = null;
                rental.RentalAuslieferungUser = null;
                _context.Rentals.Update(rental);
                return Convert.ToBoolean(await _context.SaveChangesAsync());
            }

            return false;


        }

        public async Task<bool> clearZurückgabeFields(int rentalId)
        {
            var rental = await _context.Rentals.Where(rental => rental.RentalId == rentalId).FirstOrDefaultAsync();

            if (rental != null)
            {
                rental.RentalZurückgabe = null;
                rental.RentalZurückgabe = null;
                _context.Rentals.Update(rental);
                return Convert.ToBoolean(await _context.SaveChangesAsync());
            }

            return false;

        }


    }
}
