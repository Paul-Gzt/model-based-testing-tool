using Basket.API;

namespace Microsoft.eShopOnContainers.Services.Basket.API.IntegrationEvents.EventHandling;

public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
{
    private readonly ILogger<ProductPriceChangedIntegrationEventHandler> _logger;
    private readonly IBasketRepository _repository;

    public ProductPriceChangedIntegrationEventHandler(
        ILogger<ProductPriceChangedIntegrationEventHandler> logger,
        IBasketRepository repository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task Handle(ProductPriceChangedIntegrationEvent @event)
    {
        using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            var userIds = _repository.GetUsers();

            foreach (var id in userIds)
            {
                if (Mutant.IsActive(14))
                {
                    break;
                }
                var basket = await _repository.GetBasketAsync(id);

                if (Mutant.IsActive(15))
                {
                    continue;
                }
                await UpdatePriceInBasketItems(@event.ProductId, @event.NewPrice, @event.OldPrice, basket);
            }
        }
    }

    private async Task UpdatePriceInBasketItems(int productId, decimal newPrice, decimal oldPrice, CustomerBasket basket)
    {
        var itemsToUpdate = basket?.Items?.Where(x => x.ProductId == productId).ToList();

        if (Mutant.IsActive(16))
        {
            itemsToUpdate = basket?.Items?.Where(x => x.ProductId != productId).ToList();
        }

        if (Mutant.IsActive(17))
        {
            if (itemsToUpdate == null)
            {
                _logger.LogInformation("----- ProductPriceChangedIntegrationEventHandler - Updating items in basket for user: {BuyerId} ({@Items})", basket.BuyerId, itemsToUpdate);

                foreach (var item in itemsToUpdate)
                {
                    if (item.UnitPrice == oldPrice)
                    {
                        var originalPrice = item.UnitPrice;
                        item.UnitPrice = newPrice;
                        item.OldUnitPrice = originalPrice;
                    }
                }
                await _repository.UpdateBasketAsync(basket);
                return;
            }

            return;
        }
        

        if (itemsToUpdate != null)
        {
            _logger.LogInformation("----- ProductPriceChangedIntegrationEventHandler - Updating items in basket for user: {BuyerId} ({@Items})", basket.BuyerId, itemsToUpdate);

            foreach (var item in itemsToUpdate)
            {
                if (Mutant.IsActive(18))
                {
                    break;
                }

                if (Mutant.IsActive(19))
                {
                    if (item.UnitPrice != oldPrice)
                    {
                        var originalPrice = item.UnitPrice;
                        item.UnitPrice = newPrice;
                        item.OldUnitPrice = originalPrice;
                    }

                    continue;
                }
                if (item.UnitPrice == oldPrice)
                {
                    var originalPrice = item.UnitPrice;
                    item.UnitPrice = newPrice;
                    item.OldUnitPrice = originalPrice;
                }
            }

            if (Mutant.IsActive(20))
            {
                return;
            }
            
            await _repository.UpdateBasketAsync(basket);
        }
    }
}
