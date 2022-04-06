using OdrabiamyD;

OdrabiamyDownloader od = new OdrabiamyDownloader("https://odrabiamy.pl/api/v2/sessions");
od.DownloadStatus += (message) =>
{
    Console.WriteLine(message);
};
CancellationTokenSource csource = new CancellationTokenSource();
var ctoken = csource.Token;
string token = "";
Console.WriteLine("Login:");
var login = Console.ReadLine() ?? string.Empty;
Console.WriteLine("Hasło:");
var password = Console.ReadLine() ?? string.Empty;
try
{
    Console.WriteLine("Uzyskiwanie tokenu...");
    token = await od.GetTokenAsync(login, password) ?? throw new Exception("Gówno");
    Console.WriteLine("Token uzyskany!");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return;
}
Console.WriteLine("Pobieranie Premium? (y/n)");
if(Console.ReadKey(true).Key == ConsoleKey.Y)
    await Premium(token);
else
    await NonPremium();

async Task NonPremium()
{
    od.ChangeHeaders(Headers.NonPremium);
    Console.WriteLine("ID Cionszki:");
    int bookid = int.Parse(Console.ReadLine() ?? "0");
    Console.WriteLine("Strona startowa:");
    int firstpage = int.Parse(Console.ReadLine() ?? "0");
    Console.WriteLine("Ostatnia strona:");
    int lastpage = int.Parse(Console.ReadLine() ?? "0");
    var cancelTask = Task.Run(() =>
    {
        while (Console.ReadKey(true).Key != ConsoleKey.C) { }
        csource.Cancel();
    });
    try
    {
        Console.WriteLine("Pobieranie rozpoczęte...\nwciśnij 'c' aby anulować");
        var book = await od.DownloadBookAsync(firstpage, lastpage, bookid, ctoken);
        await od.SaveBookAsHTMLAsync(book, $"Cionszka-{bookid}");
        Console.WriteLine("Koniec!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Coś poszło nie tak");
        Console.WriteLine(ex.Message);
    }
}
async Task Premium(string token)
{
    od.ChangeHeaders(Headers.Premium, token);
    Console.WriteLine("ID Cionszki:");
    int bookid = int.Parse(Console.ReadLine() ?? "0");
    Console.WriteLine("Strona startowa:");
    int firstpage = int.Parse(Console.ReadLine() ?? "0");
    Console.WriteLine("Ostatnia strona:");
    int lastpage = int.Parse(Console.ReadLine() ?? "0");
    var cancelTask = Task.Run(() =>
    {
        while (Console.ReadKey(true).Key != ConsoleKey.C) { }
        csource.Cancel();
    });
    try
    {
        Console.WriteLine("Pobieranie rozpoczęte...\nwciśnij 'c' aby anulować");
        var book = await od.DownloadBookPremiumAsync(firstpage, lastpage, bookid, ctoken);
        await od.SaveBookAsHTMLAsync(book, $"Cionszka-{bookid}");
        Console.WriteLine("Koniec!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Coś poszło nie tak");
        Console.WriteLine(ex.Message);
    }
}
