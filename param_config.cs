using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using VMS.TPS;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace ABE
{
    public interface IParam_config
    {
        string Export_Path { get; }
        int Smoothing_Iterations { get; }
        double Smoothing_Strength { get; }
        double Expansion_Strength { get; }
        double Max_Displacement { get; }
    }

    public class Param_config : IParam_config
    {
        public string Export_Path { get; private set; }
        public int Smoothing_Iterations { get; private set; }
        public double Smoothing_Strength { get; private set; }
        public double Expansion_Strength { get; private set; }
        public double Max_Displacement { get; private set; }

        public Param_config()
        {
            Export_Path = GetExportPath();
            Smoothing_Iterations = GetSmoothingIterations();
            Smoothing_Strength = GetSmoothingStrength();
            Expansion_Strength = GetExpansionStrength();
            Max_Displacement = GetMaxDisplacement();
        }

        private string GetExportPath()
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "param_config.xml");

            if (!File.Exists(configFile)) 
            {
                throw new FileNotFoundException("param_config.xml was not found.", configFile);
            }
            XDocument config = XDocument.Load(configFile);

            string exportPath = config.Descendants("export_path").FirstOrDefault()?.Value?.Trim();

            if (string.IsNullOrWhiteSpace(exportPath))
            {
                throw new InvalidDataException("Export path is missing from param_config.xml.");
            }

            if (!Directory.Exists(exportPath))
            {
                throw new DirectoryNotFoundException("Export directory does not exist: " + exportPath);
            }

            return exportPath;
        }

        private int GetSmoothingIterations()
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "param_config.xml");

            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException("param_config.xml was not found.", configFile);
            }
            XDocument config = XDocument.Load(configFile);

            string smoothingIterations = config.Descendants("smoothing_iterations").FirstOrDefault()?.Value?.Trim();

            if (string.IsNullOrWhiteSpace(smoothingIterations))
            {
                throw new InvalidDataException("Smoothing iterations is missing from param_config.xml.");
            }

            if (!int.TryParse(smoothingIterations, out int iterations))
            {
                throw new InvalidDataException("Invalid value for smoothing iterations in param_config.xml.");
            }

            return iterations;
        }

        private double GetSmoothingStrength()
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "param_config.xml");

            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException("param_config.xml was not found.", configFile);
            }
            XDocument config = XDocument.Load(configFile);

            string smoothingStrength = config.Descendants("smoothing_strength").FirstOrDefault()?.Value?.Trim();

            if (string.IsNullOrWhiteSpace(smoothingStrength))
            {
                throw new InvalidDataException("Smoothing strength is missing from param_config.xml.");
            }

            if (!double.TryParse(smoothingStrength, out double strength))
            {
                throw new InvalidDataException("Invalid value for smoothing strength in param_config.xml.");
            }

            return strength;
        }

        private double GetExpansionStrength()
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "param_config.xml");

            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException("param_config.xml was not found.", configFile);
            }
            XDocument config = XDocument.Load(configFile);

            string expansionStrength = config.Descendants("expansion_strength").FirstOrDefault()?.Value?.Trim();

            if (string.IsNullOrWhiteSpace(expansionStrength))
            {
                throw new InvalidDataException("Expansion strength is missing from param_config.xml.");
            }

            if (!double.TryParse(expansionStrength, out double strength))
            {
                throw new InvalidDataException("Invalid value for expansion strength in param_config.xml.");
            }

            return strength;
        }

        private double GetMaxDisplacement()
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "param_config.xml");

            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException("param_config.xml was not found.", configFile);
            }
            XDocument config = XDocument.Load(configFile);

            string maxDisplacement = config.Descendants("max_displacement").FirstOrDefault()?.Value?.Trim();

            if (string.IsNullOrWhiteSpace(maxDisplacement))
            {
                throw new InvalidDataException("Max displacement is missing from param_config.xml.");
            }

            if (!double.TryParse(maxDisplacement, out double displacement))
            {
                throw new InvalidDataException("Invalid value for max displacement in param_config.xml.");
            }

            return displacement;
        }
    }
}