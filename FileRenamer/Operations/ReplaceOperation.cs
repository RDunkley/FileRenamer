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

namespace FileRenamer.Operations
{
	/// <summary>
	///   Represents an operation to replace text in the filename.
	/// </summary>
	public partial class ReplaceOperation : BaseOperation
	{
		#region Observable Fields

		/// <summary>
		///   String to be replaced in the file name.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("NewExampleName")]
		public string _toBeReplaced = string.Empty;

		/// <summary>
		///   String to replace the text in the file name with.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("NewExampleName")]
		public string _toReplace = string.Empty;

		/// <summary>
		///   True if the case should be ignored when matching the text. False when case sensitivity should be preserved.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("NewExampleName")]
		public bool _ignoreCase = false;

		/// <summary>
		///   Determines the occurrence of the text replacement.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("NewExampleName")]
		public TextOccurrence _occurrence = TextOccurrence.First;

		#endregion

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ReplaceOperation"/> object.
		/// </summary>
		/// <param name="exampleName">Example name to use when configuring the settings.</param>
		public ReplaceOperation(string exampleName)
			:base(exampleName)
		{
		}

		/// <summary>
		///   Gets a description of the operation.
		/// </summary>
		/// <returns>A description of the operation.</returns>
		public override string GetDescription()
		{
			return $"Replace {Occurrence} '{ToBeReplaced}' with '{ToReplace}'";
		}

		/// <summary>
		///   Gets the new name with the operation performed.
		/// </summary>
		/// <param name="name">Name to apply the operation to.</param>
		/// <param name="index">Index of the rename. May or may not be used depending on the operation settings.</param>
		/// <returns><paramref name="name"/> with the operation applied.</returns>
		public override string GetNewName(string name, int index)
		{
			if (string.IsNullOrEmpty(ToBeReplaced))
				return name;
			if(Occurrence == TextOccurrence.All)
				return name.Replace(ToBeReplaced, ToReplace, IgnoreCase, null);

			int off;
			if(Occurrence == TextOccurrence.First)
				off = name.IndexOf(ToBeReplaced);
			else
				off = name.LastIndexOf(ToBeReplaced);

			if (off == -1)
				return name;

			name = name.Remove(off, ToBeReplaced.Length);
			name = name.Insert(off, ToReplace);
			return name;
		}

		#endregion
	}
}
