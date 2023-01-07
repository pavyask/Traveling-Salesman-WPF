using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TSP_WPF.Helpers;
using TSP_Shared.Models;
using H.Pipes;
using System.Threading.Tasks;
using System;
using System.Threading;
using Xceed.Wpf.Toolkit;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using H.Pipes.Args;
using Microsoft.VisualBasic;
using System.Timers;

namespace TSP_WPF.ViewModels
{

    public partial class MainViewModel : ObservableObject
    {
        private PipeServer<PipeMessage> _pipeServer;

        private PausableTimer _timer;

        public enum ApplicationState
        {
            DATA_NOT_LOADED,
            DATA_LOADED,
            RUNNING,
            PAUSED,
            FINISHED
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AbleToStart))]
        [NotifyPropertyChangedFor(nameof(AbleToPause))]
        [NotifyPropertyChangedFor(nameof(AbleToUnpause))]
        [NotifyPropertyChangedFor(nameof(AbleToStop))]
        [NotifyPropertyChangedFor(nameof(AbleToExit))]
        private ApplicationState _appState;

        public bool AbleToStart => AppState == ApplicationState.DATA_LOADED || AppState == ApplicationState.FINISHED;

        public bool AbleToPause => AppState == ApplicationState.RUNNING;

        public bool AbleToUnpause => AppState == ApplicationState.PAUSED;

        public bool AbleToStop => AppState == ApplicationState.RUNNING || AppState == ApplicationState.PAUSED;

        public bool AbleToExit => AppState == ApplicationState.DATA_LOADED || AppState == ApplicationState.FINISHED;

        private double _canvasWidth;
        public double CanvasWidth
        {
            get { return _canvasWidth - 10; }
            set
            {
                _canvasWidth = value;
                OnPropertyChanged(nameof(CanvasWidth));
                RefreshCanvas();
            }
        }

        private double _canvasHeight { get; set; }
        public double CanvasHeight
        {
            get { return _canvasHeight - 10; }
            set
            {
                _canvasHeight = value;
                OnPropertyChanged(nameof(CanvasHeight));
                RefreshCanvas();
            }
        }

        [ObservableProperty]
        private MainWindow _window;

        private readonly string _solutionPath = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;

        [ObservableProperty]
        private string _filePath;

        [ObservableProperty]
        private bool _tasksChecked;

        [ObservableProperty]
        private bool _threadsChecked;

        [ObservableProperty]
        private int _howMany;

        [ObservableProperty]
        private int _howLongPhase1;

        [ObservableProperty]
        private int _howLongPhase2;

        [ObservableProperty]
        private int _numberOfEpochs;

        [ObservableProperty]
        private double _bestResult;

        [ObservableProperty]
        private string _bestThreadId;

        [ObservableProperty]
        private int _solutionCount;

        [ObservableProperty]
        private string _progressString;

        [ObservableProperty]
        private int _progress;

        public ObservableCollection<City> OptimalTour { get; private set; }

        public ObservableCollection<City> Cities { get; private set; }

        public ObservableCollection<CityViewModel> CityViewModels { get; private set; }

        public ObservableCollection<EdgeViewModel> EdgeViewModels { get; private set; }

        public MainViewModel(MainWindow window)
        {
            Task.Run(async () => await Server());


            _window = window;
            AppState = ApplicationState.DATA_NOT_LOADED;
            FilePath = "File path...";
            TasksChecked = true;
            ThreadsChecked = false;
            HowMany = 4;
            HowLongPhase1 = 5;
            HowLongPhase2 = 5;
            NumberOfEpochs = 5;
            BestResult = -1;
            BestThreadId = "-1";
            SolutionCount = -1;
            ProgressString = "-1";
            Progress = 0;
            OptimalTour = new ObservableCollection<City>();
            Cities = new ObservableCollection<City>();
            CityViewModels = new ObservableCollection<CityViewModel>();
            EdgeViewModels = new ObservableCollection<EdgeViewModel>();
            LoadCitiesFromFile(_solutionPath + @"\Resources\wi29.tsp");
        }

        [RelayCommand]
        private void LoadFile()
        {
            Debug.WriteLine("LoadFile...");

            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = _solutionPath + @"\Resources";
            if (openFileDialog.ShowDialog() == true)
                LoadCitiesFromFile(openFileDialog.FileName);
        }

        private void LoadCitiesFromFile(string path)
        {
            FilePath = path;
            Cities.Clear();
            OptimalTour.Clear();
            var cities = TspFileLoader.CreateCitiesListFromFile(_filePath);

            foreach (var city in cities)
            {
                Cities.Add(city);
                OptimalTour.Add(city);
            }

            AppState = ApplicationState.DATA_LOADED;
            RefreshCanvas();
        }

        private void RefreshCanvas()
        {
            CityViewModels.Clear();
            EdgeViewModels.Clear();

            foreach (var city in OptimalTour)
            {
                double xVM = (city.X - CalculateOffsetX()) * CalculateScaleX();
                double yVM = (city.Y - CalculateOffsetY()) * CalculateScaleY();
                CityViewModels.Add(new CityViewModel(city.Id, xVM, yVM));
            }

            for (var i = 0; i < OptimalTour.Count; i++)
            {
                var j = (i + 1) % OptimalTour.Count;
                EdgeViewModels.Add(new EdgeViewModel(CityViewModels[i], CityViewModels[j]));
            }
        }
        private async Task Server()
        {
            _pipeServer = new PipeServer<PipeMessage>("tsp-wpf");
            _pipeServer.ClientConnected += ClientConnected;
            _pipeServer.MessageReceived += MessageReceived;

            await _pipeServer.StartAsync();
            await Task.Delay(Timeout.InfiniteTimeSpan);
        }

        private async void ClientConnected(object? sender, ConnectionEventArgs<PipeMessage> args)
        {
            Console.WriteLine($"Client {args.Connection.PipeName} is now connected!");

            await args.Connection.WriteAsync(new PipeMessage
            {
                CityList = Cities.ToList(),
                Type = MessageType.START,
                HowMany = HowMany
            });
        }

        private void MessageReceived(object? sender, ConnectionMessageEventArgs<PipeMessage?> args)
        {
            Console.WriteLine($"Client {args.Connection.PipeName} says: {args.Message}");
        }


        [RelayCommand]
        private void Start()
        {
            Debug.WriteLine("Start...");
            AppState = ApplicationState.RUNNING;

            if (TasksChecked)
            {
                Process.Start(_solutionPath + @"\TSP-WPF\bin\Debug\net7.0\TSP-Task.exe");
            }
            else
            {
                Process.Start(_solutionPath + @"\TSP-WPF\bin\Debug\net7.0\TSP-Thread.exe");
            }

            InitTimer();
        }

        private void InitTimer()
        {
            var milisecs = (HowLongPhase1 + HowLongPhase2) * NumberOfEpochs * 1000;
            _timer = new PausableTimer(milisecs);
            _timer.Elapsed += new ElapsedEventHandler(TimeOutEvent);
            _timer.AutoReset = false;
            _timer.Start();
            Debug.WriteLine($"Timer started at {DateTime.Now}");
        }

        private void TimeOutEvent(object? sender, ElapsedEventArgs e)
        {
            Stop();
        }

        [RelayCommand]
        private void Pause()
        {
            Debug.WriteLine("Pause...");
            AppState = ApplicationState.PAUSED;
            _pipeServer.WriteAsync(new PipeMessage { Type = MessageType.PAUSE });
            _timer.Pause();
            Debug.WriteLine($"Timer paused at {DateTime.Now}");
        }

        [RelayCommand]
        private void Unpause()
        {
            Debug.WriteLine("Unpause...");
            AppState = ApplicationState.RUNNING;
            _pipeServer.WriteAsync(new PipeMessage { Type = MessageType.UNPAUSE });
            _timer.Resume();
            Debug.WriteLine($"Timer unpaused at {DateTime.Now}");
        }

        [RelayCommand]
        private void Stop()
        {
            Debug.WriteLine("Stop...");
            AppState = ApplicationState.FINISHED;
            _pipeServer.WriteAsync(new PipeMessage { Type = MessageType.STOP });
            _timer.Stop();
            Debug.WriteLine($"Timer stoped at {DateTime.Now}");
        }

        [RelayCommand]
        private void Exit()
        {
            Debug.WriteLine("Exit...");
            System.Windows.Application.Current.Shutdown();
        }

        [RelayCommand]
        private void CheckData()
        {
            Debug.WriteLine("CheckData...");
            Debug.WriteLine($"Canvas width: {CanvasWidth}, Canvas height: {CanvasHeight}");
            foreach (var field in this.GetType().GetRuntimeProperties())
            {
                var key = field.Name;
                var value = field.GetValue(this);
                Debug.WriteLine($"Property: {key} | Value: {value}");
            }
        }

        private double CalculateScaleX()
        {
            double min = CalculateOffsetX();
            double max = Cities[0].X;
            foreach (City city in Cities)
                if (city.X > max)
                    max = city.X;

            return CanvasWidth / (max - min);
        }

        private double CalculateScaleY()
        {
            double min = CalculateOffsetY();
            double max = Cities[0].Y;
            foreach (City city in Cities)
                if (city.Y > max)
                    max = city.Y;

            return CanvasHeight / (max - min);
        }

        private double CalculateOffsetX()
        {
            double min = Cities[0].X;
            foreach (City city in Cities)
                if (city.X < min)
                    min = city.X;

            return min;
        }

        private double CalculateOffsetY()
        {
            double min = Cities[0].Y;
            foreach (City city in Cities)
                if (city.Y < min)
                    min = city.Y;

            return min;
        }
    }
}