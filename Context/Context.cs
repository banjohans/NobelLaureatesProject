using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MyApp.Models;

namespace MyApp.Context
{
    public class DataContext
    {
        // Stripper JSON ned ein overordna klasse, sidan me neppe får bruk for Laureates på andre planeter trenger me ikkje denne overordna klassen. (Men me beheld filen inntakt, du veit aldri når du oppdager ein nobelprisutdeling på ein annan planet eller dimensjon som gjer at me treng ein ny klasse.)
        public List<Laureate> Laureates { get; set; } = new List<Laureate>();

       public DataContext(string jsonFilePath)
        {
            try
            {
                var json = File.ReadAllText(jsonFilePath);
                // Hadde udefinert problem med JSON, som gav behov for å logge og debugge fleire steg her.
                Console.WriteLine("JSON-fil lastet inn.");

                // Forsøkte å debugge med å fjerne upper og lowercase sensitivitet, det fungerte ikkje.
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // La inn den nye options var'en her, som ikkje fungerte, men lot den stå likevel - likegreit
                var root = JsonSerializer.Deserialize<Root>(json, options);

                if (root?.Laureates != null)
                {
                    foreach (var laureate in root.Laureates)
                    {
                        foreach (var prize in laureate.Prizes)
                        {
                            // Feilen blei oppdaga i opplistinga av det følgende skrittet. Det forekommer mange doble lister av typen "[[]]" under "affiliations" (sjekk JSON, det er over 260 slike oppføringer)
                            // I stedet for å endre dataene i JSON lista (som enkelt gjøres med å fjerne samtlige indre square-brackets, blir feilen her adressert ved følgende kode som fjerner ugyldige elementer under Affilitaion. Dumt for dataprosessering, bra for å forstå litt meir om koding)
                            if (prize.Affiliations.Any(a => a is null || a.GetType() != typeof(Affiliation)))
                            {
                                Console.WriteLine("Ugyldig affiliations oppdaget. Rensing pågår.");
                                // Dette er hinsides mitt nåværende nivå, men bruker ein LINQ metode som filtrerer bort ugyldige elementer.
                                prize.Affiliations = prize.Affiliations
                                    .Where(a => a != null && a.GetType() == typeof(Affiliation))
                                    .ToList();
                            }
                        }
                    }
                    Laureates = root.Laureates;
                    // Del av debugginga, denne var på 0 før rensinga fiksa biffen - tallet frå denne JSON lista skal være 1004 prismottakarar.
                    Console.WriteLine($"Antall laureater lastet: {Laureates.Count}");
                }
                else
                {
                    Console.WriteLine("Ingen laureater funnet i JSON-filen.");
                    Laureates = new List<Laureate>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Feil ved lesing eller deserialisering av JSON: {ex.Message}");
                Laureates = new List<Laureate>();
            }
        }
    }
}
