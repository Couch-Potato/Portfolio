#Novovu File System
The Novovu File System is used for file storage within Novovu project files.

## Xenon Asset File


**FileStream**

FileStream of the asset

**Name**

The Name of the Asset

**ID**

The ID of the asset to be added to the global asset store 
ex. 0x1231212131

**Hash**
The hash of the stream.


## Xenon Asset Storage

**Assets**
Dictionary of all assets sorted by their ID
```CSharp
XenonAssetFile file = XenonAssetStorage.Assets["0x0123123123"];
```

**[M] LoadAsset**
Loads an asset into the XenonAssetStorage
```CSharp
string Asset = XenonAssetStorage.LoadAsset(new FileInfo("file.txt"), useImporter: false);
```

**[M] CreateStorageFile**
Stores all assets into a .nva file.

## Xenon Level File

**[M] Save**
Serializes a level and saves it to a .nvl file. Will serialize the asset store if requested. 

## Xenon Game File

**[M] Compile**
A compiled game .nvg file that can be run by the player.

## Xenon Material File

**[M] Save**
Saves a serialzed material file to a .nvm file.

## Xenon Object File

**[M] Save**
Saves a serialized object to a .nvo file.

## Xenon Component File

**[M] Save**
Saves a serialized object to a .nvc file.


## Xenon Project File

**[M] Save**
Save a game, and all of its assets to a .nvp file.


## Xenon Global Storage File
Storage for the entire game. This is only made at compile time.


#Files
**.nva** Novovu Asset

**.nvg** Novovu Game Compiled

**.nvc** Novovu Component

**.nvo** Novovu Object

**.nvm** Novovu Material

**.nvl** Novovu Level File

**.nvp** Novovu Project File

#File Structure

-.nva
--manifest.xml
--asset.a

-.nvl
--manifest.xml
--assets
---asset.nva

-.nvp
--manifest.xml
---levels
----level.nvl

-.nvc
--manifest.xml
--scripts
---script.nva

-.nvo
--manifest.xml
--scripts
---script.nva
--assets
---asset.nva
--materials
---material.nva

-.nvg
--game.manifest
--assets
---asset.nva
--levels
---level_1.manifest



