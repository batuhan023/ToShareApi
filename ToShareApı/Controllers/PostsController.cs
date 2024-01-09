using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using ToShareApı.Data;
using ToShareApı.Models;
using ToShareApı.Service;

namespace ToShareApı.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private ApiDbContext _ApiDbContext;
        //private readonly PostService _postService;

        public PostsController(ApiDbContext apiDbContext)
        {
           
            _ApiDbContext = apiDbContext;
            //_postService = postService;
        }


        //[HttpPost("approveapplications/{postId}")]
        //public IActionResult ApproveApplications(int postId)
        //{
        //    _applyService.ApproveApplications(postId);
        //    return Ok("Applications approved successfully");
        //}


        //[HttpGet("updatestatus")]
        //public IActionResult UpdatePostStatus()
        //{
        //    _postService.RunUpdatePostStatus();
        //    return Ok("Post status updated successfully");
        //}


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


        //Add Post son deneme
        [HttpPost("[action]")]
        public async Task<IActionResult> AddNewPost(int userId, int categoryId, string name, string adress, int count
            , string description, string image, DateTime endtime)
        {
            var newPost = new Post
            {
                UserId = userId,
                CategoryId = categoryId,
                Name = name,
                Adres = adress,
                Count = count,
                Description = description,
                Image = image,
                EndTime = endtime,
                StartTime = DateTime.Now
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
        public async Task<IActionResult> GetPostsByPostId(int postId)
        {
            var posts = await _ApiDbContext.Posts.FirstOrDefaultAsync(x => x.Id == postId);
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


        [HttpGet("[action]")]
        public async Task<IActionResult> SearchPostsByLetter(string letter)
        {
            DateTime currentTime = DateTime.Now;

            var matchingPosts = await _ApiDbContext.Posts
                .Where(x => x.EndTime > currentTime)
                .ToListAsync();
            matchingPosts = matchingPosts
                .Where(x => x.Adres != null && x.Adres.Contains(letter, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(matchingPosts);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyPost(int userId, int postId)
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

                //return Ok(apply);

                var jsonOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    MaxDepth = 64 // İsteğe bağlı: Derinlik sınırlarını artırabilirsiniz
                };

                return Ok(JsonSerializer.Serialize(apply, jsonOptions));

            }
            catch (Exception ex)
            {
                return BadRequest($"Başvuru işlemi sırasında bir hata oluştu: {ex.Message}");
            }
        }



        [HttpGet("[action]")]
        public async Task<List<Post>> GetUserAppliedPosts(int userId)
        {
            try
            {
                // Kullanıcının başvurduğu postları getir
                var userAppliedPosts = await _ApiDbContext.Apply
                    .Where(a => a.UserId == userId && a.IsAproved == false)
                    .Select(a => a.Post)
                    .ToListAsync();

                return userAppliedPosts;
            }
            catch (Exception ex)
            {
                // Hata durumunda uygun bir cevap döndür
                Console.WriteLine($"Başvurulan postları getirirken bir hata oluştu: {ex.Message}");
                throw;
            }
        }

        [HttpGet("[action]")]
        public async Task<List<Post>> GetUserApprovedPosts(int userId)
        {
            try
            {
                // Kullanıcının başvurduğu postları getir
                var userAppliedPosts = await _ApiDbContext.Apply
                    .Where(a => a.UserId == userId && a.IsAproved == true)
                    .Select(a => a.Post)
                    .ToListAsync();

                return userAppliedPosts;
            }
            catch (Exception ex)
            {
                // Hata durumunda uygun bir cevap döndür
                Console.WriteLine($"Başvurulan postları getirirken bir hata oluştu: {ex.Message}");
                throw;
            }
        }
    }
}

    
