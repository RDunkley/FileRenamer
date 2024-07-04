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
using FileRenamer.Data;
using FileRenamer.ViewModels;

namespace FileRenamer.Operations
{
	/// <summary>
	///   Represents a base operation to perform on a filename.
	/// </summary>
	public abstract class BaseOperation : ViewModelBase, IRenameOperation
	{
		#region Fields

		/// <summary>
		///   Stores the example file name to display to the user as the operation is created.
		/// </summary>
		private string _exampleName;

		#endregion

		#region Read-Only Properties

		/// <summary>
		///   Gets the new name of <see cref="_exampleName"/> according to the operation settings.
		/// </summary>
		public string NewExampleName
		{
			get
			{
				return GetNewName(_exampleName, 0);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="BaseOperation"/> instance using the provided example name.
		/// </summary>
		/// <param name="exampleName">Example name to use while configuring the operation.</param>
		public BaseOperation(string exampleName)
		{
			_exampleName = exampleName;
		}

		/// <summary>
		///   Updates the example name used while configuring the operation.
		/// </summary>
		/// <param name="exampleName">New name to be used.</param>
		public void UpdateExampleName(string exampleName)
		{
			_exampleName = exampleName;
		}

		/// <summary>
		///   Gets a description of the operation.
		/// </summary>
		/// <returns>A description of the operation.</returns>
		public abstract string GetDescription();

		/// <summary>
		///   Gets the new name with the operation performed.
		/// </summary>
		/// <param name="name">Name to apply the operation to.</param>
		/// <param name="index">Index of the rename. May or may not be used depending on the operation settings.</param>
		/// <returns><paramref name="name"/> with the operation applied.</returns>
		public abstract string GetNewName(string name, int index);

		#endregion

	}
}
