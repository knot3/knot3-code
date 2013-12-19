using System;
using System.Collections.Generic;
using System.Linq;

using System;
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

using Core;
using GameObjects;
using Screens;
using RenderEffects;
using KnotData;

namespace Widgets
{
    /// <summary>
    /// Eine Schaltfläche, der eine Zeichenkette anzeigt und auf einen Linksklick reagiert.
    /// </summary>
    public class MenuButton : MenuItem
    {

        #region Properties

        /// <summary>
        /// Die Aktion, die ausgeführt wird, wenn der Spieler auf die Schaltfläche klickt.
        /// </summary>
        public Action OnClick { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Erzeugt ein neues MenuButton-Objekt und initialisiert dieses mit dem zugehörigen GameScreen-Objekt.
        /// Zudem sind Angabe der Zeichenreihenfolge, einer Zeichenkette für den Namen der Schaltfläche
        /// und der Aktion, welche bei einem Klick ausgeführt wird Pflicht.
        /// </summary>
        public void MenuButton (GameScreen screen, DisplayLayer drawOrder, String name, Action onClick)
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }
}

