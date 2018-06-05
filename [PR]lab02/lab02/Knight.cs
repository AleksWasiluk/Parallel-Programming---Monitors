using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.ComponentModel;

namespace lab02
{
    class Knight
    {
        protected int indexList;
        static TellingManager tellingManager;
        static DrinkingManager drinkingManager;
        static KnightManager knightManager;
        public static void InitTellingManager(TellingManager tm)
        {
            tellingManager = tm;
        }
        public static void InitDrinkingManager(DrinkingManager dm)
        {
            drinkingManager = dm;
        }
        public static void InitKnightManager(KnightManager km)
        {
            knightManager = km;
        }
       
        //int indexRightKnight, indexLeftKnight;
        protected List<int> listeningKnights;
        Random r;
        
        public Knight(int seedRand,int _indexList)
        {


            indexList = _indexList;
            InitListeningKnights();
            

            r = new Random(seedRand);
            thread.DoWork += Thread_DoWork;

        }

        protected virtual void InitListeningKnights()
        {
            listeningKnights = new List<int>
            {
                Const.ShiftLeft(indexList),//sasiedzi
                Const.ShiftRight(indexList)
            };
        }

        private void Thread_DoWork(object sender, DoWorkEventArgs e)
        {
            knightManager.KnightIsAdded();
            for(int i=0;i< Const.threadIteration;i++)
            {
                Sleep();
                Drink();
                TellStory();
                
                
            }
            ConsoleWriter.WriteMessage($"Knight {indexList} is under the table!", ConsoleColor.DarkMagenta);
        }

        public void StartThread()
        {
            thread.RunWorkerAsync();
        }

        protected BackgroundWorker thread = new BackgroundWorker();


        private void Sleep()
        {
            ConsoleWriter.WriteMessage($"Knight {indexList} starts sleeping zZz", ConsoleColor.Gray);
            Thread.Sleep(r.Next(2*Const.minTime,2* Const.maxTime));
        }
        protected virtual void TellStory()
        {
            tellingManager.StartTellingStory(indexList, listeningKnights);
            //telling story
            Thread.Sleep(r.Next(Const.minTime, Const.maxTime));

            tellingManager.EndTellingStory(indexList, listeningKnights);
        }

        private void Drink()
        {
            drinkingManager.StartDrinking(indexList);
            

            drinkingManager.EndDrinking(indexList);

        }


    }

    class King : Knight
    {
        public King(int seedRand,int _indexList):base(seedRand, _indexList)
        {
            
        }
        protected override void InitListeningKnights()
        {
            listeningKnights = new List<int>();

            for(int i=0 ; i<Const.n ; i++)
            {
                if (i != indexList)
                    listeningKnights.Add(i);//wszyscy sluchaja krola oprocz niego
            }
           
        }
    }
}
