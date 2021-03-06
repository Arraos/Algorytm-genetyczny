﻿// Program klasycznego algorytmy genetycznego v.1.0
// ostatnia aktualizacja 2018-08-25
// CopyLeft Feliks Kurp 2018

using System;

namespace Genetyczny_v_1_0

{
    struct Parametry        //wprowadzono zmiany. Dodano przestrzeń poszukiwań dla przedziałów x[3.000, 5.000]  y[2.000, 4.000]
    { //parametry symulacji
        public const int lp = 40;
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
        //liczba genów w pojedynczym chromosomie
        public const int pula = 60;
        //liczba osobników  w populacji (liczba parzysta)
        public const double pk = 0.75;
        //prawdopodobieństwo krzyżowania
        public const double pm = 0.01;
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
        Byte[,] populacja;
        //dwuwymiarowa tablica aktualnych chromosomów populacji,
        //rozmieszczonych w kolejnych wierszach tablicy

        public Populacja()
        //konstruktor populacji - przydziela pamięć dla bitowej
        //reprezentacji populacji chromosomów
        { populacja = new Byte[Parametry.pula, Parametry.N]; }

        double[] tablicaFenotypow = new double[Parametry.pula];
        //tablica wartości fenotypów dla populacji chromosomów

        int power = LiczbyBazowe.Power();

        double[] tablicaDostosowanie = new double[Parametry.pula];
        //tablica wartości funkcji dostosowania dla populacji chromosomw

        public void LosujPopulacje()
        {   //metoda losuje nową populację chromosomów
            //i umieszcza je w lokalnej tablicy 'populacja'
            for (int pozycja = 0; pozycja < Parametry.pula; pozycja++)
            {
                Random Generator = new Random(LiczbyBazowe.PobierzBazowa());
                for (int j = 0; j < Parametry.N; j++)
                    populacja[pozycja, j] = (Byte)Generator.Next(2);
            }
        }

        int ObliczFenotypChromosomu(int pozycjaChromosomu)
        {   //metoda liczy reprezentację dziesiętną wskazanego chromosomu
            int fenotyp = 0, rat = 1;
            for (int j = 0; j < Parametry.N; j++)
            { fenotyp = fenotyp + populacja[pozycjaChromosomu, j] * rat; rat = rat * 2; }
            return fenotyp;
        }
        public void ObliczFenotypyX()  // wprowadzono zmiany. Dodano funkcję obliczajaca wartosc fenotypow dla populacji x oraz y
        {   //metoda liczy wartości fenotypów chromosomów populacji
            //w liniowej przestrzeni poszukiwań <a,b>
            //i umieszcza je w tablicy 'tablicaFenotypów'
            for (int pozycja = 0; pozycja < Parametry.pula; pozycja++)
                tablicaFenotypow[pozycja] = Parametry.a + (Parametry.b - Parametry.a)
                * ObliczFenotypChromosomu(pozycja) / power;
        }
        public void ObliczFenotypyY()  // wprowadzono zmiany. Dodano funkcję obliczajaca wartosc fenotypow dla populacji x oraz y
        {   //metoda liczy wartości fenotypów chromosomów populacji
            //w liniowej przestrzeni poszukiwań <a,b>
            //i umieszcza je w tablicy 'tablicaFenotypów'
            for (int pozycja = 0; pozycja < Parametry.pula; pozycja++)
                tablicaFenotypow[pozycja] = Parametry.c + (Parametry.d - Parametry.c)
                * ObliczFenotypChromosomu(pozycja) / power;
        }

        public void ObliczDostosowanie()                       //wprowadzona zmiana. Dodano druga tablice dla funkcji f(x,y)
        {   //metoda oblicza wartości funkcji dostosowania,
            //umieszcza je w tablicy 'tablicaDostosowanie'
            //a następnie normalizuje
            double x;
            double y;
            for (int i = 0; i < Parametry.pula; i++)
            {
                x = tablicaFenotypow[i];
                y = tablicaFenotypow[i];
                tablicaDostosowanie[i] = 1 - (Math.Sin(1 + x) + Math.Cos(y));
            }
   /*       for (int j =0;j < Parametry.pula; j++)
            {
               y = tablicaFenotypow[j];
               tablicaDostosowanie[j] = (Math.Exp(y) * Math.Sin(Math.PI * y) + 1) / y;
               1-(Math.Sin(1+x) + Math.Cos(y))
            } */
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

            Byte[,] nowePokolenie = new Byte[Parametry.pula, Parametry.N];
            //tablica pomocnicza chromosomów dla ruletki

            double[] tablicaNI = new double[Parametry.pula];
            //tablica pomocnicza ruletki

            DostosowanieNormalizacja();

            double sumaDostosowanie = 0;
            foreach (double dostosowanie in tablicaDostosowanie)
                sumaDostosowanie += dostosowanie;

            for (int i = 0; i < Parametry.pula; i++)
            {
                tablicaNI[i] = tablicaDostosowanie[i]
                               / sumaDostosowanie * power;
            }

            int[] losowe = new int[Parametry.pula];
            //tabela 'losowe' przechowuje liczby losowe z przedziału 0...Power()
            Random Generator = new Random(LiczbyBazowe.PobierzBazowa());
            for (int i = 0; i < Parametry.pula; i++)
                losowe[i] = Generator.Next(power);

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
                    nowePokolenie[i, k] = populacja[j, k];
            }
            populacja = nowePokolenie;
        }

        void Krzyzowanie()
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
                losoweMiejsca[i] = Generator.Next(Parametry.N - 2);

            //proces krzyżowania genów w parach
            int pierwszy = 0; //indeks pierwszego osobnika w każdej parze
            byte bufor;
            for (int para = 0; para < liczbaPar; para++)
            {
                if (losowePary[para] < Parametry.pk * 100)
                    for (int i = losoweMiejsca[para]; i < Parametry.N; i++)
                    {
                        bufor = populacja[pierwszy, i];
                        populacja[pierwszy, i] = populacja[pierwszy + 1, i];
                        populacja[pierwszy + 1, i] = bufor;
                    }
                pierwszy += 2;
            }
        }

        public void PokazFenotypyPopulacji()
        {   //wyświetla fenotypy aktualnej populacji
            foreach (double fenotyp in tablicaFenotypow)
                Console.Write("{0:#.##}\n ", fenotyp);
        }

        public void Mutacje()
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
                    miejsceMutacji = Generator.Next(Parametry.N);
                    if (populacja[i, miejsceMutacji] == 0)
                        populacja[i, miejsceMutacji] = 1;
                    else populacja[i, miejsceMutacji] = 0;
                }
        }

        public void PokazDostosowaniePopulacji()
        {   //wyświetla wartości funkcji dostosowania 
            //chromosomów aktualnej populacji
            foreach (double dostosowanie in tablicaDostosowanie)
                Console.Write("{0:#.##}\n ", dostosowanie);
        }

        void PokazChromosomyPopulacji()
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
                populacja.ObliczFenotypyX();  //wprowadzono zmiany. Użyto metodę obliczanie wartosci fenotypów dla populacji X oraz Y.
                populacja.ObliczFenotypyY();
                populacja.ObliczDostosowanie();
                Console.WriteLine("Nr pokolenia Srednia wartosc funkcji dostosowania");
                Console.Write("{0, 3}          ", nrPokolenia);
                populacja.PokazDostosowanieSrednie();
                while (nrPokolenia < Parametry.lp)
                {
                    nrPokolenia++;
                    populacja.Ruletka();
                    populacja.Krzyzowanie();
                    populacja.Mutacje();
                    populacja.ObliczFenotypyX();
                    populacja.ObliczFenotypyY();
                    populacja.ObliczDostosowanie();
                    Console.Write("{0, 3}          ", nrPokolenia);
                    populacja.PokazDostosowanieSrednie();
                }
                Console.ReadKey();
            }
        }
    }
}