using System.Globalization;
using System.Numerics;
using System.Linq;

namespace LaptopBerles
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Kölcsönzés> kölcsönzések = new List<Kölcsönzés>();

            using (StreamReader sr = new StreamReader("laptoprentals2022.csv"))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string row = sr.ReadLine();
                    string[] i = row.Split(';');


                    Bérlő bérlő = new Bérlő
                    {
                        PersonalId = i[0],
                        Name = i[1],
                        DateOfBirth = DateOnly.Parse(i[2]),
                        PostCode = Convert.ToInt32(i[3]),
                        City = i[4],
                        Address = i[5]
                    };

                    Laptop laptop = new Laptop
                    {
                        Invnumber = i[6],
                        Model = i[7],
                        County = i[8],
                        RAM = Convert.ToInt32(i[9]),
                        Color = i[10],
                        DailyFee = Convert.ToInt32(i[11]),
                        Deposit = Convert.ToInt32(i[12])
                    };

                    Kölcsönzés kölcsönzés = new Kölcsönzés
                    {
                        Bérlő = bérlő,
                        Laptop = laptop,
                        StartDate = DateOnly.Parse(i[13]),
                        EndDate = DateOnly.Parse(i[14]),
                        UseDeposit = i[15] == "0" ? false : true,
                        Uptime = Convert.ToDouble(i[16])
                    };

                    kölcsönzések.Add(kölcsönzés);
                }
            }

            Console.WriteLine($"3. feladat: Bérletek száma:  {kölcsönzések.Count}");

            //---------------------------------------------
            Console.WriteLine($"4. feladat: ");
            var acerSzurke = kölcsönzések.Where(k => k.Laptop.Model.ToLower().Contains("acer") && k.Laptop.Color.ToLower() == "szürke")
                                        .OrderBy(k => k.Laptop.Invnumber);

            foreach (var item in acerSzurke)
            {
                Console.WriteLine($"{item.Laptop.Invnumber} - {item.Bérlő.Name}, {item.StartDate} - {item.EndDate}");
            }

            //---------------------------------------------
            Console.WriteLine("\n5. feladat: Két legkevesebb bérlésű vármegye:");
            var wiiw = kölcsönzések
                .GroupBy(k => k.Laptop.County)
                .Select(g => new { County = g.Key, Count = g.Count() })
                .OrderBy(x => x.Count)
                .Take(2);

            foreach (var county in wiiw)
            {
                Console.WriteLine($"{county.County} ({county.Count} db)");
            }

            //---------------------------------------------
            Console.WriteLine("\n6. feladat: Keresés leltári szám alapján (kilépés: 0):");
            while (true)
            {
                Console.Write("Kérem a leltári számot (pl. LPT123456): ");
                string input = Console.ReadLine();

                if (input == "0") break;

                if (!System.Text.RegularExpressions.Regex.IsMatch(input, @"^LPT\d{6}$"))
                {
                    Console.WriteLine("Hibás formátum! (Helyes: LPT és 6 számjegy)");
                    break;
                }

                var found = kölcsönzések.Where(k => k.Laptop.Invnumber == input).ToList();

                if (found.Any())
                {
                    foreach (var item in found)
                    {
                        Console.WriteLine($"{item.Laptop.Invnumber} - {item.Bérlő.Name}, {item.StartDate} - {item.EndDate}, napi díj: {item.Laptop.DailyFee} Ft, kaució: {item.Laptop.Deposit} Ft, sérült: {item.UseDeposit}");
                        break;
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("Nincs ilyen leltári számú laptop a beolvasott adatok között!");
                    break;
                }
            }

            //---------------------------------------------
            Console.WriteLine("\n7. feladat: A cég teljes bevétele :");

            int total = 0;
            foreach (var kölcsönzés in kölcsönzések)
            {
                int rentalDays = kölcsönzés.EndDate.DayNumber - kölcsönzés.StartDate.DayNumber + 1;
                int rentalFee = rentalDays * kölcsönzés.Laptop.DailyFee;
                int depositUsed = kölcsönzés.UseDeposit ? kölcsönzés.Laptop.Deposit : 0;
                total += rentalFee + depositUsed;
            }

            Console.WriteLine($"Bevétel: {total:N0} Ft");

            //---------------------------------------------
            Console.Write("Kérem a leltári számot (pl. LPT123456): ");
            string inputos = Console.ReadLine();

            double AvgUpTime2(List<Kölcsönzés> kölcsönzések, string invnumber)
            {
                var rentalList = kölcsönzések.Where(k => k.Laptop.Invnumber == invnumber).ToList();
                return rentalList.Count > 1 ? rentalList.Average(k => k.Uptime) : 0;
            }

            Console.WriteLine("\n8. feladat: Átlagos üzemidő kiszámítása:");
            double avg = Math.Round(AvgUpTime2(kölcsönzések, inputos), 2);
            Console.WriteLine($"{inputos} leltári számú laptop átlagos üzemideje {avg} óra/bérlés");
        }

        public class Bérlő
        {
            public string PersonalId { get; set; }
            public string Name { get; set; }
            public DateOnly DateOfBirth { get; set; }
            public int PostCode { get; set; }
            public string City { get; set; }
            public string Address { get; set; }
        }

        public class Laptop
        {
            public string Invnumber { get; set; }
            public string Model { get; set; }
            public string County { get; set; }
            public int RAM { get; set; }
            public string Color { get; set; }
            public int DailyFee { get; set; }
            public int Deposit { get; set; }
        }

        public class Kölcsönzés
        {
            public Bérlő Bérlő { get; set; }
            public Laptop Laptop { get; set; }
            public DateOnly StartDate { get; set; }
            public DateOnly EndDate { get; set; }
            public bool UseDeposit { get; set; }
            public double Uptime { get; set; }
        }
    }
}
