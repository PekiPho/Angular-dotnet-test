# Reddit (Angular + .NET Core)

Ceo sistem je zasticen JWT autentifikacijom i pokriven testovima, a podeljen je na API backend i Single Page Application (SPA) frontend.

## Sta sve aplikacija moze?
* **Autentifikacija:** Login i registracija korisnika. Sve je zasticeno JWT tokenima, tako da nema neovlascenog pristupa.
* **Feed i Community:** Glavna stranica sa postovima i mogucnost ulaska u specifican community (`/community/:name`).
* **Detalji posta:** Otvaranje konkretnog posta sa svim detaljima i komentarima (`/post/:postID`).
* **Korisnicki profili:** Svaki korisnik ima svoj profilni pregled (`/profile/:userID`).
* **Pretraga:** Pisanje upita i dobijanje rezultata pretrage (`/search/:query`).

## Tech Stack

### Backend (.NET Core Web API)
* **Baza i ORM:** Koristi se SQL Server, a sva komunikacija ide preko **Entity Framework Core**.
* **Security:** **JWT (JSON Web Tokens)** za autentifikaciju. Konfigurisan je i CORS tako da lokalni Angular server moze normalno da gadja backend rute.
* **AutoMapper:** Ubacen je da automatski mapira podatke iz Entity klasa u DTO objekte.
* **API Testing:** Podignut je **Swagger** zbog lakseg vizuelnog prikaza.
* **Statika:** Konfigurisano je i serviranje statickih fajlova (slika i media), tako da se slike vuku direktno sa backenda.

### Frontend (Angular)
* **Struktura:** Sve je podeljeno na komponente. Iskoriscene su child route unutar MainPage. Ucitava se samo taj deo ekrana, a ne cela stranica iznova.
* **Navigacija:** Pored klasicnog rutiranja, napravljena je i "Get Started" landing stranica koja sluzi za onboarding korisnika pre logina.

### Testiranje (QA Arhitektura)
* **Backend Integracioni Testovi (NUnit + EF Core):** Napravljena je posebna testna baza u memoriji. Ovde se direktno testira logika kontrolera, kaskadno brisanje (npr. kada se obrise korisnik, proverava se da li postovi ostaju ali gube autora), logika za paginaciju i slozeno sortiranje (npr. filtriranje "Top Today" ili "Hot" postova).
* **API Testovi (Playwright):** Direktno gadjanje API endpointova spolja. Ovi testovi simuliraju HTTP zahteve, handluju JWT tokene i proveravaju vracene statuse.
* **E2E (End-to-End) i UI Testovi (Playwright):** Testovi koji pokrecu browser i krecu se kroz aplikaciju kao stvarni korisnik (login, kreiranje postova, upvote/downvote mehanika).
* **Frontend Komponentni Testovi:** Svaka Angular komponenta ima svoje izolovane testove kako bi se osiguralo da UI ispravno reaguje na sve promene stanja.
* **Automatsko ciscenje baze:** Svi testovi koriste `Setup` i `TearDown` metode koje pre i posle svakog testa ciste bazu. Time je osigurano da testovi uvek krecu "od nule" i ne uticu jedni na druge.
