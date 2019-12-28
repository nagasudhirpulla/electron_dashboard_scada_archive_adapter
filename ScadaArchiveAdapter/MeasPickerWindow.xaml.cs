using AdapterUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScadaArchiveAdapter
{
    /// <summary>
    /// Interaction logic for MeasPickerWindow.xaml
    /// </summary>
    public partial class MeasPickerWindow : Window
    {
        private const string measFilename = "scadaArchiveMeas.json";
        public ConfigurationManager Config_ { get; set; } = new ConfigurationManager();
        private List<ScadaArchMeasurement> ScadaArchMeasList_ = new List<ScadaArchMeasurement>();
        DataFetcher dataFetcher = new DataFetcher();
        public MeasPickerWindow()
        {
            InitializeComponent();
            Config_.Initialize();
            dataFetcher.Config_ = Config_;
            Initialize();
        }

        private void ShutdownApp()
        {
            Application.Current.Shutdown();
        }
        private string GetMeasListJSONPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + "\\" + measFilename;
        }
        public void Initialize()
        {
            string jsonPath = GetMeasListJSONPath();

            if (File.Exists(jsonPath))
            {
                ScadaArchMeasList_ = JsonConvert.DeserializeObject<List<ScadaArchMeasurement>>(File.ReadAllText(jsonPath));
                MeasListView.ItemsSource = ScadaArchMeasList_;
            }
            else
            {
                RefreshMeasurementsAsync();
            }
        }

        public void SaveMeasList()
        {
            string ConfigJSONStr = JsonConvert.SerializeObject(ScadaArchMeasList_, Formatting.Indented);

            string jsonPath = GetMeasListJSONPath();

            File.WriteAllText(jsonPath, ConfigJSONStr);
        }

        private async Task RefreshMeasurementsAsync()
        {

            ScadaArchMeasList_ = await dataFetcher.FetchMeasList();
            MeasListView.ItemsSource = ScadaArchMeasList_;
            SaveMeasList();
        }

        // todo delete this function
        private List<ScadaArchMeasurement> FetchMeasList(ConfigurationManager config)
        {
            return new List<ScadaArchMeasurement>();
        }
        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshMeasurementsAsync();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ShutdownApp();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            //todo console the selected measurement
            int selectedIndex = MeasListView.SelectedIndex;
            if (selectedIndex > -1)
            {
                var selObj = MeasListView.SelectedItems[0];
                string measId = (string)selObj.GetType().GetProperty("MeasId").GetValue(selObj, null);
                string measDesc = (string)selObj.GetType().GetProperty("MeasTag").GetValue(selObj, null);
                // measId, measName, measDescription
                ConsoleUtils.FlushMeasData(measId, measId, measDesc);
                ShutdownApp();
            }
            else
            {
                MessageBox.Show("Please select a measurement...");
            }
        }

        private void FilterTxt_Changed(object sender, RoutedEventArgs e)
        {
            List<ScadaArchMeasurement> filteredMeasurements = ScadaArchMeasList_;
            if (!string.IsNullOrEmpty(TypeFilter.Text))
            {
                filteredMeasurements = filteredMeasurements.Where(item =>
                {
                    string measType = item.MeasType;
                    return measType.ToUpper().Contains(TypeFilter.Text.ToUpper());
                }).ToList();
            }
            if (!string.IsNullOrEmpty(IdFilter.Text))
            {
                filteredMeasurements = filteredMeasurements.Where(item =>
                {
                    string measId = item.MeasId;
                    return measId.ToUpper().Contains(IdFilter.Text.ToUpper());
                }).ToList();
            }
            if (!string.IsNullOrEmpty(DescFilter.Text))
            {
                filteredMeasurements = filteredMeasurements.Where(item =>
                {
                    string measDesc = item.MeasTag;
                    return measDesc.ToUpper().Contains(DescFilter.Text.ToUpper());
                }).ToList();
            }
            MeasListView.ItemsSource = filteredMeasurements;
        }

    }
}
