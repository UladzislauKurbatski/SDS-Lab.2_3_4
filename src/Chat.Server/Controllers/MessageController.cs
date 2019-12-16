using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Chat.Domain.Interfaces;
using Chat.Server.Extensions;
using Chat.Domain.Entities;

namespace Chat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        public const string FILE_MEDIA_TYPE = "application/vnd.ms-excel";


        private readonly IChatServerService<byte[], byte[]> _fileChatServerService;
        private readonly IChatServerService<Message, IEnumerable<Message>> _dbChatServerService;

        public MessageController(
            IChatServerService<byte[], byte[]> fileChatServerService,
            IChatServerService<Message, IEnumerable<Message>> dbChatServerService)
        {
            _fileChatServerService = fileChatServerService;
            _dbChatServerService = dbChatServerService;
        }

        [HttpPost("file")]
        public IActionResult Post()
        {
            var formFile = HttpContext.Request.Form.Files.FirstOrDefault();
            if (formFile == null)
            {
                return BadRequest();
            }

            var content = _fileChatServerService.Process(formFile.GetFileBuffer());

            return File(content, FILE_MEDIA_TYPE);
        }

        [HttpPost("database")]
        public IActionResult Post(Message message)
        {
            if (message == null)
            {
                return BadRequest();
            }

            var response = _dbChatServerService.Process(message);
            return Ok(response);
        }

        [HttpGet("give")]
        public IActionResult Get()
        {
            return Ok("Hello world!");
        }
    }
}