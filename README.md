# Soccer Card Manager — MongoDB Edition

Sista uppgiften i DB-kursen. Det är fult att ljuga, så jag erkänner att det här inte var min bästa upplevelse. Men när jag tittar tillbaks på grejerna jag skrivit, så inser jag att jag har haft rätt kul med dem ändå.

När väl SQL Server började sätta sig så var det exakt när vi började köra dokument-orienterade DBs med MongoDB, så jag hade problem att få grejerna att funka. Det var mycket felsök och hattande fram-och-tillbaka. Därför valde jag att göra en förenklad version av min FETA SOCCER CARD MANAGER i SQL.

Men förenklad innebär inte att den är slarvigt ihopsatt. För att visa hur, skrev jag om arkitekturen för att göra applikationen helt Mongo.
Och använda C# ihop med MongoDB.Driver och MongoDB Atlas, med fullt fungerande CRUD-funktioner.
---

## Översikt

Eftersom MongoDB är lite mer pang-på, så gjorde jag om strukturen för de klasser jag valde att använda. Och istället för att ha ofantligt många tables med *Foreign Keys* så "nestade" jag dokumenten med någon sorts hierarkiskt tänk.


Traditionellt SQL-tänk:

Competitions → Teams → Players → Cards
(alltså separata tabeller som sitter ihop via FKs)

**Att tänka Mongo:**

Team 
  ├── Team (namm, land)
  └── Players Array
      └── Player Object
          ├── Player Info (namn, position)
          └── Cards Array
              └── Card Object (kortnummer, "rarity", power)

**"Ett feltänk som blir helrätt"**
Under arbetet, flaxade jag fram och tillbaka mellan tutorials, lektionsvideos, övningar, och gamla project-filer för att få appen att köra utan Errors. Och när allt faktiskt gjorde det kändes det lite tomt att inte ha skapat alla tables som documents (som det var i SQL-versionen) men efter att ha snackat med fransosen, så slog det mig, att ett feltänk blev helrätt, eftersom det här är ju exakt vad som gör Mongo mer snabbjobbat än SQL. Alltså det perfekta alternativet när man vill logga sina kort snabbt.

## Architecture

### 💡 SQL vs MongoDB: Why This Design and when to apply it?

**Den här designen är toppen när:**
- Du har data med naturlig hierarkisk ordning (Teams → Players → Cards)
- Du ofta behöver samtidig access till relaterad data
- Du vill kunna lägga till ytterligare properties på ett smidigt sätt


**SQL är att föredra när:**
- Du behöver köra queries på kort över alla lag, tävlingar, och säsonger
- Du har FKs med komplexa relationer som emellanåt kräver en join 
- Lagring är begränsad
- Storage space is limited

## Features

**Full-CRUD** - Create, Read, Update, Delete på alla nivåer  
**Document-orienterad Design** - Data ordnad efter inbördes hierarki 
**MongoDB Atlas-integration** - Databasen bor i molnet  
**Säker Config** - Connection string i separat fil  
**Query-kapacitet** - Flera sätt att analysera data i collections 
**Portabel och lätt att klona** - Följ instruktionerna nednför och testa  


### Prerequisites
- .NET 8.0+
- MongoDB Atlas account (gratis finns)
- Git

### Installation

1. **Klona repo:**
git clone https://github.com/je-suis-paradis/SoccerCardMongo.git

cd SoccerCardMongo

(Eller tanka hem filen från GitHub.com om GUI är mer din bag)

2. **Skapa `appsettings.json` i projektets root:**

Hitta connection string iCopy from `appsettings.example.json` and fill in your MongoDB Atlas connection string:


{
  "MongoDb": {
    "ConnectionString": "mongodb+srv://username:password@yourcluster.mongodb.net/?appName=YourCluster",
    "DatabaseName": "SoccerCardMongo"
  }
}


3. **Bygg och kör:**
dotnet build (CTRL+Shift+B)
dotnet run (F5)


---

## Lek runt och testa (mockdata finns):

### Main Menu

1. Manage Teams
2. Manage Players
3. Manage Cards
4. Queries & Reports
5. Exit


### Appens flow:

## Har du brådis? Kör ett snabbtest:
**Snabbtesta flödet i appen:**
1. Skapa ett lag: "Manchester United" (England)
2. Skapa en spelare: "Cristiano Ronaldo" (Forward)
3. Skapa ett kort: "#1" (Rare, Power: 95)
4. Kolla en spelares kort → PANG! Alla stats
5. Kolla lag-stats → visa en spelare, ett kort, genomsnittlig Power
6. Uppdatera kortets Power till 98
7. Radera kortet
8. Försäkra dig om att kortet är borta

### Annars
Skapa en kollektion:

1. **Create Team** → "Arsenal FC" (England)
2. **Create Player** → "David Raya" (Goalkeeper) under Arsenal
3. **Create Card** → "#001" (Common, Power: 85) för David Raya
4. **Query** → Se en spelares alla kort, lag-stats, eller kort efter power

### CRUD:a kollektionen
All entities support:
- **Create** - Skapa, lag, spelare, kort
- **Read** - Kolla ALL data på ALLA nivåer
- **Update** - Editer, spelare, lag, kort
- **Delete** - Radera ett document (auto-cascade)

### Preppade queries

1. **Show Player's Cards** - Se alla kort på en specifik spelare
2. **Show Team Statistics** - Totalt antal spelare och kort, Power-snitt
3. **Show Cards by Power** - Alla sorterade, från toppen till botten
---

### Tech Stack

- **Språk:** C# (.NET 8.0)
- **Databas-driver:** MongoDB.Driver
- **Databas-host:** MongoDB Atlas (Moln)
- **Config-fil:** appsettings.json
- **Interface:** Konsolapplikation
---

### Projektstruktur

~/SoccerCardMongo/
├── Models/
│   ├── Card.cs         
│   ├── Player.cs       
│   └── Team.cs         
├── Data/
│   └── SoccerCardMongoContext.cs    
├── Program.cs          
├── appsettings.json    
├── appsettings.example.json
└── .gitignore
	

### Ett par nötter om säkerheten:
◑⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘◐
‖ - appsettings.json är ocommitad (den innehåller lösenord)   	 
‖																	  
‖ - appsettings.example.json är commitad (med potatislösenord) 
‖																	  
‖ - Klona repot → kopiera exampelfilen → fyll i DINA uppgifter  
‖																	  
‖ - Commita aldrig riktiga på GitHub! (... dela inte nålar heller!)
◑⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘⫘◐
