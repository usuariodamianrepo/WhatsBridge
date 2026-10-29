using Clone.Models.Dtos;
using Clone.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Clone.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private const string HUB_MODE_SUBSCRIBE = "subscribe";

        private readonly IConfiguration _configuration;
        private readonly OrderPublisher _orderPublisher;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IConfiguration configuration, OrderPublisher orderPublisher, ILogger<OrdersController> logger)
        {
            _configuration = configuration;
            _orderPublisher = orderPublisher;
            _logger = logger;
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
        public async Task<IActionResult> Receive([FromHeader(Name = "X-Hub-Signature-256")] string? signature)
        {
            using var buffer = new MemoryStream();
            await Request.Body.CopyToAsync(buffer);
            var body = buffer.ToArray();

            if (body.Length == 0)
            {
                return BadRequest();
            }

            var appSecret = _configuration["WhatsApp:AppSecret"]; //SECRETKEY
            if (string.IsNullOrWhiteSpace(appSecret))
            {
                _logger.LogError("WhatsApp:AppSecret not configured; webhook rejected.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            if (!WhatsAppSignatureValidator.IsValid(body, signature, appSecret))
            {
                _logger.LogWarning("X-Hub-Signature-256 signature invalid; webhook rejected.");
                return Unauthorized();
            }

            var payload = JsonSerializer.Deserialize<WhatsAppWebhookDto>(body, JsonSerializerOptions.Web);
            if (payload is null)
            {
                return BadRequest();
            }

            _logger.LogInformation("Webhook received: {Object}", payload.Object);
            await _orderPublisher.OrderCreated(payload);
            return Ok();
        }
    }
}
