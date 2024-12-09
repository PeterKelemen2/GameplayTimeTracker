# Gameplay Time Tracker Application

**Gameplay Time Tracker** is a desktop application developed in `C#` using the `WPF` framework. It allows users to monitor and track the total time spent on specific applications or games by observing their executables.
Monitored applications can be launched from the interface as well.
<p align="center">
	<img alt="Screenshot" src="https://www.kepfeltoltes.eu/images/2024/12/07/917main.png" width="500"/>
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
    -  `bigBgImages`
        - Default value: `false`
        - When set to `true`, the background blurred image of the icon is wider

	
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
- **Menus**
  - Preferences
    - All the previously mentioned settings can be configured here.  
  - Themes
    - Clicking the buttons allows the user to quickly switch the colors of the corresponding element.
    - Theme updates dynamically as the user changes the colors.
	  
<p align="center">
	<img alt="Screenshot" src="https://i.imgur.com/9k5Gp2a.png" width="240" style="margin-right: 20px;"/>
	<img alt="Screenshot" src="https://i.imgur.com/3uTIKNE.png" width="240"/>
</p>

  If a theme or its colors are missing from the configuration file, the application will regenerate default themes to ensure proper functionality.
  Each theme contains the following customizable color properties:

| Property       | Description                                  |Example Value     |
|----------------|----------------------------------------------|------------------|
| `bgColor`      | Background color of the application.         | `"#1E2030"`      | 
| `tileColor1`   | First gradient color of a tile.              | `"#414769"`      | 
| `tileColor2`   | Second gradient color of a tile.             | `"#2E324A"`      | 
| `leftColor`    | Left-side gradient color of a progress bar.  | `"#89ACF2"`      | 
| `rightColor`   | Right-side gradient color of a progress bar. | `"#B7BDF8"`      |
| `editColor1`   | First gradient color of and edit dropdown.   | `"#7DD6EB"`      | 
| `editColor2`   | Second highlight color of and edit dropdown. | `"#7DD6EB"`      |
| `shadowColor`  | Shadow color under a tile.                   | `"#151515"`      | 
| `fontColor`    | Text color.                                  | `"#DAE4FF"`      | 
| `runningColor` | Highlight text color for running apps.       | `"#C3E88D"`      |
| `footerColor`  | Footer bar color.                            | `"#90EE90"`      |

## Data
Each monitored application is tracked with the following properties:
<p align="center">
	<img alt="Editing using the Custom theme" src="https://i.imgur.com/Z4856Su.png" width="500"/>
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

### Editing
All properties except `lastPlayedTime` can be modified via the application's edit menu.


## License

This project is licensed under the [GNU General Public License v3.0](https://www.gnu.org/licenses/gpl-3.0.html).

You are free to use, modify, and distribute this software under the terms of the GPL. See the [LICENSE](./LICENSE) file for full details.


