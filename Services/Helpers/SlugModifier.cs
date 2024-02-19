namespace Services.Helpers;

public static class SlugModifier
{
    public static string GenerateUniqueHash()
    {
        string guid = Guid.NewGuid().ToString("N");

        
        int length = 3;
        if (guid.Length < length)
        {
            length = guid.Length;
        }

        string uniqueHash = guid.Substring(0, length);

        return uniqueHash;
    }
    
    public static string ReplaceTurkishCharacters(string input)
    {
        input = input.Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c");
        input = input.Replace("İ", "i").Replace("Ğ", "g").Replace("Ü", "u").Replace("Ş", "s").Replace("Ö", "o").Replace("Ç", "c");
        return input;
    }
    
    public static string RemoveNonAlphanumericAndSpecialChars(string input)
    {
        // LINQ kullanarak boşluk, tire ve nokta karakterlerini filtrele
        var filteredCharacters = input
            .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-')
            .ToArray();

        // Filtrelenmiş karakterleri yeni bir string olarak oluştur
        string result = new string(filteredCharacters);

        return result;
    }
}