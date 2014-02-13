using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
using Knot3.GameObjects;
using Knot3.Screens;
using Knot3.RenderEffects;
using Knot3.Widgets;
using Knot3.Utilities;
using Knot3.Development;

namespace Knot3.KnotData
{
	/// <summary>
	/// Diese Klasse repräsentiert einen Knoten, bestehend aus einem Knoten-Metadaten-Objekt und einer doppelt-verketteten Liste von Kanten. Ein Knoten ist eine zyklische Kantenfolge, bei der keine zwei Kanten Kanten den gleichen Raum einnehmen.
	/// </summary>
	public sealed class Knot : ICloneable, IEnumerable<Edge>, IEquatable<Knot>
	{
		#region Properties

		/// <summary>
		/// Der Name des Knotens, welcher auch leer sein kann.
		/// Beim Speichern muss der Nutzer in diesem Fall zwingend einen nichtleeren Namen wählen.
		/// Der Wert dieser Eigenschaft wird aus der \glqq Name\grqq~-Eigenschaft des Metadaten-Objektes geladen
		/// und bei Änderungen wieder in diesem gespeichert.
		/// Beim Ändern dieser Eigenschaft wird automatisch auch der im Metadaten-Objekt enthaltene Dateiname verändert.
		/// </summary>
		public string Name
		{
			get { return MetaData.Name; }
			set { MetaData.Name = value; }
		}

		/// <summary>
		/// Das Startelement der doppelt-verketteten Liste, in der die Kanten gespeichert werden.
		/// </summary>
		private CircleEntry<Edge> startElement;

		/// <summary>
		/// Die Metadaten des Knotens.
		/// </summary>
		public KnotMetaData MetaData { get; private set; }

		/// <summary>
		/// Ein Ereignis, das in der Move-Methode ausgelöst wird, wenn sich die Struktur der Kanten geändert hat.
		/// </summary>
		public Action EdgesChanged = () => {};

		/// <summary>
		/// Enthält die aktuell vom Spieler selektierten Kanten in der Reihenfolge, in der sie selektiert wurden.
		/// </summary>
		public IEnumerable<Edge> SelectedEdges { get { return selectedEdges; } }

		/// <summary>
		/// Enthält die selektierten Kanten.
		/// </summary>
		private HashSet<Edge> selectedEdges;

		/// <summary>
		/// WTF?!
		/// </summary>
		public int debugId;

		/// <summary>
		/// Wird aufgerufen, wenn sich die Selektion geändert hat.
		/// </summary>
		public Action SelectionChanged = () => {};

		/// <summary>
		/// Enthält die zuletzt selektierte Kante.
		/// </summary>
		private CircleEntry<Edge> lastSelected;

		/// <summary>
		/// Wird aufgerufen, wenn sich die Startkante geändert hat.
		/// </summary>
		public Action<Vector3> StartEdgeChanged = (v) => {};

		/// <summary>
		/// Der Cache für die Knotencharakteristik.
		/// </summary>
		private KnotCharakteristic? CharakteristicCache = null;

		public Vector3 OffSet { get; private set;}

		#endregion

		#region Constructors

		/// <summary>
		/// Erstellt einen minimalen Standardknoten. Das Metadaten-Objekt enthält in den Eigenschaften,
		/// die das Speicherformat und den Dateinamen beinhalten, den Wert \glqq null\grqq.
		/// </summary>
		public Knot ()
		{
			debugId++;
			MetaData = new KnotMetaData (String.Empty, () => startElement.Count, null, null);
			startElement = new CircleEntry<Edge> (new Edge[] {
				// Edge.Up, Edge.Right, Edge.Right, Edge.Down, Edge.Backward,
				// Edge.Up, Edge.Left, Edge.Left, Edge.Down, Edge.Forward
				Edge.Up, Edge.Right, Edge.Down, Edge.Left
			}
			                                     );
			selectedEdges = new HashSet<Edge> ();
			OffSet = Vector3.Zero;
		}

		/// <summary>
		/// Erstellt einen neuen Knoten mit dem angegebenen Metadaten-Objekt und den angegebenen Kanten,
		/// die in der doppelt verketteten Liste gespeichert werden.
		/// Die Eigenschaft des Metadaten-Objektes, die die Anzahl der Kanten enthält,
		/// wird auf ein Delegate gesetzt, welches jeweils die aktuelle Anzahl der Kanten dieses Knotens zurückgibt.
		/// </summary>
		public Knot (KnotMetaData metaData, IEnumerable<Edge> edges)
		{
			debugId++;
			Stack<Direction> structure = new Stack<Direction> ();
			foreach (Edge edge in edges) {
				structure.Push (edge.Direction);
			}
			if (!IsValidStructure (structure)) {
				throw new InvalidDataException ();
			}
			MetaData = new KnotMetaData (
			    name: metaData.Name,
			    countEdges: () => this.startElement.Count,
			    format: metaData.Format,
			    filename: metaData.Filename
			);
			this.startElement = new CircleEntry<Edge> (edges);
			selectedEdges = new HashSet<Edge> ();
			OffSet = Vector3.Zero;
		}

		private Knot (KnotMetaData metaData, CircleEntry<Edge> start, HashSet<Edge> selected, Vector3 offset)
		{
			startElement = start;
			MetaData = new KnotMetaData (
			    name: metaData.Name,
			    countEdges: () => this.startElement.Count,
			    format: metaData.Format,
			    filename: metaData.Filename
			);
			selectedEdges = selected;
			OffSet = offset;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Prüft ob die gegeben Struktur einen gültigen Knoten darstellt.
		/// </summary>
		public bool IsValidStructure (IEnumerable<Direction> knot)
		{
			Vector3 position3D = Vector3.Zero;
			HashSet<Vector3> occupancy = new HashSet<Vector3> ();
			if (knot.Count () < 4) {
				return false;
			}
			foreach (Direction peek in knot) {
				if (occupancy.Contains (position3D + (peek / 2))) {
					return false;
				}
				else {
					occupancy.Add (position3D + (peek / 2));
					position3D += peek;
				}
			}
			if (position3D.DistanceTo (Vector3.Zero) > 0.00001f) {
				return false;
			}
			return true;
		}

		private bool IsValidStructure (IEnumerable<Edge> edges)
		{
			return IsValidStructure (from e in edges select e.Direction);
		}

		/// <summary>
		/// Verschiebt die aktuelle Kantenauswahl in die angegebene Richtung um die angegebene Distanz.
		/// </summary>
		public bool TryMove (Direction direction, int distance, out Knot newknot)
		{
			if (direction == Direction.Zero || distance == 0) {
				newknot = this;
				return true;
			}

			Log.Debug ("TryMove: direction = ", direction, ", distance = ", distance);
			Log.Debug ("Current Knot #", startElement.Count, " = ", string.Join (", ", from c in startElement select c.Direction));

			HashSet<Edge> selected = new HashSet<Edge> (selectedEdges);
			CircleEntry<Edge> newCircle = CircleEntry<Edge>.Empty;

			foreach (Tuple<Edge, Edge, Edge> triple in startElement.Triples) {
				Edge previousEdge = triple.Item1;
				Edge currentEdge = triple.Item2;
				Edge nextEdge = triple.Item3;

				if (selectedEdges.Contains (currentEdge) && !selectedEdges.Contains (previousEdge)) {
					distance.Repeat (i => newCircle.Add (new Edge (direction: direction, color: currentEdge)));
				}

				newCircle.Add (currentEdge);

				if (selectedEdges.Contains (currentEdge) && !selectedEdges.Contains (nextEdge)) {
					distance.Repeat (i => newCircle.Add (new Edge (direction: direction.Reverse, color: currentEdge)));
				}
			}

			Log.Debug ("New Knot #", newCircle.Count, " = ", string.Join (", ", from c in newCircle select c.Direction));

			Vector3 localOffset = OffSet;
			CircleEntry<Edge> current = newCircle;
			do {
				if (current [- 1].Direction == current [- 2].Direction.Reverse) {
					// Selektierte nicht löschen
					if (selected.Contains (current [- 1]) || selected.Contains (current [- 2])) {
						Log.Debug ("Error: Selektierte nicht löschen");
						newknot = null;
						return false;
					}
					if (newCircle == current - 1) {
						localOffset += (current - 1).Value;
						newCircle = current;
					}
					else if (newCircle == current - 2) {
						localOffset += (current - 1).Value.Direction + (current - 1).Value.Direction;
						newCircle = current;
					}
					(current - 2).Remove ();
					(current - 1).Remove ();
				}
				++ current;
			}
			while (current != newCircle);

			Log.Debug ("New Knot after Remove #", newCircle.Count, " = ", string.Join (", ", from c in newCircle select c.Direction));

			if (!IsValidStructure (newCircle)) {
				Log.Debug ("Error: newCircle ist keine valide Struktur");
				newknot = null;
				return false;
			}
			newknot = new Knot (MetaData, newCircle, selected, localOffset);
			return true;
		}

		public Vector3 MoveCenterToZero()
		{
			Vector3 position3D = Vector3.Zero;
			Dictionary<Vector3, Edge> occupancy = new Dictionary<Vector3, Edge>();
			foreach (Edge edge in startElement) {
				occupancy.Add(position3D + (edge.Direction / 2), edge);
				position3D += edge;
			}
			Vector3 mid = Vector3.Zero;
			foreach (KeyValuePair<Vector3,Edge> pos in occupancy) {
				mid += pos.Key;
			}
			mid /= startElement.Count;
			float minDistance = mid.Length();
			Edge newStart = startElement.Value;
			foreach (KeyValuePair<Vector3,Edge> pos in occupancy) {
				float testDistance = pos.Key.DistanceTo(mid);
				if (testDistance < minDistance) {
					newStart = pos.Value;
					minDistance = testDistance;
				}
			}
			Vector3 offset = Vector3.Zero;
			foreach (Edge edge in startElement.WayTo(newStart)) {
				offset += edge;
			}
			startElement.Contains(newStart, out startElement);
			offset += OffSet;
			OffSet = Vector3.Zero;
			return offset;
		}

		/// <summary>
		/// Verschiebt die aktuelle Kantenauswahl in die angegebene Richtung um die angegebene Distanz.
		/// </summary>
		public bool Move (Direction direction, int distance)
		{
			Knot newKnot;
			if (TryMove (direction, distance, out newKnot)) {
				startElement = newKnot.startElement;
				selectedEdges = newKnot.selectedEdges;
				return true;
			}
			else {
				return false;
			}
		}

		/// <summary>
		/// Gibt an ob ein Move in diese Richtung überhaupt möglich ist.
		/// </summary>
		public bool IsValidMove (Direction dir)
		{
			// Nichts selektiert
			if (selectedEdges.Count == 0) {
				return false;
			}
			// Alles selektiert
			if (selectedEdges.Count == startElement.Count) {
				return true;
			}

			HashSet<Axis> axes = new HashSet<Axis> ();
			foreach (Tuple<Edge, Edge, Edge> triple in startElement.Triples) {
				Edge previousEdge = triple.Item1;
				Edge currentEdge = triple.Item2;
				Edge nextEdge = triple.Item3;

				// Wenn Kante nach der Bewegung gelöscht werden müsste ist ein Zug nicht möglich
				if (selectedEdges.Contains (currentEdge) && !selectedEdges.Contains (previousEdge)
				        && currentEdge.Direction == dir.Reverse && previousEdge.Direction != dir.Reverse) {
					return false;
				}
				// Wenn Kante nach der Bewegung gelöscht werden müsste ist ein Zug nicht möglich
				if (selectedEdges.Contains (currentEdge) && !selectedEdges.Contains (nextEdge)
				        && currentEdge.Direction == dir && nextEdge.Direction != dir) {
					return false;
				}

				if (selectedEdges.Contains (currentEdge)) {
					axes.Add (currentEdge.Direction.Axis);
				}
			}
			// Wenn alle Kanten entlang einer Achse angeordnet sind und die Verschieberichtung die selbe Achse hat
			if (axes.Count == 1 && axes.Contains (dir.Axis)) {
				return false;
			}
			return true;
		}

		private void onEdgesChanged ()
		{
			CharakteristicCache = null;
			EdgesChanged ();
		}

		/// <summary>
		/// Gibt die doppelt-verkettete Kantenliste als Enumerator zurück.
		/// </summary>
		public IEnumerator<Edge> GetEnumerator ()
		{
			return startElement.GetEnumerator ();
		}

		/// <summary>
		/// Speichert den Knoten unter dem Dateinamen in dem Dateiformat, das in dem Metadaten-Objekt angegeben ist.
		/// Enthalten entweder die Dateiname-Eigenschaft, die Dateiformat-Eigenschaft
		/// oder beide den Wert \glqq null\grqq, dann wird eine IOException geworfen.
		/// </summary>
		public void Save ()
		{
			if (MetaData.Format == null) {
				throw new IOException ("Error: Knot: MetaData.Format is null!");
			}
			else if (MetaData.Filename == null) {
				throw new IOException ("Error: Knot: MetaData.Filename is null!");
			}
			else {
				MetaData.Format.Save (this);
			}
		}

		/// <summary>
		/// Erstellt eine vollständige Kopie des Knotens, inklusive der Kanten-Datenstruktur und des Metadaten-Objekts.
		/// </summary>
		public Object Clone ()
		{
			CircleEntry<Edge> newCircle = new CircleEntry<Edge> (startElement as IEnumerable<Edge>);
			KnotMetaData metaData = new KnotMetaData (
			    name: MetaData.Name,
			    countEdges: () => 0,
			    format: MetaData.Format,
			    filename: MetaData.Filename
			);
			return new Knot (metaData: metaData, edges: newCircle) {
				selectedEdges = new HashSet<Edge> (selectedEdges),
				EdgesChanged = null,
				SelectionChanged = null,
			};
		}

		private void OnSelectionChanged ()
		{
			SelectionChanged ();
		}

		/// <summary>
		/// Fügt die angegebene Kante zur aktuellen Kantenauswahl hinzu.
		/// </summary>
		public void AddToSelection (Edge edge)
		{
			IEnumerable<CircleEntry<Edge>> found = startElement.Find (edge);
			if (found.Any ()) {
				if (!selectedEdges.Contains (edge)) {
					selectedEdges.Add (edge);
				}
				lastSelected = found.ElementAt (0);
			}
			OnSelectionChanged ();
		}

		/// <summary>
		/// Entfernt die angegebene Kante von der aktuellen Kantenauswahl.
		/// </summary>
		public void RemoveFromSelection (Edge edge)
		{
			selectedEdges.Remove (edge);
			if (lastSelected.Value == edge) {
				lastSelected = null;
			}
			OnSelectionChanged ();
		}

		/// <summary>
		/// Hebt die aktuelle Kantenauswahl auf.
		/// </summary>
		public void ClearSelection ()
		{
			selectedEdges.Clear ();
			lastSelected = null;
			OnSelectionChanged ();
		}

		/// <summary>
		/// Fügt alle Kanten auf dem kürzesten Weg zwischen der zuletzt ausgewählten Kante und der angegebenen Kante
		/// zur aktuellen Kantenauswahl hinzu. Sind beide Wege gleich lang,
		/// wird der Weg in Richtung der ersten Kante ausgewählt.
		/// </summary>
		public void AddRangeToSelection (Edge selectedEdge)
		{
			if (lastSelected == null) {
				AddToSelection (selectedEdge);
				return;
			}
			CircleEntry<Edge> selectedCircle = null;
			if (startElement.Contains (selectedEdge, out selectedCircle) && selectedEdge != lastSelected.Value) {
				List<Edge> forward = new List<Edge> (lastSelected.RangeTo (selectedCircle));
				List<Edge> backward = new List<Edge> (selectedCircle.RangeTo (lastSelected));

				if (forward.Count < backward.Count) {
					foreach (Edge e in forward) {
						if (!selectedEdges.Contains (e)) {
							selectedEdges.Add (e);
						}
					}
				}
				else {
					foreach (Edge e in backward) {
						if (!selectedEdges.Contains (e)) {
							selectedEdges.Add (e);
						}
					}
				}
				lastSelected = selectedCircle;
			}
			OnSelectionChanged ();
		}

		/// <summary>
		/// Prüft, ob die angegebene Kante in der aktuellen Kantenauswahl enthalten ist.
		/// </summary>
		public Boolean IsSelected (Edge edge)
		{
			return selectedEdges.Contains (edge);
		}

		/// <summary>
		/// Gibt die doppelt-verkettete Kantenliste als Enumerator zurück.
		/// [name=IEnumerable.GetEnumerator]
		/// [keywords= ]
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator (); // just return the generic version
		}

		/// <summary>
		/// Speichert den Knoten unter dem angegebenen Dateinamen in dem angegebenen Dateiformat.
		/// </summary>
		public void Save (IKnotIO format, string filename)
		{
			KnotMetaData metaData = new KnotMetaData (MetaData.Name, () => MetaData.CountEdges, format, filename);
			Knot knotToSave = new Knot (metaData, startElement);
			format.Save (knotToSave);
		}

		/// <summary>
		/// Prüft, ob die räumliche Struktur identisch ist, unabhängig von dem Startpunkt und der Richtung der Datenstruktur.
		/// [parameters=Knot other]
		/// </summary>
		public bool Equals (Knot other)
		{
			KnotCharakteristic thisCharakteristik = Charakteristic ();
			KnotCharakteristic otherCharakteristik = other.Charakteristic ();
			if (thisCharakteristik.CountEdges != otherCharakteristik.CountEdges) {
				return false;
			}
			// Bei Struktur im gleicher Richtung
			if (thisCharakteristik.CharacteristicalEdge.Value.Direction == otherCharakteristik.CharacteristicalEdge.Value.Direction) {
				CircleEntry<Edge> currentThisElement = thisCharakteristik.CharacteristicalEdge.Next;
				CircleEntry<Edge> currentOtherElement = otherCharakteristik.CharacteristicalEdge.Next;
				while (currentThisElement != thisCharakteristik.CharacteristicalEdge) {
					if (currentThisElement.Value.Direction != currentOtherElement.Value.Direction) {
						return false;
					}
					currentThisElement++;
					currentOtherElement++;
				}
				return true;
			}
			// Bei Struktur in entgegengesetzter Richtung
			else if (thisCharakteristik.CharacteristicalEdge.Value.Direction == otherCharakteristik.CharacteristicalEdge.Value.Direction.Reverse) {
				CircleEntry<Edge> currentThisElement = thisCharakteristik.CharacteristicalEdge.Next;
				CircleEntry<Edge> currentOtherElement = otherCharakteristik.CharacteristicalEdge.Next;
				while (currentThisElement != thisCharakteristik.CharacteristicalEdge) {
					if (currentThisElement.Value.Direction != currentOtherElement.Value.Direction.Reverse) {
						return false;
					}
					currentThisElement++;
					currentOtherElement++;
				}
				return true;
			}
			else {
				return false;
			}
		}

		/// <summary>
		/// Gibt chrakteristische Werte zurück, die bei gleichen Knoten gleich sind.
		/// Einmal als Key ein eindeutiges Circle\<Edge\> Element und als Value
		/// einen Charakteristischen Integer. Momentan die Anzahl der Kanten.
		/// </summary>
		private KnotCharakteristic Charakteristic ()
		{
			if (CharakteristicCache.HasValue) {
				return CharakteristicCache.Value;
			}

			CircleEntry<Edge> charakteristikElement = startElement;
			Vector3 position3D = startElement.Value.Direction;
			Vector3 bestPosition3D = startElement.Value.Direction / 2;
			CircleEntry<Edge> edgePointer = startElement.Next;

			int edgeCount = 1;
			for (edgeCount = 1; edgePointer != startElement; edgePointer++, edgeCount++) {
				Vector3 nextPosition3D = position3D + edgePointer.Value.Direction / 2;
				if ((nextPosition3D.X < bestPosition3D.X)
				        || (nextPosition3D.X == bestPosition3D.X && nextPosition3D.Y < bestPosition3D.Y)
				        || (nextPosition3D.X == bestPosition3D.X && nextPosition3D.Y == bestPosition3D.Y && nextPosition3D.Z < bestPosition3D.Z)) {
					bestPosition3D = position3D + edgePointer.Value.Direction / 2;
					charakteristikElement = edgePointer;
				}
				position3D += edgePointer.Value.Direction;
			}

			CharakteristicCache = new KnotCharakteristic (charakteristikElement, edgeCount);
			return CharakteristicCache.Value;
		}

		public override string ToString ()
		{
			return "Knot(name=" + Name + ",#edgecount=" + startElement.Count.ToString ()
			       + ",format=" + (MetaData.Format != null ? MetaData.ToString () : "null")
			       + ")";
		}

		#endregion

		#region Classes and Structs

		private struct KnotCharakteristic {
			public CircleEntry<Edge> CharacteristicalEdge { get; private set; }

			public int CountEdges { get; private set; }

			public KnotCharakteristic (CircleEntry<Edge> characteristicalEdge, int countEdges)
			: this()
			{
				CharacteristicalEdge = characteristicalEdge;
				CountEdges = countEdges;
			}
		}

		#endregion
	}
}
