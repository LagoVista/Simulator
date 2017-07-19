using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Commanding;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;

namespace LagoVista.PlatformManager.Core.ViewModels
{
    public class InstanceViewModel : AppViewModelBase
    {
        public InstanceViewModel()
        {
            InstanceTapCommand = new RelayCommand(InstanceTapped);
            ListenerTapCommand = new RelayCommand(ListenerTapped);
            PlannerTapCommand = new RelayCommand(PlannerTapped);
            MessageTypeTapCommand = new RelayCommand(MessageTypeTapped);
            PipelineModuleTapCommand = new RelayCommand(PipelineModuleTapped);
        }

        public void InstanceTapped(object id)
        {
            NavigateAndViewAsync<MonitorInstanceViewModel>(id.ToString());
        }

        public void ListenerTapped(object id)
        {
            NavigateAndViewAsync<ListenerViewModel>(id.ToString());
        }

        public void PlannerTapped(object id)
        {

        }

        public void MessageTypeTapped(object id)
        {

        }

        public void PipelineModuleTapped(object id)
        {
            NavigateAndViewAsync<PipelineViewModel>(id.ToString());
        }


        public async override Task InitAsync()
        {
            await PerformNetworkOperation(async () =>
            {
                var result = await RestClient.GetAsync<InvokeResult<InstanceRuntimeDetails>>($"/api/deployment/instance/{LaunchArgs.ChildId}/runtime");
                if(result.Successful)
                {
                    RuntimeDetails = result.Result.Result;
                }
                return result.ToInvokeResult();
            });
        }

        InstanceRuntimeDetails _runtimeDetails;
        public InstanceRuntimeDetails RuntimeDetails
        {
            get { return _runtimeDetails; }
            set { Set(ref _runtimeDetails, value); }
        }

        public RelayCommand InstanceTapCommand { get; private set; }
        public RelayCommand ListenerTapCommand { get; private set; }
        public RelayCommand PlannerTapCommand { get; private set; }
        public RelayCommand MessageTypeTapCommand { get; private set; }
        public RelayCommand PipelineModuleTapCommand { get; private set; }
    }
}
