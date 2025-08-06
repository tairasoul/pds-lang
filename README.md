# pds-lang

A custom language for PneumaticCraft: Repressurized's drone programming.

Highly unfinished (no abstractions besides a function abstraction) & incredibly prone to weird syntax / user error.

Also likely inefficient or badly made seeing as I have never made a custom language before.

## Config

The possible config options (pdsl.json) are:
```json5
{
  "sourceDir": "./src", // The root directory all .pdsl files are in. Subdirectories are currently not supported.
  "outDir": "./dist", // The output directory.
  "allowOverwrites": false // Whether or not to allow overwriting lua-side parsers
}
```

## Lua-side processors

pds-lang uses MoonSharp to make more dynamic processors for different pieces, and additional processors can be added in the "lua" folder within the same directory as pdsl.json

All files are loaded into the same lua script, and files that return nothing are skipped after being run, allowing for utility functions (ex. lua/lib.lua)

Return structures are as follows:

### Lua file

The lua file itself must return the following:
```lua
return {
  identifier = "ident", -- The name of the piece when being called in a .pdsl file.
  processor = func, -- The processor called to create a piece.
  arguments = {
    -- The arguments for the processor (if any).
    {
      name = "name", -- The name of the argument (will be used if I make a language server for this)
      types = { "string" }, -- The valid types for this argument (has a few weird quirks with the lua env setup)
      required = false, -- Whether or not this argument is required (optional, has to be put after all required arguments)
      validate = function(n) -- Function to validate argument, returns a bool.
        return not string.match(n, "invalid")
      end
    }
  },
  validOutsideArguments = true -- Is this piece valid outside of the arguments for another piece? Optional, defaults to true.
}
```

### Lua processor

The processor has to return the following:
```lua
return { -- List of widgets
  {
    name = "pneumaticcraft:text", -- Widget name
    x = x, -- x position, should in most cases just be the same as the x position passed in
    y = y, -- y position, should in most cases just be the same as the y position passed in
    newX = x, -- the x position for the next piece, should in most cases just be the same as the x position passed in
    newY = y + 11, -- the y position for the next piece, should be y + piece_height
    width = 15, -- width, unused internally but might be useful for other pieces
    height = 11, -- height, unused internally but might be useful for other pieces
    -- Piece-specific properties
    string = "hello!"
  }
}
```
Their arguments are `x`, `y`, and the arguments defined in the file's `return`.

## Quirks

The base `area` implementation only accounts for the `box` area type, and assumes it to be filled.

For argument types, while you can use something like `string[]`, there are two quirks for this:

  1. You cannot do `string[][]` (the lua validator has to create the type and I have not accounted for this yet)

  2. Arrays containing more than one type will default back to `object[]` and will not be turned into something like `(string | number)[]`

## Additional notes

Pieces which jump to labels (like conditions) can use blocks in place of label names, as they will automatically be turned into valid labels.

These labels are formatted as "Block(BlockNum)Label", and only the label name gets passed in.

## Example code

Comments within the code are turned into actual comment pieces within pneumaticcraft.
```
start()

# compiler test!

function a {
  for_each_coordinate([area([20 20 20] [50 50 50] "box")] "coord" {
    dig([area("coord" "coord" "box")])
  })
  suicide()
}

a()
```

## Missing Pieces

Currently missing pieces are:

`Item Assignment`, `Coordinate Operator` and `Crafting`