using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using Azure;
using MaReSy2_Api.Data.Models;
using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.RentalDTO;
using MaReSy2_Api.Models.DTO.SingleProductDTO;
using MaReSy2_Api.Services.ProductService;
using MaReSy2_Api.Services.SingleProductService;
using MaReSy2_Api.Services.UserService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Identity.Client;
using NuGet.DependencyResolver;
using NuGet.Packaging;

namespace MaReSy2_Api.Services.RentalService
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

        public async Task<APIResponse<string>> AddNewRentalAsync(CreateRentalDTO rental, int userId)
        {
            var result = new APIResponse<string>();

            List<IdentityResult> errors = new List<IdentityResult>();

            //wenn setId == null und (productId == null und productamount == null) == null, dann fehler
            if (rental.setId == null && (rental.productId == null || rental.productAmount == null))
            {
                result.Errors.Add(new ErrorDetail { Field = "Set/Product", Error = "Entweder ein Set oder ein Produkt in Kombination mit einer Productamount muss gegeben sein." });

            }

            //setId und (productId, productamount) dürfen nicht gleichzeitig != null sein
            if (rental.setId != null && (rental.productId != null || rental.productAmount != null))
            {
                result.Errors.Add(new ErrorDetail { Field = "Set/Product", Error = "Es dürfen nicht gleichzeitig ein Set und ein Produkt gegeben sein." });

                result.StatusCode = 400;
                return result;
            }

            // Wenn es sich um ein Produkt handelt, darf ProductAmount niemals null oder <= 0 sein
            if (rental.productId != null && (rental.productAmount == null || rental.productAmount <= 0))
            {
                result.Errors.Add(new ErrorDetail { Field = "ProductAmount", Error = "Bei einem Produkt darf die Productamount niemals null oder <= 0 sein!" });
            }

            bool datesGiven = rental.fromDate == DateOnly.MaxValue && rental.endDate == DateOnly.MaxValue;

            if (datesGiven)
            {
                result.Errors.Add(new ErrorDetail { Field = "Dates", Error = "Jede Reservierung braucht ein Start- und Enddatum." });

            }

            if (rental.fromDate < DateOnly.FromDateTime(DateTime.Today))
            {
                result.Errors.Add(new ErrorDetail { Field = "FromDate", Error = "Nachträglich kann keine Reservierung angelegt werden!" });
            }

            else if (!datesGiven && (rental.fromDate > rental.endDate || rental.endDate < rental.fromDate))
            {
                result.Errors.Add(new ErrorDetail { Field = "Dates", Error = "Das Startdatum darf nicht nach dem Enddatum liegen bzw. Das Enddatum darf nicht vor dem Startdatum liegen." });
            }

            if (result.Errors.Any())
            {
                result.StatusCode = 400;
                return result;
            }


            //user muss existieren
            var userExists = await _userManagementService.FindUserAsync(userId);

            if (userExists.Data == null)
            {
                result.Errors.Add(new ErrorDetail { Field = "UserId", Error = "Der User mit der angegebenen UserId existiert nicht!" });
                result.StatusCode = 404;
                return result;

            }

            //das set muss es geben und darf nicht inaktiv sein, wenn es sich um ein set handelt
            if (rental.setId != null)
            {
                var setExists = await _setService.GetSetByIdAsync((int)rental.setId);

                if (setExists.Data == null)
                {

                    result.Errors.Add(new ErrorDetail { Field = "SetId", Error = "Das Set mit der angegebenen SetId existiert nicht!" });

                    result.StatusCode = 404;
                    return result;
                }

                else if (setExists.Data != null && setExists.Data!.Setactive == false)
                {
                    result.Errors.Add(new ErrorDetail { Field = "setActive", Error = "Ein inaktives Set kann nicht reserviert werden!" });

                    result.StatusCode = 400;
                    return result;

                }

            }

            //das product muss es geben, wenn es sich um ein produkt handelt
            if (rental.productId != null)
            {
                var productExists = await _productService.GetProductByIdAsync((int)rental.productId);

                if (productExists.Data == null)
                {
                    result.Errors.Add(new ErrorDetail { Field = "productId", Error = "Das Produkt mit der angegebenen ProductId existiert nicht!" });

                }

                else if (productExists.Data != null && productExists.Data.Productactive == false)
                {
                    result.Errors.Add(new ErrorDetail { Field = "productId", Error = "Ein inaktives Produkt kann nicht reserviert werden!" });
                }
            }

            var response = await checkAvailabilityAndMakeRental(rental, userId);

            if (response.Errors.Any())
            {
                result.Errors.AddRange(response.Errors);
                return result;
            }

            if (response.Data?.Count > 0)
            {
                result.Data = "Reservierung erfolgreich angelegt.";
            }

            return result;
        }

        private async Task<APIResponse<List<SingleProduct>>> checkAvailabilityAndMakeRental(CreateRentalDTO rental, int userId)
        {

            var result = new APIResponse<List<SingleProduct>>();
            List<SingleProduct> singleProductsForRental = new List<SingleProduct>();





            if (rental.setId != null)
            {
                var set = await _context.Sets.Where(set => set.SetId == rental.setId).FirstAsync();


                if (set == null)
                {
                    result.Errors.Add(new ErrorDetail { Field = "SetId", Error = "Das Set konnte nicht gefunden werden." });
                    result.StatusCode = 404;
                    return result;
                }

                if (await _productService.SetContainsInactiveProduct(set.SetId))
                {
                    result.Errors.Add(new ErrorDetail { Field = "Set", Error = "Da das Set ein inaktives Produkt enthält, kann es nicht reserviert werden." });
                    result.StatusCode = 400;
                    return result;
                }




                var setProducts = await _productService.GetProductsForSet((int)rental.setId);

                foreach (var product in setProducts)
                {
                    int neededAmount = product.productAmount;
                    Product needed_product = product.product;

                    var rentable = await _singleProductService.GetRentableSingleproducts(needed_product.ProductId);



                    if (!(rentable.Count >= neededAmount))
                    {
                        result.Errors.Add(new ErrorDetail { Field = "Product", Error = $"Es sind nicht genug Einheiten von {needed_product.Productname} (ID: {needed_product.ProductId}) verfügbar, daher kann das Set {set.Setname} (SetId: {set.SetId}) nicht reserviert werden." });
                    }

                    else
                    {
                        var neededSingleproducts = await _singleProductService.GetNeededSingleProducts(needed_product.ProductId, neededAmount);

                        singleProductsForRental.AddRange(neededSingleproducts);
                    }
                }

                if (result.Errors.Any())
                {
                    result.StatusCode = 400;
                    return result;
                }

                Rental newRental = new Rental()
                {
                    UserId = userId,
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


                result.Data = singleProductsForRental;
                result.StatusCode = 200;
                return result;
            }

            if (rental.productId != null || rental.productId != 0)
            {
                var needed_product = await _context.Products.Where(product => product.ProductId == rental.productId).FirstOrDefaultAsync();

                if (needed_product == null)
                {
                    //result.Errors.Add(new ErrorDetail { Field = "ProductId", Error = "Das Produkt konnte nicht gefunden werden." });
                    result.StatusCode = 404;
                    return result;
                }

                var rentable = await _singleProductService.GetRentableSingleproducts(needed_product!.ProductId);

                if (!(rentable.Count >= rental.productAmount))
                {

                    result.Errors.Add(new ErrorDetail { Field = "ProductAmount", Error = $"Es sind nicht genug Einheiten von {needed_product.Productname} (ID: {needed_product.ProductId}) verfügbar, daher kann das Produkt derzeit nicht reserviert werden." });
                    result.StatusCode = 400;
                    return result;

                }
                else
                {
                    singleProductsForRental.AddRange(await _singleProductService.GetNeededSingleProducts(needed_product.ProductId, (int)rental.productAmount));

                    Rental newRental = new Rental()
                    {
                        UserId = userId,
                        RentalStart = rental.fromDate,
                        RentalEnd = rental.endDate,
                        RentalAnforderung = DateTime.Now,
                        Status = 1,
                        RentalNote = rental.rentalNote
                    };

                    newRental.SingleProducts.AddRange(singleProductsForRental);

                    await _context.AddAsync(newRental);
                    await _context.SaveChangesAsync();


                    result.Data = singleProductsForRental;
                    result.StatusCode = 200;

                }

            }
            return result;

        }
        public async Task<APIResponse<List<RentalDTO>>> GetAllRentalsAsync()
        {
            var result = new APIResponse<List<RentalDTO>>();

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

            result.Data = allRentals;
            result.StatusCode = 200;
            return result;
        }

        public async Task<APIResponse<RentalDTO>> GetRentalAsync(int rentalId, int userId)
        {
            var result = new APIResponse<RentalDTO>();

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


            if (rental == null || user == null || user.Role.Rolename.ToLower() != "admin" && rental.UserId != userId)
            {
                result.Errors.Add(new ErrorDetail { Field = "Rental", Error = "Mietobjekt nicht gefunden oder Benutzer nicht berechtigt." });
                result.StatusCode = 403;
                return result;
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

            result.Data = userRental;
            result.StatusCode = 200;

            return result;
        }

        public async Task<APIResponse<List<RentalDTO>>> GetAllUserRentalsAsync(int userId)
        {
            var result = new APIResponse<List<RentalDTO>>();

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

            result.Data = allRentals;
            result.StatusCode = 200;

            return result;
        }

        public async Task<APIResponse<string>> UpdateRental(int userId, ActionDTO rentalAction)
        {
            var result = new APIResponse<string>();

            var rental = await _context.Rentals
                .Include(rental => rental.StatusNavigation)
                .Where(rental => rental.RentalId == rentalAction.rentalId).FirstOrDefaultAsync();

            var actionUser = await _context.Users.Where(user => user.UserId == userId).FirstOrDefaultAsync();

            if (rental == null)
            {
                result.Errors.Add(new ErrorDetail { Field = "RentalId", Error = "Es gibt kein Rental mit der angegebenen Id" });
                return result;
            }

            if (actionUser == null)
            {
                result.Errors.Add(new ErrorDetail { Field = "ActionUserId", Error = "Der ActionUser mit der angegebenen Id existiert nicht" });
                return result;
            }

            if (actionUser!.RoleId != 2)
            {
                result.Errors.Add(new ErrorDetail { Field = "ActionUser", Error = $"Der User {actionUser.Username} hat keine Berechtigung diese Aktion auszuführen." });
                return result;
            }


            switch (rentalAction.action)
            {
                case -1:
                    //ablehnen
                    if (rental!.Status == 1)
                    {
                        rental!.Status = 3;
                        rental.RentalAblehnung = DateTime.Now;
                        rental.RentalAblehnungUser = actionUser.UserId;
                        _context.Rentals.Update(rental);
                        await _context.SaveChangesAsync();
                        result.Data = "Rental wurde abgelehnt.";
                    }
                    else
                    {
                        result.Errors.Add(new ErrorDetail { Field = "Status", Error = $"Das Rental befindet sich im Status {rental.StatusNavigation.Bezeichnung}, in diesem Status kann es nicht abgelehnt werden!" });
                    }



                    break;

                case 0:
                    if (rental!.Status == 3)
                    {
                        await clearAblehnungFields(rental.RentalId);
                    }

                    if (rental!.Status == 2)
                    {
                        await clearFreigabeFields(rental.RentalId);
                    }

                    if (rental!.Status == 3 || rental!.Status == 2)
                    {
                        rental!.Status = 1;
                        _context.Rentals.Update(rental);
                        await _context.SaveChangesAsync();
                        result.Data = "Rental wurde zurückgesetzt.";
                    }
                    if(rental!.Status != 1)
                    {
                        result.Errors.Add(new ErrorDetail { Field = "Status", Error = $"Das Rental befindet sich im Status {rental.StatusNavigation.Bezeichnung}, in diesem Status kann es nicht zurückgesetzt werden!" });
                    }


                    break;

                case 1:
                    //freigeben

                    if (rental!.Status == 3)
                    {
                        await clearAblehnungFields(rental.RentalId);
                    }

                    if (rental!.Status == 4)
                    {
                        await clearAuslieferungFields(rental.RentalId);

                    }



                    if (rental!.Status == 1 || rental!.Status == 3)
                    {
                        rental!.Status = 2;
                        rental.RentalFreigabe = DateTime.Now;
                        rental.RentalFreigabeUser = actionUser.UserId;
                        _context.Rentals.Update(rental);
                        await _context.SaveChangesAsync();
                        result.Data = "Rental wurde freigegeben.";
                    }

                    if (rental!.Status == 4)
                    {
                        rental!.Status = 2;
                        _context.Rentals.Update(rental);
                        await _context.SaveChangesAsync();
                    }



                    break;


                case 2:
                    //ausliefern
                    //freigabe oder rückgabe


                    if (rental!.Status == 5)
                    {
                        await clearZurückgabeFields(rental.RentalId);
                        rental!.Status = 4;
                        _context.Rentals.Update(rental);
                        await _context.SaveChangesAsync();
                    }

                    if (rental!.Status == 2)
                    {
                        rental!.Status = 4;
                        rental.RentalAuslieferung = DateTime.Now;
                        rental.RentalAuslieferungUser = actionUser.UserId;
                        _context.Rentals.Update(rental);
                        await _context.SaveChangesAsync();
                        result.Data = "Rental wurde ausgeliefert.";
                    }
                    if(rental!.Status != 4)
                    {
                        result.Errors.Add(new ErrorDetail { Field = "Status", Error = $"Das Rental befindet sich im Status {rental.StatusNavigation.Bezeichnung}, in diesem Status kann es nicht ausgeliefert werden!" });
                    }




                    break;


                case 3:

                    //zurücknehmen
                    if (rental!.Status == 4)
                    {
                        rental!.Status = 5;
                        rental.RentalZurückgabe = DateTime.Now;
                        rental.RentalZurückgabeUser = actionUser.UserId;
                        _context.Rentals.Update(rental);
                        await _context.SaveChangesAsync();
                        result.Data = "Rental wurde zurückgenommen.";

                    }
                    if(rental!.Status != 5)
                    {
                        result.Errors.Add(new ErrorDetail { Field = "Status", Error = $"Das Rental befindet sich im Status {rental.StatusNavigation.Bezeichnung}, in diesem Status kann es nicht zurückgenommen werden!" });
                    }



                    break;


                default:
                    result.Errors.Add(new ErrorDetail { Field = "Action", Error = "Die angegebene Action gibt es nicht." });

                    break;
            }

            return result;

        }

        public async Task<APIResponse<string>> userCancelRental(int rentalId, int userId)
        {
            var result = new APIResponse<string>();

            var rental = await _context.Rentals
                .Where(rental => rental.RentalId == rentalId)
                .Include(rental => rental.StatusNavigation)
                .FirstOrDefaultAsync();

            var user = await _context.Users.Where(user => user.UserId == userId).FirstOrDefaultAsync();


            if (rental == null)
            {
                result.Errors.Add(new ErrorDetail { Field = "RentalId", Error = $"Das Rental mit der ID {rentalId} gibt es nicht!" });
            }


            if (user == null)
            {
                result.Errors.Add(new ErrorDetail { Field = "UserId", Error = $"Den User mit der ID {userId} gibt es nicht!" });
            }

            if (result.Errors.Any())
            {
                result.StatusCode = 404;
                return result;
            }


            if (rental != null && user != null && rental.UserId != userId)
            {
                result.Errors.Add(new ErrorDetail { Field = "userId", Error = $"Die Rental befindet sich nicht im Besitz von {user.Username}, daher kann Sie nicht storniert werden!" });
                result.StatusCode = 403;
                return result;

            }


            //nur möglich, wenn reservierung im Status angefordert

            if (rental!.Status == 1)
            {
                rental!.RentalStornierung = DateTime.Now;
                rental!.Status = 6;

                _context.Rentals.Update(rental);
                await _context.SaveChangesAsync();
                result.Message = "Rental wurde storniert.";
            }

            else
            {
                result.Errors.Add(new ErrorDetail { Field = "Status", Error = $"Das Rental befindet sich im Status {rental.StatusNavigation.Bezeichnung}, in diesem Status kann es nicht mehr storniert werden!" });
            }



            return result;






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
                rental.RentalZurückgabeUser = null;
                _context.Rentals.Update(rental);
                return Convert.ToBoolean(await _context.SaveChangesAsync());
            }

            return false;

        }


    }
}
