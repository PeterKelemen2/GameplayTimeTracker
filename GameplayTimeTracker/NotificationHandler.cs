using System;
using System.ComponentModel;
using System.Windows;

namespace GameplayTimeTracker;

public class NotificationHandler
{
    // private System.Windows.Forms.NotifyIcon m_notifyIcon;
    private const string? AppIcon = "assets/MyAppIcon.ico";
    // System.Windows.Window mainWindow = Application.Current.MainWindow;
    public System.Windows.Forms.NotifyIcon m_notifyIcon { get; set; }

    public NotificationHandler()
    {
        // SetupNotifyIcon();
        InitializeNotifyIcon();
    }
    
    // Sets up initial values for notification icons
    public void InitializeNotifyIcon()
    {
        m_notifyIcon = new System.Windows.Forms.NotifyIcon();
        m_notifyIcon.BalloonTipText = "The app has been minimized. Click the tray icon to show.";
        m_notifyIcon.BalloonTipTitle = "Gameplay Time Tracker";
        m_notifyIcon.Text = "Gameplay Time Tracker";
        m_notifyIcon.Icon =
            new System.Drawing.Icon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppIcon));
        m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);
    }
    
    // Deletes notification from the memory when closing
    public void OnCloseNotify(object sender, CancelEventArgs args)
    {
        // Only dispose if you are exiting
        if (!args.Cancel)
        {
            m_notifyIcon.Dispose();
            m_notifyIcon = null;
        }
    }

    private WindowState m_storedWindowState = WindowState.Normal;
    
    public void OnStateChanged(object sender, EventArgs args)
    {
        if (Utils.mainWindow.WindowState == WindowState.Minimized)
        {
            Utils.mainWindow.Hide();
            if (m_notifyIcon != null)
                m_notifyIcon.ShowBalloonTip(2000);
        }
        else
            m_storedWindowState = Utils.mainWindow.WindowState;
    }

    // Shows notification icon only when app is minimized
    public void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
        if (m_notifyIcon != null)
            m_notifyIcon.Visible = !Utils.mainWindow.IsVisible;
    }

    void m_notifyIcon_Click(object sender, EventArgs e)
    {
        Utils.mainWindow.Show();
        Utils.mainWindow.WindowState = m_storedWindowState;
    }

    void ShowTrayIcon(bool show)
    {
        if (m_notifyIcon != null)
            m_notifyIcon.Visible = show;
    }
}