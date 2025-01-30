using System.Diagnostics;

namespace Parallel;

class Program
{
    static void Main()
    {
        int[] arraySizes = { 100000, 1000000, 10000000 };

        foreach (var size in arraySizes)
        {
            Console.WriteLine($"-------------Вычисления для массива из {size} элементов-------------");
            int[] array = Enumerable.Range(1, size).ToArray();

            // Обычное
            Stopwatch sw = Stopwatch.StartNew();
            long sumNormal = SumNormal(array);
            sw.Stop();
            Console.WriteLine($"Обычное: Сумма = {sumNormal}, Время = {sw.ElapsedMilliseconds}мс");

            // Параллельное (для реализации использовать Thread, например List)
            sw.Restart();
            long sumParallelThreads = SumParallelThreads(array);
            sw.Stop();
            Console.WriteLine($"Параллельное (Threads): Сумма = {sumParallelThreads}, Время = {sw.ElapsedMilliseconds}мс");

            // Параллельное с помощью LINQ
            sw.Restart();
            long sumParallelLINQ = SumParallelLINQ(array);
            sw.Stop();
            Console.WriteLine($"Параллельное (LINQ): Сумма = {sumParallelLINQ}, Время = {sw.ElapsedMilliseconds}мс");
        }
    }

    static long SumNormal(int[] array)
    {
        long sum = 0;
        foreach (var value in array)
            sum += value;
        return sum;
    }

    static long SumParallelThreads(int[] array)
    {
        int numThreads = Environment.ProcessorCount;
        long[] chunklSums = new long[numThreads];
        Thread[] threads = new Thread[numThreads];
        int chunkSize = array.Length / numThreads;
        for (int i = 0; i < numThreads; i++)
        {
            int start = i * chunkSize;
            int end = ((i+1) == numThreads) ? array.Length : (i + 1) * chunkSize;
            int threadNum = i;
            threads[threadNum] = new Thread(() => {
                long sum = 0;
                for (int j = start; j < end; j++)
                    sum += array[j];
                chunklSums[threadNum] = sum;
            });
            threads[i].Start();
        }
        foreach (var thread in threads)
        {
            thread.Join();
        }
        return chunklSums.Sum();
    }

    static long SumParallelLINQ(int[] array)
    {
        return array.AsParallel().Sum(x => (long)x);
    }
}