class TreeNode
{
    public string Name { get; set; }
    public List<TreeNode> Children { get; set; } = new List<TreeNode>();
}

class Program
{
    static void Main(string[] args)
    {
        // Create nodes
        TreeNode rootNode = new TreeNode { Name = "Root" };

        TreeNode nodeA = new TreeNode { Name = "Node A" };
        TreeNode nodeB = new TreeNode { Name = "Node B" };
        TreeNode nodeC = new TreeNode { Name = "Node C" };
        TreeNode nodeD = new TreeNode { Name = "Node D" };

        TreeNode nodeAA = new TreeNode { Name = "Node AA" };
        TreeNode nodeAB = new TreeNode { Name = "Node AB" };
        TreeNode nodeBA = new TreeNode { Name = "Node BA" };
        TreeNode nodeBB = new TreeNode { Name = "Node BB" };
        TreeNode nodeCA = new TreeNode { Name = "Node CA" };
        TreeNode nodeCB = new TreeNode { Name = "Node CB" };
        TreeNode nodeDA = new TreeNode { Name = "Node DA" };
        TreeNode nodeDB = new TreeNode { Name = "Node DB" };

        // Build tree structure
        rootNode.Children.Add(nodeA);
        rootNode.Children.Add(nodeB);
        rootNode.Children.Add(nodeC);
        rootNode.Children.Add(nodeD);

        nodeA.Children.Add(nodeAA);
        nodeA.Children.Add(nodeAB);
        nodeB.Children.Add(nodeBA);
        nodeB.Children.Add(nodeBB);
        nodeC.Children.Add(nodeCA);
        nodeC.Children.Add(nodeCB);
        nodeD.Children.Add(nodeDA);
        nodeD.Children.Add(nodeDB);

        // Display tree structure
        Console.WriteLine("Tree Structure:");
        DisplayTree(rootNode, 0);

        Console.ReadLine();
    }

    static void DisplayTree(TreeNode node, int level)
    {
        string indentation = new string(' ', level * 4);
        Console.WriteLine($"{indentation}{node.Name}");

        foreach (var child in node.Children)
        {
            DisplayTree(child, level + 1);
        }
    }
}
