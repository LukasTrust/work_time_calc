using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ArbeitszeitBerechner
{
    public partial class MainWindow : Window
    {
        private List<Arbeitstag> arbeitszeitenList;
        private int currentMonth;
        private int currentYear;
        private int dateOffset;
        public MainWindow()
        {
            InitializeComponent();
            // Initialize the list and load existing data from the file
            arbeitszeitenList = new List<Arbeitstag>();
            LoadArbeitszeitenFromFile();


            DateTime currentDate = DateTime.Now;
            txtMonatJahr.Text = currentDate.ToString("MMMM yyyy");
            dateOffset = 0;
            currentMonth = currentDate.Month;
            currentYear = currentDate.Year;
            LadeMonatsdaten(currentMonth, currentYear);
            CalculateAndDisplayTotalArbeitszeit(currentDate.Month + dateOffset, currentDate.Year);
        }
        private void LoadArbeitszeitenFromFile()
        {
            try
            {
                // Read all lines from the file
                string[] lines = File.ReadAllLines("arbeitszeiten.txt");

                // Parse each line and create Arbeitstag objects
                foreach (string line in lines)
                {
                    string[] data = line.Split(';');
                    if (data.Length >= 6)
                    {
                        DateTime datum = DateTime.ParseExact(data[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        TimeSpan arbeitsbeginn = TimeSpan.Parse(data[1]);
                        TimeSpan pausenbeginn = TimeSpan.Parse(data[2]);
                        TimeSpan pausenende = TimeSpan.Parse(data[3]);
                        TimeSpan arbeitsende = TimeSpan.Parse(data[4]);
                        TimeSpan arbeitszeit = TimeSpan.Parse(data[5]);

                        Arbeitstag arbeitstag = new Arbeitstag(datum, arbeitsbeginn, pausenbeginn, pausenende, arbeitsende, arbeitszeit);
                        arbeitszeitenList.Add(arbeitstag);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Arbeitszeiten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveArbeitszeitenListToFile()
        {
            try
            {
                // Convert the list of Arbeitstag objects to a list of strings in the required format
                List<string> lines = new List<string>();
                foreach (Arbeitstag arbeitstag in arbeitszeitenList)
                {
                    lines.Add($"{arbeitstag.Datum:yyyy-MM-dd};{arbeitstag.Arbeitsbeginn};{arbeitstag.Pausenbeginn};{arbeitstag.Pausenende};{arbeitstag.Arbeitsende};{arbeitstag.Arbeitszeit}");
                }

                // Write the data to the file (overwrite the existing file)
                File.WriteAllLines("arbeitszeiten.txt", lines);
                CalculateAndDisplayTotalArbeitszeit(currentMonth + dateOffset, currentYear);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Arbeitszeiten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime selectedDate = datePicker.SelectedDate ?? DateTime.MinValue;


                // Check if an entry with the selected date already exists
                bool entryExists = arbeitszeitenList.Any(item => item.Datum.Date == selectedDate.Date);

                if (entryExists)
                {
                    MessageBox.Show("Eintrag für das ausgewählte Datum existiert bereits.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                    return; // Exit the method without saving the entry
                }

                // Create a new Arbeitstag with the provided input values
                Arbeitstag newArbeitstag = new Arbeitstag(selectedDate,
                                                         TimeSpan.Parse(txtArbeitsbeginn.Text),
                                                         TimeSpan.Parse(txtPausenbeginn.Text),
                                                         TimeSpan.Parse(txtPausenende.Text),
                                                         TimeSpan.Parse(txtArbeitsende.Text),
                                                         CalculateArbeitszeit(txtArbeitsbeginn.Text,
                                                                              txtPausenbeginn.Text,
                                                                              txtPausenende.Text,
                                                                              txtArbeitsende.Text));

                // Add the new entry to the arbeitszeitenList
                arbeitszeitenList.Add(newArbeitstag);

                // Save the updated list to the file
                SaveArbeitszeitenListToFile();

                // Reload the DataGrid with the updated data for the selected month and year
                LadeMonatsdaten(currentMonth, currentYear);

                // Clear the input fields in the Arbeitszeit section after successful save
                txtArbeitsbeginn.Text = "";
                txtPausenbeginn.Text = "";
                txtPausenende.Text = "";
                txtArbeitsende.Text = "";
                txtArbeitszeit.Text = "";

                // Display a success message
                MessageBox.Show("Eintrag wurde erfolgreich gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Arbeitszeit: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string arbeitsbeginnStr = txtArbeitsbeginn.Text;
                string pausenbeginnStr = txtPausenbeginn.Text;
                string pausenendeStr = txtPausenende.Text;
                string arbeitsendeStr = txtArbeitsende.Text;

                TimeSpan arbeitszeit = CalculateArbeitszeit(arbeitsbeginnStr, pausenbeginnStr, pausenendeStr, arbeitsendeStr);

                // Format the working hours as "Stunden: <Anzahl der Stunden> Minuten: <Anzahl der Minuten>"
                string formattedArbeitszeit = $"Stunden: {arbeitszeit.Hours} Minuten: {arbeitszeit.Minutes}";

                // Update the "Arbeitszeit" TextBox with the formatted working hours
                txtArbeitszeit.Text = formattedArbeitszeit;
            }
            catch (Exception ex)
            {
                // Handle the exception if necessary
            }
        }
        private bool IsDateAlreadyExists(DateTime date)
        {
            try
            {
                string[] lines = File.ReadAllLines("arbeitszeiten.txt");
                foreach (string line in lines)
                {
                    string[] timeData = line.Split(';');
                    if (timeData.Length >= 2)
                    {
                        DateTime savedDate = DateTime.ParseExact(timeData[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        if (savedDate.Date == date.Date)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Überprüfen des Datums: {ex.Message}");
                return false; // Return false in case of an error
            }
        }
        private TimeSpan CalculateArbeitszeit(string arbeitsbeginnStr, string pausenbeginnStr, string pausenendeStr, string arbeitsendeStr)
        {
            TimeSpan arbeitsbeginn = TimeSpan.Parse(arbeitsbeginnStr);
            TimeSpan pausenbeginn = TimeSpan.Parse(pausenbeginnStr);
            TimeSpan pausenende = TimeSpan.Parse(pausenendeStr);
            TimeSpan arbeitsende = TimeSpan.Parse(arbeitsendeStr);

            TimeSpan pauseZeit = pausenende - pausenbeginn;
            TimeSpan arbeitszeit = (arbeitsende - arbeitsbeginn) - pauseZeit;

            return arbeitszeit;
        }
        private void BtnArbeitszeitAendern_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if any item is selected in the DataGrid
                if (dataGridMonatsdaten.SelectedItem is Arbeitstag selectedArbeitstag)
                {
                    // Remove the selected item from the arbeitszeitenList
                    arbeitszeitenList.Remove(selectedArbeitstag);

                    // Create a new Arbeitstag with the updated values from the input fields
                    Arbeitstag updatedArbeitstag = new Arbeitstag(selectedArbeitstag.Datum,
                                                                 TimeSpan.Parse(txtArbeitsbeginn.Text),
                                                                 TimeSpan.Parse(txtPausenbeginn.Text),
                                                                 TimeSpan.Parse(txtPausenende.Text),
                                                                 TimeSpan.Parse(txtArbeitsende.Text),
                                                                 CalculateArbeitszeit(txtArbeitsbeginn.Text,
                                                                                      txtPausenbeginn.Text,
                                                                                      txtPausenende.Text,
                                                                                      txtArbeitsende.Text));

                    // Add the updated item to the arbeitszeitenList
                    arbeitszeitenList.Add(updatedArbeitstag);

                    // Save the updated list to the file
                    SaveArbeitszeitenListToFile();

                    // Reload the DataGrid with the updated data for the selected month and year
                    LadeMonatsdaten(currentMonth, currentYear);

                    // Clear the input fields in the Arbeitszeit section after successful update
                    txtArbeitsbeginn.Text = "";
                    txtPausenbeginn.Text = "";
                    txtPausenende.Text = "";
                    txtArbeitsende.Text = "";
                    txtArbeitszeit.Text = "";

                    // Disable the "Ändern" and "Löschen" buttons after successful update
                    btnArbeitszeitAendern.IsEnabled = false;
                    btnArbeitszeitLoeschen.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Ändern der Arbeitszeit: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void UpdateArbeitstagInFile(Arbeitstag updatedArbeitstag)
        {
            try
            {
                // Read all lines from the "arbeitszeiten.txt" file
                string[] lines = File.ReadAllLines("arbeitszeiten.txt");

                // Find the line that corresponds to the selected date and update it with the modified data
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] timeData = lines[i].Split(';');
                    if (timeData.Length >= 2)
                    {
                        DateTime savedDate = DateTime.ParseExact(timeData[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        if (savedDate.Date == updatedArbeitstag.Datum.Date)
                        {
                            // Update the line with the modified data
                            lines[i] = $"{updatedArbeitstag.Datum:yyyy-MM-dd};{updatedArbeitstag.Arbeitsbeginn};{updatedArbeitstag.Pausenbeginn};{updatedArbeitstag.Pausenende};{updatedArbeitstag.Arbeitsende};{updatedArbeitstag.Arbeitszeit};{updatedArbeitstag.Arbeitszeit.Hours};{updatedArbeitstag.Arbeitszeit.Minutes}";
                            break;
                        }
                    }
                }

                // Write the updated lines back to the file
                File.WriteAllLines("arbeitszeiten.txt", lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Aktualisieren der Arbeitszeit: {ex.Message}");
            }
        }
        private void CalculateAndDisplayTotalArbeitszeit(int month, int year)
        {
            try
            {
                // Filter the arbeitszeitenList to get data for the selected month and year
                List<Arbeitstag> filteredData = arbeitszeitenList.Where(a => a.Datum.Month == month && a.Datum.Year == year).ToList();

                // Calculate the total Arbeitszeit for the selected month and year
                TimeSpan totalArbeitszeit = TimeSpan.Zero;
                foreach (Arbeitstag arbeitstag in filteredData)
                {
                    totalArbeitszeit += arbeitstag.Arbeitszeit;
                }

                // Display the total Arbeitszeit in hours and minutes
                int totalHours = (int)totalArbeitszeit.TotalHours;
                int totalMinutes = totalArbeitszeit.Minutes;

                // Update the corresponding text block with the calculated total Arbeitszeit
                txtArbeitszeitGesamt.Text = $"Arbeitszeit gesamt: {totalHours} Stunden, {totalMinutes} Minuten";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Berechnen der Gesamtarbeitszeit: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnVorherigerMonat_Click(object sender, RoutedEventArgs e)
        {
            dateOffset--;
            currentMonth--;
            if (currentMonth == 0)
            {
                currentMonth = 12;
                currentYear--;
            }
            LoadAndUpdateData(dateOffset);
        }
        private void BtnNachfolgenderMonat_Click(object sender, RoutedEventArgs e)
        {
            dateOffset++;
            currentMonth++;
            if (currentMonth == 13)
            {
                currentMonth = 1;
                currentYear++;
            }
            LoadAndUpdateData(dateOffset);
        }
        private void LoadAndUpdateData(int monthOffset)
        {
            DateTime currentDate = DateTime.Now.AddMonths(monthOffset);
            txtMonatJahr.Text = currentDate.ToString("MMMM yyyy");
            LadeMonatsdaten(currentDate.Month, currentDate.Year);
            CalculateAndDisplayTotalArbeitszeit(currentDate.Month, currentDate.Year);
        }
        private void LadeMonatsdaten(int month, int year)
        {
            // Filter the arbeitszeitenList to get data for the selected month and year
            List<Arbeitstag> filteredData = arbeitszeitenList.Where(a => a.Datum.Month == month && a.Datum.Year == year).ToList();

            // Sort the filtered data by date
            filteredData.Sort((a, b) => a.Datum.CompareTo(b.Datum));

            // Set the filtered and sorted data as the DataGrid's ItemsSource
            dataGridMonatsdaten.ItemsSource = filteredData;
        }
        private void DataGridMonatsdaten_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if any item is selected in the DataGrid
            if (dataGridMonatsdaten.SelectedItem is Arbeitstag selectedArbeitstag)
            {
                // Fill the input fields with the data of the selected item
                txtArbeitsbeginn.Text = selectedArbeitstag.Arbeitsbeginn.ToString();
                txtPausenbeginn.Text = selectedArbeitstag.Pausenbeginn.ToString();
                txtPausenende.Text = selectedArbeitstag.Pausenende.ToString();
                txtArbeitsende.Text = selectedArbeitstag.Arbeitsende.ToString();
                txtArbeitszeit.Text = selectedArbeitstag.Arbeitszeit.ToString();

                // Enable the "Ändern" and "Löschen" buttons since an item is selected
                btnArbeitszeitAendern.IsEnabled = true;
                btnArbeitszeitLoeschen.IsEnabled = true;
            }
            else
            {
                // Clear the input fields when no item is selected
                txtArbeitsbeginn.Text = "";
                txtPausenbeginn.Text = "";
                txtPausenende.Text = "";
                txtArbeitsende.Text = "";
                txtArbeitszeit.Text = "";

                // Disable the "Ändern" and "Löschen" buttons when no item is selected
                btnArbeitszeitAendern.IsEnabled = false;
                btnArbeitszeitLoeschen.IsEnabled = false;
            }
        }
        private void BtnArbeitszeitLoeschen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if any item is selected in the DataGrid
                if (dataGridMonatsdaten.SelectedItem is Arbeitstag selectedArbeitstag)
                {
                    // Remove the selected item from the arbeitszeitenList
                    arbeitszeitenList.Remove(selectedArbeitstag);

                    // Save the updated list to the file
                    SaveArbeitszeitenListToFile();

                    // Reload the DataGrid with the updated data for the selected month and year
                    LadeMonatsdaten(currentMonth, currentYear);

                    // Clear the input fields in the Arbeitszeit section after successful deletion
                    txtArbeitsbeginn.Text = "";
                    txtPausenbeginn.Text = "";
                    txtPausenende.Text = "";
                    txtArbeitsende.Text = "";
                    txtArbeitszeit.Text = "";

                    // Disable the "Ändern" and "Löschen" buttons after successful deletion
                    btnArbeitszeitAendern.IsEnabled = false;
                    btnArbeitszeitLoeschen.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Löschen der Arbeitszeit: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
public class Arbeitstag
{
    public DateTime Datum { get; }
    public TimeSpan Arbeitsbeginn { get; }
    public TimeSpan Pausenbeginn { get; }
    public TimeSpan Pausenende { get; }
    public TimeSpan Arbeitsende { get; }
    public TimeSpan Arbeitszeit { get; }
    public int ArbeitszeitHours { get; }
    public int ARbeitszeitMinutes { get; }

    public Arbeitstag(DateTime datum, TimeSpan arbeitsbeginn, TimeSpan pausenbeginn, TimeSpan pausenende, TimeSpan arbeitsende, TimeSpan arbeitszeit)
    {
        Datum = datum;
        Arbeitsbeginn = arbeitsbeginn;
        Pausenbeginn = pausenbeginn;
        Pausenende = pausenende;
        Arbeitsende = arbeitsende;
        Arbeitszeit = arbeitszeit;
    }
}