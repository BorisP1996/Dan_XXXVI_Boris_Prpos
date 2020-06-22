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
        static int[] array = new int[10000];
        static List<int> oddNumberList = new List<int>();
        static string path = @"../../OddNumbers.txt";
        static object lockObject = new object();
        static object lockObject2 = new object();
        static int[,] matrix;
        static Random rnd = new Random();

        static void Main(string[] args)
        {

            Thread t1 = new Thread(() => CreateMatrix());
            Thread t2 = new Thread(() => CreateNumbers());
            t1.Start();
            t2.Start();

            Thread t3 = new Thread(() => ReadOddNumbers());
            Thread t4 = new Thread(() => ReadFile());
            t4.Start();
            t3.Start();
            //t3.Join();
            
          
            Console.ReadLine();
        }
        static void CreateMatrix()
        {
            lock (lockObject)
            {
                matrix = new int[100, 100];
                Monitor.Wait(lockObject);
                int index = 0;
                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        matrix[i, j] = array[index];
                        index++;
                    }
                }
            }
        }
        static void CreateNumbers()
        {
            lock (lockObject)
            {
                for (int i = 0; i < 10000; i++)
                {
                    array[i] = rnd.Next(10, 100);
                }
                Monitor.Pulse(lockObject);
            }
        }

        static void ReadOddNumbers()
        {
            
                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        if (matrix[i, j] % 2 == 1)
                        {
                            oddNumberList.Add(matrix[i, j]);
                        }
                    }
                }
            lock (lockObject)
            {
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

                Monitor.Pulse(lockObject);

                
            }

        }

        static void ReadFile()
        {
            lock (lockObject)
            {
                Monitor.Wait(lockObject);

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
}
