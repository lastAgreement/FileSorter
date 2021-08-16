using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileSorter
{
    public static class FileSorter
    {
        // оптимальный batchSize зависит от доступной оперативной памяти
        public static void ExternalSort(string path, int batchSize = 2)
        {
            string directoryPath = path + " parts";
            CreateDirectory(directoryPath);

            List<string> parts = CreateParts(path, directoryPath, batchSize);
            while (parts.Count > 1)
            {
                parts = MergeParts(directoryPath, parts);
            }

            File.Delete(path);
            Directory.Move(parts[0], path);
            Directory.Delete(directoryPath, true);
        }

        private static List<string> MergeParts(string directoryPath, List<string> parts)
        {
            List<string> newParts = new List<string>();
            for (var i = 0; i < parts.Count - 1 ; i += 2)
            {
                string newPartPath = directoryPath + "\\" + parts.Count + "." + (newParts.Count + 1).ToString() + ".txt";
                newParts.Add(newPartPath);
                MergeTwoParts(newPartPath, parts[i], parts[i+1]);
            }
            if (parts.Count % 2 == 1) newParts.Add(parts[parts.Count-1]);
            return newParts;
        }

        private static void MergeTwoParts(string newPartPath, string part1, string part2)
        {
            using (StreamReader reader1 = new StreamReader(part1))
            using (StreamReader reader2 = new StreamReader(part2))
            using (FileStream fs = File.Create(newPartPath))
            {
                string s1 = ReadLine(reader1);
                string s2 = ReadLine(reader2);

                while (!string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2))
                {
                    if (String.Compare(s1, s2) < 0)
                    {
                        WriteString(fs, s1);
                        s1 = ReadLine(reader1);
                    }
                    else
                    {
                        WriteString(fs, s2);
                        s2 = ReadLine(reader2);
                    }
                }

                if (!string.IsNullOrEmpty(s1))
                {
                    WriteString(fs, s1);
                    FinishMergingTwoParts(reader1, fs);
                }
                else if (!string.IsNullOrEmpty(s2))
                {
                    WriteString(fs, s2);
                    FinishMergingTwoParts(reader2, fs);
                }
            }
        }

        private static string ReadLine(StreamReader reader)
        {
            return reader.EndOfStream ? null : reader.ReadLine();
        }

        private static void FinishMergingTwoParts(StreamReader reader, FileStream fs)
        {
            while (!reader.EndOfStream)
            {
                WriteString(fs, reader.ReadLine());
            }
        }

        private static void CreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
        }

        private static void WriteString(FileStream fs, string s)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(s + "\n");
            fs.Write(info, 0, info.Length);
        }

        private static List<string> CreateParts(string path, string directoryPath, int rowsPerPart)
        {
            List<string> parts = new List<string>();
            List<string> rows = new List<string>();

            StreamReader reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                rows.Add(reader.ReadLine());
                if (rows.Count >= rowsPerPart)
                {
                    CreatePart(directoryPath, rows, parts);
                    rows.Clear();
                }
            }
            reader.Close();

            if (rows.Count > 0)
            {
                CreatePart(directoryPath, rows, parts);
            }

            return parts;
        }

        private static void CreatePart(string directoryPath, List<string> rows, List<string> parts)
        {
            rows.Sort();
            string newPartPath = directoryPath + "\\start." + (parts.Count + 1).ToString() + ".txt";
            parts.Add(newPartPath);
            using (FileStream fs = File.Create(newPartPath))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(string.Join("\n", rows));
                fs.Write(info, 0, info.Length);
            }
        }
    }
}
