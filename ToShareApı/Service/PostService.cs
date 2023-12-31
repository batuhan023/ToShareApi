using Microsoft.EntityFrameworkCore;
using ToShareApı.Data;

namespace ToShareApı.Service
{
    public class PostService
    {
        private readonly ApiDbContext _dbContext;

        public PostService(ApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void RunUpdatePostStatus()
        {
            // İstenilen yerde bu metodu çağırarak post durumlarını güncelleyebilirsiniz
            UpdatePostStatus();
        }

        private void UpdatePostStatus()
        {
            var posts = _dbContext.Posts.Include(p => p.Applies).ToList();

            foreach (var post in posts)
            {
                if (DateTime.Now > post.EndTime)
                {
                    if (post.Applies != null)
                    {
                        foreach (var apply in post.Applies)
                        {
                            apply.IsActive = false;
                        }
                    }
                }
            }

            _dbContext.SaveChanges();
        }
    }
}
