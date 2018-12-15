using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SDK;
using CCWin;
using CCWin.SkinControl;
using CCWin.Win32;
using System.IO;

namespace AirlineBodyCamera
{
    public partial class frmsMain : Skin_Mac
    {
        public frmsMain()
        {
            InitializeComponent();
        }



        #region 参数定义

        public static string IDCode = string.Empty; //执法仪识别码
        //设置密码
        public static string DevicePassword = string.Empty;
        //设备初始化返回值
        private string DestinFolder = string.Empty;//目的文件夹
        private System.Threading.Thread thdAddFile; //创建一个线程

        DeviceType LoginDevice;
        bool bRestart = false;
        bool bCheckDevice = false; //
        bool bCopyFile = false;//是否正在copy文件,复制期间，不能关闭和重启

        public const int WM_DEVICECHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;
        public const int DBT_CONFIGCHANGED = 0x0018;
        public const int DBT_CUSTOMEVENT = 0x8006;
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;
        public const int DBT_DEVNODES_CHANGED = 0x0007;
        public const int DBT_QUERYCHANGECONFIG = 0x0017;
        public const int DBT_USERDEFINED = 0xFFFF;

        /// <summary>
        /// 登陆的设备类型
        /// </summary>
        public enum DeviceType
        {
            NA,
            EasyStorage,
            Cammpro
        }

        /// <summary>
        /// 设备的分辨率
        /// </summary>
        public class DeviceResolution
        {
            public int Resolution_Width { get; set; }
            public int Resolution_Height { get; set; }
            public int Bps { get; set; }
            public int Fps { get; set; }
        }

        /// <summary>
        /// 设备的信息(序列号等等)
        /// </summary>
        public class DeviceInfo
        {
            public string cSerial { get; set; }
            public string userNo { get; set; }
            public string userName { get; set; }
            public string unitNo { get; set; }
            public string unitName { get; set; }

        }

        #endregion


        #region 防止屏幕闪烁
        protected override CreateParams CreateParams
        {
            get
            {

                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }

        }
        #endregion


        #region 动态加载


        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == WM_DEVICECHANGE)
                {
                    switch (m.WParam.ToInt32())
                    {
                        case WM_DEVICECHANGE:
                            break;
                        case DBT_DEVICEARRIVAL://U盘插入

                            if (bCheckDevice)
                            {
                                DriveInfo[] s = DriveInfo.GetDrives();
                                foreach (DriveInfo drive in s)
                                {
                                    if (drive.DriveType == DriveType.Removable)
                                    {
                                        updateMessage(lstInfo, "U盘已插入，盘符为:" + drive.Name.ToString());
                                        /// Thread.Sleep(1000);
                                        DestinFolder = drive.Name.ToString();
                                        btnCopyFile.Enabled = true;
                                        this.btnEjectSD.Enabled = true;
                                        if (chkSetCopy.Enabled)
                                            AutoCopyBodyFile();


                                        break;
                                    }
                                }
                            }
                            break;
                        case DBT_CONFIGCHANGECANCELED:
                            break;
                        case DBT_CONFIGCHANGED:
                            break;
                        case DBT_CUSTOMEVENT:
                            break;
                        case DBT_DEVICEQUERYREMOVE:
                            break;
                        case DBT_DEVICEQUERYREMOVEFAILED:
                            break;
                        case DBT_DEVICEREMOVECOMPLETE: //U盘卸载
                            if (bCheckDevice)
                            {
                                updateMessage(lstInfo, "U盘已卸载！");
                            }
                            break;
                        case DBT_DEVICEREMOVEPENDING:
                            break;
                        case DBT_DEVICETYPESPECIFIC:
                            break;
                        case DBT_DEVNODES_CHANGED:
                            break;
                        case DBT_QUERYCHANGECONFIG:
                            break;
                        case DBT_USERDEFINED:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                updateMessage(lstInfo, "Error:" + ex.Message);

            }
            base.WndProc(ref m);
        }
        #endregion


        private void frmsMain_Load(object sender, EventArgs e)
        {
            load();
        }




        private void load()
        {
            //禁止Form窗口调整大小方法：
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            //不能使用最大化窗口： 
            this.MaximizeBox = false;

            this.Location = new Point(
            (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
            (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
             );

            //不能使用最小化窗口： 
            this.MinimizeBox = false;
            //私版
            this.Text = "执勤记录仪管理软件,Ver:" + Application.ProductVersion;

            if (p.SetCopy == "1")
                chkSetCopy.Checked = true;
            if (p.SetCopy == "0")
                chkSetCopy.Checked = false;

            txtCopyFileDestPath.Text = p.CopyDestFolder;
            txtCopyFileDestPath.Select(this.txtCopyFileDestPath.TextLength, 0);//光标定位到文本最后
            txtFilePath.Text = p.UpdateFile;

            InitUI();
            bRestart = false;
            // Control.CheckForIllegalCrossThreadCalls = false;
            this.TopMost = true;
        }


        #region 更新信息
        /// <summary>
        /// 更新信息到listbox中
        /// </summary>
        /// <param name="slistbox">listbox name</param>
        /// <param name="message">message</param>
        public static void updateMessage(SkinListBox slistbox, string message)
        {
            if (slistbox.Items.Count > 1000)
                slistbox.Items.RemoveAt(0);
           // string item = string.Empty;
            //listbox.Items.Add("");
            SkinListBoxItem item  = new SkinListBoxItem();
             item .Text = DateTime.Now.ToString("HH:mm:ss") + " " + @message;
            slistbox.Items.Add(item);
            if (slistbox.Items.Count > 1)
            {
                slistbox.TopIndex = slistbox.Items.Count - 1;
                slistbox.SetSelected(slistbox.Items.Count - 1, true);
            }
        }
        #endregion
    }
}
