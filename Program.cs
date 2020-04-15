namespace NFA2DFA
{
    public class Durum{
        public int isim;
        public Dictionary<char, HashSet<int>> baglantilar;


        public Durum(int n, List<char> sigma)
        {
            this.isim = n;
            this.baglantilar = new Dictionary<char, HashSet<int>>();
            foreach(char c in sigma) this.baglantilar.Add(c, new HashSet<int>());
        }
    }

    public class DurumSeti
    {
        public char isim;
        public HashSet<int> durumlar;    
        public bool proc;

   
        public DurumSeti()
        {
            this.isim = '\0';
            this.durumlar = new HashSet<int>();
            this.proc = false;
        }
        
        public void girdi(DurumSeti kaynak)
        {
            foreach(int i in kaynak.durumlar) this.durumlar.Add(i);
        }

 
        public void girdi(Durum kaynak)
        {
            this.durumlar.Add(kaynak.isim);
        }
    }

    public class DFA
    {
        public DurumSeti q0;                                            
        public List<DurumSeti> F;                                       
        public int toplam;                                              
        public List<char> sigma;                                        
        public Dictionary<DurumSeti, Dictionary<char, DurumSeti>> qDelta; 

        public DFA(List<char> L, DurumSeti q)
        {
            sigma = new List<char>();
            F = new List<DurumSeti>();
            toplam = 0;
            foreach (char c in L)
            {
                if (c != 'E') this.sigma.Add(c);
            }
            q.isim = 'A';
            q0 = q;
            qDelta = new Dictionary<DurumSeti,Dictionary<char,DurumSeti>>();
            qDelta.Add(q, new Dictionary<char,DurumSeti>());
            toplam++;
        }

        public void DFAyazdir()
        {
           
            List<DurumSeti> anahtarListesi = new List<DurumSeti>(qDelta.Keys);
            
            Console.Out.Write("Başlangıç durumu :\t{0}\n", q0.isim);
            Console.Out.Write("Bitiş durumu :\t{"); foreach(DurumSeti s in F)Console.Out.Write(s.isim); Console.Out.Write("}\n");
            Console.Out.Write("Toplam durum :\t{0}\n", toplam);
            Console.Out.Write("Durum\t" + String.Join("\t\t", sigma) + "\n");
            foreach (DurumSeti q in anahtarListesi)
            {
                Console.Out.Write(q.isim.ToString());

                bool boslukGirdi = false;
                for (int c = 0; c < sigma.Count; c++)
                {
                    if (boslukGirdi) Console.Out.Write("\t\t{");
                    else Console.Out.Write("\t{");
                    boslukGirdi = true;
                    if (qDelta[q].ContainsKey(sigma[c])) Console.Out.Write(qDelta[q][sigma[c]].isim.ToString());
                    Console.Out.Write("}");
                }
                Console.Out.Write("\t: [");
                bool girdi = false;
                foreach (int i in q.durumlar)
                {
                    if (girdi) Console.Out.Write("," + i);
                    else Console.Out.Write(i);
                    girdi = true;
                }
                Console.Out.Write("]");
                Console.Out.Write("\n");
            }
        }
    }

    class NFA
    {
        public int q0;              // Başlangıç durumu
        public List<int> F;         // Bitiş durumu
        public int toplamDurum;      
        public List<char> L;        
        public List<Durum> qDelta;  

        public NFA(List<String> girdi)
        {
            foreach (String item in girdi)
            {
                #region Nfa başlangıç durumu tanımlama
                if (Regex.IsMatch(item, "^Başlangıç durumu:\\s*[\\d]*\\s*$"))
                {
                    foreach (Match match in Regex.Matches(item, "^Başlangıç durumu:\\s*([\\d]*)\\s*$"))
                    {
                        q0 = Convert.ToInt32(match.Groups[1].Value);
                    }
                }
                #endregion
                #region Nfa bitiş durumu tanımlama
                else if (Regex.IsMatch(item, "^Bitiş durumu:\\s*\\{[\\d,]*\\}\\s*$"))
                {
                    foreach (Match match in Regex.Matches(item, "^Bitiş durumu:\\s*\\{([\\d,]*)\\}\\s*$"))
                    {
                        F = match.Groups[1].Value.Split(',').Select(int.Parse).ToList();
                    }

                }
                #endregion
                #region NFA toplam tanımlama
                else if (Regex.IsMatch(item, "^Total States:\\s*[\\d]*\\s*$"))
                {
                    foreach (Match match in Regex.Matches(item, "^Total States:\\s*([\\d]*)\\s*$"))
                    {
                        toplamDurum = Convert.ToInt32(match.Groups[1].Value);
                    }
                    qDelta = new List<Durum>(toplamDurum);
                }
                #endregion
                #region 
                else if (Regex.IsMatch(item, "^Durum[.]*"))
                {
                    String geciciString;
                    L = new List<char>();
                    geciciString = item.Substring(5);
                    for (int i = 0; i < geciciString.Length; i++)
                    {
                        if (geciciString[i] >= 'A' && geciciString[i] <= 'z')
                        {
                            L.Add(geciciString[i]);
                        }
                    }
                   
                }
                #endregion
                #region NFA 
                else if (Regex.IsMatch(item, "^[\\d]*[.]*"))
                {

                    int durumId = -1;
                    int iChar = 0;
                    Durum dugum = null;

                    foreach (String parca in item.Split(' '))
                    {
                        
                       
                        if (Regex.IsMatch(parca, "^[\\d]+$"))
                        {
                            durumId = Convert.ToInt32(parca);
                            dugum = new Durum(durumId, L);
                            if (durumId > toplamDurum)
                            {
                                Console.Error.Write("Durum adedi toplamdan fazla olamaz.");
                            }
                        }
                        else if (Regex.IsMatch(parca, "^\\{[,\\d]*\\}$"))
                        {
                            if (dugum == null) Console.Error.Write("Hatalı index değeri.\n");
                            else if (iChar > L.Count) Console.Error.Write("Hata : beklenenden daha fazla dil değeri.\n");
                            else
                            {
                                String gecici = parca.Trim(new char[] { '{', '}' });
                                if (gecici != "")
                                {
                                    foreach (int i in gecici.Split(new char[] { ',' }).Select(int.Parse).ToArray())
                                    {
                                        //  Add transition item
                                        if (dugum != null)
                                        {
                                            dugum.baglantilar[L[iChar]].Add(i);
                                        }
                                    }
                                }
                                iChar++;
                            }
                        }
                        else
                        {
                           
                        }
                    }
                    qDelta.Add(dugum);
                }
                #endregion
                #region 
                else
                {
                    Console.Error.Write("Hata : dosya formatı \n");
                }
                #endregion
            }
        }

       
        public void NFAtoDFA()
        {
            char geciciKarakter = 'A';
            DFA dfa = null;
            int oncekiToplam;
            DurumSeti geciciQ;
            
            foreach(Durum q in qDelta) if (q0 == q.isim) dfa = new DFA(L, Kapatma(q));


         
            dfa.q0.proc = true;
            foreach(char c in dfa.sigma)
            {
                geciciQ = new DurumSeti();
            
                foreach(int i in dfa.q0.durumlar)
                {
                    foreach (Durum q in qDelta)
                    {
                        if(q.isim == i)geciciQ.girdi(Tasi(q,c));
                    }
                }
              
                HashSet<int> geciciHash = new HashSet<int>(geciciQ.durumlar); ;
                foreach(int i in geciciHash)
                {
                    foreach (Durum q in qDelta)
                    {
                        if(q.isim == i) geciciQ.girdi(Kapatma(q));
                    }
                }
                if(geciciQ.durumlar.Count > 0 && !geciciQ.durumlar.SetEquals(dfa.q0.durumlar))
                {
                    geciciQ.isim = ++geciciKarakter;
                    dfa.qDelta[dfa.q0].Add(c,geciciQ);
                    dfa.qDelta.Add(geciciQ, new Dictionary<char, DurumSeti>());
                    dfa.toplam++;
                }
               
            }
            
            do
            {
                oncekiToplam = dfa.toplam;
                geciciKarakter = dahaFazlaDurumBul(dfa, geciciKarakter);
            } while (oncekiToplam != dfa.toplam);
           
            sonDurum(dfa);

           
            Console.Clear();
            Console.Out.Write("\n\n--------------------NFA--------------------\n");
            printNFA();
            Console.Out.Write("\n\n--------------------DFA--------------------\n");
            dfa.DFAyazdir();
        }

  
        public char dahaFazlaDurumBul(DFA dfa, char geciciKarakter)
        {
            List<DurumSeti> anahtarListesi = new List<DurumSeti>(dfa.qDelta.Keys);



            foreach(DurumSeti q in anahtarListesi)
            {
                if (!q.proc)
                {
                    q.proc = true;
                    DurumSeti geciciQ;
                    foreach (char c in dfa.sigma)
                    {
                        geciciQ = new DurumSeti();
                      
                        foreach (int i in q.durumlar)
                        {
                            foreach (Durum r in qDelta)
                            {
                                if (r.isim == i) geciciQ.girdi(Tasi(r, c));
                            }
                        }
                       
                        HashSet<int> geciciHash = new HashSet<int>(geciciQ.durumlar);
                        foreach (int i in geciciHash)
                        {
                            foreach (Durum r in qDelta)
                            {
                                if (r.isim == i) geciciQ.girdi(Kapatma(r));
                            }
                        }
                        if (geciciQ.durumlar.Count > 0 && !geciciQ.durumlar.SetEquals(q.durumlar))
                        {
                            geciciQ.isim = ++geciciKarakter;
                            dfa.qDelta[q].Add(c, geciciQ);
                            dfa.qDelta.Add(geciciQ, new Dictionary<char, DurumSeti>());
                            dfa.toplam++;
                        }
                    }
                   
                    return geciciKarakter;
                }
            }
            return geciciKarakter;
        }

        public void sonDurum(DFA dfa)
        {
            List<DurumSeti> anahtarListesi = new List<DurumSeti>(dfa.qDelta.Keys);

            foreach (DurumSeti q in anahtarListesi)
            {
                foreach(int i in F)
                {
                    if (q.durumlar.Contains(i)) dfa.F.Add(q);
                }
            }
        }

   
        public DurumSeti Kapatma(Durum q)
        {
            DurumSeti eKapali = new DurumSeti();

            eKapali.girdi(q);

       
            foreach (int num in q.baglantilar['E'])
            {
                foreach (Durum qTmp in qDelta)
                {
                   
                    if (qTmp.isim == num && !eKapali.durumlar.Contains(qTmp.isim))
                    {
                        eKapali.girdi(qTmp); 
                        eKapali.girdi(Kapatma(qTmp));
                    }
                }
            }
            return eKapali;
        }

   
        public DurumSeti Tasi(Durum q, char c)
        {
            DurumSeti delta = new DurumSeti();

        
            foreach (int num in q.baglantilar[c])
            {
                foreach (Durum qGecici in qDelta)
                {
                    if (qGecici.isim == num)
                    {
                        delta.girdi(qGecici);
                    }
                }
            }
            return delta;
        }


        public void printNFA()
        {
            Console.Out.Write("Başlangıç durumu:\t{0}\n", q0);
            Console.Out.Write("Bitiş durumu:\t{"+String.Join(",",F)+"}\n");
            Console.Out.Write("Toplam durumlar:\t{0}\n", toplamDurum);
            Console.Out.Write("Durum\t" + String.Join("\t\t", L) + "\n");
            foreach (Durum q in qDelta)
            {
                Console.Out.Write(q.isim.ToString());
                bool twoSpace = false;
                for (int c = 0; c < L.Count; c++)
                {
                    if (twoSpace) Console.Out.Write("\t\t{");
                    else Console.Out.Write("\t{");
                    twoSpace = true;
                    bool comma = false;
                    foreach (int it in q.baglantilar[L[c]])
                    {
                        if (comma) Console.Out.Write(",");
                        Console.Out.Write(it.ToString());
                        comma = true;
                    }
                    Console.Out.Write("}");
                }
                Console.Out.Write("\n");
            }
        }

    }

    class Program
    {

        static void Main(string[] args)
        {        
            NFA nfa;

            if (!Console.IsInputRedirected)
            {
                Console.Error.Write("'inputNFA.txt' formatında giriş yapınız.");
                return;
            }

            List<String> girdi = new List<String>();
            string satir;

            while ((satir = Console.ReadLine()) != null)
            {

                girdi.Add(satir);
            }
            nfa = new NFA(girdi);

            nfa.NFAtoDFA();

          
            Console.ReadKey();

        }

    }

}
