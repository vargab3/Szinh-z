using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SzinhaziJegyertekesoProgram
{
    class Reservation
    {
        public string Name { get; set; }
        public int Row { get; set; }
        public int Seat { get; set; }
    }

    class Program
    {
        private static List<bool> seatStatus = new List<bool>(Enumerable.Repeat(false, 240)); // 16 sor * 15 szék
        private static List<Reservation> reservations = new List<Reservation>();
        private static Random random = new Random();

        static void Main(string[] args)
        {
            LoadReservations();
            InitializeSeats();

            while (true)
            {
                Console.Clear();
                PrintMenu();
                Console.Write("Válasszon menüpontot: ");
                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Hibás választás!");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        ShowAvailableSeats();
                        break;
                    case 2:
                        ReserveSeatsManually();
                        break;
                    case 3:
                        ReserveSeatsRandomly();
                        break;
                    case 4:
                        ModifyReservation();
                        break;
                    case 5:
                        CancelReservation();
                        break;
                    case 6:
                        PrintReservations();
                        break;
                    case 7:
                        SaveReservations();
                        Console.WriteLine("Foglalások sikeresen mentve.");
                        string dateStr = Console.ReadLine();
                        break;
                    case 8:
                        LoadReservations();
                        Console.WriteLine("Foglalások sikeresen betöltve.");
                        string dateStr2 = Console.ReadLine();
                        break;
                    case 9:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Hibás választás!");
                        break;
                }
            }
        }

        static void PrintMenu()
        {
            Console.WriteLine("=== Színházi Jegyértékesítő Program ===");
            Console.WriteLine("1. Szabad helyek megjelenítése");
            Console.WriteLine("2. Helyfoglalás manuálisan");
            Console.WriteLine("3. Helyfoglalás véletlenszerűen");
            Console.WriteLine("4. Foglalás módosítása");
            Console.WriteLine("5. Foglalás törlése");
            Console.WriteLine("6. Foglalások listázása");
            Console.WriteLine("7. Foglalások mentése");
            Console.WriteLine("8. Foglalások betöltése");
            Console.WriteLine("9. Kilépés");
        }

        static void InitializeSeats()
        {
            // Kezdetben a helyek 10%-a foglalt legyen
            int numReserved = seatStatus.Count / 10;
            for (int i = 0; i < numReserved; i++)
            {
                int randomSeat = random.Next(seatStatus.Count);
                seatStatus[randomSeat] = true;
            }
        }

        static void ShowAvailableSeats()
        {
            Console.WriteLine("Szabad helyek:");
            for (int i = 0; i < seatStatus.Count; i++)
            {
                if (!seatStatus[i])
                {
                    int row = i / 15 + 1;
                    int seat = i % 15 + 1;
                    Console.WriteLine($"Sor: {row}, Szék: {seat}");
                }
            }
            Console.WriteLine();
            string dateStr = Console.ReadLine();
        }

        static void ReserveSeatsManually()
        {
            Console.Write("Adja meg a foglaló nevét: ");
            string name = Console.ReadLine();

            Console.Write("Hány jegyet szeretne foglalni? ");
            int numTickets;
            if (!int.TryParse(Console.ReadLine(), out numTickets) || numTickets <= 0)
            {
                Console.WriteLine("Hibás bemenet!");
                return;
            }

            for (int i = 0; i < numTickets; i++)
            {
                Console.Write($"Adja meg a(z) {i + 1}. jegy sorát (1-16): ");
                int row;
                if (!int.TryParse(Console.ReadLine(), out row) || row < 1 || row > 16)
                {
                    Console.WriteLine("Hibás sor!");
                    return;
                }

                Console.Write($"Adja meg a(z) {i + 1}. jegy székét (1-15): ");
                int seat;
                if (!int.TryParse(Console.ReadLine(), out seat) || seat < 1 || seat > 15)
                {
                    Console.WriteLine("Hibás szék!");
                    return;
                }

                int index = (row - 1) * 15 + (seat - 1);
                if (seatStatus[index])
                {
                    Console.WriteLine("Ez a szék már foglalt!");
                    i--; // Visszaállunk az előző lépésre
                }
                else
                {
                    seatStatus[index] = true;
                    reservations.Add(new Reservation { Name = name, Row = row, Seat = seat });
                }
            }

            Console.WriteLine("Foglalások sikeresen végrehajtva!");
        }

        static void ReserveSeatsRandomly()
        {
            Console.Write("Adja meg a foglaló nevét: ");
            string name = Console.ReadLine();

            Console.Write("Hány jegyet szeretne foglalni? ");
            int numTickets;
            if (!int.TryParse(Console.ReadLine(), out numTickets) || numTickets <= 0)
            {
                Console.WriteLine("Hibás bemenet!");
                return;
            }

            for (int i = 0; i < numTickets; i++)
            {
                int randomIndex;
                do
                {
                    randomIndex = random.Next(seatStatus.Count);
                } while (seatStatus[randomIndex]);

                int row = randomIndex / 15 + 1;
                int seat = randomIndex % 15 + 1;
                seatStatus[randomIndex] = true;
                reservations.Add(new Reservation { Name = name, Row = row, Seat = seat });
            }

            Console.WriteLine("Foglalások sikeresen végrehajtva!");
        }

        static void ModifyReservation()
        {
            Console.Write("Adja meg a foglaló nevét: ");
            string name = Console.ReadLine();

            var userReservations = reservations.Where(r => r.Name == name).ToList();
            if (userReservations.Count == 0)
            {
                Console.WriteLine("Nincs foglalás ezzel a névvel!");
                return;
            }

            Console.WriteLine("Az Ön foglalásai:");
            for (int i = 0; i < userReservations.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Sor: {userReservations[i].Row}, Szék: {userReservations[i].Seat}");
            }

            Console.Write("Válassza ki a módosítani kívánt foglalást (számot): ");
            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > userReservations.Count)
            {
                Console.WriteLine("Hibás választás!");
                return;
            }

            var chosenReservation = userReservations[choice - 1];
            Console.Write("Adja meg az új sor számát (1-16): ");
            int newRow;
            if (!int.TryParse(Console.ReadLine(), out newRow) || newRow < 1 || newRow > 16)
            {
                Console.WriteLine("Hibás sor szám!");
                return;
            }

            Console.Write("Adja meg az új szék számát (1-15): ");
            int newSeat;
            if (!int.TryParse(Console.ReadLine(), out newSeat) || newSeat < 1 || newSeat > 15)
            {
                Console.WriteLine("Hibás szék szám!");
                return;
            }

            int newIndex = (newRow - 1) * 15 + (newSeat - 1);
            if (seatStatus[newIndex])
            {
                Console.WriteLine("Ez a szék már foglalt!");
                return;
            }

            seatStatus[(chosenReservation.Row - 1) * 15 + (chosenReservation.Seat - 1)] = false;
            seatStatus[newIndex] = true;
            chosenReservation.Row = newRow;
            chosenReservation.Seat = newSeat;

            Console.WriteLine("Foglalás sikeresen módosítva!");
        }

        static void CancelReservation()
        {
            Console.Write("Adja meg a foglaló nevét: ");
            string name = Console.ReadLine();

            var userReservations = reservations.Where(r => r.Name == name).ToList();
            if (userReservations.Count == 0)
            {
                Console.WriteLine("Nincs foglalás ezzel a névvel!");
                return;
            }

            Console.WriteLine("Az Ön foglalásai:");
            for (int i = 0; i < userReservations.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Sor: {userReservations[i].Row}, Szék: {userReservations[i].Seat}");
            }

            Console.Write("Válassza ki a törölni kívánt foglalást (számot): ");
            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > userReservations.Count)
            {
                Console.WriteLine("Hibás választás!");
                return;
            }

            var chosenReservation = userReservations[choice - 1];
            seatStatus[(chosenReservation.Row - 1) * 15 + (chosenReservation.Seat - 1)] = false;
            reservations.Remove(chosenReservation);

            Console.WriteLine("Foglalás sikeresen törölve!");
        }

        static void PrintReservations()
        {
            Console.WriteLine("Összes foglalás:");
            foreach (var reservation in reservations)
            {
                Console.WriteLine($"Név: {reservation.Name}, Sor: {reservation.Row}, Szék: {reservation.Seat}");
                string dateStr = Console.ReadLine();
            }
        }

        static void SaveReservations()
        {
            using (StreamWriter writer = new StreamWriter("foglalasok.txt"))
            {
                foreach (var reservation in reservations)
                {
                    writer.WriteLine($"{reservation.Name},{reservation.Row},{reservation.Seat}");
                }
            }
        }

        static void LoadReservations()
        {
            if (File.Exists("foglalasok.txt"))
            {
                using (StreamReader reader = new StreamReader("foglalasok.txt"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length == 3 && int.TryParse(parts[1], out int row) && int.TryParse(parts[2], out int seat))
                        {
                            reservations.Add(new Reservation { Name = parts[0], Row = row, Seat = seat });
                            seatStatus[(row - 1) * 15 + (seat - 1)] = true;
                        }
                    }
                }
            }
        }
    }
}
