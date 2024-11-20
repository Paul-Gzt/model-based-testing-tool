using Basket.API;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _repository;
    private readonly IIdentityService _identityService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<BasketController> _logger;

    public BasketController(
        ILogger<BasketController> logger,
        IBasketRepository repository,
        IIdentityService identityService,
        IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _identityService = identityService;
        _eventBus = eventBus;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
    {
        if (Mutant.IsActive(1))
        {
            return BadRequest();
        }
        if (Mutant.IsActive(2))
        {
            return Ok();
        }
        
        var basket = await _repository.GetBasketAsync(id);

        return Ok(basket ?? new CustomerBasket(id));
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody] CustomerBasket value)
    {
        if (Mutant.IsActive(3))
        {
            return BadRequest();
        }
        if (Mutant.IsActive(4))
        {
            return Ok();
        }
        return Ok(await _repository.UpdateBasketAsync(value));
    }

    [Route("checkout")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout, [FromHeader(Name = "x-requestid")] string requestId)
    {
        var userId = _identityService.GetUserIdentity();

        if (Mutant.IsActive(5))
        {
            basketCheckout.RequestId = !(Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty) ? guid : basketCheckout.RequestId;
        }
        if (Mutant.IsActive(6))
        {
            basketCheckout.RequestId = (Guid.TryParse(requestId, out Guid guid) || guid != Guid.Empty) ? guid : basketCheckout.RequestId;
        }
        if (Mutant.IsActive(7))
        {
            basketCheckout.RequestId = (Guid.TryParse(requestId, out Guid guid) && guid == Guid.Empty) ? guid : basketCheckout.RequestId;
        }
        else
        {
            basketCheckout.RequestId = (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty) ? guid : basketCheckout.RequestId;
        }
        
        var basket = await _repository.GetBasketAsync(userId);

        if (Mutant.IsActive(8))
        {
            if (basket != null)
            {
                return BadRequest();
            }
        }
        
        if (basket == null) 
        {
            return BadRequest();
        }

        var userName = this.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Name).Value;

        if (Mutant.IsActive(9))
        {
            userName = this.HttpContext.User.FindFirst(x => x.Type != ClaimTypes.Name).Value;
        }

        var eventMessage = new UserCheckoutAcceptedIntegrationEvent(userId, userName, basketCheckout.City, basketCheckout.Street,
            basketCheckout.State, basketCheckout.Country, basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName,
            basketCheckout.CardExpiration, basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer, basketCheckout.RequestId, basket);

        // Once basket is checkout, sends an integration event to
        // ordering.api to convert basket to order and proceeds with
        // order creation process
        try
        {
            if (Mutant.IsActive(10))
            {
                return Accepted();
            }
            _eventBus.Publish(eventMessage);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName}", eventMessage.Id, Program.AppName);

            if (Mutant.IsActive(11))
            {
                return Accepted();
            }
            
            throw;
        }

        return Accepted();
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task DeleteBasketByIdAsync(string id)
    {
        if (Mutant.IsActive(12))
        {
            return;
        }
        await _repository.DeleteBasketAsync(id);
    }
}
