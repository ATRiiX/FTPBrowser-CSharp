using System;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using FluentFTP;
using System.Net;

namespace FTPClient
{
    public partial class MainForm : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        public MainForm()
        {
            InitializeComponent();

            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            // Add dummy data to the listview

        }


        private void MaterialButtonsetting2_Click(object sender, EventArgs e)
        {
            materialSkinManager.Theme = materialSkinManager.Theme == MaterialSkinManager.Themes.DARK ? MaterialSkinManager.Themes.LIGHT : MaterialSkinManager.Themes.DARK;
        }

        private int colorSchemeIndex;

        private void MaterialButtonsetting1_Click(object sender, EventArgs e)
        {
            colorSchemeIndex++;
            if (colorSchemeIndex > 2) colorSchemeIndex = 0;

            //These are just example color schemes
            switch (colorSchemeIndex)
            {
                case 0:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
                    break;
                case 1:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);
                    break;
                case 2:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Green600, Primary.Green700, Primary.Green200, Accent.Red100, TextShade.WHITE);
                    break;
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            materialTabControl1.SelectedTab = tabPage1;
        }

        FtpClient client;
        bool isconnect = true;
        private void materialButtonConnect_Click(object sender, EventArgs e)
        {
            
            //登录！！
            client = new FtpClient(materialSingleLineTextField1.Text);
            client.Credentials = new NetworkCredential(materialSingleLineTextField2.Text, materialSingleLineTextField3.Text);
            try
            { client.Connect(); isconnect = true; }
            catch (Exception ee)
            {
                MessageBox.Show("Connect Failed! Please retry again");
                isconnect = false;
            }

            if (isconnect)
            {
                tabPage2.Text = "CONNECTED";
                materialTabControl1.SelectedTab = tabPage2;
                materialSingleLineTextField1.Enabled = false;
                materialSingleLineTextField2.Enabled = false;
                materialSingleLineTextField3.Enabled = false;
                materialButtonDisConnect.Enabled = true;
                materialButtonDisConnect.Visible = true;
                materialButtonConnect.Enabled = false;
                materialButtonConnect.Visible = false;
                var data1 = new[] { "..", "","",""};
                var item1 = new ListViewItem(data1);
                materialListView1.Items.Add(item1);
                foreach (FtpListItem itemm in client.GetListing("/"))
                {
                    long size = 0;
                    // if this is a file
                    if (itemm.Type == FtpFileSystemObjectType.File)
                    {

                        // get the file size
                        size = client.GetFileSize(itemm.FullName);

                    }

                    // get modified date/time of the file or folder
                    DateTime time = client.GetModifiedTime(itemm.FullName);

                     data1 = new[] { itemm.FullName, size.ToString(), itemm.Type.ToString(), itemm.RawPermissions };
                     item1 = new ListViewItem(data1);
                    materialListView1.Items.Add(item1);


                }


                this.Text = "ftp://" + materialSingleLineTextField1.Text + client.GetWorkingDirectory();




            }




            //存入history
            var data =
                new[] { DateTime.Now.ToString(), materialSingleLineTextField1.Text, materialSingleLineTextField2.Text, materialSingleLineTextField3.Text };
            var item = new ListViewItem(data);
            materialListView2.Items.Insert(0, item);


        }

        private void materialFlatButton4_Click(object sender, EventArgs e)
        {
            if (materialTabControl1.SelectedTab == tabPage1)
            {
                materialSingleLineTextField1.Text = "";
                materialSingleLineTextField2.Text = "";
                materialSingleLineTextField3.Text = "";
            }
            if (materialTabControl1.SelectedTab == tabPage0)
            {
                materialListView2.Items.Clear();
            }
        }

        private void materialListView2_DoubleClick(object sender, EventArgs e)
        {



        }

        private void materialSingleLineTextField1_Click(object sender, EventArgs e)
        {

        }

        private void materialButtonDisConnect_Click(object sender, EventArgs e)
        {
            client.Disconnect();
            isconnect = false;
            tabPage2.Text = "UNCONNECTED";
            materialSingleLineTextField1.Enabled = true;
            materialSingleLineTextField2.Enabled = true;
            materialSingleLineTextField3.Enabled = true;
            materialButtonDisConnect.Enabled = false;
            materialButtonDisConnect.Visible = false;
            materialButtonConnect.Enabled = true;
            materialButtonConnect.Visible = true;
            materialListView1.Items.Clear();
            Text = "FTP Client";
        }

        private void materialTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialTabControl1.SelectedTab == tabPage0 || materialTabControl1.SelectedTab == tabPage1)
            {
                materialFlatButton4.Enabled = true;
                materialFlatButton3.Enabled = false;
                materialFlatButton2.Enabled = false;
                materialFlatButton1.Enabled = false;
                materialFlatButton5.Enabled = false;
            }
            else
            if (materialTabControl1.SelectedTab == tabPage2)
            {
                materialFlatButton4.Enabled = false;
                materialFlatButton3.Enabled = true;
                materialFlatButton2.Enabled = true;
                materialFlatButton1.Enabled = true;
                materialFlatButton5.Enabled = true;
            }
            else
            {
                materialFlatButton4.Enabled = false;
                materialFlatButton3.Enabled = false;
                materialFlatButton2.Enabled = false;
                materialFlatButton1.Enabled = false;
                materialFlatButton5.Enabled = false;
            }
        }

        private void materialListView1_DoubleClick(object sender, EventArgs e)
        {
            //选中list中项目双击
            var a = materialListView1.SelectedItems[0].SubItems[0].Text;
            if (a=="..")
            {
                a = client.GetWorkingDirectory();
            }
            if (client.DirectoryExists(a))
            {
                materialListView1.Items.Clear();
                var data1 = new[] { "..", "", "", "" };
                var item1 = new ListViewItem(data1);
                materialListView1.Items.Add(item1);
                foreach (FtpListItem itemm in client.GetListing(a))
                {
                    long size = 0;
                    // if this is a file
                    if (itemm.Type == FtpFileSystemObjectType.File)
                    {
                        // get the file size
                        size = client.GetFileSize(itemm.FullName);
                    }
                    // get modified date/time of the file or folder
                    DateTime time = client.GetModifiedTime(itemm.FullName);

                    data1 = new[] { itemm.FullName, size.ToString(), itemm.Type.ToString(), itemm.RawPermissions };
                    item1 = new ListViewItem(data1);
                    materialListView1.Items.Add(item1);

                }


                this.Text = "ftp://" + materialSingleLineTextField1.Text + a;
            }

        }

        private void materialListView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
