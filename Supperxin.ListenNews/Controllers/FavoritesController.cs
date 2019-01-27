using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Supperxin.ListenNews.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Supperxin.ListenNews.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly int PageSize = 500;
        public FavoritesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            // query items with user's favorite status
            var query = from item in _context.Item
                        join favorite in (from f in _context.Favorite where f.UserId == user.Id && !f.Deleted select f)
                        on item.Id equals favorite.ItemId
                        select new { Item = item, Favorite = favorite };

            var items = (await query.ToListAsync())
                .Select(x => { x.Item.FavoriteId = x.Favorite.Id; return x.Item; })
                .OrderByDescending(i => i.Created)
                .Take(PageSize).ToArray();

            return View(items);
        }
    }
}
