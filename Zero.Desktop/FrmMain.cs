using System.Reflection;
using NewLife;
using NewLife.Reflection;
using XCode.DataAccessLayer;

namespace Zero.Desktop;

public partial class FrmMain : Form
{
    DAL _dal;

    public FrmMain()
    {
        InitializeComponent();
    }

    private void FrmMain_Load(Object sender, EventArgs e)
    {
        var asm = AssemblyX.Create(Assembly.GetExecutingAssembly());
        Text = String.Format("{2} v{0} {1:HH:mm:ss}", asm.FileVersion, asm.Compile, Text);

        cbConns.DataSource = DAL.ConnStrs.Keys;
    }

    private void btnOpen_Click(Object sender, EventArgs e)
    {
        var connName = cbConns.SelectedItem as String;
        if (connName.IsNullOrEmpty()) return;

        var btn = sender as Button;
        if (btn.Text == "打开")
        {
            _dal = DAL.Create(connName);
            listBox1.DataSource = _dal.Tables.ToList();

            cbConns.Enabled = false;
            groupBox2.Enabled = true;
            btn.Text = "关闭";
        }
        else
        {
            cbConns.Enabled = true;
            groupBox2.Enabled = false;
            btn.Text = "打开";
        }
    }

    private void listBox1_SelectedIndexChanged(Object sender, EventArgs e)
    {
        var table = listBox1.SelectedItem as IDataTable;
        if (table == null) return;

        var sql = $"select * from {table.TableName}";
        var ds = _dal.Select(new SelectBuilder(sql), 0, 1000);

        dataGridView1.DataSource = ds.Tables[0];
        dataGridView1.Refresh();
    }
}