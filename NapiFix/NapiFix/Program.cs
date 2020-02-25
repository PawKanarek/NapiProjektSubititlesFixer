using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NapiFix
{
    internal class Program
    {
        private static readonly Dictionary<char, char> shittyChars = new Dictionary<char, char>
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
            ['Œ'] = 'Ś',
            ['£'] = 'Ł',
        };

        private static async Task Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            var currentDir = Directory.GetCurrentDirectory();
            var files = Directory.GetFiles(currentDir);
            var subitlesExtensions = new List<string> { ".txt", ".srt" };
            var napiFiles = files.Where(f => subitlesExtensions.Any(e => e.Equals(Path.GetExtension(f), StringComparison.OrdinalIgnoreCase)));

            var tasks = new List<Task>(napiFiles.Count());
            foreach (var txtFile in napiFiles)
            {
                tasks.Add(Task.Run(() =>
                {
                    var encoding = Encoding.Default;
                    var text = string.Empty;
                    var reader = new StreamReader(txtFile, encoding, true);
                    text = reader.ReadToEnd();
                    encoding = reader.CurrentEncoding;
                    //Console.WriteLine($"{txtFile.Split("\\").Reverse().FirstOrDefault()}, encoding {encoding}");
                    if (text.Contains("�")) // fallback for shitty encoding (probably windows-1252
                    {
                        reader.Dispose();
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                        encoding = Encoding.GetEncoding(1252);
                        reader = new StreamReader(txtFile, encoding, false);
                        text = reader.ReadToEnd();
                        encoding = Encoding.Default;
                    }

                    foreach (var shittyChar in shittyChars)
                    {
                        text = text.Replace(shittyChar.Key, shittyChar.Value);
                    }

                    reader.Dispose();
                    File.WriteAllText(txtFile, text, encoding);
                    Console.WriteLine($"Fixed: {txtFile}");
                }));
            }
            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine($"Fixed {tasks.Count} file{(tasks.Count > 1 ? "s" : string.Empty)}. Task took {stopwatch.ElapsedMilliseconds}ms.");
        }
    }
}
