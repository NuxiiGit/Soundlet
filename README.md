# Media Player Playlist Manager

This repository contains the source code for `mpc-pls`; a command-line tool which can be used to automatically build and edit Media Player playlist files.

### Why the Name?

I named it `mpc-pls` because originally it only supported the `.mpcpl` (Media Player *Classic* PlayList) format. I've expanded the available range to the ones Media Player Classic natively exports to.

### Supported Playlist Formats

The tool currently supports `.asx`, `.m3u` and `.m3u8`, `.pls`, and `.mpcpl` playlist formats.

## Basic Behaviour

The tool features a a single expressive command. Simply call:

```
mpc-pls <source> <destination> [<commands>]
```

Where `source` is the file location of an existing playlist file, or the directory containing your media files; `destination` is the final location you want to save your new playlist file to; and `commands` are a list of optional commands, beginning with a `-`.

For a list of commands which can be used with this tool, you can simply call `mpc-pls` on it's own to display the landing page.

### Example

Let's say you want to create a playlist of all *Jazz* songs in your music folder. First, navigate to where your `Music` folder is located, and then you would type the following:

```
mpc-pls ./Music jazz.pls -genre Jazz
```

So long as you have correctly captioned your music with "Jazz" in the genre tag, all your files will be compiled into a playlist called `jazz.pls`.

## Advanced Behaviour

Additionally, some commands accept *modifiers* which can change their behaviour. Currently, the only example of commands which accept modifiers are: `-genre`, `-artist`, `-album`, and `-year`; all of which accept the following modifiers:
 
 - `all` only accepts elements which satisfy all the command arguments.
 - `any` accepts any elements which satisfy one or more command arguments.
 - `either` only accepts elements which satisfy exactly one command argument.
 - `only` only accepts singleton elements which satisfy any of the command arguments.

### Example

Let's say you want to build a playlist of all songs which are `Heavy Metal` or `Ambient`, but you only want songs which are made by `Slipknot` with no other contributers/remixes. You would use the following command:

```
mpc-pls ./Music metalbent.pls -genre "Heavy Metal" "Ambient" -artist:only "Slipknot"
```

The `:only` tag on the `-artist` command modifies the behaviour such that only songs which are purely made by Slipknot are selected for the playlist, hence it will exclude all your Dubstep/Trance remixes of *People = Shit*.

## Getting Started

### Downloads

You can download pre-built executables from the available [releases](https://github.com/NuxiiGit/mpc-playlist-manager/releases). (`mpc-pls.zip`)

### Installing

The zip file contains everything you should need, so there is no installation needed.

You can simply call the tool using `mpc-pls` in the windows command line. Add the exe to your environment `PATH` for easy use.

### Building

If you want to build the project yourself, you will need:

 - Visual Studio
 - Visual Basic .NET
 - Id3.NET v0.6.0
