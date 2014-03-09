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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Knot3.Framework.Utilities
{
    [ExcludeFromCodeCoverageAttribute]
    public static class EnumHelper
    {
        public static IEnumerable<string> ToEnumDescriptions<T> (this IEnumerable<T> enumValues)
        {
            foreach (T val in enumValues) {
                yield return val.ToEnumDescription<T> ();
            }
        }

        public static Hashtable GetDescriptionTable<T> ()
        {
            Hashtable table = new Hashtable ();
            foreach (T val in ToEnumValues<T>()) {
                string descr = val.ToEnumDescription<T> ();
                table [val] = descr;
                table [descr] = val;
            }
            return table;
        }

        public static IEnumerable<T> ToEnumValues<T> ()
        {
            Type enumType = typeof (T);

            return enumType.ToEnumValues<T> ();
        }

        public static IEnumerable<T> ToEnumValues<T> (this Type enumType)
        {
            if (enumType.BaseType != typeof (Enum)) {
                throw new ArgumentException ("T must be of type System.Enum");
            }

            Array enumValArray = Enum.GetValues (enumType);

            foreach (int val in enumValArray) {
                yield return (T)Enum.Parse (enumType, val.ToString ());
            }
        }

        public static string ToEnumDescription<T> (this T value)
        {
            Type enumType = typeof (T);

            if (enumType.BaseType != typeof (Enum)) {
                throw new ArgumentException ("T must be of type System.Enum");
            }

            FieldInfo fi = value.GetType ().GetField (value.ToString ());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes (
                    typeof (DescriptionAttribute),
                    false);

            if (attributes != null && attributes.Length > 0) {
                return attributes [0].Description;
            }
            else {
                return value.ToString ();
            }
        }

        public static T ToEnumValue<T> (this string value)
        {
            Type enumType = typeof (T);
            if (enumType.BaseType != typeof (Enum)) {
                throw new ArgumentException ("T must be of type System.Enum");
            }

            T returnValue = default (T);
            foreach (T enumVal in ToEnumValues<T>()) {
                if (enumVal.ToEnumDescription<T> () == value) {
                    returnValue = enumVal;
                    break;
                }
            }
            return returnValue;
        }
    }
}
