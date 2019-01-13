using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Supperxin.ListenNews.Data;
using Supperxin.ListenNews.Models;
using Supperxin.ListenNews.Services;

namespace Supperxin.ListenNews.Controllers
{
    public class HomeController : Controller
    {
        private readonly ItemAudiosController _itemAudioController;

        public HomeController(ApplicationDbContext context, IAudioService audioService)
        {
            _itemAudioController = new ItemAudiosController(context, audioService);
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Privacy([FromQuery]int? index, [FromQuery]int? count)
        {
            var totalUnlisten = await _itemAudioController.GetTotalUnlisten();
            var newAudios = await _itemAudioController.GetItem(index ?? int.MaxValue, count ?? 10);
            var audios = new List<Audio>();
            newAudios.Value.ToList().ForEach(a =>
            {
                audios.Add(new Audio()
                {
                    id = a.Id,
                    name = a.Title,
                    artist = a.Source,
                    url = $"/api/ItemAudios/{a.Id}",
                    created = a.Created
                });
            });

            var viewModel = new ListenViewModel();
            viewModel.Audios = audios;
            viewModel.TotalUnlisten = totalUnlisten;

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
