using CordiaManagementTools.Orm.Mappers;
using CordiaManagementTools.Orm.Attributes;
using Microsoft.SharePoint.Client;
using System;
using CordiaManagamentTools.Api.Option;
using CodeToCaml.SpTypes;

namespace CordiaManagamentTools.Api.Contracts.Model
{

    public class BaseContract
    {
        public string DocumentName { get; set; }


        [SpData(Name = "Szerz_x0151_d_x00e9_ssz_x00e1_m")]
        [SharePointField("Szerz_x0151_d_x00e9_ssz_x00e1_m")]
        public string Title { get; set; }


        [SpData(Name = "Created")]
        [SharePointField("Created")]
        public DateTime Created { get; set; }


        [SpData(Name = "Modified")]
        [SharePointField("Modified")]
        public DateTime Modified { get; set; }


        [SpData(Name = "Statusz")]
        [SharePointField("Statusz")]
        public string Status { get; set; }


        [SpData(Name = "Megrendel_x0151__x0020_n_x00e9_v")]
        [SharePointField("Megrendel_x0151__x0020_n_x00e9_v")]
        public string CustomerName { get; set; }

        
        [SpData(Name = "Partner_x0020_n_x00e9_v_x0020__x")]
        [SharePointField("Partner_x0020_n_x00e9_v_x0020__x")]
        public string PartnerName { get; set; }

        
        [SpData(Name = "Projekt_x0020_n_x00e9_v_x0020__x")]
        [SharePointField("Projekt_x0020_n_x00e9_v_x0020__x", SharePointType.BusinessData)]
        public string ProjectName { get; set; }

        
        [SpData(Name = "Szerz_x0151_d_x00e9_s_x0020_t_x00")]
        //[SpData(Name = "Szerz_x0151_d_x00e9_s_x0020_T_x03")]
        [SharePointField("Szerz_x0151_d_x00e9_s_x0020_t_x00", SharePointType.BusinessData)]
        public string DocumentType { get; set; }

        
        [SpData(Name = "Szerz_x0151_d_x00e9_s_x0020_t_x0")]
        [SharePointField("Szerz_x0151_d_x00e9_s_x0020_t_x0")]
        public string DocumentSubject { get; set; }
        
        
        public string DocumentUrl { get; set; }


        public BaseContract(ListItem listItem, SharePointOption option)
        {
            EntityMapper.Map(listItem, this);

            DocumentName = "Szerz.";            

            var itemId = Convert.ToString(listItem["ID"]);
            DocumentUrl = $@"{option.SiteUrl}_layouts/15/NintexForms/Modern/DisplayForm.aspx?
                 List={option.ContractListId}&
                 ID={itemId}";
            
        }
    }
}
