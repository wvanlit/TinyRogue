module TinyRogue.Engine.Engine

open TinyRogue.Engine.Types
open TinyRogue.Engine.Actor
open TinyRogue.Engine.ProcGen.DungeonGeneration
open TinyRogue.Engine.ProcGen.Random


let CreateEngine () =
    let dungeon = createSimpleDungeon 48u 32u 6u

    let cx, cy = (randomItem dungeon.Rooms).Center
    let player = createPlayer (int cx) (int cy)

    let enemies =
        dungeon.Rooms
        |> List.map (fun r -> (randomUnsigned 0u 2u), r)
        |> List.map (fun (c, r) -> List.init (int c) (fun _ -> r.RandomPointInRoom()))
        |> List.concat
        |> List.indexed
        |> List.map (fun (idx, pos) ->
            { id = uint idx + 1u
              role = NPC
              position = pos })

    { Turn = 0u
      Actors = [ player ] @ enemies
      Dungeon = dungeon }

let replaceActorWith engine actor =
    { engine with
        Actors = List.filter (fun a -> a.id <> actor.id) engine.Actors @ [ actor ] }
