# Gameplay Time Tracker Application

**Gameplay Time Tracker** is a desktop application developed in `C#` using the `WPF` framework. It allows users to
monitor and track the total time spent on specific applications or games by observing their executables.
Monitored applications can be launched from the interface as well. Each game's save files can be backed up to a remote
machine, saves will be retained for a specified number of days.

## Data Update Process (Version 1.3.2 and Later)

In version `1.3.1`, a new method for storing elapsed time was introduced, along with a button to manually
convert legacy data. In version `1.3.2`, this process has been automated. Upon the first launch after
updating to version `1.3.2`, the user will be prompted to update from the previous data format.

If legacy data conversion is required, it is strongly recommended to complete this process using a build prior to
version `2.0.0`, as this functionality has been removed in the rewritten version.


<p align="center">
	<img alt="Screenshot" src="https://imgur.com/b0lroBC.png" width="500"/>
</p>

## Application Configuration

The configuration file will be generated in `%USERPROFILE%\Documents\Gameplay Time Tracker\settings.json`
There are two main parts that can be set by the user both on the user interface and in the configuration file.

- **Preferences**
    - `Start With System`
        - Default value: `true`
        - When enabled (`true`), the application will automatically start upon system boot by creating a shortcut in
          the system's startup directory.
    - `Prefer SteamGridDB Images`
        - Default value: `true`
        - When enabled (`true`), the application will attempt to load Icon and Hero images from
          [SteamGridDB](https://www.steamgriddb.com/)
        - When disabled (`false`), the images will be loaded directly from the executable of the selected application.
    - `SteamGridDB API key`
        - Default value: `empty`
        - When provided, this key enables the integration with [SteamGridDB](https://www.steamgriddb.com/), allowing the
          application to retrieve assets from the SteamGridDB service.
    - `Quick Add`
        - Default value: `false`
        - When disabled (`false`), the application will prompt the user with a basic configuration menu after loading
          an executable.
        - When enabled (`true`), the executable will be added with default values without prompting for configuration.
    - `Performance Mode`
        - Default value: `false`
        - When enabled (`true`), the application will perform reduced animations to improve performance, particularly
          useful for lower-end systems.
    - `Backup On Exit`
        - Default value: `false`
        - When enabled (`true`), the application will create a backup of the currently loaded data file under
          `%USERPROFILE%\Documents\Gameplay Time Tracker\Backup Data\` upon exiting.
    - `Display`
        - Default value: `Horizontal`
        - Specifies the layout of the application.
        - Currently available options: `Horizontal`,`Vertical`, `Compact`
    - `Save Frequency`
        - Default value: `1`
        - Specifies the interval, in minutes, at which the application will save data to the file.
    <p align="center">
        <img alt="Screenshot" src="https://imgur.com/qwEaPfG.png" width="350"/>   
    </p>

- **Themes**
    - Users can specify the color scheme of the application.
    - Each color component of the theme can be individually configured through the user interface.
    - Default themes: `Dark`, `Pink`, `Custom`.
    - If a theme is deleted from the settings file, it will be automatically recreated using the default color values.
    - Currently available colors:

    | Property          | Description                                  | Example Value |
    |-------------------|----------------------------------------------|---------------|
    | `Background`      | Background color of the application.         | `"#1E2030"`   | 
    | `Card 1`          | First gradient color of a card.              | `"#414769"`   | 
    | `Card 2`          | Second gradient color of a card.             | `"#2E324A"`   | 
    | `Progress Bar 1`  | Left-side gradient color of a progress bar.  | `"#89ACF2"`   | 
    | `Progress Bar 2`  | Right-side gradient color of a progress bar. | `"#B7BDF8"`   |
    | `Font`            | Text color.                                  | `"#DAE4FF"`   | 
    | `Running`         | Highlight text color for running apps.       | `"#C3E88D"`   |
    | `Footer`          | Footer bar color.                            | `"#FF3377A6"` |
    | `Footer Font`     | Footer bar font color.                       | `"#FFDAE4FF"` |
    | `Button`          | Normal button color.                         | `"#FF3BC9E3"` |
    | `Positive Button` | Positive button color.                       | `"#FF90EE90"` |
    | `Negative Button` | Negative button color.                       | `"#FFED0C0C"` |
    | `Shadow`          | Currently not in use.                        | `"#FF151515"` | 
    | `Transparent`     | Currently not in use.                        | `"#00000000"` | 

<p align="center">
    <img alt="Screenshot" src="https://imgur.com/vKxtCzd.png" width="350"/>
</p>

- **Backup**
    - Users can create and load backup data files.
    - When selecting a backup file, its contents will be displayed to assist in identifying and choosing the correct
      file.
    <p align="center">
        <img alt="Screenshot" src="https://imgur.com/OLDWa8J.png" width="350"/>
    </p>

- **Remote Machine**
    - Users have the option to enable remote saving functionality.
    - To configure a remote machine, the `Address`, `Port`, `User` and `Password` must be provided.
    - A remote folder must also be specified to allow saves to be uploaded to the remote machine.
    - After configuring the remote machine, users can test the connection by using the provided test button.
    <p align="center">
        <img alt="Screenshot" src="https://imgur.com/Oo0ecNr.png" width="350"/>
    </p>

## Data Configuration

Each monitored application is tracked with the following attributes:

<p align="center">
	<img alt="Editing Menu" src="https://imgur.com/iyZ431H.png" width="500"/>
</p>

- **Name**
    - The name of the application as a string.
    - Initially, this value is derived from the `File Description` of the executable, if available.
- **Total Playtime**
    - An integer array representing the total duration the application has been open.
    - To ensure compatibility, the time format from previous versions, which used double precision, will be
      automatically converted.
- **Last Playtime**
    - An integer array representing the duration of the most recent application session.
    - This value is automatically updated and cannot be edited manually.
- **Path**
    - A string specifying the full path to the application's executable file.
    - The Launch button will be enabled only if the specified path exists or the file is a valid an executable.
- **Arguments**
    - A string that contains any launch parameters for the executable.
    - Arguments are automatically extracted from shortcut files during the application's addition.
- **Icon Path** and **Hero Path**
    - Strings specifying the paths to `.png` files representing the application’s icon and hero image.
    - Icons are either extracted from the application's executable or retrieved from `SteamGridDB`.
    - If no valid icon is found or the path is invalid, a default image (`Assets/DefaultIcon.png`) will be used as a
      fallback.
    - If retrieving assets from `SteamGridDB` fails, or if `Prefer SteamGridDB Images` is disabled, a Hero image will be
      generated from the Icon image.
    - All images are stored in `%USERPROFILE%\Documents\Gameplay Time Tracker\Images\`.
    - Refresh assets using the following options:
      - `Full SGDB`: Reloads all assets from `SteamGridDB`
      - `Hero Local`: Generates a hero image from the current icon.
      - `Icon Local`: Extracts the icon from the executable at the specified path.
- **Statistics Graph**
    - By pressing the `Show Stats` button, a graph displaying the playtime data of the last 7 days will be shown.
    <p align="center">
        <img alt="Stats graph" src="https://imgur.com/XQmGRW4.png" width="400"/>
    </p>

### Remote Backup Configuration

- Each loaded application can be individually configure for creating backups.
- **Local Save Path**
    - Specifies the directory where the application's save files are stored.
- **Save if session longer**
    - Defines the minimum session duration (in minutes) required for a backup to be created.
    - A threshold of 5 minutes is applied to this calculation.
- **Retentions Period**
    - Specifies the number of days save files will be retained before being deleted.
- **Backup Method: `Manual`** or **`Automatic`**
    - Enabling the `Remote Backup` option allows automatic backups when a session ends.
    - Manual backups can be performed at any time, regardless of the session duration.
- **Restoring Saves**
    - Save files can be retrieved from the remote machine by selecting an available backup entry.

## License

This project is licensed under the [GNU General Public License v3.0](https://www.gnu.org/licenses/gpl-3.0.html).

You are free to use, modify, and distribute this software under the terms of the GPL. See the [LICENSE](./LICENSE) file
for full details.

