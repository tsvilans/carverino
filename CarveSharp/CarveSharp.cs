/**
 * CarveSharp, .NET Wrapper for Carve's CSG and mesh boolean operations
 *
 * Copyright (C) 2015  Mehran Maghoumi (https://www.maghoumi.com)
 * and Copyright (C) 2017 Tom Svilans (http://tomsvilans.com)
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
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace CarveSharp
{
    /// <summary>
    /// Class to hold return data from Carve.
    /// </summary>
    public class CarveMesh
    {
        public double[] Vertices;
        public int[] FaceIndices;
        public int[] FaceSizes;

        /// <summary>
        /// Duplicate CarveMesh.
        /// </summary>
        /// <returns></returns>
        public CarveMesh Duplicate()
        {
            CarveMesh cm = new CarveMesh();
            cm.Vertices = new double[Vertices.Length];
            cm.FaceIndices = new int[FaceIndices.Length];
            cm.FaceSizes = new int[FaceSizes.Length];

            Vertices.CopyTo(cm.Vertices, 0);
            FaceIndices.CopyTo(cm.FaceIndices, 0);
            FaceSizes.CopyTo(cm.FaceSizes, 0);
            return cm;
        }
    }

    /// <summary>
    /// Contains the methods for performing CSG operations using Carve
    /// </summary>
    public static class CarveSharp
    {
        /// <summary>
        /// Enum of all the operations that can be performed on
        /// two meshes using CSG.
        /// </summary>
        public enum CSGOperations
        {
            /// <summary>
            /// In A or in B.
            /// </summary>
            Union,

            /// <summary>
            /// In A and in B.
            /// </summary>
            Intersection,

            /// <summary>
            /// In A, but not in B.
            /// </summary>
            AMinusB,

            /// <summary>
            /// In B, but not in A.
            /// </summary>
            BMinusA,

            /// <summary>
            /// In A or B, but not in both.
            /// </summary>
            SymmetricDifferent,
            
            /// <summary>
            /// All split faces from A and B.
            /// </summary>
            All
        }

        /// <summary>
        /// Defines the low-level structure that the DLL wrapper uses
        /// to represent a triangular mesh.
        /// </summary>
        private unsafe struct InteropMesh
        {
            /// <summary>
            /// The array containing the vertices
            /// </summary>
            public double* vertices;

            /// <summary>
            /// The array containing the triangle indices
            /// </summary>
            public int* faceIndices;

            /// <summary>
            /// The array containing the face sizes (number of verts
            /// per face)
            /// </summary>
            public int* faceSizes;

            /// <summary>
            /// The number of elements in the vertices array
            /// </summary>
            public int numVertices;

            /// <summary>
            /// The number of faces in the mesh. 
            /// </summary>
            public int numFaces;

            /// <summary>
            /// The number of indices in the face indices array
            /// </summary>
            public int numFaceIndices;
        }

        /// <summary>
        /// The DLL entry definition for performing CSG operations.
        /// </summary>
        /// <param name="a">The first mesh</param>
        /// <param name="b">The second mesh</param>
        /// <param name="op">The operation that should be performed on the meshes</param>
        /// <returns>The resulted mesh</returns>
        [DllImport("CarveLibWrapper.dll")]
        private static unsafe extern InteropMesh* performCSG(InteropMesh* a, InteropMesh* b, CSGOperations op);

        /// <summary>
        /// The DLL entry definition for freeing the memory after a CSG operation.
        /// </summary>
        /// <param name="a"></param>
        [DllImport("CarveLibWrapper.dll")]
        private static unsafe extern void freeMesh(InteropMesh* a);

        /// <summary>
        /// Performs the specified operation on the provided meshes.
        /// </summary>
        /// <param name="firstVerts">The first mesh vertices as double list.</param>
        /// <param name="secondVerts">The second mesh vertices as double list.</param>
        /// <param name="firstFaceIndices">Face indices of first mesh as int list.</param>
        /// <param name="firstFaceSizes">Face sizes of first mesh as int list.</param>
        /// <param name="secondFaceIndices">Face indices of second mesh as int list.</param>
        /// <param name="secondFaceSizes">Face sizes of second mesh as int list.</param>
        /// <param name="operation">The mesh opration to perform on the two meshes</param>
        /// <returns>A triangular mesh resulting from performing the specified operation. If the resulting mesh is empty, will return null.</returns>
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        public static CarveMesh PerformCSG(double[] firstVerts, int[] firstFaceIndices, int[] firstFaceSizes, 
            double[] secondVerts, int[] secondFaceIndices, int[] secondFaceSizes, 
            CSGOperations operation)
        {
            CarveMesh finalResult = null;

            unsafe
            {
                InteropMesh a = new InteropMesh();
                InteropMesh b = new InteropMesh();

                InteropMesh* result;

                fixed (double* aVerts = &firstVerts[0], bVerts = &secondVerts[0])
                {
                    fixed (int* aFaces = &firstFaceIndices[0], bFaces = &secondFaceIndices[0])
                    {
                        fixed (int* aFSizes = &firstFaceSizes[0], bFSizes = &secondFaceSizes[0])
                        {
                            a.numVertices = firstVerts.Length;
                            a.numFaceIndices = firstFaceIndices.Length;
                            a.vertices = aVerts;
                            a.faceIndices = aFaces;
                            a.faceSizes = aFSizes;
                            a.numFaces = firstFaceSizes.Length;

                            b.numVertices = secondVerts.Length;
                            b.numFaceIndices = secondFaceIndices.Length;
                            b.vertices = bVerts;
                            b.faceIndices = bFaces;
                            b.faceSizes = bFSizes;
                            b.numFaces = secondFaceSizes.Length;

                            try
                            {
                                result = performCSG(&a, &b, operation);
                            }

                            catch (SEHException ex)
                            {
                                ArgumentException e = new ArgumentException("Carve has thrown an error. Possible reason is corrupt or self-intersecting meshes", ex);
                                throw e;
                            }
                        }
                    }
                }

                if (result->numVertices == 0)
                {
                    freeMesh(result);
                    return null;
                }


                finalResult = new CarveMesh();

                finalResult.Vertices = new double[result->numVertices];
                finalResult.FaceIndices = new int[result->numFaceIndices];
                finalResult.FaceSizes = new int[result->numFaces];

                if (result->numVertices > 0)
                    Parallel.For(0, finalResult.Vertices.Length, i =>
                    {
                        finalResult.Vertices[i] = result->vertices[i];
                    });

                if (result->numFaceIndices > 0)
                    Parallel.For(0, finalResult.FaceIndices.Length, i =>
                    {
                        finalResult.FaceIndices[i] = result->faceIndices[i];
                    });

                if (result->numFaces > 0)
                    Parallel.For(0, finalResult.FaceSizes.Length, i =>
                    {
                        finalResult.FaceSizes[i] = result->faceSizes[i];
                    });

                freeMesh(result);

            }   // end-unsafe

            return finalResult;
        }

        /// <summary>
        /// Performs the specified operation on the provided meshes.
        /// </summary>
        /// <param name="MeshA">The first mesh.</param>
        /// <param name="MeshB">The second mesh.</param>
        /// <param name="operation">The mesh opration to perform on the two meshes</param>
        /// <returns>A triangular mesh resulting from performing the specified operation. If the resulting mesh is empty, will return null.</returns>
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        public static CarveMesh PerformCSG(CarveMesh MeshA, CarveMesh MeshB, CSGOperations operation)
        {
            return PerformCSG(MeshA.Vertices, MeshA.FaceIndices, MeshA.FaceSizes, 
                MeshB.Vertices, MeshB.FaceIndices, MeshB.FaceSizes, operation);
        }


    }
}
