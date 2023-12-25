module TinyRogue.Engine.Engine

open TinyRogue.Engine.Types
open TinyRogue.Engine.Actor
open TinyRogue.Engine.ProcGen.DungeonGeneration
open TinyRogue.Engine.ProcGen.Random


let CreateEngine () =
    let dungeon = createSimpleDungeon 48u 32u 6u
    
    let cx, cy = (randomItem dungeon.Rooms).Center
    let player = createPlayer (int cx) (int cy)
    
    { Turn = 0u
      Actors = [ player ]
      Dungeon = dungeon }

let replaceActorWith engine actor =
    { engine with
        Actors = List.filter (fun a -> a.id <> actor.id) engine.Actors @ [ actor ] }
