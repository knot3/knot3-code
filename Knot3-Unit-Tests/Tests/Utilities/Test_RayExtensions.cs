﻿using System;
using System.Text;
using System.Collections.Generic;

namespace Knot3.UnitTests.Tests.Utilities
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für Test_RayExtensions
	/// </summary>
	[TestFixture]
	public class Test_RayExtensions
	{
		public Test_RayExtensions()
		{
			//
			// TODO: Konstruktorlogik hier hinzufügen
			//
		}

		private TestContext testContextInstance;

		/// <summary>
		///Ruft den Textkontext mit Informationen über
		///den aktuellen Testlauf sowie Funktionalität für diesen auf oder legt diese fest.
		///</summary>
		public TestContext TestContext
		{
			get {
				return testContextInstance;
			}
			set {
				testContextInstance = value;
			}
		}

		#region Zusätzliche Testattribute
		//
		// Sie können beim Schreiben der Tests folgende zusätzliche Attribute verwenden:
		//
		// Verwenden Sie ClassInitialize, um vor Ausführung des ersten Tests in der Klasse Code auszuführen.
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Verwenden Sie ClassCleanup, um nach Ausführung aller Tests in einer Klasse Code auszuführen.
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Mit TestInitialize können Sie vor jedem einzelnen Test Code ausführen.
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Mit TestCleanup können Sie nach jedem einzelnen Test Code ausführen.
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[Test]
		public void Test1()
		{
			//
			// TODO: Testlogik hier hinzufügen
			//
		}
	}
}
