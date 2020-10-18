using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace lcsclib
{
    public class LCSCarcConverter
    {
        public struct ArcParams
        {
            public double center_x;
            public double center_y;
            public double radius;
            public double startAngle;
            public double endAngle;
        }
        public struct SvgArcPath
        {
            public double x1;
            public double y1;
            public double rx;
            public double ry;
            public double phi;
            public bool fA;
            public bool fS;
            public double x2;
            public double y2;
        }

        private static double Radian(double ux, double uy, double vx, double vy)
        {
            var dot = ux * vx + uy * vy;
            var mod = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
            var rad = Math.Acos(dot / mod);
            if (ux * vy - uy * vx < 0.0)
            {
                rad = -rad;
            }
            return rad;
        }
        //conversion_from_endpoint_to_center_parameterization
        //sample :  svgArcToCenterParam(200,200,50,50,0,1,1,300,200)
        // x1 y1 rx ry φ fA fS x2 y2
        private static ArcParams SvgArcToArcParam(SvgArcPath svgArc)
        {
            double cx, cy, startAngle, deltaAngle, endAngle;
            var PIx2 = Math.PI * 2.0;

            if (svgArc.rx < 0)
            {
                svgArc.rx = -svgArc.rx;
            }
            if (svgArc.ry < 0)
            {
                svgArc.ry = -svgArc.ry;
            }
            if (svgArc.rx == 0.0 || svgArc.ry == 0.0)
            { // invalid arguments
                throw new Exception("rx and ry can not be 0");
            }

            var s_phi = Math.Sin(svgArc.phi);
            var c_phi = Math.Cos(svgArc.phi);
            var hd_x = (svgArc.x1 - svgArc.x2) / 2.0; // half diff of x
            var hd_y = (svgArc.y1 - svgArc.y2) / 2.0; // half diff of y
            var hs_x = (svgArc.x1 + svgArc.x2) / 2.0; // half sum of x
            var hs_y = (svgArc.y1 + svgArc.y2) / 2.0; // half sum of y

            // F6.5.1
            var x1_ = c_phi * hd_x + s_phi * hd_y;
            var y1_ = c_phi * hd_y - s_phi * hd_x;

            // F.6.6 Correction of out-of-range radii
            //   Step 3: Ensure radii are large enough
            var lambda = (x1_ * x1_) / (svgArc.rx * svgArc.rx) + (y1_ * y1_) / (svgArc.ry * svgArc.ry);
            if (lambda > 1)
            {
                svgArc.rx *= Math.Sqrt(lambda);
                svgArc.ry *= Math.Sqrt(lambda);
            }

            var rxry = svgArc.rx * svgArc.ry;
            var rxy1_ = svgArc.rx * y1_;
            var ryx1_ = svgArc.ry * x1_;
            var sum_of_sq = rxy1_ * rxy1_ + ryx1_ * ryx1_; // sum of square
            if (sum_of_sq == 0)
            {
                throw new Exception("start point can not be same as end point");
            }
            var coe = Math.Sqrt(Math.Abs((rxry * rxry - sum_of_sq) / sum_of_sq));
            if (svgArc.fA == svgArc.fS) { coe = -coe; }

            // F6.5.2
            var cx_ = coe * rxy1_ / svgArc.ry;
            var cy_ = -coe * ryx1_ / svgArc.rx;

            // F6.5.3
            cx = c_phi * cx_ - s_phi * cy_ + hs_x;
            cy = s_phi * cx_ + c_phi * cy_ + hs_y;

            var xcr1 = (x1_ - cx_) / svgArc.rx;
            var xcr2 = (x1_ + cx_) / svgArc.rx;
            var ycr1 = (y1_ - cy_) / svgArc.ry;
            var ycr2 = (y1_ + cy_) / svgArc.ry;

            // F6.5.5
            startAngle = Radian(1.0, 0.0, xcr1, ycr1);

            // F6.5.6
            deltaAngle = Radian(xcr1, ycr1, -xcr2, -ycr2);
            while (deltaAngle > PIx2) { deltaAngle -= PIx2; }
            while (deltaAngle < 0.0) { deltaAngle += PIx2; }
            if (!svgArc.fS) { deltaAngle -= PIx2; }
            endAngle = startAngle + deltaAngle;
            while (endAngle > PIx2) { endAngle -= PIx2; }
            while (endAngle < 0.0) { endAngle += PIx2; }

            ArcParams outputObj = new ArcParams()
            { /* cx, cy, startAngle, deltaAngle */
                center_x = cx,
                center_y = cy,
                startAngle = startAngle * (180 / Math.PI),
                endAngle = endAngle * (180 / Math.PI),
                radius = svgArc.rx
            };

            return outputObj;
        }
        public static bool TryParseArcPath(string ArcPath, out SvgArcPath arc)
        {
            string[] strArray = ArcPath.Split(' ');
            if (strArray[0] == "M" && strArray[3] == "A")
            {
                double.TryParse(strArray[1], out arc.x1);//x1
                double.TryParse(strArray[2], out arc.y1);//y1
                double.TryParse(strArray[4], out arc.rx);//rx
                double.TryParse(strArray[5], out arc.ry);//ry
                double.TryParse(strArray[6], out arc.phi);//x-axis-rotation
                int.TryParse(strArray[7], out int fA);//large-arc-flag
                arc.fA = fA == 1;
                int.TryParse(strArray[8], out int fS);//sweep-flag
                arc.fS = fS == 1;
                double.TryParse(strArray[9], out arc.x2);//x2
                double.TryParse(strArray[10], out arc.y2);//y2
                return true;
            }
            else if (strArray[0].StartsWith("M") && strArray[2].StartsWith("A"))
            {
                double.TryParse(strArray[0][1..], out arc.x1);//x1
                double.TryParse(strArray[1], out arc.y1);//y1
                double.TryParse(strArray[2][1..], out arc.rx);//rx
                double.TryParse(strArray[3], out arc.ry);//ry
                double.TryParse(strArray[4], out arc.phi);//x-axis-rotation
                int.TryParse(strArray[5], out int fA);//large-arc-flag
                arc.fA = fA == 1;
                int.TryParse(strArray[6], out int fS);//sweep-flag
                arc.fS = fS == 1;
                double.TryParse(strArray[7], out arc.x2);//x2
                double.TryParse(strArray[8], out arc.y2);//y2
                return true;
            }
            else
            {
                string[] twoParts = ArcPath.Split('A');
                if (twoParts[0].StartsWith("M"))
                {
                    strArray = twoParts[0].Split(' ');
                    double.TryParse(strArray[0][1..], out arc.x1);
                    double.TryParse(strArray[1], out arc.y1);
                    strArray = twoParts[1].Split(' ');
                    double.TryParse(strArray[0], out arc.rx);
                    double.TryParse(strArray[1], out arc.ry);
                    double.TryParse(strArray[2], out arc.phi);
                    int.TryParse(strArray[3], out int fA);
                    arc.fA = fA == 1;
                    int.TryParse(strArray[4], out int fS);
                    arc.fS = fS == 1;
                    double.TryParse(strArray[5], out arc.x2);
                    double.TryParse(strArray[6], out arc.y2);
                    return true;
                }
            }
            arc = new SvgArcPath();
            return false;
        }
        public static ArcParams SvgArcPathToArc(SvgArcPath svgArc)
        {
            return SvgArcToArcParam(svgArc);
        }
    }
}
