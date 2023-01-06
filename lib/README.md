# Unofficial Native Libraries

This directory contains unofficial builds of the native libraries supported by **Sharpie**. All libraries are built using GitHub workflows.

> These libraries are provided AS IS with NO WARRANTY whatsoever from my side.

## NCurses
NCurses libraries are built for Linux, MacOs and Windows using [this workflow](https://github.com/pavkam/sharpie/blob/main/.github/workflows/ncurses-build-and-pack.yml).
You can check it out to see the used compilation settings.
The original source repository is located [here](https://github.com/mirror/ncurses/)

### License
```
Copyright 2018-2021,2022 Thomas E. Dickey
Copyright 1998-2017,2018 Free Software Foundation, Inc.

Permission is hereby granted, free of charge, to any person obtaining a
copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, distribute with modifications, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE ABOVE COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR
THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Except as contained in this notice, the name(s) of the above copyright
holders shall not be used in advertising or otherwise to promote the
sale, use or other dealings in this Software without prior written
authorization.
```

## PDCurses
NCurses libraries are built for Linux, MacOs and Windows using [this workflow](https://github.com/pavkam/sharpie/blob/main/.github/workflows/pd-curses-build-and-pack.yml).
The original source repository is located [here](https://github.com/wmcbrine/PDCurses)

### License
```
The core package and most ports are in the public domain, but a few files 
in the demos and X11 directories are subject to copyright under licenses described there.
This software is provided AS IS with NO WARRANTY whatsoever.
```

## PDCursesMod
PDCursesMod libraries are built for Linux, MacOs and Windows using [this workflow](https://github.com/pavkam/sharpie/blob/main/.github/workflows/pd-curses-mod-build-and-pack.yml).

> The `libpdcursesmod-vt-arm64.dylib` library has been built on a Mac M1 locally.

The original source repository is located [here](https://github.com/Bill-Gray/PDCursesMod)

### License
```
The core package is in the public domain, but small portions of PDCursesMod 
are subject to copyright under various licenses. Each directory contains a 
README file, with a section titled "Distribution Status" which describes 
the status of the files in that directory.

If you use PDCursesMod in an application, an acknowledgement would be appreciated, 
but is not mandatory. If you make corrections or enhancements to PDCursesMod, 
please forward them to the current maintainer for the benefit of other users.

This software is provided AS IS with NO WARRANTY whatsoever.
```
