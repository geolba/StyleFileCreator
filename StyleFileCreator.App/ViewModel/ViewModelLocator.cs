/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:MvvmLightWpf.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using CommonServiceLocator;
using StyleFileCreator.App.Model;
using StyleFileCreator.App.Utils;
using ESRI.ArcGIS.Framework;

using Unity;
using Unity.ServiceLocation;
using Unity.Lifetime;

namespace StyleFileCreator.App.ViewModel
{
    public class ViewModelLocator
    {
        private static IUnityContainer Container { get; set; }

        #region static constructor

        static ViewModelLocator()
        {
            // Register the IOC container as SimpleIoc
            //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Register Unity as the IOC container
            Container = new UnityContainer();           
            IServiceLocator locator = new UnityServiceLocator(Container);
            ServiceLocator.SetLocatorProvider(() => locator);
            
            if (ViewModelBase.IsInDesignModeStatic)
            {
                //SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
                Container.RegisterType<IDataService, Design.DesignDataService>();
            }
            else
            {
                //SimpleIoc.Default.Register<IDataService, DataService>();
                Container.RegisterType<IDataService, DataService>();
            }

            //SimpleIoc.Default.Register<DialogService>();
            //SimpleIoc.Default.Register<MainViewModel>();          
            //SimpleIoc.Default.Register<SettingsViewModel>();
            //SimpleIoc.Default.Register<WelcomeViewModel>();
            //SimpleIoc.Default.Register<FormularData>();
            //SimpleIoc.Default.Register<GeometryTypeViewModel>();
            //SimpleIoc.Default.Register<DataViewModel>();
            //SimpleIoc.Default.Register<ExportViewModel>();
            //SimpleIoc.Default.Register<AboutViewModel>();

            //DIALOGSERVICE
            Container.RegisterType<DialogService>(new ContainerControlledLifetimeManager());
            //viewmodels
            Container.RegisterType<MainViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<SettingsViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<WelcomeViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<FormularData>(new ContainerControlledLifetimeManager());
            Container.RegisterType<GeometryTypeViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<DataViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ExportViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<AboutViewModel>(new ContainerControlledLifetimeManager());            
        }

        #endregion

        public static IApplication Application
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                MainViewModel vm = ServiceLocator.Current.GetInstance<MainViewModel>();
                return vm;
            }
        }       

        public SettingsViewModel Settings
        {
            get
            {               
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        public WelcomeViewModel Welcome
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WelcomeViewModel>();
            }
        }

        public GeometryTypeViewModel GeometryType
        {
            get
            {
                return ServiceLocator.Current.GetInstance<GeometryTypeViewModel>();
            }
        }

        public DataViewModel Data
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DataViewModel>();
            }
        }

        public ExportViewModel Export
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ExportViewModel>();
            }
        }

        public AboutViewModel About
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AboutViewModel>();
            }
        }
              

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Reset()
        {
            // TODO Clear the ViewModels  
            if (Container.IsRegistered<SettingsViewModel>())
            {
                var settingsViewModel = ServiceLocator.Current.GetInstance<SettingsViewModel>();
                settingsViewModel.Reset();
            }
            if (Container.IsRegistered<GeometryTypeViewModel>())
            {
                var geometryTypeViewModel = ServiceLocator.Current.GetInstance<GeometryTypeViewModel>();
                geometryTypeViewModel.Reset();
            }
            if (Container.IsRegistered<ExportViewModel>())
            {
                var exportViewModel = ServiceLocator.Current.GetInstance<ExportViewModel>();
                exportViewModel.Reset();
            }
            if (Container.IsRegistered<DataViewModel>())
            {
                var dataViewModel = ServiceLocator.Current.GetInstance<DataViewModel>();
                dataViewModel.Reset();
            }
            if (Container.IsRegistered<MainViewModel>())
            {
                var mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();
                mainViewModel.Reset();
            }
        }
              
        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {


            // TODO Clear the ViewModels  
            if (Container.IsRegistered<SettingsViewModel>())
            {
                //var settingViewModel = SimpleIoc.Default.GetInstance<SettingsViewModel>();
                var settingViewModel = Container.Resolve<SettingsViewModel>();
                settingViewModel.Cleanup();
            }
            if (Container.IsRegistered<GeometryTypeViewModel>())
            {
                //var geometryTypeViewModel = SimpleIoc.Default.GetInstance<GeometryTypeViewModel>();
                var geometryTypeViewModel = Container.Resolve<GeometryTypeViewModel>();
                geometryTypeViewModel.Cleanup();
            }
            if (Container.IsRegistered<DataViewModel>())
            {
                //var dataViewModel = SimpleIoc.Default.GetInstance<DataViewModel>();
                var dataViewModel = Container.Resolve<DataViewModel>();
                dataViewModel.Cleanup();
            }
            if (Container.IsRegistered<ExportViewModel>())
            {
                //var exportViewModel = SimpleIoc.Default.GetInstance<ExportViewModel>();
                var exportViewModel = Container.Resolve<ExportViewModel>();
                exportViewModel.Cleanup();
            }
            if (Container.IsRegistered<MainViewModel>())
            {
                //var mainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
                var mainViewModel = Container.Resolve<MainViewModel>();
                mainViewModel.Cleanup();
                //SimpleIoc.Default.Unregister<MainViewModel>();
            }

            //clear dataservice
            if (Container.IsRegistered<IDataService>())
            {
                //var dataService = SimpleIoc.Default.GetInstance<IDataService>();
                var dataService = Container.Resolve<IDataService>();
                dataService.Dispose();
            }


        }
    }
}
