// Create nodes
TreeNode rootNode = new() { Name = "Root" };
TreeNode nodeA = new() { Name = "Node A" };
TreeNode nodeB = new() { Name = "Node B" };
TreeNode nodeC = new() { Name = "Node C" };
TreeNode nodeD = new() { Name = "Node D" };
TreeNode nodeAA = new() { Name = "Node AA" };
TreeNode nodeAB = new() { Name = "Node AB" };
TreeNode nodeBA = new() { Name = "Node BA" };
TreeNode nodeBB = new() { Name = "Node BB" };
TreeNode nodeCA = new() { Name = "Node CA" };
TreeNode nodeCB = new() { Name = "Node CB" };
TreeNode nodeDA = new() { Name = "Node DA" };
TreeNode nodeDB = new() { Name = "Node DB" };

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


static void DisplayTree(TreeNode node, int level)
{
    string indentation = new(' ', level * 4);
    Console.WriteLine($"{indentation}{node.Name}");
    foreach (var child in node.Children)
    {
        DisplayTree(child, level + 1);
    }
}
class TreeNode
{
    public string Name { get; set; }
    public List<TreeNode> Children { get; set; } = new List<TreeNode>();
}
