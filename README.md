# Valheim Plus Chat Filter
![image](https://user-images.githubusercontent.com/6819907/114248375-6c394e00-998f-11eb-8a46-42e213d629aa.png)

Chat Filter is a *server-side* only HarmonyX mod for Valheim. 

This mod filters profanities and censors them in the in game chat.

Everything is done server side, clients connecting to your server do not need to install this mod.

It censors the words for every one else in the server but the person who wrote the message will still see the profanities used.

## Default Profanity Filter

By default the chat filter has a premade list so you can use it out of the box with no configuration. The default list of profanities [can be seen in the library we used here](https://github.com/stephenhaunts/ProfanityDetector/blob/main/ProfanityFilter/ProfanityFilter/ProfanityList.cs).

## Dependencies

BepInEx - The mod is loaded into Valheim using BepInEx.

It will work on a vanilla server as long as you run BepInEx along side it. 

It *should* also work along side any other mods, including existing chat mods.

## Installation

Download the [latest release from here](https://github.com/valheimPlus/VPlusChatFilter/releases/latest). Extract the files, and place them into the following folder:

> C:\Program Files (x86)\Steam\steamapps\common\Valheim dedicated server\BepInEx\plugins

The above location will be different for you if you have Valheim installed elsewhere.

Your folder structure should then look like this:

* Valheim
  * BepInEx
    * plugins
      * VPlusChatFilter.dll
      * Filters

## To-Do

The only feature outstanding is to enable adding or removing words from the chat filter while the server is still live.

Currently to add or remove words from the Chat Filter you need to restart the server.


## Usage & Configuration

The plugin is controlled via a config file, which is generated when the mod loads for the first time and is located in

> C:\Program Files (x86)\Steam\steamapps\common\Valheim\BepInEx\config

The config file is called ```mixone.valheimplus.chatfilter```

### Config Options

You can change the file name for the custom add or remove filters using this section:
```ini
[Filtering]

# Filename where custom words to add are.
# Setting type: String
# Default value: customAdd.filter
CustomWordsFilename = customAdd.filter

# Filename where words to remove from filter are.
# Setting type: String
# Default value: customRemove.filter
RemoveWordsFilename = customRemove.filter
```
You can change the file name for the custom add or remove filters using this section:
```ini
[Filtering]

# Filename where custom words to add are.
# Setting type: String
# Default value: customAdd.filter
CustomWordsFilename = customAdd.filter

# Filename where words to remove from filter are.
# Setting type: String
# Default value: customRemove.filter
RemoveWordsFilename = customRemove.filter
```

You can enable or disable the custom add or remove filters using this section:

```ini
[Filtering.Toggles]

# Add your own custom words.
# Setting type: Boolean
# Default value: false
AddCustomWords = false

# Remove certain words from filters.
# Setting type: Boolean
# Default value: false
RemoveWords = false
```
You can set what symbol to replace censored words with, enable or disable the filter and also whether to wrap around words here:

```ini
[General]

# Symbol to censor words with.
# Setting type: String
# Default value: *
ReplaceKey = *

[General.Toggles]

# Whether or not to filter the chat.
# Setting type: Boolean
# Default value: true
EnableFilter = true

# Whether or not to wrap around words.
# Setting type: Boolean
# Default value: true
WrapWords = true
```

## Adding or Remove Custom Words

If you want to add or remove custom words from the default profanity filter. You can do the following:

* Make sure you have enabled the option in the config as seen in the above section.
* Edit the ```customAdd.filter``` or ```customRemove.filter```, the format needs to be as seen in the ```template.filter``` example file. each word needs to be separated by a comma and then a space.

```ini
# This file is a template to show how to add words
# Do NOT add anything else other than the words separated by ', '
# Remember to remove these three lines before using this file
word1, word2, word3
```


## Contributing
Pull requests are welcome. 

Please make sure to update tests as appropriate.

## Credits

[https://github.com/stephenhaunts/ProfanityDetector](https://github.com/stephenhaunts/ProfanityDetector) - Credit to Stephan Haunts for his Profanity Detector library which was used in this mod 
