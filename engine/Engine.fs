module TinyRogue.Engine.Engine

open TinyRogue.Engine.Types
open TinyRogue.Engine.ProcGen.DungeonGeneration
open TinyRogue.Engine.ProcGen.Random
open TinyRogue.Engine.FOV
open TinyRogue.Engine.Types.Entities
open TinyRogue.Engine.Types.Primitives

let calculateFieldOfView dungeon pos =
    let initialShadowMap =
        Array2D.init (dungeon.Walls.GetLength 0) (dungeon.Walls.GetLength 1) (fun _ _ -> Visibility.Unexplored)

    computeFov pos (fun p -> dungeon.Walls[p.y, p.x]) (fun p -> initialShadowMap[p.y, p.x] <- Visible)

    initialShadowMap

let updateFieldOfView engine =
    let pos = (List.find Actor.IsPlayer engine.Actors).position
    let newShadowMap = calculateFieldOfView engine.Dungeon pos

    for y in 0 .. newShadowMap.GetLength(0) - 1 do
        for x in 0 .. newShadowMap.GetLength(1) - 1 do
            if engine.ShadowMap[y, x] <> Unexplored && newShadowMap[y, x] = Unexplored then
                newShadowMap[y, x] <- Explored

    { engine with ShadowMap = newShadowMap }

let CreateEngine () : GameEngine =
    let dungeon = createSimpleDungeon 48u 32u 6u

    let cx, cy = (randomItem dungeon.Rooms).Center
    let player = Actor.Player(Position(cx, cy))

    let enemies =
        dungeon.Rooms
        |> List.map (fun r -> (randomUnsigned 0u 2u), r)
        |> List.map (fun (c, r) -> List.init (int c) (fun _ -> r.RandomPointInRoom()))
        |> List.concat
        |> List.indexed
        |> List.map (fun (idx, pos) -> Actor.Npc (uint idx + 1u) pos)

    { Turn = 0u
      Actors = [ player ] @ enemies
      Dungeon = dungeon
      ShadowMap = calculateFieldOfView dungeon player.position }



let replaceActorWith engine (actor: Actor) =
    { engine with
        Actors = List.filter (fun a -> a.id <> actor.id) engine.Actors @ [ actor ] }

let removeActor id engine =
    { engine with
        Actors = List.filter (fun a -> a.id <> id) engine.Actors }
