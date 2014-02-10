using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Microsoft.Xna.Framework;

using Knot3.Core;
using Knot3.GameObjects;
using Knot3.KnotData;
using Knot3.RenderEffects;

namespace Knot3.Node_Tests
{
	[TestFixture]
	public class Node_Tests
	{
		private Node node1;
		private Node node2;

		public Node_Tests()
		{
		}

		[SetUp]
		public void Initialize()
		{
			node1 = new Node(1, 1, 1);
			node2 = new Node(1, 1, 1);
		}

		[Test]
		public void Node_Vector_Tests()
		{
			// Assert.AreEqual(node1.Vector, ); // ???
		}

		[Test]
		public void Node_Operator_Tests()
		{
			Vector3 result;

			result = node1 - node2;
			Assert.AreEqual(result, Vector3.Zero);

			result = node1 + node2;
			//Assert.AreEqual(result, ); // ???

			Assert.IsTrue(node1 == node2);
			Assert.IsFalse(node1 != node2);
		}

		[Test]
		public void Node_Equals_Tests()
		{
			Assert.IsTrue(node1.Equals(node1));
			Assert.IsTrue(node1.Equals(node2));
			Assert.IsFalse(node1.Equals(null));
			Assert.IsTrue(Node.Equals(node1, node2));
		}

		[Test]
		public void Node_CenterBetween_Tests()
		{
		}

		[Test]
		public void Node_Clone_Tests()
		{
			Object clonedNode = node1.Clone();
			Assert.IsTrue(clonedNode.Equals(node1));
		}

		[Test]
		public void Node_ToString_Tests()
		{
			Assert.AreEqual(node1.ToString(), "(1,1,1)");
		}
	}
}
