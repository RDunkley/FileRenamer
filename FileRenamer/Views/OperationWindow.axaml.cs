// ******************************************************************************************************************************
// Copyright © Richard Dunkley 2024
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ******************************************************************************************************************************
using Avalonia.Controls;
using Avalonia.Interactivity;
using FileRenamer.Data;
using FileRenamer.Operations;

namespace FileRenamer.Views
{
	/// <summary>
	///   Code behind for the operation window.
	/// </summary>
	public partial class OperationWindow : Window
	{
		public AddOperation _add;
		public RemoveOperation _remove;
		public ReplaceOperation _replace;

		public bool WasCancelled { get { return CancelButton.IsCancel; } }

		public OperationWindow()
		{
			InitializeComponent();

			CancelButton.Click += CancelButton_Click;
			OkButton.Click += OkButton_Click;
		}

		public OperationWindow(IRenameOperation operation, string exampleName)
			: this()
		{
			OpTabControl.SelectedIndex = 0;
			if(operation is AddOperation)
			{
				_add = operation as AddOperation;
				OpTabControl.SelectedIndex = 0;
			}
			else
			{
				_add = new AddOperation(exampleName);
			}
			AddControl.DataContext = _add;

			if(operation is RemoveOperation)
			{
				_remove = operation as RemoveOperation;
				OpTabControl.SelectedIndex = 1;
			}
			else
			{
				_remove = new RemoveOperation(exampleName);
			}
			RemoveControl.DataContext = _remove;

			if(operation is ReplaceOperation)
			{
				_replace = operation as ReplaceOperation;
				OpTabControl.SelectedIndex = 2;
			}
			else
			{
				_replace = new ReplaceOperation(exampleName);
			}
			ReplaceControl.DataContext = _replace;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close(DialogResult.Cancel);
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close(DialogResult.Ok);
		}

		public IRenameOperation GetOperation()
		{
			if (OpTabControl.SelectedIndex == 0)
				return _add;
			if (OpTabControl.SelectedIndex == 1)
				return _remove;
			return _replace;
		}
	}
}
