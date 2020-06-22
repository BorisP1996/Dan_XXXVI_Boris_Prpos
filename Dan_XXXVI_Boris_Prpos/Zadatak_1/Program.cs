using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak_1
{
    class Program
    {
        static List<int> list = new List<int>(10000);
        static List<int> oddNumberList = new List<int>();
        static string path = @"../../OddNumbers.txt";
        static object lockObject = new object();

        static void Main(string[] args)
        {

            Thread t1 = new Thread(() => CreateMatrix());
            t1.Start();
          
            Console.ReadLine();
        }
        static void CreateMatrix()
        {
            int[,] matrix = new int[100, 100];
            Thread t2 = new Thread(() => CreateNumbers());
            t2.Start();
            t2.Join();
            
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    for (int k = 0; k < list.Count; k++)
                    {
                        matrix[i, j] = list[i];
                    }
                }
            }
            //for (int i = 0; i < 100; i++)
            //{
            //    Console.WriteLine("\n");
            //    for (int j = 0; j < 100; j++)
            //    {
            //        Console.Write(matrix[i,j]);
            //    }
            //}

            Thread t3 = new Thread(() => ReadOddNumbers(matrix));
            Thread t4 = new Thread(() => ReadFile());
            t3.Start();
            t3.Join();
            t4.Start();
        }
        static void CreateNumbers()
        {
            Random rnd = new Random();

            for (int i = 0; i < 10000; i++)
            {
                list.Add(rnd.Next(10, 100));
            }
        }

        static void ReadOddNumbers(int[,] matrix)
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    if (matrix[i,j]%2==1)
                    {
                        oddNumberList.Add(matrix[i, j]);
                    }
                }
            }
            int[] array = oddNumberList.ToArray();

           
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                StreamWriter sw = new StreamWriter(path, true);
                for (int i = 0; i < array.Length; i++)
                {
                    sw.WriteLine(array[i]);
                }
            sw.Close();
            
        }

        static void ReadFile()
        {
            
                StreamReader sr = new StreamReader(path);
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            sr.Close();
            
        }
    }
}
