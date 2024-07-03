using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace CarSales
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Metoda pro otevření souboru
        private void OpenXmlButton_Click(object sender, RoutedEventArgs e)
        {
            //Otevření dialogu pro výběr souboru
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";


            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    //Volání metody, která dále pracuje se souborem, pokud byl úspěšně otevřen
                    LoadXmlFile(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    //Zachycení a zobrazení chyby
                    MessageBox.Show("Chyba při otevírání souboru: " + ex.Message);
                }
            }
        }


        //Metoda pro načtení a zpracování souboru
        private void LoadXmlFile(string fileName)
        {
            string xmlContent = File.ReadAllText(fileName);
            XElement xml = XElement.Parse(xmlContent);

            //Seskupení dat podle modelu
            var cars = from car in xml.Elements("Auto")
                       let model = car.Element("Model")
                       where model != null //filtrování záznamů bez modelu
                       group car by (string)model into carGroup
                       select new
                       {
                           Model = carGroup.Key,
                           //Výpočty celkové ceny vozidel a celkové ceny s DPH, kde pokud je uvedená hodnota null nastavíme 0
                           CelkovaCena = carGroup.Sum(x => (double?)x.Element("Cena") ?? 0.0),
                           CelkovaCenaSDPH = carGroup.Sum(x => ((double?)x.Element("Cena") ?? 0.0) * (1 + ((double?)x.Element("DPH")?? 0.0) / 100))
                       };

            XmlDataGrid.ItemsSource = cars.ToList();
        }

    }
}
