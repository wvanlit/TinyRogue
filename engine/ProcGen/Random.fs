module TinyRogue.Engine.ProcGen.Random

open System

let rng = Random.Shared

let randomBool() = rng.Next(0, 2) = 1

let chance percentage = (rng.NextDouble() * 100.0) <= float percentage