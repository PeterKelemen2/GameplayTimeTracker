using System.ComponentModel.DataAnnotations;

namespace GameplayTimeTracker.Models;

public class RemoteMachine : BaseDataModel
{
    [Display(Name = "Host OS")] public OsType HostOs { get; set; } = OsType.Windows;

    [Display(Name = "Host Name")] public string HostName { get; set; } = "localhost";

    [Display(Name = "Port")] public int Port { get; set; } = 22;

    [Display(Name = "Username")] public string User { get; set; } = "";

    [Display(Name = "Password")] public string Password { get; set; } = "";

    [Display(Name = "Save Folder")] public string SaveFolder { get; set; } = "";
}