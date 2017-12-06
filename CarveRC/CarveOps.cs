/**
 * Carverino, a plug-in for Rhinoceros 3d and Grasshopper based
 * on the .NET wrapper for Carve's CSG and mesh boolean operations
 * by Mehran Maghoumi (https://www.maghoumi.com)
 * adapted by Tom Svilans
 *
 * Copyright (C) 2017 Tom Svilans (http://tomsvilans.com)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;
using CarveSharp;

namespace CarveRC
{
    public static class CarveOps
    {
        public static Mesh PerformCSG(Mesh A, Mesh B, Operations Operation)
        {
            A.Weld(Math.PI);
            B.Weld(Math.PI);

            return CarveSharp.CarveSharp.PerformCSG(A.ToCarveMesh(), B.ToCarveMesh(), 
                (CarveSharp.CarveSharp.CSGOperations)Operation).ToRhinoMesh();
        }
    }

    public enum Operations
    {
        Union = 0,
        Intersection = 1,
        AMinusB = 2,
        BMinusA = 3,
        SymmetricalDifferent = 4,
        All = 5
    }
}
