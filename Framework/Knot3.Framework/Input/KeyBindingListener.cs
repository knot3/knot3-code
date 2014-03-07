using System;
using Knot3.Framework.Core;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Knot3.Framework.Storage;
using Knot3.Framework.Platform;
using System.Linq;
using Knot3.Framework.Utilities;
using Knot3.Framework.Development;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace Knot3.Framework.Input
{
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

        static KeyBindingListener ()
        {
            Log.Message ("Static constructor of KeyBindingListener (non-generic) called!");
            DefaultKeyAssignment = new Dictionary<Keys, PlayerAction> ();
            CurrentKeyAssignment = new Dictionary<Keys, PlayerAction> ();
            CurrentKeyAssignmentReversed = new Dictionary<PlayerAction, Keys> ();
        }

        public KeyBindingListener (IScreen screen, DisplayLayer drawOrder)
            : base(screen, drawOrder)
        {
        }

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

                // Erstelle eine Option...
                KeyOption option = new KeyOption (
                    section: "controls",
                    name: actionName,
                    defaultValue: defaultReversed [action],
                    configFile: Config.Default
                );
                // und lese den Wert aus und speichere ihn in der Zuordnung.
                CurrentKeyAssignment [option.Value] = action;
            }
            CurrentKeyAssignmentReversed = CurrentKeyAssignment.ReverseDictionary ();
        }

        public static Keys LookupKey (PlayerAction action)
        {
            if (CurrentKeyAssignmentReversed.ContainsKey (action))
                return CurrentKeyAssignmentReversed [action];
            else
                throw new InvalidOperationException ("There is no action -> key binding for action: " + action);
        }

        public static bool ContainsKey (Keys key)
        {
            return CurrentKeyAssignment.ContainsKey (key);
        }

        public static PlayerAction LookupAction (Keys key)
        {
            if (CurrentKeyAssignment.ContainsKey (key))
                return CurrentKeyAssignment [key];
            else
                throw new InvalidOperationException ("There is no key -> action binding for key: " + key);
        }
    }

    public abstract class KeyBindingListener<T> : KeyBindingListener, IKeyEventListener
    {
        /// <summary>
        /// Was bei den jeweiligen Aktionen ausgeführt wird.
        /// </summary>
        protected static Dictionary<PlayerAction, Action<GameTime>> ActionBindings { get; private set; }

        static KeyBindingListener ()
        {
            Log.Message ("Static constructor of KeyBindingListener<", typeof(T), "> called!");
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
        : base(screen, drawOrder)
        {
            // Tasten
            ValidKeys = new List<Keys> ();
            OnControlSettingsChanged ();
        }

        /// <summary>
        /// Wird ausgeführt, sobald ein KeyBindingListener erstellt wird und danach,
        /// wenn sich die Tastenbelegung geändert hat.
        /// </summary>
        protected void OnControlSettingsChanged ()
        {
            ReadKeyAssignments ();

            // Aktualisiere die Liste von Tasten, zu denen wir als IKeyEventListener benachrichtigt werden
            ValidKeys.Clear ();
            ValidKeys.AddRange (CurrentKeyAssignment.Keys.AsEnumerable ());
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

