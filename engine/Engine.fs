module TinyRogue.Engine.Engine

open TinyRogue.Engine.Actor
open TinyRogue.Engine.Map

type TinyRogueEngine =
    { Turn: uint
      Actors: Actor list
      Dungeon: Dungeon }

let CreateEngine () =
    { Turn = 0u
      Actors = [ createPlayer 3 5 ]
      Dungeon = createSimpleDungeon 16 16 }

let replaceActorWith engine actor =
    { engine with
        Actors = List.filter (fun a -> a.id <> actor.id) engine.Actors @ [ actor ] }
