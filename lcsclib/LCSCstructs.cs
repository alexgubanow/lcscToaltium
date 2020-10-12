using System.Text.Json;

namespace lcsclib
{
    public struct GenericResponse
    {
        public bool success { get; set; }
        public int code { get; set; }
        public JsonElement result { get; set; }
    }
    public struct Owner
    {
        public string uuid { get; set; }
        public string username { get; set; }
        public string nickname { get; set; }
        public string avatar { get; set; }
    }
    public struct ComponentResponse
    {
        public Owner owner { get; set; }
        public string uuid { get; set; }
        public string description { get; set; }
        public int docType { get; set; }
        public JsonElement dataStr { get; set; }
        public string title { get; set; }
        public string[] tags { get; set; }
        public int type { get; set; }
        public int updateTime { get; set; }
        public string updated_at { get; set; }
        public bool writable { get; set; }
        public bool isFavorite { get; set; }
        public JsonElement packageDetail { get; set; }
        public bool verify { get; set; }
    }
    public struct FootprintResponse
    {
        public Owner owner { get; set; }
        public string uuid { get; set; }
        public string description { get; set; }
        public int docType { get; set; }
        public Footprint dataStr { get; set; }
        public string title { get; set; }
        public string[] tags { get; set; }
        public int type { get; set; }
        public int updateTime { get; set; }
        public string updated_at { get; set; }
        public bool verify { get; set; }
        public bool writable { get; set; }
        public bool isFavorite { get; set; }
    }
    public struct Footprint
    {
        public JsonElement head { get; set; }
        public string canvas { get; set; }
        public string[] shape { get; set; }
        public string systemColor { get; set; }
        public string[] layers { get; set; }
        public BBox BBox { get; set; }
    }
    public struct BBox
    {
        public double x { get; set; }
        public double y { get; set; }
        public double width { get; set; }
        public double height { get; set; }
    }
}