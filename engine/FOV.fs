module TinyRogue.Engine.FOV

open TinyRogue.Engine.Types.Primitives

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
            | North -> Position((ox + col), (oy - row))
            | South -> Position((ox + col), (oy + row))
            | East -> Position((ox + row), (oy + col))
            | West -> Position((ox - row), (oy + col))

    type Row(_depth: int, _startSlope: float, _endSlope: float) =
        let roundTiesUp n = (n + 0.51) |> floor |> int
        let roundTiesDown n = (n - 0.51) |> ceil |> int

        member _.depth = _depth
        member val startSlope = _startSlope with get, set
        member val endSlope = _endSlope with get, set

        member r.tiles: RelativePosition list =
            let minCol = (float r.depth * r.startSlope) |> roundTiesUp
            let maxCol = (float r.depth * r.endSlope) |> roundTiesDown
            List.init (maxCol + 1 - minCol) (fun c -> (_depth, c + minCol))

        member r.next = Row(r.depth + 1, r.startSlope, r.endSlope)

    let slope (tile: int * int) =
        let rowDepth, col = tile
        let fCol = float col
        let fDepth = float rowDepth
        (2.0 * fCol - 1.0) / (2.0 * fDepth)

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
            let mutable prevTile = None

            for tile in row.tiles do
                if isWall tile || isSymmetric row tile then
                    reveal tile

                if prevTile.IsSome && isWall prevTile.Value && isFloor tile then
                    row.startSlope <- slope tile

                if prevTile.IsSome && isFloor prevTile.Value && isWall tile then
                    let nextRow = row.next
                    nextRow.endSlope <- slope tile
                    scan nextRow

                prevTile <- Some tile

            if prevTile.IsSome && isFloor prevTile.Value then
                scan row.next

        let firstRow = Row(1, -1.0, 1.0)
        scan firstRow
