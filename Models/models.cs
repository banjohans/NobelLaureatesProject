namespace MyApp.Models
{
    public class Root
    {
        public List<Laureate>? Laureates {get; set;}
    }
    public class Affiliation
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public class Prize
    {
        public string? Year { get; set; }
        public string? Category { get; set; }
        public string? Share { get; set; }
        public string? Motivation { get; set; }
        public List<Affiliation> Affiliations { get; set; } = new();
    }

    public class Laureate
    {
        public string? Id { get; set; }
        public string? Firstname { get; set; }
        public string? Surname { get; set; }
        public string? Born { get; set; }
        public string? Died { get; set; }
        public string? BornCountry { get; set; }
        public string? BornCountryCode { get; set; }
        public string? BornCity { get; set; }
        public string? DiedCountry { get; set; }
        public string? DiedCountryCode { get; set; }
        public string? DiedCity { get; set; }
        public string? Gender { get; set; }
        public List<Prize> Prizes { get; set; } = new();
    }
}
