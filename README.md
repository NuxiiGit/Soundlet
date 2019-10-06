# Media Player Playlist Manager

This repository contains the source code for `mpc-pls`; a command-line tool which can be used to automatically build and edit Media Player playlist files.

## Features

The following commands can be used by typing `mpc-pls <command> [args]`.
 
 - `build` for building a new playlist from a directory of files.
 - `print` for listing the contents of a playlist file.
 - `shuffle` for shuffling a playlist file.
 - `update` for adding or removing files from a playlist file.

For more information on a specific command use `mpc-pls help <command>`.

### Supported Playlist Formats

The tool currently supports the following playlist formats:

 - `.asx`
 - `.m3u` and `.m3u8`
 - `.pls`
 - `.mpcpl`

### Why the Name?

I named it `mpc-pls` because originally it only supported the `.mpcpl` (Media Player *Classic* PlayList) format. I've expanded the available range to the ones Media Player Classic natively exports to.

## Downloads

You can download pre-built executables from the available [releases](https://github.com/NuxiiGit/mpc-playlist-manager/releases). (`mpc-pls.zip`)

### Installing

The zip file contains everything you should need, so there is no installation needed.

You can simply call the tool using `mpc-pls` in the windows command line. Add the exe to your environment `PATH` for easy use.

## Building

If you want to build the project yourself, you will need:

 - Visual Studio
 - Visual Basic .NET
 - Id3.NET v0.6.0