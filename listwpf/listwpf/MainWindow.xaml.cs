using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace listwpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NewRecord_Click(object sender, RoutedEventArgs e)
        {
            Window1 win = new Window1();
            if (win.ShowDialog() == true)
            {
                listaUczniow.Items.Add(win.NowyUczen);
            }
        }

        private void RemoveSel_Click(object sender, RoutedEventArgs e)
        {
            while (listaUczniow.SelectedItems.Count > 0)
            {
                listaUczniow.Items.Remove(listaUczniow.SelectedItems[0]);
            }
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Pliki CSV z separatorem ',' lub ';'",
                Title = "Otwórz plik CSV"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                listaUczniow.Items.Clear();
                string delimiter = openFileDialog.FilterIndex == 1 ? "," : ";";

                try
                {
                    var lines = System.IO.File.ReadAllLines(openFileDialog.FileName, Encoding.UTF8);
                    foreach (var line in lines)
                    {
                        var columns = line.Split(delimiter);
                        if (columns.Length >= 9)
                        {
                            Uczen uczen = new()
                            {
                                Pesel = columns[0],
                                Imie = columns[1],
                                DrugieImie = columns[2],
                                Nazwisko = columns[3],
                                DataUrodzenia = columns[4],
                                Telefon = columns[5],
                                Adres = columns[6],
                                Miejscowosc = columns[7],
                                KodPocztowy = columns[8]
                            };
                            listaUczniow.Items.Add(uczen);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas odczytu pliku: " + ex.Message);
                }
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Pliki CSV z separatorem ',' lub ';'",
                Title = "Zapisz jako plik CSV"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string delimiter = saveFileDialog.FilterIndex == 1 ? "," : ";";

                try
                {
                    using (var writer = new System.IO.StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8))
                    {
                        foreach (Uczen item in listaUczniow.Items)
                        {
                            var row = string.Join(delimiter, new string[]
                            {
                                item.Pesel,
                                item.Imie,
                                item.DrugieImie,
                                item.Nazwisko,
                                item.DataUrodzenia,
                                item.Telefon,
                                item.Adres,
                                item.Miejscowosc,
                                item.KodPocztowy
                            });
                            writer.WriteLine(row);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas zapisu pliku: " + ex.Message);
                }
            }
        }
    }
        public class Uczen
    {
        public required string Pesel { get; set; }
        public required string Imie { get; set; }
        public string DrugieImie { get; set; }
        public required string Nazwisko { get; set; }
        public required string DataUrodzenia { get; set; }
        public string Telefon { get; set; }
        public required string Adres { get; set; }
        public required string Miejscowosc { get; set; }
        public required string KodPocztowy { get; set; }
    }
}
