using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToShareApı.Data;
using ToShareApı.Models;

namespace ToShareApı.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private ApiDbContext _ApiDbContext;
        public UsersController(ApiDbContext apiDbContext)
        {
            _ApiDbContext = apiDbContext;
        }

        //Register
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] User userModel)
        {
            // Model validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the email is already registered
            if (_ApiDbContext.Users.Any(x => x.UserEmail == userModel.UserEmail))
            {
                return BadRequest("Email is already registered.");
            }

            // Hash the password (you should never store passwords in plain text)
            //string hashedPassword = HashPassword(userModel.UserPassword);

            // Create a new user object
            var newUser = new User
            {
                UserName = userModel.UserName,
                UserSurname = userModel.UserSurname,
                UserEmail = userModel.UserEmail,
                UserPassword = userModel.UserPassword,
                UserPhone = userModel.UserPhone,
                //BirthDate = userModel.BirthDate,

            };

            // Add the new user to the database
            _ApiDbContext.Users.Add(newUser);
            await _ApiDbContext.SaveChangesAsync();


            return Ok(new List<User> { newUser });
        }

        //Get user by ıd
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUsersByUserId(string useremail)
        {
            var users = await _ApiDbContext.Users.Where(x => x.UserEmail == useremail).ToListAsync();
            var a = users.FirstOrDefault();
            return Ok(a);
        }



        //Login
        [HttpGet("[action]")]
        public async Task<IActionResult> Login([FromQuery] string email, string password)
        {
            var result = _ApiDbContext.Users.FirstOrDefault(x => x.UserEmail == email && x.UserPassword == password);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(new List<User> { result });
        }

        //Get All Users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _ApiDbContext.Users
                .Include(u => u.Products)
                .Include(u => u.Applies)
                .Select(user => new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    UserSurname = user.UserSurname,
                    UserEmail = user.UserEmail,
                    UserPassword = user.UserPassword,
                    UserPhone = user.UserPhone,
                    BirthDate = user.BirthDate,
                    ProfilPhoto = user.ProfilePhoto,
                    Salary = user.Salary,
                    FamilySize = user.FamilySize,
                    Priority = user.Priority,
                    Products = user.Products.Select(product => new
                    {
                        ProductId = product.Id,
                    }),
                    Applies = user.Applies.Select(apply => new
                    {
                        ApplyId = apply.Id,
                    })
                }).ToListAsync();
            return Ok(users);
        }
    }
}

    
