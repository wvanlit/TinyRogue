module TinyRogue.Engine.ProcGen.BinarySpacePartition

open TinyRogue.Engine.ProcGen.Random
open TinyRogue.Engine.ProcGen.Room

type BspDirection =
    | Horizontal
    | Vertical
    | None

type BinarySpacePartition(minRoomSize: uint, width: uint, height: uint) =
    let BSP_BORDER = 4u
    let MIN_BSP_SIZE = minRoomSize + BSP_BORDER
    let MIN_SPLIT_SIZE = MIN_BSP_SIZE * 2u
    let RANDOM_STOP_CHANCE = 15
    let RANDOM_STOP_AREA = width * height

    let findSplitDirection (room: RectangularRoom) =
        match room.Height <= MIN_SPLIT_SIZE, room.Width <= MIN_SPLIT_SIZE with
        | _ when chance RANDOM_STOP_CHANCE && room.Area <= RANDOM_STOP_AREA -> None
        | true, true -> None
        | true, false -> Vertical
        | false, true -> Horizontal
        | false, false -> if chance 50 then Vertical else Horizontal

    let rec split room =
        match findSplitDirection room with
        | None -> room, room // End Condition
        | Horizontal ->
            let maxRoomSize = room.Height - MIN_BSP_SIZE
            let splitPoint = randomUnsigned MIN_BSP_SIZE maxRoomSize

            let roomA = RectangularRoom(room.X, room.Y, room.Width, splitPoint + 1u)

            let roomB =
                RectangularRoom(room.X, room.Y + uint splitPoint, room.Width, room.Height - splitPoint)

            roomA, roomB
        | Vertical ->
            let maxRoomSize = room.Width - MIN_BSP_SIZE
            let splitPoint = randomUnsigned MIN_BSP_SIZE maxRoomSize

            let roomA = RectangularRoom(room.X, room.Y, splitPoint + 1u, room.Height)

            let roomB =
                RectangularRoom(room.X + uint splitPoint, room.Y, room.Width - splitPoint, room.Height)

            roomA, roomB

    let rec splitUntilDone room =
        let a, b = split room

        if a = b then
            [ room ]
        else
            List.concat [ splitUntilDone a; splitUntilDone b ]
            
    let generateInnerRoom (room: RectangularRoom) =
        let randomWidth = max (randomUnsigned minRoomSize room.Width - BSP_BORDER) minRoomSize
        let randomHeight = max (randomUnsigned minRoomSize room.Height - BSP_BORDER) minRoomSize
        
        RectangularRoom(room.X, room.Y, randomWidth, randomHeight)
        

    member this.Generate() =
        let baseRooms = splitUntilDone (RectangularRoom(0u, 0u, width, height))
        
        if baseRooms.Length >= 3 then
            List.map generateInnerRoom baseRooms
        else
            this.Generate()