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

using System.Diagnostics.CodeAnalysis;

namespace Knot3.Framework.Input
{
    /// <summary>
    /// Repräsentiert die möglichen Eingabeaktionen, wie sie von verschiedenen Inputhandlern berechnet und verwendet werden können.
    /// Die aktuelle Eingabeaktion wird zentral in der Klasse InputManager gehalten.
    /// </summary>
    public enum InputAction {
        /// <summary>
        /// Keine Eingabe bzw. undefiniert.
        /// </summary>
        None=0,
        /// <summary>
        /// Der Spieler bewegt mit der Maus oder der Tastatur das Kamera-Ziel.
        /// </summary>
        CameraTargetMove,
        /// <summary>
        /// Der Spieler bewegt die Kamera mit der Maus oder der Tastatur wie auf einer Kugel
        /// um ein Objekt herum, wobei die Distanz zum Zielobjekt gleich bleibt.
        /// </summary>
        ArcballMove,
        /// <summary>
        /// Der Spieler bewegt die Maus frei und keine Maustasten sind gedrückt;
        /// es sollen keine expliziten Berechnungen auf Basis der absoluten oder relativen Mausposition stattfinden.
        /// Über Aktionen, die die Tastatur betreffen, wird keine Aussage gemacht.
        /// </summary>
        FreeMouse,
        /// <summary>
        /// Der Spieler bewegt die Kamera wie in einem First-Person-Shooter (im Gegensatz zu z.B. einer Arcball-Bewegung).
        /// </summary>
        FirstPersonCameraMove,
        /// <summary>
        /// Das in der World-Klasse als selektiert markierte Spielobjekt wird endgültig verschoben, nachdem die Maus losgelassen wurde.
        /// Wird immer nach SelectedObjectShadowMove ausgeführt.
        /// </summary>
        SelectedObjectMove,
        /// <summary>
        /// Das in der World-Klasse als selektiert markierte Spielobjekt wird bei gedrückt gehaltener Maustaste verschoben.
        /// Dabei können die Dekorierer ShadowObject und ShadowModel zum Einsatz kommen. Die Aktion wird vorerst nur visuell ausgeführt und
        /// nicht in Datenstrukturen übernommen. Dies kann nach einer eventuell auszuführenden Gültigkeitsprüfung
        /// in einem nachfolgenden SelectedObjectMove geschehen.
        /// </summary>
        SelectedObjectShadowMove,
    }
}
