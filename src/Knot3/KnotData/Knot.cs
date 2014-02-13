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

		private HashSet<Edge> selectedEdges;
		public int debugId;
		/// <summary>
		///
		/// </summary>
		public Action SelectionChanged = () => {};
		private List<SelectionBlock> StructuredSelection;
		private CircleEntry<Edge> lastSelected;
		public Action<Vector3> StartEdgeChanged = (v) => {};
		private KnotCharakteristic? CharakteristicCache = null;

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
		}

		private Knot (KnotMetaData metaData, CircleEntry<Edge> start, HashSet<Edge> selected)
		{
			startElement = start;
			MetaData = new KnotMetaData (
			    name: metaData.Name,
			    countEdges: () => this.startElement.Count,
			    format: metaData.Format,
			    filename: metaData.Filename
			);
			selectedEdges = selected;
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
			Vector3 position3D = Vector3.Zero;
			HashSet<Vector3> occupancy = new HashSet<Vector3> ();
			if (edges.Count () < 4) {
				return false;
			}
			foreach (Direction edge in edges) {
				if (occupancy.Contains (position3D + (edge / 2))) {
					return false;
				}
				else {
					occupancy.Add (position3D + (edge / 2));
					position3D += edge;
				}
			}
			if (position3D.DistanceTo (Vector3.Zero) > 0.00001f) {
				return false;
			}
			return true;
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

			HashSet<Edge> selected = new HashSet<Edge> ();
			List<Edge> list = new List<Edge> ();

			foreach (Tuple<Edge, Edge, Edge> triple in startElement.Triples) {
				Edge previousEdge = triple.Item1;
				Edge currentEdge = triple.Item2;
				Edge nextEdge = triple.Item3;

				if (selectedEdges.Contains (currentEdge) && !selectedEdges.Contains (previousEdge)) {
					// Log.Debug ("Insert ", distance, "x: ", direction);
					distance.Repeat (i => list.Add (new Edge (direction: direction, color: currentEdge)));
				}

				// Log.Debug ("Copy: ", currentEdge);
				list.Add (currentEdge);

				if (selectedEdges.Contains (currentEdge) && !selectedEdges.Contains (nextEdge)) {
					// Log.Debug ("Insert ", distance, "x: ", direction.Reverse);
					distance.Repeat (i => list.Add (new Edge (direction: direction.Reverse, color: currentEdge)));
				}
			}

			Log.Debug ("New Knot #", list.Count, " = ", string.Join (", ", from c in list select c.Direction));

			CircleEntry<Edge> newCircle = list.ToCircle ();
			CircleEntry<Edge> current = newCircle;
			do {
				if (current [- 1].Direction == current [- 2].Direction.Reverse && current.Count > 4) {
					// Selektierte nicht löschen
					if (selected.Contains (current [- 1]) || selected.Contains (current [- 2])) {
						Log.Debug ("Error: Selektierte nicht löschen");
						newknot = null;
						return false;
					}
					if (newCircle == current - 1 || newCircle == current - 2) {
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
			newknot = new Knot (MetaData, newCircle, selected);
			return true;
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
			// Erstelle die Liste nur, wenn es notwendig ist.
			if (StructuredSelection == null) {
				StructuredSelection = CreateStructuredSelection (startElement);
			}
			if (StructuredSelection.Count == 0) {
				return false;
			}
			// Alles selektiert
			if (StructuredSelection [0].Begin == StructuredSelection [0].End.Next) {
				return true;
			}
			HashSet<Axis> axes = new HashSet<Axis> ();
			// Für Jeden Block werden Start und ende untersucht.
			foreach (SelectionBlock block in StructuredSelection) {
				// Wenn Kante nach der Bewegung gelöscht werden müsste ist ein Zug nicht möglich
				if (block.Begin.Value.Direction == dir.Reverse && block.Begin.Previous.Value.Direction != dir.Reverse) {
					return false;
				}
				// Wenn Kante nach der Bewegung gelöscht werden müsste ist ein Zug nicht möglich
				if (block.End.Value.Direction == dir && block.End.Next.Value.Direction != dir) {
					return false;
				}
				foreach (Edge edge in block.Begin.RangeTo(block.End)) {
					axes.Add (edge.Direction.Axis);
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
			StructuredSelection = null;
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
			StructuredSelection = null;
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

		/// <summary>
		/// Erstellt, falls notwendig, eine Liste mit Circle Elementen, die jeweils Anfang und Ende eines selektierten Bereiches makieren.
		/// Die Liste hat immer 2n Einträge. Abwechselnd immer Anfang und Ende. Beide Innerhalb der Selektion.
		/// D.h. Ist die Selektion nur eine Kante lang, dann sind die Einträge identisch.
		/// </summary>
		private List<SelectionBlock> CreateStructuredSelection (CircleEntry<Edge> circle)
		{
			List<SelectionBlock> selection = new List<SelectionBlock> ();
			// wenn nichts ausgewählt ist muss nichts weiter erstellt werden.
			if (selectedEdges.Count == 0) {
				return selection;
			}
			// wenn alles ausgewählt ist kann man die erstellung verkürzen.
			if (selectedEdges.Count == MetaData.CountEdges) {
				selection.Add (new SelectionBlock (circle, circle.Previous));
				return selection;
			}
			CircleEntry<Edge> start = circle;
			CircleEntry<Edge> stop = start.Previous;
			// Suche eine Stelle an der ein Selektionsblock beginnt.
			if (selectedEdges.Contains (start.Value)) {
				// Wenn "edges" in der Selektion ist geh nach links, bis zum Anfang des Blockes.
				while (selectedEdges.Contains (start.Previous.Value)) {
					start--;
				}
			}
			else {
				// Wenn "edges" nicht selektiert ist, gehe nach rechts bis zum beginn des nächsten Blockes.
				while (!selectedEdges.Contains (start.Value)) {
					start++;
				}
			}
			do {
				// "start" zeigt auf den Beginn eines Blockes und wird daher hinzu gefügt.
				CircleEntry<Edge> begin = start;
				stop = start;
				// Gehe bis zum Ende des selektierten Blockes.
				while (selectedEdges.Contains (stop.Next.Value)) {
					stop++;
				}
				CircleEntry<Edge> end = stop;

				// Gehe bis zum start des nächsten Blockes.
				start = stop.Next;
				while (!selectedEdges.Contains (start.Value)) {
					start++;
				}

				// Füge den Selektions-Block der Liste hinzu
				selection.Add (new SelectionBlock (begin, end));
			}
			// Höre auf, wenn man wieder beim element ist mit dem man begonnen hat.
			while (start != selection [0].Begin);

			return selection;
		}

		#endregion

		#region Classes and Structs

		private class SelectionBlock
		{
			public CircleEntry<Edge> Begin { get; set; }

			public CircleEntry<Edge> End { get; set; }

			public SelectionBlock (CircleEntry<Edge> begin, CircleEntry<Edge> end)
			{
				Begin = begin;
				End = end;
			}
		}

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
