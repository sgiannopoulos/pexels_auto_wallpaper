// See https://aka.ms/new-console-template for more information

using System.Net;
using PexelsDotNetSDK.Api;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using PexelsDotNetSDK.Models;

// DLL Import to set the wallpaper
[DllImport("user32.dll", CharSet = CharSet.Auto)]
static extern Int32 SystemParametersInfo(UInt32 action, UInt32 uParam, string vParam, UInt32 winIni);


// Settings and Configuration
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
string ApiKey = config["API_KEY"];
int pageSizeInput = int.Parse(config["PageSize"]);

var generator = new Random();
int selection = generator.Next(0, pageSizeInput - 1);
int page = 1;

if (selection > 80)
{
    page = selection / 80 + 1;
    selection = selection % 80;
}


string searchQuery = config["SearchQuery"];

Console.WriteLine("Updating Wallpaper");

var pexelsClient = new PexelsClient(ApiKey);

var result = await pexelsClient.SearchPhotosAsync(searchQuery, page:page, pageSize:pageSizeInput);

if (result.photos.Any())
{
    using (WebClient client = new WebClient())
    {
        Photo wallpaper = result.photos.ToList()[selection];
        
        string url = wallpaper.source.original;
        string photoName = wallpaper.alt;
        string fileName = @"C:\Users\Sotos\Pictures\Pexels\" + photoName + '.' + url.Split('.').Last();
        
        client.DownloadFile(new Uri(url), fileName);
        SetWallpaper(fileName);
    }
}


void SetWallpaper(string path)
{
    SystemParametersInfo(0x14, 0, path, 0x01 | 0x02);
}