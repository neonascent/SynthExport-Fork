using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace SynthExport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SynthData synthData;

        PointCloudDownloader pointCloudDownloader;

        Thread exportThread;

        bool exportInProgress;

        public MainWindow()
        {
            InitializeComponent();

            pointCloudDownloader = new PointCloudDownloader();
            pointCloudDownloader.CallbackEvent += pointCloudDownloader_CallbackEvent;
        }

        private void pointCloudDownloader_CallbackEvent(object sender, PointCloudDownloader.CallbackEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                progressBar.Value = e.FilesDownloaded;
                progressBar.Maximum = e.TotalFileCount;
            }));
        }

        private void UpdateStatus(string status)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                statusTextBlock.Text = status;
            }));
        }

        public void ExportThread(object exportSettings)
        {
            ExportSettings settings = (ExportSettings)exportSettings;

            try
            {
                if (settings.Source == ExportSource.Website)
                {
                    synthData = SynthData.Load(settings.SourcePath);

                    if (settings.ExportPointClouds)
                    {
                        UpdateStatus("Downloading point clouds..");
                        pointCloudDownloader.DownloadPointClouds(synthData);
                    }
                }
                else
                    synthData = SynthData.LoadFromZipFile(settings.SourcePath);
            }
            catch (Exception e)
            {
                if (!(e is ThreadAbortException))
                    Dispatcher.Invoke(new Action<Exception>(ExportThreadError), e);

                return;
            }

            Dispatcher.Invoke(new Action<ExportSettings>(ExportThreadFinished), settings);
        }

        private void ExportThreadFinished(ExportSettings exportSettings)
        {
            if (synthData.CoordinateSystems.Length == 0)
            {
                MessageBox.Show("This photosynth is empty.", "No data in this synth", MessageBoxButton.OK, MessageBoxImage.Information);
                EndExport(true);
                return;
            }

            if (synthData.CoordinateSystems.Length > 1)
            {
                CoordinateSystemSelectionWindow coordinateSystemsWindow = new CoordinateSystemSelectionWindow(exportSettings.ExportPointClouds);

                coordinateSystemsWindow.Owner = this;

                foreach (CoordinateSystem coordSystem in synthData.CoordinateSystems)
                    coordinateSystemsWindow.CoordinateSystems.Add(coordSystem);

                if (coordinateSystemsWindow.ShowDialog() == false)
                {
                    EndExport(false);
                    return;
                }
            }
            else
                synthData.CoordinateSystems[0].ShouldBeExported = true;

            var coordSystemsToExport = synthData.CoordinateSystems.Where(cs => cs.ShouldBeExported);

            bool dataExported = false;

            if (exportSettings.ExportPointClouds)
            {
                var coordSystemsWithPointCloud = coordSystemsToExport.Where(cs => cs.PointCloud != null);

                if (coordSystemsWithPointCloud.Count() == 0)
                    MessageBox.Show("This photosynth has not any point clouds.", "No point clouds in this synth", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();

                    saveFileDialog.Title = "Save point clouds";

                    switch (comboBoxOutputFormat.SelectedIndex)
                    {
                        case 0:
                            saveFileDialog.Filter = "OBJ format (*.obj)|*.obj";
                            break;
                        case 1:
                        case 2:
                            saveFileDialog.Filter = "Polygon file format (*.ply)|*.ply";
                            break;
                        case 3:
                            saveFileDialog.Filter = "VRML format (*.wrl)|*.wrl";
                            break;
                        case 4:
                            saveFileDialog.Filter = "X3D format (*.x3d)|*.x3d";
                            break;
                    }

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        foreach (CoordinateSystem coordSystem in coordSystemsWithPointCloud)
                        {
                            string fileName = saveFileDialog.FileName.Insert(saveFileDialog.FileName.Length - 4, "_" + coordSystem.ID);

                            switch (comboBoxOutputFormat.SelectedIndex)
                            {
                                case 0:
                                    coordSystem.PointCloud.ExportAsObj(fileName);
                                    break;
                                case 1:
                                    coordSystem.PointCloud.ExportAsPlyAscii(fileName);
                                    break;
                                case 2:
                                    coordSystem.PointCloud.ExportAsPlyBinary(fileName);
                                    break;
                                case 3:
                                    coordSystem.PointCloud.ExportAsVrml(fileName);
                                    break;
                                case 4:
                                    coordSystem.PointCloud.ExportAsX3d(fileName);
                                    break;
                            }
                        }

                        MessageBox.Show("The point clouds have been exported.", "Export completed", MessageBoxButton.OK, MessageBoxImage.Information);

                        dataExported = true;
                    }
                }
            }

            if (exportSettings.ExportCameraParameters)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Title = "Save camera parameters";
                saveFileDialog.Filter = "Comma-separated values (*.csv)|*.csv";

                if (saveFileDialog.ShowDialog() == true)
                {
                    foreach (CoordinateSystem coordSystem in coordSystemsToExport)
                    {
                        string fileName = saveFileDialog.FileName.Insert(saveFileDialog.FileName.Length - 4, "_" + coordSystem.ID);

                        coordSystem.CameraParameterList.ExportAsCsv(fileName);
                    }

                    MessageBox.Show("The camera parameters have been exported.", "Export completed", MessageBoxButton.OK, MessageBoxImage.Information);

                    dataExported = true;
                }
            }

            if (exportSettings.ExportMaxScript)
            {
                
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Title = "Save 3DS Max object setup";
                saveFileDialog.Filter = "MaxScript (*.ms)|*.ms";

                if (saveFileDialog.ShowDialog() == true)
                {
                    foreach (CoordinateSystem coordSystem in coordSystemsToExport)
                    {
                        string fileName = saveFileDialog.FileName.Insert(saveFileDialog.FileName.Length - 3, "_" + coordSystem.ID);

                        // and save max script 
                        coordSystem.CameraParameterList.ExportAsMaxScript(fileName);

                    }

                    MessageBox.Show("The camera parameters have been exported.", "Export completed", MessageBoxButton.OK, MessageBoxImage.Information);

                    dataExported = true;
                }
            }


            if (exportSettings.ExportMaxScriptPos)
            {
                
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Title = "Save 3DS Max camera position setup";
                saveFileDialog.Filter = "MaxScript (*.ms)|*.ms";

                if (saveFileDialog.ShowDialog() == true)
                {
                    foreach (CoordinateSystem coordSystem in coordSystemsToExport)
                    {
                        string name = saveFileDialog.SafeFileName.Substring(0, saveFileDialog.SafeFileName.Length - 3); 
                        
                        string fileName = saveFileDialog.FileName.Insert(saveFileDialog.FileName.Length - 3, "-positions_" + coordSystem.ID);

                        // save just camera positions
                        coordSystem.CameraParameterList.ExportAsMaxScriptJustCameras(fileName, name);
                    }

                    MessageBox.Show("The camera positions have been exported.", "Export completed", MessageBoxButton.OK, MessageBoxImage.Information);

                    dataExported = true;
                }
            }

            if (exportSettings.ExportMaxScriptSpheres)
            {

                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Title = "Save 3DS Max sensor value spheres";
                saveFileDialog.Filter = "MaxScript (*.ms)|*.ms";

                if (saveFileDialog.ShowDialog() == true)
                {
                    foreach (CoordinateSystem coordSystem in coordSystemsToExport)
                    {
                        string name = saveFileDialog.SafeFileName.Substring(0, saveFileDialog.SafeFileName.Length - 3);

                        string fileName = saveFileDialog.FileName.Insert(saveFileDialog.FileName.Length - 3, "-sensor_values_" + coordSystem.ID);

                        // save just camera positions
                        coordSystem.CameraParameterList.ExportAsMaxScriptSensorSpheres(fileName, name);
                    }

                    MessageBox.Show("The sensor values have been exported.", "Export completed", MessageBoxButton.OK, MessageBoxImage.Information);

                    dataExported = true;
                }
            }

            EndExport(dataExported);
        }

        public void ExportThreadError(Exception exception)
        {
            EndExport(false);

            if (exception is ApplicationException)
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show(string.Format("An error occurred during export: {0}", exception.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void StartExport()
        {
            buttonExport.Content = "Abort...";
            statusTextBlock.Text = "Getting synth data..";
            exportInProgress = true;
            groupBox1.IsEnabled = false;
            checkBoxPointClouds.IsEnabled = false;
            checkBoxCameraParameters.IsEnabled = false;
            comboBoxOutputFormat.IsEnabled = false;
            progressBar.Value = 0;
        }

        public void EndExport(bool success)
        {
            synthData = null;
            exportThread = null;

            buttonExport.Content = "Export...";
            statusTextBlock.Text = success ? "Done." : "Ready.";
            exportInProgress = false;
            groupBox1.IsEnabled = true;
            checkBoxPointClouds.IsEnabled = true;
            checkBoxCameraParameters.IsEnabled = true;
            comboBoxOutputFormat.IsEnabled = true;
            progressBar.Value = success ? 100 : 0;
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Environment.ExpandEnvironmentVariables("%tmp%\\Photosynther");
            openFileDialog.Filter = "Synth data file (collection.synth.bin)|collection.synth.bin";

            if (openFileDialog.ShowDialog() == true)
                textBoxFile.Text = openFileDialog.FileName;
        }

        private void buttonExport_Click(object sender, RoutedEventArgs e)
        {
            if (!exportInProgress)
            {
                ExportSource exportSource;
                string sourcePath;                

                if (radioButtonFromUrl.IsChecked == true)
                {
                    if (textBoxUrl.Text == string.Empty)
                    {
                        MessageBox.Show("You haven't specified the URL of your synth. Go to your synth on photosynth.net and copy the address from your browser.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string url = textBoxUrl.Text.Trim().ToLower();

                    int i = url.IndexOf("cid=");

                    if (i < 0 || url.Length < i + 40)
                    {
                        MessageBox.Show("The URL you have entered is invalid. Go to your synth on photosynth.net and copy the address from your browser.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    exportSource = ExportSource.Website;
                    sourcePath = url.Substring(i + 4, 36);
                }
                else
                {
                    if (textBoxFile.Text == string.Empty)
                    {
                        string photosynthFolder = Environment.ExpandEnvironmentVariables("%tmp%\\Photosynther");

                        MessageBox.Show(string.Format("You haven't specified the path to your collection.synth.bin file. It should be in the data folder of Photosynth ({0}).", photosynthFolder), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!File.Exists(textBoxFile.Text))
                    {
                        MessageBox.Show("The file path you have entered does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    exportSource = ExportSource.ZipFile;
                    sourcePath = textBoxFile.Text;
                }

                bool exportPointClouds = checkBoxPointClouds.IsChecked == true;
                bool exportCameraParameters = checkBoxCameraParameters.IsChecked == true;
                bool exportMaxScript = checkBoxMaxScript.IsChecked == true;
                bool exportMaxScriptPos = checkBoxMaxScriptPos.IsChecked == true;
                bool exportMaxScriptSpheres = checkBoxMaxScriptSpheres.IsChecked == true;

                if (!exportPointClouds && !exportCameraParameters && !exportMaxScript && !exportMaxScriptPos && !exportMaxScriptSpheres)
                {
                    MessageBox.Show("You haven't selected the data you want to export. You can export point clouds, camera parameters or both.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ExportSettings exportSettings = new ExportSettings(exportSource, sourcePath, exportPointClouds, exportCameraParameters, exportMaxScript, exportMaxScriptPos, exportMaxScriptSpheres);

                StartExport();

                exportThread = new Thread(new ParameterizedThreadStart(ExportThread));

                exportThread.IsBackground = true;
                exportThread.Priority = ThreadPriority.BelowNormal;

                exportThread.Start(exportSettings);
            }
            else
            {
                if (MessageBox.Show("Abort point cloud export?", "Abort?", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
                    return;

                if (exportThread != null)
                {
                    exportThread.Abort();
                    exportThread.Join();

                    EndExport(false);
                }
            }
        }

        private void textBlockWebsite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://synthexport.codeplex.com/");
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                buttonExport_Click(sender, new RoutedEventArgs());
            }
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void comboBox1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void textBlock2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://tacticalspace.org/");
        }
    }
}
