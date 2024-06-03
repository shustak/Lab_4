using System.Windows;

namespace IntegralCalculatorApp
{
    public partial class InputWindow : Window
    {
        public double LowerLimit { get; private set; }
        public double UpperLimit { get; private set; }
        public int Intervals { get; private set; }

        public InputWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(LowerLimitTextBox.Text, out double lower) &&
                double.TryParse(UpperLimitTextBox.Text, out double upper) &&
                int.TryParse(IntervalsTextBox.Text, out int intervals))
            {
                LowerLimit = lower;
                UpperLimit = upper;
                Intervals = intervals;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please enter valid numerical values.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
