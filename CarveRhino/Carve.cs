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
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

using CarveRC;
using Rhino.DocObjects;

namespace CarveRhino
{
    [System.Runtime.InteropServices.Guid("12c1dfae-542f-4662-8f25-438cf442fb3d")]
    public class Carve : Command
    {
        public Carve()
        {
            Instance = this;
        }

        public static Carve Instance
        {
            get; private set;
        }

        public override string EnglishName
        {
            get { return "Carve"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Mesh m1, m2;
            ObjRef objref1, objref2;
            Result res = Result.Nothing;

            res = RhinoGet.GetOneObject("Select first mesh", false, ObjectType.Mesh, out objref1);
            if (res != Result.Success)
                return res;

            m1 = objref1.Mesh();
            if (m1 == null)
                return Result.Failure;

            RhinoApp.WriteLine("Got Mesh1 with {0} verts and {1} faces.", m1.Vertices.Count, m1.Faces.Count);

            // De-select first mesh...

            RhinoObject rhinoObject = objref1.Object();
            if (null != rhinoObject)
                rhinoObject.Select(false);



            res = RhinoGet.GetOneObject("Select second mesh", false, ObjectType.Mesh, out objref2);

            if (res != Result.Success)
                return res;

            m2 = objref2.Mesh();
            if (m2 == null)
                return Result.Failure;

            RhinoApp.WriteLine("Got Mesh2 with {0} verts and {1} faces.", m2.Vertices.Count, m2.Faces.Count);


            int option = 0;
            res = RhinoGet.GetInteger("Union (0), Intersection (1), AMinusB (2), BMinusA (3), SymmetricalDifferent (4), All (5)", false, ref option);
            RhinoApp.WriteLine("Operation {0} selected.", ((CarveRC.Operations)(option)).ToString());

            if (option < 0 || option > 5)
            {
                RhinoApp.WriteLine("Invalid input value for Carve operation.");
                return Result.Failure;
            }

            Mesh result = CarveRC.CarveOps.PerformCSG(m1, m2, (CarveRC.Operations)option);

            RhinoApp.WriteLine("Performed Carve operation. New mesh has {0} verts and {1} faces.", result.Vertices.Count, result.Faces.Count);

            doc.Objects.AddMesh(result);
            doc.Views.Redraw();

            return Result.Success;
        }
    }
}
