public class Program
{

    static List<string> vowels = new() { "a", "e", "i","o", "u",
       "ā", "ē", "ī", "ō", "ū", // 'a with macron' etc
        "é"
    }; 

    static bool IsVowel(string character)
    {
        return vowels.Contains(character, StringComparer.OrdinalIgnoreCase);
    }


    static VowelData Process(string place)
    {
        return new VowelData(place, place.Replace(" ", null).Length, place.Count(c => IsVowel(c.ToString())));
    }

    public static async Task Main()
    {
        // source: https://gazetteer.linz.govt.nz/gaz.csv

        List<VowelData> data = new();
        var placeData = (await File.ReadAllLinesAsync("NZPlaceData.txt")).Distinct();

        Console.WriteLine($"Place count: {placeData.Count()}");

        Console.WriteLine();

        foreach (var place in placeData) {
            data.Add(Process(place));
        }

        var allVowels = data.Where(v => v.VowelPercentage() == 100);
        allVowels.Output("All vowels");

        Console.WriteLine();

        var mostVowels = data.OrderByDescending(d => d.Vowels).Take(3);
        
        mostVowels.Output("Most vowels");
        Console.WriteLine();

        var leastVowels = data.OrderBy(d => d.Vowels).Take(3);

        leastVowels.Output("Least vowels");
        Console.WriteLine();

        var averageVowels = data.Average(d => d.Vowels);

        Console.WriteLine($"Average vowels: {averageVowels:0.00}");
        Console.WriteLine();

        var percentageVowels = data.Average(d => d.VowelPercentage());

        Console.WriteLine($"Average vowel percentage: {percentageVowels:0.00}");

        Console.WriteLine();

        var averageLength = data.Average(d => d.Characters);

        Console.WriteLine($"Average place length (excluding spaces): {averageLength:0.00}");

        Console.WriteLine();
    }
}

public record VowelData(string Place, int Characters, int Vowels) {
    public override string ToString()
    {
        return $"Place: {Place} Vowels: {Vowels} Percentage: {this.VowelPercentage():0.00}";
    }
};

public static class Extensions
{
    public static decimal VowelPercentage(this VowelData data)
    {
        if (data.Vowels == 0) {
            return 0;
        }

        return (decimal)data.Vowels / data.Characters * 100 ;
    }

    public static IEnumerable<VowelData> GetData(this IEnumerable<VowelData> data, Func<IEnumerable<VowelData>, IEnumerable<VowelData>> selector)
    {
        return selector(data);
    }

    public static void Output(this IEnumerable<VowelData> data, string text)
    {
        Console.WriteLine(text);

        foreach (var d in data)
        {
            Console.WriteLine(d.ToString());
        }
    }
}