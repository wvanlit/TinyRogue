module TinyRogue.Engine.Utils

let unless condition func input = if condition then input else func input
