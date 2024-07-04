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
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace FileRenamer.ViewModels;

/// <summary>
///   Main View-Model for the application.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
	#region ObservableProperties

	/// <summary>
	///   Stores the folder that is currently being targetted.
	/// </summary>
	[ObservableProperty]
	public string _folder = Environment.CurrentDirectory;

	/// <summary>
	///   True if the files can be renamed, false if they can't (an error condition will occur).
	/// </summary>
	[ObservableProperty]
	public bool _canRename = false;

	/// <summary>
	///   Collection of the files found in the <see cref="Folder"/> and their rename state.
	/// </summary>
	public ObservableCollection<FileRenameInfo> Files { get; } = new ObservableCollection<FileRenameInfo>();

	/// <summary>
	///   Collection of rename operations.
	/// </summary>
	public ObservableCollection<OperationListItem> Renames { get; } = new ObservableCollection<OperationListItem>();

	#endregion

	#region Overloads

	/// <summary>
	///   Called when <see cref="Folder"/> is changed.
	/// </summary>
	/// <param name="value">New value.</param>
	partial void OnFolderChanged(string value)
	{
		RefreshFolder();
	}

	#endregion

	#region Methods

	/// <summary>
	///   Instantiates a new <see cref="MainViewModel"/> object.
	/// </summary>
	public MainViewModel()
	{
	}

	/// <summary>
	///   Refreshes the currently selected folder. This can be used if files have changed on the file system.
	/// </summary>
	public void RefreshFolder()
	{
		Files.Clear();

		if (Folder == null)
			return;

		if (!Directory.Exists(Folder))
			return;

		var files = Directory.GetFiles(Folder);
		foreach(var file in files)
			Files.Add(new FileRenameInfo(file));

		UpdateNames();
	}

	/// <summary>
	///   Updates the computed new names of each file. This should be called when changes to the operation list have occurred.
	/// </summary>
	public void UpdateNames()
	{
		bool canRename = true;
		if (Renames.Count == 0)
			canRename = false;

		var newNames = new List<string>(Files.Count);
		bool selected = false;
		int index = 0;
		for(int i = 0; i < Files.Count; i++)
		{
			if (Files[i].Selected)
			{
				selected = true;
				var name = Files[i].CurrentFileName;
				foreach(var op in Renames)
					name = op.Operation.GetNewName(name, index);
				Files[i].NewFileName = name;
				if (string.IsNullOrEmpty(name))
					canRename = false;
				if (newNames.Contains(name.ToLower()))
					canRename = false;
				else
					newNames.Add(name.ToLower());
				index++;
			}
		}

		if (!selected)
			canRename = false;
		CanRename = canRename;
	}

	/// <summary>
	///   Performs renaming of the files after checking that it is able to do so.
	/// </summary>
	/// <exception cref="InvalidOperationException">An attempt was made to rename when the new names are not all correct file names.</exception>
	public void RenameFiles()
	{
		// Check for null or duplicate new names.
		var newNames = new List<string>(Files.Count);
		foreach (var file in Files)
		{
			if (file.Selected)
			{
				if (file.NewFileName == null)
					throw new InvalidOperationException($"The new name for file {file.CurrentFileName} is null.");

				var newName = file.NewFileName.ToLower();
				if (newNames.Contains(newName))
					throw new InvalidOperationException($"The new name for file {file.CurrentFileName} matches another file's new name. The new names must be unique (case insensitive) to perform any renames.");
				newNames.Add(newName);
			}
		}

		// TODO: Add a check here for names that will correspond with non-selected actual names. - RD

		// Perform the renames.
		foreach(var file in Files)
		{
			if (file.Selected)
				file.Rename();
		}

		UpdateNames();
	}

	#endregion
}
