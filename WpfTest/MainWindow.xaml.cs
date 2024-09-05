using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TqkLibrary.WinApi.Helpers;

namespace WpfTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly System.Timers.Timer _timer = new System.Timers.Timer(1000);
        public MainWindow()
        {
            InitializeComponent();
            _timer.Elapsed += _timer_Elapsed;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }
        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.InvokeAsync(Test_MonitorHelpersGetLocationApp);
        }









        int screenPossIndex = 0;
        private void Test_MonitorHelpersGetLocationApp()
        {
            (var size, var point) = MonitorHelper.GetLocationApp(screenPossIndex);
            screenPossIndex++;
            this.Width = size.Width;
            this.Height = size.Height;
            this.Top = point.Y;
            this.Left = point.X;
        }
    }
}