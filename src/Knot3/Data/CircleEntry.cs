#region Copyright

/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Core;
using Knot3.Input;
using Knot3.GameObjects;
using Knot3.Screens;
using Knot3.RenderEffects;
using Knot3.Widgets;

namespace Knot3.Data
{
	/// <summary>
	/// Eine doppelt verkettete Liste.
	/// </summary>
	public class CircleEntry<T> : IEnumerable<T>, ICollection<T>, IList<T>
	{
		public T Value { get; set; }

		public CircleEntry<T> Next { get; set; }

		public CircleEntry<T> Previous { get; set; }

		public CircleEntry (T value)
		{
			Value = value;
			Previous = this;
			Next = this;
		}

		private CircleEntry ()
		{
			Previous = this;
			Next = this;
		}

		public CircleEntry (IEnumerable<T> list)
		{
			bool first = true;
			CircleEntry<T> inserted = this;
			foreach (T obj in list) {
				if (first) {
					Value = obj;
					Previous = this;
					Next = this;
				}
				else {
					inserted = inserted.InsertAfter (obj);
				}
				first = false;
			}
		}

		public static CircleEntry<T> Empty
		{
			get {
				return new CircleEntry<T> ();
			}
		}

		public CircleEntry<T> InsertBefore (T obj)
		{
			CircleEntry<T> insert = new CircleEntry<T> (obj);
			insert.Previous = this.Previous;
			insert.Next = this;
			this.Previous.Next = insert;
			this.Previous = insert;
			return insert;
		}

		public CircleEntry<T> InsertAfter (T obj)
		{
			//Log.Debug (this, ".InsertAfter (", obj, ")");
			CircleEntry<T> insert = new CircleEntry<T> (obj);
			insert.Next = this.Next;
			insert.Previous = this;
			this.Next.Previous = insert;
			this.Next = insert;
			return insert;
		}

		public void Remove ()
		{
			Previous.Next = Next;
			Next.Previous = Previous;
			Previous = null;
			Next = null;
		}

		public int Count
		{
			get {
				CircleEntry<T> current = this;
				int count = 0;
				do {
					++count;
					current = current.Next;
				}
				while (current != this);
				return count;
			}
		}

		public bool Contains (T obj, out IEnumerable<CircleEntry<T>> item)
		{
			item = Find (obj);
			return item.Count () > 0;
		}

		public bool Contains (Func<T, bool> func, out IEnumerable<CircleEntry<T>> item)
		{
			item = Find (func);
			return item.Count () > 0;
		}

		public bool Contains (T obj, out CircleEntry<T> item)
		{
			item = Find (obj).ElementAtOrDefault (0);
			return item != null;
		}

		public bool Contains (Func<T, bool> func, out CircleEntry<T> item)
		{
			item = Find (func).ElementAtOrDefault (0);
			return item != null;
		}

		public IEnumerable<CircleEntry<T>> Find (T obj)
		{
			return Find ((t) => t.Equals (obj));
		}

		public IEnumerable<CircleEntry<T>> Find (Func<T, bool> func)
		{
			CircleEntry<T> current = this;
			do {
				if (func (current.Value)) {
					yield return current;
				}
				current = current.Next;
			}
			while (current != this);
			yield break;
		}

		public int IndexOf (T obj)
		{
			return IndexOf ((t) => t.Equals (obj));
		}

		public int IndexOf (Func<T, bool> func)
		{
			int i = 0;
			CircleEntry<T> current = this;
			do {
				if (func (current.Value)) {
					return i;
				}
				current = current.Next;
				++ i;
			}
			while (current != this);
			return -1;
		}

		public IEnumerable<T> RangeTo (CircleEntry<T> other)
		{
			CircleEntry<T> current = this;
			do {
				yield return current.Value;
				current = current.Next;
			}
			while (current != other.Next && current != this);
		}

		public IEnumerable<T> WayTo (T other)
		{
			CircleEntry<T> current = this;
			while (!current.Value.Equals (other)) {
				yield return current.Value;
				current = current.Next;
				if (current == this) {
					break;
				}
			}
		}

		public IEnumerable<Tuple<T,T>> Pairs
		{
			get {
				CircleEntry<T> current = this;
				do {
					yield return Tuple.Create (current.Value, current.Next.Value);
					current = current.Next;
				}
				while (current != this);
			}
		}

		public IEnumerable<Tuple<T,T,T>> Triples
		{
			get {
				CircleEntry<T> current = this;
				do {
					yield return Tuple.Create (current.Previous.Value, current.Value, current.Next.Value);
					current = current.Next;
				}
				while (current != this);
			}
		}

		public IEnumerator<T> GetEnumerator ()
		{
			CircleEntry<T> current = this;
			do {
				//Log.Debug (this, " => ", current.Content);
				yield return current.Value;
				current = current.Next;
			}
			while (current != this);
		}

		// explicit interface implementation for nongeneric interface
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator (); // just return the generic version
		}

		public override string ToString ()
		{
			return "CircleEntry (" + Value.ToString () + ")";
		}

		public static CircleEntry<T> operator + (CircleEntry<T> circle, int i)
		{
			CircleEntry<T> next = circle;
			while (i > 0) {
				next = next.Next;
				i--;
			}
			while (i < 0) {
				next = next.Previous;
				i++;
			}
			return next;
		}

		public T this [int index]
		{
			get {
				return (this + index).Value;
			}
			set {
				(this + index).Value = value;
			}
		}

		public static CircleEntry<T> operator - (CircleEntry<T> circle, int i)
		{
			return circle + (-i);
		}

		public static CircleEntry<T> operator ++ (CircleEntry<T> circle)
		{
			return circle.Next;
		}

		public static CircleEntry<T> operator -- (CircleEntry<T> circle)
		{
			return circle.Previous;
		}

		public static implicit operator T (CircleEntry<T> circle)
		{
			return circle.Value;
		}

		public bool IsReadOnly { get { return false; } }

		public bool Contains (T obj)
		{
			CircleEntry<T> item = Find (obj).ElementAtOrDefault (0);
			return item != null;
		}

		public bool Remove (T value)
		{
			CircleEntry<T> item;
			if (Contains (value, out item)) {
				item.Remove ();
				return true;
			}
			else {
				return false;
			}
		}

		public void RemoveAt (int i)
		{
			(this + i).Remove ();
		}

		public void Insert (int i, T value)
		{
			(this + i).InsertBefore (value);
		}

		public void Add (T value)
		{
			if (Value == null) {
				Value = value;
			}
			else {
				InsertBefore (value);
			}
		}

		public void Clear ()
		{
			Remove ();
			Next = Previous = this;
		}

		public void CopyTo (T[] array, int start)
		{
			foreach (T value in this) {
				array.SetValue (value, start);
				++start;
			}
		}
	}

	public static class CircleExtensions
	{
		public static CircleEntry<T> ToCircle<T> (this IEnumerable<T> enumerable)
		{
			return new CircleEntry<T> (enumerable);
		}
	}
}
