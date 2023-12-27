using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToShareApı.Data;
using ToShareApı.Models;

namespace ToShareApı.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private ApiDbContext _ApiDbContext;

        public PostsController(ApiDbContext apiDbContext)
        {
            _ApiDbContext = apiDbContext;
        }

        //Add Post
        [HttpPost("[action]")]
        public async Task<IActionResult> AddPosts(int userId,int productID, [FromBody] Post posts)
        {
            posts.ProductId = productID;
            posts.UserId = userId;
            _ApiDbContext.Posts.Add(posts);
            await _ApiDbContext.SaveChangesAsync();

            return Ok(posts);
        }

        //Add Post
        [HttpPost("[action]")]
        public async Task<IActionResult> AddPosts2(int userId, int productID, [FromBody] Post posts)
        {
            var newPost = new Post
            {
                ProductId = userId,
                UserId = productID
            };
            _ApiDbContext.Posts.Add(newPost);
            await _ApiDbContext.SaveChangesAsync();
            return Ok(newPost);
        }

            //List by UserId
        [HttpGet("[action]")]
        public async Task<IActionResult> GetPostsByUserId(int userId)
        {
            var posts = await _ApiDbContext.Posts.Where(x => x.UserId == userId).ToListAsync();
            return Ok(posts);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPostsByCategoryId(int categoryId)
        {
            var posts = await _ApiDbContext.Posts.Where(x => x.CategoryId == categoryId).ToListAsync();
            return Ok(posts);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCategories()
        {
            var category = await _ApiDbContext.Category.ToListAsync();
            return Ok(category);
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _ApiDbContext.Posts.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }


        //List All post
        [HttpGet("[action]")]
        public async Task<IActionResult> GetActivePosts()
        {
            DateTime currenttime = DateTime.Now;
            var activePosts = await _ApiDbContext.Posts
                .Where(x => x.EndTime > currenttime)
                .ToListAsync();

            return Ok(activePosts);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyPost(int postId, int userId)
        {
            try
            {
                // Başvuruyu oluştur
                var apply = new Apply
                {
                    PostId = postId,
                    UserId = userId,
                    ApplyTime = DateTime.Now,
                    IsActive = true,
                    IsAproved = false
                };

                // Apply'yi veritabanına ekle
                _ApiDbContext.Apply.Add(apply);
                await _ApiDbContext.SaveChangesAsync();

                // Başvuruyu kullanıcının Apply listesine ekleyin
                var user = await _ApiDbContext.Users.Include(u => u.Applies).FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    if (user.Applies == null)
                        user.Applies = new List<Apply>();

                    user.Applies.Add(apply);
                    await _ApiDbContext.SaveChangesAsync();
                }

                return Ok(apply);
            }
            catch (Exception ex)
            {
                return BadRequest($"Başvuru işlemi sırasında bir hata oluştu: {ex.Message}");
            }
        }


    }
}

    
