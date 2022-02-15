using Backend.Services;
using Microsoft.AspNetCore.SignalR;

namespace Backend;

public class PasswordHub : Hub<IPasswordHub>
{

    public async Task<string> CrackPassword(string hash, string alphabet, int length)
    {
        var passwordService = new PasswordService();

        var timer = new System.Timers.Timer(1000);
        timer.Elapsed += (sender, args) => Clients.All.UpdateProgress(passwordService.CalculateProgress());
        timer.Start();

        string result;
        if (alphabet.Length == 0 || length <= 0) result = passwordService.CrackPassword2(hash).Result;
        else result = passwordService.CrackPassword(hash, alphabet, length).Result;
        timer.Dispose();
        return result;
    }
}
