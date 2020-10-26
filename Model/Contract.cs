using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using CordiaManagementTools.Orm.Attributes;
using CordiaManagementTools.Orm.Mappers;
using System.Linq;


namespace CordiaManagamentTools.Api.Contracts.Model
{   
    public class Contract
    {        
        public Contract(ListItem item, IEnumerable<KeyValuePair<int, string>> currencies)
        {
            EntityMapper.Map(item, this);

            Keretszerzodes = ((string)item.FieldValues["Keretszerz_x0151_d_x00e9_s"] == "igen" ? true : false);            
            Megrendeloi_szolgaltatas = ((double?)item.FieldValues["Megrendel_x0151_i_x0020_szolg_x00"] == null ? "" : ((double?)item.FieldValues["Megrendel_x0151_i_x0020_szolg_x00"]).ToString());
            Szerzodott_netto_osszeg_osszesen = (double?)item.FieldValues["Szerz_x0151_d_x00f6_tt_x0020_net0"];
            Megrendeloi_szolgaltatas_szaz = (double?)item.FieldValues["Megrendel_x0151_i_x0020_szolg_x0"] * 100;
            Megrendeloi_szolgaltatas_osszege = (double?)item.FieldValues["Megrendel_x0151_i_x0020_szolg_x01"];

           

            string Szerzodes_RS_Xml = (string)item.FieldValues["Szerzodes_RS"];
            string Sconto_RS_Xml = (string)item.FieldValues["Sconto_RS"];
            string Visszatartasok_RS_Xml = (string)item.FieldValues["Visszatart_x00e1_sok_RS"];
            string CashflowAlap_RS_Xml = (string)item.FieldValues["CashflowAlap_RS"];
            string Cashflow_reszletes_bontas_RS_Xml = (string)item.FieldValues["Cashflow_x0020_r_x00e9_szletes_x"];

            XDocument doc;

            Szerzodes_RS = new List<Contract_Value>();
            if (Szerzodes_RS_Xml != null)
            {
                doc = XDocument.Parse(Szerzodes_RS_Xml);
                string szlaPnem = "";
                string szerPnem = "";
                foreach (XElement element in doc.Descendants("Item"))
                {
                    if ((string)element.Element("Számlázáspénzneme") != "" &&
                        (string)element.Element("Számlázáspénzneme") != null &&
                        (string)element.Element("Szerződéspénzneme") != "" &&
                        (string)element.Element("Szerződéspénzneme") != null) 
                    {
                        szlaPnem = currencies.ToList()[(int)element.Element("Számlázáspénzneme") - 1].Value;
                        szerPnem = currencies.ToList()[(int)element.Element("Szerződéspénzneme") - 1].Value;
                    }
                    
                    Szerzodes_RS.Add(new Contract_Value()
                    {                        
                        Szamlazas_penzneme = szlaPnem,
                        Szerzodes_penzneme = szerPnem,
                        Szerzodott_netto_osszeg = (string)element.Element("Szerződöttnettóösszeg")
                    });;
                }
            }

            Sconto_RS = new List<Sconto_Detail>();
            if (Sconto_RS_Xml != null)
            {
                doc = XDocument.Parse(Sconto_RS_Xml);
                foreach (XElement element in doc.Descendants("Item"))
                {
                    if ((string)element.Element("Sc_Scontóösszeg") != "" && (string)element.Element("Sc_Scontóösszeg") != "0")
                    {
                        var scSz = (double)element.Element("Sc_Scontószázalék") * 100;
                        Sconto_RS.Add(new Sconto_Detail()
                        {
                            Sc_Sconto_osszeg = (string)element.Element("Sc_Scontóösszeg"),
                            Sc_Sconto_szazalek = (string) scSz.ToString(),
                            Sc_Fizetesi_hatarido = (int)element.Element("Sc_Fizetésihatáridő")
                        });
                    }
                }
            }

            Visszatartasok_RS = new List<Retention_Detail>();
            if (Visszatartasok_RS_Xml != null)
            {
                doc = XDocument.Parse(Visszatartasok_RS_Xml);
                foreach (XElement element in doc.Descendants("Item"))
                {
                    if ((string)element.Element("V_Összeg") != "" && (string)element.Element("V_Összeg") != "0")
                    {
                        var vtSz = (double)element.Element("V_Százalék") * 100;
                        Visszatartasok_RS.Add(new Retention_Detail()
                        {
                            V_Jogcim = FormattingUtils.CorrectHChar((string)element.Element("V_Jogcím")),
                            V_osszeg = (string)element.Element("V_Összeg"),
                            V_Szazalek = (string) vtSz.ToString(),
                            V_Esedekessegi_datum = FormatDate((DateTime)element.Element("V_Esedékességidátum"))
                        });
                    }
                }
            }

            CashflowAlap_RS = new List<CashFlow_Base>();
            if (CashflowAlap_RS_Xml != null)
            {
                doc = XDocument.Parse(CashflowAlap_RS_Xml);
                foreach (XElement element in doc.Descendants("Item"))
                {
                    CashflowAlap_RS.Add(new CashFlow_Base()
                    {
                        CFA_aFA_kulcs = (string)element.Element("CFA__x00c1_FA_x0020_kulcs"),
                        CFA_aFA_tipus = (string)element.Element("CFA__x00c1_FA_x0020_t_x00ed_pus"),
                        CFA_CashFlow_kod_nev = (string)element.Element("CFA_CashFlow_x0020_k_x00f3_d_x00"),
                        CFA_CF_reszletezo = (string)element.Element("CFA_CF_x0020_r_x00e9_szletez_x01"),
                        CFA_Cikk_aFA_csoport = (string)element.Element("CFA_Cikk_x0020__x00c1_FA_x0020_c"),
                        CFA_Cost_Center = (string)element.Element("CFA_Cost_x0020_Center"),
                        CFA_Eloleg = (bool?)element.Element("CFA_El_x0151_leg"),
                        CFA_Funkcionalis_terulet = (string)element.Element("CFA_Funkcion_x00e1_lis_x0020_ter"),
                        CFA_osszeg = (string)element.Element("CFA__x00d6_sszeg"),
                        CFA_Penznem = (string)element.Element("CFA_P_x00e9_nznem"),
                        CFA_Periodicitas = (string)element.Element("CFA_Periodicit_x00e1_s"),
                        CFA_Projekt_kod_nev = (string)element.Element("CFA_Projekt_x0020_k_x00f3_d_x002"),
                        CFA_szazalek = (string)element.Element("CFA_szazalek")
                    });
                }
            }

            Cashflow_reszletes_bontas_RS = new List<CashFlow_Detail>();
            if (Cashflow_reszletes_bontas_RS_Xml != null)
            {
                doc = XDocument.Parse(Cashflow_reszletes_bontas_RS_Xml);
                foreach (XElement element in doc.Descendants("Item"))
                {
                    Cashflow_reszletes_bontas_RS.Add(new CashFlow_Detail()
                    {
                        CFR_CashFlow_kod_nev = (string)element.Element("CFR_CashFlowkódnév"),
                        CFR_CF_reszletezo = (string)element.Element("CFR_CFrészletező"),
                        CFR_Cost_center = (string)element.Element("CFR_Cost_x0020_Center"),
                        CFR_Funkcionalis_terulet = (string)element.Element("CFR_Funkcionálisterület"),
                        CFR_osszeg = (string)element.Element("CFR_Összeg"),
                        CFR_Penznem = (string)element.Element("CFR_Pénznem"),
                        CFR_Potmunka = (bool?)element.Element("CFR_Pótmunka"),
                        CFR_Projekt_kod_nev = (string)element.Element("CFR_Projektkódnév"),
                        CFR_Varhato_kifizetesi_datum = (DateTime?)element.Element("CFR_Várhatókifizetésidátum")
                    });
                }
            }
        }


        [SharePointField("Projekt_x0020_n_x00e9_v_x0020__x0")]
        public string Projekt_nev_es_kod_Code { get; set; }


        [SharePointField("Projekt_x0020_n_x00e9_v_x0020__x1")]
        public string Projekt_nev_es_kod_Name { get; set; }

                          
        [SharePointField("Fizet_x00e9_si_x0020_felt_x00e9_0", SharePointType.Lookup)]
        public string Fizetesi_feltetel_Code { get; set; }


        [SharePointField("Fizet_x00e9_si_x0020_felt_x00e9_", SharePointType.Lookup)]
        public string Fizetesi_feltetel_Name { get; set; }

        
        [SharePointField("_x00c1_FA_x0020_t_x00ed_pusa")]
        public string aFA_tipusa { get; set; }

        
        [SharePointField("Fizet_x00e9_si_x0020_felt_x00e9_2", SharePointType.Lookup)]
        public string Fizetesi_feltetel { get; set; }

        
        [SharePointField("F_x0151_k_x00f6_nyvi_x0020_sz_x0")]
        public string Fokonyvi_szam { get; set; }


        //[SharePointField("Megrendel_x0151_i_x0020_szolg_x0", SharePointType.Double)]
        public double? Megrendeloi_szolgaltatas_szaz { get; set; }

        
        //[SharePointField("Megrendel_x0151_i_x0020_szolg_x00", SharePointType.Int32)]
        public string Megrendeloi_szolgaltatas { get; set; }
        

        //[SharePointField("Megrendel_x0151_i_x0020_szolg_x01", SharePointType.Double)]
        public double? Megrendeloi_szolgaltatas_osszege { get; set; }
        

        [SharePointField("Megjegyz_x00e9_sek")]//, SharePointType.Comment)]
        public string Megjegyzesek { get; set; }

        
        [SharePointField("Szerz_x0151_d_x00e9_ssz_x00e1_m")]
        public string SzerzodesSzam { get; set; }
                       

        [SharePointField("Projekt_x0020_n_x00e9_v_x0020__x")]
        public string Projekt_nev_es_kod { get; set; }
                
                          
        [SharePointField("Statusz")]
        public string Statusz { get; set; }

        
        [SharePointField("Megrendel_x0151__x0020_n_x00e9_v")]
        public string Megrendelo_nev { get; set; }

        
        [SharePointField("Megrendel_x0151__x0020_kapcsolat", SharePointType.Lookup)]
        public string Megrendelo_kapcsolattartoja { get; set; }

        
        [SharePointField("Partner_x0020_n_x00e9_v_x0020__x")]
        public string Partner_nev_es_kod { get; set; }

        
        [SharePointField("Partner_x0020_kapcsolattart_x00f")]
        public string Partner_kapcsolattartoja { get; set; }

        
        [SharePointField("Szerz_x0151_d_x00e9_s_x0020_t_x0")]
        public string Szerzodes_targya { get; set; }

        
        //[SharePointField("Keretszerz_x0151_d_x00e9_s", SharePointType.Boolean)]
        public bool? Keretszerzodes { get; set; }
        

        [SharePointField("Szerz_x0151_d_x00e9_s_x0020_T_x03", SharePointType.Lookup)]
        public string Szerzodes_tipus { get; set; }
        
        
        [SharePointField("Kezd_x0151__x0020_d_x00e1_tum")]
        public DateTime? Kezdo_datum { get; set; }

        
        [SharePointField("Z_x00e1_r_x00f3__x0020_d_x00e1_t")]
        public DateTime? Zaro_datum { get; set; }

        
        [SharePointField("Szerz_x0151_d_x00e9_s_x0020_al_x")]
        public DateTime? Szerzodes_alairas_datuma { get; set; }

                          
        //[SharePointField("Szerz_x0151_d_x00f6_tt_x0020_net0", SharePointType.Double)]
        public double? Szerzodott_netto_osszeg_osszesen { get; set; }
        
        
        [SharePointField("Project_x0020_manager", SharePointType.Lookup)]
        public string Project_manager { get; set; }

        
        [SharePointField("Szerz_x0151_d_x00e9_s_x0020_jell")]
        public string Szerzodes_jellege { get; set; }

        
        [SharePointField("Partner_x0020_n_x00e9_v_x0020__x")]
        public string Partner_nev_es_kod_Code { get; set; }

        
        [SharePointField("Partner_x0020_n_x00e9_v_x0020__x")]
        public string Partner_nev_es_kod_Name { get; set; }



        public List<Sconto_Detail> Sconto_RS { get; set; }
        public List<CashFlow_Base> CashflowAlap_RS { get; set; }
        public List<CashFlow_Detail> Cashflow_reszletes_bontas_RS { get; set; }
        public List<Contract_Value> Szerzodes_RS { get; set; }
        public List<Retention_Detail> Visszatartasok_RS { get; set; }



        public string FormatDate(DateTime date)
        {
            return date.ToLocalTime().ToShortDateString();
        }
    }     
}
