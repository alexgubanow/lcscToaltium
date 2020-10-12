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
}