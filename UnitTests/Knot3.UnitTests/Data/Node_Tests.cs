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

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Knot3.Game.Data;

namespace Knot3.UnitTests.Data
{
    [TestFixture]
    public class Node_Tests
    {
        private Node node1;
        private Node node2;

        public Node_Tests ()
        {
        }

        [SetUp]
        public void Initialize ()
        {
            node1 = new Node (1, 1, 1);
            node2 = new Node (1, 1, 1);
        }

        [Test]
        public void Node_Vector_Tests ()
        {
            // Assert.AreEqual (node1.Vector, ); // ???
        }

        [Test]
        public void Node_Operator_Tests ()
        {
            Vector3 result;

            result = node1 - node2;
            Assert.AreEqual (result, Vector3.Zero);

            result = node1 + node2;
            //Assert.AreEqual (result, ); // ???

            Assert.IsTrue (node1 == node2);
            Assert.IsFalse (node1 != node2);
        }

        [Test]
        public void Node_Equals_Tests ()
        {
            Assert.IsTrue (node1.Equals (node1));
            Assert.IsTrue (node1.Equals (node2));
            Assert.IsFalse (Node.Equals (node1, null));
            Assert.IsTrue (Node.Equals (node1, node2));
        }

        [Test]
        public void Node_CenterBetween_Tests ()
        {
        }

        [Test]
        public void Node_Clone_Tests ()
        {
            Object clonedNode = node1.Clone ();
            Assert.IsTrue (clonedNode.Equals (node1));
        }

        [Test]
        public void Node_ToString_Tests ()
        {
            Assert.AreEqual (node1.ToString (), "(1,1,1)");
        }
    }
}
