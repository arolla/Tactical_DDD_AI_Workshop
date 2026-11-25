using TheaterReservation.Application;
using TheaterReservation.Domain;

namespace TheaterReservation.Infrastructure;

public sealed class LegacyPricePolicy : IPricePolicy
{
    private readonly IPerformancePriceProvider prices;
    private readonly ICustomerSubscriptionReader subscriptions;
    private readonly IVoucherPolicy vouchers;

    public LegacyPricePolicy(
        IPerformancePriceProvider? prices = null,
        ICustomerSubscriptionReader? subscriptions = null,
        IVoucherPolicy? vouchers = null)
    {
        this.prices = prices ?? new LegacyPerformancePriceProvider();
        this.subscriptions = subscriptions ?? new LegacyCustomerSubscriptionReader();
        this.vouchers = vouchers ?? new LegacyVoucherPolicy();
    }

    public Money PriceFor(PerformanceSnapshot performance, IReadOnlyList<SeatSelection> seats, CustomerId customerId)
    {
        var basePrice = prices.GetBasePrice(performance.Id).Amount;
        var initial = seats.Sum(seat => basePrice * (seat.Category == SeatCategory.Premium ? 1.5m : 1m));
        var subscribedAmount = subscriptions.IsSubscribed(customerId) ? initial * (1m - 0.175m) : initial;
        return new Money(decimal.Round(subscribedAmount * (1m - vouchers.DiscountFor(performance.StartTime)), 2, MidpointRounding.ToEven));
    }
}
