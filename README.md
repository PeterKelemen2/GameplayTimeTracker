# Gameplay Time Tracker Application

**Gameplay Time Tracker** is a desktop application developed in `C#` using the `WPF` framework. It allows users to monitor and track the total time spent on specific applications or games by observing their executables.
Monitored applications can be launched from the interface as well.
<p align="center">
	<img alt="Screenshot" src="https://www.kepfeltoltes.eu/images/2024/12/05/284app_screenshot2.png" width="400"/>
</p>

## Configuration
The configuration file will be generated in `%USERPROFILE%\Documents\Gameplay Time Tracker\settings.json`
There are two main parts that can be set by the user both on the user interface and in the configuration file.

- **Preferences**
	- `startWithSystem` ,
		- Default value: `true`
		- When set to `true`, the application will automatically start with the system by creating a shortcut in the startup directory.
	- `horizontalTileGradient` ,
		- Default value: `true`
		- When set to `true`, the gradient brush of the Tile entries will be in a horizontal orientation.
        - When set to `false`, the orientation will be vertical.
	- `horizontalEditGradient` ,
		- Default value: `true`
		- When set to `true`, the gradient brush of the Tile's edit menu will be in a horizontal orientation.
		- When set to `false`, the orientation will be vertical.

		<p align="left">
			<img alt="Screenshot" src="https://www.kepfeltoltes.eu/images/2024/12/07/574prefs.png" height="250"/>
		</p>
	
- **Themes**
	- `selectedTheme`
		- Default value: `Default`
		- Specifies the name of the theme to load on startup.
	- `themeList`
		- A list of themes, each containing a unique name and a set of colors for different UI components.
		- By default, three themes are included:
			-   **Default**: A dark-themed design.
			-   **Pink**: A vibrant pink-themed design.
			-   **Custom**: A placeholder for user-defined customization.

 	<p align="left">
  		<img alt="Screenshot" src="https://www.kepfeltoltes.eu/images/2024/12/07/959themes.png" height="250"/>
  	</p>

  If a theme or its colors are missing from the configuration file, the application will regenerate default themes to ensure proper functionality.
  Each theme contains the following customizable color properties:

| Property       |Description                                     |Example Value     |
|----------------|------------------------------------------------|------------------|
| `bgColor`      | Background color of the application.           | `"#1E2030"`      | 
| `footerColor`  | Footer bar color.                              | `"#90EE90"`      | 
| `darkColor`    | Darker accents used in the UI.                 | `"#1E2030"`      | 
| `lightColor`   | Lighter accents used in the UI.                | `"#2E324A"`      | 
| `fontColor`    | Text color.                                    | `"#DAE4FF"`      | 
| `runningColor` | Highlight text color for running apps.         | `"#C3E88D"`      | 
| `leftColor`    | Left-side element color.                       | `"#89ACF2"`      | 
| `rightColor`   | Right-side element color.                      | `"#B7BDF8"`      | 
| `tileColor1`   | First gradient color of a tile.                | `"#414769"`      | 
| `tileColor2`   | Second gradient color of a tile.               | `"#2E324A"`      | 
| `shadowColor`  | Shadow color under a tile.                     | `"#151515"`      | 
| `editColor1`   | First gradient color of and edit dropdown.     | `"#7DD6EB"`      | 
| `editColor2`   | Second highlight color of and edit dropdown.   | `"#7DD6EB"`      |

## Data

Each monitored application is tracked with the following properties:
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

### Editing
All properties except `lastPlayedTime` can be modified via the application's edit menu.

## License

This project is licensed under the [GNU General Public License v3.0](https://www.gnu.org/licenses/gpl-3.0.html).

You are free to use, modify, and distribute this software under the terms of the GPL. See the [LICENSE](./LICENSE) file for full details.


