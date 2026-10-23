using Clone.Models.Dtos;
using Clone.Utility;
using Clone.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clone.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private const string X_API_KEY = "3416117792";

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { DateTime.Now.ToDateHHmm(), Guid.NewGuid().ToString() };
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromHeader(Name = "x-api-key")] string verify_token, [FromBody] WhatsAppWebhookDto order, OrderPublisher orderPublisher)
        {
            if (verify_token != X_API_KEY)
            {
                return Unauthorized();
            }

            Console.WriteLine(order);
            await orderPublisher.OrderCreated(order);
            return Ok(order);
        }
    }
}
