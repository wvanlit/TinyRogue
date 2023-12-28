module TinyRogue.Engine.FOV

open TinyRogue.Engine.Types

// Calculate FOV using recursive shadow casting
// Based on: https://www.albertford.com/shadowcasting

module private ShadowCasting =
    type Cardinal =
        | North
        | East
        | South
        | West

    let cardinalDirections = [ North; East; South; West ]

    type RelativePosition = int * int

    type Quadrant(cardinal: Cardinal, origin: Position) =
        let ox = origin.x
        let oy = origin.y

        /// Converts a relative position into a grid position
        member _.transform(tile: RelativePosition) =
            let row, col = tile

            match cardinal with
            | North -> pos (ox + col) (oy - row)
            | South -> pos (ox + col) (oy + row)
            | East -> pos (ox + row) (oy + col)
            | West -> pos (ox - row) (oy + col)

    type Row(depth: int, startSlope: float, endSlope: float) =
        let roundTiesUp n = (n + 0.5) |> floor |> int
        let roundTiesDown n = (n + 0.5) |> ceil |> int

        member _.depth = depth
        member val startSlope = startSlope with get, set
        member val endSlope = endSlope with get, set

        member _.tiles: RelativePosition list =
            let minCol = (float depth * startSlope) |> roundTiesUp
            let maxCol = (float depth * endSlope) |> roundTiesDown
            List.init (maxCol - minCol) (fun c -> (depth, c + minCol))

        member _.next = Row(depth + 1, startSlope, endSlope)

    let slope (tile: int * int) =
        let rowDepth, col = tile
        (2.0 * float col - 1.0) / (2.0 * float rowDepth)

    let isSymmetric (row: Row) (tile: RelativePosition) =
        let _, col = tile

        float col >= ((float row.depth) * row.startSlope)
        && float col <= ((float row.depth) * row.endSlope)

open ShadowCasting

let computeFov (origin: Position) (isBlocking: Position -> bool) (markVisible: Position -> unit) =
    markVisible origin

    for direction in cardinalDirections do
        let quadrant = Quadrant(direction, origin)

        let reveal tile = quadrant.transform tile |> markVisible
        let isWall tile = quadrant.transform tile |> isBlocking
        let isFloor = isWall >> not

        let rec scan (row: Row) =
            let prevTile = None

            for tile in row.tiles do
                if isWall tile || isSymmetric row tile then
                    reveal tile

                if prevTile.IsSome && isWall prevTile.Value && isFloor tile then
                    row.startSlope <- slope tile

                if prevTile.IsSome && isFloor prevTile.Value && isWall tile then
                    let nextRow = row.next
                    nextRow.endSlope <- slope tile
                    scan row

        let firstRow = Row(1, -1.0, 1.0)
        scan firstRow
