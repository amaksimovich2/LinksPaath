namespace LinksGraph.Bl.Models
{
    public class TreeNode<TValueType>
    {
        public TValueType Value { get; set; }
        public List<TreeNode<TValueType>> Children { get; set; }

        public TreeNode(TValueType value)
        {
            Value = value;
            Children = new List<TreeNode<TValueType>>();
        }
    }

    public class Tree<TValueType>
    {
        public TreeNode<TValueType> Root { get; set; }

        public Tree(TreeNode<TValueType> root)
        {
            Root = root;
        }
    }
}