using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Supperxin.ListenNews.Data;
using Supperxin.ListenNews.Models;

namespace Supperxin.ListenNews.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IMemoryCache _cache;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly string[] _nonePublicSources;
        private readonly int PageSize = 120 * 2;
        private readonly int DateSize = 10;

        public NewsController(ApplicationDbContext context, IMemoryCache cache, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _cache = cache;
            _userManager = userManager;

            _nonePublicSources = configuration.GetSection("NonePublicSources").Get<string[]>();
        }

        // GET: News
        public async Task<IActionResult> Index(DateTime? date)
        {
            var queryDate = date ?? DateTime.Now;
            var dateStart = queryDate.Date;
            var dateEnd = queryDate.Date.AddDays(1);

            var viewModel = new NewsViewModel();
            var user = await _userManager.GetUserAsync(User);
            if (null == user)
            {
                viewModel.News = await _context.Item.Where(i => i.Created >= dateStart && i.Created < dateEnd && !_nonePublicSources.Contains(i.Source)).Take(PageSize).ToArrayAsync();
            }
            else
            {
                // query items with user's favorite status
                var query = from item in _context.Item
                            join favorite in (from f in _context.Favorite where f.UserId == user.Id && !f.Deleted select f)
                            on item.Id equals favorite.ItemId into itemsWithFavorite
                            from itemWithFavorite in itemsWithFavorite.DefaultIfEmpty()
                            where item.Created >= dateStart && item.Created < dateEnd && !_nonePublicSources.Contains(item.Source)
                            select new { Item = item, Favorite = itemWithFavorite };

                viewModel.News = (await query.ToListAsync()).Select(x => { if (null != x.Favorite) x.Item.FavoriteId = x.Favorite.Id; return x.Item; }).Take(PageSize).ToArray();

                if (null != date)
                {
                    var viewHistory = new ViewHistory()
                    {
                        Module = "News",
                        ViewKey = "News_" + date?.ToString("yyyy-MM-dd"),
                        UserId = user.Id
                    };
                    _context.ViewHistory.Add(viewHistory);
                    await _context.SaveChangesAsync();
                }
            }
            viewModel.Dates = await _cache.GetOrCreateAsync("NewsDates", async entry =>
             {
                 entry.SlidingExpiration = TimeSpan.FromMinutes(5);

                 Console.WriteLine("create cache for news dates.");

                 var dates = await (from dc in
                                        (from item in _context.Item
                                         group item by item.Created.Date into g
                                         orderby g.Key descending
                                         select new
                                         {
                                             DateString = g.Key.ToString("yyyy-MM-dd"),
                                             ViewKey = "News_" + g.Key.ToString("yyyy-MM-dd"),
                                             Count = g.Count()
                                         }
                                        )
                                    join vh in
                                    (from vh in _context.ViewHistory
                                     where user != null && user.Id == vh.UserId
                                     select vh)
                                    on dc.ViewKey equals vh.ViewKey into g
                                    from fdc in g.DefaultIfEmpty()
                                    select new DateCount()
                                    {

                                        DateString = dc.DateString,
                                        Visited = fdc.Module != null,
                                        Count = dc.Count
                                    }
                                    ).Take(DateSize).ToArrayAsync();

                 return dates;
             });
            //.GroupBy(i => i.Created.Date).OrderByDescending(i => i.Key).Take(10).SelectMany().ToArrayAsync();


            return View(viewModel);
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.Id == id);
        }
    }
}
