# pds-lang

A custom language for PneumaticCraft: Repressurized's drone programming.

Highly unfinished (no abstractions besides a function abstraction) & incredibly prone to weird syntax / user error.

Also likely inefficient or badly made seeing as I have never made a custom language before.

Please do not use this if you're not willing to look through lexer/lexer.cs to figure out how to call varying PNC:R functions, and all 3000 lines of processor/visitors/externs.cs to figure out the arguments.

## Config

The possible config options (pdsl.json) are:
```json5
{
  "sourceDir": "./src", // The root directory all .pdsl files are in. Subdirectories are currently not supported.
  "outDir": "./dist" // The output directory.
}
```

## Example code

Comments within the code are turned into actual comment pieces within pneumaticcraft.
```
start()

# compiler test!

function a {
  suicide()
}

a()
```