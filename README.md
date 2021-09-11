# Line Counter

This is a small command-line utility to count lines in a file. That's it. That's all it does. Technically, it doesn't even care if it's text. 

This is the .NET 6.0 + AVX2 version of my [other .NET line counter](https://github.com/hyrmn/nlc).

There are some counting assumptions that I made. I had originally chosen to have this match my editor's line count. That is, if Visual Studio Code shows `x` lines then my logic would also show `x` lines. However, I've chosen to follow the behavior of `wc -l`. I count carriage returns (`\n`). If a file does not end with a carriage return then the last line will not be counted.

## How to Build

Clone this repository and then run `dotnet build` from the solution root.

```posh
> dotnet build -c Release --nologo
```

This will create a release build of the utility at `/bin/Release/net6.0/nlc.exe`. From there, you will need to copy it to a place in your `%PATH%`.

## How to Use

Unlike `nlc`, `fnlc` can only parse files.

To read a file:

```
> fnlc "path/to/your/file.txt"
```

The only output from `fnlc` will be the line count, or the error (errors are not written to STDERR)

So the full run might look like 

```
> fnlc "path/to/your/file.txt"
109
```

## Special Thanks

I'd like to thank Ben Dornis of [linksfor.dev(s)](https://linksfor.dev/) fame for cracking the mystery of how to get a Vector256 into a Popcount. This approach wasn't worth it until we got that code.