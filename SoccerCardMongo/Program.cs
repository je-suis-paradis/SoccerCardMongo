using MongoDB.Bson;
using MongoDB.Driver;
using SoccerCardMongo.Data;
using SoccerCardMongo.Models;

namespace SoccerCardMongo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Loading connString, appsettings.json
            var config = LoadConfig();
            var context = new SoccerCardMongoContext(
                config["ConnectionString"],
                config["DatabaseName"]
            );

            bool running = true;
            while (running)
            {
                DisplayMainMenu();
                string choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1":
                        TeamMenu(context);
                        break;
                    case "2":
                        PlayerMenu(context);
                        break;
                    case "3":
                        CardMenu(context);
                        break;
                    case "4":
                        QueryMenu(context);
                        break;
                    case "5":
                        running = false;
                        Console.WriteLine("\nThanks for using Soccer Card Manager! Goodbye!");
                        break;
                    default:
                        Console.WriteLine("\n[!] Invalid choice. Try again.");
                        break;
                }

                if (running && choice != "")
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        // LOAD CONFIG
        static Dictionary<string, string> LoadConfig()
        {
            try
            {
                var json = System.IO.File.ReadAllText("appsettings.json");
                
                var connStart = json.IndexOf("\"ConnectionString\": \"") + "\"ConnectionString\": \"".Length;
                var connEnd = json.IndexOf("\"", connStart);
                var connectionString = json.Substring(connStart, connEnd - connStart);

                var dbStart = json.IndexOf("\"DatabaseName\": \"") + "\"DatabaseName\": \"".Length;
                var dbEnd = json.IndexOf("\"", dbStart);
                var databaseName = json.Substring(dbStart, dbEnd - dbStart);

                return new Dictionary<string, string>
                {
                    { "ConnectionString", connectionString },
                    { "DatabaseName", databaseName }
                };
            }
            catch
            {
                Console.WriteLine("[!] Error loading appsettings.json");
                Environment.Exit(1);
                return null;
            }
        }

        // MENU 
        static void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║   SOCCER CARD MANAGER - MONGODB        ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");
            Console.WriteLine("1. Manage Teams");
            Console.WriteLine("2. Manage Players");
            Console.WriteLine("3. Manage Cards");
            Console.WriteLine("4. Queries & Reports");
            Console.WriteLine("5. Exit");
            Console.WriteLine("\n[?] Choose an option (1-5): ");
        }

        static void DisplaySubMenu(string entityName)
        {
            Console.Clear();
            Console.WriteLine($"╔════════════════════════════════════════╗");
            Console.WriteLine($"║ {entityName.PadRight(36)}              ║");
            Console.WriteLine($"╚════════════════════════════════════════╝\n");
            Console.WriteLine("1. Create");
            Console.WriteLine("2. Read");
            Console.WriteLine("3. Update");
            Console.WriteLine("4. Delete");
            Console.WriteLine("5. Back");
            Console.WriteLine("\n[?] Choose an option (1-5): ");
        }

        // TEAM CRUD
        static void TeamMenu(SoccerCardMongoContext context)
        {
            bool inMenu = true;
            while (inMenu)
            {
                DisplaySubMenu("TEAMS");
                string choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1":
                        CreateTeam(context);
                        break;
                    case "2":
                        ReadTeams(context);
                        break;
                    case "3":
                        UpdateTeam(context);
                        break;
                    case "4":
                        DeleteTeam(context);
                        break;
                    case "5":
                        inMenu = false;
                        break;
                    default:
                        Console.WriteLine("[!] Invalid choice.");
                        break;
                }

                if (inMenu && choice != "")
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void CreateTeam(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[+] CREATE TEAM\n");

            Console.Write("Team Name: ");
            string name = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Country: ");
            string country = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(country))
            {
                Console.WriteLine("[!] Name and Country are required!");
                return;
            }

            var team = new Team
            {
                Id = ObjectId.GenerateNewId(),
                Name = name,
                Country = country,
                Players = new()
            };

            context.Teams.InsertOne(team);
            Console.WriteLine($"Team '{name}' created! ID: {team.Id}");
        }

        static void ReadTeams(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[*] ALL TEAMS\n");

            var teams = context.Teams.Find(_ => true).ToList();
            if (teams.Count == 0)
            {
                Console.WriteLine("[!] No teams found.");
                return;
            }

            foreach (var team in teams)
            {
                Console.WriteLine($"[ID: {team.Id}] {team.Name} | {team.Country} | {team.Players.Count} players");
            }
        }

        static void UpdateTeam(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[~] UPDATE TEAM\n");

            ReadTeams(context);

            Console.Write("\nEnter Team ID to update: ");
            if (!ObjectId.TryParse(Console.ReadLine(), out ObjectId teamId))
            {
                Console.WriteLine("[!] Invalid ID.");
                return;
            }

            var team = context.Teams.Find(t => t.Id == teamId).FirstOrDefault();
            if (team == null)
            {
                Console.WriteLine("[!] Team not found.");
                return;
            }

            Console.Write($"New Name ({team.Name}): ");
            string newName = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(newName)) team.Name = newName;

            Console.Write($"New Country ({team.Country}): ");
            string newCountry = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(newCountry)) team.Country = newCountry;

            context.Teams.ReplaceOne(t => t.Id == teamId, team);
            Console.WriteLine("Team updated!");
        }

        static void DeleteTeam(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[X] DELETE TEAM\n");

            ReadTeams(context);

            Console.Write("\nEnter Team ID to delete: ");
            if (!ObjectId.TryParse(Console.ReadLine(), out ObjectId teamId))
            {
                Console.WriteLine("[!] Invalid ID.");
                return;
            }

            var team = context.Teams.Find(t => t.Id == teamId).FirstOrDefault();
            if (team == null)
            {
                Console.WriteLine("[!] Team not found.");
                return;
            }

            context.Teams.DeleteOne(t => t.Id == teamId);
            Console.WriteLine($"Team '{team.Name}' deleted!");
        }

        // PLAYER CRUD
        static void PlayerMenu(SoccerCardMongoContext context)
        {
            bool inMenu = true;
            while (inMenu)
            {
                DisplaySubMenu("PLAYERS");
                string choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1":
                        CreatePlayer(context);
                        break;
                    case "2":
                        ReadPlayers(context);
                        break;
                    case "3":
                        UpdatePlayer(context);
                        break;
                    case "4":
                        DeletePlayer(context);
                        break;
                    case "5":
                        inMenu = false;
                        break;
                    default:
                        Console.WriteLine("[!] Invalid choice.");
                        break;
                }

                if (inMenu && choice != "")
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void CreatePlayer(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[+] CREATE PLAYER\n");

            var teams = context.Teams.Find(_ => true).ToList();
            if (teams.Count == 0)
            {
                Console.WriteLine("[!] No teams exist. Create a team first!");
                return;
            }

            Console.WriteLine("Available Teams:");
            foreach (var team in teams)
            {
                Console.WriteLine($"[ID: {team.Id}] {team.Name}");
            }

            Console.Write("\nSelect Team ID: ");
            if (!ObjectId.TryParse(Console.ReadLine(), out ObjectId teamId))
            {
                Console.WriteLine("[!] Invalid Team ID.");
                return;
            }

            var selectedTeam = teams.FirstOrDefault(t => t.Id == teamId);
            if (selectedTeam == null)
            {
                Console.WriteLine("[!] Team not found.");
                return;
            }

            Console.Write("First Name: ");
            string firstName = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Last Name: ");
            string lastName = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Position (e.g., Goalkeeper, Defender, Midfielder, Forward): ");
            string position = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(position))
            {
                Console.WriteLine("[!] First Name, Last Name, and Position are required!");
                return;
            }

            var player = new Player
            {
                Id = ObjectId.GenerateNewId(),
                FirstName = firstName,
                LastName = lastName,
                Position = position,
                Cards = new()
            };

            selectedTeam.Players.Add(player);
            context.Teams.ReplaceOne(t => t.Id == teamId, selectedTeam);
            Console.WriteLine($"Player '{firstName} {lastName}' created! ID: {player.Id}");
        }

        static void ReadPlayers(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[*] ALL PLAYERS\n");

            var teams = context.Teams.Find(_ => true).ToList();
            if (teams.Count == 0)
            {
                Console.WriteLine("[!] No teams found.");
                return;
            }

            foreach (var team in teams)
            {
                Console.WriteLine($"\n[Team: {team.Name}]");
                if (team.Players.Count == 0)
                {
                    Console.WriteLine("  [!] No players");
                    continue;
                }

                foreach (var player in team.Players)
                {
                    Console.WriteLine($"  [ID: {player.Id}] {player.FirstName} {player.LastName} ({player.Position}) | {player.Cards.Count} cards");
                }
            }
        }

        static void UpdatePlayer(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[~] UPDATE PLAYER\n");

            ReadPlayers(context);

            Console.Write("\nEnter Player ID to update: ");
            if (!ObjectId.TryParse(Console.ReadLine(), out ObjectId playerId))
            {
                Console.WriteLine("[!] Invalid ID.");
                return;
            }

            var teams = context.Teams.Find(_ => true).ToList();
            var team = teams.FirstOrDefault(t => t.Players.Any(p => p.Id == playerId));
            var player = team?.Players.FirstOrDefault(p => p.Id == playerId);

            if (player == null)
            {
                Console.WriteLine("[!] Player not found.");
                return;
            }

            Console.Write($"New First Name ({player.FirstName}): ");
            string newFirst = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(newFirst)) player.FirstName = newFirst;

            Console.Write($"New Last Name ({player.LastName}): ");
            string newLast = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(newLast)) player.LastName = newLast;

            Console.Write($"New Position ({player.Position}): ");
            string newPos = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(newPos)) player.Position = newPos;

            context.Teams.ReplaceOne(t => t.Id == team.Id, team);
            Console.WriteLine("Player updated!");
        }

        static void DeletePlayer(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[X] DELETE PLAYER\n");

            ReadPlayers(context);

            Console.Write("\nEnter Player ID to delete: ");
            if (!ObjectId.TryParse(Console.ReadLine(), out ObjectId playerId))
            {
                Console.WriteLine("[!] Invalid ID.");
                return;
            }

            var teams = context.Teams.Find(_ => true).ToList();
            var team = teams.FirstOrDefault(t => t.Players.Any(p => p.Id == playerId));
            var player = team?.Players.FirstOrDefault(p => p.Id == playerId);

            if (player == null)
            {
                Console.WriteLine("[!] Player not found.");
                return;
            }

            team.Players.Remove(player);
            context.Teams.ReplaceOne(t => t.Id == team.Id, team);
            Console.WriteLine($"Player '{player.FirstName} {player.LastName}' deleted!");
        }

        // CARD CRUD 
        static void CardMenu(SoccerCardMongoContext context)
        {
            bool inMenu = true;
            while (inMenu)
            {
                DisplaySubMenu("CARDS");
                string choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1":
                        CreateCard(context);
                        break;
                    case "2":
                        ReadCards(context);
                        break;
                    case "3":
                        UpdateCard(context);
                        break;
                    case "4":
                        DeleteCard(context);
                        break;
                    case "5":
                        inMenu = false;
                        break;
                    default:
                        Console.WriteLine("[!] Invalid choice.");
                        break;
                }

                if (inMenu && choice != "")
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void CreateCard(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[+] CREATE CARD\n");

            var teams = context.Teams.Find(_ => true).ToList();
            var allPlayers = teams.SelectMany(t => t.Players).ToList();

            if (allPlayers.Count == 0)
            {
                Console.WriteLine("[!] No players exist. Create a player first!");
                return;
            }

            Console.WriteLine("Available Players:");
            foreach (var player in allPlayers.Take(10))
            {
                Console.WriteLine($"[ID: {player.Id}] {player.FirstName} {player.LastName}");
            }
            if (allPlayers.Count > 10) Console.WriteLine($"... and {allPlayers.Count - 10} more");

            Console.Write("\nSelect Player ID: ");
            if (!ObjectId.TryParse(Console.ReadLine(), out ObjectId playerId))
            {
                Console.WriteLine("[!] Invalid ID.");
                return;
            }

            var team = teams.FirstOrDefault(t => t.Players.Any(p => p.Id == playerId));
            var player2 = team?.Players.FirstOrDefault(p => p.Id == playerId);

            if (player2 == null)
            {
                Console.WriteLine("[!] Player not found.");
                return;
            }

            Console.Write("Card Number: ");
            string cardNumber = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Rarity (e.g., Common, Rare, Ultra Rare): ");
            string rarity = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Power (1-100): ");
            if (!int.TryParse(Console.ReadLine(), out int power) || power < 1 || power > 100)
            {
                Console.WriteLine("[!] Invalid power value (1-100).");
                return;
            }

            if (string.IsNullOrEmpty(cardNumber) || string.IsNullOrEmpty(rarity))
            {
                Console.WriteLine("[!] Card Number and Rarity are required!");
                return;
            }

            var card = new Card
            {
                Id = ObjectId.GenerateNewId(),
                CardNumber = cardNumber,
                Rarity = rarity,
                Power = power
            };

            player2.Cards.Add(card);
            context.Teams.ReplaceOne(t => t.Id == team.Id, team);
            Console.WriteLine($"Card created! ID: {card.Id}");
        }

        static void ReadCards(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[*] ALL CARDS\n");

            var teams = context.Teams.Find(_ => true).ToList();
            if (teams.Count == 0)
            {
                Console.WriteLine("[!] No teams found.");
                return;
            }

            foreach (var team in teams)
            {
                foreach (var player in team.Players)
                {
                    if (player.Cards.Count > 0)
                    {
                        Console.WriteLine($"\n[{player.FirstName} {player.LastName}]");
                        foreach (var card in player.Cards)
                        {
                            Console.WriteLine($"  [ID: {card.Id}] #{card.CardNumber} | {card.Rarity} | Power: {card.Power}");
                        }
                    }
                }
            }
        }

        static void UpdateCard(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[~] UPDATE CARD\n");

            ReadCards(context);

            Console.Write("\nEnter Card ID to update: ");
            if (!ObjectId.TryParse(Console.ReadLine(), out ObjectId cardId))
            {
                Console.WriteLine("[!] Invalid ID.");
                return;
            }

            var teams = context.Teams.Find(_ => true).ToList();
            var team = teams.FirstOrDefault(t => t.Players.Any(p => p.Cards.Any(c => c.Id == cardId)));
            var player = team?.Players.FirstOrDefault(p => p.Cards.Any(c => c.Id == cardId));
            var card = player?.Cards.FirstOrDefault(c => c.Id == cardId);

            if (card == null)
            {
                Console.WriteLine("[!] Card not found.");
                return;
            }

            Console.Write($"New Card Number ({card.CardNumber}): ");
            string newNumber = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(newNumber)) card.CardNumber = newNumber;

            Console.Write($"New Rarity ({card.Rarity}): ");
            string newRarity = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(newRarity)) card.Rarity = newRarity;

            Console.Write($"New Power ({card.Power}): ");
            if (int.TryParse(Console.ReadLine(), out int newPower) && newPower >= 1 && newPower <= 100)
                card.Power = newPower;

            context.Teams.ReplaceOne(t => t.Id == team.Id, team);
            Console.WriteLine("Card updated!");
        }

        static void DeleteCard(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[X] DELETE CARD\n");

            ReadCards(context);

            Console.Write("\nEnter Card ID to delete: ");
            if (!ObjectId.TryParse(Console.ReadLine(), out ObjectId cardId))
            {
                Console.WriteLine("[!] Invalid ID.");
                return;
            }

            var teams = context.Teams.Find(_ => true).ToList();
            var team = teams.FirstOrDefault(t => t.Players.Any(p => p.Cards.Any(c => c.Id == cardId)));
            var player = team?.Players.FirstOrDefault(p => p.Cards.Any(c => c.Id == cardId));
            var card = player?.Cards.FirstOrDefault(c => c.Id == cardId);

            if (card == null)
            {
                Console.WriteLine("[!] Card not found.");
                return;
            }

            player.Cards.Remove(card);
            context.Teams.ReplaceOne(t => t.Id == team.Id, team);
            Console.WriteLine("Card deleted!");
        }

        // PREMADE QUERIES
        static void QueryMenu(SoccerCardMongoContext context)
        {
            bool inMenu = true;
            while (inMenu)
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║            QUERIES & REPORTS           ║");
                Console.WriteLine("╚════════════════════════════════════════╝\n");
                Console.WriteLine("1. Show Player's Cards");
                Console.WriteLine("2. Show Team Statistics");
                Console.WriteLine("3. Show All Cards by Power");
                Console.WriteLine("4. Back");
                Console.WriteLine("\n[?] Choose an option (1-4): ");

                string choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1":
                        QueryPlayerCards(context);
                        break;
                    case "2":
                        QueryTeamStats(context);
                        break;
                    case "3":
                        QueryCardsByPower(context);
                        break;
                    case "4":
                        inMenu = false;
                        break;
                    default:
                        Console.WriteLine("[!] Invalid choice.");
                        break;
                }

                if (inMenu && choice != "")
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void QueryPlayerCards(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[*] PLAYER'S CARDS\n");

            var teams = context.Teams.Find(_ => true).ToList();
            var allPlayers = teams.SelectMany(t => t.Players).ToList();

            Console.WriteLine("Available Players:");
            foreach (var player in allPlayers.Take(10))
            {
                Console.WriteLine($"[ID: {player.Id}] {player.FirstName} {player.LastName}");
            }

            Console.Write("\nSelect Player ID: ");
            if (!ObjectId.TryParse(Console.ReadLine(), out ObjectId playerId))
            {
                Console.WriteLine("[!] Invalid ID.");
                return;
            }

            var team = teams.FirstOrDefault(t => t.Players.Any(p => p.Id == playerId));
            var player2 = team?.Players.FirstOrDefault(p => p.Id == playerId);

            if (player2 == null)
            {
                Console.WriteLine("[!] Player not found.");
                return;
            }

            Console.Clear();
            Console.WriteLine($"[*] {player2.FirstName} {player2.LastName} ({player2.Position})\n");

            if (player2.Cards.Count == 0)
            {
                Console.WriteLine("[!] No cards found.");
                return;
            }

            foreach (var card in player2.Cards)
            {
                Console.WriteLine($"[ID: {card.Id}] #{card.CardNumber} | {card.Rarity} | Power: {card.Power}");
            }
        }

        static void QueryTeamStats(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[*] TEAM STATISTICS\n");

            var teams = context.Teams.Find(_ => true).ToList();

            Console.WriteLine("Available Teams:");
            foreach (var team in teams)
            {
                Console.WriteLine($"[ID: {team.Id}] {team.Name}");
            }

            Console.Write("\nSelect Team ID: ");
            if (!ObjectId.TryParse(Console.ReadLine(), out ObjectId teamId))
            {
                Console.WriteLine("[!] Invalid ID.");
                return;
            }

            var selectedTeam = teams.FirstOrDefault(t => t.Id == teamId);
            if (selectedTeam == null)
            {
                Console.WriteLine("[!] Team not found.");
                return;
            }

            Console.Clear();
            Console.WriteLine($"[*] {selectedTeam.Name} ({selectedTeam.Country})\n");
            Console.WriteLine($"Total Players: {selectedTeam.Players.Count}");

            int totalCards = selectedTeam.Players.Sum(p => p.Cards.Count);
            int avgPower = selectedTeam.Players.SelectMany(p => p.Cards).Count() > 0
                ? (int)selectedTeam.Players.SelectMany(p => p.Cards).Average(c => c.Power)
                : 0;

            Console.WriteLine($"Total Cards: {totalCards}");
            Console.WriteLine($"Average Power: {avgPower}");

            Console.WriteLine("\n[Players]");
            foreach (var player in selectedTeam.Players)
            {
                Console.WriteLine($"  {player.FirstName} {player.LastName} ({player.Position}) - {player.Cards.Count} cards");
            }
        }

        static void QueryCardsByPower(SoccerCardMongoContext context)
        {
            Console.Clear();
            Console.WriteLine("[*] CARDS SORTED BY POWER\n");

            var teams = context.Teams.Find(_ => true).ToList();
            var allCards = teams
                .SelectMany(t => t.Players)
                .SelectMany(p => p.Cards)
                .OrderByDescending(c => c.Power)
                .ToList();

            if (allCards.Count == 0)
            {
                Console.WriteLine("[!] No cards found.");
                return;
            }

            foreach (var card in allCards)
            {
                var team = teams.FirstOrDefault(t => t.Players.Any(p => p.Cards.Any(c => c.Id == card.Id)));
                var player = team?.Players.FirstOrDefault(p => p.Cards.Any(c => c.Id == card.Id));
                Console.WriteLine($"[Power: {card.Power}] #{card.CardNumber} ({card.Rarity}) - {player?.FirstName} {player?.LastName}");
            }
        }
    }
}