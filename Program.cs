using System;
using System.Threading;

class Program
{
    static Mutex mutex = new Mutex();
    static Semaphore customers = new Semaphore(0, int.MaxValue);
    static Semaphore barbers = new Semaphore(0, int.MaxValue);
    static int chairs = 5; // Number of chairs in the waiting area

    static void Main(string[] args)
    {
        Thread barberThread = new Thread(Barber);
        barberThread.Start();

        for (int i = 1; i <= 10; i++) // Number of customers
        {
            Thread customerThread = new Thread(() => Customer(i));
            customerThread.Start();
            Thread.Sleep(1000); // Delay to create new customers
        }

        Console.ReadLine();
    }

    static void Barber()
    {
        while (true)
        {
            customers.WaitOne(); // Barber waits until a customer arrives
            mutex.WaitOne();
            chairs++;
            barbers.Release(); // Release the barber to serve the customer
            mutex.ReleaseMutex();
            Console.WriteLine("Barber is cutting hair");
            Thread.Sleep(5000); // Simulate haircut time
        }
    }

    static void Customer(int id)
    {
        mutex.WaitOne();
        if (chairs > 0)
        {
            chairs--;
            Console.WriteLine("Customer " + id + " takes a seat");
            customers.Release(); // Release the barber if the customer takes a seat
            mutex.ReleaseMutex();
            barbers.WaitOne(); // Wait until the barber is available for a haircut
            Console.WriteLine("Customer " + id + " is getting a haircut");
        }
        else
        {
            mutex.ReleaseMutex();
            Console.WriteLine("Customer " + id + " leaves because there are no empty chairs");
        }
    }
}
