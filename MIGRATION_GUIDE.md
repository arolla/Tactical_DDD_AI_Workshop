# Migration Guide - Ancien vs Nouveau Code

## 🔄 Mapping Rapide : Comment Remplacer le Vieux Code

### 1️⃣ Statuts de Siège

#### ❌ Ancien Code
```csharp
string seatStatus = "FREE";
if (seatStatus.Equals("FREE")) { /* ... */ }
if (!aSeat.getStatus().Equals("BOOKED") && !aSeat.getStatus().Equals("BOOKING_PENDING")) {
	// ...
}
```

#### ✅ Nouveau Code
```csharp
using TheaterReservation.Domain.ValueObjects;

SeatStatus seatStatus = SeatStatus.Free;
if (seatStatus.IsFree) { /* ... */ }
if (aSeat.Status.IsAvailable) {
	// ...
}
```

---

### 2️⃣ Statuts de Réservation

#### ❌ Ancien Code
```csharp
reservation.setStatus("PENDING");
// plus tard
if (reservation.getStatus().Equals("CANCELLED")) {
	// ...
}
```

#### ✅ Nouveau Code
```csharp
using TheaterReservation.Domain.ValueObjects;

reservation.Status = ReservationStatus.Pending;
// plus tard
if (reservation.Status == ReservationStatus.Cancelled) {
	// ...
}
```

---

### 3️⃣ Performance Nature

#### ❌ Ancien Code
```csharp
if (performance.performanceNature.Equals("PREMIERE")) {
	// apply special rules
}
```

#### ✅ Nouveau Code
```csharp
using TheaterReservation.Domain.ValueObjects;

if (performance.Nature.IsPremiere) {
	// apply special rules
}
```

---

### 4️⃣ Catégories de Sièges et Tarification

#### ❌ Ancien Code
```csharp
string zoneCategory = zone.getCategory();
decimal categoryRatio = zoneCategory.Equals("STANDARD") ? 1 : 1.5m;
initialPrice = initialPrice + (basePrice * categoryRatio);
```

#### ✅ Nouveau Code
```csharp
using TheaterReservation.Domain.ValueObjects;

SeatCategory category = SeatCategory.From(zone.GetCategory());
decimal categoryRatio = category.GetPricingRatio();
initialPrice = initialPrice + (basePrice * categoryRatio);

// ou plus simplement avec Price Value Object :
var price = new Price(basePrice).Multiply(category.GetPricingRatio());
```

---

### 5️⃣ Réservation ID

#### ❌ Ancien Code
```csharp
String res_id = ReservationService.initNewReservation();
reservation.setReservationId(Convert.ToInt64(res_id));
```

#### ✅ Nouveau Code
```csharp
using TheaterReservation.Domain.ValueObjects;
using TheaterReservation.Domain.DomainServices;

IReservationIdAllocator idAllocator = new SequentialReservationIdAllocator();
ReservationId reservationId = idAllocator.AllocateNewId();
reservation.Id = reservationId;
```

---

### 6️⃣ Allocation de Sièges

#### ❌ Ancien Code (150+ lignes implicites)
```csharp
int bookedSeats = 0;
List<String> foundSeats = new List<string>();
Dictionary<String, String> seatsCategory = new Dictionary<string, string>();
String zoneCategory;
int remainingSeats = 0;
int totalSeats = 0;
bool foundAllSeats = false;

for (int i = 0; i < room.getZones().Length; i++) {
	Zone zone = room.getZones()[i];
	zoneCategory = zone.getCategory();
	for (int j = 0; j < zone.getRows().Length; j++) {
		Row row = zone.getRows()[j];
		List<String> seatsForRow = new List<string>();
		int streakOfNotReservedSeats = 0;
		for (int k = 0; k < row.getSeats().Length; k++) {
			totalSeats++;
			Seat aSeat = row.getSeats()[k];
			if (!aSeat.getStatus().Equals("BOOKED") && !aSeat.getStatus().Equals("BOOKING_PENDING")) {
				remainingSeats++;
				if (!reservationCategory.Equals(zoneCategory)) {
					continue;
				}
				if (!foundAllSeats) {
					seatsForRow.Add(aSeat.getSeatId());
					streakOfNotReservedSeats++;
					if (streakOfNotReservedSeats >= reservationCount) {
						foreach (String seat in seatsForRow) {
							foundSeats.Add(seat);
							seatsCategory.Add(seat, zoneCategory);
						}
						foundAllSeats = true;
						remainingSeats -= streakOfNotReservedSeats;
					}
				}
			} else {
				seatsForRow = new List<string>();
				streakOfNotReservedSeats = 0;
			}
		}
		// ...
	}
}
```

#### ✅ Nouveau Code (5 lignes claires)
```csharp
using TheaterReservation.Domain.DomainServices;
using TheaterReservation.Domain.ValueObjects;

ISeatAllocator allocator = new ContiguousSeatAllocator();

var availableSeats = theaterRoom.GetAllAvailableSeats(); // Helper à créer
var request = new SeatAllocationRequest {
	SeatsRequested = reservationCount,
	CategoryRequested = SeatCategory.From(reservationCategory),
	AvailableSeats = availableSeats
};

var result = allocator.AllocateSeats(request);

if (result.IsSuccessful) {
	var allocatedSeatIds = result.AllocatedSeatIds;
	// process allocation
} else {
	throw new SeatAllocationException(result.Reason);
}
```

---

### 7️⃣ Calcul de Prix

#### ❌ Ancien Code
```csharp
decimal myPrice = performancePriceDao.fetchPerformancePrice(performance.id);
decimal initialPrice = Math.Round(zero, 2, MidpointRounding.ToEven);

foreach (String foundSeat in foundSeats) {
	decimal categoryRatio = seatsCategory[foundSeat].Equals("STANDARD") ? one : Convert.ToDecimal("1.5");
	initialPrice = initialPrice + (myPrice * categoryRatio);
}

decimal discountTime = VoucherProgramDao.fetchVoucherProgram(performance.startTime);
bool isSubscribed = customerSubscriptionDao.fetchCustomerSubscription(customerId);

decimal totalBilling = initialPrice;
if (isSubscribed) {
	decimal removePercent = Math.Round(Convert.ToDecimal("0.175"), 3, MidpointRounding.ToEven);
	totalBilling = (one - removePercent) * initialPrice;
}
decimal discountRatio = one - discountTime;
String total = Math.Round(totalBilling * discountRatio, 2, MidpointRounding.ToEven) + "€";
```

#### ✅ Nouveau Code
```csharp
using TheaterReservation.Domain.DomainServices;
using TheaterReservation.Domain.ValueObjects;

IPriceCalculator calculator = new PriceCalculator();

var basePrice = new Price(performancePriceDao.fetchPerformancePrice(performance.id));
var seatCategories = foundSeats.Select(seatId => GetSeatCategory(seatId)).ToArray();

var discountPolicy = new CompositeDiscountPolicy(
	new TimeBasedDiscountPolicy(timeDiscounts),
	new SubscriptionDiscountPolicy()
);

var discountContext = new DiscountContext {
	PerformanceStartTime = performance.startTime,
	IsCustomerSubscribed = isSubscribed,
	PerformanceNature = PerformanceNature.From(performance.performanceNature)
};

var request = new PriceCalculationRequest {
	BasePrice = basePrice,
	SeatCategories = seatCategories,
	DiscountPolicy = discountPolicy,
	DiscountContext = discountContext
};

Price finalPrice = calculator.CalculatePrice(request);
string total = finalPrice.ToString(); // "123.45€"
```

---

### 8️⃣ Vérification des Règles VIP

#### ❌ Ancien Code
```csharp
if (performance.performanceNature.Equals("PREMIERE") && remainingSeats < totalSeats * 0.5) {
	foundSeats = new List<string>();
	Console.WriteLine("Not enough VIP seats available for Premiere");
} else if (performance.performanceNature.Equals("PREVIEW") && remainingSeats < totalSeats * 0.9) {
	foundSeats = new List<string>();
	Console.WriteLine("Not enough VIP seats available for Preview");
}
```

#### ✅ Nouveau Code
```csharp
using TheaterReservation.Domain.DomainServices;
using TheaterReservation.Domain.ValueObjects;

IVipReservationPolicy vipPolicy = new PerformanceNatureBasedVipPolicy();

var vipContext = new VipReservationContext {
	PerformanceNature = PerformanceNature.From(performance.performanceNature),
	TotalSeatsInTheater = totalSeats,
	RemainingSeatsInTheater = remainingSeats
};

var vipResult = vipPolicy.CheckVipSeatReservation(vipContext);

if (!vipResult.CanProceedWithReservation) {
	throw new InsufficientVipSeatsException(vipResult.Reason);
}
```

---

### 9️⃣ Génération d'ID (Remplacement du Singleton)

#### ❌ Ancien Code (Anti-pattern)
```csharp
// Dans ReservationService
public class ReservationService {
	private static Int64 currentId = 123455;

	public static String initNewReservation() {
		currentId++;
		return currentId.ToString();
	}
}

// Usage
String resId = ReservationService.initNewReservation();
```

#### ✅ Nouveau Code (Injectables)
```csharp
using TheaterReservation.Domain.DomainServices;
using TheaterReservation.Domain.ValueObjects;

// En startup / IoC container
var idAllocator = new SequentialReservationIdAllocator(startingId: 123455);

// Dans le service
public class ReservationService {
	private readonly IReservationIdAllocator _idAllocator;

	public ReservationService(IReservationIdAllocator idAllocator) {
		_idAllocator = idAllocator;
	}

	public ReservationId CreateNewReservation() {
		return _idAllocator.AllocateNewId();
	}
}

// Usage
ReservationId resId = service.CreateNewReservation();
```

---

## 🎯 Stratégie de Migration Progressive

### Approche : Non-breaking changes

Les nouveaux concepts coexistent avec l'ancien code :

```csharp
// Ancien data model (pour la compatibilité)
public class Seat {
	public string getStatus() => _status;
	public void setStatus(string status) => _status = status;
	private string _status;
}

// Nouveau (ajouter une propriété)
public class Seat {
	public SeatStatus Status {
		get => SeatStatus.From(_status);
		set => _status = value.ToString();
	}
}

// Ainsi, ancien et nouveau code fonctionnent ensemble
```

### Phases de Migration

1. **Phase 1 (FAIT)** : Créer les Value Objects et Services
2. **Phase 2** : Adapter les Data Objects pour accepter les Value Objects
3. **Phase 3** : Refactoriser TheaterService.reservation() progressivement
4. **Phase 4** : Ajouter des tests sur les nouveaux concepts
5. **Phase 5** : Déprecier les anciens patterns

---

## 🧪 Tests de Validation

Pour chaque nouveau concept, ajoutez des tests :

```csharp
[TestFixture]
public class SeatStatusTests {
	[Test]
	public void From_ValidStatus_CreatesCorrectInstance() {
		var status = SeatStatus.From("FREE");
		Assert.That(status, Is.EqualTo(SeatStatus.Free));
	}

	[Test]
	public void IsAvailable_ForFreeStatus_ReturnsTrue() {
		Assert.That(SeatStatus.Free.IsAvailable, Is.True);
	}
}

[TestFixture]
public class PriceTests {
	[Test]
	public void Multiply_ValidFactor_CalculatesCorrectly() {
		var price = new Price(100m);
		var result = price.Multiply(1.5m);
		Assert.That(result.Amount, Is.EqualTo(150m));
	}

	[Test]
	public void ApplyDiscount_ValidPercentage_CalculatesCorrectly() {
		var price = new Price(100m);
		var result = price.ApplyDiscount(0.1m);
		Assert.That(result.Amount, Is.EqualTo(90m));
	}
}
```

---

## 📋 Checklist de Migration

- [ ] Remplacer strings de statuts par SeatStatus
- [ ] Remplacer strings de statuts par ReservationStatus
- [ ] Remplacer strings de nature par PerformanceNature
- [ ] Remplacer strings de catégorie par SeatCategory
- [ ] Remplacer Int64/String de réservationId par ReservationId
- [ ] Remplacer calculs de prix par Price + PriceCalculator
- [ ] Remplacer algorithme allocation par SeatAllocator
- [ ] Remplacer singleton par IReservationIdAllocator
- [ ] Remplacer logique VIP par IVipReservationPolicy
- [ ] Ajouter tests pour chaque nouveau concept
- [ ] Supprimer ancien code une fois migration complète

---

## 💡 Bonnes Pratiques de Migration

1. **Graduel** : Ne pas refactoriser tout d'un coup
2. **Tests d'abord** : Écrire tests sur les nouveaux concepts
3. **Coexistence** : Laisser ancien et nouveau code cohabiter
4. **Documentation** : Documenter les patterns utilisés
5. **Équipe** : Aligner l'équipe sur les nouveaux patterns
6. **Recette** : Avoir une recette testée avant de généraliser

---

**Migration complète estimée : 1-2 jours pour une petite équipe** ✅
