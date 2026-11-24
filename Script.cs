using ABE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace VMS.TPS
{
    public class Script // This is the script that will be called by Eclipse.
    {
        public Script()
        {

        }

        public void Execute(ScriptContext context)
        {
            if (context.StructureSet == null)
            {
                throw new InvalidOperationException("No structure set selected.");
            }

            IParam_config paramConfig = new param_config(); //Load the parameter configuration from the param_config.xml file

            string patientId = context.Patient.Id; //Get the patient ID
            string bolusId = patientId.Substring(patientId.Length - 3);

            foreach (var structure in context.StructureSet.Structures) //Go through all structures in the structure set
            {
                if (!structure.HasSegment) //Check if the structure has a segment, if not, skip it
                {
                    continue;
                }

                var text = structure.Id; // Get the structure ID
                bool proc = text.IndexOf("bolus", StringComparison.OrdinalIgnoreCase) >= 0; //Check if the structure ID contains the word "bolus" (case insensitive)

                if (proc)
                {
                    //extract bolus geometry from ESAPI structure and expose it through the IMesh interface
                    //the mesh properties are copied from the original ESAPI mesh as to not operate on the original mesh, but rather on a copy of it
                    IMesh mesh = new Mesh(structure);

                    //Smooth the mesh using a basic laplacian smoothing algorithm
                    mesh.Smooth(paramConfig.Smoothing_iterations, paramConfig.Smoothing_strength);

                    //prepare the export path by creating a new folder with the current date in the export path specified in the param_config.xml file
                    string basePath = paramConfig.Export_path;
                    string dateFolder = DateTime.Today.ToString("yyyy-MM-dd");
                    string folderPath = Path.Combine(basePath, dateFolder);
                    Directory.CreateDirectory(folderPath);

                    //Create a new STL exporter with the export path and the mesh
                    ISTL_Exporter stlExporter = new STL_Exporter(folderPath + @"\" + structure.Id + "_" + bolusId + ".stl", mesh);
                    //Export the mesh to an STL file
                    stlExporter.ExportMeshToSTL();

                }
            }
        }
    }
}
