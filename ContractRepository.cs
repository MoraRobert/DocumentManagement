using CodeToCaml;
using CordiaManagamentTools.Api.Contracts.Model;
using CordiaManagamentTools.Api.Option;
using static CordiaManagamentTools.Api.Contracts.TextParser;
using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CordiaManagamentTools.Api.Contracts
{
    public interface IContractRepository
    {
        string GetContractJson(IEnumerable<KeyValuePair<int, string>> currencies);
        
        Task<Contract> GetContractAsync(string id, IEnumerable<KeyValuePair<int, string>> currencies);

        Task<IEnumerable<BaseContract>> SearchContractAsync(
            DateTime? from,
            DateTime? to,
            string customerName,
            string partnerName,
            string projectName,
            string documentType,
            string documentSubject,
            string searchText);
    }

    public class ContractRepository : IContractRepository
    {
        private Guid contractListId;

        private readonly SharePointOption _sharePointOption;        
        
        public ContractRepository(
            IOptions<SharePointOption> sharePointOptions)
        {
            _sharePointOption = sharePointOptions.Value;
            contractListId = new Guid(_sharePointOption.ContractListId);
        }
              
        public async Task<Contract> GetContractAsync(string id, IEnumerable<KeyValuePair<int, string>> currencies)
        {
            var clientContext = LoginToSharepoint();
            
            List contractList = clientContext.Web.Lists.GetById(contractListId);

            ListItem item = contractList.GetItemById(Convert.ToInt32(id));
            clientContext.Load(item);
            await clientContext.ExecuteQueryAsync();

            Contract contractmodel = new Contract(item, currencies);

            return contractmodel;
        }

        public string GetContractJson(IEnumerable<KeyValuePair<int, string>> currencies)
        {
            var clientContext = LoginToSharepoint();

            List contractList = clientContext.Web.Lists.GetById(contractListId);

            CamlQuery query = CamlQuery.CreateAllItemsQuery(10);
            ListItemCollection items = contractList.GetItems(query);
            clientContext.Load(items);
            clientContext.ExecuteQuery();

            #region samples
            

            List<Contract> contractlisttoserialize = new List<Contract>();            

            foreach (var contract in items)
            {
                contractlisttoserialize.Add(new Contract(contract, currencies));
            }

            #endregion
            
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Include,
            };
            string json = JsonConvert.SerializeObject(contractlisttoserialize[6], Formatting.Indented, settings);
            System.IO.File.WriteAllText("szerzjson.json", json);
            return json;
        }

        public async Task<IEnumerable<BaseContract>> SearchContractAsync(
            DateTime? from,
            DateTime? to,
            string customerName,
            string partnerName,
            string projectName,
            string documentType,
            string documentSubject,
            string searchText)
        {
            var clientContext = LoginToSharepoint();

            List contractList = clientContext.Web.Lists.GetById(contractListId);

            var caml = new Caml<BaseContract>();

            if (from != null)
            {
                caml = caml.AndAlso(bc => bc.Created >= from);
            }

            if (to != null)
            {
                caml = caml.AndAlso(bc => bc.Created <= to);
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                caml = caml.AndAlso(bc => bc.CustomerName == customerName);
            }

            if (!string.IsNullOrEmpty(partnerName))
            {
                caml = caml.AndAlso(bc => bc.PartnerName == partnerName);
            }

            if (!string.IsNullOrEmpty(projectName))
            {
                caml = caml.AndAlso(bc => bc.ProjectName == projectName);
            }

            if (!string.IsNullOrEmpty(documentType))
            {
                caml = caml.AndAlso(bc => bc.DocumentType == documentType);
            }           

            if (!string.IsNullOrEmpty(searchText))
            {
                Dictionary<string, Operator> parsedString = ParseString(searchText);

                foreach (KeyValuePair<string, Operator> kvp in parsedString)
                {
                    if (kvp.Value.Equals(Operator.And))
                    {
                        caml = caml.AndAlso(bc => (bc.CustomerName.Contains(kvp.Key) ||
                                                  bc.PartnerName.Contains(kvp.Key) ||
                                                  bc.ProjectName.Contains(kvp.Key) ||
                                                  bc.DocumentType.Contains(kvp.Key) ||
                                                  bc.DocumentSubject.Contains(kvp.Key)));
                    }
                    else if (kvp.Value.Equals(Operator.Or))
                    {
                        caml = caml.OrElse(bc => bc.CustomerName.Contains(kvp.Key) ||
                                                 bc.PartnerName.Contains(kvp.Key) ||
                                                 bc.ProjectName.Contains(kvp.Key) ||
                                                 bc.DocumentType.Contains(kvp.Key) ||
                                                 bc.DocumentSubject.Contains(kvp.Key));
                    }
                    else if (kvp.Value.Equals(Operator.Not))
                    {
                        caml = caml.AndAlso(bc => bc.CustomerName != kvp.Key &&
                                                  bc.PartnerName != kvp.Key &&
                                                  bc.ProjectName != kvp.Key &&
                                                  bc.DocumentType != kvp.Key &&
                                                  bc.DocumentSubject != kvp.Key);
                    }
                    else if (kvp.Value.Equals(Operator.Quotation))
                    {
                        caml = caml.AndAlso(bc => bc.CustomerName == kvp.Key ||
                                                  bc.PartnerName == kvp.Key ||
                                                  bc.ProjectName == kvp.Key ||
                                                  bc.DocumentType == kvp.Key ||
                                                  bc.DocumentSubject == kvp.Key);
                    }
                }
            }

            var camlQuery = new CamlQuery();
            camlQuery.ViewXml = caml.ToString();

            var items = contractList.GetItems(camlQuery);

            clientContext.Load(items);
            await clientContext.ExecuteQueryAsync();

            var contracts = new List<BaseContract>();

            foreach (var listItem in items)
            {
                var contract = new BaseContract(listItem, _sharePointOption);
                contracts.Add(contract);
            }

            return contracts;
        }

        private ClientContext LoginToSharepoint()
        {
            var authManager = new OfficeDevPnP.Core.AuthenticationManager();
            return authManager.GetNetworkCredentialAuthenticatedContext(
                _sharePointOption.SiteUrl, _sharePointOption.UserName,
                _sharePointOption.Password, _sharePointOption.Domain);
        }
    }
}
