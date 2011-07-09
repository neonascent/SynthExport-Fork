using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SynthExport
{
    /// <summary>
    /// Interaction logic for CoordinateSystemSelectionWindow.xaml
    /// </summary>
    public partial class CoordinateSystemSelectionWindow : Window
    {
        public ObservableCollection<CoordinateSystem> CoordinateSystems { get; private set; }

        public CoordinateSystemSelectionWindow(bool showPointColumn)
        {
            InitializeComponent();

            if (showPointColumn)
                ((GridView)listViewCoordinateSystems.View).Columns.Add((GridViewColumn)FindResource("pointsColumn"));

            CoordinateSystems = new ObservableCollection<CoordinateSystem>();

            listViewCoordinateSystems.ItemsSource = CoordinateSystems;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (!CoordinateSystems.Any(cs => cs.ShouldBeExported))
                MessageBox.Show("You haven't selected any coordinate system yet.", "No coordinate system selected", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                DialogResult = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
