namespace TheaterReservation.Domain.DomainServices;

using TheaterReservation.Domain.ValueObjects;

public interface IPriceCalculator
{
    Price CalculatePrice(PriceCalculationRequest request);
}

public class PriceCalculationRequest
{
    public required Price BasePrice { get; init; }
    public required SeatCategory[] SeatCategories { get; init; }
    public required IDiscountPolicy DiscountPolicy { get; init; }
    public required DiscountContext DiscountContext { get; init; }
}

public class PriceCalculator : IPriceCalculator
{
    public Price CalculatePrice(PriceCalculationRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.SeatCategories == null || request.SeatCategories.Length == 0)
            throw new ArgumentException("At least one seat category is required.", nameof(request));

        // Calculate raw price (base price * sum of category ratios)
        var totalCategoryRatio = request.SeatCategories
            .Sum(category => category.GetPricingRatio());

        var rawPrice = request.BasePrice.Multiply(totalCategoryRatio);

        // Apply discount
        var discountRatio = request.DiscountPolicy.CalculateDiscountRatio(request.DiscountContext);
        var finalPrice = rawPrice.ApplyDiscount(discountRatio);

        return finalPrice;
    }
}
