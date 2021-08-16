using System;

namespace FileSorter
{
    class Program
    {

        static void Main(string[] args)
        {
            string path = "E:\\Temp\\Test2.txt";

            FileGenerator.Generate(path, 100000, 1500);
            FileSorter.ExternalSort(path, 150);
        }

    }
}
