namespace TheaterReservation.Domain.DomainServices;

using TheaterReservation.Domain.ValueObjects;

public interface IVipReservationPolicy
{
    VipReservationCheckResult CheckVipSeatReservation(VipReservationContext context);
}

public class VipReservationContext
{
    public required PerformanceNature PerformanceNature { get; init; }
    public required int TotalSeatsInTheater { get; init; }
    public required int RemainingSeatsInTheater { get; init; }
}

public class VipReservationCheckResult
{
    public required bool CanProceedWithReservation { get; init; }
    public required string Reason { get; init; }
}

public class PerformanceNatureBasedVipPolicy : IVipReservationPolicy
{
    public VipReservationCheckResult CheckVipSeatReservation(VipReservationContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        return context.PerformanceNature switch
        {
            PerformanceNature p when p.IsPremiere => CheckPremiereVipReserve(context),
            PerformanceNature p when p.IsPreview => CheckPreviewVipReserve(context),
            _ => new VipReservationCheckResult
            {
                CanProceedWithReservation = true,
                Reason = "No VIP seat restrictions for this performance nature."
            }
        };
    }

    private static VipReservationCheckResult CheckPremiereVipReserve(VipReservationContext context)
    {
        const decimal vipThreshold = 0.5m; // Keep 50% for VIP
        var availablePercentage = (decimal)context.RemainingSeatsInTheater / context.TotalSeatsInTheater;

        if (availablePercentage < vipThreshold)
        {
            return new VipReservationCheckResult
            {
                CanProceedWithReservation = false,
                Reason = $"Not enough VIP seats available for Premiere. " +
                        $"Required: {vipThreshold * 100}%, Available: {availablePercentage * 100}%"
            };
        }

        return new VipReservationCheckResult
        {
            CanProceedWithReservation = true,
            Reason = "Enough VIP seats reserved for Premiere."
        };
    }

    private static VipReservationCheckResult CheckPreviewVipReserve(VipReservationContext context)
    {
        const decimal vipThreshold = 0.1m; // Keep 10% for VIP
        var availablePercentage = (decimal)context.RemainingSeatsInTheater / context.TotalSeatsInTheater;

        if (availablePercentage < vipThreshold)
        {
            return new VipReservationCheckResult
            {
                CanProceedWithReservation = false,
                Reason = $"Not enough VIP seats available for Preview. " +
                        $"Required: {vipThreshold * 100}%, Available: {availablePercentage * 100}%"
            };
        }

        return new VipReservationCheckResult
        {
            CanProceedWithReservation = true,
            Reason = "Enough VIP seats reserved for Preview."
        };
    }
}
