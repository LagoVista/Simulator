using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.PlatformManager.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace LagoVista.PlatformManager.Controls
{
    public class InstanceRuntimeTreeControl : StackLayout
    {
        public static readonly BindableProperty RuntimeDetailsProperty = BindableProperty.Create(
                                                    propertyName: nameof(RuntimeDetails),
                                                    returnType: typeof(InstanceRuntimeDetails),
                                                    declaringType: typeof(InstanceRuntimeTreeControl),
                                                    defaultValue: null,
                                                    defaultBindingMode: BindingMode.Default,
                                                    propertyChanged: HandleRuntimeDetailsAssigned);

        private static void HandleRuntimeDetailsAssigned(BindableObject bindable, object oldValue, object newValue)
        {
            var ctl = (InstanceRuntimeTreeControl)bindable;
            ctl.RuntimeDetails = newValue as InstanceRuntimeDetails;
        }

        public InstanceRuntimeDetails RuntimeDetails
        {
            get { return (InstanceRuntimeDetails)base.GetValue(RuntimeDetailsProperty); }
            set
            {
                base.SetValue(RuntimeDetailsProperty, value);
                Populate(value);
            }
        }


        public static readonly BindableProperty InstanceTapCommandProperty = BindableProperty.Create(
                                    propertyName: nameof(InstanceTapCommand),
                                    returnType: typeof(ICommand),
                                    declaringType: typeof(InstanceRuntimeTreeControl),
                                    defaultValue: null,
                                    defaultBindingMode: BindingMode.Default,
                                    propertyChanged: HandleInstanceTapCommandAssigned);

        private static void HandleInstanceTapCommandAssigned(BindableObject bindable, object oldValue, object newValue)
        {
            var ctl = (InstanceRuntimeTreeControl)bindable;
            ctl.InstanceTapCommand = newValue as ICommand;
        }

        public ICommand InstanceTapCommand
        {
            get { return (ICommand)base.GetValue(InstanceTapCommandProperty); }
            set { base.SetValue(InstanceTapCommandProperty, value); }
        }



        public static readonly BindableProperty PlannerTapCommandProperty = BindableProperty.Create(
                            propertyName: nameof(PlannerTapCommand),
                            returnType: typeof(ICommand),
                            declaringType: typeof(InstanceRuntimeTreeControl),
                            defaultValue: null,
                            defaultBindingMode: BindingMode.Default,
                            propertyChanged: HandlePlannerTapCommandAssigned);

        private static void HandlePlannerTapCommandAssigned(BindableObject bindable, object oldValue, object newValue)
        {
            var ctl = (InstanceRuntimeTreeControl)bindable;
            ctl.PlannerTapCommand = newValue as ICommand;
        }

        public ICommand PlannerTapCommand
        {
            get { return (ICommand)base.GetValue(PlannerTapCommandProperty); }
            set { base.SetValue(PlannerTapCommandProperty, value); }
        }



        public static readonly BindableProperty ListenerTapCommandProperty = BindableProperty.Create(
                    propertyName: nameof(ListenerTapCommand),
                    returnType: typeof(ICommand),
                    declaringType: typeof(InstanceRuntimeTreeControl),
                    defaultValue: null,
                    defaultBindingMode: BindingMode.Default,
                    propertyChanged: HandlePlannerTapCommandAssigned);

        private static void HandleListenerTapCommandAssigned(BindableObject bindable, object oldValue, object newValue)
        {
            var ctl = (InstanceRuntimeTreeControl)bindable;
            ctl.ListenerTapCommand = newValue as ICommand;
        }

        public ICommand ListenerTapCommand
        {
            get { return (ICommand)base.GetValue(ListenerTapCommandProperty); }
            set { base.SetValue(ListenerTapCommandProperty, value); }
        }



        public static readonly BindableProperty PipelineModuleTapCommandProperty = BindableProperty.Create(
            propertyName: nameof(PipelineModuleTapCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(InstanceRuntimeTreeControl),
            defaultValue: null,
            defaultBindingMode: BindingMode.Default,
            propertyChanged: HandlePipelineModuleTapCommandAssigned);

        private static void HandlePipelineModuleTapCommandAssigned(BindableObject bindable, object oldValue, object newValue)
        {
            var ctl = (InstanceRuntimeTreeControl)bindable;
            ctl.PipelineModuleTapCommand = newValue as ICommand;
        }

        public ICommand PipelineModuleTapCommand
        {
            get { return (ICommand)base.GetValue(PipelineModuleTapCommandProperty); }
            set { base.SetValue(PipelineModuleTapCommandProperty, value); }
        }



        public static readonly BindableProperty MessageTypeTapCommandProperty = BindableProperty.Create(
            propertyName: nameof(MessageTypeTapCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(InstanceRuntimeTreeControl),
            defaultValue: null,
            defaultBindingMode: BindingMode.Default,
            propertyChanged: HandleMessageTypeTapCommandAssigned);

        private static void HandleMessageTypeTapCommandAssigned(BindableObject bindable, object oldValue, object newValue)
        {
            var ctl = (InstanceRuntimeTreeControl)bindable;
            ctl.MessageTypeTapCommand = newValue as ICommand;
        }

        public ICommand MessageTypeTapCommand
        {
            get { return (ICommand)base.GetValue(MessageTypeTapCommandProperty); }
            set { base.SetValue(MessageTypeTapCommandProperty, value); }
        }



        private void AddSectionHeader(int leftMargin, string header)
        {
            var label = new Label();
            label.Margin = new Thickness(leftMargin, 5, 0, 0);
            label.Text = header;
            label.FontSize = 18;
            label.FontAttributes = FontAttributes.Bold;
            Children.Add(label);
        }

        private void AddItem(int leftMargin, string name, string status, string type, string id)
        {
            var grd = new Grid();
            grd.Margin = new Thickness(leftMargin, 0, 0, 0);
            grd.HeightRequest = 48;
            grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(220 - leftMargin, GridUnitType.Absolute) });
            grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            var label = new Label();
            label.SetValue(Grid.ColumnProperty, 0);
            label.Text = name;
            label.FontSize = 16;
            label.VerticalOptions = new LayoutOptions(LayoutAlignment.Center, false);
            grd.Children.Add(label);

            var statusLabel = new Label();
            statusLabel.SetValue(Grid.ColumnProperty, 1);
            statusLabel.Text = status;
            statusLabel.FontSize = 16;
            statusLabel.VerticalOptions = new LayoutOptions(LayoutAlignment.Center, false);
            grd.Children.Add(statusLabel);

            Children.Add(grd);

            if (!String.IsNullOrEmpty(status))
            {
                var tapRecognizer = new TapGestureRecognizer();
                tapRecognizer.Tapped += TapRecognizer_Tapped;
                grd.BindingContext = new TappedItem() { Id = id, Type = type };
                grd.GestureRecognizers.Add(tapRecognizer);
            }
        }

        private void TapRecognizer_Tapped(object sender, EventArgs e)
        {
            var tapGen = sender as Grid;
            var tappedItem = tapGen.BindingContext as TappedItem;
            switch(tappedItem.Type)
            {
                case INSTANCE_TYPE: InstanceTapCommand?.Execute(tappedItem.Id); break;
                case PLANNER_TYPE: PlannerTapCommand?.Execute(tappedItem.Id); break;
                case LISTENER_TYPE: ListenerTapCommand?.Execute(tappedItem.Id); break;
                case MESSAGETYPE_TYPE: MessageTypeTapCommand?.Execute(tappedItem.Id); break;
                case PIPELINEMODULE_TYPE: PipelineModuleTapCommand?.Execute(tappedItem.Id); break;
            }
        }

        const string INSTANCE_TYPE = "instance";
        const string PLANNER_TYPE = "planner";
        const string LISTENER_TYPE = "listener";
        const string MESSAGETYPE_TYPE = "messagetype";
        const string PIPELINEMODULE_TYPE = "pipelinemodule";

        private void Populate(InstanceRuntimeDetails runtimeDetails)
        {
            Children.Clear();

            AddItem(0, runtimeDetails.Name, runtimeDetails.Status.ToString(), INSTANCE_TYPE, runtimeDetails.Id);

            AddSectionHeader(0, PlatformManagerResources.InstanceDetails_Listeners);
            foreach (var listener in runtimeDetails.Listeners)
            {
                AddItem(10, listener.Name, listener.Status.ToString(), LISTENER_TYPE, listener.Id);
            }

            AddSectionHeader(0, PlatformManagerResources.InstanceDetails_Planner);
            AddItem(10, runtimeDetails.Planner.Name, runtimeDetails.Planner.Status.ToString(), PLANNER_TYPE, runtimeDetails.Planner.Id);

            AddSectionHeader(0, PlatformManagerResources.InstanceDetails_DeviceConfigurations);
            foreach (var deviceConfig in runtimeDetails.DeviceConfigurations)
            {
                AddItem(10, deviceConfig.Name, "", "", deviceConfig.Id);
                AddSectionHeader(10, PlatformManagerResources.InstanceDetails_Routes);
                foreach (var route in deviceConfig.Routes)
                {
                    AddItem(20, route.Name, "", "", route.Id);
                    AddSectionHeader(20, PlatformManagerResources.InstanceDetails_MessageTypes);
                    foreach (var msgType in route.MessageTypes)
                    {
                        AddItem(30, msgType.Name, "",MESSAGETYPE_TYPE, msgType.Id);
                    }

                    AddSectionHeader(20, PlatformManagerResources.InstanceDetails_PipelineModules);
                    foreach (var pipelienModule in route.PipelineModules)
                    {
                        AddItem(30, pipelienModule.Name, pipelienModule.Status.ToString(), PIPELINEMODULE_TYPE, pipelienModule.Id);
                    }
                }
            }
        }

        public class TappedItem
        {
            public String Id { get; set; }
            public String Type { get; set; }
        }
    }
}
