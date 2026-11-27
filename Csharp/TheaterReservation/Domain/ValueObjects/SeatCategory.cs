namespace TheaterReservation.Domain.ValueObjects;

public sealed class SeatCategory : IEquatable<SeatCategory>
{
    private readonly string _value;

    public static readonly SeatCategory Standard = new("STANDARD");
    public static readonly SeatCategory Premium = new("PREMIUM");

    private SeatCategory(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public static SeatCategory From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Seat category cannot be empty.", nameof(value));

        return value.ToUpper() switch
        {
            "STANDARD" => Standard,
            "PREMIUM" => Premium,
            _ => throw new ArgumentException($"Unknown seat category: {value}", nameof(value))
        };
    }

    public bool IsStandard => this == Standard;
    public bool IsPremium => this == Premium;

    public decimal GetPricingRatio()
    {
        return this == Premium ? 1.5m : 1.0m;
    }

    public override string ToString() => _value;
    public override bool Equals(object? obj) => Equals(obj as SeatCategory);
    public bool Equals(SeatCategory? other) => other != null && _value == other._value;
    public override int GetHashCode() => _value.GetHashCode();

    public static bool operator ==(SeatCategory? left, SeatCategory? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(SeatCategory? left, SeatCategory? right)
    {
        return !Equals(left, right);
    }
}
