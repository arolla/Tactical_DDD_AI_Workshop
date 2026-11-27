namespace TheaterReservation.Domain.DomainServices;

using TheaterReservation.Domain.ValueObjects;

public interface IDiscountPolicy
{
    decimal CalculateDiscountRatio(DiscountContext context);
}

public class DiscountContext
{
    public required DateTime PerformanceStartTime { get; init; }
    public required bool IsCustomerSubscribed { get; init; }
    public required PerformanceNature PerformanceNature { get; init; }
}

public class TimeBasedDiscountPolicy : IDiscountPolicy
{
    private readonly Dictionary<DateTime, decimal> _discountsByDateTime;

    public TimeBasedDiscountPolicy(Dictionary<DateTime, decimal> discountsByDateTime)
    {
        _discountsByDateTime = discountsByDateTime ?? new Dictionary<DateTime, decimal>();
    }

    public decimal CalculateDiscountRatio(DiscountContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        return _discountsByDateTime.TryGetValue(context.PerformanceStartTime, out var discount)
            ? discount
            : 0m; // No discount if date/time not in map
    }
}

public class SubscriptionDiscountPolicy : IDiscountPolicy
{
    private const decimal SubscriptionDiscountPercentage = 0.175m;

    public decimal CalculateDiscountRatio(DiscountContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        return context.IsCustomerSubscribed 
            ? SubscriptionDiscountPercentage 
            : 0m;
    }
}

public class CompositeDiscountPolicy : IDiscountPolicy
{
    private readonly IDiscountPolicy[] _policies;

    public CompositeDiscountPolicy(params IDiscountPolicy[] policies)
    {
        _policies = policies ?? throw new ArgumentNullException(nameof(policies));
    }

    public decimal CalculateDiscountRatio(DiscountContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        // Combine discounts (can be adjusted based on business rules)
        return _policies
            .Select(p => p.CalculateDiscountRatio(context))
            .Aggregate(0m, (acc, ratio) => acc + ratio);
    }
}
