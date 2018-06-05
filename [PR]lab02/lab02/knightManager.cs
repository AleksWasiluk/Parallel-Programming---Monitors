using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lab02
{
    class KnightManager
    {
        object o = new object();
        //int numberOfKnights = 0;
        int n;
        int numberOfKnights;
        bool wasPulseAll = false;

        public KnightManager()
        {
            n = Const.n;
            numberOfKnights = 0;
        }

        public void KnightIsAdded()
        {
            

          
            lock(o)
            {
                numberOfKnights++;
                while (numberOfKnights != n)
                    Monitor.Wait(o);

                if(!wasPulseAll)
                {
                    Monitor.PulseAll(o);
                    wasPulseAll = true;
                }
                   


            }
        }


    }
}
