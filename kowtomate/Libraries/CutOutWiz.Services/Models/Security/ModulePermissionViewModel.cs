
namespace CutOutWiz.Services.Models.Security
{
    public class ModulePermissionViewModel
    {
        public string ModuleObjectId { get; set; }
        public string ModuleName { get; set; }
        public string PermissionObjectId { get; set; }
        public string DisplayName { get; set; }
        public string PermissionValue { get; set; }
        public decimal DisplayOrder { get; set; }
    }

    public class TreeNode
    {
        public string Id    { get; set; }   
        public string Name { get; set; }

        public string NodeType { get; set; }
        //public bool IsSelected { get; set; }

        public List<TreeNode> ChildNodes { get; set; }
    }



}
