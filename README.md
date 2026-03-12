<div align="center">
  
# 🚗 FleetFlow – Mobile Driver App

Egy többplatformos mobilalkalmazás, amely **.NET MAUI**-val készült flottakezeléshez. Az alkalmazást **sofőrök** számára terveztük, és a FleetFlow REST API backendhez csatlakozik. Lehetővé teszi a sofőrök számára az utak kezelését, a tankolások rögzítését, szervizigények beküldését, értesítések megtekintését, valamint a profiljuk naprakészen tartását — mindezt egyetlen, igényesen kidolgozott mobilalkalmazásból.

[![.NET CI](https://github.com/FleetFlow-Zarodolgozat/FleetFlow_mobil/actions/workflows/dotnet.yml/badge.svg)](https://github.com/FleetFlow-Zarodolgozat/FleetFlow_mobil/actions/workflows/dotnet.yml)

</div>

---

## 📑 Tartalomjegyzék

- [Technológiai stack](#-technológiai-stack)
- [Projektstruktúra](#-projektstruktúra)
- [Architektúra](#-architektúra)
- [Alkalmazás oldalak](#-alkalmazás-oldalak)
  - [Bejelentkezés oldal](#-1-bejelentkezés-oldal)
  - [Irányítópult oldal](#-2-irányítópult-oldal)
  - [Utak oldal](#-3-utak-oldal)
  - [Tankolás oldal](#-4-tankolás-oldal)
  - [Szerviz oldal](#-5-szerviz-oldal)
  - [Értesítések oldal](#-6-értesítések-oldal)
  - [Profil oldal](#-7-profil-oldal)
- [Felugró ablakok](#-felugró-ablakok)
  - [Elfelejtett jelszó felugró](#-elfelejtett-jelszó-felugró)
  - [Szerviz részletek felugró](#-szerviz-részletek-felugró)
  - [Naptár nap felugró](#-naptár-nap-felugró)
- [Navigáció](#-navigáció)
- [Szolgáltatások](#-szolgáltatások)
- [Modellek](#-modellek)
- [Hitelesítés és munkamenet-kezelés](#-hitelesítés-és-munkamenet-kezelés)
- [Téma támogatás](#-téma-támogatás)
- [Függőségek](#-függőségek)

---

## 🧰 Technológiai stack

| Technology | Version | Purpose |
|---|---|---|
| .NET MAUI | net10.0 | Többplatformos UI keretrendszer |
| C# | 12+ | Alkalmazáslogika |
| XAML | — | UI elrendezés definíció |
| CommunityToolkit.Maui | 14.0.1 | Felugrók, konverterek, kiterjesztések |
| CommunityToolkit.Mvvm | 8.4.0 | MVVM forrásgenerátorok, parancsok |
| Plugin.Maui.Calendar | 3.0.1 | Interaktív naptár komponens |
| SkiaSharp | 3.119.2 | 2D grafikai megjelenítés |
| Microsoft.Extensions.Http | 10.0.4 | Típusos HttpClient DI |
| FontAwesome | — | Ikon betűkészletek (Solid & Regular) |

**Támogatott platformok:** Android (API 21+), iOS (15.0+), macOS Catalyst (15.0+), Windows (10.0.17763+)

**Backend API:** `https://fleetflow-zarodolgozat-backend-ressdominik.jcloud.jedlik.cloud/api/`

---

## 🗂️ Projektstruktúra

```
FleetFlow_mobil/
└── mobil/
    └── mobil/
        ├── App.xaml / App.xaml.cs          # Alkalmazás belépési pont
        ├── AppShell.xaml / AppShell.xaml.cs # Shell navigációs útvonalak
        ├── MauiProgram.cs                  # Dependency injection beállítás
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
        │   └── AuthHttpHandler.cs  # Bearer token hozzáadása minden kéréshez
        └── Converters/             # Érték-konverterek XAML kötésekhez
```

---

## 🏗️ Architektúra

Az alkalmazás a **MVVM (Model-View-ViewModel)** mintát követi a `CommunityToolkit.Mvvm` forrásgenerátorainak használatával:

- **Modellek** – Egyszerű C# osztályok, amelyek az API adatokat reprezentálják (Driver, Trip, Fuel, stb.)
- **Services** – Típusos `HttpClient` burkolók, amelyek a REST API-val kommunikálnak
- **ViewModel-ek** – Adatokat és parancsokat tesznek elérhetővé a UI számára `ObservableObject` és `[RelayCommand]` segítségével
- **Nézetek (Pages/Popups)** – XAML-alapú UI, amely ViewModel-ekhez kötődik
- **Dependency Injection** – Minden szolgáltatás, ViewModel és oldal regisztrálva van a `MauiProgram.cs` fájlban

Minden hitelesített API kérésen áthalad az `AuthHttpHandler`, amely automatikusan hozzáfűzi a JWT Bearer tokent a `SessionService`-ből (ami `SecureStorage`-ban van tárolva).

---

## 📱 Alkalmazás oldalak

### 🔐 1. Bejelentkezés oldal

**Útvonal:** `LoginPage` (alapértelmezett / induló oldal)

Az alkalmazás belépési pontja. A sofőrök az e-mail címükkel és jelszavukkal jelentkeznek be.

**UI elemek:**
- Kék fejléc FleetFlow logóval (autó ikon) és alkalmazás címmel
- E-mail beviteli mező e-mail ikonnal
- Jelszó beviteli mező lakat ikonnal és megjelenítés/elrejtés kapcsoló gombbal
- Hibaüzenet sáv (sikertelen bejelentkezéskor látható)
- "Forgot Password?" gomb — megnyitja az **Elfelejtett jelszó felugrót**
- "Sign In" gomb — elindítja a hitelesítést
- Betöltésjelző bejelentkezés közben
- Lábléc szerzői jogi felirat

**Funkcionalitás:**
- `POST /login-mobile` kérést küld e-maillel és jelszóval
- Siker esetén: elmenti a JWT tokent `SecureStorage`-ba és navigál a `DashboardPage` oldalra
- Sikertelenség esetén: felhasználóbarát hibaüzenetet jelenít meg
- Jelszó láthatóság kapcsoló (szem ikon)
- A űrlap letiltása betöltés alatt a dupla beküldések elkerülése érdekében

---

### 🧭 2. Irányítópult oldal

**Útvonal:** `DashboardPage`

A bejelentkezés után megjelenő fő kezdőképernyő. Teljes áttekintést ad a sofőr állapotáról, a hozzárendelt járműről, személyes statisztikákról és a közelgő ütemezett események[...]

**UI elemek:**
- Felső sáv FleetFlow logóval, értesítési csengővel (olvasatlan jelvénnyel) és profil avatarral
- Üdvözlő üzenet a sofőr nevével
- **Sofőr profil kártya**: név, szerepkör jelvény, e-mail, telefon, jogosítvány szám, jogosítvány lejárati dátum, szerkesztési gyorsgomb
- **Gyors művelet gombok**: "New Trip" és "New Fuel Log" (gyors utak a létrehozó űrlapokhoz)
- **Járművem kártya**: márka/modell, rendszám, évjárat, VIN, aktuális kilométeróra állás (km), státusz jelvény
- **Statisztikáim** rács (6 stat kártya):
  - Total Trips
  - Total Distance (km)
  - Total Services
  - Total Service Cost (Ft)
  - Total Fuels
  - Total Fuel Cost (Ft)
- **Interaktív naptár**: eseményeket (utak, szervizek) színes pontokkal jelenít meg; egy nap megérintése megnyitja a **Naptár nap felugrót**
- Betöltésjelző és hibaállapot

**Funkcionalitás:**
- A sofőr profilt, hozzárendelt járművet, személyes statisztikákat és naptár eseményeket az API-ból tölti be az oldal megjelenésekor
- Az értesítési csengő piros jelvényt mutat, ha vannak olvasatlan értesítések
- A csengő megérintése az Értesítések oldalra navigál
- Az avatar megérintése a Profil oldalra navigál
- A profilkártyán lévő szerkesztés ikon (ceruza) megérintése a Profil oldalra visz szerkesztő módban
- A "New Trip" gyorsgomb az Utak oldalra navigál, előre megnyitott létrehozó űrlappal
- A "New Fuel Log" gyorsgomb a Tankolás oldalra navigál, előre megnyitott létrehozó űrlappal
- Egy naptárnap megérintése felugrót nyit, amely felsorolja az adott napi eseményeket

---

### 🚗 3. Utak oldal

**Útvonal:** `TripPage`

Lehetővé teszi a sofőrök számára az utak rögzítését, megtekintését és törlését.

**UI elemek:**
- Felső sáv teherautó ikonnal, "Trip Log" címmel és "+ New" gombbal
- Kinyitható **Új út űrlap** (a "+ New" gomb kapcsolja):
  - Kezdő dátum és kezdő idő választók
  - Befejező dátum és befejező idő választók
  - Indulási hely szövegmező
  - Érkezési hely szövegmező
  - Távolság (km) numerikus mező
  - Kezdő kilométeróra (km) numerikus mező
  - Záró kilométeróra (km) numerikus mező
  - Megjegyzések szövegmező (opcionális)
  - "Cancel" és "Create Trip" gombok
- Siker és hiba üzenetsávok
- "MY TRIPS" szekció fejléc
- Üres állapot illusztráció és üzenet (ha nincs út)
- **Út kártya lista** — minden kártya megjeleníti:
  - Útvonal: Indulási hely → Érkezési hely
  - Dátum és időtartam
  - Távolság (km)
  - Rendszám
  - Megjegyzések (ha van)
  - Törlés gomb (megerősítő párbeszéddel)
- **Lapozó vezérlők** (Prev / Page X of Y / Next), teljes elemszám megjelenítéssel

**Funkcionalitás:**
- Lapozott utakat kér le az API-ból (10 elem oldalanként)
- Új utat hoz létre egy űrlap beküldésével (kezdő/záró idők, helyek, távolság, kilométeróra adatok)
- Út törlése felhasználói megerősítés után riasztás párbeszédablakon keresztül
- Oldalak közötti navigáció Previous/Next gombokkal
- Megnyitható előre kinyitott létrehozó űrlappal (pl. az Irányítópult gyorsműveletből)

---

### ⛽ 4. Tankolás oldal

**Útvonal:** `FuelPage`

Lehetővé teszi a sofőrök számára a tankolások rögzítését, megtekintését és törlését, blokk/fénykép feltöltéssel.

**UI elemek:**
- Felső sáv benzinkút ikonnal, "Fuel Log" címmel és "+ New" gombbal
- Kinyitható **Új tankolás űrlap**:
  - Dátum és idő választók
  - Kilométeróra állás (km) numerikus mező
  - Liter numerikus mező
  - Összköltség numerikus mező
  - Kút neve szövegmező (opcionális)
  - Helyszín szövegmező (opcionális)
  - Blokk fotó szekció: "Pick from Gallery" és "Take Photo" gombok
  - Blokk fotó előnézet megerősítő jelzéssel
  - "Cancel" és "Log Fuel" gombok
- Siker és hiba üzenetsávok
- "MY FUEL LOGS" szekció fejléc
- Üres állapot üzenet (ha nincs bejegyzés)
- **Tankolás kártya lista** — minden kártya megjeleníti:
  - Dátum
  - Üzemanyag mennyiség (liter) és összkölts��g
  - Kút neve (ha megadott)
  - Rendszám
  - Törlés gomb (megerősítő párbeszéddel)
- **Lapozó vezérlők** (Prev / Page X of Y / Next)

**Funkcionalitás:**
- Lapozott tankolás naplót kér le az API-ból (10 elem oldalanként)
- Új tankolás naplót hoz létre; opcionálisan csatol blokk képet (galériából vagy kamerából)
- A blokk fotót multipart form-data formátumban tölti fel az API felé
- Tankolás bejegyzés törlése felhasználói megerősítés után
- Megnyitható előre kinyitott létrehozó űrlappal (pl. az Irányítópult gyorsműveletből)

---

### 🔧 5. Szerviz oldal

**Útvonal:** `ServicePage`

Lehetővé teszi a sofőrök számára járműszerviz igények beküldését, megtekintését, részletek frissítését és törlését.

**UI elemek:**
- Felső sáv villáskulcs ikonnal, "Service Requests" címmel és lila "+ New" gombbal
- Kinyitható **Új szerviz igény űrlap**:
  - Cím szövegmező (pl. "Oil change needed")
  - Leírás szövegmező (opcionális, max. 500 karakter)
  - "Cancel" és "Create Request" gombok
- Siker és hiba üzenetsávok
- "MY SERVICE REQUESTS" szekció fejléc
- Üres állapot üzenet
- **Szerviz kártya lista** — minden kártya megjeleníti:
  - Lila felső kiemelő csík
  - Cím és rendszám
  - Státusz jelvény (színkódolt: Pending / In Progress / Completed / stb.)
  - Leírás (2 sorra rövidítve)
  - Tervezett kezdési dátum
  - Sofőr által jelentett költség (Ft)
  - "Details" gomb — megnyitja a **Szerviz részletek felugrót**
  - Törlés gomb (megerősítő párbeszéddel)
- **Lapozó vezérlők** (Prev / Page X of Y / Next)

**Funkcionalitás:**
- Lapozott szerviz igényeket kér le az API-ból (10 elem oldalanként)
- Új szerviz igényt hoz létre címmel és opcionális leírással
- Megnyitja a Szerviz részletek felugrót költség, megjegyzés szerkesztéséhez és számla fotó feltöltéséhez
- Szerviz igény törlése felhasználói megerősítés után
- A státusz megfelelő ikonokkal és színkódolással jelenik meg

---

### 🔔 6. Értesítések oldal

**Útvonal:** `NotificationPage`

Az összes rendszerértesítést megjeleníti, amelyet a sofőr kap (pl. út frissítések, szerviz státusz változások).

**UI elemek:**
- Felső sáv "FleetFlow" címmel, "Notifications" alcímmel és "Mark all read" gombbal
- Betöltésjelző
- "NOTIFICATIONS / Your latest updates" fejléc
- **Értesítés kártya lista** — minden kártya megjeleníti:
  - Típus ikon (színkódolt az értesítés típusa szerint)
  - Cím (félkövér) olvasatlan kék pont jelzővel
  - Üzenet törzs (max. 2 sor)
  - Típus jelvény (színe egyezik az ikonnal)
  - Időbélyeg (yyyy.MM.dd HH:mm)
  - Törlés gomb (piros kuka ikon)
- Üres állapot (csengő ikon + "No notifications / You're all caught up!")

**Funkcionalitás:**
- Az oldalon megjelenéskor betölti az összes értesítést az API-ból
- Az értesítések típus alapján vizuálisan elkülönülnek (különböző ikon színek és jelvény feliratok)
- Az olvasatlan értesítések kék pontot mutatnak a cím mellett
- A "Mark all read" gomb minden értesítést olvasottnak jelöl
- Egyedi értesítések törölhetők a kuka ikon gombbal

---

### 👤 7. Profil oldal

**Útvonal:** `ProfilePage`

Megjeleníti a sofőr személyes profilját és lehetővé teszi a személyes adatok, jelszó és profilkép szerkesztését. Emellett téma beállításokat és kijelentkezést is biztosít.

**UI elemek:**
- **Megtekintő mód:**
  - Nagy, kör alakú profil avatar (fotó vagy FA ikon tartalék)
  - Teljes név
  - Szerepkör jelvény
  - E-mail, telefon, jogosítvány szám, jogosítvány lejárat információ sorok
  - "Edit Profile" gomb
- **Szerkesztő mód** (Edit gombbal kapcsolható):
  - Teljes név szövegmező
  - Telefonszám szövegmező
  - Új jelszó mező (opcionális)
  - Jelszó megerősítése mező (opcionális)
  - Profilkép szekció: "Pick from Gallery" és "Take Photo (Camera)" gombok
  - Fotó előnézet megerősítő jelzéssel
  - "Delete Photo" gomb (ha van profilkép)
  - "Cancel" és "Save Changes" gombok
- **Megjelenés (Téma) szekció:**
  - Három rádió-stílusú gomb: "System", "Light", "Dark"
  - Az aktuálisan kiválasztott téma kiemelve
- **Kijelentkezés gomb** (piros, alul)
- Siker és hiba üzenetsávok
- Betöltésjelző

**Funkcionalitás:**
- Betölti a sofőr profil adatait és a profilkép bélyegképet az oldal megjelenésekor
- Szerkeszti a teljes nevet, telefonszámot, és opcionálisan jelszót változtat
- Új profilképet tölt fel (galériából vagy kamerából) multipart form-data segítségével
- Törli a meglévő profilképet
- Témát vált (Light / Dark / System default) — a beállítás `Preferences`-ben tárolódik
- Kijelentkezés: törli a Bearer tokent a `SecureStorage`-ból és visszanavigál a `LoginPage` oldalra
- Közvetlenül megnyitható szerkesztő módban (pl. az Irányítópult szerkesztési gyorsgombjával)

---

## 🪟 Felugró ablakok

### 🔑 Elfelejtett jelszó felugró

A Bejelentkezés oldalról nyílik meg a "Forgot Password?" gombbal.

- E-mail beviteli mező
- "Send" gomb — meghívja a `POST /profile/forgot-password` végpontot az e-maillel
- "Cancel" gomb — bezárja a felugrót
- Hibaüzenet és betöltésjelző

### 🧾 Szerviz részletek felugró

A Szerviz oldalon, egy szerviz kártya "Details" gombjára kattintva nyílik meg.

- Megjeleníti a szerviz címet és az aktuális státuszt / rendszámot
- **Sofőr által jelentett költség (Ft)** numerikus mező
- **Lezárási megjegyzés** szövegmező (opcionális, max. 500 karakter)
- **Számla fotó** szekció:
  - "Gallery" gomb — fotó kiválasztása a készülék galériájából
  - "Camera" gomb — új fotó készítése
  - Fotó előnézet megerősítő jelzéssel
- "Cancel" és "Save" gombok
- Betöltésjelző és hiba/siker üzenetsávok
- A részleteket `PUT` / `PATCH` kéréssel küldi az API felé, a számla fájlt multipart form-data formátumban csatolva

### 📅 Naptár nap felugró

Az Irányítópult naptárán egy nap megérintésével nyílik meg.

- Eseménylista megjelenítése (utak, szerviz időpontok) a kiválasztott dátumra
- Mutatja az esemény címet, típust, kezdő/záró időt
- Új esemény létrehozása az adott napra
- Gyors áttekintést ad az adott nap ütemezéséről
- Bezáráskor frissíti a naptárt

---

## 🧭 Navigáció

Az alkalmazásban regisztrálva van az `AppShell.xaml` fájlban:

| Route | Page | Description |
|---|---|---|
| `LoginPage` | `LoginPage` | Hitelesítés (kezdő oldal) |
| `DashboardPage` | `DashboardPage` | Fő kezdőképernyő |
| `NotificationPage` | `NotificationPage` | Értesítések lista |
| `ProfilePage` | `ProfilePage` | Sofőr profil és beállítások |
| `TripPage` | `TripPage` | Utak kezelése |
| `FuelPage` | `FuelPage` | Tankolás napló kezelése |
| `ServicePage` | `ServicePage` | Szerviz igény kezelés |

A navigáció programozottan történik a `Shell.Current.GoToAsync("RouteName")` használatával. Query paraméterek (pl. `IsNewTrip`, `IsEditing`) átadhatók egy oldal állapotának előkonfigurál�[...]

### 🧷 Alsó navigációs sáv

Egy egyedi `BottomNavigation` komponens jelenik meg minden fő oldalon (Irányítópult, Utak, Tankolás, Szervizek, Értesítések). Kiemeli az aktuálisan aktív fület, és lehetővé teszi a szekc[...]

Fülek:
- 🏠 Irányítópult
- 🚗 Utak
- ⛽ Tankolás
- 🔧 Szervizek

---

## 🧩 Szolgáltatások

| Service | Responsibilities |
|---|---|
| `AuthService` | `POST /login-mobile`, `POST /profile/forgot-password` |
| `DashboardService` | Sofőr profil, jármű adatok, statisztikák, naptár események, olvasatlan értesítési állapot, profilkép bélyegkép betöltése |
| `TripService` | Utak listázása (lapozott), út létrehozása, út törlése |
| `FuelService` | Tankolások listázása (lapozott), tankolás létrehozása (opcionális blokk fájllal), tankolás törlése |
| `ServiceService` | Szerviz igények listázása (lapozott), szerviz igény létrehozása, szerviz részletek frissítése (költség, megjegyzés, számla fájl), szerviz igény törlése |
| `NotificationService` | Értesítések listázása, összes olvasottnak jelölése, értesítés törlése |
| `ProfileService` | Profil adatok frissítése (név, telefon, jelszó, fotó), profilkép törlése |
| `SessionService` | JWT token mentése/kiolvasása/törlése `SecureStorage` használatával |
| `ThemeService` | Téma beállítás lekérdezése/beállítása/alkalmazása (System/Light/Dark) `Preferences` használatával |
| `AuthHttpHandler` | `DelegatingHandler` — hozzáfűzi az `Authorization: Bearer <token>` fejlécet minden HTTP kéréshez |

---

## 🧾 Modellek

### 🧑‍✈️ Driver

| Property | Type | Description |
|---|---|---|
| `Id` | `ulong?` | Egyedi sofőr azonosító |
| `FullName` | `string?` | Sofőr teljes neve |
| `Email` | `string?` | E-mail cím |
| `Phone` | `string?` | Telefonszám |
| `LicenseNumber` | `string?` | Jogosítvány száma |
| `LicenseExpiryDate` | `DateTime` | Jogosítvány lejárati dátuma |
| `ProfileImgFileId` | `ulong?` | Profilkép fájl azonosítója |
| `Role` | `string?` | Felhasználói szerepkör (pl. Driver) |

### 🚙 Vehicle

| Property | Type | Description |
|---|---|---|
| `BrandModel` | `string?` | Márka és modell neve |
| `LicensePlate` | `string?` | Jármű rendszáma |
| `Year` | `int` | Gyártási év |
| `CurrentMileageKm` | `int` | Aktuális kilométeróra állás (km) |
| `Vin` | `string?` | Alvázszám (VIN) |
| `Status` | `string?` | Jármű státusz (pl. Active) |

### 📊 Stats

| Property | Type | Description |
|---|---|---|
| `TotalTrips` | `int` | Rögzített utak száma |
| `TotalDistance` | `decimal` | Összes megtett távolság (km) |
| `TotalServices` | `int` | Szerviz igények száma |
| `TotalServicesCost` | `decimal` | Összes szerviz költség (Ft) |
| `TotalFuels` | `int` | Tankolások száma |
| `TotalFuelCost` | `decimal` | Összes üzemanyag költség (Ft) |

### 🛣️ Trip

| Property | Type | Description |
|---|---|---|
| `Id` | `ulong` | Egyedi út azonosító |
| `StartTime` | `DateTime` | Út kezdési időpontja |
| `Long` | `TimeSpan?` | Út időtartama (az út hossza) |
| `StartLocation` | `string?` | Indulási hely |
| `EndLocation` | `string?` | Érkezési hely |
| `DistanceKm` | `decimal?` | Távolság kilométerben |
| `Notes` | `string?` | Opcionális sofőr megjegyzések |
| `LicensePlate` | `string?` | Kapcsolt jármű rendszáma |

### 🧴 Fuel

| Property | Type | Description |
|---|---|---|
| `Id` | `ulong` | Egyedi tankolás azonosító |
| `Date` | `DateTime` | Tankolás dátuma/ideje |
| `Liters` | `decimal` | Üzemanyag mennyisége (liter) |
| `TotalCostCur` | `string?` | Formázott végösszeg |
| `StationName` | `string?` | Benzinkút neve |
| `ReceiptFileId` | `ulong?` | Blokk kép fájl azonosító |
| `LicensePlate` | `string?` | Kapcsolt jármű rendszáma |

### 🛠️ Service

| Property | Type | Description |
|---|---|---|
| `Id` | `ulong` | Egyedi szerviz igény azonosító |
| `Title` | `string?` | Igény címe |
| `Description` | `string?` | Igény leírása |
| `Status` | `string?` | Aktuális státusz (Pending, In Progress, Completed, stb.) |
| `ScheduledStart` | `DateTime?` | Tervezett kezdés dátuma/ideje |
| `DriverReportCost` | `decimal?` | Sofőr által jelentett költség (Ft) |
| `InvoiceFileId` | `ulong?` | Számla kép fájl azonosító |
| `LicensePlate` | `string?` | Kapcsolt jármű rendszáma |
| `ClosedAt` | `DateTime?` | Befejezés dátuma/ideje |

### 🔔 Notification

| Property | Type | Description |
|---|---|---|
| `Id` | `ulong` | Egyedi értesítés azonosító |
| `UserId` | `ulong` | Címzett felhasználó azonosító |
| `Type` | `string?` | Értesítés típusa (meghatározza az ikont/színt) |
| `Title` | `string?` | Értesítés címe |
| `Message` | `string?` | Értesítés szövege |
| `IsRead` | `bool` | Olvasott-e az értesítés |
| `RelatedServiceRequestId` | `ulong?` | Kapcsolt szerviz igény (ha van) |
| `CreatedAt` | `DateTime` | Létrehozás időbélyeg |

---

## 🔐 Hitelesítés és munkamenet-kezelés

1. A felhasználó megadja az e-mailt és jelszót a **Bejelentkezés oldalon**.
2. Az `AuthService` elküldi a `POST /login-mobile` kérést az API-nak.
3. Siker esetén az API visszaad egy JWT Bearer tokent.
4. A `SessionService.SaveToken()` biztonságosan eltárolja a tokent a .NET MAUI `SecureStorage` segítségével.
5. Az `AuthHttpHandler` (egy `DelegatingHandler`) kiolvassa a tokent a `SessionService.GetToken()`-nel, és hozzáfűzi az `Authorization: Bearer <token>` fejlécet minden kimenő API kéréshez.
6. Kijelentkezéskor a `SessionService.Logout()` eltávolítja a tokent a `SecureStorage`-ból, és az alkalmazás visszanavigál a Bejelentkezés oldalra.

---

## 🎨 Téma támogatás

Az alkalmazás három megjelenési módot támogat, amelyek a **Profil oldalon** választhatók:

| Mode | Behavior |
|---|---|
| **System** | Követi az eszköz rendszer szintű világos/sötét beállítását |
| **Light** | Világos témát kényszerít |
| **Dark** | Sötét témát kényszerít |

A kiválasztott beállítás `Preferences` használatával kerül eltárolásra, és azonnal alkalmazódik az `Application.Current.UserAppTheme` segítségével. Minden oldal és komponens `AppThemeBi[...]

---

## 📦 Függőségek

```xml
<PackageReference Include="CommunityToolkit.Maui"      Version="14.0.1" />
<PackageReference Include="CommunityToolkit.Mvvm"      Version="8.4.0" />
<PackageReference Include="Microsoft.Extensions.Http"  Version="10.0.4" />
<PackageReference Include="Microsoft.Maui.Controls"    Version="10.0.41" />
<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.4" />
<PackageReference Include="Plugin.Maui.Calendar"       Version="3.0.1" />
<PackageReference Include="SkiaSharp"                  Version="3.119.2" />
```

**Egyedi betűkészletek:**
- `OpenSans-Regular.ttf` / `OpenSans-Semibold.ttf` — törzsszöveg
- `fa-solid-900.ttf` (FontAwesomeSolid) — kitöltött ikonok
- `fa-regular-400.ttf` (FontAwesomeRegular) — körvonalas ikonok

---

<div align="center">

*© 2026 FleetFlow. Minden jog fenntartva.*

</div>
