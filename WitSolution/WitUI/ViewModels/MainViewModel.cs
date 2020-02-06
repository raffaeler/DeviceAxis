using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using IAmRaf.MVVM;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WitCom;
using WitUI.Configurations;
using WitUI.Extensions;
using WitUI.Models;

namespace WitUI.ViewModels
{
    public class MainViewModel : CommonViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MainViewModel> _logger;
        private readonly GeneralConfig _generalConfig;
        private readonly Dispatcher _dispatcher;
        private Wit _wit;

        public MainViewModel(
            IServiceProvider serviceProvider,
            ILogger<MainViewModel> logger,
            IOptions<GeneralConfig> generalConfig)
        {
            this._serviceProvider = serviceProvider;
            this._logger = logger;
            this._generalConfig = generalConfig.Value;
            this._dispatcher = Dispatcher.CurrentDispatcher;

            this.Title = "WitUI, by Raf @raffaeler, 2020";

            StartStopCommand = new DelegateCommand(() => OnStartStop());
            RefreshPortsCommand = new DelegateCommand(() => OnRefreshPorts());
            OpenCloseCOMCommand = new DelegateCommand(() => OnOpenCloseCOM());

            OnRefreshPorts();
        }

        public ICommand StartStopCommand { get; }
        public ICommand RefreshPortsCommand { get; }
        public ICommand OpenCloseCOMCommand { get; }


        public async override Task ViewLoadedAsync(FrameworkElement frameworkElement)
        {
            if (_generalConfig.StartMaximized)
            {
                App.Current.MainWindow.WindowState = WindowState.Maximized;
            }

            await Task.Delay(0);
        }

        private bool _isRecording;
        public bool IsRecording
        {
            get => _isRecording;
            set { _isRecording = value; OnPropertyChanged(); }
        }

        private bool _isOpenCOM;
        public bool IsOpenCOM
        {
            get => _isOpenCOM;
            set { _isOpenCOM = value; OnPropertyChanged(); }
        }

        private string _text;
        public string Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(); }
        }

        private GraphData _graphData;
        public GraphData GraphData
        {
            get => _graphData;
            set { _graphData = value; OnPropertyChanged(); }
        }

        private int _totalSamples;
        public int TotalSamples
        {
            get => _totalSamples;
            set { _totalSamples = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> Ports { get; } = new ObservableCollection<string>();

        private string _selectedPort;
        public string SelectedPort
        {
            get => _selectedPort;
            set { _selectedPort = value; OnPropertyChanged(); }
        }

        private void OnRefreshPorts()
        {
            Ports.Clear();
            Ports.AddRange(Wit.GetPortNames());
            if (Ports.Contains(_generalConfig.COMPort))
                SelectedPort = _generalConfig.COMPort;
            else
                SelectedPort = Ports.FirstOrDefault();
        }

        private void OnOpenCloseCOM()
        {
            //...
            IsOpenCOM = !IsOpenCOM;
        }

        private void OnStartStop()
        {
            IsRecording = !IsRecording;
            if (IsRecording)
            {
                Text = "New session";
                var startTime = TimeSpan.Zero;
                GraphData = new GraphData(startTime);
            }
            else
            {
                Text = "Session ended";
            }
        }

    }
}
