using EmbeddedAILibrary;
using System.Text.Json;
using System.Windows;

namespace WpfAIApp;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        SubmitButton.IsEnabled = false;
        SubmitButton.Content = "Processing...";

        try
        {
            AIGenerator generator = new(@"D:\models\Llama-3.2-3B-Instruct-Q4_K_M.gguf");

            using JsonDocument jsonDocument = JsonDocument.Parse(SampleStructure.Text);
            int numberOfRecords = int.Parse(NumberOfRecords.Text);
            var result = await generator.GetSampleDataAsync(numberOfRecords, jsonDocument);

            JsonSerializerOptions options = new() { WriteIndented = true };
            string? output = JsonSerializer.Serialize(result, options);

            Results.Text = output;
        }
        finally
        {
            SubmitButton.IsEnabled = true;
            SubmitButton.Content = "Submit";
        }
    }
}