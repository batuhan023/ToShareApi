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
        public async Task<IActionResult> AddPosts(int userId, [FromBody] Post posts)
        {
            posts.UserId = userId;
            _ApiDbContext.Posts.Add(posts);
            await _ApiDbContext.SaveChangesAsync();

            return Ok(posts);
        }

        //List by UserId
        [HttpGet("[action]")]
        public async Task<IActionResult> GetPostsByUserId(int userId)
        {
            var posts = await _ApiDbContext.Posts.Where(x => x.UserId == userId).ToListAsync();
            return Ok(posts);
        }

    }
}

    
