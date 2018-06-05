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
    class TellingManager
    {
        int n;
        bool[] isListening;
        bool[] isTelling;
        int[] nListening;
        List<Knight> knights;
        Queue<ConditionVariable> QueuePulse;
        ConditionVariable[] conditionVariable;

        ConditionVariable waitForQueuePulse;
        ConditionVariable helperQueue;
        bool isManagerHelperDoing;
        object o = new object();
        public TellingManager(List<Knight> _knights)
        {
            knights = _knights;
            n = knights.Count;
            isListening = new bool[n];
            nListening = new int[n];
            isTelling = new bool[n];
            conditionVariable = new ConditionVariable[n];
            for (int i = 0; i < n; i++)
                conditionVariable[i] = new ConditionVariable();

            QueuePulse = new Queue<ConditionVariable>();
            waitForQueuePulse = new ConditionVariable();
            isManagerHelperDoing = false;
            helperQueue = new ConditionVariable();
        }

        public void StartTellingStory(int knightTelling, List<int> knightListening)
        {
            
            lock (o)
            {
                while (isManagerHelperDoing)
                    waitForQueuePulse.Wait(o);

                while (isListening[knightTelling] == true)
                    conditionVariable[knightTelling].Wait(o);
                //--------------------------------------------------------

                ConsoleWriter.WriteMessage($"Starting telling Story by Knight : {knightTelling} ",ConsoleColor.DarkGreen);

                isTelling[knightTelling] = true;

                for(int i=0;i< knightListening.Count;i++)
                {
                    int index = knightListening[i];
                    if (!isTelling[index])
                    {
                        if (!isListening[index])
                        {
                            isListening[index] = true;
                        }
                        nListening[index]++;
                    }
                    
                }
                
            }
            


        }

        public void EndTellingStory(int knightTelling, List<int> knightListening)
        {
            lock (o)
            {

                isTelling[knightTelling] = false;

                for (int i = 0; i < knightListening.Count; i++)
                {
                    int index = knightListening[i];
                    if (!isTelling[index])
                    {
                        if(nListening[index] != 0)
                            nListening[index]--;
                        if (nListening[index] == 0)
                        {
                            isListening[index] = false;
                            QueuePulse.Enqueue(conditionVariable[index]);
                        }
                    }
                    
                }
                ConsoleWriter.WriteMessage($"Ending telling Story by Knight : {knightTelling}", ConsoleColor.DarkRed);

                if(QueuePulse.Count != 0)
                {
                    isManagerHelperDoing = true;
                    helperQueue.Pulse();
                }
                
            }
        }

        public void DequeuePulse()
        {
            lock(o)
            {
                while (!isManagerHelperDoing)
                    helperQueue.Wait(o);

                if (QueuePulse.Count!=0)
                    QueuePulse.Dequeue().Pulse();
                else
                {
                    isManagerHelperDoing = false;
                    waitForQueuePulse.PulseAll();
                }

            }
        }

    }
    class TellingManagerHelper
    {
        BackgroundWorker thread;
        TellingManager tellingManager;

        public TellingManagerHelper(TellingManager _tellingManager)
        {
            thread = new BackgroundWorker();
            tellingManager = _tellingManager;
            thread.DoWork += Thread_DoWork;
        }

        private void Thread_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
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
