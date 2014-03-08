/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * This source code file is part of Knot3. Copying, redistribution and
 * use of the source code in this file in source and binary forms,
 * with or without modification, are permitted provided that the conditions
 * of the MIT license are met:
 *
 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 *
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 * 
 * See the LICENSE file for full license details of the Knot3 project.
 */

using System;
using System.Diagnostics.CodeAnalysis;

using Knot3.Framework.Input;

namespace Knot3.Game.Input
{
    [ExcludeFromCodeCoverageAttribute]
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
