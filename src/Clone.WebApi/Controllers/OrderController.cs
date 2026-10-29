using Clone.Models.Dtos;
using Clone.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clone.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private const string HUB_MODE_SUBSCRIBE = "subscribe";

        private readonly IConfiguration _configuration;
        private readonly OrderPublisher _orderPublisher;

        public OrderController(IConfiguration configuration, OrderPublisher orderPublisher)
        {
            _configuration = configuration;
            _orderPublisher = orderPublisher;
        }

        [HttpGet]
        public IActionResult Verify(
            [FromQuery(Name = "hub.mode")] string? mode,
            [FromQuery(Name = "hub.challenge")] string? challenge,
            [FromQuery(Name = "hub.verify_token")] string? verifyToken)
        {
            var expectedToken = _configuration["WhatsApp:VerifyToken"];

            if (string.IsNullOrWhiteSpace(expectedToken) || mode != HUB_MODE_SUBSCRIBE || verifyToken != expectedToken)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            return Content(challenge ?? string.Empty, "text/plain");
        }

        [HttpPost]
        public async Task<IActionResult> Receive(
            [FromQuery(Name = "hub.verify_token")] string? verifyToken,
            [FromBody] WhatsAppWebhookDto payload)
        {
            var expectedToken = _configuration["WhatsApp:VerifyToken"];

            if (string.IsNullOrWhiteSpace(expectedToken) || verifyToken != expectedToken)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            if (payload is null)
            {
                return BadRequest();
            }

            Console.WriteLine(payload);
            await _orderPublisher.OrderCreated(payload);
            return Ok();
        }
    }
}
