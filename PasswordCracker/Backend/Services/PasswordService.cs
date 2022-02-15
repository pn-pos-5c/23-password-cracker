using HtmlAgilityPack;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Services;

public class PasswordService
{
    private const string password0 = "A746222F09D85605C52D4E636788D6FFDC274698B98B8C5F3244C06958683A69";
    private const string password1 = "3086E346353248775A2C5D74E36A9C9B9BD226A1EE401F830AC499633DC00031";
    private const string password2 = "26775436073E00D207E192857EE3730CFCA19DE16F01F0780096EF217C2919EF";
    private const string password3 = "43C19A093B34B581DDCC7207F6BD094F6940DB69F035C444425ED84D2CAC037D";

    private bool running = true;
    private string result = "* no match *";

    private int threadCount = Environment.ProcessorCount;

    private long totalValues;
    private long valuesCalced = 0;

    public async Task<string> CrackPassword(string hash, string alphabet, int length)
    {
        var hashBytes = HexStringToByte(hash);
        var alphabetBytes = Encoding.UTF8.GetBytes(alphabet);

        totalValues = (long)Math.Pow(alphabet.Length, length);

        await Crack(hashBytes, alphabetBytes, length);
        return result;
    }

    public async Task<string> CrackPassword2(string hash)
    {
        var hashBytes = HexStringToByte(hash);
        var web = new HtmlWeb();
        HtmlDocument document = web.Load("https://de.wikipedia.org/wiki/Liste_von_Fabelwesen");
        HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//*[@id=\"mw-content-text\"]/div/ul/li/a");

        var sha256 = SHA256.Create();
        foreach (var word in nodes.Select(node => node.InnerHtml))
        {
            byte[] pwHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(word));
            if (pwHash.SequenceEqual(hashBytes))
            {
                result = word;
                return result;
            }
        }

        return result;
    }

    private async Task Crack(byte[] hash, byte[] alphabet, int length)
    {
        using var sha256 = SHA256.Create();
        var pw = new byte[length];
        var indices = new int[length];

        for (int i = 0; i < length; i++)
        {
            pw[i] = alphabet[0];
        }

        while (running)
        {
            byte[] pwHash = sha256.ComputeHash(pw);
            if (pwHash.SequenceEqual(hash))
            {
                result = Encoding.UTF8.GetString(pw);
                running = false;
                return;
            }

            valuesCalced++;
            indices[0]++;

            for (int i = 0; i < indices.Length; i++)
            {
                if (indices[i] == alphabet.Length)
                {
                    if (i == length - 1) return;

                    indices[i] = 0;
                    indices[i + 1]++;
                    pw[i] = alphabet[indices[i]];
                }
                else
                {
                    pw[i] = alphabet[indices[i]];
                    break;
                }
            }
        }
    }

    // ©Michael Wiesinger
    public static byte[] HexStringToByte(string hex)
    {
        var data = new byte[hex.Length / 2];
        for (var i = 0; i < data.Length; i++)
        {
            string byteValue = hex.Substring(i * 2, 2);
            data[i] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return data;
    }

    public double CalculateProgress()
    {
        Console.WriteLine(valuesCalced);
        Console.WriteLine(totalValues);
        return (double)valuesCalced / totalValues;
    }
}
