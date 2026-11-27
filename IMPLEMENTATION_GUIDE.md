# Implémentation du Refactoring DDD - Guide Détaillé

## Concepts Métier Extraits ✅

### 1. Value Objects

#### ✅ SeatStatus
- **Fichier** : `Domain/ValueObjects/SeatStatus.cs`
- **Concept** : Encapsule les états possibles d'un siège
- **États** : `FREE`, `BOOKED`, `BOOKING_PENDING`
- **Avantages** :
  - Élimine les strings magiques
  - Fournit des prédicats : `IsFree`, `IsAvailable`
  - Validations centralisées
  - Transitions d'état explicites

#### ✅ ReservationStatus
- **Fichier** : `Domain/ValueObjects/ReservationStatus.cs`
- **Concept** : Encapsule les états possibles d'une réservation
- **États** : `PENDING`, `ABORTED`, `CANCELLED`, `CONFIRMED`
- **Avantages** :
  - Remplace les strings disparates
  - Prédicats : `IsPending`, `IsActive`, `IsCancelled`
  - Logique métier centralisée

#### ✅ PerformanceNature
- **Fichier** : `Domain/ValueObjects/PerformanceNature.cs`
- **Concept** : Encapsule les types de performances
- **Types** : `PREMIERE`, `PREVIEW`, `REGULAR`
- **Avantages** :
  - Détermine les règles VIP
  - Prédicats : `IsPremiere()`, `IsPreview()`
  - Évite les conversions de strings

#### ✅ SeatCategory
- **Fichier** : `Domain/ValueObjects/SeatCategory.cs`
- **Concept** : Encapsule les catégories de sièges
- **Catégories** : `STANDARD`, `PREMIUM`
- **Avantages** :
  - Encapsule la tarification : `GetPricingRatio()`
  - Logique de calcul centralisée
  - Prédicats : `IsStandard()`, `IsPremium()`

#### ✅ ReservationId
- **Fichier** : `Domain/ValueObjects/ReservationId.cs`
- **Concept** : Identité fortement typée d'une réservation
- **Avantages** :
  - Remplace `Int64` / `String`
  - Validation : ID > 0
  - Type-safe dans les signatures

#### ✅ Price
- **Fichier** : `Domain/ValueObjects/Price.cs`
- **Concept** : Montant monétaire immuable
- **Opérations** :
  - `Add()` : Addition de prix
  - `Multiply()` : Multiplication (par ratio de catégorie)
  - `ApplyDiscount()` : Application de réductions
- **Avantages** :
  - Encapsulation des calculs financiers
  - Arrondi automatique à 2 décimales
  - Comparaisons supportées

---

### 2. Services de Domaine

#### ✅ SeatAllocator
- **Fichier** : `Domain/DomainServices/SeatAllocator.cs`
- **Responsabilité** : Allouer des sièges contiguës selon les critères
- **Interface** : `ISeatAllocator`
- **Implémentation** : `ContiguousSeatAllocator`
- **Entrées** :
  ```csharp
  SeatAllocationRequest {
	SeatsRequested: int,
	CategoryRequested: SeatCategory,
	AvailableSeats: SeatAvailability[]
  }
  ```
- **Sorties** :
  ```csharp
  SeatAllocationResult {
	AllocatedSeatIds: string[],
	IsSuccessful: bool,
	Reason: string
  }
  ```
- **Avantages** :
  - Algorithme d'allocation réutilisable
  - Testable indépendamment
  - Validation explicite
  - Messages d'erreur détaillés

#### ✅ DiscountPolicy
- **Fichier** : `Domain/DomainServices/DiscountPolicy.cs`
- **Responsabilité** : Calculer les réductions applicables
- **Implémentations** :
  - `TimeBasedDiscountPolicy` : Réductions par créneaux horaires
  - `SubscriptionDiscountPolicy` : Réduction fidélité (17.5%)
  - `CompositeDiscountPolicy` : Combinaison de politiques
- **Avantages** :
  - Pattern Strategy pour extensibilité
  - Politiques composables
  - Respect de l'Open/Closed Principle

#### ✅ PriceCalculator
- **Fichier** : `Domain/DomainServices/PriceCalculator.cs`
- **Responsabilité** : Calculer le prix final d'une réservation
- **Entrées** :
  ```csharp
  PriceCalculationRequest {
	BasePrice: Price,
	SeatCategories: SeatCategory[],
	DiscountPolicy: IDiscountPolicy,
	DiscountContext: DiscountContext
  }
  ```
- **Sortie** : `Price`
- **Formule** :
  ```
  finalPrice = basePrice × Σ(categoryRatio) × (1 - discountRatio)
  ```
- **Avantages** :
  - Calcul centralisé et testable
  - Dépend d'abstractions
  - Pas de calcul implicite dans la réservation

#### ✅ VipReservationPolicy
- **Fichier** : `Domain/DomainServices/VipReservationPolicy.cs`
- **Responsabilité** : Vérifier les contraintes VIP avant allocation
- **Implémentation** : `PerformanceNatureBasedVipPolicy`
- **Règles** :
  - **PREMIERE** : Réserver 50% pour VIP
  - **PREVIEW** : Réserver 10% pour VIP
  - **REGULAR** : Pas de restriction
- **Avantages** :
  - Règles métier explicites
  - Vérifiable avant allocation
  - Messages d'erreur contextuels

#### ✅ ReservationIdAllocator
- **Fichier** : `Domain/DomainServices/ReservationIdAllocator.cs`
- **Responsabilité** : Générer des IDs séquentiels uniques
- **Implémentation** : `SequentialReservationIdAllocator`
- **Avantages** :
  - Thread-safe avec `lock`
  - Remplace le singleton anti-pattern
  - Testable et injectable
  - Peut être remplacé par d'autres implémentations (UUID, etc.)

---

## Résumé des Améliorations

| Concept | Avant | Après | Bénéfice |
|---------|-------|-------|----------|
| Statuts | `string` | `SeatStatus`, `ReservationStatus`, `PerformanceNature` | Type-safety + validation |
| Catégories | `string` | `SeatCategory` | Tarification centralisée |
| Prix | `decimal` | `Price` | Opérations financières encapsulées |
| IDs | `Int64` | `ReservationId` | Domaine ubiquitous |
| Allocation | Algos implicites | `ISeatAllocator` | Testable + réutilisable |
| Tarification | Mélangée | `PriceCalculator` | Séparation des responsabilités |
| Réductions | Disparates | `IDiscountPolicy` | Stratégies composables |
| VIP Rules | Implicites | `IVipReservationPolicy` | Explicite + testable |
| Génération ID | Singleton | `IReservationIdAllocator` | Injectable + testable |

---

## Prochaines Étapes de Refactoring

### Phase 2 : Agrégats Enrichis
- Transformer `Reservation` en Aggregate Root riche
- Créer `ReservationItem` pour les sièges alloués
- Implémenter les invariants d'agrégat

### Phase 3 : Repositories
- Créer `IReservationRepository`
- Transformer `ReservationDao` en adapter
- Implémenter la persistence

### Phase 4 : Application Services
- Créer `ReservationApplicationService`
- Orchestrer les Services de Domaine
- Gérer les transactions

### Phase 5 : Événements de Domaine
- Implémenter `ReservationCreated`, `ReservationCancelled`
- Publier les événements
- Implémenter des listeners

---

## Tests Recommandés

### Tests des Value Objects
```csharp
// Test SeatStatus
[Test]
public void From_ValidStatus_CreatesInstance()
{
	var status = SeatStatus.From("FREE");
	Assert.That(status.IsFree, Is.True);
}

// Test Price
[Test]
public void Multiply_ValidFactor_ReturnsCorrectPrice()
{
	var price = new Price(100m);
	var result = price.Multiply(1.5m);
	Assert.That(result.Amount, Is.EqualTo(150m));
}
```

### Tests des Services de Domaine
```csharp
// Test SeatAllocator
[Test]
public void AllocateSeats_EnoughContiguousSeats_SuccessfulAllocation()
{
	var allocator = new ContiguousSeatAllocator();
	var request = new SeatAllocationRequest { ... };
	var result = allocator.AllocateSeats(request);
	Assert.That(result.IsSuccessful, Is.True);
}

// Test PriceCalculator
[Test]
public void CalculatePrice_WithDiscount_AppliesCorrectly()
{
	var calculator = new PriceCalculator();
	// ...
}
```

---

## Recettes de Refactoring Appliquées

1. ✅ **Extract Value Object** → SeatStatus, ReservationStatus, etc.
2. ✅ **Replace String with Enum** → Statuts typés
3. ✅ **Extract Method Object** → SeatAllocationRequest
4. ✅ **Extract Strategy** → Discount Policies
5. ✅ **Introduce Service Object** → PriceCalculator, SeatAllocator
6. ⏳ **Replace Singleton** → ReservationIdAllocator (prêt à l'usage)
7. ⏳ **Extract Aggregate** → Reservation enrichit (phase 2)
