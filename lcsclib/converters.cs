using AltiumSharp;
using AltiumSharp.BasicTypes;
using AltiumSharp.Records;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lcsclib
{
    public class Converters
    {
        public static Dictionary<string, Layer> LCSCLayerAltiumLayerMap = new Dictionary<string, Layer>
        {
            {"1", Layer.TopLayer},
        {"2", Layer.BottomLayer},
        {"3", Layer.TopOverlay},
        {"4", Layer.BottomOverlay},
        {"5", Layer.TopPaste},
        {"6", Layer.BottomPaste},
        {"7", Layer.TopSolder},
        {"8", Layer.BottomSolder},
        {"9", Layer.ConnectLayer},
        {"10", Layer.KeepOutLayer},
        {"11", Layer.MultiLayer},
        {"12", Layer.NoLayer},
        {"13", Layer.NoLayer},
        {"14", Layer.NoLayer},
        {"15", Layer.NoLayer},
        {"19", Layer.NoLayer},
        {"21", Layer.MidLayer1},
        {"22", Layer.MidLayer2},
        {"23", Layer.MidLayer3},
        {"24", Layer.MidLayer4},
        {"25", Layer.MidLayer5},
        {"26", Layer.MidLayer6},
        {"27", Layer.MidLayer7},
        {"28", Layer.MidLayer8},
        {"29", Layer.MidLayer9},
        {"30", Layer.MidLayer10},
        {"31", Layer.MidLayer11},
        {"32", Layer.MidLayer12},
        {"33", Layer.MidLayer13},
        {"34", Layer.MidLayer14},
        {"35", Layer.MidLayer15},
        {"36", Layer.MidLayer16},
        {"37", Layer.MidLayer17},
        {"38", Layer.MidLayer18},
        {"39", Layer.MidLayer19},
        {"40", Layer.MidLayer20},
        {"41", Layer.MidLayer21},
        {"42", Layer.MidLayer22},
        {"43", Layer.MidLayer23},
        {"44", Layer.MidLayer24},
        {"45", Layer.MidLayer25},
        {"46", Layer.MidLayer26},
        {"47", Layer.MidLayer27},
        {"48", Layer.MidLayer28},
        {"49", Layer.MidLayer29},
        {"50", Layer.MidLayer30},
        {"51", Layer.NoLayer},
        {"52", Layer.NoLayer},
        {"99", Layer.NoLayer},
        {"100", Layer.NoLayer}
    };
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

        private static List<CoordPoint> TrackPointsToListXY(string points, double zeroX, double zeroY)
        {
            string[] pointsArray = points.Split(' ');
            if ((pointsArray.Length % 2) == 1)
            {
                return new List<CoordPoint>();
            }
            var tmp = new List<CoordPoint>();
            for (int i = 0; i < pointsArray.Length; i += 2)
            {
                double.TryParse(pointsArray[i], out double x);
                double.TryParse(pointsArray[i + 1], out double y);
                x -= zeroX;
                y -= zeroY;
                y = 0 - y;
                tmp.Add(CoordPoint.FromMils((Coord)LCSCcoordToMil(x), (Coord)LCSCcoordToMil(y)));
            }
            return tmp;
        }
        public static PcbComponent FootprintResponseToPcbComponent(FootprintResponse component)
        {
            var pcbComp = new PcbComponent();
            //calc offset for zero
            string p1 = component.dataStr.shape.FirstOrDefault(c => c.Split('~')[(int)PADoffsets.command] == "PAD");
            string[] p1Array = p1.Split('~');
            double.TryParse(p1Array[(int)PADoffsets.center_x], out double zero_x);
            double.TryParse(p1Array[(int)PADoffsets.center_y], out double zero_y);
            //iterate over pads
            foreach (var item in component.dataStr.shape)
            {
                string[] shape = item.Split('~');
                if (shape[0] == "PAD")
                {
                    double.TryParse(shape[(int)PADoffsets.center_x], out double center_x);
                    double.TryParse(shape[(int)PADoffsets.center_y], out double center_y);
                    double.TryParse(shape[(int)PADoffsets.width], out double size_x);
                    double.TryParse(shape[(int)PADoffsets.height], out double size_y);
                    double.TryParse(shape[(int)PADoffsets.rotation], out double rotationAngle);
                    Coord HoleSize = 0;
                    PcbPadHoleShape HoleShape = PcbPadHoleShape.Round;
                    Coord HoleSlotLengthCoord = 0;
                    int holeAngle = 0;
                    PcbPadTemplate padTemplate = PcbPadTemplate.Tht;
                    if (LCSCLayerAltiumLayerMap[shape[(int)PADoffsets.layer_id]] == Layer.MultiLayer)
                    {
                        padTemplate = PcbPadTemplate.Tht;
                        double.TryParse(shape[(int)PADoffsets.hole_radius], out double hole_radius);
                        HoleSize = Coord.FromMils(LCSCcoordToMil(hole_radius)) * 2;
                        if (shape[(int)PADoffsets.Hole_Length] != "")
                        {
                            double.TryParse(shape[(int)PADoffsets.Hole_Length], out double HoleSlotLength);
                            if (HoleSlotLength > 0)
                            {
                                HoleShape = PcbPadHoleShape.Slot;
                                HoleSlotLengthCoord = Coord.FromMils(LCSCcoordToMil(HoleSlotLength));
                                holeAngle = 270;
                            }
                        }
                    }
                    else
                    {
                        padTemplate = LCSCLayerAltiumLayerMap[shape[(int)PADoffsets.layer_id]] ==
                            Layer.TopLayer ? PcbPadTemplate.SmtTop : PcbPadTemplate.SmtBottom;
                    }
                    pcbComp.Add(new PcbPad(padTemplate)
                    {
                        Layer = LCSCLayerAltiumLayerMap[shape[(int)PADoffsets.layer_id]],
                        Location = CoordPoint.FromMils(LCSCcoordToMil(center_x - zero_x), 0 - LCSCcoordToMil(center_y - zero_y)),
                        Designator = shape[(int)PADoffsets.Designator],
                        SizeTop = CoordPoint.FromMils(LCSCcoordToMil(size_x), LCSCcoordToMil(size_y)),
                        Rotation = rotationAngle,
                        SolderMaskExpansion = Coord.FromMils(4),//TODO implement recognition of solder mask expansion
                        Shape = LCSCshapeToAltium(shape[(int)PADoffsets.shape]),
                        HoleSize = HoleSize,
                        HoleShape = HoleShape,
                        HoleSlotLength = HoleSlotLengthCoord,
                        HoleRotation = holeAngle,
                        IsPlated = shape[(int)PADoffsets.Plated] == "Y"
                    });
                }
                else if (shape[0] == "ARC")
                {
                    double.TryParse(shape[(int)ARCoffsets.stroke_width], out double stroke_width);
                    if (LCSCarcConverter.TryParseArcPath(shape[(int)ARCoffsets.path_string], out LCSCarcConverter.SvgArcPath svgArc) != true)
                    {
                        throw new Exception("failed to parse arc parameters");
                    }
                    var arc = LCSCarcConverter.SvgArcPathToArc(svgArc);
                    pcbComp.Add(new PcbArc
                    {
                        Layer = LCSCLayerAltiumLayerMap[shape[(int)ARCoffsets.layer_id]],
                        Location = CoordPoint.FromMils(LCSCcoordToMil(arc.center_x - zero_x), 0 - LCSCcoordToMil(arc.center_y - zero_y)),
                        Width = Coord.FromMils(LCSCcoordToMil(stroke_width)),
                        Radius = Coord.FromMils(LCSCcoordToMil(arc.radius)),
                        StartAngle = arc.endAngle,
                        EndAngle = arc.startAngle
                    });
                }
                else if (shape[0] == "CIRCLE")
                {
                    double.TryParse(shape[(int)CIRCLEoffsets.center_x], out double center_x);
                    double.TryParse(shape[(int)CIRCLEoffsets.center_y], out double center_y);
                    double.TryParse(shape[(int)CIRCLEoffsets.stroke_width], out double stroke_width);
                    double.TryParse(shape[(int)CIRCLEoffsets.radius], out double radius);
                    pcbComp.Add(new PcbArc
                    {
                        Layer = LCSCLayerAltiumLayerMap[shape[(int)CIRCLEoffsets.layer_id]],
                        Location = CoordPoint.FromMils(LCSCcoordToMil(center_x - zero_x), 0 - LCSCcoordToMil(center_y - zero_y)),
                        Width = Coord.FromMils(LCSCcoordToMil(stroke_width)),
                        Radius = Coord.FromMils(LCSCcoordToMil(radius)),
                        StartAngle = 0,
                        EndAngle = 360
                    });
                }
                else if (shape[0] == "HOLE")
                {
                    double.TryParse(shape[(int)HOLEoffsets.center_x], out double center_x);
                    double.TryParse(shape[(int)HOLEoffsets.center_y], out double center_y);
                    double.TryParse(shape[(int)HOLEoffsets.radius], out double radius);
                    pcbComp.Add(new PcbArc
                    {
                        Layer = Layer.DrillDrawing,
                        Location = CoordPoint.FromMils(LCSCcoordToMil(center_x - zero_x), 0 - LCSCcoordToMil(center_y - zero_y)),
                        Radius = Coord.FromMils(LCSCcoordToMil(radius)),
                        StartAngle = 0,
                        EndAngle = 360
                    });
                }
                else if (shape[0] == "TRACK")
                {
                    double.TryParse(shape[(int)TRACKoffsets.stroke_width], out double stroke_width);
                    pcbComp.Add(new PcbMetaTrack
                    {
                        Layer = LCSCLayerAltiumLayerMap[shape[(int)TRACKoffsets.layer_id]],
                        Vertices = TrackPointsToListXY(shape[(int)TRACKoffsets.points], zero_x, zero_y),
                        Width = Coord.FromMils(stroke_width)
                    });
                }
                else if (shape[0] == "RECT")
                {
                    double.TryParse(shape[(int)RECToffsets.x], out double x);
                    double.TryParse(shape[(int)RECToffsets.y], out double y);
                    double.TryParse(shape[(int)RECToffsets.width], out double width);
                    double.TryParse(shape[(int)RECToffsets.height], out double height);
                    double.TryParse(shape[(int)RECToffsets.stroke_width], out double stroke_width);
                    pcbComp.Add(new PcbMetaTrack
                    {
                        Layer = Layer.TopOverlay,
                        Vertices = new List<CoordPoint>()
                        {
                            CoordPoint.FromMils((Coord)LCSCcoordToMil(x - zero_x), 0 - (Coord)LCSCcoordToMil(y - zero_y)),
                            CoordPoint.FromMils((Coord)LCSCcoordToMil(x - zero_x + width), 0 - (Coord)LCSCcoordToMil(y - zero_y)),
                            CoordPoint.FromMils((Coord)LCSCcoordToMil(x - zero_x + width), 0 - (Coord)LCSCcoordToMil(y - zero_y + height)),
                            CoordPoint.FromMils((Coord)LCSCcoordToMil(x - zero_x), 0 - (Coord)LCSCcoordToMil(y - zero_y + height)),
                            CoordPoint.FromMils((Coord)LCSCcoordToMil(x - zero_x), 0 - (Coord)LCSCcoordToMil(y - zero_y))
                        },
                        Width = Coord.FromMils(stroke_width)
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
