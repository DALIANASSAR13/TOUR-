# Tour Guide Booking — Class Diagrams

UML diagrams for the whole system, split by use case so each one can be read on its own.
Sources: the SRS, the Class Specs, and `AGENTS.md`. Diagrams are written in Mermaid; GitHub and VS Code (with a Mermaid extension) render them.

**Contents**

1. How to read the diagrams
2. Big picture: layers and use cases
3. Domain model: relationships, guide side, booking side
4. State diagrams: booking status, guide verification status
5. Class diagrams by use case (8 cases)
6. Layer reference: DAL, BLL shared classes, validators and mapping, Web cross-cutting
7. Catalog: DTOs, ViewModels, EF configurations

---

## 1. How to read the diagrams

| Symbol | Meaning |
| --- | --- |
| `A --> B` | A depends on B (A holds a reference to B, injected by DI) |
| `A ..> B` | A uses B (creates it, returns it, or reads it) |
| `A <\|.. B` | B implements interface A |
| `A <\|-- B` | B inherits from A |
| `A *-- B` | Composition: B cannot exist without A |
| `A -- B` | Association between entities, with multiplicity (`1`, `0..1`, `0..*`, `1..*`) |
| `<<interface>>` | An interface, registered in DI |
| `+` / `-` / `#` | public / private / protected |
| `$` after a member | static |

- Every async method also takes a `CancellationToken`. It is left out of the diagrams to keep them short.
- Every service and repository is registered in DI by its interface (see `AGENTS.md` → "DI lifetimes").

---

## 2. Big picture

### 2.1 Layers

Calls go one way only: Web → BLL → DAL → SQL Server.

```mermaid
flowchart TB
    subgraph Web["Web (Presentation)"]
        C["Controllers (Public, Admin, Guide, Tourist areas)"]
        VM["ViewModels + Views + client-side validation"]
        J["BookingStatusJob (BackgroundService)"]
        WM["ViewModel AutoMapper profiles"]
    end
    subgraph BLL["BLL (Business logic)"]
        S["Services (8) behind interfaces"]
        D["Domain: BookingStateMachine, PriceCalculator, BookingRules"]
        V["FluentValidation validators"]
        M["Entity/DTO AutoMapper profiles"]
        I["Infrastructure: FileStorage, PaymentGateway, CountryProvider"]
    end
    subgraph DAL["DAL (Data access)"]
        U["UnitOfWork"]
        R["Repositories (generic + specific)"]
        X["AppDbContext + configurations"]
        E["Entities + Enums"]
    end
    DB[("SQL Server")]

    C --> S
    J --> S
    S --> D
    S --> V
    S --> M
    S --> I
    S --> U
    U --> R
    R --> X
    X --> E
    X --> DB
```

### 2.2 Use cases by actor

Each use case below has its own class diagram in section 5.

```mermaid
flowchart LR
    T(["Tourist"])
    G(["Guide"])
    A(["Admin"])
    SYS(["System job"])

    UC1["5.1 Register and log in"]
    UC2["5.2 Guide verification"]
    UC3["5.3 Guide profile"]
    UC4["5.4 Search guides"]
    UC5["5.5 Booking request"]
    UC6["5.6 Payment"]
    UC7["5.7 Review"]
    UC8["5.8 Auto-cancel and auto-complete"]

    T --> UC1
    T --> UC4
    T --> UC5
    T --> UC6
    T --> UC7
    G --> UC1
    G --> UC2
    G --> UC3
    G --> UC5
    A --> UC1
    A --> UC2
    SYS --> UC8
```

---

## 3. Domain model

All 10 entities and 5 enums live in `TourGuide.DAL/Entities` and `TourGuide.DAL/Enums`. The model is shown in three parts: relationships only, then the guide side and the booking side with every property.

### 3.1 Relationships at a glance

```mermaid
classDiagram
    direction LR
    class ApplicationUser
    class GuideProfile
    class GuideDocument
    class Language
    class Specialization
    class GuideLanguage
    class GuideSpecialization
    class Booking
    class Payment
    class Review
    ApplicationUser "1" -- "0..1" GuideProfile : is a guide
    ApplicationUser "0..1" -- "0..*" GuideProfile : reviews as admin
    GuideProfile "1" *-- "0..3" GuideDocument : uploads
    GuideProfile "1" *-- "1..*" GuideLanguage
    Language "1" -- "0..*" GuideLanguage
    GuideProfile "1" *-- "1..*" GuideSpecialization
    Specialization "1" -- "0..*" GuideSpecialization
    ApplicationUser "1" -- "0..*" Booking : books as tourist
    GuideProfile "1" -- "0..*" Booking : is booked
    Booking "1" *-- "0..1" Payment : paid by
    Booking "1" -- "0..1" Review : reviewed in
    ApplicationUser "1" -- "0..*" Review : writes
    GuideProfile "1" -- "0..*" Review : receives
```

### 3.2 Guide side: accounts, guides, documents, languages, specializations

```mermaid
classDiagram
    direction LR
    class ApplicationUser {
        <<IdentityUser>>
        +string Id
        +string Email
        +string FullName
        +string CountryCode
        +DateTime CreatedAtUtc
    }
    class GuideProfile {
        +int Id
        +string UserId
        +VerificationStatus VerificationStatus
        +string? RejectionReason
        +DateTime SubmittedAtUtc
        +DateTime? ReviewedAtUtc
        +string? ReviewedByAdminId
        +string? CountryCode
        +string? City
        +string? TimeZoneId
        +string? Bio
        +string? PhotoFileName
        +int? YearsOfExperience
        +decimal? PricePerHour
        +decimal AverageRating
        +int ReviewCount
    }
    class GuideDocument {
        +int Id
        +int GuideProfileId
        +DocumentType DocumentType
        +string StoredFileName
        +string OriginalFileName
        +string ContentType
        +long SizeBytes
        +DateTime UploadedAtUtc
    }
    class Language {
        +int Id
        +string Name
    }
    class Specialization {
        +int Id
        +string Name
    }
    class GuideLanguage {
        +int GuideProfileId
        +int LanguageId
    }
    class GuideSpecialization {
        +int GuideProfileId
        +int SpecializationId
    }
    class VerificationStatus {
        <<enumeration>>
        Pending
        Approved
        Rejected
    }
    class DocumentType {
        <<enumeration>>
        ProofOfResidence
        GovernmentId
        CriminalRecord
    }

    ApplicationUser "1" -- "0..1" GuideProfile : is a guide
    ApplicationUser "0..1" -- "0..*" GuideProfile : reviews as admin
    GuideProfile "1" *-- "0..3" GuideDocument : uploads
    GuideProfile "1" *-- "1..*" GuideLanguage
    Language "1" -- "0..*" GuideLanguage
    GuideProfile "1" *-- "1..*" GuideSpecialization
    Specialization "1" -- "0..*" GuideSpecialization
    GuideProfile ..> VerificationStatus
    GuideDocument ..> DocumentType
```

- One `GuideDocument` per `DocumentType` (unique `GuideProfileId` + `DocumentType`), so at most 3.
- `1..*` on the join classes applies once the profile is complete. A new guide has none yet.

### 3.3 Booking side: bookings, payments, reviews

```mermaid
classDiagram
    direction LR
    class ApplicationUser {
        <<IdentityUser>>
        see 3.2
    }
    class GuideProfile {
        see 3.2
    }
    class Booking {
        +int Id
        +string TouristId
        +int GuideProfileId
        +BookingType BookingType
        +DateTime StartUtc
        +DateTime EndUtc
        +int? Hours
        +int? Days
        +int BillableHours
        +decimal PricePerHourAtRequest
        +decimal TotalPrice
        +BookingStatus Status
        +DateTime CreatedAtUtc
        +DateTime? RespondedAtUtc
        +DateTime? PaymentDeadlineUtc
        +DateTime? CancelledAtUtc
        +CancellationReason? CancellationReason
        +DateTime? CompletedAtUtc
        +byte[] RowVersion
    }
    class Payment {
        +int Id
        +int BookingId
        +decimal Amount
        +string TransactionReference
        +DateTime PaidAtUtc
    }
    class Review {
        +int Id
        +int BookingId
        +string TouristId
        +int GuideProfileId
        +int Rating
        +string? Comment
        +DateTime CreatedAtUtc
    }
    class BookingType {
        <<enumeration>>
        Hourly
        MultiDay
    }
    class BookingStatus {
        <<enumeration>>
        Pending
        Accepted
        Confirmed
        Completed
        Rejected
        Cancelled
    }
    class CancellationReason {
        <<enumeration>>
        WithdrawnByTourist
        NoGuideResponse
        NotPaid
    }

    ApplicationUser "1" -- "0..*" Booking : books as tourist
    GuideProfile "1" -- "0..*" Booking : is booked
    Booking "1" *-- "0..1" Payment : paid by
    Booking "1" -- "0..1" Review : reviewed in
    ApplicationUser "1" -- "0..*" Review : writes
    GuideProfile "1" -- "0..*" Review : receives
    Booking ..> BookingType
    Booking ..> BookingStatus
    Booking ..> CancellationReason
```

- `Booking.PricePerHourAtRequest` is a copy of the guide's price when the request was sent; later price changes don't affect it.
- `Review.GuideProfileId` and `Review.TouristId` are copied from the booking so profile pages load fast.

## 4. State diagrams

### 4.1 Booking status

`IBookingStateMachine` is the only code allowed to make these changes (BR-05 to BR-10). Any change not drawn here is refused with `ErrorCodes.InvalidStatusChange`.

```mermaid
stateDiagram-v2
    direction LR
    [*] --> Pending : Tourist sends request (FR-11)
    Pending --> Accepted : Guide accepts (no overlap with Accepted or Confirmed)
    Pending --> Rejected : Guide rejects
    Pending --> Cancelled : Tourist withdraws, or no answer in 24 h (System)
    Accepted --> Confirmed : Tourist pays before PaymentDeadlineUtc
    Accepted --> Cancelled : Tourist withdraws, or deadline passed (System)
    Confirmed --> Completed : EndUtc passed (System)
    Completed --> [*] : Tourist can review once
    Rejected --> [*]
    Cancelled --> [*]

    note right of Accepted
        PaymentDeadlineUtc =
        earlier of (accept time + 24 h) and StartUtc
    end note
    note right of Cancelled
        CancellationReason:
        WithdrawnByTourist, NoGuideResponse, NotPaid
    end note
```

| From | To | Who | Condition | Sets |
| --- | --- | --- | --- | --- |
| — | Pending | Tourist | Guide approved and profile complete; valid request | `CreatedAtUtc`, prices |
| Pending | Accepted | Guide (owner) | No overlap with the guide's Accepted or Confirmed bookings | `RespondedAtUtc`, `PaymentDeadlineUtc` |
| Pending | Rejected | Guide (owner) | — | `RespondedAtUtc` |
| Pending | Cancelled | Tourist (owner) | — | `CancelledAtUtc`, `WithdrawnByTourist` |
| Pending | Cancelled | System | 24 h after `CreatedAtUtc` | `CancelledAtUtc`, `NoGuideResponse` |
| Accepted | Confirmed | Tourist (owner) | Before `PaymentDeadlineUtc`; a `Payment` is saved in the same transaction | — |
| Accepted | Cancelled | Tourist (owner) | — | `CancelledAtUtc`, `WithdrawnByTourist` |
| Accepted | Cancelled | System | `PaymentDeadlineUtc` passed | `CancelledAtUtc`, `NotPaid` |
| Confirmed | Completed | System | `EndUtc` passed | `CompletedAtUtc` |

### 4.2 Guide verification status

A guide is shown in search and can be booked only when **Approved and the profile is complete** (BR-01, BR-02).

```mermaid
stateDiagram-v2
    direction LR
    [*] --> Pending : Guide signs up with 3 documents (FR-02)
    Pending --> Approved : Admin approves (all 3 documents present)
    Pending --> Rejected : Admin rejects with a reason (10-500 chars)
    Rejected --> Pending : Guide re-applies with new documents

    state Approved {
        direction LR
        [*] --> ProfileIncomplete
        ProfileIncomplete --> Bookable : Guide completes profile (FR-08)
    }

    note right of Rejected
        Guide sees the RejectionReason.
        Re-apply replaces the old files.
    end note
```

| State | Guide can... | Shown in search |
| --- | --- | --- |
| Pending | See "waiting for review" | No |
| Rejected | See the reason and re-apply | No |
| Approved, profile incomplete | Fill in the profile | No |
| Approved, profile complete (Bookable) | Receive and answer requests | Yes |

---

## 5. Class diagrams by use case

Each diagram follows one use case from the controller down to the entities. Classes shared by many cases (for example `IUnitOfWork`, `Result`) appear in full only in section 6.

### 5.1 Register and log in

**Actors:** Tourist, Guide, Admin · **FRs:** FR-01, FR-02, FR-03 · **Rules:** public sign-up creates only Tourist or Guide; admins are seeded.

```mermaid
classDiagram
    direction LR
    class AccountController {
        <<Web Controller>>
        +Register() IActionResult
        +Register(RegisterTouristViewModel) Task~IActionResult~
        +RegisterGuide() IActionResult
        +RegisterGuide(RegisterGuideViewModel) Task~IActionResult~
        +Login(LoginViewModel) Task~IActionResult~
        +Logout() Task~IActionResult~
    }
    class IAccountService {
        <<interface>>
        +RegisterTouristAsync(RegisterTouristRequest) Task~Result~string~~
        +RegisterGuideAsync(RegisterGuideRequest) Task~Result~string~~
    }
    class AccountService
    class RegisterTouristValidator {
        <<FluentValidation>>
    }
    class RegisterGuideValidator {
        <<FluentValidation>>
    }
    class FileUploadValidator {
        <<FluentValidation>>
    }
    class ICountryProvider {
        <<interface>>
        +GetAll() List~CountryDto~
        +Exists(string code) bool
    }
    class IFileStorage {
        <<interface>>
    }
    class IUnitOfWork {
        <<interface>>
    }
    class UserManager~ApplicationUser~ {
        <<Identity>>
    }
    class SignInManager~ApplicationUser~ {
        <<Identity>>
    }
    class ApplicationUser
    class GuideProfile
    class GuideDocument

    AccountController --> IAccountService
    AccountController --> SignInManager~ApplicationUser~ : log in and out
    IAccountService <|.. AccountService
    AccountService --> UserManager~ApplicationUser~ : create user, add role
    AccountService --> RegisterTouristValidator
    AccountService --> RegisterGuideValidator
    RegisterTouristValidator --> ICountryProvider
    RegisterGuideValidator --> ICountryProvider
    RegisterGuideValidator --> FileUploadValidator : each of 3 documents
    AccountService --> IFileStorage : save documents
    AccountService --> IUnitOfWork
    AccountService ..> ApplicationUser : creates
    AccountService ..> GuideProfile : creates as Pending
    AccountService ..> GuideDocument : creates 3
```

| Class | Responsibility |
| --- | --- |
| `AccountController` | Sign-up forms for both roles, log in, log out. Uses `SignInManager` only for the HTTP sign-in. |
| `AccountService` | Creates the user with `UserManager`, adds the role, and for a guide also saves the 3 files and creates `GuideProfile` + 3 `GuideDocument` rows in one transaction. |
| `RegisterTouristValidator` | Full name 2–100, valid email, country code in the list. |
| `RegisterGuideValidator` | Same as tourist, plus all 3 documents present and valid. |

### 5.2 Guide verification

**Actors:** Admin, Guide · **FRs:** FR-04 to FR-07 · **Rules:** BR-01, BR-03; reason required on reject; re-apply only from Rejected.

```mermaid
classDiagram
    direction LR
    class GuideVerificationController {
        <<Admin Area>>
        +Index() Task~IActionResult~
        +Review(int id) Task~IActionResult~
        +Document(int documentId) Task~IActionResult~
        +Approve(int id) Task~IActionResult~
        +Reject(RejectGuideViewModel) Task~IActionResult~
    }
    class VerificationController {
        <<Guide Area>>
        +Status() Task~IActionResult~
        +Reapply(ReapplyViewModel) Task~IActionResult~
    }
    class IGuideVerificationService {
        <<interface>>
        +GetPendingAsync() Task~List~PendingGuideDto~~
        +GetForReviewAsync(int guideProfileId) Task~Result~GuideReviewDto~~
        +OpenDocumentAsync(int documentId) Task~Result~FileContentDto~~
        +ApproveAsync(string adminId, int guideProfileId) Task~Result~
        +RejectAsync(string adminId, RejectGuideRequest) Task~Result~
        +GetMyStatusAsync(string guideUserId) Task~Result~GuideStatusDto~~
        +ReapplyAsync(string guideUserId, ReapplyRequest) Task~Result~
    }
    class GuideVerificationService
    class RejectGuideValidator {
        <<FluentValidation>>
    }
    class ReapplyValidator {
        <<FluentValidation>>
    }
    class FileUploadValidator {
        <<FluentValidation>>
    }
    class IFileStorage {
        <<interface>>
    }
    class IUnitOfWork {
        <<interface>>
    }
    class IGuideProfileRepository {
        <<interface>>
        +GetPendingAsync() Task~List~GuideProfile~~
        +GetWithDetailsAsync(int id) Task~GuideProfile?~
        +GetByUserIdAsync(string userId) Task~GuideProfile?~
    }
    class IGuideDocumentRepository {
        <<interface>>
        +GetByGuideAsync(int guideId) Task~List~GuideDocument~~
        +GetByTypeAsync(int guideId, DocumentType type) Task~GuideDocument?~
    }
    class GuideMappingProfile {
        <<AutoMapper>>
    }
    class GuideProfile
    class GuideDocument

    GuideVerificationController --> IGuideVerificationService
    VerificationController --> IGuideVerificationService
    IGuideVerificationService <|.. GuideVerificationService
    GuideVerificationService --> RejectGuideValidator
    GuideVerificationService --> ReapplyValidator
    ReapplyValidator --> FileUploadValidator
    GuideVerificationService --> IFileStorage : read and replace files
    GuideVerificationService --> IUnitOfWork
    GuideVerificationService ..> GuideMappingProfile : via IMapper
    IUnitOfWork --> IGuideProfileRepository
    IUnitOfWork --> IGuideDocumentRepository
    IGuideProfileRepository ..> GuideProfile
    IGuideDocumentRepository ..> GuideDocument
```

| Class | Responsibility |
| --- | --- |
| `GuideVerificationController` | Admin only (`[Authorize(Roles = Roles.Admin)]`). Lists pending guides, shows documents, approves or rejects. `Document` streams the file; it is never a public URL. |
| `VerificationController` | Guide only. Shows the current status and reason, and the re-apply form. |
| `GuideVerificationService` | Checks the status rules: approve needs 3 documents; reject needs a reason; re-apply only from Rejected and sets status back to Pending. |

### 5.3 Guide profile

**Actors:** Guide (edit), Tourist (view) · **FRs:** FR-08, FR-09 · **Rules:** only approved guides edit; profile complete = country, city, time zone, price, at least 1 language and 1 specialization.

```mermaid
classDiagram
    direction LR
    class ProfileController {
        <<Guide Area>>
        +Edit() Task~IActionResult~
        +Edit(EditGuideProfileViewModel) Task~IActionResult~
        +Photo(int guideProfileId) Task~IActionResult~
    }
    class GuidesController {
        <<Public>>
        +Details(int id, int page) Task~IActionResult~
    }
    class IGuideProfileService {
        <<interface>>
        +GetMyProfileAsync(string guideUserId) Task~Result~GuideProfileEditDto~~
        +UpdateMyProfileAsync(string guideUserId, UpdateGuideProfileRequest) Task~Result~
        +GetPublicProfileAsync(int guideProfileId, int reviewPage) Task~Result~PublicGuideProfileDto~~
        +GetPhotoAsync(int guideProfileId) Task~Result~FileContentDto~~
    }
    class GuideProfileService
    class UpdateGuideProfileValidator {
        <<FluentValidation>>
    }
    class FileUploadValidator {
        <<FluentValidation>>
    }
    class ICountryProvider {
        <<interface>>
    }
    class IFileStorage {
        <<interface>>
    }
    class IUnitOfWork {
        <<interface>>
    }
    class IGuideProfileRepository {
        <<interface>>
    }
    class IReviewRepository {
        <<interface>>
        +GetForGuideAsync(int guideId, int page) Task~PagedList~Review~~
    }
    class IRepository~Language~ {
        <<interface>>
    }
    class IRepository~Specialization~ {
        <<interface>>
    }
    class GuideMappingProfile {
        <<AutoMapper>>
    }
    class GuideProfile
    class GuideLanguage
    class GuideSpecialization

    ProfileController --> IGuideProfileService
    GuidesController --> IGuideProfileService
    IGuideProfileService <|.. GuideProfileService
    GuideProfileService --> UpdateGuideProfileValidator
    UpdateGuideProfileValidator --> ICountryProvider
    UpdateGuideProfileValidator --> FileUploadValidator : photo
    GuideProfileService --> IFileStorage : save photo
    GuideProfileService --> IUnitOfWork
    GuideProfileService ..> GuideMappingProfile : via IMapper
    IUnitOfWork --> IGuideProfileRepository
    IUnitOfWork --> IReviewRepository
    IUnitOfWork --> IRepository~Language~
    IUnitOfWork --> IRepository~Specialization~
    GuideProfile *-- GuideLanguage
    GuideProfile *-- GuideSpecialization
```

| Class | Responsibility |
| --- | --- |
| `ProfileController` | Guide only. Edit form for country, city, time zone, languages, specializations, price, bio, experience, photo. |
| `GuidesController` | Public profile page with average rating, reviews (paged) and a Book button. |
| `GuideProfileService` | Refuses edits unless the guide is Approved. Replaces the language and specialization lists as a whole. |
| `UpdateGuideProfileValidator` | Price > 0 and ≤ 1,000; experience 0–60; bio ≤ 1,000; ≥ 1 language and specialization; valid time zone id; photo JPG/PNG ≤ 5 MB. |

### 5.4 Search guides

**Actor:** Tourist · **FR:** FR-10 · **Rule:** only Approved guides with a complete profile (BR-02).

```mermaid
classDiagram
    direction LR
    class SearchController {
        <<Public>>
        +Index(SearchViewModel) Task~IActionResult~
    }
    class ISearchService {
        <<interface>>
        +SearchAsync(GuideSearchFilter) Task~Result~PagedList~GuideCardDto~~~
        +GetFilterOptionsAsync() Task~SearchFilterOptionsDto~
    }
    class SearchService
    class GuideSearchFilterValidator {
        <<FluentValidation>>
    }
    class GuideSearchFilter {
        <<DTO>>
        +string? CountryCode
        +int? LanguageId
        +int? SpecializationId
        +decimal? MinPrice
        +decimal? MaxPrice
        +int Page
    }
    class GuideSearchQuery {
        <<DAL query object>>
    }
    class ICountryProvider {
        <<interface>>
    }
    class IUnitOfWork {
        <<interface>>
    }
    class IGuideProfileRepository {
        <<interface>>
        +SearchAsync(GuideSearchQuery) Task~PagedList~GuideProfile~~
    }
    class PagedList~T~ {
        +List~T~ Items
        +int Page
        +int PageSize
        +int TotalCount
    }
    class GuideMappingProfile {
        <<AutoMapper>>
    }

    SearchController --> ISearchService
    ISearchService <|.. SearchService
    SearchService --> GuideSearchFilterValidator
    SearchService ..> GuideSearchFilter : input
    SearchService ..> GuideSearchQuery : builds
    SearchService --> ICountryProvider
    SearchService --> IUnitOfWork
    SearchService ..> GuideMappingProfile : via IMapper
    IUnitOfWork --> IGuideProfileRepository
    IGuideProfileRepository ..> PagedList~T~ : returns
```

| Class | Responsibility |
| --- | --- |
| `SearchService` | Validates the filter, turns it into a `GuideSearchQuery`, maps results to `GuideCardDto`. |
| `GuideSearchFilterValidator` | Min price ≤ max price; prices ≥ 0; page ≥ 1. |
| `IGuideProfileRepository.SearchAsync` | Always adds "Approved and profile complete" to the query. |
| `PagedList<T>` | Lives in DAL (`Common/`) so both DAL and BLL can use it. |

### 5.5 Booking request

**Actors:** Tourist, Guide · **FRs:** FR-11, FR-12, FR-13, FR-15, FR-16 · **Rules:** BR-04 to BR-09, BR-11.

```mermaid
classDiagram
    direction LR
    class BookingsController {
        <<Tourist Area>>
        +Index() Task~IActionResult~
        +Create(int guideProfileId) Task~IActionResult~
        +Create(CreateBookingViewModel) Task~IActionResult~
        +PreviewPrice(CreateBookingViewModel) Task~IActionResult~
        +Withdraw(int id) Task~IActionResult~
    }
    class BookingRequestsController {
        <<Guide Area>>
        +Index() Task~IActionResult~
        +Accept(int id) Task~IActionResult~
        +Reject(int id) Task~IActionResult~
    }
    class IBookingService {
        <<interface>>
        +PreviewPriceAsync(CreateBookingRequest) Task~Result~PriceQuoteDto~~
        +CreateAsync(string touristId, CreateBookingRequest) Task~Result~BookingDto~~
        +AcceptAsync(string guideUserId, int bookingId) Task~Result~
        +RejectAsync(string guideUserId, int bookingId) Task~Result~
        +WithdrawAsync(string touristId, int bookingId) Task~Result~
        +GetForTouristAsync(string touristId) Task~List~BookingDto~~
        +GetForGuideAsync(string guideUserId) Task~List~BookingDto~~
    }
    class BookingService
    class CreateBookingValidator {
        <<FluentValidation>>
    }
    class IBookingStateMachine {
        <<interface>>
    }
    class IPriceCalculator {
        <<interface>>
    }
    class BookingRules {
        <<static>>
    }
    class TimeProvider {
        <<.NET 8>>
    }
    class IUnitOfWork {
        <<interface>>
    }
    class IBookingRepository {
        <<interface>>
        +HasOverlapAsync(int guideId, DateTime startUtc, DateTime endUtc, int? excludeId) Task~bool~
        +GetForTouristAsync(string touristId) Task~List~Booking~~
        +GetForGuideAsync(int guideId) Task~List~Booking~~
    }
    class IGuideProfileRepository {
        <<interface>>
    }
    class BookingMappingProfile {
        <<AutoMapper>>
    }
    class Booking

    BookingsController --> IBookingService
    BookingRequestsController --> IBookingService
    IBookingService <|.. BookingService
    BookingService --> CreateBookingValidator
    CreateBookingValidator --> TimeProvider : start at least 24 h ahead
    CreateBookingValidator ..> BookingRules : limits
    BookingService --> IBookingStateMachine : accept, reject, withdraw
    BookingService --> IPriceCalculator : billable hours, total
    BookingService --> TimeProvider
    BookingService --> IUnitOfWork
    BookingService ..> BookingMappingProfile : via IMapper
    IUnitOfWork --> IBookingRepository
    IUnitOfWork --> IGuideProfileRepository
    IBookingRepository ..> Booking
```

| Class | Responsibility |
| --- | --- |
| `BookingsController` | Tourist: request form, live price preview (FR-12), my bookings list, withdraw. |
| `BookingRequestsController` | Guide: incoming requests list (FR-13, FR-16), accept, reject. |
| `BookingService` | Checks the guide is bookable, copies the price, converts local times to UTC with the guide's `TimeZoneId`, checks overlap on accept, and changes status only through `IBookingStateMachine`. Saves with `RowVersion` so two changes can't collide. |
| `CreateBookingValidator` | Hourly: 2–10 hours, same local day. MultiDay: 2–14 days. Start ≥ now + 24 h. |

### 5.6 Payment

**Actor:** Tourist · **FR:** FR-14 · **Rules:** BR-07, BR-08, BR-12, NFR-04 (no card data stored).

```mermaid
classDiagram
    direction LR
    class PaymentsController {
        <<Tourist Area>>
        +Checkout(int bookingId) Task~IActionResult~
        +Pay(CheckoutViewModel) Task~IActionResult~
    }
    class IPaymentService {
        <<interface>>
        +GetCheckoutAsync(string touristId, int bookingId) Task~Result~CheckoutDto~~
        +PayAsync(string touristId, PaymentRequest) Task~Result~PaymentReceiptDto~~
    }
    class PaymentService
    class PaymentValidator {
        <<FluentValidation>>
    }
    class IPaymentGateway {
        <<interface>>
        +ChargeAsync(decimal amount, CardDetails card) Task~GatewayResult~
    }
    class SimulatedPaymentGateway
    class IBookingStateMachine {
        <<interface>>
    }
    class TimeProvider {
        <<.NET 8>>
    }
    class IUnitOfWork {
        <<interface>>
        +BeginTransactionAsync() Task~IDbContextTransaction~
        +SaveChangesAsync() Task~int~
    }
    class IPaymentRepository {
        <<interface>>
        +GetByBookingAsync(int bookingId) Task~Payment?~
    }
    class IBookingRepository {
        <<interface>>
    }
    class Payment
    class Booking

    PaymentsController --> IPaymentService
    IPaymentService <|.. PaymentService
    PaymentService --> PaymentValidator : card format only
    PaymentService --> IPaymentGateway
    IPaymentGateway <|.. SimulatedPaymentGateway
    PaymentService --> IBookingStateMachine : Accepted to Confirmed
    PaymentService --> TimeProvider : deadline check
    PaymentService --> IUnitOfWork : one transaction
    IUnitOfWork --> IPaymentRepository
    IUnitOfWork --> IBookingRepository
    PaymentService ..> Payment : creates
    Booking "1" *-- "0..1" Payment
```

| Class | Responsibility |
| --- | --- |
| `PaymentService` | Checks the booking is Accepted, owned by the tourist, and before the deadline; calls the gateway; saves `Payment` and sets `Confirmed` in one transaction. Card details are passed to the gateway and then dropped. |
| `SimulatedPaymentGateway` | Always succeeds and returns a new GUID as `TransactionReference`. A real gateway later is a new class behind `IPaymentGateway`. |
| `PaymentValidator` | Card number digits and length, expiry in the future, CVV 3–4 digits. |

### 5.7 Review

**Actor:** Tourist · **FRs:** FR-09, FR-19, FR-20 · **Rules:** BR-13 to BR-15.

```mermaid
classDiagram
    direction LR
    class ReviewsController {
        <<Tourist Area>>
        +Create(CreateReviewViewModel) Task~IActionResult~
    }
    class IReviewService {
        <<interface>>
        +CanReviewAsync(string touristId, int bookingId) Task~bool~
        +AddAsync(string touristId, CreateReviewRequest) Task~Result~
    }
    class ReviewService
    class CreateReviewValidator {
        <<FluentValidation>>
    }
    class IUnitOfWork {
        <<interface>>
    }
    class IReviewRepository {
        <<interface>>
        +ExistsForBookingAsync(int bookingId) Task~bool~
        +GetForGuideAsync(int guideId, int page) Task~PagedList~Review~~
    }
    class IBookingRepository {
        <<interface>>
    }
    class IGuideProfileRepository {
        <<interface>>
    }
    class ReviewMappingProfile {
        <<AutoMapper>>
    }
    class Review
    class Booking
    class GuideProfile {
        +decimal AverageRating
        +int ReviewCount
    }

    ReviewsController --> IReviewService
    IReviewService <|.. ReviewService
    ReviewService --> CreateReviewValidator
    ReviewService --> IUnitOfWork : one transaction
    ReviewService ..> ReviewMappingProfile : via IMapper
    IUnitOfWork --> IReviewRepository
    IUnitOfWork --> IBookingRepository
    IUnitOfWork --> IGuideProfileRepository
    ReviewService ..> Review : creates
    ReviewService ..> GuideProfile : updates rating
    Booking "1" -- "0..1" Review
```

| Class | Responsibility |
| --- | --- |
| `ReviewService` | Checks the booking is Completed, belongs to the tourist, and has no review yet. Saves the review and updates `AverageRating` and `ReviewCount` in one transaction. |
| `CreateReviewValidator` | Rating 1–5 whole number; comment ≤ 1,000 characters. |

### 5.8 Auto-cancel and auto-complete (System job)

**Actor:** System · **FRs:** FR-17, FR-18 · **Rules:** BR-06, BR-08, BR-10, NFR-06.

```mermaid
classDiagram
    direction LR
    class BookingStatusJob {
        <<BackgroundService>>
        -IServiceScopeFactory scopeFactory
        -IOptions~BookingJobOptions~ options
        #ExecuteAsync(CancellationToken) Task
    }
    class BookingJobOptions {
        <<Options>>
        +TimeSpan Interval
    }
    class IBookingStatusJobService {
        <<interface>>
        +RunAsync() Task~JobRunSummary~
    }
    class BookingStatusJobService
    class IBookingStateMachine {
        <<interface>>
    }
    class TimeProvider {
        <<.NET 8>>
    }
    class IUnitOfWork {
        <<interface>>
    }
    class IBookingRepository {
        <<interface>>
        +GetDueForAutoCancelAsync(DateTime nowUtc) Task~List~Booking~~
        +GetDueForCompletionAsync(DateTime nowUtc) Task~List~Booking~~
    }
    class JobRunSummary {
        +int CancelledNoResponse
        +int CancelledNotPaid
        +int Completed
    }
    class Booking

    BookingStatusJob --> BookingJobOptions
    BookingStatusJob --> IBookingStatusJobService : new scope each run
    IBookingStatusJobService <|.. BookingStatusJobService
    BookingStatusJobService --> IBookingStateMachine
    BookingStatusJobService --> TimeProvider
    BookingStatusJobService --> IUnitOfWork
    IUnitOfWork --> IBookingRepository
    IBookingRepository ..> Booking
    BookingStatusJobService ..> JobRunSummary : returns
```

| Class | Responsibility |
| --- | --- |
| `BookingStatusJob` | Runs every `Interval` (from `appsettings.json`). Creates a DI scope each run and logs the `JobRunSummary`. No rules here. |
| `BookingStatusJobService` | Pending older than 24 h → Cancelled (`NoGuideResponse`); Accepted past `PaymentDeadlineUtc` → Cancelled (`NotPaid`); Confirmed past `EndUtc` → Completed. |

---

## 6. Layer reference

### 6.1 DAL: repositories, Unit of Work, DbContext

```mermaid
classDiagram
    direction LR
    class AppDbContext {
        <<IdentityDbContext>>
        +DbSet~GuideProfile~ GuideProfiles
        +DbSet~GuideDocument~ GuideDocuments
        +DbSet~Language~ Languages
        +DbSet~Specialization~ Specializations
        +DbSet~GuideLanguage~ GuideLanguages
        +DbSet~GuideSpecialization~ GuideSpecializations
        +DbSet~Booking~ Bookings
        +DbSet~Payment~ Payments
        +DbSet~Review~ Reviews
        #OnModelCreating(ModelBuilder) void
    }
    class IRepository~T~ {
        <<interface>>
        +GetByIdAsync(int id) Task~T?~
        +AddAsync(T entity) Task
        +Update(T entity) void
        +FindAsync(Expression predicate) Task~List~T~~
        +ExistsAsync(Expression predicate) Task~bool~
    }
    class Repository~T~ {
        #AppDbContext Context
        #DbSet~T~ Set
    }
    class IGuideProfileRepository {
        <<interface>>
        +SearchAsync(GuideSearchQuery) Task~PagedList~GuideProfile~~
        +GetPendingAsync() Task~List~GuideProfile~~
        +GetWithDetailsAsync(int id) Task~GuideProfile?~
        +GetByUserIdAsync(string userId) Task~GuideProfile?~
    }
    class IGuideDocumentRepository {
        <<interface>>
        +GetByGuideAsync(int guideId) Task~List~GuideDocument~~
        +GetByTypeAsync(int guideId, DocumentType type) Task~GuideDocument?~
    }
    class IBookingRepository {
        <<interface>>
        +HasOverlapAsync(int guideId, DateTime startUtc, DateTime endUtc, int? excludeId) Task~bool~
        +GetForTouristAsync(string touristId) Task~List~Booking~~
        +GetForGuideAsync(int guideId) Task~List~Booking~~
        +GetDueForAutoCancelAsync(DateTime nowUtc) Task~List~Booking~~
        +GetDueForCompletionAsync(DateTime nowUtc) Task~List~Booking~~
    }
    class IPaymentRepository {
        <<interface>>
        +GetByBookingAsync(int bookingId) Task~Payment?~
    }
    class IReviewRepository {
        <<interface>>
        +GetForGuideAsync(int guideId, int page) Task~PagedList~Review~~
        +ExistsForBookingAsync(int bookingId) Task~bool~
    }
    class GuideProfileRepository
    class GuideDocumentRepository
    class BookingRepository
    class PaymentRepository
    class ReviewRepository
    class IUnitOfWork {
        <<interface>>
        +IGuideProfileRepository GuideProfiles
        +IGuideDocumentRepository GuideDocuments
        +IBookingRepository Bookings
        +IPaymentRepository Payments
        +IReviewRepository Reviews
        +IRepository~Language~ Languages
        +IRepository~Specialization~ Specializations
        +SaveChangesAsync() Task~int~
        +BeginTransactionAsync() Task~IDbContextTransaction~
    }
    class UnitOfWork {
        -AppDbContext context
    }

    IRepository~T~ <|.. Repository~T~
    IRepository~T~ <|-- IGuideProfileRepository
    IRepository~T~ <|-- IGuideDocumentRepository
    IRepository~T~ <|-- IBookingRepository
    IRepository~T~ <|-- IPaymentRepository
    IRepository~T~ <|-- IReviewRepository
    Repository~T~ <|-- GuideProfileRepository
    Repository~T~ <|-- GuideDocumentRepository
    Repository~T~ <|-- BookingRepository
    Repository~T~ <|-- PaymentRepository
    Repository~T~ <|-- ReviewRepository
    IGuideProfileRepository <|.. GuideProfileRepository
    IGuideDocumentRepository <|.. GuideDocumentRepository
    IBookingRepository <|.. BookingRepository
    IPaymentRepository <|.. PaymentRepository
    IReviewRepository <|.. ReviewRepository
    IUnitOfWork <|.. UnitOfWork
    UnitOfWork --> AppDbContext
    Repository~T~ --> AppDbContext
```

- Repositories never call `SaveChanges` and never return `IQueryable`. No `Delete` exists.
- `PagedList<T>` and `GuideSearchQuery` live in `TourGuide.DAL/Common` and `TourGuide.DAL/Queries`.

### 6.2 BLL shared classes: domain, infrastructure, results, options

```mermaid
classDiagram
    direction LR
    class IBookingStateMachine {
        <<interface>>
        +CanChange(BookingStatus from, BookingStatus to, Actor actor) bool
        +Apply(Booking booking, BookingStatus to, Actor actor, DateTime nowUtc, CancellationReason? reason) Result
    }
    class BookingStateMachine
    class IPriceCalculator {
        <<interface>>
        +BillableHours(BookingType type, int? hours, int? days) int
        +Total(int billableHours, decimal pricePerHour) decimal
    }
    class PriceCalculator
    class BookingRules {
        <<static>>
        +int MinHours$
        +int MaxHours$
        +int MinDays$
        +int MaxDays$
        +int HoursPerDay$
        +int MinAdvanceHours$
        +int PendingExpiryHours$
        +int PaymentWindowHours$
        +decimal MaxPricePerHour$
        +int MaxYearsOfExperience$
        +int MinRating$
        +int MaxRating$
    }
    class IFileStorage {
        <<interface>>
        +SaveAsync(FileUpload file, string folder) Task~string~
        +OpenReadAsync(string storedName, string folder) Task~Stream~
        +ReplaceAsync(string oldName, FileUpload file, string folder) Task~string~
    }
    class LocalFileStorage
    class IPaymentGateway {
        <<interface>>
        +ChargeAsync(decimal amount, CardDetails card) Task~GatewayResult~
    }
    class SimulatedPaymentGateway
    class ICountryProvider {
        <<interface>>
        +GetAll() List~CountryDto~
        +Exists(string code) bool
    }
    class CountryProvider
    class Result {
        +bool Succeeded
        +string? ErrorCode
        +string? Message
        +List~ValidationError~ Errors
        +Ok()$ Result
        +Fail(string code, string message)$ Result
        +Invalid(List~ValidationError~ errors)$ Result
    }
    class Result~T~ {
        +T? Value
    }
    class ValidationError {
        +string Field
        +string Message
    }
    class ErrorCodes {
        <<static>>
        +string NotFound$
        +string Forbidden$
        +string Validation$
        +string InvalidStatusChange$
        +string Overlap$
        +string PaymentDeadlinePassed$
        +string GuideNotBookable$
        +string AlreadyReviewed$
    }
    class Actor {
        <<enumeration>>
        Tourist
        Guide
        System
    }
    class FileStorageOptions {
        <<Options>>
        +string RootPath
        +long MaxFileSizeBytes
        +List~string~ AllowedContentTypes
    }
    class BookingJobOptions {
        <<Options>>
        +TimeSpan Interval
    }

    IBookingStateMachine <|.. BookingStateMachine
    IPriceCalculator <|.. PriceCalculator
    IFileStorage <|.. LocalFileStorage
    IPaymentGateway <|.. SimulatedPaymentGateway
    ICountryProvider <|.. CountryProvider
    Result <|-- Result~T~
    Result *-- ValidationError
    Result ..> ErrorCodes
    BookingStateMachine ..> Actor
    BookingStateMachine ..> BookingRules
    PriceCalculator ..> BookingRules
    LocalFileStorage --> FileStorageOptions
```

- `Actor` is a BLL enum used only by the state machine to know who asks for a change. It is not stored.
- `BookingRules` values: hours 2–10, days 2–14, 8 hours per day, 24 h advance, 24 h pending expiry, 24 h payment window, max price 1,000, max experience 60, rating 1–5.

### 6.3 Validators and AutoMapper profiles

```mermaid
classDiagram
    direction LR
    class AbstractValidator~T~ {
        <<FluentValidation>>
    }
    class RegisterTouristValidator
    class RegisterGuideValidator
    class RejectGuideValidator
    class ReapplyValidator
    class UpdateGuideProfileValidator
    class GuideSearchFilterValidator
    class CreateBookingValidator
    class PaymentValidator
    class CreateReviewValidator
    class FileUploadValidator

    AbstractValidator~T~ <|-- RegisterTouristValidator
    AbstractValidator~T~ <|-- RegisterGuideValidator
    AbstractValidator~T~ <|-- RejectGuideValidator
    AbstractValidator~T~ <|-- ReapplyValidator
    AbstractValidator~T~ <|-- UpdateGuideProfileValidator
    AbstractValidator~T~ <|-- GuideSearchFilterValidator
    AbstractValidator~T~ <|-- CreateBookingValidator
    AbstractValidator~T~ <|-- PaymentValidator
    AbstractValidator~T~ <|-- CreateReviewValidator
    AbstractValidator~T~ <|-- FileUploadValidator
    RegisterGuideValidator --> FileUploadValidator
    ReapplyValidator --> FileUploadValidator
    UpdateGuideProfileValidator --> FileUploadValidator
```

```mermaid
classDiagram
    direction LR
    class Profile {
        <<AutoMapper>>
    }
    class AccountMappingProfile
    class GuideMappingProfile
    class BookingMappingProfile
    class ReviewMappingProfile
    class LookupMappingProfile
    class AdminViewModelProfile
    class GuideViewModelProfile
    class TouristViewModelProfile
    class PublicViewModelProfile

    Profile <|-- AccountMappingProfile
    Profile <|-- GuideMappingProfile
    Profile <|-- BookingMappingProfile
    Profile <|-- ReviewMappingProfile
    Profile <|-- LookupMappingProfile
    Profile <|-- AdminViewModelProfile
    Profile <|-- GuideViewModelProfile
    Profile <|-- TouristViewModelProfile
    Profile <|-- PublicViewModelProfile
```

| Validator | Validates | Key rules |
| --- | --- | --- |
| `RegisterTouristValidator` | `RegisterTouristRequest` | Name 2–100, email, country in list |
| `RegisterGuideValidator` | `RegisterGuideRequest` | As tourist + 3 valid documents |
| `RejectGuideValidator` | `RejectGuideRequest` | Reason 10–500 |
| `ReapplyValidator` | `ReapplyRequest` | 3 valid documents |
| `UpdateGuideProfileValidator` | `UpdateGuideProfileRequest` | Price, experience, bio, lists, time zone, photo |
| `GuideSearchFilterValidator` | `GuideSearchFilter` | Min ≤ max price, page ≥ 1 |
| `CreateBookingValidator` | `CreateBookingRequest` | Hours/days by type, start ≥ now + 24 h, same-day end |
| `PaymentValidator` | `PaymentRequest` | Card format only |
| `CreateReviewValidator` | `CreateReviewRequest` | Rating 1–5, comment ≤ 1,000 |
| `FileUploadValidator` | `FileUpload` | PDF/JPG/PNG by content signature, ≤ 5 MB |

| Profile | Project | Maps |
| --- | --- | --- |
| `AccountMappingProfile` | BLL | Register requests → `ApplicationUser`, `GuideProfile` |
| `GuideMappingProfile` | BLL | `GuideProfile`, `GuideDocument` ↔ guide DTOs |
| `BookingMappingProfile` | BLL | `Booking` → booking DTOs; `CreateBookingRequest` → `Booking` (no price or status) |
| `ReviewMappingProfile` | BLL | `Review` ↔ review DTOs |
| `LookupMappingProfile` | BLL | `Language`, `Specialization` → `LookupDto` |
| `AdminViewModelProfile` | Web | Admin area DTOs ↔ ViewModels |
| `GuideViewModelProfile` | Web | Guide area DTOs ↔ ViewModels |
| `TouristViewModelProfile` | Web | Tourist area DTOs ↔ ViewModels |
| `PublicViewModelProfile` | Web | Home, account, search, public profile |

### 6.4 Web cross-cutting classes

```mermaid
classDiagram
    direction LR
    class ICurrentUserService {
        <<interface>>
        +string? UserId
        +IsInRole(string role) bool
    }
    class CurrentUserService {
        -IHttpContextAccessor accessor
    }
    class ModelStateExtensions {
        <<static>>
        +AddResultErrors(ModelStateDictionary, Result)$ void
    }
    class ValidationAttribute {
        <<ASP.NET>>
    }
    class IClientModelValidator {
        <<interface>>
        +AddValidation(ClientModelValidationContext) void
    }
    class MaxFileSizeAttribute {
        +long MaxBytes
    }
    class AllowedFileTypesAttribute {
        +string[] Extensions
    }
    class FutureDateAttribute {
        +int MinHoursAhead
    }
    class BookingStatusJob {
        <<BackgroundService>>
    }
    class BackgroundService {
        <<.NET>>
    }

    ICurrentUserService <|.. CurrentUserService
    ValidationAttribute <|-- MaxFileSizeAttribute
    ValidationAttribute <|-- AllowedFileTypesAttribute
    ValidationAttribute <|-- FutureDateAttribute
    IClientModelValidator <|.. MaxFileSizeAttribute
    IClientModelValidator <|.. AllowedFileTypesAttribute
    IClientModelValidator <|.. FutureDateAttribute
    BackgroundService <|-- BookingStatusJob
```

- Each custom attribute has a matching jQuery adapter in `wwwroot/js/validation.js`, so the same rule runs in the browser and on the server.
- `ModelStateExtensions.AddResultErrors` copies a failed `Result`'s field errors into `ModelState`, so FluentValidation errors appear under the right field.

---

## 7. Catalog

### 7.1 DTOs (BLL)

| Feature | Request DTOs (input) | Response DTOs (output) |
| --- | --- | --- |
| Account | `RegisterTouristRequest`, `RegisterGuideRequest` | — |
| Verification | `RejectGuideRequest`, `ReapplyRequest` | `PendingGuideDto`, `GuideReviewDto`, `GuideStatusDto`, `GuideDocumentDto` |
| Profile | `UpdateGuideProfileRequest` | `GuideProfileEditDto`, `PublicGuideProfileDto`, `ReviewDto` |
| Search | `GuideSearchFilter` | `GuideCardDto`, `SearchFilterOptionsDto`, `LookupDto`, `CountryDto` |
| Booking | `CreateBookingRequest` | `BookingDto`, `PriceQuoteDto` |
| Payment | `PaymentRequest` (contains `CardDetails`) | `CheckoutDto`, `PaymentReceiptDto`, `GatewayResult` |
| Review | `CreateReviewRequest` | `ReviewDto` |
| Shared | `FileUpload` (name, content type, stream, size) | `FileContentDto`, `JobRunSummary` |

### 7.2 ViewModels (Web)

| Area | ViewModels | Screen (SRS section 8) |
| --- | --- | --- |
| Public | `RegisterTouristViewModel`, `RegisterGuideViewModel`, `LoginViewModel` | Register, Log in |
| Public | `SearchViewModel`, `GuideCardViewModel` | Guide list |
| Public | `PublicGuideProfileViewModel` | Public guide profile |
| Admin | `PendingGuidesViewModel`, `GuideReviewViewModel`, `RejectGuideViewModel` | Admin screens |
| Guide | `GuideDashboardViewModel`, `EditGuideProfileViewModel`, `GuideStatusViewModel`, `ReapplyViewModel` | Guide dashboard, profile edit |
| Tourist | `CreateBookingViewModel`, `MyBookingsViewModel`, `CheckoutViewModel`, `CreateReviewViewModel` | Booking form, My Bookings, Payment, Review |

### 7.3 EF Core configurations (DAL)

One `IEntityTypeConfiguration<T>` per entity in `Data/Configurations/`.

| Configuration | Keys and indexes | Constraints |
| --- | --- | --- |
| `ApplicationUserConfiguration` | Identity defaults | FullName ≤ 100, CountryCode char(2) |
| `GuideProfileConfiguration` | Unique `UserId`; index (`VerificationStatus`, `CountryCode`) | Price `decimal(10,2)` 0–1,000; experience 0–60; rating `decimal(3,2)` |
| `GuideDocumentConfiguration` | Unique (`GuideProfileId`, `DocumentType`) | Size > 0 and ≤ 5,242,880 |
| `LanguageConfiguration` | Unique `Name` | Name ≤ 50 |
| `SpecializationConfiguration` | Unique `Name` | Name ≤ 50 |
| `GuideLanguageConfiguration` | Key (`GuideProfileId`, `LanguageId`) | — |
| `GuideSpecializationConfiguration` | Key (`GuideProfileId`, `SpecializationId`) | — |
| `BookingConfiguration` | Index (`GuideProfileId`, `Status`, `StartUtc`); index `TouristId`; `RowVersion` | Hours 2–10 or null; Days 2–14 or null; money `decimal(10,2)` |
| `PaymentConfiguration` | Unique `BookingId`; unique `TransactionReference` | Amount `decimal(10,2)` |
| `ReviewConfiguration` | Unique `BookingId`; index `GuideProfileId` | Rating 1–5; Comment ≤ 1,000 |

All relationships use `DeleteBehavior.Restrict`, because nothing is deleted.
