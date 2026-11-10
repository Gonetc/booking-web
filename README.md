# Booking Web

[![CI](https://github.com/Gonetc/booking-web/actions/workflows/ci.yml/badge.svg)](https://github.com/Gonetc/booking-web/actions/workflows/ci.yml) ![.NET 10](https://img.shields.io/badge/.NET-10-512BD4) ![License: MIT](https://img.shields.io/badge/license-MIT-green)

Online booking website and admin panel for a barbershop / beauty salon, built with **Blazor (.NET 10)**.
It is the web front end of a small product and talks to the [Booking API](https://github.com/Gonetc/booking-api).

```
 Client ─┐
         ├──► Booking Web (Blazor Server) ──► Booking API ──► PostgreSQL
 Admin ──┘
```

## Features

### For clients

- **Home page**: salon services with duration and price, team of masters
- **4-step booking wizard**: service → master → date & time → contact details
  - only masters who provide the selected service are shown
  - only the master's working days are offered
  - free time slots come live from the API
  - client-side validation of name and phone
  - if someone books the same slot a moment earlier, the user sees a clear message and refreshed times
- **Deep links**: `/book?serviceId=1` opens the wizard with a service preselected
- **Graceful degradation**: a friendly message instead of a crash when the API is unreachable
- Responsive layout, no CSS framework

### For the salon administrator (`/admin`)

- **Sign in** with the API's JWT; the token is kept in encrypted browser session storage
  (ASP.NET Core Data Protection), survives page reloads, and expired sessions redirect to the login page
- **Daily schedule**: bookings for any date, number of bookings, expected revenue, cancellations;
  cancel a booking with a two-step confirmation
- **Masters**: add and edit masters, choose the services they provide, edit the weekly schedule, deactivate
- **Services**: add and edit services, durations and prices, hide services from clients

## Tech stack

| Area | Technology |
|---|---|
| UI | Blazor Web App, Interactive Server rendering |
| API access | Typed `HttpClient`, `System.Net.Http.Json` |
| Forms | `EditForm` + DataAnnotations validation |
| Admin auth | JWT from the API, `ProtectedSessionStorage` |
| Styling | Hand-written CSS with design tokens |

Because Blazor Server calls the API from the server, the API address is never exposed to the browser
and the API does not need CORS.

## Getting started

1. Start the [Booking API](https://github.com/Gonetc/booking-api) (it listens on `http://localhost:8080`):
   ```bash
   cd ../booking-api
   docker compose up -d
   ```
2. Run the website:
   ```bash
   dotnet run --project src/BookingWeb
   ```
3. Open <http://localhost:5090>.
4. Admin panel: <http://localhost:5090/admin> — user `admin`, password `admin12345`
   (development credentials of the API).

## Configuration

| Key | Default | Description |
|---|---|---|
| `BookingApi:BaseUrl` | `http://localhost:8080/` | Booking API address |
| `Salon:Name`, `Tagline`, `Address`, `Phone` | demo values | Salon details shown on the site |
| `Salon:Currency` | `$` | Currency symbol for prices |
| `Salon:BookingDaysAhead` | `14` | How far ahead clients can book |

## Project structure

```
src/BookingWeb
├── Components/
│   ├── Admin/AdminPageBase.cs    shared logic of admin pages (errors, expired session)
│   ├── Layout/                   MainLayout (site), AdminLayout (sidebar + auth guard)
│   └── Pages/                    Home, Book (wizard), AdminLogin, NotFound, Error
│       └── Admin/                Schedule, Masters, Services
├── Models/                       API contracts, booking form
├── Services/                     API clients, admin session, formatting, salon settings
└── wwwroot/app.css               styles
```

## Roadmap

- [ ] Component tests with bUnit
- [ ] Docker Compose for the whole product

## License

MIT
