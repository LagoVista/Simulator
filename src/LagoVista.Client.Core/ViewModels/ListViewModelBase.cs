using LagoVista.Client.Core.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Client.Core.ViewModels
{
    public class ListViewModelBase<TModel, TSummaryModel> : IoTAppViewModelBase where TModel : new() where TSummaryModel : class
    {
        IFormRestClient<TModel, TSummaryModel> _formRestClient;

        public ListViewModelBase()
        {
            _formRestClient = new ListRestClient<TModel, TSummaryModel>(RestClient);

        }


        public IFormRestClient<TModel, TSummaryModel> FormRestClient { get { return _formRestClient; } }

        TModel _model;
        public TModel Model
        {
            get { return _model; }
            set { Set(ref _model, value); }
        }


    }
}
