namespace TheaterReservation.Domain.DomainServices;

using TheaterReservation.Domain.ValueObjects;

public interface ISeatAllocator
{
    SeatAllocationResult AllocateSeats(SeatAllocationRequest request);
}

public class SeatAllocationRequest
{
    public required int SeatsRequested { get; init; }
    public required SeatCategory CategoryRequested { get; init; }
    public required SeatAvailability[] AvailableSeats { get; init; }
}

public class SeatAvailability
{
    public required string SeatId { get; init; }
    public required SeatStatus Status { get; init; }
    public required SeatCategory Category { get; init; }
}

public class SeatAllocationResult
{
    public required string[] AllocatedSeatIds { get; init; }
    public required bool IsSuccessful { get; init; }
    public required string Reason { get; init; }
}

public class ContiguousSeatAllocator : ISeatAllocator
{
    public SeatAllocationResult AllocateSeats(SeatAllocationRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.SeatsRequested <= 0)
            return new SeatAllocationResult
            {
                AllocatedSeatIds = Array.Empty<string>(),
                IsSuccessful = false,
                Reason = "Must request at least one seat."
            };

        var availableInCategory = request.AvailableSeats
            .Where(s => s.Category == request.CategoryRequested && s.Status.IsAvailable)
            .ToArray();

        if (availableInCategory.Length < request.SeatsRequested)
            return new SeatAllocationResult
            {
                AllocatedSeatIds = Array.Empty<string>(),
                IsSuccessful = false,
                Reason = $"Not enough available seats in category {request.CategoryRequested}. " +
                         $"Requested: {request.SeatsRequested}, Available: {availableInCategory.Length}"
            };

        var contiguousSeats = FindContiguousSeats(availableInCategory, request.SeatsRequested);

        if (contiguousSeats.Length == 0)
            return new SeatAllocationResult
            {
                AllocatedSeatIds = Array.Empty<string>(),
                IsSuccessful = false,
                Reason = $"Cannot find {request.SeatsRequested} contiguous seats in category {request.CategoryRequested}."
            };

        return new SeatAllocationResult
        {
            AllocatedSeatIds = contiguousSeats.Select(s => s.SeatId).ToArray(),
            IsSuccessful = true,
            Reason = "Seats allocated successfully."
        };
    }

    private static SeatAvailability[] FindContiguousSeats(SeatAvailability[] availableSeats, int seatsRequested)
    {
        for (int i = 0; i <= availableSeats.Length - seatsRequested; i++)
        {
            var contiguousBlock = availableSeats.Skip(i).Take(seatsRequested).ToArray();
            if (contiguousBlock.Length == seatsRequested)
            {
                return contiguousBlock;
            }
        }

        return Array.Empty<SeatAvailability>();
    }
}
