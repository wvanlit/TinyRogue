module TinyRogue.Engine.ProcGen.Random

open System
open TinyRogue.Engine.Types

let rng = Random.Shared

let randomBool () = rng.Next(0, 2) = 1

let chance percentage =
    (rng.NextDouble() * 100.0) <= float percentage

let randomUnsigned (from: uint) (until: uint) =
    rng.Next(int from, int (until + 1u)) |> uint

let rec randomDirection () =
    let rx = rng.Next(-1, 2)
    let ry = rng.Next(-1, 2)
    // 0,0 is not a valid direction
    if rx = 0 && ry = 0 then randomDirection () else Position(rx, ry)

let randomItem<'a> (from: 'a list) = from[rng.Next(0, from.Length)]