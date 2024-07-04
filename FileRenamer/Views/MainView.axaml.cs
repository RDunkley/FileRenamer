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
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using FileRenamer.Data;
using FileRenamer.Operations;
using FileRenamer.ViewModels;
using System;
using System.Collections.Generic;

namespace FileRenamer.Views;

/// <summary>
///   Code behind for the main view.
/// </summary>
public partial class MainView : UserControl
{
	private MainViewModel _view;

	public MainView()
	{
		InitializeComponent();

		OpSplitView.PaneClosed += OpSplitView_PaneClosed;
	}

	protected override void OnDataContextChanged(EventArgs e)
	{
		base.OnDataContextChanged(e);
		_view = DataContext as MainViewModel;
	}

	private async void OpenFolderButton_Clicked(object sender, RoutedEventArgs e)
	{
		var topLevel = TopLevel.GetTopLevel(this);

		var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions
		{
			Title = "Open Folder Containing Files to Rename",
			AllowMultiple = false,
		});

		if (folders.Count != 1)
			return;

		_view.Folder = folders[0].Path.LocalPath;
	}

	/// <summary>
	///   Uses the <see cref="FileGrid"/> to determine the index of the file that should be used as an example.
	/// </summary>
	/// <returns>Index of the file or -1 if no selected file could be found.</returns>
	private int GetExampleTextFromIndex()
	{
		if (FileGrid.SelectedIndex != -1 && _view.Files[FileGrid.SelectedIndex].Selected == true)
			return FileGrid.SelectedIndex;
		else if (_view.Files.Count > 0)
		{
			for (int i = 0; i < _view.Files.Count; i++)
			{
				if (_view.Files[i].Selected == true)
					return i;
			}
		}
		return -1;
	}

	/// <summary>
	///   Gets example file name to use when configuring an operation.
	/// </summary>
	/// <param name="op">If null then all the operations are applied to the selected name. If non-null then only the operations up to this operation are used.</param>
	/// <returns>Example file name or an empty string if none was found.</returns>
	private string GetExampleText(IRenameOperation op)
	{
		// Determine the example text to put in.
		int index = GetExampleTextFromIndex();
		if (index == -1)
			return string.Empty;

		// If op is null then return the new file name because we are using all ops.
		if (op == null)
			return _view.Files[index].NewFileName;

		// Run all previous ops on the current file name.
		var name = _view.Files[index].CurrentFileName;
		foreach(var opItem in _view.Renames)
		{
			if (opItem.Operation == op)
				return name;
			else
				name = opItem.Operation.GetNewName(name, 0);
		}
		return name;
	}

	private async void OpAddButton_Click(object sender, RoutedEventArgs e)
	{
		var text = GetExampleText(null);

		var window = new OperationWindow(new AddOperation(text), text);
		window.Height = 450;
		window.Width = 800;
		var result = await window.ShowDialog<DialogResult>(this.GetVisualRoot() as Window);
		if(result == DialogResult.Ok)
		{
			IRenameOperation op = window.GetOperation();
			_view.Renames.Add(new OperationListItem(op));
			_view.UpdateNames();
		}
	}

	private void OpRemoveButton_Click(object sender, RoutedEventArgs e)
	{
		var list = new List<OperationListItem>();
		foreach(OperationListItem listItem in OpList.SelectedItems)
			list.Add(listItem);

		// Remove all the items.
		foreach (var listItem in list)
			_view.Renames.Remove(listItem);
		_view.UpdateNames();
	}

	private void OpUpButton_Click(object sender, RoutedEventArgs e)
	{
		int index = OpList.SelectedIndex;
		if (index < 1) return;

		var item = _view.Renames[index];
		_view.Renames.RemoveAt(index);
		_view.Renames.Insert(index - 1, item);
		OpList.SelectedIndex = index - 1;
		_view.UpdateNames();
	}

	private void OpDownButton_Click(object sender, RoutedEventArgs e)
	{
		int index = OpList.SelectedIndex;
		if (index < 0) return;
		if (index == _view.Renames.Count-1) return;

		var item = _view.Renames[index];
		_view.Renames.RemoveAt(index);
		_view.Renames.Insert(index + 1, item);
		OpList.SelectedIndex = index + 1;
		_view.UpdateNames();
	}

	private void OpClearButton_Click(object sender, RoutedEventArgs e)
	{
		_view.Renames.Clear();
		_view.UpdateNames();
	}

	private async void OpEditButton_Click(object sender, RoutedEventArgs e)
	{
		int index = OpList.SelectedIndex;
		if (index < 0) return;

		var text = GetExampleText(_view.Renames[index].Operation);

		var item = _view.Renames[index];
		item.Operation.UpdateExampleName(text);
		var window = new OperationWindow(item.Operation, text);
		window.Height = 450;
		window.Width = 800;
		var result = await window.ShowDialog<DialogResult>(this.GetVisualRoot() as Window);
		if(result == DialogResult.Ok)
		{
			IRenameOperation op = window.GetOperation();
			_view.Renames[index] = new OperationListItem(op);
			_view.UpdateNames();
		}
	}

	private void RefreshFolderButton_Clicked(object sender, RoutedEventArgs e)
	{
		_view.RefreshFolder();
	}

	private void OpSplitView_PaneClosed(object sender, RoutedEventArgs e)
	{
		OpPanelControlButton.Content = "<";
	}

	private void OpPanelControlButton_Clicked(object sender, RoutedEventArgs e)
	{
		if(OpPanelControlButton.Content.ToString() == "<")
		{
			// Open the panel.
			OpSplitView.IsPaneOpen = true;
			OpPanelControlButton.Content = ">";
		}
		else
		{
			// Close the panel.
			OpSplitView.IsPaneOpen = false;
		}
	}
}
