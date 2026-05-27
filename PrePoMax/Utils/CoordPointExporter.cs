using CaeMesh;
using System;
using System.Collections.Generic;
using System.IO;

namespace PrePoMax.Utils
{
    internal class CoordPointExporter
    {
        public static bool ExportXYZ(CoordPointSet pointSet, string fileName)
        // Formato |
        //         V
        /*
        # CSV file produced by PrePoMax Manufai Version
        # Date of creation: 2026-03-04 19:14:41
        # Length unit: Millimeter [mm]
        #
        # Calculated robot trajectory - including all modifications
        #
        #Orientation: local vector
        1
        #
        # order;activity;x-coordinate;y-coordinate;z-coordinate;x-orientation;y-orientation;z-orientation
        1;true;0.5044234;-0.2982667;0.003258564;0.0;0.0;0.001
        2;true;0.5044234;-0.3391758;0.04416765;0.0;0.0;0.001
        */
        {
            List<string> lines = new List<string>();

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            lines.Add(
                "# CSV file produced by PrePoMax Manufai Version\n" +
                $"# Date of creation: {timestamp}\n" +
                "# Length unit: Millimeter [mm]\n" +
                "#\n" +
                "# Calculated robot trajectory - including all modifications\n" +
                "#\n" +
                "#Orientation: local vector\n" +
                "1\n" +
                "#\n" +
                "# order;activity;x-coordinate;y-coordinate;z-coordinate;x-orientation;y-orientation;z-orientation"
            );

            foreach (CoordPoint point in pointSet.Points)
            {
                lines.Add(
                    $"{point.Id};true;{point.X};{point.Y};{point.Z};0;0;1"
                );
            }
            try
            {
                File.WriteAllLines(fileName, lines);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}