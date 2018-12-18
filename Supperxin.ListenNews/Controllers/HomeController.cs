using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Supperxin.ListenNews.Data;
using Supperxin.ListenNews.Models;

namespace Supperxin.ListenNews.Controllers
{
    public class HomeController : Controller
    {
        private readonly ItemAudiosController _itemAudioController;

        public HomeController(ApplicationDbContext context)
        {
            _itemAudioController = new ItemAudiosController(context);
        }
        public IActionResult Index()
        {
            return View();
        }

        public class Audio
        {
            public int id { get; set; }
            public string name { get; set; }
            public string artist { get; set; }
            public string url { get; set; }
        }
        public async Task<IActionResult> Privacy([FromQuery]int? index, [FromQuery]int? count)
        {
            var newAudios = await _itemAudioController.GetItem(index ?? 100, count ?? 10);
            var audios = new List<Audio>();
            newAudios.Value.ToList().ForEach(a =>
            {
                audios.Add(new Audio()
                {
                    id = a.Id,
                    name = a.Title,
                    artist = a.Source,
                    url = $"/api/ItemAudios/{a.Id}"
                });
            });

            return View(audios);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
