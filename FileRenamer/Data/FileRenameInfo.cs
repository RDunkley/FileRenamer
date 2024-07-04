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
using System;
using System.ComponentModel;
using System.IO;
using System.Security;

namespace FileRenamer.Data
{
	/// <summary>
	///   Stores information about a file found in the selected folder.
	/// </summary>
	public class FileRenameInfo : INotifyPropertyChanged
	{
		private string _newFileName;
		private string _filePath;
		private bool _selected;

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///   Gets or sets whether the current file is selected for rename or not.
		/// </summary>
		/// <remarks>Setting this property to a different value triggers the <see cref="PropertyChanged"/> event.</remarks>
		public bool Selected
		{
			get
			{
				return _selected;
			}
			set
			{
				if(_selected != value)
				{
					_selected = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Selected"));
				}
			}
		}

		/// <summary>
		///   Gets or sets the current file name on disk.
		/// </summary>
		public string CurrentFileName
		{
			get
			{
				return Path.GetFileNameWithoutExtension(_filePath);
			}
		}

		/// <summary>
		///   Gets or sets the new file name which the file will be changed to when <see cref="Rename"/> is called.
		/// </summary>
		/// <remarks>Setting this property to a different value triggers the <see cref="PropertyChanged"/> event.</remarks>
		public string NewFileName
		{
			get
			{
				return _newFileName;
			}
			set
			{
				if(_newFileName != value)
				{
					_newFileName = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NewFileName"));
				}
			}
		}

		/// <summary>
		///   Creates a new <see cref="FileRenameInfo"/> object from the specified file path.
		/// </summary>
		/// <param name="filePath">Full path to the file this object represents.</param>
		/// <exception cref="ArgumentNullException"><paramref name="filePath"/> is null or empty.</exception>
		/// <exception cref="ArgumentException"><paramref name="filePath"/> is not valid.</exception>
		public FileRenameInfo(string filePath)
		{
			if(string.IsNullOrEmpty(filePath))
				throw new ArgumentNullException(nameof(filePath));

			try
			{
				_filePath = Path.GetFullPath(filePath);
			}
			catch(Exception ex) when (ex is SecurityException || ex is NotSupportedException || ex is PathTooLongException)
			{
				throw new ArgumentException($"The file path provided ({filePath}) is not valid. {ex.Message}.", ex);
			}

			_newFileName = CurrentFileName;
			_selected = true;
		}

		/// <summary>
		///   Renames the file from <see cref="CurrentFileName"/> to <see cref="NewFileName"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException"><see cref="NewFileName"/> is empty or null.</exception>
		public void Rename()
		{
			if (string.IsNullOrEmpty(_newFileName))
				throw new InvalidOperationException($"The new file name is empty or null.");

			var extension = Path.GetExtension(_filePath);
			var folder = Path.GetDirectoryName(_filePath);
			var newFileName = Path.Combine(folder, $"{_newFileName}{extension}");
			File.Move(_filePath, newFileName);

			// Update to new file name.
			_newFileName = CurrentFileName;
			_filePath = newFileName;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentFileName"));
		}
	}
}
