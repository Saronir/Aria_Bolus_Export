using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ABE
{
    public interface ISTL_Exporter
    {
        string Export_Path { get; }
        IMesh Mesh { get; }
        void ExportMeshToSTL();
    }
    public class STL_Exporter : ISTL_Exporter
    {
        public string Export_Path { get; private set; }
        public IMesh Mesh { get; private set; }
        public STL_Exporter(string exportPath, IMesh mesh)
        {
            Export_Path = exportPath;
            Mesh = mesh;
        }
        public void ExportMeshToSTL()
        {
            if (Mesh == null)
            {
                throw new InvalidOperationException("Mesh is not set.");
            }
            if (string.IsNullOrWhiteSpace(Export_Path))
            {
                throw new ArgumentException("Export path cannot be null or whitespace.", nameof(Export_Path));
            }
            using (var writer = new System.IO.StreamWriter(Export_Path))
            {
                writer.WriteLine("solid mesh");
                for (int i = 0; i < Mesh.TriangleIndices.Count; i += 3)
                {
                    var a = Mesh.Positions[Mesh.TriangleIndices[i]];
                    var b = Mesh.Positions[Mesh.TriangleIndices[i + 1]];
                    var c = Mesh.Positions[Mesh.TriangleIndices[i + 2]];
                    var normal = CalculateNormal(a, b, c);
                    writer.WriteLine($"facet normal {normal.X} {normal.Y} {normal.Z}");
                    writer.WriteLine("outer loop");
                    writer.WriteLine($"vertex {a.X} {a.Y} {a.Z}");
                    writer.WriteLine($"vertex {b.X} {b.Y} {b.Z}");
                    writer.WriteLine($"vertex {c.X} {c.Y} {c.Z}");
                    writer.WriteLine("endloop");
                    writer.WriteLine("endfacet");
                }
                writer.WriteLine("endsolid mesh");
            }
        }
        private Vector3D CalculateNormal(Point3D a, Point3D b, Point3D c)
        {
            // Calculate two edge vectors
            Vector3D ab = b - a;
            Vector3D ac = c - a;

            // Cross product gives the surface normal
            Vector3D normal = Vector3D.CrossProduct(ab, ac);

            // Normalize to unit length
            if (normal.LengthSquared > 0)
            {
                normal.Normalize();
            }

            return normal;
        }

    }
}
