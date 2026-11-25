Prompt: 
Refactor the code to a better DDD design
Propose moi un plan de refactoring en te basant sur les fichiers recipe
Implement the plan: Plan de refactoring DDD progressif de la réservation de théâtre

Plan:
J’ai examiné le fichier copilot-skills/decision-tree-tactical-ddd.recipe ainsi que la solution C#.
Constats principaux :
•	TheaterService.cs concentre la réservation, la sélection contiguë des sièges, les règles de disponibilité, le calcul du prix, les remises, la persistance et le formatage XML.
•	Les objets de domaine sous Csharp/TheaterReservation/Data sont essentiellement des données mutables avec getters/setters (Reservation, Performance, TheaterRoom, Zone, Row, Seat).
•	Les règles métier sont exprimées par chaînes ("FREE", "BOOKED", "PREMIERE", "STANDARD", etc.), ce qui fragilise les invariants.
•	ReservationService et plusieurs DAO utilisent des méthodes statiques, des singletons implicites ou instancient directement leurs dépendances.
•	TheaterRoomDao mélange le stockage, la construction de données de test et la mutation détaillée des sièges.
•	Les tests actuels sont des tests d’approbation MSTest qui vérifient le XML retourné par TheaterService, notamment les scénarios de réservation, d’annulation et d’échec.
•	Le plan suivra les axes de la recipe : distinguer Value Objects et Entities, encapsuler les comportements dans le domaine, identifier un agrégat autour de la réservation/salle, réduire le rôle du service applicatif et formaliser les repositories.

Le plan de refactoring DDD progressif est prêt et visible dans le panneau de prévisualisation.
Il couvre notamment :
•	la conservation stricte du contrat XML et des tests ApprovalTests ;
•	l’introduction des dossiers Domain, Application et Infrastructure ;
•	la création de Value Objects immuables ;
•	l’encapsulation des règles dans les Entities et Aggregates ;
•	la transformation de TheaterService en service applicatif mince ;
•	le remplacement progressif des DAO par des repositories ;
•	la suppression des méthodes statiques et de l’état global ;
•	la migration progressive des tests et la validation à chaque étape.
