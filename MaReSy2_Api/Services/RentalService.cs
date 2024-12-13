using MaReSy2_Api.Models.DTO.RentalDTO;
using MaReSy2_Api.Models.DTO.SingleProductDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace MaReSy2_Api.Services
{
    public class RentalService : IRentalService
    {
        private readonly MaReSyDbContext _context;
        public RentalService(MaReSyDbContext context)
        {
            _context = context;
        }

        public Task<List<IdentityResult>> AddNewRentalAsync(CreateRentalDTO rental)
        {
            List<IdentityResult> errors = new List<IdentityResult>();

            //setId != null und setId mit 0 ignorieren

            //productId != null und productAmount mit 0 ignorieren

            //productamount != null und productAmount mit 0 --> fehler

            //product erst wenn setId == null

            return null;
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

            return  allRentals;
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
