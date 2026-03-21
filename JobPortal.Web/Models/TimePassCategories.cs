namespace JobPortal.Web.Models
{
    public class TimePassCategories
    {
        public int PKID { get; set; }
        public string TPCategory { get; set; }
        public int ParentId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string Path { get; set; }

        public string TLevel { get; set; }

        public bool IsFiveLine { get; set; }
    }
}
