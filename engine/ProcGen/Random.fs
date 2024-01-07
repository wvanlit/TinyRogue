module TinyRogue.Engine.ProcGen.Random

open System

let rng = Random.Shared

let randomBool () = rng.Next(0, 2) = 1

let chance percentage =
    (rng.NextDouble() * 100.0) <= float percentage

let randomUnsigned (from: uint) (until: uint) =
    rng.Next(int from, int (until + 1u)) |> uint

let randomItem<'a> (from: 'a list) = from[rng.Next(0, from.Length)]