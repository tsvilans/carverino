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

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CarveGH
{
    public class CarveComponent : GH_Component
    {
        public CarveComponent()
          : base("Carve", "Carve",
              "Perform boolean operations on two meshes using the Carve library.",
              "Mesh", "Util")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("MeshA", "A", "First mesh.", GH_ParamAccess.item);
            pManager.AddMeshParameter("MeshB", "B", "Second mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Operation", "Op", "Operation to perform (Union = 0, Intersection = 1, " +
                "AMinusB = 2, BMinusA = 3, SymmetricalDifferent = 4, All = 5", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Result", "R", "Result of boolean operation.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mA = null, mB = null;
            int op = 0;
            if (!DA.GetData("MeshA", ref mA))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "MeshA has invalid input.");
                return;
            }

            if (!DA.GetData("MeshB", ref mB))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "MeshB has invalid input.");
                return;
            }

            DA.GetData("Operation", ref op);

            if (op < 0 || op > 5)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid operation type. Refer to input description for available types.");
                return;
            }

            if (mA == null || mB == null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Something went wrong...");
                return;
            }

            Mesh res = CarveRC.CarveOps.PerformCSG(mA, mB, (CarveRC.Operations)op);

            if (res == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Carve operation failed...");
                return;
            }

            DA.SetData("Result", res);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.CarveGH_icon;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("f9a926d3-7124-47ab-a321-452547c0606a"); }
        }
    }
}
