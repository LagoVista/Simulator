using LagoVista.Client.Core.Net;
using LagoVista.Core.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.ViewModels
{
    public abstract class ListViewModelBase<TSummaryModel> : AppViewModelBase where  TSummaryModel : class
    {
        ListRestClient<TSummaryModel> _formRestClient;

        public ListViewModelBase()
        {
            _formRestClient = new ListRestClient<TSummaryModel>(RestClient);
        }

        public ListRestClient<TSummaryModel> FormRestClient { get { return _formRestClient; } }

        IEnumerable<TSummaryModel> _listItems;
        public IEnumerable<TSummaryModel> ListItems
        {
            get { return _listItems; }
            set { Set(ref _listItems, value); }
        }

        protected async Task<InvokeResult> LoadItems()
        {
            ListItems = null;
            var listResponse = await FormRestClient.GetForOrgAsync(GetListURI(), null);
            if (listResponse.Successful)
            {
                ListItems = listResponse.Result.Model;
            }

            return listResponse.ToInvokeResult();
        }

        protected abstract string GetListURI();


        public override Task InitAsync()
        {
            return PerformNetworkOperation(LoadItems);
        }

        public override Task ReloadedAsync()
        {
            return PerformNetworkOperation(LoadItems);
        }

        protected virtual void ItemSelected(TSummaryModel model) { }

        /* so far will always be null just used to detect clicking on object */
        TSummaryModel _selectedSimulator;
        public TSummaryModel SelectedItem
        {
            get { return _selectedSimulator; }
            set
            {
                if (value != null && _selectedSimulator != value)
                {
                    ItemSelected(value);
                }

                _selectedSimulator = value;

                RaisePropertyChanged();
            }
        }

    }
}