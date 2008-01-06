
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D;
using System.Drawing;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Geometry
{
	public class Polygon : List<Vector2D>
	{
		#region ================== Variables

		// Tree variables
		private List<Polygon> children;
		private bool inner;

		#endregion

		#region ================== Properties

		public List<Polygon> Children { get { return children; } }
		public bool Inner { get { return inner; } set { inner = value; } }

		#endregion

		#region ================== Constructors

		// Constructor
		internal Polygon()
		{
			// Initialize
			children = new List<Polygon>();
		}

		// Constructor
		internal Polygon(Polygon p, Vector2D add) : base(p)
		{
			// Initialize
			base.Add(add);
			children = new List<Polygon>();
		}

		#endregion
		
		#region ================== Methods

		// Point inside the polygon?
		// See: http://local.wasp.uwa.edu.au/~pbourke/geometry/insidepoly/
		public bool Intersect(Vector2D p)
		{
			float miny, maxy, maxx, xint;
			Vector2D v1 = base[base.Count - 1];
			Vector2D v2;
			int c = 0;
			
			// Go for all vertices
			for(int i = 0; i < base.Count; i++)
			{
				// Get next vertex
				v2 = base[i];

				// Determine min/max values
				miny = Math.Min(v1.y, v2.y);
				maxy = Math.Max(v1.y, v2.y);
				maxx = Math.Max(v1.x, v2.x);

				// Check for intersection
				if((p.y > miny) && (p.y <= maxy))
				{
					if(p.x <= maxx)
					{
						if(v1.y != v2.y)
						{
							xint = (p.y - v1.y) * (v2.x - v1.x) / (v2.y - v1.y) + v1.x;
							if((v1.x == v2.x) || (p.x <= xint)) c++;
						}
					}
				}
				
				// Move to next
				v1 = v2;
			}

			// Return result
			return ((c & 0x01) != 0);
		}
		
		// This inserts a polygon if it is a child of this one
		public bool InsertChild(Polygon p)
		{
			// Polygon must have at least 1 vertex
			if(p.Count == 0) return false;
			
			// Check if it can be inserted at a lower level
			foreach(Polygon child in children)
			{
				if(child.InsertChild(p)) return true;
			}

			// Check if it can be inserted here
			if(this.Intersect(p[0]))
			{
				// Make the polygon the inverse of this one
				p.Inner = !inner;
				children.Add(p);
				return true;
			}

			// Can't insert it as a child
			return false;
		}
		
		#endregion
	}
}
