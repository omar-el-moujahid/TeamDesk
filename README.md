# TeamDesk

Application web de **gestion de demandes internes** (matériel, accès, support).
Les collaborateurs créent des demandes, un responsable les assigne, l'équipe échange par commentaires, et chacun suit l'avancement grâce aux statuts.

C'est l'équivalent d'une liste collaborative de type SharePoint, développée sur mesure avec **ASP.NET Core**, **Entity Framework Core**, **SQL Server** et **Angular**.

---

## Fonctionnalités

**Tous les utilisateurs connectés**
- Connexion sécurisée par **JWT**
- Liste des demandes avec **filtre par statut** et **pagination côté serveur**
- Page de détail : description, informations, historique des **commentaires**
- Ajout de commentaires

**Rôle `User`**
- Création d'une demande (titre, description, catégorie, priorité) avec validation du formulaire
- Changement du statut d'une demande **qui lui est assignée**

**Rôle `Manager`**
- **Assignation** d'une demande à un collaborateur
- Changement du statut de **n'importe quelle** demande
- **Tableau de bord** : nombre de demandes par statut

---

## Stack technique

| Couche | Technologies |
|---|---|
| Back-end | ASP.NET Core Web API (.NET 10, C#) |
| Accès aux données | Entity Framework Core 10, approche **Code First** avec migrations |
| Base de données | SQL Server |
| Sécurité | JWT Bearer, rôles `User` / `Manager`, hash des mots de passe (PBKDF2 via `PasswordHasher`) |
| Front-end | Angular 22 (composants standalone, signals, Reactive Forms, router, HttpClient) |
| Documentation API | OpenAPI + Swagger UI |
| Tests | xUnit + EF Core InMemory |

---

## Architecture

### Back-end : architecture en couches

```
Requête HTTP
   │
   ▼
Middlewares   → gestion globale des erreurs, CORS, authentification, autorisation
   │
   ▼
Controllers   → routes, codes HTTP, lecture de l'utilisateur dans le token
   │
   ▼
Services      → logique métier (interfaces + injection de dépendances)
   │
   ▼
DbContext     → Entity Framework Core → SQL Server
```

- Les contrôleurs n'exposent **jamais les entités** : uniquement des **DTOs**.
- Les services sont injectés via leurs **interfaces**, ce qui permet de les tester sans HTTP.
- Un **middleware global** intercepte les exceptions non gérées, les journalise et renvoie une réponse `ProblemDetails` (500) sans exposer la stack trace.
- Lectures optimisées : projections vers des DTOs, `AsNoTracking()`, agrégations (`GroupBy`) exécutées par SQL Server.

### Front-end

```
Pages (composants)  → affichage et interactions
   │
   ▼
Services            → appels HTTP vers l'API
   │
   ▼
Interceptor         → ajoute le token JWT à chaque requête vers l'API, gère les 401
```

- **Guards** : `authGuard` (utilisateur connecté) et `managerGuard` (rôle Manager).
- **Interceptor** : ajoute l'en-tête `Authorization: Bearer <token>` uniquement pour les appels vers l'API, et déconnecte l'utilisateur si le token a expiré.
- État des composants géré avec les **signals**.

---

## Modèle de données

```
User     (Id, Name, Email, PasswordHash, Role)
Category (Id, Name)
Request  (Id, Title, Description, CategoryId → Category, Priority, Status,
          CreatedAt, CreatedById → User, AssignedToId → User?)
Comment  (Id, Content, CreatedAt, RequestId → Request, AuthorId → User)
```

- `Request` possède **deux clés étrangères vers `User`** (créateur et personne assignée), configurées en **Fluent API** avec `DeleteBehavior.Restrict`.
- `Status`, `Priority` et `Role` sont des **enums**, sérialisés en texte dans le JSON (`JsonStringEnumConverter`).
- Index unique sur `User.Email`.

---

## API

| Méthode | Route | Accès |
|---|---|---|
| POST | `/api/auth/login` | public |
| GET | `/api/requests?status=&page=&pageSize=` | connecté |
| GET | `/api/requests/{id}` | connecté |
| POST | `/api/requests` | `User` |
| PUT | `/api/requests/{id}/assign` | `Manager` |
| PATCH | `/api/requests/{id}/status` | `Manager` ou personne assignée |
| POST | `/api/requests/{id}/comments` | connecté |
| GET | `/api/dashboard` | `Manager` |
| GET | `/api/categories` | connecté |
| GET | `/api/users` | `Manager` |

Codes de retour utilisés : `200`, `201` (avec en-tête `Location`), `204`, `400`, `401`, `403`, `404`, `500`.

Un scénario de test complet est disponible dans `TeamDesk.Api/TeamDesk.Api.http` (exécutable depuis Visual Studio).

---

## Sécurité

- Mots de passe **hachés** (PBKDF2 avec sel aléatoire), jamais stockés en clair.
- Token JWT **signé** (HMAC-SHA256), avec émetteur, audience et **expiration** vérifiés.
- L'identité de l'utilisateur (créateur d'une demande, auteur d'un commentaire) est **lue dans le token**, jamais envoyée par le client.
- Message d'erreur identique pour un email inconnu ou un mauvais mot de passe (pas d'énumération des comptes).
- **CORS** limitée à l'origine du front Angular.
- Les guards Angular servent à l'ergonomie : **la sécurité réelle est appliquée par l'API** (`[Authorize]` et rôles).

---

## Lancer le projet

### Prérequis

- SDK **.NET 10**
- **SQL Server** (ou SQL Server LocalDB)
- **Node.js** (version LTS) et **Angular CLI** (`npm install -g @angular/cli`)

### 1. Base de données et API

1. Vérifier la chaîne de connexion `DefaultConnection` dans `TeamDesk.Api/appsettings.json`.
2. Appliquer les migrations (elles créent aussi les données de test) :
   - dans Visual Studio (Console du Gestionnaire de package) : `Update-Database`
   - ou en ligne de commande : `dotnet ef database update` (depuis `TeamDesk.Api`)
3. Lancer l'API :

```powershell
cd TeamDesk.Api
dotnet run
```

L'API écoute sur `https://localhost:7216`. Swagger UI est disponible en développement.

### 2. Front-end

```powershell
cd teamdesk-web
npm install
ng serve
```

Ouvrir `http://localhost:4200`.

### 3. Tests

Depuis la racine du dépôt :

```powershell
dotnet test
```

---

## Comptes de test

Mot de passe pour tous les comptes : `Passw0rd!`

| Email | Rôle |
|---|---|
| `karim@teamdesk.local` | Manager |
| `sara@teamdesk.local` | User |
| `yassine@teamdesk.local` | User |

> La clé JWT présente dans `appsettings.json` est une **clé de développement**. En production, elle serait stockée dans les user-secrets ou dans Azure Key Vault.

---

## Tests unitaires

Les tests portent sur la **logique métier** de `RequestService`, avec une base EF Core en mémoire (une base isolée par test) :

- création refusée si la catégorie n'existe pas ;
- changement de statut **refusé** pour un utilisateur ni Manager ni assigné ;
- changement de statut **accepté** pour la personne assignée ;
- assignation refusée vers un utilisateur inexistant ;
- tableau de bord : comptage correct par statut, y compris les statuts sans demande.

---

## Structure du dépôt

```
TeamDesk/
├── TeamDesk.Api/           API ASP.NET Core
│   ├── Controllers/
│   ├── Data/               DbContext, Fluent API, données de test
│   ├── Dtos/
│   ├── Entities/
│   ├── Middlewares/
│   ├── Migrations/
│   └── Services/
├── TeamDesk.Tests/         tests xUnit
└── teamdesk-web/           application Angular
    └── src/app/
        ├── guards/
        ├── interceptors/
        ├── models/
        ├── pages/          login, liste, détail, création, tableau de bord
        └── services/
```

---

## Pistes d'amélioration

- **Refresh token** et stockage du token dans un cookie `HttpOnly` (protection contre le XSS)
- Tests d'intégration avec une vraie base (SQLite en mémoire ou SQL Server)
- **Docker Compose** (API + SQL Server + front) et pipeline **CI GitHub Actions**
- Gestion des utilisateurs et des catégories depuis l'interface
- Notifications lors d'une assignation ou d'un changement de statut
- Intégration à l'environnement Microsoft 365 (authentification Azure AD / Entra ID, SharePoint)
