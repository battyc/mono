//
// Tests for System.Web.UI.WebControls.ListBoxTest.cs
//
// Author:
//	Jackson Harper (jackson@ximian.com)
//

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

using NUnit.Framework;
using System;
using System.IO;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace MonoTests.System.Web.UI.WebControls
{
	class ListBoxPoker : ListBox {

		public ListBoxPoker ()
		{
			TrackViewState ();
		}
		
		public object SaveState ()
		{
			return SaveViewState ();
		}

		public void LoadState (object o)
		{
			LoadViewState (o);
		}

		public StateBag _ViewState {
			get { return ViewState; }
		}

					
		public string Render ()
		{
			StringWriter sw = new StringWriter ();
			sw.NewLine = "\n";
			HtmlTextWriter writer = new HtmlTextWriter (sw);
			base.Render (writer);
			return writer.InnerWriter.ToString ();
		}
	}

	[TestFixture]	
	public class ListBoxTest {

		[Test]
		public void Defaults ()
		{
			ListBox lb = new ListBox ();

			Assert.AreEqual (lb.BorderColor, Color.Empty, "A1");
			Assert.AreEqual (lb.BorderStyle, BorderStyle.NotSet, "A2");
			Assert.AreEqual (lb.BorderWidth, Unit.Empty, "A3");
			Assert.AreEqual (lb.Rows, 4, "A4");
			Assert.AreEqual (lb.SelectionMode, ListSelectionMode.Single, "A5");
			Assert.AreEqual (lb.ToolTip, String.Empty, "A6");
		}

		[Test]
		public void SetProps ()
		{
			ListBox lb = new ListBox ();

			lb.BorderColor = Color.Black;
			Assert.AreEqual (lb.BorderColor, Color.Black, "A1");

			lb.BorderStyle = BorderStyle.Dashed;
			Assert.AreEqual (lb.BorderStyle, BorderStyle.Dashed, "A2");

			lb.BorderWidth = 0;
			Assert.AreEqual (lb.BorderWidth, (Unit) 0, "A3");

			lb.BorderWidth = 15;
			Assert.AreEqual (lb.BorderWidth, (Unit) 15, "A3");

			lb.Rows = 1;
			Assert.AreEqual (lb.Rows, 1, "A4");

			lb.SelectionMode = ListSelectionMode.Multiple;
			Assert.AreEqual (lb.SelectionMode, ListSelectionMode.Multiple, "A6");

			lb.ToolTip = "foo";
#if NET_2_0
			Assert.AreEqual (lb.ToolTip, "foo", "A7");
#else
			Assert.AreEqual (lb.ToolTip, String.Empty, "A7"); // Always empty in 1.x
#endif
		}

		[Test]
#if !NET_2_0
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
#endif
		public void RowsTooHigh ()
		{
			ListBox lb = new ListBox ();
			lb.Rows = 2001;
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void RowsTooLow ()
		{
			ListBox lb = new ListBox ();
			lb.Rows = 0;
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void BadSelectionMode ()
		{
			ListBox lb = new ListBox ();
			lb.SelectionMode = (ListSelectionMode) 500;
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void BadBorderStyle ()
		{
			ListBox lb = new ListBox ();
			lb.BorderStyle = (BorderStyle) 500;
		}

		[Test]
		public void ViewState ()
		{
			ListBoxPoker p = new ListBoxPoker ();

			p.BorderColor = Color.Red;
			Assert.AreEqual (p._ViewState ["BorderColor"],
					Color.Red, "A1");

			p.BorderStyle = BorderStyle.Double;
			Assert.AreEqual (p._ViewState ["BorderStyle"],
					BorderStyle.Double, "A2");

			p.BorderWidth = 25;
			Assert.AreEqual (p._ViewState ["BorderWidth"],
					(Unit) 25, "A3");

			p.SelectionMode = ListSelectionMode.Multiple;
			Assert.AreEqual (p._ViewState ["SelectionMode"],
					ListSelectionMode.Multiple, "A4");
		}

		[Test]
		public void Render1 ()
		{
			ListBoxPoker l = new ListBoxPoker ();
			for (int i = 0; i < 3; i ++)
				l.Items.Add (i.ToString ());

			l.SelectedIndex = l.Items.Count - 1;
#if NET_2_0
			string exp = @"<select size=""4"">
	<option value=""0"">0</option>
	<option value=""1"">1</option>
	<option selected=""selected"" value=""2"">2</option>

</select>";
#else
			string exp = @"<select name size=""4"">
	<option value=""0"">0</option>
	<option value=""1"">1</option>
	<option selected=""selected"" value=""2"">2</option>

</select>";
#endif
			Assert.AreEqual (exp, l.Render ());
		}

		DataSet GetExampleData ()
		{
			DataSet ds = new DataSet ();
			ds.ReadXml (new StringReader (@"
<DataSet>
	<Stocks Company='Novell Inc.'     Symbol='NOVL' Price='6.14'   />
	<Stocks Company='Microsoft Corp.' Symbol='MSFT' Price='25.92'  />
	<Stocks Company='Google'          Symbol='GOOG' Price='291.60' />
</DataSet>
"));
			return ds;
		}
		
		[Test]
		public void DoubleDataBind ()
		{
			ListBoxPoker l = new ListBoxPoker ();
			l.DataSource = GetExampleData ();
			l.DataTextField = "Company";
			l.DataBind ();
			l.DataBind ();
#if NET_2_0
			string exp = @"<select size=""4"">
	<option value=""Novell Inc."">Novell Inc.</option>
	<option value=""Microsoft Corp."">Microsoft Corp.</option>
	<option value=""Google"">Google</option>

</select>";
#else
			string exp = @"<select name size=""4"">
	<option value=""Novell Inc."">Novell Inc.</option>
	<option value=""Microsoft Corp."">Microsoft Corp.</option>
	<option value=""Google"">Google</option>

</select>";
#endif
			Assert.AreEqual (exp, l.Render ());
		}
	}
}

