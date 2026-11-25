using TheaterReservation.Application;
using TheaterReservation.Dao;
using TheaterReservation.Domain;

namespace TheaterReservation.Infrastructure;

public sealed class LegacyPerformancePriceProvider : IPerformancePriceProvider
{
    private readonly PerformancePriceDao dao = new();
    public Money GetBasePrice(PerformanceId performanceId) => new(dao.fetchPerformancePrice(performanceId.Value));
}

public sealed class LegacyVoucherPolicy : IVoucherPolicy
{
    public decimal DiscountFor(DateTime date) => VoucherProgramDao.fetchVoucherProgram(date);
}

public sealed class LegacyCustomerSubscriptionReader : ICustomerSubscriptionReader
{
    private readonly CustomerSubscriptionDao dao = new();
    public bool IsSubscribed(CustomerId customerId) => dao.fetchCustomerSubscription(customerId.Value);
}
