# Media Player Playlist Manager

This repository contains the source code for `mpc-pls`; a command-line tool which can be used to automatically build and edit Media Player playlist files.

## Features

The tool features a a single expressive command. Simply call:

```
mpc-pls <source> <destination> [<commands>]
```

Where `source` is the file location of an existing playlist file, or the directory containing your media files; `destination` is the final location you want to save your new playlist file to; and `commands` are a list of optional commands, beginning with a `-`.

### Supported Playlist Formats

The tool currently supports `.asx`, `.m3u` and `.m3u8`, `.pls`, and `.mpcpl` playlist formats.

### Why the Name?

I named it `mpc-pls` because originally it only supported the `.mpcpl` (Media Player *Classic* PlayList) format. I've expanded the available range to the ones Media Player Classic natively exports to.

## Getting Started

### Downloads

You can download pre-built executables from the available [releases](https://github.com/NuxiiGit/mpc-playlist-manager/releases). (`mpc-pls.zip`)

### Installing

The zip file contains everything you should need, so there is no installation needed.

You can simply call the tool using `mpc-pls` in the windows command line. Add the exe to your environment `PATH` for easy use.

## Building

If you want to build the project yourself, you will need:

 - Visual Studio
 - Visual Basic .NET
 - Id3.NET v0.6.0