using System;
using System.Text;
using System.IO;
using System.Linq;

namespace FileSorter
{
    public static class FileGenerator
    {
        public static void Generate(string fileName, long rowsCount, int rowLength)
        {
            using (FileStream fs = File.Create(fileName))
            {
                for (var i = 0; i < rowsCount ; i++)
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(GenerateRandomString(rowLength) + "\n");
                    fs.Write(info, 0, info.Length);
                }
            }

        }
        private static Random random = new Random();
        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, random.Next(1, length))
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
