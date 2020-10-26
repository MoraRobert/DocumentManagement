using System;

namespace CordiaManagamentTools.Api.Contracts.Model
{
    public class CashFlow_Detail
    {
        public string CFR_osszeg { get; set; }
        public string CFR_Penznem { get; set; }
        public string CFR_Projekt_kod_nev { get; set; }
        public string CFR_Cost_center { get; set; }
        public string CFR_CashFlow_kod_nev { get; set; }
        public string CFR_CF_reszletezo { get; set; }
        public string CFR_Funkcionalis_terulet { get; set; }
        public DateTime? CFR_Varhato_kifizetesi_datum { get; set; }
        public bool? CFR_Potmunka { get; set; }
    }
}
