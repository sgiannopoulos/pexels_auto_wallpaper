// See https://aka.ms/new-console-template for more information

using System.Net;
using PexelsDotNetSDK.Api;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

// DLL Import to set the wallpaper
[DllImport("user32.dll", CharSet = CharSet.Auto)]
static extern Int32 SystemParametersInfo(UInt32 action, UInt32 uParam, string vParam, UInt32 winIni);


// Settings and Configuration
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
string ApiKey = config["API_KEY"];
int pageSizeInput = int.Parse(config["PageSize"]);

var pexelsClient = new PexelsClient(ApiKey);

var result = await pexelsClient.SearchPhotosAsync("background",pageSize:pageSizeInput);

if (result.photos.Any())
{
    //SetWallpaper(result.photos.First().url);
    using (WebClient client = new WebClient())
    {
        string url = result.photos.First().source.original;
        string photoName = result.photos.First().alt;
        string fileName = @"C:\Users\Sotos\Pictures\Pexels\" + photoName + '.' + url.Split('.').Last();
        
        client.DownloadFile(new Uri(url), fileName);
        
        SetWallpaper(fileName);
    }
}


void SetWallpaper(string path)
{
    SystemParametersInfo(0x14, 0, path, 0x01 | 0x02);
}