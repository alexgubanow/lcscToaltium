using AltiumSharp;
using AltiumSharp.BasicTypes;
using AltiumSharp.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lcsclib
{
    public class Converters
    {
        private static double LCSCcoordToINCH(double val) { return val / 100.0; }
        private static double LCSCcoordToMil(double val) { return val * 10.0; }
        private static PcbPadShape LCSCshapeToAltium(string val)
        {
            switch (val)
            {
                case "OVAL":
                    return PcbPadShape.Round;
                case "":
                    return PcbPadShape.Round;
                case "RECT":
                    return PcbPadShape.Rectangular;
                default:
                    return PcbPadShape.Round;
            }
        }
        public static PcbComponent FootprintResponseToPcbComponent(lcsclib.FootprintResponse component)
        {
            var pcbComp = new PcbComponent();
            //calc offset for zero
            string p1 = component.dataStr.shape.FirstOrDefault(c => c.Split('~')[(int)lcsclib.PADoffsets.command] == "PAD");
            string[] p1Array = p1.Split('~');
            double.TryParse(p1Array[(int)lcsclib.PADoffsets.center_x], out double zero_x);
            double.TryParse(p1Array[(int)lcsclib.PADoffsets.center_y], out double zero_y);
            //iterate over pads
            foreach (var item in component.dataStr.shape)
            {
                string[] shape = item.Split('~');
                if (shape[(int)lcsclib.PADoffsets.command] == "PAD")
                {
                    double.TryParse(shape[(int)lcsclib.PADoffsets.center_x], out double center_x);
                    double.TryParse(shape[(int)lcsclib.PADoffsets.center_y], out double center_y);
                    double.TryParse(shape[(int)lcsclib.PADoffsets.width], out double size_x);
                    double.TryParse(shape[(int)lcsclib.PADoffsets.height], out double size_y);
                    double.TryParse(shape[(int)lcsclib.PADoffsets.rotation], out double rotationAngle);
                    pcbComp.Add(new PcbPad(PcbPadTemplate.SmtTop)
                    {
                        Location = CoordPoint.FromMils(LCSCcoordToMil(center_x - zero_x), 0 - LCSCcoordToMil(center_y - zero_y)),
                        Designator = shape[(int)lcsclib.PADoffsets.Designator],
                        SizeTop = CoordPoint.FromMils(LCSCcoordToMil(size_x), LCSCcoordToMil(size_y)),
                        Rotation = rotationAngle,
                        SolderMaskExpansion = Coord.FromMils(4),
                        Shape = LCSCshapeToAltium(shape[(int)lcsclib.PADoffsets.shape])
                    });
                }
            }
            pcbComp.Description = component.title;
            pcbComp.ItemGuid = component.uuid;
            pcbComp.RevisionGuid = component.uuid;
            return pcbComp;
        }
        public static void SavePcbComponentToFile(PcbComponent pcbComponent)
        {
            var pcbLib = new PcbLib
                {
                    pcbComponent
                };
            pcbLib.Header.BoardInsightViewConfigurationName = "";
            using var writer = new PcbLibWriter();
            writer.Write(pcbLib, pcbComponent.Description + ".pcblib", true);
        }
    }
}
