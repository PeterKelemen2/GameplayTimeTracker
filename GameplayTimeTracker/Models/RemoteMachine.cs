using System.ComponentModel.DataAnnotations;

namespace GameplayTimeTracker.Models;

public class RemoteMachine : BaseDataModel
{
    public OsType HostOs { get; set; } = OsType.Windows;
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 22;
    public string User { get; set; } = "";
    public string Password { get; set; } = "";
    public string SaveFolder { get; set; } = "";
}