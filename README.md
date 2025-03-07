# UltimaOnlineScripts

A collection of utility scripts and systems for Ultima Online server administrators and developers.

## Overview

This repository contains various C# scripts designed to enhance and extend the functionality of Ultima Online servers. These scripts provide tools for server administrators and developers to generate lists of items and mobiles, as well as implement a language system for players.

## Scripts

### GenerateMobilesList.cs

A utility script that generates a list of all mobile creatures in the Server.Mobiles namespace that inherit from BaseCreature. The script:

- Registers a command `GenMobilesList` that can be used by GameMasters
- Filters out specific types of creatures (vendors, escorts, familiars, etc.)
- Outputs the list to a file named `MobilesList.txt`

#### Usage

1. Add the script to your server's custom scripts directory
2. Compile the server
3. In-game, use the command `[GenMobilesList]` as a GameMaster or higher

### GenerateItemsList.cs

A utility script that generates a list of all items in the Server.Items namespace. The script:

- Registers a command `GenItemsList` that can be used by Administrators
- Filters out specific types of items (addons, armor, weapons, tools)
- Outputs the list to a file named `ItemsList.txt`

#### Usage

1. Add the script to your server's custom scripts directory
2. Compile the server
3. In-game, use the command `[GenItemsList]` as an Administrator

### Language.cs

A comprehensive language system that allows players to learn and speak different languages. Other players who haven't learned a specific language will see the text as runes. The system includes:

- Multiple languages: Common, Ancient, Tribal, Pagan, and Glyph
- A learning system where players can gradually increase their understanding of languages
- Books that players can read to learn languages
- A command system to switch between languages

#### Installation

1. Add the code from the region marked `#region Apply To PlayerMobile.cs` to your server's PlayerMobile.cs file
2. Add the serialization and deserialization code to the appropriate methods in PlayerMobile.cs
3. Add the Language.cs file to your server's custom scripts directory
4. Compile the server

#### Usage

- Players can use the `[Speak]` command followed by the language name to switch languages
- Example: `[Speak Ancient]` to speak in the Ancient language
- Players need to have a 100% understanding of a language to speak it
- Players can increase their understanding by reading language books or by listening to others speak the language

## Requirements

- RunUO or ServUO server environment
- Basic knowledge of C# and server administration
- Access to the server's source code for installation

## Contributing

Contributions are welcome! If you have improvements or new scripts to add, please feel free to submit a pull request.

## License

This project is available for use under standard open-source terms. Please credit the original authors when using or modifying these scripts.