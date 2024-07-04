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
namespace FileRenamer.Data
{
	/// <summary>
	///   Defines the interface for a renaming operation.
	/// </summary>
	public interface IRenameOperation
	{
		/// <summary>
		///   Gets the newly computed example name.
		/// </summary>
		string NewExampleName { get; }

		/// <summary>
		///   Updates the input example name or the source example name (not <see cref="NewExampleName"/>.
		/// </summary>
		/// <param name="exampleName">Example name to now use.</param>
		void UpdateExampleName(string exampleName);

		/// <summary>
		///   Gets a description of the operation.
		/// </summary>
		/// <returns>A description of the operation.</returns>
		string GetDescription();

		/// <summary>
		///   Gets the new name with the operation performed.
		/// </summary>
		/// <param name="name">Name to apply the operation to.</param>
		/// <param name="index">Index of the rename. May or may not be used depending on the operation settings.</param>
		/// <returns><paramref name="name"/> with the operation applied.</returns>
		string GetNewName(string name, int index);
	}
}
