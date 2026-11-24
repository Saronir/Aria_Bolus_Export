using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.IO;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace ABE
{
    public interface IMesh
    {
        Point3DCollection Positions { get; }
        Int32Collection TriangleIndices { get; }
        void Smooth(int iterations = 2, double lambda = 0.2);
    }
    public class Mesh : IMesh
    {
        public Mesh(Structure structure)
        {
            if (structure == null)
            {
                throw new ArgumentNullException("structure");
            }
            if (!structure.HasSegment)
            {
                throw new ArgumentException("Structure does not have a segment.");
            }
            var mesh = structure.MeshGeometry; //origional mesh from ESAPI
            Positions = new Point3DCollection(mesh.Positions); //copy the positions from the ESAPI mesh to the new mesh to be smoothed
            TriangleIndices = new Int32Collection(mesh.TriangleIndices); //copy the triangle indices from the ESAPI mesh to the new mesh to be smoothed
        }
        public Point3DCollection Positions { get; private set; }
        public Int32Collection TriangleIndices { get; private set; }

        public void Smooth(int iterations, double lambda)
        {
            int n = Positions.Count;

            // Build vertex adjacency from triangle indices
            var neighbors = new HashSet<int>[n];

            for (int i = 0; i < n; i++)
            {
                neighbors[i] = new HashSet<int>();
            }
            for (int i = 0; i < TriangleIndices.Count; i += 3)
            {
                int a = TriangleIndices[i];
                int b = TriangleIndices[i + 1];
                int c = TriangleIndices[i + 2];

                neighbors[a].Add(b);
                neighbors[a].Add(c);

                neighbors[b].Add(a);
                neighbors[b].Add(c);

                neighbors[c].Add(a);
                neighbors[c].Add(b);
            }

            // Iterative Laplacian smoothing
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                var newPositions = new Point3D[n];

                for (int i = 0; i < n; i++)
                {
                    Point3D p = Positions[i];

                    if (neighbors[i].Count == 0)
                    {
                        newPositions[i] = p;
                        continue;
                    }

                    double x = 0, y = 0, z = 0;

                    foreach (int j in neighbors[i])
                    {
                        x += Positions[j].X;
                        y += Positions[j].Y;
                        z += Positions[j].Z;
                    }

                    double count = neighbors[i].Count;

                    newPositions[i] = new Point3D(
                        p.X + lambda * (x / count - p.X),
                        p.Y + lambda * (y / count - p.Y),
                        p.Z + lambda * (z / count - p.Z)
                    );
                }

                for (int i = 0; i < n; i++)
                    Positions[i] = newPositions[i];
            }
        }
    }
}
