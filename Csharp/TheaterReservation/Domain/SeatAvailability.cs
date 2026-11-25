using TheaterReservation.Application;

namespace TheaterReservation.Domain;

public sealed class SeatAvailability : ISeatAvailability
{
    private sealed class SeatState
    {
        public SeatId Id { get; }
        public SeatCategory Category { get; }
        public SeatStatus Status { get; set; }
        public SeatState(SeatId id, SeatCategory category, SeatStatus status) => (Id, Category, Status) = (id, category, status);
    }

    private readonly List<List<SeatState>> rows;
    public int TotalSeats => rows.Sum(row => row.Count);
    public int RemainingSeats => rows.Sum(row => row.Count(seat => seat.Status == SeatStatus.Free));

    public SeatAvailability(IEnumerable<IEnumerable<(SeatId Id, SeatCategory Category, SeatStatus Status)>> rows)
        => this.rows = rows.Select(row => row.Select(seat => new SeatState(seat.Id, seat.Category, seat.Status)).ToList()).ToList();

    public IReadOnlyList<SeatSelection> ReserveContiguous(int count, SeatCategory category)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        foreach (var row in rows)
        {
            var streak = new List<SeatState>();
            foreach (var seat in row)
            {
                if (seat.Status == SeatStatus.Free && seat.Category == category)
                {
                    streak.Add(seat);
                    if (streak.Count == count)
                    {
                        foreach (var selected in streak) selected.Status = SeatStatus.BookingPending;
                        return streak.Select(seat => new SeatSelection(seat.Id, seat.Category)).ToList();
                    }
                }
                else
                {
                    streak.Clear();
                }
            }
        }
        return Array.Empty<SeatSelection>();
    }

    public void Release(IReadOnlyCollection<SeatId> seats)
    {
        var ids = seats.Select(seat => seat.Value).ToHashSet(StringComparer.Ordinal);
        foreach (var seat in rows.SelectMany(row => row).Where(seat => ids.Contains(seat.Id.Value)))
            seat.Status = SeatStatus.Free;
    }
}
