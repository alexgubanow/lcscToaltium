namespace lcsclib
{
    public enum PADoffsets
    {
        command,//PAD for pad entry
        shape,
        center_x,// divide by 100 to get inches
        center_y,// divide by 100 to get inches
        width,// x10 to get mil
        height,// x10 to get mil
        layer_id,//11 (All)
        net,
        Designator,
        hole_radius,// x10 to get mil
        outline_points,// divide by 100 to get inches
        rotation,//degrees
        id,
        Hole_Length,// x10 to get mil
        Hole_Points, // slot hole from to point
        Plated,//Y/N
        locked,
        unknow1,
        SolderMaskExpansion,
        unknow2
    }
    public enum ARCoffsets
    {
        command,
        stroke_width,
        layer_id,
        net,
        path_string,
        helper_dots,
        id,
        locked
    }
    public enum CIRCLEoffsets
    {
        command,
        center_x,
        center_y,
        radius,
        stroke_width,
        layer_id,
        id,
        locked
    }
    public enum HOLEoffsets
    {
        command,
        center_x,
        center_y,
        radius,
        id,
        locked
    }
    public enum TRACKoffsets
    {
        command,
        stroke_width,
        layer_id,
        net,
        points,
        id,
        locked
    }
    public enum RECToffsets
    {
        command,
        x,
        y,
        width,
        height,
        layer_id,
        id,
        fill,
        stroke_width,
        locked
    }

}