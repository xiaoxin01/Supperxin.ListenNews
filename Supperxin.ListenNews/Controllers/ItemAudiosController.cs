using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supperxin.ListenNews.Data;
using Supperxin.ListenNews.Models;
using Supperxin.ListenNews.Services;

namespace Supperxin.ListenNews.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemAudiosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAudioService _audioService;
        private readonly object _generateAudioLock = new object();

        public ItemAudiosController(ApplicationDbContext context, IAudioService audioService)
        {
            _context = context;
            _audioService = audioService;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItem(int index, int count, string genericAudio = null)
        {
            if (string.IsNullOrWhiteSpace(genericAudio))
            {
                // select item with no listen history.
                var query = from i in _context.Item
                            join l in _context.ListenHistory on i.Id equals l.ItemId into ItemListenHistory
                            from il in ItemListenHistory.DefaultIfEmpty()
                            where il == null && i.AudioStatus == 1
                            select i;

                return await query.Where(i => i.Id <= index).OrderByDescending(i => i.Id).Take(count).ToListAsync();
            }
            else
            {
                lock (_generateAudioLock)
                {
                    while (true)
                    {
                        var noAudioQuery = from i in _context.Item
                                           where i.AudioStatus == 0
                                           select i;

                        var audioItems = noAudioQuery.Take(10).ToList();
                        if (audioItems.Count == 0)
                        {
                            break;
                        }
                        audioItems.ForEach(item =>
                        {
                            Console.WriteLine($"generate audio {item.Id}");
                            var result = GenerateAudio(item);
                            item.AudioStatus = result.Success ? 1 : -1;
                            item.AudioErrorMessage = result.Success ? null : result.ErrorMsg;
                            // QPS=100
                            System.Threading.Thread.Sleep(10);
                        });

                        _context.UpdateRange(audioItems);
                        _context.SaveChanges();
                    }
                }

                return Ok("Done");
            }
        }

        // GET: api/Items
        [HttpGet]
        public async Task<int> GetTotalUnlisten()
        {
            // select item with no listen history.
            var query = from i in _context.Item
                        join l in _context.ListenHistory on i.Id equals l.ItemId into ItemListenHistory
                        from il in ItemListenHistory.DefaultIfEmpty()
                        where il == null && i.AudioStatus == 1
                        select i;
            return await query.CountAsync();
        }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var item = await _context.Item.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            string audio = GetAudioFileName(item);
            if (System.IO.File.Exists(audio))
            {
                await MakeItemListen(item);
                return File(System.IO.File.ReadAllBytes(audio), "audio/mpeg", $"{item.Id}.mp3", true);
                //return Redirect($"/audios/{item.Source}/{item.Id}.mp3");
            }

            var result = GenerateAudio(item);

            if (result.ErrorCode == 0)
            {
                await MakeItemListen(item);
                return File(result.Data, "audio/mpeg", $"{item.Id}.mp3", true);
            }

            return NotFound();
        }

        private async Task MakeItemListen(Item item)
        {
            // add listen history
            var listenHistory = new ListenHistory() { ItemId = item.Id, UserId = null };
            await _context.ListenHistory.AddAsync(listenHistory);
            await _context.SaveChangesAsync();
        }

        private static string GetAudioFileName(Item item)
        {
            var sChar = System.IO.Path.DirectorySeparatorChar;
            var audio = $"wwwroot{sChar}audios{sChar}{item.Source}{sChar}{item.Id}.mp3";
            return audio;
        }

        private Baidu.Aip.Speech.TtsResponse GenerateAudio(Item item)
        {
            var audio = GetAudioFileName(item);
            var audioString = $"{item.Title}。{item.Content}";
            var result = _audioService.GeneriteAudio(audioString);

            if (result.ErrorCode == 0)  // 或 result.Success
            {
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(audio)))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(audio));
                }
                System.IO.File.WriteAllBytes(audio, result.Data);
            }

            return result;
        }

        // // PUT: api/Items/5
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutItem(int id, Item item)
        // {
        //     if (id != item.Id)
        //     {
        //         return BadRequest();
        //     }

        //     _context.Entry(item).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!ItemExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }

        //     return NoContent();
        // }

        // // POST: api/Items
        // [HttpPost]
        // public async Task<ActionResult<Item>> PostItem(Item item)
        // {
        //     _context.Item.Add(item);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction("GetItem", new { id = item.Id }, item);
        // }

        // // DELETE: api/Items/5
        // [HttpDelete("{id}")]
        // public async Task<ActionResult<Item>> DeleteItem(int id)
        // {
        //     var item = await _context.Item.FindAsync(id);
        //     if (item == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.Item.Remove(item);
        //     await _context.SaveChangesAsync();

        //     return item;
        // }

        // private bool ItemExists(int id)
        // {
        //     return _context.Item.Any(e => e.Id == id);
        // }

        // // POST: api/Items
        // [HttpPost("PostItems")]
        // public async Task<ActionResult<Item>> PostItems(Item[] item)
        // {
        //     _context.Item.AddRange(item);
        //     await _context.SaveChangesAsync();

        //     return Ok("success");
        // }
    }
}
