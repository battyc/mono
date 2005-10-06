//
// RadioButtonTest.cs
//	- Unit tests for System.Web.UI.WebControls.RadioButton
//
// Author:
//	Dick Porter  <dick@ximian.com>
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using NUnit.Framework;

namespace MonoTests.System.Web.UI.WebControls {

	public class TestRadioButton : RadioButton {
		public StateBag StateBag {
			get { return base.ViewState; }
		}

		public string Render ()
		{
			HtmlTextWriter writer = new HtmlTextWriter (new StringWriter ());
			base.Render (writer);
			return writer.InnerWriter.ToString ();
		}
	}

	[TestFixture]
	public class RadioButtonTest {

		[Test]
		public void DefaultProperties ()
		{
			TestRadioButton r = new TestRadioButton ();
			
			Assert.AreEqual (0, r.Attributes.Count, "Attributes.Count");

			Assert.IsFalse (r.AutoPostBack, "AutoPostBack");
			Assert.IsFalse (r.Checked, "Checked");
			Assert.AreEqual (String.Empty, r.Text, "Text");
			Assert.AreEqual (TextAlign.Right, r.TextAlign, "TextAlign");
			Assert.AreEqual (String.Empty, r.GroupName, "GroupName");
			
			Assert.AreEqual (0, r.Attributes.Count, "Attributes.Count-2");
		}

		[Test]
		public void NullProperties ()
		{
			TestRadioButton r = new TestRadioButton ();
			
			r.Text = null;
			Assert.AreEqual (String.Empty, r.Text, "Text");
			r.TextAlign = TextAlign.Right;
			Assert.AreEqual (TextAlign.Right, r.TextAlign, "TextAlign");
			r.AutoPostBack = true;
			Assert.IsTrue (r.AutoPostBack, "AutoPostBack");
			r.Checked = true;
			Assert.IsTrue (r.Checked, "Checked");
			r.GroupName = null;
			Assert.AreEqual (String.Empty, r.GroupName, "GroupName");
			
			Assert.AreEqual (0, r.Attributes.Count, "Attributes.Count");
			Assert.AreEqual (3, r.StateBag.Count, "ViewState.Count-1");
		}

		[Test]
		public void CleanProperties ()
		{
			TestRadioButton r = new TestRadioButton ();

			r.Text = "text";
			Assert.AreEqual ("text", r.Text, "Text");
			r.AutoPostBack = true;
			r.TextAlign = TextAlign.Left;
			r.Checked = true;
			r.GroupName = "groupname";
			Assert.AreEqual ("groupname", r.GroupName, "GroupName");
			
			Assert.AreEqual (5, r.StateBag.Count, "ViewState.Count");
			Assert.AreEqual (0, r.Attributes.Count, "Attributes.Count");

			r.Text = null;
			r.AutoPostBack = false;
			r.TextAlign = TextAlign.Right;
			r.Checked = false;
			r.GroupName = null;
			
			// If Text is null it is removed from the
			// ViewState.  Ditto GroupName
			Assert.AreEqual (3, r.StateBag.Count, "ViewState.Count-2");
			Assert.AreEqual (TextAlign.Right, r.StateBag["TextAlign"], "TextAlign");
			Assert.AreEqual (0, r.Attributes.Count, "Attributes.Count-2");
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void TextAlign_Invalid ()
		{
			RadioButton r = new RadioButton ();
			r.TextAlign = (TextAlign)Int32.MinValue;
		}

		[Test]
		public void TextAlign_Values ()
		{
			RadioButton r = new RadioButton ();

			foreach (TextAlign ta in Enum.GetValues (typeof (TextAlign))) {
				r.TextAlign = ta;
			}
		}
		
		/* Segfaults on ms runtime */
		[Test]
		[Category ("NotDotNet")]
		public void Render ()
		{
			TestRadioButton r = new TestRadioButton ();

			string s = r.Render ();

			Assert.IsTrue (s.IndexOf (" type=\"radio\"") > 0, "type");

			r.Text = "label text";
			s = r.Render ();
			Assert.IsTrue (s.IndexOf (">label text</label>") > 0, "text");

			r.TextAlign = TextAlign.Left;
			s = r.Render ();
			Assert.IsTrue (s.IndexOf (">label text</label><input") > 0, "text left");
			r.TextAlign = TextAlign.Right;
			s = r.Render ();
			Assert.IsTrue (s.IndexOf ("/><label for") > 0, "text right");
		}
	}
}
