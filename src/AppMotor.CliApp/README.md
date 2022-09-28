---
icon: terminal
---
# APIs for Command Line Apps

This library contains various building blocks for command line applications:

* [**TermText**](Terminals/README.TermText.md): a `string` that supports colors and some formatting (like: underline).
* **`Terminal`**: An alternative to `System.Console` that only supports cross-platform APIs (and thus make writing Console/Terminal code less error-prone). It also provides direct support for `TermText` (or any "raw" string ANSI escape sequences) by making sure they automatically work on Windows.
* [**CLI Application Framework**](README.CliAppFramework.md): Types to make hosting/running CLI applications easier, including proper exception handling, command line parsing and Generic Host support.

## Code Documentation

This library has full code documentation. You can find more details about each type there.
