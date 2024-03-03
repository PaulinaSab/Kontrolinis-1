using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kontrolinis1
{
    class Miestas
    {
        string miestas { get; private set; }
        string salis { get; private set; }
        int gyventojai { get; private set; }
        double plotas { get; private set; }
        DateTime data { get; private set; }

        //Konstruktorius be parametrų
        public Miestas()
        {

        }
        //Užklotas tostring metodas
        public override string ToString()
        {
            return string.Format("| {0, -12} | {1, -12} | {2, 8} | {3, 7} | {4, 12} |", miestas, salis, gyventojai, plotas, data.ToString("d"));
        }
        //Konstruktorius su parametrais
        public Miestas(string miestas, string salis, int gyventojai, double plotas, DateTime data)
        {
            this.miestas = miestas;
            this.salis = salis;
            this.gyventojai = gyventojai;
            this.plotas = plotas;
            this.data = data;
        }

        //Užkloti palyginimo operatoriai

        public static bool operator >(Miestas pirmas, Miestas antras)
        {
            int p = string.Compare(pirmas.miestas, antras.miestas, StringComparison.CurrentCulture);
            return (pirmas.gyventojai > antras.gyventojai || (pirmas.gyventojai == antras.gyventojai && p < 0));
        }
        public static bool operator <(Miestas pirmas, Miestas antras)
        {
            int p = string.Compare(pirmas.miestas, antras.miestas, StringComparison.CurrentCulture);
            return (pirmas.gyventojai < antras.gyventojai || (pirmas.gyventojai == antras.gyventojai && p > 0));
        }
    }

    class DaugMiestų
    {
        public const int Cn = 100;
        public int n { get; private set; }
        private Miestas[] miestai;

        //Konstruktorius
        public DaugMiestų()
        {
            n = 0;
            miestai = new Miestas[Cn];
        }
        public Miestas Imti(int i)
        {
            return miestai[i];
        }
        public void Deti(Miestas miestas)
        {
            miestai[n++] = miestas;
        }
        public void DetiI(Miestas miestas, int indeksas)
        {
            if (indeksas <= n)
            {
                for (int i = n-1; i > indeksas; i++)
                {
                    miestai[i + 1] = miestai[i];    
                }
                miestai[indeksas] = miestas;
            }
        }
        public void PasalintiI(int indeksas)
        {
            if (indeksas < n)
            {
                for (int i = indeksas; i < n-1; i++)
                {
                    miestai[i] = miestai[i + 1];
                }
                miestai[n-1] = null;
                n--;
            }
        }
        // rikiavimas burbuliuko metodu
        public void Rikiuoti()
        {
            bool bk;
            bk = true;
            int i = 0;
            while(bk)
            {
                bk = false;
                for (int j = n-1; j > i; j--)
                {
                    if (miestai[j] > miestai[j-1])
                    {
                        bk = true;
                        Miestas laikinas = miestai[j];
                        miestai[j] = miestai[j-1];
                        miestai[j-1] = laikinas;
                    }
                }
                i++;
            }
        }

    }
    internal class Program
    {
        const string CFd = "..\\..\\Duomenys.txt";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            DaugMiestų M = new DaugMiestų();
            DaugMiestų V = new DaugMiestų();
            int metai;
            string salis; //miestas, kuris bus pašalintas
            int indeksas; //valstybės didžiausio miesto indeksas

            Skaityti(CFd, M);
            Spausdinti(M, "Pradiniai duomenys");

            Console.WriteLine("Nurodykite norimus metus: ");
            metai = int.Parse(Console.ReadLine());

            string antraste = String.Format("Atrinktų {0} metų miestai", metai);
            Atrinkti(M, V, metai);
            if (V.n > 0)
            {
                Spausdinti(V, antraste);
            }
            else
            {
                Console.WriteLine("Rinkinio sudaryti nepavyko");
            }

            Console.WriteLine("Įveskite šalį, kuros sunkiausią miestą norite rasti");
            salis = Console.ReadLine();
            

            indeksas = RastiSunkiausia(V, salis);

            Console.WriteLine("Didžiausias miestas {0}, jo plotas {1}", V.Imti(indeksas).miestas, V.Imti(indeksas).plotas);
            if (indeksas > 0)
            {
                V.PasalintiI(indeksas);
            }
            else
            {
                Console.WriteLine("Tokios šalies miestų nerasta");
            }

            if (V.n > 0)
            {
                antraste = String.Format("Miestų sąrašas, pašalinus {0} šalies didžiausio ploto miestą", salis);
                Spausdinti(V, antraste);
            }
            else
            {
                Console.WriteLine("Rinkinio sudaryti nepavyko");
            }

            V.Rikiuoti();

            if (V.n > 0)
            {
                antraste = String.Format("Surikiuotas miestų sąrašas, pašalinus {0} didžiausio ploto miestą", salis);

                Spausdinti(V, antraste);
            }
            else
            {
                Console.WriteLine("Rinkinio sudaryti nepavyko");
            }


        }

        static void Skaityti(string fr, DaugMiestų M)
        {
            using (StreamReader reader = new StreamReader(fr))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(';');
                    string pavadinimas = parts[0];
                    string salis = parts[1];
                    int gyventojai = int.Parse(parts[2]);
                    double plotas = double.Parse(parts[3]);
                    DateTime data = DateTime.Parse(parts[4]);
                    Miestas miestas = new Miestas(pavadinimas, salis, gyventojai, plotas, data);
                    M.Deti(miestas);
                }
            }
        }
        static void Spausdinti(DaugMiestų miestai, string antraste)
        {
            Console.WriteLine(antraste);
            Console.WriteLine("-------------------------------------------------------------------------");
            Console.WriteLine("| {5, 3} | {0, -12} | {1, -12} | {2, 8} | {3, 7} | {4, 12} |", "Pavadinimas", "Šalis", "Gyv. sk.", "Plotas", "Data", "Nr.");
            Console.WriteLine("-------------------------------------------------------------------------");

            for (int i = 0; i < miestai.n; i++)
            {
                Console.WriteLine("| {0}   {1}", i+1, miestai.Imti(i));
            }
            Console.WriteLine("-------------------------------------------------------------------------");
        }

        static void Atrinkti(DaugMiestų M, DaugMiestų V, int metai)
        {
            for (int i = 0; i < M.n; i++)
            {
                Miestas miest = M.Imti(i);

                if (miest.data.Year == metai)
                {
                    string pavadinimas = miest.miestas;
                    string salis = miest.salis;
                    int gyventojai = miest.gyventojai;
                    double plotas = miest.plotas;
                    DateTime data = miest.data;
                    Miestas miestas = new Miestas(pavadinimas, salis, gyventojai, plotas, data);
                    V.Deti(miestas);
                }
                
            }
        }
        static int RastiSunkiausia(DaugMiestų V, string salis)
        {
            double didziausiasPlotas = 0;
            int indeksas = -1;
            for (int i = 0; i < V.n; i++)
            {
                if (V.Imti(i).salis == salis)
                {
                    if (V.Imti(i).plotas > didziausiasPlotas)
                    {
                        indeksas = i;
                        didziausiasPlotas = V.Imti(i).plotas;
                    }
                }
            }
            return indeksas;
        }
    }
}
