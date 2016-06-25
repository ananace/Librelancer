﻿/* The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an "AS IS"
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
 * License for the specific language governing rights and limitations
 * under the License.
 * 
 * 
 * The Initial Developer of the Original Code is Callum McGing (mailto:callum.mcging@gmail.com).
 * Portions created by the Initial Developer are Copyright (C) 2013-2016
 * the Initial Developer. All Rights Reserved.
 */
using System;

namespace LibreLancer.Vertices
{
	public class VertexDeclaration
	{
		public int Stride;
		public VertexElement[] Elements;

		public VertexDeclaration (int stride, params VertexElement[] elements)
		{
			Stride = stride;
			Elements = elements;
		}
		internal void SetPointers()
		{
			foreach (var e in Elements) {
				GL.EnableVertexAttribArray (e.Slot);
				GL.VertexAttribPointer ((uint)e.Slot, e.Elements, (int)e.Type, e.Normalized, Stride, (IntPtr)(e.Offset));
			}
		}

	}
}
