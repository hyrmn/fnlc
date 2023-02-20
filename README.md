# Line Counter

This is a small command-line utility to count lines in a file. That's it. That's all it does. Technically, it doesn't even care if it's text. 

This is the .NET 6.0 + AVX2 version of my [other .NET line counter](https://github.com/hyrmn/nlc).

There are some counting assumptions that I made. I had originally chosen to have this match my editor's line count. That is, if Visual Studio Code shows `x` lines then my logic would also show `x` lines. However, I've chosen to follow the behavior of `wc -l`. I count carriage returns (`\n`). If a file does not end with a carriage return then the last line will not be counted.

## How to Build

Clone this repository and then run `dotnet build` from the solution root.

```posh
> dotnet build -c Release --nologo
```

This will create a release build of the utility at `/bin/Release/net6.0/fnlc.exe`. From there, you will need to copy it to a place in your `%PATH%`.

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

## A Note for Visual Studio Users

At this stage of the .NET 6.0 release cycle (this was built with RC2), it is safe to say that the easiest approach is to use VS 2022. VS2019 will most likely work but, as of VS 2019 Version 16.11.2, you're going to hit friction around how global usings are brought along.

## Special Thanks

I'd like to thank [@buildstarted](https://github.com/buildstarted) of [linksfor.dev(s)](https://linksfor.dev/) fame for cracking the mystery of how to get a Vector256 into a Popcount. This approach wasn't worth it until we got that code.

And I'd like to thank [Adam Sitnik](https://github.com/adamsitnik) for his help and pointers on cleaning the code up and squeezing more performance out of this code. He educated me on the Native Memory additions and, as you can see on https://github.com/hyrmn/fnlc/pull/1, that really cleans up the approach.
