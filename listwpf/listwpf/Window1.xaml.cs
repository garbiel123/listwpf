using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace listwpf
{
    public partial class Window1 : Window
    {
        public Uczen? NowyUczen { get; private set; }
        private bool _isModified = false;

        public Window1()
        {
            InitializeComponent();
            ChckChanges();
        }

        private void ChckChanges()
        {
            foreach (var control in new Control[] { PeselBox, ImieBox, DrugieImieBox, NazwiskoBox, TelefonBox, AdresBox, MiejscowoscBox, KodPocztowyBox })
            {
                if (control is TextBox tb)
                    tb.TextChanged += (s, e) => _isModified = true;
            }
            DataUrodzeniaPicker.SelectedDateChanged += (s, e) => _isModified = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (_isModified && MessageBox.Show("Czy na pewno chcesz zamknąć bez zapisywania?", "Potwierdzenie", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            this.DialogResult = false;
            this.Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            ClearErrors();
            if (!Fields())
                return;

            NowyUczen = new Uczen()
            {
                Pesel = PeselBox.Text.Trim(),
                Imie = FormatText(ImieBox.Text),
                DrugieImie = FormatText(DrugieImieBox.Text),
                Nazwisko = FormatText(NazwiskoBox.Text),
                DataUrodzenia = DataUrodzeniaPicker.SelectedDate?.ToShortDateString() ?? "",
                Telefon = FormatPhone(TelefonBox.Text),
                Adres = FormatText(AdresBox.Text),
                Miejscowosc = FormatText(MiejscowoscBox.Text),
                KodPocztowy = KodPocztowyBox.Text.Trim()
            };

            this.DialogResult = true;
            this.Close();
        }

        private void ClearErrors()
        {
            foreach (var control in new Control[] { PeselBox, ImieBox, NazwiskoBox, DataUrodzeniaPicker, AdresBox, MiejscowoscBox, KodPocztowyBox })
            {
                if (control is TextBox tb)
                    tb.BorderBrush = Brushes.Gray;
                else if (control is DatePicker dp)
                    dp.BorderBrush = Brushes.Gray;
            }
        }

        private bool Fields()
        {
            bool valid = true;

            if (string.IsNullOrWhiteSpace(PeselBox.Text) || !IsPeselRight(PeselBox.Text))
            {
                PeselBox.BorderBrush = Brushes.Red;
                valid = false;
            }

            if (!CheckPeselBirthdate(PeselBox.Text, DataUrodzeniaPicker.SelectedDate))
            {
                PeselBox.BorderBrush = Brushes.Red;
                DataUrodzeniaPicker.BorderBrush = Brushes.Red;
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(ImieBox.Text))
            {
                ImieBox.BorderBrush = Brushes.Red;
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(NazwiskoBox.Text))
            {
                NazwiskoBox.BorderBrush = Brushes.Red;
                valid = false;
            }

            if (DataUrodzeniaPicker.SelectedDate == null)
            {
                DataUrodzeniaPicker.BorderBrush = Brushes.Red;
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(AdresBox.Text))
            {
                AdresBox.BorderBrush = Brushes.Red;
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(MiejscowoscBox.Text))
            {
                MiejscowoscBox.BorderBrush = Brushes.Red;
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(KodPocztowyBox.Text))
            {
                KodPocztowyBox.BorderBrush = Brushes.Red;
                valid = false;
            }

            return valid;
        }

        private string FormatText(string text)
        {
            text = text.Trim();
            if (text.Length == 0)
                return "";
            text = text.ToLower();
            return char.ToUpper(text[0]) + text.Substring(1);
        }

        private string FormatPhone(string text)
        {
            string digits = "";
            foreach (char c in text)
            {
                if (char.IsDigit(c))
                    digits += c;
            }

            if (digits.StartsWith("48"))
                return "+" + digits;
            if (!digits.StartsWith("48"))
                return "+48" + digits;

            return "+" + digits;
        }

        private bool IsPeselRight(string pesel)
        {
            if (pesel.Length != 11 || !ulong.TryParse(pesel, out _))
                return false;

            int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            int sum = 0;
            for (int i = 0; i < 10; i++)
                sum += weights[i] * (pesel[i] - '0');

            int controlDigit = (10 - (sum % 10)) % 10;
            return controlDigit == pesel[10] - '0';
        }

        private bool CheckPeselBirthdate(string pesel, DateTime? date)
        {
            if (pesel.Length < 6 || date == null)
                return false;

            string peselDate = pesel.Substring(0, 6);
            int year = date.Value.Year % 100;
            int month = date.Value.Month + 20;
            int day = date.Value.Day;

            string expected = year.ToString("D2") + month.ToString("D2") + day.ToString("D2");
            return peselDate == expected;
        }
    }
}
