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
	///   Represents an operation to remove text from the filename.
	/// </summary>
	public partial class RemoveOperation : BaseOperation
	{
		#region Observable Fields

		/// <summary>
		///   The offset into the name to remove characters from. Only used if <see cref="StartPoint"/> is set to <see cref="Location.Offset"/>.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("NewExampleName")]
		public int _characterIndex = 0;

		/// <summary>
		///   Number of characters to be removed. Only used if <see cref="WhichToRemove"/> is <see cref="RemoveType.NumberOfCharacters"/>.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("NewExampleName")]
		public int _numToRemove = 1;

		/// <summary>
		///   String to be removed. Only used if <see cref="WhichToRemove"/> is <see cref="RemoveType.String"/>.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("NewExampleName")]
		public string _textToRemove = string.Empty;

		/// <summary>
		///   Determines what text should be removed.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("CharacterIndexEnabled")]
		[NotifyPropertyChangedFor("NewExampleName")]
		public RemoveType _whichToRemove = RemoveType.String;

		/// <summary>
		///   Determines the occurrence of the removed text.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("NewExampleName")]
		public TextOccurrence _occurrence = TextOccurrence.First;

		/// <summary>
		///   Determines what location in the filename the text will be removed from. Only used if <see cref="WhichToRemove"/> is <see cref="RemoveType.NumberOfCharacters"/>.
		/// </summary>
		[ObservableProperty]
		[NotifyPropertyChangedFor("StartPoint")]
		[NotifyPropertyChangedFor("CharacterIndexEnabled")]
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

		/// <summary>
		///   True if character index should be enabled.
		/// </summary>
		public bool CharacterIndexEnabled
		{
			get
			{
				// TODO: This property could be replaced by multi-binding on the GUI side since it isn't really relavant here. - RD
				if (WhichToRemove != RemoveType.NumberOfCharacters)
					return false;
				if (StartPoint != TextLocation.Index)
					return false;
				return true;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="RemoveOperation"/> object.
		/// </summary>
		/// <param name="exampleName">Example name to use when configuring the settings.</param>
		public RemoveOperation(string exampleName)
			: base(exampleName) {
			
		}

		/// <summary>
		///   Gets the location of the string.
		/// </summary>
		/// <returns></returns>
		private string GetLocationString()
		{
			if (StartPoint == TextLocation.Start)
				return "Start";
			if (StartPoint == TextLocation.End)
				return "End";
			return $"Offset {CharacterIndex}";
		}

		/// <summary>
		///   Gets a description of the operation.
		/// </summary>
		/// <returns>A description of the operation.</returns>
		public override string GetDescription()
		{
			if(WhichToRemove == RemoveType.String)
				return $"Remove {Occurrence} '{TextToRemove}'";
			return $"Remove {NumToRemove} Characters from {GetLocationString()}.";
		}

		/// <summary>
		///   Gets the new name with the operation performed.
		/// </summary>
		/// <param name="name">Name to apply the operation to.</param>
		/// <param name="index">Index of the rename. May or may not be used depending on the operation settings.</param>
		/// <returns><paramref name="name"/> with the operation applied.</returns>
		public override string GetNewName(string name, int index)
		{
			if(WhichToRemove == RemoveType.String)
			{
				int place = -1;
				switch(Occurrence)
				{
					case TextOccurrence.First:
						place = name.IndexOf(TextToRemove);
						if (place == -1)
							return name;
						return name.Remove(place, TextToRemove.Length);
					case TextOccurrence.Last:
						place = name.LastIndexOf(TextToRemove);
						if (place == -1)
							return name;
						return name.Remove(place, TextToRemove.Length);
					case TextOccurrence.All:
						return name.Replace(TextToRemove, string.Empty);
					default:
						throw new NotImplementedException();
				}
			}

			if (NumToRemove > name.Length)
				return string.Empty;

			switch (StartPoint)
			{
				case TextLocation.Start:
					return name.Remove(0, NumToRemove);
				case TextLocation.End:
					return name.Remove(name.Length - NumToRemove, NumToRemove);
				case TextLocation.Index:
					if (CharacterIndex >= name.Length)
						return name.Remove(name.Length - NumToRemove, NumToRemove);
					return name.Remove(CharacterIndex, NumToRemove);
				default:
					throw new NotImplementedException();
			}
		}

		#endregion
	}
}
