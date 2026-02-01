using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.ViewModels;

public class RemoteMachineViewModel : RemoteMachine
{
    public string OsTypeLabel => "OS Type";
    public string HostNameLabel => "Host Name";
    public string PortLabel => "Port";
    public string UserLabel => "User";
    public string PasswordLabel => "Password";
    public string SaveFolderLabel => "Save Folder";
}