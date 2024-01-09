using Microsoft.EntityFrameworkCore;
using ToShareApı.Data;
using ToShareApı.Models;

namespace ToShareApı.Service
{
    public class ApplyApprovalService
    {
            private Timer _timer;
            private readonly IServiceProvider _serviceProvider;
            private readonly object _lockObject = new object();

            public ApplyApprovalService(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;

                // Timer'ı başlat ve belirli aralıklarla kontrol et
                _timer = new Timer(CheckPostApplies, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); // 15 dakikada bir kontrol et, bu süreyi ihtiyacınıza göre ayarlayabilirsiniz
            }

            public void CheckPostApplies(object state)
            {
            //lock (_lockObject)
            //{
            //    using (var scope = _serviceProvider.CreateScope())
            //    {
            //        var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>(); // DbContext sınıfınızı kullanmanız gerekiyor

            //        var currentTime = DateTime.Now;

            //        // Post'un end time'ı geçmiş ve onay bekleyen başvuruları al
            //        var postsToCheck = dbContext.Posts.Include(p => p.Applies)
            //            .Where(p => p.EndTime < currentTime && p.Applies.Any(a => a.IsAproved == false))
            //            .ToList();

            //        foreach (var post in postsToCheck)
            //        {
            //        // Posta başvuru yapanları al
            //        if (post.Applies != null)
            //        {
            //            var appliesToApprove = post.Applies.Where(a => a.IsAproved == false).ToList();

            //            foreach (var apply in appliesToApprove)
            //            {
            //                // Başvuruyu onayla
            //                apply.IsAproved = true;
            //                apply.ApplyTime = currentTime;

            //                // Burada gerekli başka işlemleri yapabilirsiniz
            //            }
            //        }
            //        else
            //        {
            //            // Eğer post.Applies null ise, başka bir işlem yapabilir veya hata işleme stratejisi uygulayabilirsiniz.
            //        }

            //        }

            //        // DbContext'teki değişiklikleri kaydet
            //        dbContext.SaveChanges();
            //    }
            //}

            lock (_lockObject)
            {
                using ( var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

                    var currentTime = DateTime.Now;

                    // Post'un end time'ı geçmiş ve onay bekleyen başvuruları al
                    var postsToCheck = dbContext.Posts
                        .Include(p => p.Applies)
                        .Where(p => p.EndTime < currentTime && p.Applies.Any(a => a.IsAproved == false))
                        .ToList();

                    foreach (var post in postsToCheck)
                    {
                        if (post.Applies != null)
                        {
                            // Başvuruları salary özelliğine göre sırala
                            var appliesToApprove = dbContext.Apply
                               .Include(a => a.User)
                               .AsEnumerable()
                               .Where(a => post.Applies.Any(pa => pa.Id == a.Id) && a.IsAproved == false)
                               .OrderBy(a => a.UserId.HasValue && a.User != null ? a.User.Salary ?? 0 : 0)
                               .ToList();

                            // Belirli sayıdaki başvuruyu seç
                            var numberOfApplicantsToApprove = Math.Min(post.Count ?? 0, appliesToApprove.Count);
                            var selectedApplicantsToApprove = appliesToApprove.Take(numberOfApplicantsToApprove);

                            foreach (var apply in selectedApplicantsToApprove)
                            {
                                // Başvuruyu onayla
                                apply.IsAproved = true;
                                apply.ApplyTime = currentTime;
                                post.Count--;

                                if (post.Count == -1)
                                {
                                    apply.IsAproved = false;
                                }

                            }
                        }
                        else
                        {
                            // Eğer post.Applies null ise, başka bir işlem yapabilir veya hata işleme stratejisi uygulayabilirsiniz.
                        }
                    }

                    // DbContext'teki değişiklikleri kaydet
                    dbContext.SaveChanges();
                }
            }

        }
    }
}
