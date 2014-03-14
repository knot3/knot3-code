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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Input
{
    [ExcludeFromCodeCoverageAttribute]
    public abstract class KeyBindingListener : ScreenComponent
    {
        /// <summary>
        /// Die Standard-Tastenbelegung.
        /// </summary>
        public static Dictionary<Keys, PlayerAction> DefaultKeyAssignment { get; private set; }

        /// <summary>
        /// Die aktuelle Tastenbelegung, von Taste zu Aktion.
        /// </summary>
        protected internal static Dictionary<Keys, PlayerAction> CurrentKeyAssignment { get; private set; }

        /// <summary>
        /// Die aktuelle Tastenbelegung, von Aktion zu Taste.
        /// </summary>
        protected internal static Dictionary<PlayerAction, Keys> CurrentKeyAssignmentReversed { get; protected set; }

        /// <summary>
        /// Wird ausgelöst, wenn sich die Tastenbelegungen in der Konfigurationsdatei geändert haben.
        /// </summary>
        public static Action ControlSettingsChanged = () => {};

        static KeyBindingListener ()
        {
            Log.Message ("Static constructor of KeyBindingListener (non-generic) called!");
            DefaultKeyAssignment = new Dictionary<Keys, PlayerAction> ();
            CurrentKeyAssignment = new Dictionary<Keys, PlayerAction> ();
            CurrentKeyAssignmentReversed = new Dictionary<PlayerAction, Keys> ();
        }

        public KeyBindingListener (IScreen screen, DisplayLayer drawOrder)
        : base (screen, drawOrder)
        {
        }

        /// <summary>
        /// Führt die statischen Initialierer der angegebenen Typen aus. Dies kann zum Beispiel verwendet werden,
        /// um sicherzustellen, dass die Standardtastenbelegungen, die in den statischen Initialierern der von
        /// KeyBindingListener&lt;T&gt; erbenden Klassen gesetzt werden, gesetzt sind, obwohl die jeweiligen
        /// von KeyBindingListener erbenden Klassen zur Laufzeit bisher noch nie verwendet wurden und daher
        /// auch nicht initialiert wurden.
        /// </summary>
        public static void InitializeListeners (params Type[] types)
        {
            foreach (Type type in types) {
                RuntimeHelpers.RunClassConstructor (type.TypeHandle);
            }
        }

        /// <summary>
        /// Wird ausgeführt, sobald ein KeyBindingListener erstellt wird und danach,
        /// wenn sich die Tastenbelegung geändert hat.
        /// </summary>
        public static void ReadKeyAssignments ()
        {
            // Drehe die Zuordnung um; von (Taste -> Aktion) zu (Aktion -> Taste)
            Dictionary<PlayerAction, Keys> defaultReversed = DefaultKeyAssignment.ReverseDictionary ();

            // Leere die aktuelle Zuordnung
            CurrentKeyAssignment.Clear ();

            // Fülle die aktuelle Zuordnung mit aus der Einstellungsdatei gelesenen werten.
            // Iteriere dazu über alle gültigen PlayerActions...
            foreach (PlayerAction action in PlayerAction.Values) {
                string actionName = action.Name;
                if (defaultReversed.ContainsKey (action)) {
                    // Erstelle eine Option...
                    KeyOption option = new KeyOption (
                        section: "controls",
                        name: actionName,
                        defaultValue: defaultReversed [action],
                        configFile: Config.Default
                    ) { Verbose = false };
                    // und lese den Wert aus und speichere ihn in der Zuordnung.
                    CurrentKeyAssignment [option.Value] = action;
                }
            }
            CurrentKeyAssignmentReversed = CurrentKeyAssignment.ReverseDictionary ();
        }

        public static Keys LookupKey (PlayerAction action)
        {
            if (CurrentKeyAssignmentReversed.ContainsKey (action)) {
                return CurrentKeyAssignmentReversed [action];
            }
            else {
                throw new InvalidOperationException ("There is no action -> key binding for action: " + action);
            }
        }

        public static bool ContainsKey (Keys key)
        {
            return CurrentKeyAssignment.ContainsKey (key);
        }

        public static PlayerAction LookupAction (Keys key)
        {
            if (CurrentKeyAssignment.ContainsKey (key)) {
                return CurrentKeyAssignment [key];
            }
            else {
                throw new InvalidOperationException ("There is no key -> action binding for key: " + key);
            }
        }
    }

    [ExcludeFromCodeCoverageAttribute]
    public abstract class KeyBindingListener<T> : KeyBindingListener, IKeyEventListener
    {
        /// <summary>
        /// Was bei den jeweiligen Aktionen ausgeführt wird.
        /// </summary>
        protected static Dictionary<PlayerAction, Action<GameTime>> ActionBindings { get; private set; }

        static KeyBindingListener ()
        {
            Log.Message ("Static constructor of KeyBindingListener<", typeof (T), "> called!");
            ReadKeyAssignments ();

            ActionBindings = new Dictionary<PlayerAction, Action<GameTime>> ();
        }

        /// <summary>
        /// Die Tasten, auf die die Klasse reagiert. Wird aus der aktuellen Tastenbelegung berechnet.
        /// </summary>
        public List<Keys> ValidKeys { get; private set; }

        /// <summary>
        /// Zeigt an, ob die Klasse zur Zeit auf Tastatureingaben reagiert.
        /// </summary>
        public abstract bool IsKeyEventEnabled { get; }

        /// <summary>
        /// Zeigt an, ob die Klasse modal ist.
        /// </summary>
        public abstract bool IsModal { get; }

        public KeyBindingListener (IScreen screen, DisplayLayer drawOrder)
        : base (screen, drawOrder)
        {
            // Tasten
            ValidKeys = new List<Keys> ();
            KeyBindingListener.ControlSettingsChanged += UpdateKeyBindings;
            UpdateKeyBindings ();
        }

        /// <summary>
        /// Wird ausgeführt, sobald ein KeyBindingListener erstellt wird und danach,
        /// wenn sich die Tastenbelegung geändert hat.
        /// </summary>
        protected void UpdateKeyBindings ()
        {
            ReadKeyAssignments ();

            // Aktualisiere die Liste von Tasten, zu denen wir als IKeyEventListener benachrichtigt werden
            ValidKeys.Clear ();
            ValidKeys.AddRange (from binding in ActionBindings.Keys select CurrentKeyAssignmentReversed [binding]);
        }

        public virtual void OnKeyEvent (List<Keys> keys, KeyEvent keyEvent, GameTime time)
        {
            if (IsKeyEventEnabled) {
                // Iteriere über alle gedrückten Tasten
                foreach (Keys key in keys) {
                    // Ist der Taste eine Aktion zugeordnet?
                    if (CurrentKeyAssignment.ContainsKey (key)) {
                        // Während die Taste gedrückt gehalten ist...
                        if (Screen.InputManager.KeyHeldDown (key)) {
                            // führe die entsprechende Aktion aus!
                            PlayerAction action = CurrentKeyAssignment [key];
                            Action<GameTime> binding = ActionBindings [action];
                            binding (time);
                        }
                    }
                }
            }
        }
    }
}
