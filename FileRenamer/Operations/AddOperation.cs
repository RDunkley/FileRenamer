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

namespace FileRenamer.Operations
{
	/// <summary>
	///   Represents an operation to add text to the filename.
	/// </summary>
	public partial class AddOperation : BaseOperation
	{
		#region Observable Fields

		/// <summary>
		///   The offset into the name to add the string. Only used if <see cref="StartPoint"/> is set to <see cref="Location.Offset"/>.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("NewExampleName")]
		public int _characterIndex = 0;

		/// <summary>
		///   Value to begin the incrementing value with.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("NewExampleName")]
		public int _incrementValue = 0;

		/// <summary>
		///   String to be added.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("NewExampleName")]
		public string _addition = string.Empty;

		/// <summary>
		///   Determines the <see cref="AddType"/> to be used when adding text.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("NewExampleName")]
		public AddType _whichToAdd = AddType.String;

		/// <summary>
		///   Determines what location in the filename the text will be added.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("StartPoint")]
		[NotifyPropertyChangedFor("NewExampleName")]
		public int _locationSelectionValue = 0;

		#endregion

		#region Read-Only Properties

		/// <summary>
		///   Location to add the string.
		/// </summary>
		public TextLocation StartPoint
		{
			get
			{
				return (TextLocation)LocationSelectionValue;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="AddOperation"/> object.
		/// </summary>
		/// <param name="exampleName">Example name to use when configuring the settings.</param>
		public AddOperation(string exampleName)
			:base(exampleName)
		{
		}

		/// <summary>
		///   Gets a description of the operation.
		/// </summary>
		/// <returns>A description of the operation.</returns>
		public override string GetDescription()
		{
			var textToAdd = $"'{Addition}'";
			if (WhichToAdd == AddType.IncrementingNumber)
				textToAdd = $"Incrementing from {IncrementValue}";

			var additional = string.Empty;
			if (StartPoint == TextLocation.Index)
				additional = $" {CharacterIndex}";
			return $"Add {textToAdd} at {StartPoint}{additional}";
		}

		/// <summary>
		///   Gets the new name with the operation performed.
		/// </summary>
		/// <param name="name">Name to apply the operation to.</param>
		/// <param name="index">Index of the rename. May or may not be used depending on the operation settings.</param>
		/// <returns><paramref name="name"/> with the operation applied.</returns>
		public override string GetNewName(string name, int index)
		{
			var textToAdd = Addition;
			if (WhichToAdd == AddType.IncrementingNumber)
				textToAdd = $"{index + IncrementValue}";

			switch(StartPoint)
			{
				case TextLocation.Start:
					return name.Insert(0, textToAdd);
				case TextLocation.End:
					return $"{name}{textToAdd}";
				case TextLocation.Index:
					if(CharacterIndex >= name.Length)
						return $"{name}{textToAdd}";
					return name.Insert(CharacterIndex, textToAdd);
				default:
					throw new NotImplementedException();
			}
		}

		#endregion
	}
}
