using System;
using System.IO;
using System.Windows.Forms;

public class FileManager : Form
{
    private TreeView treeView;
    private TextBox textBox;
    private Button backButton;
    private string currentPath;

    public FileManager()
    {
        // Initialize components
        treeView = new TreeView { Dock = DockStyle.Left, Width = 300 };
        textBox = new TextBox { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Both };
        backButton = new Button { Text = "Back", Dock = DockStyle.Top };

        backButton.Click += BackButton_Click;
        treeView.NodeMouseDoubleClick += TreeView_NodeMouseDoubleClick;

        Controls.Add(treeView);
        Controls.Add(textBox);
        Controls.Add(backButton);

        LoadDrives();
    }

    private void LoadDrives()
    {
        treeView.Nodes.Clear();
        foreach (var drive in Directory.GetLogicalDrives())
        {
            var node = new TreeNode(drive) { Tag = drive };
            treeView.Nodes.Add(node);
            LoadDirectories(node);
        }
    }

    private void LoadDirectories(TreeNode node)
    {
        try
        {
            string path = (string)node.Tag;
            foreach (var dir in Directory.GetDirectories(path))
            {
                var childNode = new TreeNode(Path.GetFileName(dir)) { Tag = dir };
                node.Nodes.Add(childNode);
                childNode.Nodes.Add("..."); // Placeholder for lazy loading
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void TreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
        string path = (string)e.Node.Tag;

        if (Directory.Exists(path))
        {
            currentPath = path;
            LoadDirectoryContent(path);
        }
        else if (File.Exists(path))
        {
            OpenFile(path);
        }
    }

    private void LoadDirectoryContent(string path)
    {
        treeView.Nodes.Clear();
        var rootNode = new TreeNode(path) { Tag = path };
        treeView.Nodes.Add(rootNode);
        LoadDirectories(rootNode);
        LoadFiles(rootNode);
        rootNode.Expand();
    }

    private void LoadFiles(TreeNode node)
    {
        string path = (string)node.Tag;
        foreach (var file in Directory.GetFiles(path))
        {
            var childNode = new TreeNode(Path.GetFileName(file)) { Tag = file };
            node.Nodes.Add(childNode);
        }
    }

    private void OpenFile(string path)
    {
        try
        {
            textBox.Text = File.ReadAllText(path);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void BackButton_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(currentPath))
        {
            string parentPath = Directory.GetParent(currentPath)?.FullName;
            if (parentPath != null)
            {
                currentPath = parentPath;
                LoadDirectoryContent(parentPath);
            }
            else
            {
                LoadDrives();
            }
        }
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FileManager());
    }
}
