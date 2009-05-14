/*
Copyright 2009 Len Popp

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DaveyBot
{
	/// <summary>
	/// Utility functions for object creation and type reflection.
	/// </summary>
	class ObjectUtils
	{
		/// <summary>
		/// Create an object whose class is specified by the given Settings property.
		/// </summary>
		/// <param name="stSetting">Name of the Settings property that specifies the class name</param>
		/// <param name="typeApproxClass">The desired object's base class, or some other class in
		/// the same assembly.</param>
		/// <returns>An object of the specified class</returns>
		static public object CreateObjectFromSettings(string stSetting, Type typeApproxClass)
		{
			return CreateObject((string)Properties.Settings.Default[stSetting], typeApproxClass);
		}

		/// <summary>
		/// Create an object of the given class.
		/// </summary>
		/// <param name="stClass">Name of the object's class</param>
		/// <param name="typeApproxClass">The desired object's base class, or some other class in
		/// the same assembly.</param>
		/// <returns>An object of the specified class</returns>
		static public object CreateObject(string stClass, Type typeApproxClass)
		{
			object o = Assembly.GetAssembly(typeApproxClass).CreateInstance(stClass);
			if (o == null)
				throw new ApplicationException("Failed to create object of type " + stClass);
			return o;
		}

		/// <summary>
		/// Return all the subclasses of the given class.
		/// </summary>
		/// <remarks>
		/// Is there really no built-in way to do this???
		/// </remarks>
		/// <param name="typeBase">Base class</param>
		/// <returns>Array of subclasses</returns>
		static public Type[] GetSubclasses(Type typeBase)
		{
			Assembly assembly = Assembly.GetAssembly(typeBase);
			Type[] types = assembly.GetTypes();
			List<Type> subclasses = new List<Type>();
			foreach (Type type in types)
			{
				if (type.IsSubclassOf(typeBase))
					subclasses.Add(type);
			}
			return subclasses.ToArray();
		}

		/// <summary>
		/// Fill a list with the names of all the subclasses of a given class.
		/// </summary>
		/// <remarks>
		/// Useful for filling a listbox in a dialog.
		/// </remarks>
		/// <param name="typeBase">Base class</param>
		/// <param name="listSubclassNames">List to contain the subclass names</param>
		static public void FillSubclassNameList(Type typeBase, IList listSubclassNames)
		{
			Type[] subclasses = GetSubclasses(typeBase);
			foreach (Type type in subclasses)
				listSubclassNames.Add(type.FullName);
		}
	}
}
