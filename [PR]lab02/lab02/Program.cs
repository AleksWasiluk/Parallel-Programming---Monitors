using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lab02
{
    public class ConsoleWriter
    {
        private static object _MessageLock = new object();

        public static void WriteMessage(string message, ConsoleColor c = ConsoleColor.White)
        {
            lock (_MessageLock)
            {
                Console.ForegroundColor = c;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }

    public static class Const
    {
        public const int minTime = 300;
        public const int maxTime = 1000;
        public const int n = 10;
        public const int w = 6;
        public const int c = 8;
        public const int threadIteration = 10;


        public static int ShiftRight(int i)
        {
            return i == n - 1 ? 0 : i + 1;
        }
        public static int ShiftLeft(int i)
        {
            return i == 0 ? n -1 : i - 1;
        }
    }
    class Program
    {
        //Table table;
       // List<Knight> knights;
        static void Main(string[] args)
        {
            Simulate();

        }

        private static void Simulate()
        {
            Random r = new Random();
            int n = Const.n;
            Table table = new Table();

            List<Knight> knights = new List<Knight>();

            knights.Add(new King(r.Next(), 0));
            for(int i=1;i<n;i++)
            {
                knights.Add(new Knight(r.Next(), i));
            }

            

            TellingManager tellingManager = new TellingManager(knights);
            TellingManagerHelper tellingManagerHelper = new TellingManagerHelper(tellingManager);
            
            DrinkingManager drinkingManager = new DrinkingManager(table);
            DrinkingManagerHelper drinkingManagerHelper = new DrinkingManagerHelper(drinkingManager);

            BottleWaitress bottleWaitress = new BottleWaitress(drinkingManager, r.Next());
            CucumberWaitress cucumberWaitress = new CucumberWaitress(drinkingManager, r.Next());

            Knight.InitTellingManager(tellingManager);
            Knight.InitDrinkingManager(drinkingManager);
            Knight.InitKnightManager(new KnightManager());

            tellingManagerHelper.StartThread();
            drinkingManagerHelper.StartThread();

            bottleWaitress.StartThread();
            cucumberWaitress.StartThread();
            for (int i=0;i<n;i++)
            {
                knights[i].StartThread();
            }

            Thread.Sleep(1000 * 60 * 10);

        }
    }
}
