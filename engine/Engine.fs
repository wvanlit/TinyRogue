module TinyRogue.Engine.Engine

open TinyRogue.Engine.Types
open TinyRogue.Engine.Actor
open TinyRogue.Engine.ProcGen.DungeonGeneration
open TinyRogue.Engine.ProcGen.Random
open TinyRogue.Engine.FOV


let CreateEngine () : TinyRogueEngine =
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

    let initialShadowMap =
        Array2D.init (dungeon.Walls.GetLength 0) (dungeon.Walls.GetLength 1) (fun _ _ -> Visibility.Unexplored)

    computeFov player.position (fun p -> dungeon.Walls[p.y, p.x]) (fun p -> initialShadowMap[p.y, p.x] <- Visible)

    { Turn = 0u
      Actors = [ player ] @ enemies
      Dungeon = dungeon
      ShadowMap = initialShadowMap }

let replaceActorWith engine actor =
    { engine with
        Actors = List.filter (fun a -> a.id <> actor.id) engine.Actors @ [ actor ] }
