namespace TheaterReservation.Domain.ValueObjects;

public sealed class Price : IEquatable<Price>, IComparable<Price>
{
    private readonly decimal _amount;

    public Price(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(amount));

        _amount = decimal.Round(amount, 2, MidpointRounding.ToEven);
    }

    public decimal Amount => _amount;

    public Price Add(Price other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return new Price(_amount + other._amount);
    }

    public Price Multiply(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("Multiplication factor cannot be negative.", nameof(factor));

        return new Price(_amount * factor);
    }

    public Price ApplyDiscount(decimal discountPercentage)
    {
        if (discountPercentage < 0 || discountPercentage > 1)
            throw new ArgumentException("Discount must be between 0 and 1.", nameof(discountPercentage));

        return new Price(_amount * (1 - discountPercentage));
    }

    public override string ToString() => $"{_amount}€";
    public override bool Equals(object? obj) => Equals(obj as Price);
    public bool Equals(Price? other) => other != null && _amount == other._amount;
    public int CompareTo(Price? other) => other == null ? 1 : _amount.CompareTo(other._amount);
    public override int GetHashCode() => _amount.GetHashCode();

    public static bool operator ==(Price? left, Price? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Price? left, Price? right)
    {
        return !Equals(left, right);
    }

    public static bool operator <(Price? left, Price? right)
    {
        return left != null && right != null && left.CompareTo(right) < 0;
    }

    public static bool operator <=(Price? left, Price? right)
    {
        return left != null && right != null && left.CompareTo(right) <= 0;
    }

    public static bool operator >(Price? left, Price? right)
    {
        return left != null && right != null && left.CompareTo(right) > 0;
    }

    public static bool operator >=(Price? left, Price? right)
    {
        return left != null && right != null && left.CompareTo(right) >= 0;
    }
}
