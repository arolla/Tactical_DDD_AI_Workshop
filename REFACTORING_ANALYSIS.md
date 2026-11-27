# Analyse de Refactoring DDD - Tactical_DDD_AI_Workshop

## Concepts Métier Implicites Identifiés

### 1. **Statuts et États (Implicit Value Objects)**

#### Problème
- Les statuts sont représentés par des **strings magiques** dispersées dans le code :
  - Siège : `"FREE"`, `"BOOKED"`, `"BOOKING_PENDING"`
  - Réservation : `"PENDING"`, `"ABORTED"`, `"CANCELLED"`
  - Performance Nature : `"PREMIERE"`, `"PREVIEW"`

#### Concept Métier
- **SeatStatus** : Un Value Object encapsulant l'état d'un siège
- **ReservationStatus** : Un Value Object encapsulant l'état d'une réservation
- **PerformanceNature** : Un Value Object encapsulant le type de performance

#### Recipe de Refactoring
- Créer des enums ou des classes Value Objects immuables
- Remplacer les `string.Equals()` par des comparaisons d'enums
- Centraliser la validation des transitions d'état

---

### 2. **Identité de Réservation (Implicit Aggregate Root ID)**

#### Problème
- L'ID de réservation est généré par un **singleton avec un compteur statique** (anti-pattern)
- Pas d'encapsulation du concept d'ID

#### Concept Métier
- **ReservationId** : Un Value Object d'identité
- Génération d'ID décorélée du business logic

#### Recipe de Refactoring
- Créer une classe `ReservationId` (Value Object)
- Implémenter un service d'allocation d'ID distinct
- Remplacer `String` par `ReservationId` dans la signature

---

### 3. **Catégorie de Siège et Tarification (Implicit Value Objects)**

#### Problème
- Les catégories sont des strings : `"STANDARD"`, `"PREMIUM"`
- La tarification est implicitement liée à la catégorie (multiplicateur 1.5)
- La logique de calcul de prix est mélangée au business logic

#### Concept Métier
- **SeatCategory** : Un Value Object représentant le type de siège
- **Price** : Un Value Object immuable pour les montants
- **PriceCalculation** : Un Service de Domaine

#### Recipe de Refactoring
- Créer une énumération `SeatCategory` ou Value Object
- Extraire la logique de tarification dans un Service de Domaine
- Encapsuler les calculs de remise

---

### 4. **Politique VIP et Réserve de Sièges (Implicit Business Rule)**

#### Problème
- Les règles métier de réserve VIP sont implicites et dispersées :
  ```csharp
  if (performance.performanceNature.Equals("PREMIERE") && remainingSeats < totalSeats * 0.5)
  if (performance.performanceNature.Equals("PREVIEW") && remainingSeats < totalSeats * 0.9)
  ```

#### Concept Métier
- **VipReservationPolicy** : Une règle métier
- **PerformanceNature** : Détermine les seuils de réserve
- **SeatAllocationStrategy** : Une stratégie d'allocation

#### Recipe de Refactoring
- Créer une classe `VipReservationPolicy` ou `SeatReservationPolicy`
- Implémenter le pattern Strategy pour l'allocation
- Centraliser les règles métier

---

### 5. **Réduction et Fidélité Client (Implicit Service)**

#### Problème
- Les réductions sont calculées de manière implicite :
  - Réduction temporelle (via `discountTime`)
  - Réduction fidélité (25% pour abonnés)
- Mélange avec la logique de réservation

#### Concept Métier
- **Customer** : Un Aggregate Root
- **Subscription** : Une relation
- **DiscountPolicy** : Une règle métier
- **PricingPolicy** : Un Service de Domaine

#### Recipe de Refactoring
- Créer un Aggregate Customer avec Subscription
- Implémenter des stratégies de réduction (Composite Pattern)
- Dégager les calculs de prix dans un Price Calculator

---

### 6. **Allocation Contiguë de Sièges (Implicit Business Rule)**

#### Problème
- L'algorithme de recherche de sièges contiguës est complexe et implicite
- Mélangé directement dans la méthode `reservation()`
- Pas de validations explicites des contraintes

#### Concept Métier
- **SeatAllocationAlgorithm** : Une stratégie d'allocation
- **SeatAllocationRequest** : Un Value Object
- **SeatAllocationResult** : Un Value Object

#### Recipe de Refactoring
- Extraire l'algorithme dans une classe `ContiguousSeatAllocator`
- Implémenter une interface `ISeatAllocator`
- Valider les contraintes explicitement

---

### 7. **Réservation et Ses Composants (Implicit Aggregate Root)**

#### Problème
- `Reservation` est un simple DTO sans logique
- Les sièges sont un tableau de strings
- Pas d'invariants d'aggregate protégés

#### Concept Métier
- **Reservation** : Un Aggregate Root
- **ReservationItem** : Un Entity pour représenter chaque siège réservé
- **ReservationLines** : Une Collection d'items

#### Recipe de Refactoring
- Transformer `Reservation` en Aggregate Root riche
- Créer `ReservationItem` avec logique métier
- Encapsuler et valider les invariants

---

### 8. **Salle de Spectacle (Implicit Aggregate Root)**

#### Problème
- `TheaterRoom` est un simple DTO
- Pas de responsabilité sur la gestion des sièges

#### Concept Métier
- **TheaterRoom** : Un Aggregate Root représentant l'entité de salle
- **SeatInventory** : Un Service de Domaine pour gérer la disponibilité

#### Recipe de Refactoring
- Enrichir `TheaterRoom` avec des méthodes de domaine
- Créer un Service de Domaine pour la gestion d'inventaire
- Protéger les invariants d'allocation

---

## Résumé des Refactorisations Proposées

| Concept | Type DDD | Priorité | Impact |
|---------|----------|----------|--------|
| Statuts | Value Object | Critique | Élimine les strings magiques |
| ReservationId | Value Object + Service | Haute | Élimine le singleton |
| SeatCategory | Enum/Value Object | Haute | Centralise les tarifs |
| VipPolicy | Business Rule | Moyenne | Rend explicite la règle |
| Discount/Pricing | Service de Domaine | Critique | Désenchevêtrement du calcul |
| SeatAllocation | Stratégie/Service | Critique | Algorithme réutilisable |
| Reservation | Aggregate Root | Haute | Structure cohérente |
| TheaterRoom | Aggregate Root | Moyenne | Responsabilités claires |

---

## Recettes de Refactoring Appliquées

1. **Extract Value Object** : Statuts, Catégories, Identités
2. **Extract Service** : Tarification, Allocation, Réserves VIP
3. **Replace String with Enum** : Statuts et Natures
4. **Extract Strategy** : Allocation, Tarification
5. **Aggregate Root Enrichment** : Reservation, TheaterRoom
6. **Encapsulate Collection** : Sièges, Réservations
7. **Replace Singleton** : Service d'allocation d'ID
