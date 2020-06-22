using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Zadatak_1
{
    class Program
    {
        //array that will be filled with random numbers
        static int[] array = new int[10000];
        //odd numbers will be added to the lsit
        static List<int> oddNumberList = new List<int>();
        static string path = @"../../OddNumbers.txt";
        //object for lock
        static object lockObject = new object();
        //initializaing matrix
        static int[,] matrix;
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            Thread t1 = new Thread(() => CreateMatrix());
            Thread t2 = new Thread(() => CreateNumbers());
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();

            Thread t3 = new Thread(() => ReadOddNumbers());
            Thread t4 = new Thread(() => ReadFile());
            t4.Start();
            t3.Start();
                
            Console.ReadLine();
        }
        /// <summary>
        /// Method that initializes matrix, than waits for other threads to generate numbers, and than fills matrix with those numbers
        /// </summary>
        static void CreateMatrix()
        {
            lock (lockObject)
            {
                //initializing
                matrix = new int[100, 100];
                //waits until other method generates numbers
                Monitor.Pulse(lockObject);
                Monitor.Wait(lockObject);
                //filling matrix with numbers from array (numbers where generated and put into array)
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
        /// <summary>
        /// Method creates 10k random numbers and puts them into array
        /// </summary>
        static void CreateNumbers()
        {
            
            //locked with lockObject
            lock (lockObject)
            {
                //in case that this thread starts before thread 1 (before matrix is initialized) => wait until matrix is initializes (48-50)
                if (matrix==null)
                {
                    Monitor.Wait(lockObject);
                }
                for (int i = 0; i < 10000; i++)
                {
                    //elements of array are random numbers
                    array[i] = rnd.Next(10, 100);
                }
                //notifies that job is done
                Monitor.Pulse(lockObject);
            }
        }
        /// <summary>
        /// Method reads odd numbers from matrix
        /// </summary>
        static void ReadOddNumbers()
        {
            //going through matrix and looking for odd numbers
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    //odd condition
                    if (matrix[i, j] % 2 == 1)
                    {
                        //add numbers to list
                        oddNumberList.Add(matrix[i, j]);
                    }
                }
            }
            lock (lockObject)
            {
                //LIST TO ARRAY
                int[] array = oddNumberList.ToArray();
                //check if file exists
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                //create stream writer for file input
                StreamWriter sw = new StreamWriter(path, true);
                for (int i = 0; i < array.Length; i++)
                {
                    //writing to file
                    sw.WriteLine(array[i]);
                }
                sw.Close();
                //notifies that job is done
                Thread.Sleep(500);
                Monitor.Pulse(lockObject);
            }
        }
        /// <summary>
        /// Method reads from file and displays odd numbers
        /// </summary>
        static void ReadFile()
        {
            lock (lockObject)
            {
                //waits for notification that resource is free and there is posibility to acces
                Monitor.Wait(lockObject);
                //creating stream reader
                StreamReader sr = new StreamReader(path);
                string line = "";
                //reading from file
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
                sr.Close();
            }

        }
    }
}
