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
    public static class ExtensionMethods
    {
        public static CarveMesh ToCarveMesh(this Mesh m)
        {
            double[] verts = new double[m.Vertices.Count * 3];
            int[] facesizes = new int[m.Faces.Count];
            List<int> faces = new List<int>();

            int j = 0;
            for (int i = 0; i < m.Vertices.Count; ++i)
            {
                verts[j] = m.Vertices[i].X; ++j;
                verts[j] = m.Vertices[i].Y; ++j;
                verts[j] = m.Vertices[i].Z; ++j;
            }

            for (int i = 0; i < m.Faces.Count; ++i)
            {
                faces.Add(m.Faces[i].A);
                faces.Add(m.Faces[i].B);
                faces.Add(m.Faces[i].C);
                if (m.Faces[i].IsQuad)
                {
                    facesizes[i] = 4;
                    faces.Add(m.Faces[i].D);
                }
                else
                    facesizes[i] = 3;
            }

            CarveSharp.CarveMesh cm = new CarveSharp.CarveMesh();
            cm.Vertices = verts;
            cm.FaceIndices = faces.ToArray();
            cm.FaceSizes = facesizes;

            return cm;
        }

        public static Mesh ToRhinoMesh(this CarveSharp.CarveMesh cm)
        {
            Mesh m = new Mesh();
            for (int i = 0; i < cm.Vertices.Length; i += 3)
            {
                m.Vertices.Add(new Rhino.Geometry.Point3d(
                  cm.Vertices[i],
                  cm.Vertices[i + 1],
                  cm.Vertices[i + 2]));
            }

            int j = 0;
            for (int i = 0; i < cm.FaceSizes.Length; ++i)
            {
                if (cm.FaceSizes[i] == 3)
                {
                    m.Faces.AddFace(
                      cm.FaceIndices[j],
                      cm.FaceIndices[j + 1],
                      cm.FaceIndices[j + 2]);
                }
                else if (cm.FaceSizes[i] == 4)
                {
                    m.Faces.AddFace(
                      cm.FaceIndices[j],
                      cm.FaceIndices[j + 1],
                      cm.FaceIndices[j + 2],
                      cm.FaceIndices[j + 3]);
                }
                j += cm.FaceSizes[i];
            }
            return m;
        }
    }
}
