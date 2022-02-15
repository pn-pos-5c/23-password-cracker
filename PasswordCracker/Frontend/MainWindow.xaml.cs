using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Windows;

namespace Frontend
{
    public partial class MainWindow : Window
    {
        HubConnection hubConnection;

        public MainWindow()
        {
            InitializeComponent();

            hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5149/PasswordCracker")
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(10) })
                .Build();
            EstablishHubConnection();
        }

        private async void EstablishHubConnection()
        {
            try
            {
                await hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                lbl_hubConnection.Content = "Unable to connect to hub";
            }

            if (hubConnection.State != HubConnectionState.Connected) lbl_hubConnection.Content = "Unable to connect to hub";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string pwHash = txt_pwHash.Text.ToString();
            string alphabet = txt_alphabet.Text.ToString();
            int.TryParse(txt_length.Text.ToString(), out int length);

            hubConnection.On<double>("UpdateProgress", progress =>
            {
                Console.WriteLine(progress);
                progressBar.Value = progress * 100;
                lbl_result.Content = $"{Math.Round(progress * 100)} %";
            });

            lbl_hubConnection.Content = "";
            var result = await hubConnection.InvokeAsync<string>("CrackPassword", pwHash, alphabet, length);
            lbl_result.Content = result;

            hubConnection.Remove("UpdateProgress");
            progressBar.Value = 100;
        }
    }
}
