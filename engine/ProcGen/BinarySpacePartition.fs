module TinyRogue.Engine.ProcGen.BinarySpacePartition

open System
open TinyRogue.Engine.ProcGen.Random

type BspRoom =
    { x: uint
      y: uint
      width: uint
      height: uint }

type BspDirection =
    | Horizontal
    | Vertical
    | None

let MIN_ROOM_SIZE = 12
let MIN_SPLIT_SIZE = uint (MIN_ROOM_SIZE * 2)

let area room = room.width * room.height

let findSplitDirection (room: BspRoom) =
    if room.height <= MIN_SPLIT_SIZE && room.width <= MIN_SPLIT_SIZE then
        None
    elif chance 25 && area room <= 700u then
        None
    elif room.height <= MIN_SPLIT_SIZE then
        Vertical
    elif room.width <= MIN_SPLIT_SIZE then
        Horizontal
    elif room.height > MIN_SPLIT_SIZE && room.width > MIN_SPLIT_SIZE then
        if chance 50 then Vertical else Horizontal
    else
        failwith "Invalid state for Split Direction"

let split (room: BspRoom) : BspRoom * BspRoom =
    let dir = findSplitDirection room
    printfn $"Splitting {room.width} {room.height}: {dir}"

    if dir = None then
        room, room
    else
        let splitPoint =
            uint
            <| Random.Shared.Next(
                MIN_ROOM_SIZE,
                ((if dir = Horizontal then room.height else room.width) |> int) - MIN_ROOM_SIZE
            )

        let a: BspRoom =
            { x = room.x
              y = room.y
              width = if dir = Horizontal then room.width else splitPoint + 1u
              height = if dir = Vertical then room.height else splitPoint + 1u }

        let b: BspRoom =
            { x =
                if dir = Horizontal then
                    room.x
                else
                    room.x + uint splitPoint
              y = if dir = Vertical then room.y else room.y + uint splitPoint
              width =
                if dir = Horizontal then
                    room.width
                else
                    room.width - splitPoint
              height =
                if dir = Vertical then
                    room.height
                else
                    room.height - splitPoint }

        a, b

let rec splitUntilDone (room: BspRoom) : BspRoom list =
    let a, b = split room

    if a = b then
        [ room ]
    else
        List.concat [ splitUntilDone a; splitUntilDone b ]

let generateBsp (width: uint) (height: uint) : BspRoom list =
    let full =
        { x = 0u
          y = 0u
          width = width
          height = height }

    splitUntilDone full
