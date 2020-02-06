using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private readonly DispatcherTimer _timer;
        private Wit _wit;
        private ConcurrentQueue<WitFrame> _queue = new ConcurrentQueue<WitFrame>();

        public MainViewModel(
            IServiceProvider serviceProvider,
            ILogger<MainViewModel> logger,
            IOptions<GeneralConfig> generalConfig)
        {
            this._serviceProvider = serviceProvider;
            this._logger = logger;
            this._generalConfig = generalConfig.Value;
            this._dispatcher = Dispatcher.CurrentDispatcher;
            _timer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(200), DispatcherPriority.Normal, OnTimer, _dispatcher);
            _timer.IsEnabled = false;

            this.Title = "WitUI, by Raf @raffaeler, 2020";

            StartStopCommand = new DelegateCommand(() => OnStartStop());
            RefreshPortsCommand = new DelegateCommand(() => OnRefreshPorts());
            OpenCloseCOMCommand = new DelegateCommand(() => OnOpenCloseCOM());
            ZeroCommand = new DelegateCommand(() => OnZero());

            OnRefreshPorts();
        }

        public ICommand StartStopCommand { get; }
        public ICommand RefreshPortsCommand { get; }
        public ICommand OpenCloseCOMCommand { get; }
        public ICommand ZeroCommand { get; }


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

        private GraphData _graphDataX;
        public GraphData GraphDataX
        {
            get => _graphDataX;
            set { _graphDataX = value; OnPropertyChanged(); }
        }

        private GraphData _graphDataY;
        public GraphData GraphDataY
        {
            get => _graphDataY;
            set { _graphDataY = value; OnPropertyChanged(); }
        }

        private GraphData _graphDataZ;
        public GraphData GraphDataZ
        {
            get => _graphDataZ;
            set { _graphDataZ = value; OnPropertyChanged(); }
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

        private async void OnOpenCloseCOM()
        {
            _wit = new Wit(SelectedPort);
            var isOpen = await _wit.Open();
            if (!isOpen) return;

            IsOpenCOM = !IsOpenCOM;
        }

        private void OnStartStop()
        {
            if (_wit == null) return;
            IsRecording = !IsRecording;
            if (IsRecording)
            {
                Text = "New session";
                var startTime = TimeSpan.Zero;
                _wit.Start(OnFrame);
                GraphDataX = new GraphData(startTime, "X", _generalConfig.ChartConfig);
                GraphDataY = new GraphData(startTime, "Y", _generalConfig.ChartConfig);
                GraphDataZ = new GraphData(startTime, "Z", _generalConfig.ChartConfig);
                _timer.IsEnabled = true;
            }
            else
            {
                Text = "Session ended";
                _timer.IsEnabled = false;
                //_wit.Close();
                //_wit = null;
            }
        }

        private void OnFrame(WitFrame frame)
        {
            if (IsRecording)
            {
                _queue.Enqueue(frame);
            }
        }

        private void OnTimer(object sender, EventArgs e)
        {
            while (_queue.TryDequeue(out WitFrame frame))
            {
                GraphDataX.Push(frame, DataSelection.GroupByX);
                GraphDataY.Push(frame, DataSelection.GroupByY);
                GraphDataZ.Push(frame, DataSelection.GroupByZ);
            }

            GraphDataX.Update();
            GraphDataY.Update();
            GraphDataZ.Update();
        }

        private void OnZero()
        {
            if (_wit != null && _wit.IsOpen)
            {
                _wit.SendZero();
            }
        }
    }
}
