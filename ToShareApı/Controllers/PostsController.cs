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
        public async Task<IActionResult> GetCategories()
        {
            var category = await _ApiDbContext.Category.ToListAsync();
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


    }
}

    
