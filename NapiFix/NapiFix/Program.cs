using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NapiFix
{
    internal class Program
    {
        static Dictionary<char, char> shittyChars = new Dictionary<char, char>
        {
            ['¹'] = 'ą',
            ['æ'] = 'ć',
            ['ê'] = 'ę',
            ['ñ'] = 'ń',
            ['³'] = 'ł',
            ['œ'] = 'ś',
            ['¿'] = 'ż',
            ['¯'] = 'Ż',
            ['Ÿ'] = 'ź',
        };

        private static async Task Main(string[] args)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var files = Directory.GetFiles(currentDir);
            var subitlesExtensions = new List<string> { ".txt", ".srt" };
            IEnumerable<string> napiFiles = files.Where(f => subitlesExtensions.Any(e => e.Equals(Path.GetExtension(f), StringComparison.OrdinalIgnoreCase)));

            foreach (var txtFile in napiFiles)
            {
                string text = File.ReadAllText(txtFile);
                foreach (var shittyChar in shittyChars)
                {
                    text = text.Replace(shittyChar.Key, shittyChar.Value);
                }
                File.WriteAllText(txtFile, text, Encoding.UTF8);
                Console.WriteLine($"Fixed: {txtFile}");
            }
        }
    }
}
