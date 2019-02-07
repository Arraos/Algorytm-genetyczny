// Program klasycznego algorytmy genetycznego v.1.0
// ostatnia aktualizacja 2018-08-25
// CopyLeft Feliks Kurp 2018

using System;

namespace Genetyczny_v_1_0

{
    struct Parametry
    { //parametry symulacji
        public const int lp = 400;
        //liczba pokoleń w eksperymencie
        public const double a = 3.0;
        //wartość początkowa przestrzeni poszukiwań dla x
        public const double b = 5.0;
        //wartość końcowa przestrzeni poszukiwań dla x
        public const double c = 2.0;
        //wartość początkowa przestrzeni poszukiwań dla y
        public const double d = 4.0;
        //wartość końcowa przestrzeni poszukiwań dla y
        public const int N = 11;
        //łączna liczba genów w pojedynczym chromosomie
        public const int Nx = 11;
        //liczba genów w pojedynczym chromosomie
        public const int Ny = 11;
        //liczba genów w pojedynczym chromosomie
        public const int pula = 2000;
        //liczba osobników  w populacji (liczba parzysta)
        public const double pk = 0.95;
        //prawdopodobieństwo krzyżowania
        public const double pm = 0.02;
        //prawdopodobieństwo mutacji
    }

    class LiczbyBazowe
    {
        static int[] tablicaBazowa = new int[100 * Parametry.lp];
        public static int indexBazowy = 0;
        //tablica i indeks bazowych liczb losowych dla generatora Random

        public void LosujBaze()
        {
            Random Generator = new Random();
            for (int i = 0; i < 100 * Parametry.lp; i++)
                tablicaBazowa[i] = Generator.Next(256);
        }
        public static int PobierzBazowa()
        {
            return tablicaBazowa[indexBazowy++];
        }
        public static int Power()
        {   // liczy n-tą nieujemną potegę dwójki
            int power = 1;
            for (int i = 1; i <= Parametry.N; i++)
                power = power * 2;
            return power;
        }
    }

    class Populacja
    {
        public Byte[,] populacjaX;
        public Byte[,] populacjaY;

        //dwuwymiarowa tablica aktualnych chromosomów populacji,
        //rozmieszczonych w kolejnych wierszach tablicy

        public Populacja()                                     //<-----------------------------------Wprowadzić kolejną populację dla y
        //konstruktor populacji - przydziela pamięć dla bitowej
        //reprezentacji populacji chromosomów
        {
            populacjaX = new Byte[Parametry.pula, Parametry.Nx];
            populacjaY = new Byte[Parametry.pula, Parametry.Ny];
        }

        double[] tablicaFenotypowX = new double[Parametry.pula];
        //tablica wartości fenotypów dla populacji chromosomów X
        double[] tablicaFenotypowY = new double[Parametry.pula];
        //tablica wartości fenotypów dla populacji chromosomów X

        int powerX = LiczbyBazowe.Power();
        int powerY = LiczbyBazowe.Power();

        double[] tablicaDostosowanie = new double[Parametry.pula];
        double[] tablicaDostosowanieX = new double[Parametry.pula];
        double[] tablicaDostosowanieY = new double[Parametry.pula];
        //tablica wartości funkcji dostosowania dla populacji chromosomw

        public void LosujPopulacje()
        {   //metoda losuje nową populację chromosomów
            //i umieszcza je w lokalnej tablicy 'populacja'
            for (int pozycja = 0; pozycja < Parametry.pula; pozycja++)
            {
                Random Generator = new Random(LiczbyBazowe.PobierzBazowa());
                for (int j = 0; j < Parametry.N; j++)
                    populacjaX[pozycja, j] = (Byte)Generator.Next(2);

            }

            for (int pozycja = 0; pozycja < Parametry.pula; pozycja++)
            {
                Random Generator = new Random(LiczbyBazowe.PobierzBazowa());
                for (int i = 0; i < Parametry.N; i++)
                    populacjaY[pozycja, i] = (Byte)Generator.Next(2);
            }
        }

        int ObliczFenotypChromosomuX(int pozycjaChromosomu)
        {   //metoda liczy reprezentację dziesiętną wskazanego chromosomus
            int fenotyp = 0, rat = 1;
            for (int j = 0; j < Parametry.N; j++)
            { fenotyp = fenotyp + populacjaX[pozycjaChromosomu, j] * rat; rat = rat * 2; }
            return fenotyp;
        }
        int ObliczFenotypChromosomuY(int pozycjaChromosomu)
        {   //metoda liczy reprezentację dziesiętną wskazanego chromosomus
            int fenotyp = 0, rat = 1;
            for (int j = Parametry.Nx; j < Parametry.N; j++)
            { fenotyp = fenotyp + populacjaY[pozycjaChromosomu, j] * rat; rat = rat * 2; }
            return fenotyp;
        }
        public void ObliczFenotypyX()                  // <---------------------------------------------------Do przerobienia--------------------------------------
        {   //metoda liczy wartości fenotypów chromosomów populacji
            //w liniowej przestrzeni poszukiwań <a,b>
            //i umieszcza je w tablicy 'tablicaFenotypów'
            //power = Parametry.Nx;
            for (int pozycja = 0; pozycja < Parametry.pula; pozycja++)
                tablicaFenotypowX[pozycja] = Parametry.a + (Parametry.b - Parametry.a)    // <------------------------Dodać zmienne ?
                * ObliczFenotypChromosomuX(pozycja) / powerX;
                //powerX = Parametry.Nx;
        }
        public void ObliczFenotypyY()                  // <---------------------------------------------------Do przerobienia--------------------------------------
        {   //metoda liczy wartości fenotypów chromosomów populacji
            //w liniowej przestrzeni poszukiwań <a,b>
            //i umieszcza je w tablicy 'tablicaFenotypów'
          //  power = Parametry.N - Parametry.Nx;
            for (int pozycja = 0; pozycja < Parametry.pula; pozycja++)
                tablicaFenotypowY[pozycja] = Parametry.c + (Parametry.d - Parametry.c)    // <------------------------Dodać zmienne ?
                * ObliczFenotypChromosomuY(pozycja) / powerY;
              //power = Parametry.N - Parametry.Nx;
        }

        public void ObliczDostosowanie()
        {   //metoda oblicza wartości funkcji dostosowania,
            //umieszcza je w tablicy 'tablicaDostosowanie'
            //a następnie normalizuje
            double x;
            double y;
            for (int i = 0; i < Parametry.pula; i++)
            {
                x = tablicaFenotypowX[i];
                y = tablicaFenotypowY[i];
                tablicaDostosowanie[i] = 1 - (Math.Sin(1 + x) + Math.Cos(y));
            } 
        }

        void DostosowanieNormalizacja()
        {   //normalizuje tablicę wartości funkcji dostosowania
            //tak, aby zawierała wyłącznie wartości dodatnie
            double min, max, offset;
            min = max = tablicaDostosowanie[0];
            foreach (double dostosowanie in tablicaDostosowanie)
            {
                if (dostosowanie < min) min = dostosowanie;
                if (dostosowanie > max) max = dostosowanie;
            }
            offset = (max - min) / (Parametry.N - 1) - min;
            for (int i = 0; i < Parametry.pula; i++)
                tablicaDostosowanie[i] += offset;
        }

        public void Ruletka()
        {   //selekcja chromosomów w populacji metodą koła ruletki

            Byte[,] nowePokolenieX = new Byte[Parametry.pula, Parametry.N];
            Byte[,] nowePokolenieY = new Byte[Parametry.pula, Parametry.N];
            //tablica pomocnicza chromosomów dla ruletki

            double[] tablicaNI = sumaDostosowanie();

            int[] losowe = new int[Parametry.pula];
            //tabela 'losowe' przechowuje liczby losowe z przedziału 0...Power()
            Random Generator = new Random(LiczbyBazowe.PobierzBazowa());
            for (int i = 0; i < Parametry.pula; i++)
                losowe[i] = Generator.Next(LiczbyBazowe.Power());

            double[] ruletka = new double[Parametry.pula];
            //tablica pozycji wycinków ruletki

            double pozycja = 0;
            for (int i = 0; i < Parametry.pula; i++)
            {
                pozycja += tablicaNI[i];
                ruletka[i] = pozycja;
            }

            for (int i = 0; i < Parametry.pula; i++)
            {
                int j = 0;
                while (losowe[i] > ruletka[j]) j++;
                for (int k = 0; k < Parametry.N; k++)
                {
                    nowePokolenieX[i, k] = populacjaX[j, k];
                }
                for (int m = 0; m < Parametry.N; m++)
                {
                    nowePokolenieY[i, m] = populacjaY[j, m];
                }
            }
            populacjaX = nowePokolenieX;
            populacjaY = nowePokolenieY;
        }

        double[] sumaDostosowanie()
        {
            //tablica pomocnicza ruletki
            double[] tablicaNI = new double[Parametry.pula];
            double sumaDostosowanie = 0;
            foreach (double dostosowanie in tablicaDostosowanie)
                sumaDostosowanie += dostosowanie;

            for (int i = 0; i < Parametry.pula; i++)
            {
                tablicaNI[i] = tablicaDostosowanie[i]
                               / sumaDostosowanie * LiczbyBazowe.Power();
            }

            return tablicaNI;
        }


        void Krzyzowanie(Byte[,] populacja, int parametryN)
        {
            Random Generator = new Random(LiczbyBazowe.PobierzBazowa());
            //tworzy generator liczb losowych oparty o kolejną
            //liczbę bazową

            //losowanie par osobników do krzyżowania
            int liczbaPar = Parametry.pula / 2;
            int[] losowePary = new int[liczbaPar];
            for (int i = 0; i < liczbaPar; i++)
                losowePary[i] = Generator.Next(100);

            //losowanie miejsc krzyżowania dla par
            int[] losoweMiejsca = new int[liczbaPar];
            for (int i = 0; i < liczbaPar; i++)
                losoweMiejsca[i] = Generator.Next(parametryN - 2);

            //proces krzyżowania genów w parach
            int pierwszy = 0; //indeks pierwszego osobnika w każdej parze
            byte bufor;
            for (int para = 0; para < liczbaPar; para++)
            {
                if (losowePary[para] < Parametry.pk * 100)
                    for (int i = losoweMiejsca[para]; i < parametryN; i++)
                    {
                        bufor = populacja[pierwszy, i];
                        populacja[pierwszy, i] = populacja[pierwszy + 1, i];
                        populacja[pierwszy + 1, i] = bufor;
                    }
                pierwszy += 2;
            }
        }

        public void PokazFenotypyPopulacjiX()
        {   //wyświetla fenotypy aktualnej populacji
            foreach (double fenotyp in tablicaFenotypowX)
                Console.Write("{0:#.##}\n Dla X -> ", fenotyp);
        }
        public void PokazFenotypyPopulacjiY()
        {   //wyświetla fenotypy aktualnej populacji
            foreach (double fenotyp in tablicaFenotypowY)
                Console.Write("{0:#.##}\n Dla Y -> ", fenotyp);
        }
        public void Mutacje(Byte[,] populacja, int parametryN)
        {  //metoda losuje chromosomy do mutacji 
           //i mutuje losowe geny w wylosowanych chromosomach

            Random Generator = new Random(LiczbyBazowe.PobierzBazowa());
            //tworzy generator liczb losowych oparty o kolejną
            //liczbę bazową

            double[] losowe = new double[Parametry.pula];
            for (int i = 0; i < Parametry.pula; i++)
                losowe[i] = Generator.Next(100) / 100.0;

            //proces krzyżowania genów w parach
            int miejsceMutacji;
            for (int i = 0; i < Parametry.pula; i++)
                if (losowe[i] < Parametry.pm)
                {
                    miejsceMutacji = Generator.Next(parametryN);
                    if (populacja[i, miejsceMutacji] == 0)
                        populacja[i, miejsceMutacji] = 1;
                    else populacja[i, miejsceMutacji] = 0;


                }
        }



        public void PokazDostosowaniePopulacji()
        {   //wyświetla wartości funkcji dostosowania 
            //chromosomów aktualnej populacji
            foreach (double dostosowanie in tablicaDostosowanie)
                Console.Write("{0:#.##}\n ","\t", dostosowanie);
        }

        void PokazChromosomyPopulacji(Byte[,] populacja)
        {
            for (int i = 0; i < populacja.GetLength(0); i++)
            {
                for (int j = 0; j < populacja.GetLength(1); j++)
                {
                    System.Console.Write("{0} ", populacja[i, j]);
                }
                Console.WriteLine();
            }
        }



        public double ObliczDostosowanieSrednie()
        {
            double srednia = 0;
            foreach (double dostosowanie in tablicaDostosowanie)
                srednia += dostosowanie;
            return srednia / Parametry.pula;
        }

        public void PokazDostosowanieSrednie()
        {   //wyświetla wartość średnią funkcji dostosowania 
            //wszystkich chromosomów aktualnej populacji 
            Console.WriteLine("{0:#.##}", ObliczDostosowanieSrednie());
        }


        class Program
        {
            static void Main(string[] args)
            {
                int nrPokolenia = 0;
                LiczbyBazowe liczbyBazowe = new LiczbyBazowe();
                liczbyBazowe.LosujBaze();
                Populacja populacja = new Populacja();
                populacja.LosujPopulacje();
                //wylosowanie populacji rodzicielskiej
                populacja.ObliczFenotypyX();
                populacja.ObliczFenotypyY();
                populacja.ObliczDostosowanie();
                Console.WriteLine("Nr pokolenia Srednia wartosc funkcji dostosowania");
                Console.Write("{0, 3}          ", nrPokolenia);
                populacja.PokazDostosowanieSrednie();
                while (nrPokolenia < Parametry.lp)
                {
                    nrPokolenia++;
                    populacja.Ruletka();
                    populacja.Krzyzowanie(populacja.populacjaX, Parametry.N);
                    populacja.Krzyzowanie(populacja.populacjaY, Parametry.N);
                    populacja.Mutacje(populacja.populacjaX, Parametry.N);
                    populacja.Mutacje(populacja.populacjaY, Parametry.N);
                    populacja.ObliczFenotypyX();
                    populacja.ObliczFenotypyY();
                    populacja.ObliczDostosowanie();
                    Console.Write("{0, 3}          ", nrPokolenia);
                    populacja.PokazDostosowanieSrednie();
                 //  populacja.PokazFenotypyPopulacjiX();
                  //  populacja.PokazFenotypyPopulacjiY();
                //    Console.WriteLine("\n\nPoniżej kolejne pokolenie\n");
                }
                Console.ReadKey();
            }
        }
    }
}