# Tour Guide Booking — SRS

## 1. Introduction

The system is a web application where tourists find verified tour guides and book them for a few hours on one day or for several days. Guides are approved by an admin after a document review. A booking is confirmed only after the guide accepts the request and the tourist pays through a simulated payment.

- **Platform:** web, built with ASP.NET Core MVC
- **Language of the interface:** English only (first release)

## 2. Actors

The system has three human actors and one system actor.

| Actor | Who they are | Main goals |
| --- | --- | --- |
| Tourist | A traveler from any country | Find a guide, send a booking request, pay, review the guide after the trip |
| Tour Guide | A person offering guiding services | Register with documents, get approved, manage a profile, accept or reject requests |
| Admin | A platform staff member | Review guide documents and approve or reject guides |
| System (scheduled job) | Automatic background process | Cancel unpaid requests after the payment deadline, mark trips as completed when they end |

## 3. Functional Requirements

20 requirements across six areas. Each one is in scope for the 3-week MVP.

### 3.1 Accounts and registration

| ID | Requirement | Actor |
| --- | --- | --- |
| FR-01 | The system shall let a tourist create an account. | Tourist |
| FR-02 | The system shall let a guide create an account and upload three documents: proof of residence, government ID, and criminal record certificate (in Egypt: national ID and فيش وتشبيه). | Guide |
| FR-03 | The system shall let all users log in and log out, and show each user only the pages of their role. | All |

### 3.2 Guide verification

| ID | Requirement | Actor |
| --- | --- | --- |
| FR-04 | The system shall show the admin a list of guides waiting for review. | Admin |
| FR-05 | The system shall let the admin view the documents a guide uploaded. | Admin |
| FR-06 | The system shall let the admin approve a guide, or reject them with a written reason. | Admin |
| FR-07 | The system shall show a guide their current review status (pending, approved, rejected), and let a rejected guide upload new documents to apply again. | Guide |

### 3.3 Guide profile

| ID | Requirement | Actor |
| --- | --- | --- |
| FR-08 | The system shall let an approved guide set their country, city, languages, specializations, price per hour (USD), short bio, photo, and years of experience. | Guide |
| FR-09 | The system shall show a public guide profile with the guide's details, average rating, and reviews. | Tourist |

### 3.4 Search

| ID | Requirement | Actor |
| --- | --- | --- |
| FR-10 | The system shall let a tourist search approved guides and filter by country, language, price per hour (exact value or range), and specialization. | Tourist |

### 3.5 Booking and payment

| ID | Requirement | Actor |
| --- | --- | --- |
| FR-11 | The system shall let a tourist send a booking request to a guide for either a number of hours on one day or a range of several days. | Tourist |
| FR-12 | The system shall show the tourist the total price before the request is sent. | Tourist |
| FR-13 | The system shall let a guide view incoming requests and accept or reject each one. | Guide |
| FR-14 | The system shall let the tourist pay an accepted request through a simulated payment. | Tourist |
| FR-15 | The system shall show the tourist a list of their bookings with the current status of each, and let the tourist withdraw a request before it is paid. | Tourist |
| FR-16 | The system shall show the guide a list of their requests and bookings with the current status of each. | Guide |
| FR-17 | The system shall automatically cancel a Pending request with no answer after 24 hours, and an Accepted request that is not paid before the payment deadline. | System |
| FR-18 | The system shall automatically mark a confirmed booking as completed when the trip end time passes. | System |

### 3.6 Reviews

| ID | Requirement | Actor |
| --- | --- | --- |
| FR-19 | The system shall let a tourist submit a star rating and a written comment for a completed booking. | Tourist |
| FR-20 | The system shall calculate each guide's average rating from their reviews. | System |

## 4. Business Rules

A booking is confirmed only after the guide accepts and the tourist pays. Reviews open only after the trip ends.

See the booking state diagram: `docs/class-diagram.md` → 4.1 Booking status.

Dashed boxes are end states. A booking becomes Cancelled when a Pending request gets no answer in 24 hours, an Accepted one is not paid in time, or the tourist withdraws.

### 4.1 Guides

| ID | Rule |
| --- | --- |
| BR-01 | A guide cannot use the website as a guide until an admin approves their documents. |
| BR-02 | An unapproved or rejected guide does not appear in search results and cannot receive booking requests. |
| BR-03 | Only the admin can view a guide's uploaded documents. Tourists and other guides cannot. |

### 4.2 Bookings

| ID | Rule |
| --- | --- |
| BR-04 | A booking request covers either 2 to 10 hours on one day, or 2 to 14 full days. The trip must start at least 24 hours after the request is sent. |
| BR-05 | A booking has one of six statuses: Pending, Accepted, Confirmed, Completed, Rejected, Cancelled. |
| BR-06 | A new request starts as Pending. The guide moves it to Accepted or Rejected. With no answer after 24 hours, it becomes Cancelled. |
| BR-07 | The tourist can pay only after the guide accepts. Payment moves the booking to Confirmed. Before payment, the tourist can withdraw the request, which makes it Cancelled. |
| BR-08 | The payment deadline is 24 hours after acceptance, or the trip start time, whichever comes first. An Accepted booking that is unpaid at the deadline becomes Cancelled. |
| BR-09 | A guide cannot have two Accepted or Confirmed bookings that overlap in time. |
| BR-10 | A Confirmed booking becomes Completed when its trip end time passes. |
| BR-11 | The total price equals hours times the guide's price per hour, and a full day counts as 8 hours. The price is saved when the request is sent. |
| BR-12 | Payment is simulated. No real money moves and no real card details are collected. |

### 4.3 Reviews

| ID | Rule |
| --- | --- |
| BR-13 | Only the tourist who made a booking can review it. |
| BR-14 | A review can be written only after the booking is Completed. |
| BR-15 | A tourist can write one review per booking. |

## 5. Non-functional Requirements

| ID | Category | Requirement |
| --- | --- | --- |
| NFR-01 | Security | Each user can reach only the pages and data of their own role. |
| NFR-02 | Security | Guide documents are stored outside the public web folder and are served only to logged-in admins. |
| NFR-03 | Security | Passwords are stored hashed, never as plain text. |
| NFR-04 | Security | The simulated payment does not collect or store real card details. |
| NFR-05 | Data integrity | A booking can change status only along the allowed transitions in BR-05 to BR-10. |
| NFR-06 | Reliability | A scheduled background job applies the payment deadline and trip completion without any user action. |
| NFR-07 | Usability | The interface is in English. |
| NFR-08 | Maintainability | The system is built with ASP.NET Core MVC. The team works in Git with one branch per module. |

## 6. Out of Scope

These features are not part of the 3-week release. They can go in the report's Future Work section.

- Chat or messaging between tourist and guide
- Real payment gateway and refunds
- Cancellation of a Confirmed booking by the tourist or the guide
- Guide availability calendar
- Email or in-app notifications
- Languages other than English, including Arabic
- Guide replies to reviews, and admin moderation of reviews
- Admin statistics dashboard
- Fixed-price tour packages and custom group tours (the UI's "Explore Extraordinary Tours" and "Design Your Group" screens)
- Suspending a guide after approval
- Mobile app

## 7. Assumptions

The open questions are now answered with assumptions, so coding can start. Each row is an assumption, not a confirmed decision: change any row and update the FRs and BRs it lists.

### Pricing and booking

| Topic | Assumption | Affects |
| --- | --- | --- |
| Multi-day price | A full day counts as 8 hours. Total = days × 8 × price per hour. | FR-12, BR-11 |
| Price changes | The request saves the price when it is sent. Later price changes do not affect it. | BR-11 |
| Request limits | One day: 2 to 10 hours. Several days: 2 to 14 days. | FR-11, BR-04 |
| Advance notice | The trip must start at least 24 hours after the request is sent. | BR-04 |
| Unanswered requests | A Pending request with no answer after 24 hours becomes Cancelled. | FR-17, BR-06 |
| Overlapping requests | A guide cannot accept a request that overlaps another Accepted or Confirmed booking. | BR-09 |
| Withdrawing | A tourist can withdraw a request while it is Pending or Accepted (before payment). It becomes Cancelled. | FR-15, BR-07 |
| Time zone | Trip times use the guide's local time, shown with the time zone name. | FR-11 |
| Currency | All prices are in US dollars (USD), as in the UI. | FR-08, FR-12 |

### Guides and admin

| Topic | Assumption | Affects |
| --- | --- | --- |
| Rejection reason | The admin must write a reason. The guide sees it. | FR-06 |
| Re-applying | A rejected guide can upload new documents. Their status goes back to Pending. | FR-07 |
| Profile fields | The profile also has city, short bio, photo, and years of experience. | FR-08 |
| Document files | PDF, JPG, or PNG, up to 5 MB each. | FR-02 |
| Admin accounts | Admin accounts are seeded in the database. There is no public admin sign-up. | FR-03 |
| Guides outside Egypt | Guides can be from any country. They upload their own country's ID, proof of residence, and criminal record certificate. In Egypt that is the national ID and فيش وتشبيه. The country filter stays. | FR-02, FR-10 |

### Accounts and reviews

| Topic | Assumption | Affects |
| --- | --- | --- |
| Tourist sign-up | Full name, email, password, and country. | FR-01 |
| Rating scale | 1 to 5 whole stars. The written comment is optional. | FR-19 |

### Technical

| Topic | Assumption | Affects |
| --- | --- | --- |
| Framework | ASP.NET Core MVC. The scheduled job is a `BackgroundService`. | NFR-06, NFR-08 |
| Screen sizes | Desktop is required, because the UI is designed for desktop. A phone layout is a nice-to-have. | NFR-07 |

## 8. UI Screens

The 8 designed screens cover search, booking, payment, and the dashboards. Two screens are about tour packages, which are out of scope. Seven screens are still missing, mostly for the admin and the guide profile. Screen names come from the page titles in the design.

### 8.1 Designed screens

| # | Screen (title in design) | Use in this project | FRs | Changes needed |
| --- | --- | --- | --- | --- |
| 1 | Home: "Discover the World Here Your Way" | Landing page | FR-10 | Change "Featured Tours" to top-rated guides |
| 2 | Register: "Begin Your Next Great Adventure" | Sign-up with a Tourist or Guide choice | FR-01, FR-02 | Add the 3 document uploads to the guide path |
| 3 | Guide dashboard: "Welcome back" | Incoming requests with accept and decline | FR-13, FR-16 | Replace the tours section with upcoming bookings; show the review status |
| 4 | Payment: "Finalize Your Journey" | Simulated payment for an Accepted booking | FR-14 | Mark it as test mode; never save card fields (NFR-04) |
| 5 | Guide list: "Meet Your Expert Guides" | Search results | FR-10 | Add the filter sidebar from screen 8 |
| 6 | "My Bookings" | Tourist bookings with status | FR-15, FR-19 | Add Withdraw on Pending and Accepted; add Review on Completed |
| 7 | "Design Your Group" | Not used (custom group tours are out of scope) | — | Its layout can be reused for the booking request form |
| 8 | "Explore Extraordinary Tours" | Not used (tour packages are out of scope) | — | Move its filter sidebar to screen 5 |

### 8.2 Missing screens

| Screen | FRs |
| --- | --- |
| Log in (unless it is a tab of screen 2) | FR-03 |
| Public guide profile with reviews and a Book button | FR-09 |
| Guide profile edit | FR-08 |
| Booking request form with the price shown | FR-11, FR-12 |
| Review form (can be a pop-up on screen 6) | FR-19 |
| Admin: guides waiting for review | FR-04 |
| Admin: one guide's documents with Approve and Reject (with reason) | FR-05, FR-06 |
