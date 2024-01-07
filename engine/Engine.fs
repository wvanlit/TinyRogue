module TinyRogue.Engine.Engine

open TinyRogue.Engine.Types
open TinyRogue.Engine.Actor
open TinyRogue.Engine.ProcGen.DungeonGeneration
open TinyRogue.Engine.ProcGen.Random
open TinyRogue.Engine.FOV

let calculateFieldOfView dungeon pos =
    let initialShadowMap =
        Array2D.init (dungeon.Walls.GetLength 0) (dungeon.Walls.GetLength 1) (fun _ _ -> Visibility.Unexplored)

    computeFov pos (fun p -> dungeon.Walls[p.y, p.x]) (fun p -> initialShadowMap[p.y, p.x] <- Visible)

    initialShadowMap

let updateFieldOfView engine =
    let newShadowMap =
        calculateFieldOfView engine.Dungeon (List.find isPlayer engine.Actors).position

    for y in 0 .. newShadowMap.GetLength(0) - 1 do
        for x in 0 .. newShadowMap.GetLength(1) - 1 do
            if engine.ShadowMap[y, x] <> Unexplored && newShadowMap[y, x] = Unexplored then
                newShadowMap[y, x] <- Explored

    { engine with ShadowMap = newShadowMap }

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

    { Turn = 0u
      Actors = [ player ] @ enemies
      Dungeon = dungeon
      ShadowMap = calculateFieldOfView dungeon player.position }



let replaceActorWith engine actor =
    { engine with
        Actors = List.filter (fun a -> a.id <> actor.id) engine.Actors @ [ actor ] }

let removeActor id engine =
    { engine with
        Actors = List.filter (fun a -> a.id <> id) engine.Actors }
