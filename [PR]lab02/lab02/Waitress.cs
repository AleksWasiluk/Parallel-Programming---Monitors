using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lab02
{
    abstract class Waitress
    {
        protected Random r;
        protected BackgroundWorker thread = new BackgroundWorker();

        protected DrinkingManager drinkingManager;
        public void StartThread()
        {
            thread.RunWorkerAsync();
        }
        public Waitress(DrinkingManager _drinkingManager,int seed)
        {
            drinkingManager = _drinkingManager;
            r = new Random(seed);
            thread.DoWork += Thread_DoWork;
        }

        protected void InitThread()
        {
            thread.RunWorkerAsync();
        }
        protected abstract void Thread_DoWork(object sender, DoWorkEventArgs e);
       
    }
    class BottleWaitress:Waitress
    {
        public BottleWaitress(DrinkingManager _drinkingManager, int seed):base(_drinkingManager,seed)
        {

        }

        protected override void Thread_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {

                int timeSleep = r.Next(Const.minTime*10, Const.maxTime * 10);
                Thread.Sleep(timeSleep);

                ConsoleWriter.WriteMessage($"Bottle Waitress filled Bottle !", ConsoleColor.Yellow);
                drinkingManager.FillBottle();



            }
                

        }
    }

    class CucumberWaitress : Waitress
    {
        public CucumberWaitress(DrinkingManager _drinkingManager, int seed) : base(_drinkingManager, seed)
        {

        }

        protected override void Thread_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
            {
                int timeSleep = r.Next(Const.minTime * 10, Const.maxTime * 10);
                Thread.Sleep(timeSleep);

                ConsoleWriter.WriteMessage($"Cucumber Waitress filled Plate !", ConsoleColor.Blue);
                drinkingManager.FillPlates();
            }


        }
    }




}
