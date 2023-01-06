using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TSP_WPF.Helpers;
using TSP_WPF.Models;

namespace TSP_WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private MainWindow _window;
        private string _solutionPath = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;

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

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AbleToStart))]
        [NotifyPropertyChangedFor(nameof(AbleToPause))]
        [NotifyPropertyChangedFor(nameof(AbleToUnpause))]
        [NotifyPropertyChangedFor(nameof(AbleToStop))]
        [NotifyPropertyChangedFor(nameof(AbleToExit))]
        private bool _isLoaded;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AbleToStart))]
        [NotifyPropertyChangedFor(nameof(AbleToPause))]
        [NotifyPropertyChangedFor(nameof(AbleToUnpause))]
        [NotifyPropertyChangedFor(nameof(AbleToStop))]
        [NotifyPropertyChangedFor(nameof(AbleToExit))]
        private bool _isStarted;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AbleToStart))]
        [NotifyPropertyChangedFor(nameof(AbleToPause))]
        [NotifyPropertyChangedFor(nameof(AbleToUnpause))]
        [NotifyPropertyChangedFor(nameof(AbleToStop))]
        [NotifyPropertyChangedFor(nameof(AbleToExit))]
        private bool _isPaused;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AbleToStart))]
        [NotifyPropertyChangedFor(nameof(AbleToPause))]
        [NotifyPropertyChangedFor(nameof(AbleToUnpause))]
        [NotifyPropertyChangedFor(nameof(AbleToStop))]
        [NotifyPropertyChangedFor(nameof(AbleToExit))]
        private bool _isFinished;

        public bool AbleToStart => IsLoaded && (!IsStarted || IsFinished);

        public bool AbleToPause => IsStarted && !IsPaused && !IsFinished;

        public bool AbleToUnpause => IsStarted && IsPaused && !IsFinished;

        public bool AbleToStop => IsStarted && !IsFinished;

        public bool AbleToExit => !IsStarted || IsFinished;



        public MainViewModel(MainWindow window)
        {
            _window = window;

            IsLoaded = false;
            IsStarted = false;
            IsPaused = false;
            IsFinished = false;
            

            FilePath = "File path...";
            TasksChecked = true;
            ThreadsChecked = false;
            HowMany = 0;
            HowLongPhase1 = 0;
            HowLongPhase2 = 0;
            BestResult = -1;
            BestThreadId = "-1";
            SolutionCount = -1;
            ProgressString = "-1";
            Progress = 0;
            OptimalTour = new ObservableCollection<City>();
            Cities = new ObservableCollection<City>();
        }

        [RelayCommand]
        private void LoadFile()
        {
            Debug.WriteLine("LoadFile...");
            
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = _solutionPath + @"\Resources";
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                Debug.WriteLine("FilePath: " + FilePath);
                Cities.Clear();
                OptimalTour.Clear();
                var cities = TspFileLoader.CreateCitiesListFromFile(_filePath);

                foreach (var city in cities)
                {
                    Cities.Add(city);
                    OptimalTour.Add(city);
                }

                IsLoaded = true;
            }
        }

        [RelayCommand]
        private void Start()
        {
            Debug.WriteLine("Start...");
            IsStarted = true;
            IsPaused = false;
            IsFinished = false;
        }

        [RelayCommand]
        private void Pause()
        {
            Debug.WriteLine("Pause...");
            IsPaused = true;
        }

        [RelayCommand]
        private void Unpause()
        {
            Debug.WriteLine("Unpause...");
            IsPaused = false;
        }

        [RelayCommand]
        private void Stop()
        {
            Debug.WriteLine("Stop...");
            IsStarted = false;
            IsPaused = false;
            IsFinished = true;
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
            foreach (var field in this.GetType().GetRuntimeProperties())
            {
                var key = field.Name;
                var value = field.GetValue(this);
                Debug.WriteLine($"Property: {key} | Value: {value}");
            }
        }
    }
}