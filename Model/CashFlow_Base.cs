namespace CordiaManagamentTools.Api.Contracts.Model
{
    public class CashFlow_Base
    {
        public string CFA_osszeg { get; set; }
        public string CFA_szazalek { get; set; }
        public string CFA_aFA_tipus { get; set; }
        public string CFA_aFA_kulcs { get; set; }
        public string CFA_Penznem { get; set; }
        public string CFA_Projekt_kod_nev { get; set; }
        public string CFA_CashFlow_kod_nev { get; set; }
        public string CFA_Cost_Center { get; set; }
        public string CFA_CF_reszletezo { get; set; }
        public string CFA_Funkcionalis_terulet { get; set; }
        public string CFA_Periodicitas { get; set; }
        public bool? CFA_Eloleg { get; set; }
        public string CFA_Cikk_aFA_csoport { get; set; }
    }
}
