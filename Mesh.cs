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
        void LaplacianSmooth(int iterations = 2, double lambda = 0.2);
        void Constrained_Taubin_Smooth(MeshGeometry3D mesh, int iterations = 20, double lambda = 0.5, double mu = -0.53, double maxDisplacement = 0.5);
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

        public void LaplacianSmooth(int iterations, double lambda)
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
        public void Constrained_Taubin_Smooth(MeshGeometry3D mesh, int iterations, double lambda, double mu, double maxDisplacement)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException(nameof(mesh));
            }
            if (iterations < 0 || maxDisplacement < 0)
            {    
                throw new ArgumentOutOfRangeException();
            }

            // Preserve the original vertex positions.
            Point3D[] original = mesh.Positions.ToArray();

            Point3D[] vertices = mesh.Positions.ToArray();

            // Mesh connectivity remains unchanged.
            HashSet<int>[] neighbours = BuildAdjacency(mesh);

            for (int i = 0; i < iterations; i++)
            {
                // Contraction pass.
                vertices = LaplacianStep(
                    vertices, neighbours, lambda);

                // Expansion pass.
                vertices = LaplacianStep(
                    vertices, neighbours, mu);

                // Apply cumulative displacement constraint.
                ApplyConstraint(
                    vertices, original, maxDisplacement);
            }

            mesh.Positions = new Point3DCollection(vertices);

            // Remove stale normals after modifying geometry.
            mesh.Normals = new Vector3DCollection();
        }
        private static HashSet<int>[] BuildAdjacency(MeshGeometry3D mesh)
        {
            int count = mesh.Positions.Count;

            var neighbours = new HashSet<int>[count];

            for (int i = 0; i < count; i++)
            {
                neighbours[i] = new HashSet<int>();
            }
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                int a = mesh.TriangleIndices[i];
                int b = mesh.TriangleIndices[i + 1];
                int c = mesh.TriangleIndices[i + 2];

                neighbours[a].Add(b);
                neighbours[a].Add(c);

                neighbours[b].Add(a);
                neighbours[b].Add(c);

                neighbours[c].Add(a);
                neighbours[c].Add(b);
            }

            return neighbours;
        }
        private static Point3D[] LaplacianStep(Point3D[] vertices, HashSet<int>[] neighbours, double factor)
        {
            var result = new Point3D[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                if (neighbours[i].Count == 0)
                {
                    result[i] = vertices[i];
                    continue;
                }

                Vector3D displacement = new Vector3D();

                foreach (int j in neighbours[i])
                {
                    displacement += vertices[j] - vertices[i];
                }

                displacement /= neighbours[i].Count;

                result[i] = vertices[i] + factor * displacement;
            }

            return result;
        }
        private static void ApplyConstraint(Point3D[] vertices, Point3D[] original, double maxDisplacement)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3D displacement = vertices[i] - original[i];

                double distance = displacement.Length;

                if (distance > maxDisplacement)
                {
                    displacement *= maxDisplacement / distance;

                    vertices[i] = original[i] + displacement;
                }
            }
        }
    }
}
