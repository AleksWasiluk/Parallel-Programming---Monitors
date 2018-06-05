using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab02
{
    class Table
    {
        int n;

        int numberOfPlates;
        int numberOfCupes;


        BottleOfWine bottleOfWine;
        List<Cup> cupes = new List<Cup>();
        List<Plate> plates = new List<Plate>();

        public int GetNeighbourCup(int indexKnight)
        {
            int result = indexKnight % 2 == 0 ? Const.ShiftLeft(indexKnight) : Const.ShiftRight(indexKnight);
            return result;
        }
        public int GetNeighbourPlate(int indexKnight)
        {
            int result = indexKnight % 2 == 0 ? Const.ShiftRight(indexKnight) : Const.ShiftLeft(indexKnight);
            return result;
        }
        public int GetPlateIndex(int indexKnight)
        {
            return indexKnight / 2;
        }
        public int GetCupIndex(int indexKnight)
        {
            return Const.ShiftLeft(indexKnight) / 2;
        }

        public Cup GetCup(int indexKnight)
        {
            int cupIndex = GetPlateIndex(indexKnight);
            return cupes[cupIndex];
        }
        public  Plate GetPlate(int indexKnight)
        {
            int plateIndex = GetCupIndex(indexKnight);
            return plates[plateIndex];
        }


        public Table()//n parzyste
        {
            n = Const.n;
            numberOfCupes = numberOfPlates = n / 2;

            InitializeTable();
        }
        public bool IsBottleOfWineEmpty()
        {
            return bottleOfWine.IsEmpty();
        }
            
        private void InitializeTable()
        {
            InitializeWines();
            InitializeCucumbers();

        }

        private void InitializeWines()
        {
            bottleOfWine = new BottleOfWine();

            Cup.SetWineBottle(bottleOfWine);

            for (int i = 0; i < numberOfCupes; i++)
                cupes.Add(new Cup());


        }

        private void InitializeCucumbers()
        {
            for (int i = 0; i < numberOfPlates; i++)
                plates.Add(new Plate());
        }
        public void FillBottleOfWine()
        {
            lock(bottleOfWine)
            {
                bottleOfWine.Fill();
            }
        }
        public void FillPlates()
        {
            for(int i=0;i<numberOfPlates;i++)
            {
                Plate plate = plates[i];
                lock(plate)
                {
                    plate.Fill();
                }
            }
        }

    }
    class Plate
    {
        int c; //liczba ogorkow max
        int counter;
        public bool isUsed;

        bool isFull = true;
        public Plate()
        {
            isUsed = false;
            c = Const.c;
            counter = c;
        }
        public bool HasCucumber()
        {
            return counter>0;
        }

        public void Fill()
        {
            counter = c;
        }

        public void Eat()
        {
            counter--;
        }

        //bool isFull = true;
    }
    class Cup
    {

        public bool isUsed;
        public static void SetWineBottle(BottleOfWine bw)
        {
            wineBottle = bw;
        }
        public static BottleOfWine wineBottle;

        bool isFull = false;
        public Cup()
        {
            isUsed = false;
        }
        public void Drink()
        {
            isFull = false;
        }
        public void FillEmptyCup()
        {
            wineBottle.Ladle();
            isFull = true;
        }


    }
    class BottleOfWine
    {
        public bool IsEmpty()
        {
            return counter == 0;
        }

        int w;
        int counter;
        public BottleOfWine()
        {
            w = Const.w;
            counter = w;
        }
        public void Ladle()
        {
            counter--;
        }
        public void Fill()
        {
            counter = w;
        }
    }
}
