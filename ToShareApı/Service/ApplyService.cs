using Microsoft.EntityFrameworkCore;
using ToShareApı.Data;
using ToShareApı.Models;

namespace ToShareApı.Service
{
    public class ApplyService : IHostedService, IDisposable
    {
        //private readonly ApiDbContext _dbContext;

        //public ApplyService(ApiDbContext dbContext)
        //{
        //    _dbContext = dbContext;
        //}

        //public void ApproveApplications(int postId)
        //{
        //    var post = _dbContext.Posts.Include(p => p.Applies).FirstOrDefault(p => p.Id == postId);

        //    if (post != null)
        //    {
        //        foreach (var apply in post.Applies)
        //        {
        //            apply.IsAproved = true;
        //        }

        //        _dbContext.SaveChanges();
        //    }
        //}

        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public ApplyService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Timer'ı başlat, her 5 dakikada bir çalışacak şekilde ayarla (300000 milisaniye)
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var applyService = scope.ServiceProvider.GetRequiredService<ApplyService>();
                applyService.ApproveApplicationsForExpiredPosts();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Timer'ı durdur
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Timer'ı release et
            _timer?.Dispose();
        }

        public async void ApproveApplicationsForExpiredPosts()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

                // Örnek: Tüm başvuruları onayla
                var posts = await dbContext.Posts.Include(p => p.Applies).Where(p => p.EndTime < DateTime.Now).ToListAsync();

                foreach (var post in posts)
                {
                    foreach (var apply in post.Applies)
                    {
                        apply.IsAproved = true;
                    }
                }

                // Değişiklikleri kaydet
                await dbContext.SaveChangesAsync();
            }
        }
    }
    }
