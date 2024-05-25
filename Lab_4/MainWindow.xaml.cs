using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace IntegralCalculatorApp
{
    public partial class MainWindow : Window
    {
        BackgroundWorker backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker = (BackgroundWorker)this.Resources["worker"];
        }

        private async void StartDispatcherButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            StartDispatcherButton.IsEnabled = false;
            StartBackgroundWorkerButton.IsEnabled = false;

            double lowerLimit = double.Parse(LowerLimitTextBox.Text);
            double upperLimit = double.Parse(UpperLimitTextBox.Text);
            int intervals = int.Parse(IntervalsTextBox.Text);

            await CalculateAsync(lowerLimit, upperLimit, intervals);
        }

        private Task CalculateAsync(double lowerLimit, double upperLimit, int intervals)
        {
            var step = Math.Max(1, intervals / 100);
            return Task.Run(() =>
            {
                double result = 0.0;
                double stepSize = (upperLimit - lowerLimit) / intervals;

                for (int i = 0; i <= intervals; i++)
                {
                    double x = lowerLimit + i * stepSize;
                    double y = Function(x);
                    result += y * stepSize;

                    if (i % step == 0)
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                            new Action(() => ProgressBar.Value = (i * 100) / intervals));
                    }
                }

                Dispatcher.Invoke(() =>
                {
                    ProgressBar.Value = 100;
                    StartDispatcherButton.IsEnabled = true;
                    StartBackgroundWorkerButton.IsEnabled = true;
                    MessageBox.Show($"Integral Result (Dispatcher): {result}");
                });
            });
        }

        private double Function(double x)
        {
            return x * x * x; // Задание 6 
        }

        private void StartBackgroundWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            StartDispatcherButton.IsEnabled = false;
            StartBackgroundWorkerButton.IsEnabled = false;

            double lowerLimit = double.Parse(LowerLimitTextBox.Text);
            double upperLimit = double.Parse(UpperLimitTextBox.Text);
            int intervals = int.Parse(IntervalsTextBox.Text);

            backgroundWorker.RunWorkerAsync(new Tuple<double, double, int>(lowerLimit, upperLimit, intervals));
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (Tuple<double, double, int>)e.Argument;
            double lowerLimit = args.Item1;
            double upperLimit = args.Item2;
            int intervals = args.Item3;

            double result = 0.0;
            double stepSize = (upperLimit - lowerLimit) / intervals;
            int progressStep = Math.Max(1, intervals / 100);

            for (int i = 0; i <= intervals; i++)
            {
                double x = lowerLimit + i * stepSize;
                double y = Function(x);
                result += y * stepSize;

                if (i % progressStep == 0)
                {
                    int progress = (i * 100) / intervals;
                    backgroundWorker.ReportProgress(progress);
                }
            }

            e.Result = result;
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBar.Value = 100;
            StartDispatcherButton.IsEnabled = true;
            StartBackgroundWorkerButton.IsEnabled = true;
            MessageBox.Show($"Integral Result (BackgroundWorker): {e.Result}");
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(LowerLimitTextBox.Text) ||
                string.IsNullOrWhiteSpace(UpperLimitTextBox.Text) ||
                string.IsNullOrWhiteSpace(IntervalsTextBox.Text))
            {
                MessageBox.Show("All fields must be filled in.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!double.TryParse(LowerLimitTextBox.Text, out _) ||
                !double.TryParse(UpperLimitTextBox.Text, out _) ||
                !int.TryParse(IntervalsTextBox.Text, out _))
            {
                MessageBox.Show("Please enter valid numerical values.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
