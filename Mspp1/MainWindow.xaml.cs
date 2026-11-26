
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;

namespace TaskManager
{
    public partial class MainWindow : Window
    {
        ObservableCollection<ProcessInfo> processes =
            new ObservableCollection<ProcessInfo>();

        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            ProcessGrid.ItemsSource = processes;

            timer.Tick += (s, e) => RefreshProcesses();

            RefreshProcesses();
        }

        // Task 1: Display processes

        void RefreshProcesses()
        {
            int selectedId = -1;

            if (ProcessGrid.SelectedItem is ProcessInfo selected)
            {
                selectedId = selected.Id;
            }

            processes.Clear();

            Process[] allProcesses = Process.GetProcesses();

            foreach (Process process in allProcesses)
            {
                try
                {
                    ProcessInfo info = new ProcessInfo();

                    info.Name = process.ProcessName;
                    info.Id = process.Id;
                    info.ThreadCount = process.Threads.Count;

                    try
                    {
                        info.CpuTime =
                            process.TotalProcessorTime.ToString(@"hh\:mm\:ss");
                    }
                    catch
                    {
                        info.CpuTime = "Unavailable";
                    }

                    processes.Add(info);
                }
                catch
                {
                    // Some processes may be unavailable
                }
                finally
                {
                    process.Dispose();
                }
            }

            if (selectedId != -1)
            {
                ProcessInfo previous = processes.FirstOrDefault(
                    p => p.Id == selectedId);

                if (previous != null)
                {
                    ProcessGrid.SelectedItem = previous;
                    ProcessGrid.ScrollIntoView(previous);
                }
            }

            if (ProcessGrid.SelectedItem == null &&
                processes.Count > 0)
            {
                ProcessGrid.SelectedIndex = 0;
            }

            ShowDetails();
        }

        // Start automatic refresh

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(IntervalBox.Text, out int seconds) ||
                seconds < 1 || seconds > 60)
            {
                MessageBox.Show(
                    "Enter an interval from 1 to 60 seconds!");

                return;
            }

            timer.Interval = TimeSpan.FromSeconds(seconds);
            timer.Start();

            RefreshProcesses();
        }

        // Stop automatic refresh

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        // Manual refresh

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshProcesses();
        }

        // Task 2: Display process details

        private void ProcessGrid_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            ShowDetails();
        }

        void ShowDetails()
        {
            if (ProcessGrid.SelectedItem is not ProcessInfo selected)
            {
                ClearDetails();
                return;
            }

            try
            {
                using (Process process =
                       Process.GetProcessById(selected.Id))
                {
                    NameText.Text = process.ProcessName;
                    IdText.Text = process.Id.ToString();

                    try
                    {
                        StartTimeText.Text =
                            process.StartTime.ToString();
                    }
                    catch
                    {
                        StartTimeText.Text = "Unavailable";
                    }

                    try
                    {
                        CpuTimeText.Text =
                            process.TotalProcessorTime.ToString();
                    }
                    catch
                    {
                        CpuTimeText.Text = "Unavailable";
                    }

                    try
                    {
                        ThreadCountText.Text =
                            process.Threads.Count.ToString();
                    }
                    catch
                    {
                        ThreadCountText.Text = "Unavailable";
                    }

                    Process[] copies =
                        Process.GetProcessesByName(process.ProcessName);

                    CopiesText.Text = copies.Length.ToString();

                    foreach (Process copy in copies)
                    {
                        copy.Dispose();
                    }
                }
            }
            catch
            {
                ClearDetails();
                NameText.Text = "Process is no longer available.";
            }
        }

        void ClearDetails()
        {
            NameText.Text = "-";
            IdText.Text = "-";
            StartTimeText.Text = "-";
            CpuTimeText.Text = "-";
            ThreadCountText.Text = "-";
            CopiesText.Text = "-";
        }

        // Task 3: Terminate selected process

        private void Kill_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessGrid.SelectedItem is not ProcessInfo selected)
            {
                MessageBox.Show("Select a process first!");
                return;
            }

            if (selected.Id == Process.GetCurrentProcess().Id)
            {
                MessageBox.Show(
                    "You cannot terminate this application!");

                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to terminate " +
                selected.Name + "?",
                "Confirm termination",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (Process process =
                           Process.GetProcessById(selected.Id))
                    {
                        process.Kill();
                        process.WaitForExit(1000);
                    }

                    MessageBox.Show("Process terminated successfully!");

                    RefreshProcesses();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Cannot terminate process:\n" + ex.Message);
                }
            }
        }

        // Task 4: Launch applications

        void LaunchApplication(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show("Select an application first!");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = fileName,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Cannot launch application:\n" + ex.Message);
            }
        }

        private void Notepad_Click(object sender, RoutedEventArgs e)
        {
            LaunchApplication("notepad.exe");
        }

        private void Calculator_Click(object sender, RoutedEventArgs e)
        {
            LaunchApplication("calc.exe");
        }

        private void Paint_Click(object sender, RoutedEventArgs e)
        {
            LaunchApplication("mspaint.exe");
        }

        // Select custom application

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Applications (*.exe)|*.exe";

            if (dialog.ShowDialog() == true)
            {
                PathBox.Text = dialog.FileName;
            }
        }

        private void Launch_Click(object sender, RoutedEventArgs e)
        {
            LaunchApplication(PathBox.Text);
        }
    }

    // Process information class

    public class ProcessInfo
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int ThreadCount { get; set; }
        public string CpuTime { get; set; }
    }
}
