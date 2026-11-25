namespace TheaterReservation.Domain;

public readonly record struct PerformanceId(long Value)
{
    public static PerformanceId Create(long value) => value > 0 ? new(value) : throw new ArgumentOutOfRangeException(nameof(value));
    public override string ToString() => Value.ToString();
}

public readonly record struct ReservationId(long Value)
{
    public static ReservationId Create(long value) => value > 0 ? new(value) : throw new ArgumentOutOfRangeException(nameof(value));
    public override string ToString() => Value.ToString();
}

public readonly record struct CustomerId(long Value)
{
    public static CustomerId Create(long value) => value > 0 ? new(value) : throw new ArgumentOutOfRangeException(nameof(value));
}

public readonly record struct SeatId
{
    public string Value { get; }
    private SeatId(string value) => Value = value;
    public static SeatId Create(string value) => !string.IsNullOrWhiteSpace(value) ? new(value) : throw new ArgumentException("Seat id is required.", nameof(value));
    public override string ToString() => Value;
}

public enum SeatStatus { Free, Booked, BookingPending }
public enum SeatCategory { Standard, Premium }
public enum PerformanceNature { Preview, Premiere, Other }
public enum ReservationStatus { Pending, Aborted, Cancelled }

public readonly record struct Money
{
    public decimal Amount { get; }
    public static Money Zero => new(0m);
    public Money(decimal amount) : this()
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Amount = decimal.Round(amount, 2, MidpointRounding.ToEven);
    }

    public Money Add(Money other) => new(Amount + other.Amount);
    public Money ApplyRate(decimal rate) => new(Amount * rate);
    public override string ToString() => $"{Amount:0.00}€";
}

public static class DomainParsing
{
    public static SeatCategory ParseCategory(string value) => value?.ToUpperInvariant() switch
    {
        "STANDARD" => SeatCategory.Standard,
        "PREMIUM" => SeatCategory.Premium,
        _ => throw new ArgumentException($"Unknown seat category: {value}", nameof(value))
    };

    public static PerformanceNature ParseNature(string value) => value?.ToUpperInvariant() switch
    {
        "PREVIEW" => PerformanceNature.Preview,
        "PREMIERE" => PerformanceNature.Premiere,
        _ => PerformanceNature.Other
    };

    public static string ToExternal(this SeatCategory value) => value == SeatCategory.Premium ? "PREMIUM" : "STANDARD";
}
