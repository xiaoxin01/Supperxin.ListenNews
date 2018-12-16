using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supperxin.ListenNews.Data;
using Supperxin.ListenNews.Models;

namespace Supperxin.ListenNews.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemAudiosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ItemAudiosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // // GET: api/Items
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Item>>> GetItem()
        // {
        //     return await _context.Item.OrderByDescending(i => i.Id).Take(100).ToListAsync();
        // }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var item = await _context.Item.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            var sChar = System.IO.Path.DirectorySeparatorChar;
            var audio = $"wwwroot{sChar}audios{sChar}{item.Source}{sChar}{item.Id}.mp3";
            if (System.IO.File.Exists(audio))
            {
                return File(System.IO.File.ReadAllBytes(audio), "application/octet-stream", $"{item.Id}.mp3");
            }

            var audioString = $"{item.Title}。{item.Content}";


            // 设置APPID/AK/SK
            var APP_ID = "15174756";
            var API_KEY = "ZixGBp4NKBG1vq5Azec2LbEB";
            var SECRET_KEY = "FS0wCOyvvIzrAc11Or8uPVzVL2w6zHUe";

            var _ttsClient = new Baidu.Aip.Speech.Tts(API_KEY, SECRET_KEY);
            _ttsClient.Timeout = 60000;  // 修改超时时间

            // 可选参数
            var option = new Dictionary<string, object>()
            {
                {"spd", 5}, // 语速
                {"vol", 7}, // 音量
                {"per", 4}  // 发音人，4：情感度丫丫童声
            };
            var result = _ttsClient.Synthesis(audioString, option);

            if (result.ErrorCode == 0)  // 或 result.Success
            {
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(audio)))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(audio));
                }
                System.IO.File.WriteAllBytes(audio, result.Data);
                return File(result.Data, "application/octet-stream", $"{item.Id}.mp3");
            }

            return NotFound();
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
