using MyApp.Context;
using MyApp.Models;
using System.Text.Json;

var context = new DataContext("NobelLaureates.json");

// Kode for printing av alle laureates dersom man trenger det.
// Console.WriteLine($"Antall laureater lastet: {context.Laureates.Count}");
// foreach (var laureate in context.Laureates)
// {
//     Console.WriteLine($"ID: {laureate.Id}, Navn: {laureate.Firstname} {laureate.Surname}");
// }

// Select() Hent ut første og siste navn
var selectQuery = context.Laureates
    .Select(l => new { l.Firstname, l.Surname });
File.WriteAllText("selectQuery.json", JsonSerializer.Serialize(selectQuery));

// SelectMany() Hent ut fullt navn og priskategori
var selectManyQuery = context.Laureates
    .SelectMany(l => l.Prizes != null
        ? l.Prizes.Select(p => new { Laureate = l.Firstname + " " + l.Surname, p.Category })
        : Enumerable.Empty<object>()); // Ensure the type matches the anonymous type
File.WriteAllText("selectManyQuery.json", JsonSerializer.Serialize(selectManyQuery));

// GroupBy() Hent ut Priskategorier og tell opp kategoriene etter antall priser
var groupByQuery = context.Laureates
    .SelectMany(l => l.Prizes?.Select(p => p.Category) ?? Enumerable.Empty<string>())
    .GroupBy(c => c)
    .Select(g => new { Category = g.Key, Count = g.Count() });
File.WriteAllText("groupByQuery.json", JsonSerializer.Serialize(groupByQuery));

Console.WriteLine("Spørringer fullført og lagret i JSON-filer.");

// Grav ned i laureate -> pris -> affiliation -> country for å matche med "Norway"
var norwegianPrizes = context.Laureates
    .SelectMany(laureate => 
        laureate.Prizes.SelectMany(prize =>
            prize.Affiliations
                .Where(affiliation => affiliation.Country == "Norway")
                .DefaultIfEmpty() // Legg til en tom affiliation hvis listen er tom
                
                // sidan affiliate-listen var nok så mangefull bestemte eg meg for å inkludere kode som tar med fødeland, 
                .Select(affiliation => new
                {
                    Laureate = $"{laureate.Firstname} {laureate.Surname}",
                    PrizeCategory = prize.Category,
                    PrizeYear = prize.Year,
                    AffiliationName = affiliation?.Name ?? "Not Available",
                    AffiliationCity = affiliation?.City ?? "Not Available",
                    BornCountry = laureate.BornCountry
                })))
// kontroller BornCountry opp mot "Norway" og tar oppføringen med
    .Where(result => result.AffiliationName == "Not Available" && result.BornCountry?.Contains("Norway") == true ||
                     result.AffiliationName != "Not Available") // Inkluderer både Affiliations og BornCountry med Norway
    .ToList();

// Serialiser til JSON
File.WriteAllText("groupByNorgasm.json", JsonSerializer.Serialize(norwegianPrizes));

// Vis resultater i konsollen
Console.WriteLine("Resultater for 'Norway':");
foreach (var prize in norwegianPrizes)
{
    Console.WriteLine($"Laureate: {prize.Laureate}, Category: {prize.PrizeCategory}, Year: {prize.PrizeYear}");
    Console.WriteLine($"Affiliation: {prize.AffiliationName}, City: {prize.AffiliationCity}, BornCountry: {prize.BornCountry}");
    Console.WriteLine();
}

// List tilgjengelige kategorier
var categories = context.Laureates
    .SelectMany(l => l.Prizes.Select(p => p.Category))
    .Distinct()
    .OrderBy( c => c)
    .ToList();

Console.WriteLine("Tilgjengelige priskategorier:");
foreach(var cat in categories)
{
    Console.WriteLine($"- {cat}");
}

// Be brukeren om å oppgi søkekriterier
Console.WriteLine("\nSkriv inn søkekriterier:");
Console.WriteLine("Velg en kategori fra listen, eller la stå tom for å søke på alle kategorier: ");
string? selectedCategory = Console.ReadLine();

Console.Write("Skriv inn et år mellom 1901 og 2024, eller la stå tom for å søke på alle år: ");
string? selectedYear = Console.ReadLine();

// Utfør søket
var searchResults = context.Laureates
    .SelectMany(laureate => laureate.Prizes
        .Where(prize =>
            (string.IsNullOrEmpty(selectedCategory) || prize.Category?.Equals(selectedCategory, StringComparison.OrdinalIgnoreCase) == true) &&
            (string.IsNullOrEmpty(selectedYear) || prize.Year == selectedYear))
        .Select(prize => new
        {
            Laureate = $"{laureate.Firstname} {laureate.Surname}",
            Category = prize.Category,
            Year = prize.Year
        }))
    .ToList();

// Vis søkeresultatene
if (searchResults.Any())
{
    Console.WriteLine("\nSøkeresultater:");
    foreach (var result in searchResults)
    {
        Console.WriteLine($"Laureate: {result.Laureate}, Category: {result.Category}, Year: {result.Year}");
    }
}
else
{
    Console.WriteLine("\nIngen resultater funnet for de oppgitte kriteriene.");
}
