﻿using CommunityToolkit.Mvvm.ComponentModel;
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
using System.Windows;
using System.Windows.Input;
using TSP_WPF.Helpers;
using TSP_WPF.Models;

namespace TSP_WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
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

        public ObservableCollection<CityViewModel> CityViewModels { get; private set; }

        public ObservableCollection<EdgeViewModel> EdgeViewModels { get; private set; }

        public MainViewModel(MainWindow window)
        {
            _window = window;

            var canvas = window.canvas;

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

            RefreshCanvas();
            IsLoaded = true;
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

            for(var i = 0; i < OptimalTour.Count; i++)
            {
                var j = (i + 1) % OptimalTour.Count;
                EdgeViewModels.Add(new EdgeViewModel(CityViewModels[i], CityViewModels[j]));
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
            {
                if (city.X > max)
                {
                    max = city.X;
                }
            }

            return CanvasWidth / (max - min);
        }

        private double CalculateScaleY()
        {
            double min = CalculateOffsetY();
            double max = Cities[0].Y;
            foreach (City city in Cities)
            {
                if (city.Y > max)
                {
                    max = city.Y;
                }
            }

            return CanvasHeight / (max - min);
        }

        private double CalculateOffsetX()
        {
            double min = Cities[0].X;
            foreach (City city in Cities)
            {
                if (city.X < min)
                {
                    min = city.X;
                }
            }

            return min;
        }

        private double CalculateOffsetY()
        {
            double min = Cities[0].Y;
            foreach (City city in Cities)
            {
                if (city.Y < min)
                {
                    min = city.Y;
                }
            }

            return min;
        }
    }
}