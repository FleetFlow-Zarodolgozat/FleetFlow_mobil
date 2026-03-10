# FleetFlow – Mobile Driver App

A cross-platform mobile application built with **.NET MAUI** for fleet management. The app is designed for **drivers** and connects to the FleetFlow REST API backend. It allows drivers to manage trips, log fuel, submit service requests, view notifications, and keep their profiles up to date — all from a single, polished mobile app.

[![.NET CI](https://github.com/FleetFlow-Zarodolgozat/FleetFlow_mobil/actions/workflows/dotnet.yml/badge.svg)](https://github.com/FleetFlow-Zarodolgozat/FleetFlow_mobil/actions/workflows/dotnet.yml)

---

## Table of Contents

- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [App Pages](#app-pages)
  - [Login Page](#1-login-page)
  - [Dashboard Page](#2-dashboard-page)
  - [Trip Page](#3-trip-page)
  - [Fuel Page](#4-fuel-page)
  - [Service Page](#5-service-page)
  - [Notification Page](#6-notification-page)
  - [Profile Page](#7-profile-page)
- [Popups](#popups)
  - [Forgot Password Popup](#forgot-password-popup)
  - [Service Details Popup](#service-details-popup)
  - [Calendar Day Popup](#calendar-day-popup)
- [Navigation](#navigation)
- [Services](#services)
- [Models](#models)
- [Authentication & Session Management](#authentication--session-management)
- [Theme Support](#theme-support)
- [Dependencies](#dependencies)

---

## Technology Stack

| Technology | Version | Purpose |
|---|---|---|
| .NET MAUI | net10.0 | Cross-platform UI framework |
| C# | 12+ | Application logic |
| XAML | — | UI layout definition |
| CommunityToolkit.Maui | 14.0.1 | Popups, converters, extensions |
| CommunityToolkit.Mvvm | 8.4.0 | MVVM source generators, commands |
| Plugin.Maui.Calendar | 3.0.1 | Interactive calendar component |
| SkiaSharp | 3.119.2 | 2D graphics rendering |
| Microsoft.Extensions.Http | 10.0.4 | Typed HttpClient DI |
| FontAwesome | — | Icon fonts (Solid & Regular) |

**Supported platforms:** Android (API 21+), iOS (15.0+), macOS Catalyst (15.0+), Windows (10.0.17763+)

**Backend API:** `http://fleetflow-zarodolgozat-backend-ressdominik.jcloud.jedlik.cloud/api/`

---

## Project Structure

```
FleetFlow_mobil/
└── mobil/
    └── mobil/
        ├── App.xaml / App.xaml.cs          # Application entry point
        ├── AppShell.xaml / AppShell.xaml.cs # Shell navigation routes
        ├── MauiProgram.cs                  # Dependency injection setup
        ├── Pages/
        │   ├── LoginPage.xaml(.cs)
        │   ├── DashboardPage.xaml(.cs)
        │   ├── TripPage.xaml(.cs)
        │   ├── FuelPage.xaml(.cs)
        │   ├── ServicePage.xaml(.cs)
        │   ├── NotificationPage.xaml(.cs)
        │   ├── ProfilePage.xaml(.cs)
        │   └── Components/
        │       └── BottomNavigation.xaml(.cs)
        ├── Popups/
        │   ├── ForgotPasswordPopup.xaml(.cs)
        │   ├── ServiceDetailsPopup.xaml(.cs)
        │   └── CalendarDayPopup.xaml(.cs)
        ├── ViewModels/
        │   ├── LoginViewModel.cs
        │   ├── ForgotPasswordViewModel.cs
        │   ├── DashboardViewModel.cs
        │   ├── TripViewModel.cs
        │   ├── FuelViewModel.cs
        │   ├── ServiceViewModel.cs
        │   ├── ServiceDetailsViewModel.cs
        │   ├── NotificationViewModel.cs
        │   ├── ProfileViewModel.cs
        │   ├── CalendarDayViewModel.cs
        │   └── BottomNavigationViewModel.cs
        ├── Services/
        │   ├── AuthService.cs
        │   ├── DashboardService.cs
        │   ├── TripService.cs
        │   ├── FuelService.cs
        │   ├── ServiceService.cs
        │   ├── NotificationService.cs
        │   ├── ProfileService.cs
        │   ├── SessionService.cs
        │   └── ThemeService.cs
        ├── Models/
        │   ├── Login.cs
        │   ├── Profile.cs        # Driver, Vehicle, Stats, EditProfileData
        │   ├── Trip.cs           # Trip, TripCreate
        │   ├── Fuel.cs           # Fuel, FuelCreate
        │   ├── Service.cs        # Service, ServiceCreate, ServiceDetailUpload
        │   ├── Notification.cs
        │   ├── Calendarevent.cs
        │   └── PagedResponse.cs
        ├── Handlers/
        │   └── AuthHttpHandler.cs  # Adds Bearer token to every request
        └── Converters/             # Value converters for XAML bindings
```

---

## Architecture

The app follows the **MVVM (Model-View-ViewModel)** pattern using `CommunityToolkit.Mvvm` source generators:

- **Models** – Plain C# classes representing API data (Driver, Trip, Fuel, etc.)
- **Services** – Typed `HttpClient` wrappers that communicate with the REST API
- **ViewModels** – Expose data and commands to the UI via `ObservableObject` and `[RelayCommand]`
- **Views (Pages/Popups)** – XAML-based UI that binds to ViewModels
- **Dependency Injection** – All services, ViewModels, and pages are registered in `MauiProgram.cs`

All authenticated API requests are intercepted by `AuthHttpHandler`, which automatically attaches the JWT Bearer token from `SessionService` (stored in `SecureStorage`).

---

## App Pages

### 1. Login Page

**Route:** `LoginPage` (default / startup page)

The entry point of the application. Drivers sign in using their email and password credentials.

**UI Elements:**
- Blue header with FleetFlow logo (car icon) and app title
- Email entry field with email icon
- Password entry field with lock icon and toggle show/hide button
- Error message banner (visible on failed login)
- "Forgot Password?" button — opens the **Forgot Password Popup**
- "Sign In" button — triggers authentication
- Loading indicator during login
- Footer copyright label

**Functionality:**
- Sends a `POST /login-mobile` request with email and password
- On success: saves the JWT token to `SecureStorage` and navigates to `DashboardPage`
- On failure: displays a user-friendly error message
- Password visibility toggle (eye icon)
- Form disabled during loading to prevent duplicate submissions

---

### 2. Dashboard Page

**Route:** `DashboardPage`

The main home screen displayed after login. Provides a complete overview of the driver's status, assigned vehicle, personal statistics, and upcoming scheduled events.

**UI Elements:**
- Top bar with FleetFlow logo, notification bell (with unread badge), and profile avatar
- Welcome message with driver's name
- **Driver Profile Card**: name, role badge, email, phone, license number, license expiry date, edit shortcut
- **Quick Action Buttons**: "New Trip" and "New Fuel Log" (shortcuts to create forms)
- **My Vehicle Card**: brand/model, license plate, year, VIN, current mileage (km), status badge
- **My Statistics** grid (6 stat cards):
  - Total Trips
  - Total Distance (km)
  - Total Services
  - Total Service Cost (Ft)
  - Total Fuels
  - Total Fuel Cost (Ft)
- **Interactive Calendar**: displays events (trips, services) as colored dots; tapping a day opens the **Calendar Day Popup**
- Loading indicator and error state

**Functionality:**
- Loads driver profile, assigned vehicle, personal statistics, and calendar events from the API on page appear
- Notification bell shows a red badge dot when there are unread notifications
- Tapping the bell navigates to the Notification page
- Tapping the avatar navigates to the Profile page
- Tapping the edit icon (pencil) on the profile card navigates to Profile page in edit mode
- "New Trip" shortcut navigates to Trip page with the create form pre-opened
- "New Fuel Log" shortcut navigates to Fuel page with the create form pre-opened
- Tapping a calendar day opens a popup listing that day's events

---

### 3. Trip Page

**Route:** `TripPage`

Allows drivers to log, view, and delete their trips.

**UI Elements:**
- Top bar with truck icon, "Trip Log" title, and "+ New" button
- Expandable **New Trip Form** (toggled by the "+ New" button):
  - Start Date and Start Time pickers
  - End Date and End Time pickers
  - Start Location text entry
  - End Location text entry
  - Distance (km) numeric entry
  - Start Odometer (km) numeric entry
  - End Odometer (km) numeric entry
  - Notes text area (optional)
  - "Cancel" and "Create Trip" buttons
- Success and error message banners
- "MY TRIPS" section header
- Empty state illustration and message (when no trips exist)
- **Trip card list** — each card displays:
  - Route: Start Location → End Location
  - Date and duration
  - Distance (km)
  - License plate
  - Notes (if any)
  - Delete button (with confirmation dialog)
- **Pagination controls** (Prev / Page X of Y / Next), showing total entry count

**Functionality:**
- Fetches paginated trips from the API (10 items per page)
- Creates a new trip by submitting a form with start/end times, locations, distance, and odometer readings
- Deletes a trip after user confirms via an alert dialog
- Navigates between pages using Previous/Next buttons
- Can be opened with the create form pre-expanded (e.g., from Dashboard quick action)

---

### 4. Fuel Page

**Route:** `FuelPage`

Allows drivers to log, view, and delete fuel fill-ups, including optional receipt photo upload.

**UI Elements:**
- Top bar with gas pump icon, "Fuel Log" title, and "+ New" button
- Expandable **New Fuel Entry Form**:
  - Date and Time pickers
  - Odometer reading (km) numeric entry
  - Liters numeric entry
  - Total Cost numeric entry
  - Station Name text entry (optional)
  - Location text entry (optional)
  - Receipt photo section: "Pick from Gallery" and "Take Photo" buttons
  - Receipt photo preview with confirmation indicator
  - "Cancel" and "Log Fuel" buttons
- Success and error message banners
- "MY FUEL LOGS" section header
- Empty state message (when no logs exist)
- **Fuel card list** — each card displays:
  - Date
  - Fuel amount (liters) and total cost
  - Station name (if provided)
  - License plate
  - Receipt photo thumbnail (if uploaded)
  - Delete button (with confirmation dialog)
- **Pagination controls** (Prev / Page X of Y / Next)

**Functionality:**
- Fetches paginated fuel logs from the API (10 items per page)
- Creates a new fuel log; optionally attaches a receipt image (from gallery or camera)
- Uploads receipt photo as multipart form data to the API
- Deletes a fuel log after user confirmation
- Can be opened with the create form pre-expanded (e.g., from Dashboard quick action)

---

### 5. Service Page

**Route:** `ServicePage`

Enables drivers to submit, view, update details of, and delete vehicle service requests.

**UI Elements:**
- Top bar with wrench icon, "Service Requests" title, and purple "+ New" button
- Expandable **New Service Request Form**:
  - Title text entry (e.g., "Oil change needed")
  - Description text area (optional, up to 500 characters)
  - "Cancel" and "Create Request" buttons
- Success and error message banners
- "MY SERVICE REQUESTS" section header
- Empty state message
- **Service card list** — each card displays:
  - Purple top accent bar
  - Title and license plate
  - Status badge (color-coded: Pending / In Progress / Completed / etc.)
  - Description (truncated to 2 lines)
  - Scheduled start date
  - Driver-reported cost (Ft)
  - "Details" button — opens the **Service Details Popup**
  - Delete button (with confirmation dialog)
- **Pagination controls** (Prev / Page X of Y / Next)

**Functionality:**
- Fetches paginated service requests from the API (10 items per page)
- Creates a new service request with a title and optional description
- Opens the Service Details Popup for editing cost, notes, and uploading an invoice photo
- Deletes a service request after user confirmation
- Status is displayed with appropriate icons and color coding

---

### 6. Notification Page

**Route:** `NotificationPage`

Shows all system notifications sent to the driver (e.g., trip updates, service status changes).

**UI Elements:**
- Top bar with "FleetFlow" title, "Notifications" subtitle, and "Mark all read" button
- Loading indicator
- "NOTIFICATIONS / Your latest updates" header
- **Notification card list** — each card displays:
  - Type icon (color-coded by notification type)
  - Title (bold) with an unread blue dot indicator
  - Message body (up to 2 lines)
  - Type badge (color matches the icon)
  - Timestamp (yyyy.MM.dd HH:mm)
  - Delete button (red trash icon)
- Empty state (bell icon + "No notifications / You're all caught up!")

**Functionality:**
- Loads all notifications from the API on page appear
- Notifications are visually differentiated by type (different icon colors and badge labels)
- Unread notifications show a blue dot next to the title
- "Mark all read" button marks every notification as read
- Individual notifications can be deleted with the trash icon button

---

### 7. Profile Page

**Route:** `ProfilePage`

Displays the driver's personal profile and allows editing of personal data, password, and profile photo. Also provides theme settings and logout.

**UI Elements:**
- **View Mode:**
  - Large circular profile avatar (photo or FA icon fallback)
  - Full name
  - Role badge
  - Email, phone, license number, license expiry date info rows
  - "Edit Profile" button
- **Edit Mode** (toggled by Edit button):
  - Full Name text entry
  - Phone Number text entry
  - New Password entry (optional)
  - Confirm Password entry (optional)
  - Profile photo section: "Pick from Gallery" and "Take Photo (Camera)" buttons
  - Photo preview with confirmation indicator
  - "Delete Photo" button (if a profile photo exists)
  - "Cancel" and "Save Changes" buttons
- **Appearance (Theme) Section:**
  - Three radio-style buttons: "System", "Light", "Dark"
  - Currently selected theme is highlighted
- **Logout Button** (red, at the bottom)
- Success and error message banners
- Loading indicator

**Functionality:**
- Loads driver profile data and profile image thumbnail on page appear
- Edits full name, phone number, and optionally changes the password
- Uploads a new profile photo (from gallery or camera) via multipart form data
- Deletes the existing profile photo
- Switches app theme (Light / Dark / System default) — preference is persisted via `Preferences`
- Logs out: clears the Bearer token from `SecureStorage` and navigates back to `LoginPage`
- Can be opened directly in edit mode (e.g., via the Dashboard edit shortcut)

---

## Popups

### Forgot Password Popup

Opened from the Login page via the "Forgot Password?" button.

- Email text entry
- "Send" button — calls `POST /profile/forgot-password` with the email
- "Cancel" button — closes the popup
- Error message and loading indicator

### Service Details Popup

Opened from a service card's "Details" button on the Service page.

- Displays service title and current status / license plate
- **Driver Report Cost (Ft)** numeric entry
- **Close Note** text area (optional, up to 500 characters)
- **Invoice Photo** section:
  - "Gallery" button — picks a photo from the device gallery
  - "Camera" button — takes a new photo
  - Photo preview with confirmation indicator
- "Cancel" and "Save" buttons
- Loading indicator and error/success banners
- Submits the details via `PUT` / `PATCH` to the API, attaching the invoice file as multipart form data

### Calendar Day Popup

Opened by tapping a day on the Dashboard calendar.

- Displays a list of events (trips, service appointments) for the selected date
- Shows event title, type, start/end time
- Allows quick viewing of what is scheduled for that day
- Closes and refreshes the calendar on dismiss

---

## Navigation

The app uses **Shell navigation** with named routes. All routes are registered in `AppShell.xaml`:

| Route | Page | Description |
|---|---|---|
| `LoginPage` | `LoginPage` | Authentication (start page) |
| `DashboardPage` | `DashboardPage` | Main home screen |
| `NotificationPage` | `NotificationPage` | Notifications list |
| `ProfilePage` | `ProfilePage` | Driver profile & settings |
| `TripPage` | `TripPage` | Trip management |
| `FuelPage` | `FuelPage` | Fuel log management |
| `ServicePage` | `ServicePage` | Service request management |

Navigation is performed programmatically via `Shell.Current.GoToAsync("RouteName")`. Query parameters (e.g., `IsNewTrip`, `IsEditing`) can be passed to pre-configure a page's state.

### Bottom Navigation Bar

A custom `BottomNavigation` component is displayed on every main page (Dashboard, Trips, Fuel, Services, Notifications). It highlights the currently active tab and allows switching between sections.

Tabs:
- 🏠 Dashboard
- 🚗 Trips
- ⛽ Fuel
- 🔧 Services
- 🔔 Notifications

---

## Services

| Service | Responsibilities |
|---|---|
| `AuthService` | `POST /login-mobile`, `POST /profile/forgot-password` |
| `DashboardService` | Load driver profile, vehicle data, statistics, calendar events, unread notification status, profile image thumbnail |
| `TripService` | List trips (paginated), create trip, delete trip |
| `FuelService` | List fuel logs (paginated), create fuel log (with optional receipt file), delete fuel log |
| `ServiceService` | List service requests (paginated), create service request, update service details (cost, note, invoice file), delete service request |
| `NotificationService` | List notifications, mark all as read, delete a notification |
| `ProfileService` | Update profile data (name, phone, password, photo), delete profile photo |
| `SessionService` | Save/retrieve/remove JWT token using `SecureStorage` |
| `ThemeService` | Get/set/apply theme preference (System/Light/Dark) using `Preferences` |
| `AuthHttpHandler` | `DelegatingHandler` — attaches `Authorization: Bearer <token>` to every HTTP request |

---

## Models

### Driver

| Property | Type | Description |
|---|---|---|
| `Id` | `ulong?` | Unique driver ID |
| `FullName` | `string?` | Driver's full name |
| `Email` | `string?` | Email address |
| `Phone` | `string?` | Phone number |
| `LicenseNumber` | `string?` | Driving license number |
| `LicenseExpiryDate` | `DateTime` | License expiry date |
| `ProfileImgFileId` | `ulong?` | ID of profile image file |
| `Role` | `string?` | User role (e.g., Driver) |

### Vehicle

| Property | Type | Description |
|---|---|---|
| `BrandModel` | `string?` | Brand and model name |
| `LicensePlate` | `string?` | Vehicle license plate |
| `Year` | `int` | Manufacturing year |
| `CurrentMileageKm` | `int` | Current odometer reading (km) |
| `Vin` | `string?` | Vehicle Identification Number |
| `Status` | `string?` | Vehicle status (e.g., Active) |

### Stats

| Property | Type | Description |
|---|---|---|
| `TotalTrips` | `int` | Number of trips logged |
| `TotalDistance` | `decimal` | Total distance driven (km) |
| `TotalServices` | `int` | Number of service requests |
| `TotalServicesCost` | `decimal` | Total service costs (Ft) |
| `TotalFuels` | `int` | Number of fuel logs |
| `TotalFuelCost` | `decimal` | Total fuel costs (Ft) |

### Trip

| Property | Type | Description |
|---|---|---|
| `Id` | `ulong` | Unique trip ID |
| `StartTime` | `DateTime` | Trip start time |
| `Long` | `TimeSpan?` | Trip duration (length of the trip) |
| `StartLocation` | `string?` | Departure location |
| `EndLocation` | `string?` | Destination location |
| `DistanceKm` | `decimal?` | Distance in kilometres |
| `Notes` | `string?` | Optional driver notes |
| `LicensePlate` | `string?` | Associated vehicle plate |

### Fuel

| Property | Type | Description |
|---|---|---|
| `Id` | `ulong` | Unique fuel log ID |
| `Date` | `DateTime` | Fill-up date/time |
| `Liters` | `decimal` | Amount of fuel (litres) |
| `TotalCostCur` | `string?` | Formatted total cost |
| `StationName` | `string?` | Fuel station name |
| `ReceiptFileId` | `ulong?` | ID of receipt image file |
| `LicensePlate` | `string?` | Associated vehicle plate |

### Service

| Property | Type | Description |
|---|---|---|
| `Id` | `ulong` | Unique service request ID |
| `Title` | `string?` | Request title |
| `Description` | `string?` | Request description |
| `Status` | `string?` | Current status (Pending, In Progress, Completed, etc.) |
| `ScheduledStart` | `DateTime?` | Scheduled start date/time |
| `DriverReportCost` | `decimal?` | Driver-reported cost (Ft) |
| `InvoiceFileId` | `ulong?` | ID of invoice image file |
| `LicensePlate` | `string?` | Associated vehicle plate |
| `ClosedAt` | `DateTime?` | Completion date/time |

### Notification

| Property | Type | Description |
|---|---|---|
| `Id` | `ulong` | Unique notification ID |
| `UserId` | `ulong` | Recipient user ID |
| `Type` | `string?` | Notification type (determines icon/color) |
| `Title` | `string?` | Notification title |
| `Message` | `string?` | Notification body text |
| `IsRead` | `bool` | Whether the notification has been read |
| `RelatedServiceRequestId` | `ulong?` | Related service request (if any) |
| `CreatedAt` | `DateTime` | Creation timestamp |

---

## Authentication & Session Management

1. The user enters email and password on the **Login Page**.
2. `AuthService` sends a `POST /login-mobile` request to the API.
3. On success, the API returns a JWT Bearer token.
4. `SessionService.SaveToken()` stores the token securely using .NET MAUI's `SecureStorage`.
5. `AuthHttpHandler` (a `DelegatingHandler`) reads the token via `SessionService.GetToken()` and attaches it as an `Authorization: Bearer <token>` header to every outgoing API request.
6. On logout, `SessionService.Logout()` removes the token from `SecureStorage` and the app navigates back to the Login page.

---

## Theme Support

The app supports three appearance modes, selectable from the **Profile Page**:

| Mode | Behavior |
|---|---|
| **System** | Follows the device's system light/dark setting |
| **Light** | Forces light theme |
| **Dark** | Forces dark theme |

The selected preference is persisted using `Preferences` and applied immediately via `Application.Current.UserAppTheme`. All pages and components use `AppThemeBinding` for colors and styles, ensuring consistent rendering in all modes.

---

## Dependencies

```xml
<PackageReference Include="CommunityToolkit.Maui"      Version="14.0.1" />
<PackageReference Include="CommunityToolkit.Mvvm"      Version="8.4.0" />
<PackageReference Include="Microsoft.Extensions.Http"  Version="10.0.4" />
<PackageReference Include="Microsoft.Maui.Controls"    Version="10.0.41" />
<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.4" />
<PackageReference Include="Plugin.Maui.Calendar"       Version="3.0.1" />
<PackageReference Include="SkiaSharp"                  Version="3.119.2" />
```

**Custom fonts included:**
- `OpenSans-Regular.ttf` / `OpenSans-Semibold.ttf` — body text
- `fa-solid-900.ttf` (FontAwesomeSolid) — solid icons
- `fa-regular-400.ttf` (FontAwesomeRegular) — outline icons

---

*© 2025 FleetFlow. All rights reserved.*
