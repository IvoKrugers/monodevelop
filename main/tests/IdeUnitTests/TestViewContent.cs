//
// TestViewContent.cs
//
// Author:
//   Mike Krüger <mkrueger@novell.com>
//
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
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
using System.Linq;
using System.Collections.Generic;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Editor;
using MonoDevelop.Core.Text;
using System.Threading.Tasks;
using MonoDevelop.Ide.Gui.Documents;
using Microsoft.VisualStudio.Text;

namespace MonoDevelop.Ide.Gui
{
	public class TestViewContent : FileDocumentController
	{
		TextEditor data;
		
		public TextEditor Editor {
			get {
				return this.data;
			}
		}
		public TestViewContent ()
		{
			data = TextEditorFactory.CreateNewEditor ();
			AddContent (data);
			Name = "";
		}

		public TestViewContent (IReadonlyTextDocument doc)
		{
			data = TextEditorFactory.CreateNewEditor (doc);
			AddContent (data);
			Name = "";
		}

		protected override Task OnInitialize (ModelDescriptor modelDescriptor, Properties status)
		{
			if (modelDescriptor is FileDescriptor file && !file.FilePath.IsNullOrEmpty)
				data.FileName = file.FilePath;
			return base.OnInitialize (modelDescriptor, status);
		}

		protected override void OnDispose ()
		{
			base.OnDispose ();
			data.Dispose ();
		}

		FilePath name;
		public FilePath Name { 
			get { return name; }
			set { name =  data.FileName = value; }
		}
		
		public int LineCount {
			get {
				return data.LineCount;
			}
		}

		public string Text {
			get {
				return data.Text;
			}
			set {
				data.Text = value;
			}
		}
		
		public int InsertText (int position, string text)
		{
			data.InsertText (position, text);
			return text.Length;
		}
		
		public void DeleteText (int position, int length)
		{
			data.ReplaceText (position, length, "");
		}
		
		public int Length {
			get {
				return data.Length;
			}
		}
		
		public string GetText (int startPosition, int endPosition)
		{
			return data.GetTextBetween (startPosition, endPosition);
		}
		public char GetCharAt (int position)
		{
			return data.GetCharAt (position);
		}
		
		public int GetPositionFromLineColumn (int line, int column)
		{
			return data.LocationToOffset (line, column);
		}
		
		public void GetLineColumnFromPosition (int position, out int line, out int column)
		{
			var loc = data.OffsetToLocation (position);
			line = loc.Line;
			column = loc.Column;
		}
		
		public string SelectedText { get { return ""; } set { } }
		
		public int CursorPosition {
			get {
				return data.CaretOffset;
			}
			set {
				data.CaretOffset = value;
			}
		}

		public int SelectionStartPosition { 
			get {
				if (!data.IsSomethingSelected)
					return data.CaretOffset;
				return data.SelectionRange.Offset;
			}
		}
		
		public int SelectionEndPosition { 
			get {
				if (!data.IsSomethingSelected)
					return data.CaretOffset;
				return data.SelectionRange.EndOffset;
			}
		}
		
		public void Select (int startPosition, int endPosition)
		{
			data.SelectionRange = TextSegment.FromBounds (startPosition, endPosition);
		}
		
		public void ShowPosition (int position)
		{
		}
		
		public bool EnableUndo {
			get {
				return false;
			}
		}
		public bool EnableRedo {
			get {
				return false;
			}
		}
		
		public void SetCaretTo (int line, int column)
		{
		}
		public void SetCaretTo (int line, int column, bool highlightCaretLine)
		{
		}
		public void SetCaretTo (int line, int column, bool highlightCaretLine, bool centerCaret)
		{
		}
		
		public void Undo()
		{
		}
		public void Redo()
		{
		}
		
		class DisposeStub : IDisposable
		{
			public void Dispose ()
			{
			}
		}
		
		List<object> contents = new List<object> ();

		public void AddContent (object content)
		{
			contents.Add (content);
			NotifyContentChanged ();
		}

		protected override IEnumerable<object> OnGetContents (Type type)
		{
			if (type == typeof (ITextBuffer)) {
				yield return data.TextView.TextBuffer;
				yield break;
			}

			foreach (var c in data.GetContents (type))
				yield return c;

			foreach (var content in base.OnGetContents (type))
				yield return content;

			foreach (var content in contents)
				if (type.IsInstanceOfType (content))
					yield return content;
		}

		public IDisposable OpenUndoGroup ()
		{
			return new DisposeStub ();
		}
		
		public TextEditor GetTextEditorData ()
		{
			return data;
		}

		#region IEditableTextBuffer implementation
		public bool HasInputFocus {
			get {
				return false;
			}
		}
		
		public void RunWhenLoaded (System.Action action)
		{
			action ();
		}
		#endregion

		#pragma warning disable 67 // never used
		public event EventHandler CaretPositionSet;
		#pragma warning restore 67
	}
}
