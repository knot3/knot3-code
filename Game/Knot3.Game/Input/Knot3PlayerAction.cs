using System;
using Knot3.Framework.Input;

namespace Knot3.Game.Input
{
    public static class Knot3PlayerAction
    {
        public static readonly PlayerAction MoveUp = new PlayerAction ("Move Up");
        public static readonly PlayerAction MoveDown = new PlayerAction ("Move Down");
        public static readonly PlayerAction MoveLeft = new PlayerAction ("Move Left");
        public static readonly PlayerAction MoveRight = new PlayerAction ("Move Right");
        public static readonly PlayerAction MoveForward = new PlayerAction ("Move Forward");
        public static readonly PlayerAction MoveBackward = new PlayerAction ("Move Backward");
        public static readonly PlayerAction RotateUp = new PlayerAction ("Rotate Up");
        public static readonly PlayerAction RotateDown = new PlayerAction ("Rotate Down");
        public static readonly PlayerAction RotateLeft = new PlayerAction ("Rotate Left");
        public static readonly PlayerAction RotateRight = new PlayerAction ("Rotate Right");
        public static readonly PlayerAction ZoomIn = new PlayerAction ("Zoom In");
        public static readonly PlayerAction ZoomOut = new PlayerAction ("Zoom Out");
        public static readonly PlayerAction ResetCamera = new PlayerAction ("Reset Camera");
        public static readonly PlayerAction MoveToCenter = new PlayerAction ("Move Selection to Center");
        public static readonly PlayerAction ToggleMouseLock = new PlayerAction ("Toggle Mouse Lock");
        public static readonly PlayerAction AddToEdgeSelection = new PlayerAction ("Add to Selection");
        public static readonly PlayerAction AddRangeToEdgeSelection = new PlayerAction ("Add Range to Selection");
        public static readonly PlayerAction EdgeColoring = new PlayerAction ("Set Edge Color");
        public static readonly PlayerAction EdgeRectangles = new PlayerAction ("Set Edge Rectangles");
    }
}

