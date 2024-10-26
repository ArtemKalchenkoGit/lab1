using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace lab1 {
    internal class Program {
        static float[,] GenerateRandomMatrix(int rows, int cols)
        {
            float[,] matrix = new float[rows, cols];
            Random random = new Random();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = (float)(random.NextDouble() * 100); 
                }
            }

            return matrix;
        }

        static float[] GetMaxVectorByRows(float[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            float[] rowMaxima = new float[rows];

            for (int i = 0; i < rows; i++)
            {
                rowMaxima[i] = matrix[i, 0]; 
                for (int j = 1; j < cols; j++)
                {
                    if (matrix[i, j] > rowMaxima[i])
                    {
                        rowMaxima[i] = matrix[i, j];
                    }
                }
            }

            return rowMaxima;
        }

        static float[] GetParalleledMaxVectorByRows(float[,] matrix, int threadCount)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            float[] rowMaxima = new float[rows];
            Thread[] threads = new Thread[threadCount];

            if (threadCount>rows){
                threadCount = rows;
            }

            int rowsPerThread = rows / threadCount;
            int extraRows = rows % threadCount; 

            for (int t = 0; t < threadCount; t++)
            {
                int startRow = t * rowsPerThread + Math.Min(t, extraRows);
                int endRow = startRow + rowsPerThread + (t < extraRows ? 1 : 0);

                threads[t] = new Thread(() =>
                {
                    for (int i = startRow; i < endRow; i++)
                    {
                        rowMaxima[i] = matrix[i, 0]; 
                        for (int j = 1; j < cols; j++)
                        {
                            if (matrix[i, j] > rowMaxima[i])
                            {
                                rowMaxima[i] = matrix[i, j];
                            }
                        }
                    }
                });

                threads[t].Start();
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
            return rowMaxima;
        }

        static void Main(string[] args) {
            
            float[,] matrix = GenerateRandomMatrix(10000,10000);

            for (int t = 1; t<10; t++){
                Stopwatch stopwatchParallel = Stopwatch.StartNew();
                var maxVectorParallel = GetParalleledMaxVectorByRows(matrix,t);
                stopwatchParallel.Stop();

                Stopwatch stopwatch = Stopwatch.StartNew();
                var maxVector = GetMaxVectorByRows(matrix);
                stopwatch.Stop();

                float boost = (float)stopwatch.ElapsedTicks/(float)stopwatchParallel.ElapsedTicks;

                Console.WriteLine($"Прискорення: {boost} при {t} потоках\n");
            }
        }
    }
}