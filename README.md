# Gameplay Time Tracker Application

**Gameplay Time Tracker** is a desktop application developed in `C#` using the `WPF` framework. It allows users to
monitor and track the total time spent on specific applications or games by observing their executables.
Monitored applications can be launched from the interface as well.

## Data Update Process (Version 1.3.2 and Later)

In version `1.3.1`, a new method for storing elapsed time was introduced, along with a button to manually
convert legacy data. In version `1.3.2`, this process has been automated. Upon the first launch after
updating to version `1.3.2`, the user will be prompted to update from the previous data format.

If the update has already been performed, you can restore the most recent **backup** from the
Settings menu. Alternatively, you may set the `dataNeedsUpdating` flag to `false` in the settings file.

To safeguard against data loss, the application automatically creates a backup of the current data before
initiating the update.


<p align="center">
	<img alt="Screenshot" src="https://i.imgur.com/1jRT6Vo.png" width="500"/>
</p>

## Configuration

The configuration file will be generated in `%USERPROFILE%\Documents\Gameplay Time Tracker\settings.json`
There are two main parts that can be set by the user both on the user interface and in the configuration file.

- **Preferences**
    - `startWithSystem` ,
        - Default value: `true`
        - When set to `true`, the application will automatically start with the system by creating a shortcut in the
          startup directory.
    - `horizontalTileGradient` ,
        - Default value: `true`
        - When set to `true`, the gradient brush of the Tile entries will be in a horizontal orientation.
        - When set to `false`, the orientation will be vertical.
    - `horizontalEditGradient` ,
        - Default value: `true`
        - When set to `true`, the gradient brush of the Tile's edit menu will be in a horizontal orientation.
        - When set to `false`, the orientation will be vertical.
    - `bigBgImages`
        - Default value: `false`
        - When set to `true`, the background blurred image of the icon is wider
    - `dataNeedsUpdating`
        - Default value: `true`
        - When set to `true`, the app will prompt the user to update legacy time data


- **Themes**
    - `selectedTheme`
        - Default value: `Default`
        - Specifies the name of the theme to load on startup.
    - `themeList`
        - A list of themes, each containing a unique name and a set of colors for different UI components.
        - By default, three themes are included:
            - **Default**: A dark-themed design.
            - **Pink**: A vibrant pink-themed design.
            - **Custom**: A placeholder for user-defined customization.
- **Menus**
    - Preferences
        - All the previously mentioned settings can be configured here.
        - A backup can be created, that will be stored in `%USERPROFILE%\Documents\Gameplay Time Tracker\Backup Data\`
        - Restoring backup happens by selecting a previously saved backup.
    - Themes
        - Clicking the buttons allows the user to quickly switch the colors of the corresponding element.
        - Theme updates dynamically as the user changes the colors.

<p align="center">
	<img alt="Screenshot" src="https://i.imgur.com/a2678sj.png" width="240" style="margin-right: 20px;"/>
	<img alt="Screenshot" src="https://i.imgur.com/nnGBVMk.png" width="240"/>
</p>

If a theme or its colors are missing from the configuration file, the application will regenerate default themes to
ensure proper functionality.

Each theme contains the following customizable color properties:

Note: Only the `Edit` and `Remove` buttons are using the specified color values at the moment.

| Property              | Description                                  | Example Value |
|-----------------------|----------------------------------------------|---------------|
| `bgColor`             | Background color of the application.         | `"#1E2030"`   | 
| `tileColor1`          | First gradient color of a tile.              | `"#414769"`   | 
| `tileColor2`          | Second gradient color of a tile.             | `"#2E324A"`   | 
| `leftColor`           | Left-side gradient color of a progress bar.  | `"#89ACF2"`   | 
| `rightColor`          | Right-side gradient color of a progress bar. | `"#B7BDF8"`   |
| `editColor1`          | First gradient color of and edit dropdown.   | `"#7DD6EB"`   | 
| `editColor2`          | Second highlight color of and edit dropdown. | `"#7DD6EB"`   |
| `shadowColor`         | Shadow color under a tile.                   | `"#151515"`   | 
| `fontColor`           | Text color.                                  | `"#DAE4FF"`   | 
| `runningColor`        | Highlight text color for running apps.       | `"#C3E88D"`   |
| `footerColor`         | Footer bar color.                            | `"#90EE90"`   |
| `button`              | Normal button color.                         | `"#FF3BC9E3"` |
| `buttonHover`         | Normal button hover color.                   | `"#FFADD8E6"` |
| `positiveButton`      | Positive button color.                       | `"#FF90EE90"` |
| `positiveButtonHover` | Positive button hover color.                 | `"#FFB5FFB5"` |
| `negativeButton`      | Negative button color.                       | `"#FFED0C0C"` |
| `negativeButtonHover` | Negative button hover color.                 | `"#FFE33B3B"` |

## Data

Each monitored application is tracked with the following properties:
<p align="center">
	<img alt="Editing using the Custom theme" src="https://i.imgur.com/E12tyFx.png" width="500"/>
</p>

- `gameName`
    - String representation of the application's name.
    - This is initially gathered from the _File Description_ of the executable, if available.
- `totalTime`
    - An integer representing the total time the application has been open, measured in minutes.
- `lastPlayedTime`
    - An integer representing the duration of the most recent session, measured in minutes.
    - This value is automatically updated and cannot be edited manually.
- `iconPath`
    - A string specifying the path to a `.png` icon extracted from the application's executable.
    - If no icon is extracted or the path is invalid, a fallback image (`assets\no_icon.png`) is used.
    - Icon images are stored in `%USERPROFILE%\Documents\Gameplay Time Tracker\Saved Icons\`.
- `exePath`
    - A string specifying the full path to the application's executable file.
    - The Launch button will only be enabled if the path exists or the file is an executable.
- `arguments`
    - A string specifying launch parameters for the executable. Arguments are extracted from shortcut
      files upon addition.

### Editing

- All properties except `lastPlayedTime` and `dataNeedsUpdating` can be modified via the application's edit menu.
- Clicking the `Open Folder` button, the user can open the folder containing the executable file previously chosen.

## License

This project is licensed under the [GNU General Public License v3.0](https://www.gnu.org/licenses/gpl-3.0.html).

You are free to use, modify, and distribute this software under the terms of the GPL. See the [LICENSE](./LICENSE) file
for full details.


