using CodeExMachina;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lab02
{
    class DrinkingManager
    {
        object o = new object();
        Table table;
        Queue<ConditionVariable> QueuePulse;
        ConditionVariable[] conditionVariable;

      
        ConditionVariable waitForQueuePulse;
        ConditionVariable helperQueue;
        bool isManagerHelperDoing;
        bool[] isWaitingForWine;
        bool[] isWaitingForCucumber;
        bool[] isWaitingForPlate;
        bool[] isWaitingForCup;
        int n;

        public DrinkingManager(Table _table)
        {
            table = _table;
            n = Const.n;
            conditionVariable = new ConditionVariable[n];
            for (int i = 0; i < n; i++)
                conditionVariable[i] = new ConditionVariable();

            isWaitingForWine = new bool[n];
            isWaitingForCucumber = new bool[n];
            isWaitingForPlate = new bool[n];
            isWaitingForCup = new bool[n];

            QueuePulse = new Queue<ConditionVariable>();
            waitForQueuePulse = new ConditionVariable();
            isManagerHelperDoing = false;
            helperQueue = new ConditionVariable();
        }
        public void FillBottle()
        {
            lock(o)
            {
                while (isManagerHelperDoing)
                    waitForQueuePulse.Wait(o);

                table.FillBottleOfWine();

                for(int i=0;i<n;i++)
                {
                    if (isWaitingForWine[i])
                        QueuePulse.Enqueue(conditionVariable[i]);
                }

                if (QueuePulse.Count != 0)
                {
                    isManagerHelperDoing = true;
                    helperQueue.Pulse();
                }
            }

        }
        public void FillPlates()
        {
            lock (o)
            {
                while (isManagerHelperDoing)
                    waitForQueuePulse.Wait(o);

                table.FillPlates();
            }

            for (int i = 0; i < n; i++)
            {
                if (isWaitingForCucumber[i])
                    QueuePulse.Enqueue(conditionVariable[i]);
            }

            if (QueuePulse.Count != 0)
            {
                isManagerHelperDoing = true;
                helperQueue.Pulse();
            }


        }

        public void StartDrinking(int knightIndex)
        {
            lock(o)
            {
                while (isManagerHelperDoing)
                    waitForQueuePulse.Wait(o);

                Plate plate = table.GetPlate(knightIndex);
                Cup cup = table.GetCup(knightIndex);

                bool condition;
                do
                {
                    condition = cup.isUsed || plate.isUsed || (!plate.HasCucumber()) || table.IsBottleOfWineEmpty();
                    isWaitingForCucumber[knightIndex] = plate.HasCucumber();
                    isWaitingForCup[knightIndex] = plate.isUsed;
                    isWaitingForPlate[knightIndex] = plate.isUsed;
                    isWaitingForWine[knightIndex] = table.IsBottleOfWineEmpty();

                    if(condition)
                        conditionVariable[knightIndex].Wait(o);
                } while (condition);

                //-------------------------------------------------
                ConsoleWriter.WriteMessage($"Knight {knightIndex} starts drinking",ConsoleColor.DarkMagenta);

                cup.isUsed = true;
                cup.FillEmptyCup();

                plate.isUsed = true;
                plate.Eat();
                cup.Drink();
            }
        }

        public void EndDrinking(int knightIndex)
        {
            lock (o)
            {

                Plate plate = table.GetPlate(knightIndex);
                Cup cup = table.GetCup(knightIndex);

                cup.isUsed = false;
                int neighbourCup = table.GetNeighbourCup(knightIndex);
                if (isWaitingForCup[neighbourCup])
                {
                    QueuePulse.Enqueue(conditionVariable[neighbourCup]);
                }
                
                plate.isUsed = false;
                int neighbourPlate = table.GetNeighbourPlate(knightIndex);
                if (isWaitingForPlate[neighbourPlate])
                {
                    QueuePulse.Enqueue(conditionVariable[neighbourPlate]);
                }

                ConsoleWriter.WriteMessage($"Knight {knightIndex} ends drinking",ConsoleColor.DarkMagenta);

                if(QueuePulse.Count !=0)
                {
                    isManagerHelperDoing = true;
                    helperQueue.Pulse();
                }
            }
        }

        public void DequeuePulse()
        {
            lock (o)
            {
                while (!isManagerHelperDoing)
                    helperQueue.Wait(o);

                if (QueuePulse.Count != 0)
                    QueuePulse.Dequeue().Pulse();
                else
                {
                    isManagerHelperDoing = false;
                    waitForQueuePulse.PulseAll();
                }

            }
        }
    }
    class DrinkingManagerHelper
    {
        BackgroundWorker thread;
        DrinkingManager tellingManager;

        public DrinkingManagerHelper(DrinkingManager _drinkingManager)
        {
            thread = new BackgroundWorker();
            tellingManager = _drinkingManager;
            thread.DoWork += Thread_DoWork;
        }

        private void Thread_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                tellingManager.DequeuePulse();
            }
        }
        public void StartThread()
        {
            thread.RunWorkerAsync();
        }
    }
}
