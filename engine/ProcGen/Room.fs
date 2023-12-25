module TinyRogue.Engine.ProcGen.Room

open TinyRogue.Engine.Types
open TinyRogue.Engine.ProcGen.Random

type RectangularRoom(x: uint, y: uint, width: uint, height: uint) =
    inherit Room(x, y)

    member val Width = width with get
    member val Height = height with get

    override this.Area = this.Width * this.Height

    override this.Center = (x + (this.Width / 2u) + 1u, y + (this.Height / 2u) + 1u)

    member this.RandomPointInRoom ()=
        let rx = randomUnsigned 1u (this.Width - 1u)
        let ry = randomUnsigned 1u (this.Height - 1u)
        pos (int (x + rx)) (int (y + ry))

    member this.Offset xOff yOff =
        RectangularRoom(this.X + xOff, this.Y + yOff, width, height)
