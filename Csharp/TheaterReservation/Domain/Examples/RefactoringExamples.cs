namespace TheaterReservation.Domain.Examples;

using TheaterReservation.Domain.ValueObjects;
using TheaterReservation.Domain.DomainServices;

/// <summary>
/// Exemples d'utilisation des concepts métier refactorisés
/// </summary>
public class RefactoringExamples
{
    public static void ExampleSeatStatus()
    {
        // Avant : strings disparates
        // string seatStatus = "FREE";
        // if (seatStatus.Equals("FREE")) { ... }

        // Après : Value Object fortement typé
        var status = SeatStatus.Free;
        if (status.IsAvailable)
        {
            Console.WriteLine("Siège disponible");
        }

        // Création sécurisée avec validation
        var parsedStatus = SeatStatus.From("BOOKING_PENDING");
        Console.WriteLine($"Statut parsé: {parsedStatus}");
    }

    public static void ExamplePriceCalculation()
    {
        // Avant : calculs dispersés et implicites
        // decimal price = basePrice * 1.5m;
        // decimal discount = price * 0.175m;
        // decimal total = price - discount;

        // Après : Service de Domaine avec Value Objects
        var calculator = new PriceCalculator();

        var basePrice = new Price(100m);
        var seatCategories = new[] { SeatCategory.Standard, SeatCategory.Premium };

        var discountPolicy = new SubscriptionDiscountPolicy();
        var discountContext = new DiscountContext
        {
            PerformanceStartTime = DateTime.Now,
            IsCustomerSubscribed = true,
            PerformanceNature = PerformanceNature.Regular
        };

        var request = new PriceCalculationRequest
        {
            BasePrice = basePrice,
            SeatCategories = seatCategories,
            DiscountPolicy = discountPolicy,
            DiscountContext = discountContext
        };

        var finalPrice = calculator.CalculatePrice(request);
        Console.WriteLine($"Prix final: {finalPrice}");
    }

    public static void ExampleSeatAllocation()
    {
        // Avant : algorithme complexe implicite dans TheaterService
        // Après : Service de Domaine injectable et testable

        var allocator = new ContiguousSeatAllocator();

        var availableSeats = new[]
        {
            new SeatAvailability 
            { 
                SeatId = "A1", 
                Status = SeatStatus.Free, 
                Category = SeatCategory.Standard 
            },
            new SeatAvailability 
            { 
                SeatId = "A2", 
                Status = SeatStatus.Free, 
                Category = SeatCategory.Standard 
            },
            new SeatAvailability 
            { 
                SeatId = "A3", 
                Status = SeatStatus.Free, 
                Category = SeatCategory.Standard 
            }
        };

        var request = new SeatAllocationRequest
        {
            SeatsRequested = 2,
            CategoryRequested = SeatCategory.Standard,
            AvailableSeats = availableSeats
        };

        var result = allocator.AllocateSeats(request);

        if (result.IsSuccessful)
        {
            Console.WriteLine($"Sièges alloués: {string.Join(", ", result.AllocatedSeatIds)}");
        }
        else
        {
            Console.WriteLine($"Erreur: {result.Reason}");
        }
    }

    public static void ExampleVipReservationPolicy()
    {
        // Avant : règles implicites
        // if (performance.performanceNature.Equals("PREMIERE") && remainingSeats < totalSeats * 0.5)

        // Après : Politique métier explicite et testable
        var vipPolicy = new PerformanceNatureBasedVipPolicy();

        var context = new VipReservationContext
        {
            PerformanceNature = PerformanceNature.Premiere,
            TotalSeatsInTheater = 100,
            RemainingSeatsInTheater = 40  // 40% restants (< 50% requis)
        };

        var result = vipPolicy.CheckVipSeatReservation(context);

        if (result.CanProceedWithReservation)
        {
            Console.WriteLine("Réservation acceptée");
        }
        else
        {
            Console.WriteLine($"Réservation rejetée: {result.Reason}");
        }
    }

    public static void ExampleReservationIdAllocation()
    {
        // Avant : singleton avec compteur statique (anti-pattern)
        // Int64 currentId = 123455;
        // currentId++;

        // Après : Service injectable et thread-safe
        var idAllocator = new SequentialReservationIdAllocator(startingId: 123455);

        var id1 = idAllocator.AllocateNewId();
        var id2 = idAllocator.AllocateNewId();

        Console.WriteLine($"ID 1: {id1}");
        Console.WriteLine($"ID 2: {id2}");
        Console.WriteLine($"Sont différents: {id1 != id2}");
    }

    public static void ExampleCompositeDiscount()
    {
        // Après : Politiques composables
        var timeBasedPolicy = new TimeBasedDiscountPolicy(new Dictionary<DateTime, decimal>
        {
            { DateTime.Now.Date, 0.1m }  // 10% de réduction à ce jour
        });

        var subscriptionPolicy = new SubscriptionDiscountPolicy();

        var compositePolicy = new CompositeDiscountPolicy(timeBasedPolicy, subscriptionPolicy);

        var context = new DiscountContext
        {
            PerformanceStartTime = DateTime.Now,
            IsCustomerSubscribed = true,
            PerformanceNature = PerformanceNature.Regular
        };

        var totalDiscount = compositePolicy.CalculateDiscountRatio(context);
        Console.WriteLine($"Réduction totale: {totalDiscount * 100}%");
    }
}
