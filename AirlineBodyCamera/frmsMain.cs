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
using Edward;
using System.Threading;

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
                                        updateMessage(slstInfo, "U盘已插入，盘符为:" + drive.Name.ToString());
                                        /// Thread.Sleep(1000);
                                        DestinFolder = drive.Name.ToString();
                                        sbtnCopyFile.Enabled = true;
                                    
                                        sbtnOpenUDisk.Enabled = true;
                                        if (schkSetCopy.Enabled)
                                            //AutoCopyBodyFile();


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
                                updateMessage(slstInfo, "U盘已卸载！");
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
                updateMessage(slstInfo, "Error:" + ex.Message);

            }
            base.WndProc(ref m);
        }
        #endregion




        public delegate void AddFile();//定度委托
        /// <summary>
        /// 在线程上执行委托
        /// </summary>
        public void SetAddFile()
        {
            this.Invoke(new AddFile(RunAddFile));//在线程上执行指定的委托
        }

        /// <summary>
        /// 对文件进行复制，并在复制完成后关闭线程
        /// </summary>
        public void RunAddFile()
        {
            FileInfo fi = new FileInfo(stxtFilePath.Text.Trim());
            string ToFile = string.Empty;
            ToFile = DestinFolder + fi.Name;
            CopyFile(stxtFilePath.Text.Trim(), ToFile, 4096, tsPbar);//复制文件
            // CopyFile(txtFilePath.Text.Trim(), ToFile, 1024, pbarUpdate);//复制文件
            thdAddFile.Abort();//关闭线程
        }

        /// <summary>
        /// 文件的复制
        /// </summary>
        /// <param FormerFile="string">源文件路径</param>
        /// <param toFile="string">目的文件路径</param> 
        /// <param SectSize="int">传输大小</param> 
        /// <param progressBar="ProgressBar">ProgressBar控件</param> 
        public void CopyFile(string FormerFile, string toFile, int SectSize, ProgressBar progressBar1)
        {
            progressBar1.Value = 0;//设置进度栏的当前位置为0
            progressBar1.Minimum = 0;//设置进度栏的最小值为0
            FileStream fileToCreate = new FileStream(toFile, FileMode.Create);//创建目的文件，如果已存在将被覆盖
            fileToCreate.Close();//关闭所有资源
            fileToCreate.Dispose();//释放所有资源
            FileStream FormerOpen = new FileStream(FormerFile, FileMode.Open, FileAccess.Read);//以只读方式打开源文件
            FileStream ToFileOpen = new FileStream(toFile, FileMode.Append, FileAccess.Write);//以写方式打开目的文件
            int max = Convert.ToInt32(Math.Ceiling((double)FormerOpen.Length / (double)SectSize));//根据一次传输的大小，计算传输的个数
            progressBar1.Maximum = max;//设置进度栏的最大值
            int FileSize;//要拷贝的文件的大小
            if (SectSize < FormerOpen.Length)//如果分段拷贝，即每次拷贝内容小于文件总长度
            {
                byte[] buffer = new byte[SectSize];//根据传输的大小，定义一个字节数组
                int copied = 0;//记录传输的大小
                int tem_n = 1;//设置进度栏中进度块的增加个数
                while (copied <= ((int)FormerOpen.Length - SectSize))//拷贝主体部分
                {
                    Application.DoEvents();
                    FileSize = FormerOpen.Read(buffer, 0, SectSize);//从0开始读，每次最大读SectSize
                    FormerOpen.Flush();//清空缓存
                    ToFileOpen.Write(buffer, 0, SectSize);//向目的文件写入字节
                    ToFileOpen.Flush();//清空缓存
                    ToFileOpen.Position = FormerOpen.Position;//使源文件和目的文件流的位置相同
                    copied += FileSize;//记录已拷贝的大小
                    progressBar1.Value = progressBar1.Value + tem_n;//增加进度栏的进度块
                }
                int left = (int)FormerOpen.Length - copied;//获取剩余大小
                FileSize = FormerOpen.Read(buffer, 0, left);//读取剩余的字节
                FormerOpen.Flush();//清空缓存
                ToFileOpen.Write(buffer, 0, left);//写入剩余的部分
                ToFileOpen.Flush();//清空缓存
            }
            else//如果整体拷贝，即每次拷贝内容大于文件总长度
            {
                byte[] buffer = new byte[FormerOpen.Length];//获取文件的大小
                FormerOpen.Read(buffer, 0, (int)FormerOpen.Length);//读取源文件的字节
                FormerOpen.Flush();//清空缓存
                ToFileOpen.Write(buffer, 0, (int)FormerOpen.Length);//写放字节
                ToFileOpen.Flush();//清空缓存
            }
            FormerOpen.Close();//释放所有资源
            ToFileOpen.Close();//释放所有资源
            updateMessage(slstInfo, "升级文件拷贝完成.");
            // p.WriteLog("升级文件拷贝完成.");
            updateMessage(slstInfo, "请拔掉USB数据线，静待系统自动升级.");
            //  p.WriteLog("请拔掉USB数据线，静待系统自动升级.");
            progressBar1.Value = 0;
            sbtnOpenFile.Enabled = true;
            sbtnUpdataFile.Enabled = true;

        }






        /// <summary>
        /// 文件的复制
        /// </summary>
        /// <param FormerFile="string">源文件路径</param>
        /// <param toFile="string">目的文件路径</param> 
        /// <param SectSize="int">传输大小</param> 
        /// <param progressBar="ProgressBar">ProgressBar控件</param> 
        public void CopyFile(string FormerFile, string toFile, int SectSize, ToolStripProgressBar tsPbar)
        {
            sbtnCopyFile.Enabled = false;
            bCopyFile = true;
            sbtnUpdataFile.Enabled = false;
            //progressBar1.Value = 0;//设置进度栏的当前位置为0
            //progressBar1.Minimum = 0;//设置进度栏的最小值为0
            tsPbar.Visible = true;
            tsPbar.Value = 0;
            tsPbar.Minimum = 0;
            FileStream fileToCreate = new FileStream(toFile, FileMode.Create);//创建目的文件，如果已存在将被覆盖
            fileToCreate.Close();//关闭所有资源
            fileToCreate.Dispose();//释放所有资源
            FileStream FormerOpen = new FileStream(FormerFile, FileMode.Open, FileAccess.Read);//以只读方式打开源文件
            FileStream ToFileOpen = new FileStream(toFile, FileMode.Append, FileAccess.Write);//以写方式打开目的文件
            int max = Convert.ToInt32(Math.Ceiling((double)FormerOpen.Length / (double)SectSize));//根据一次传输的大小，计算传输的个数
            //  progressBar1.Maximum = max;//设置进度栏的最大值
            tsPbar.Maximum = max;
            int FileSize;//要拷贝的文件的大小
            if (SectSize < FormerOpen.Length)//如果分段拷贝，即每次拷贝内容小于文件总长度
            {
                byte[] buffer = new byte[SectSize];//根据传输的大小，定义一个字节数组
                int copied = 0;//记录传输的大小
                int tem_n = 1;//设置进度栏中进度块的增加个数
                while (copied <= ((int)FormerOpen.Length - SectSize))//拷贝主体部分
                {
                    Application.DoEvents();
                    FileSize = FormerOpen.Read(buffer, 0, SectSize);//从0开始读，每次最大读SectSize
                    FormerOpen.Flush();//清空缓存
                    ToFileOpen.Write(buffer, 0, SectSize);//向目的文件写入字节
                    ToFileOpen.Flush();//清空缓存
                    ToFileOpen.Position = FormerOpen.Position;//使源文件和目的文件流的位置相同
                    copied += FileSize;//记录已拷贝的大小
                    //progressBar1.Value = progressBar1.Value + tem_n;//增加进度栏的进度块
                    tsPbar.Value = tsPbar.Value + tem_n;

                }
                int left = (int)FormerOpen.Length - copied;//获取剩余大小
                FileSize = FormerOpen.Read(buffer, 0, left);//读取剩余的字节
                FormerOpen.Flush();//清空缓存
                ToFileOpen.Write(buffer, 0, left);//写入剩余的部分
                ToFileOpen.Flush();//清空缓存
            }
            else//如果整体拷贝，即每次拷贝内容大于文件总长度
            {
                byte[] buffer = new byte[FormerOpen.Length];//获取文件的大小
                FormerOpen.Read(buffer, 0, (int)FormerOpen.Length);//读取源文件的字节
                FormerOpen.Flush();//清空缓存
                ToFileOpen.Write(buffer, 0, (int)FormerOpen.Length);//写放字节
                ToFileOpen.Flush();//清空缓存
            }
            FormerOpen.Close();//释放所有资源
            ToFileOpen.Close();//释放所有资源
            updateMessage(slstInfo, "升级文件拷贝完成.");
            // p.WriteLog("升级文件拷贝完成.");
            updateMessage(slstInfo, "请拔掉USB数据线，静待系统自动升级.");
            //  p.WriteLog("请拔掉USB数据线，静待系统自动升级.");
            //  progressBar1.Value = 0;
            tsPbar.Value = 0;
            sbtnOpenFile.Enabled = true;
            sbtnUpdataFile.Enabled = true;
            bCopyFile = false;
            sbtnUpdataFile.Enabled = true;
            sbtnCopyFile.Enabled = true;
            tsPbar.Visible = false;

        }



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
                schkSetCopy.Checked = true;
            if (p.SetCopy == "0")
                schkSetCopy.Checked = false;

            stxtCopyFileDestPath.Text = p.CopyDestFolder;
          //  stxtCopyFileDestPath.Select(this.stxtCopyFileDestPath.Text.Length , 0);//光标定位到文本最后
            stxtFilePath.Text = p.UpdateFile;

            InitUI();
            bRestart = false;
            // Control.CheckForIllegalCrossThreadCalls = false;
            this.TopMost = true;
        }


        //UI初始化设置
        public void InitUI()
        {
            //按钮
            sbtnLogin.Enabled = false;
            sbtnExit.Enabled = false;
            sbtnEdit.Enabled = false;
            sbtnOpenUDisk.Enabled = false;
            sbtnOpenFile.Enabled = false;
            sbtnSyncTime.Enabled = false;
            sbtnEnbaleUDisk.Enabled = false;
            sbtnCheckDevice.Enabled = true;
            sbtnRestart.Enabled = true;
            scomboUserID.Enabled = false;
            scomboUserID.SelectedIndex = -1;
            slblBattery.Text = "电量:NA";
            slblResolution.Text = "分辨率:NA";
            sbtnUpdataFile.Enabled = false;
            sbtnChangePwd.Enabled = false;
            sbtnChangePwdOK.Enabled = false;
            scomboIDType.Enabled = false;
            scomboIDType.SelectedIndex = -1;
            sbtnReadDeviceInfo.Enabled = false;
            sbtnCopyFile.Enabled = false;
            //文本编辑框
            stxtDevicePassword.Text = string.Empty;
            stxtDevicePassword.ReadOnly = true;
            stxtFilePath.ReadOnly = true;
            stxtUnitID.ReadOnly = true;
            stxtDevID.ReadOnly = true;
            stxtUserID.ReadOnly = true;
            stxtUserName.ReadOnly = true;
            stxtNewPwd1.ReadOnly = true;
            stxtNewPwd2.ReadOnly = true;
            stxtUnitName.ReadOnly = true;
            stxtCopyFileDestPath.ReadOnly = true;
            LoginDevice = DeviceType.NA;
            ClearDeviceInfo();

        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearDeviceInfo()
        {
            stxtDevID.Text = string.Empty;
            stxtUnitID.Text = string.Empty;
            stxtUnitName.Text = string.Empty;
            stxtUserID.Text = string.Empty;
            stxtUserName.Text = string.Empty;

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




        /// <summary>
        /// 同步时间
        /// </summary>
        /// <param name="devicetype"></param>
        /// <param name="password"></param>
        /// <returns>true 成功,false 失败</returns>
        private bool SyncDeviceTime(DeviceType devicetype, string password)
        {
            int SyncDevTime_iRet = -1;
            if (LoginDevice == DeviceType.Cammpro)
            {
                ZFYDLL_API_MC.SyncDevTime(password, ref SyncDevTime_iRet);
                if (SyncDevTime_iRet == 5)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获取登陆设备的分辨率
        /// </summary>
        /// <param name="devicetype"></param>
        /// <param name="password"></param>
        /// <param name="deviceresolution"></param>
        /// <returns>true 成功,false 失败</returns>
        private bool GetDeviceResolution(DeviceType devicetype, string password, out DeviceResolution deviceresolution)
        {
            deviceresolution = new DeviceResolution();
            int Resolution_Width = -1;
            int Resolution_Height = -1;
            int fps = -1;
            int bps = -1;
            int _ReadDeviceResolution_iRet = -1;


            if (LoginDevice == DeviceType.Cammpro)
                ZFYDLL_API_MC.ReadDeviceResolution(ref  Resolution_Width, ref  Resolution_Height, password, ref _ReadDeviceResolution_iRet);
            //if (LoginDevice == DeviceType.H8)
            //    BODYCAMDLL_API_YZ.ReadDeviceResolution(ref  Resolution_Width, ref  Resolution_Height, password, ref _ReadDeviceResolution_iRet);
            if (_ReadDeviceResolution_iRet == 1)
            {
                // this.tb_Resolution.Text = Resolution_Width.ToString() + " X " + Resolution_Height.ToString();
                //updateMessage(lb_StateInfo, "获取视频分辨率参数成功.");
                deviceresolution.Resolution_Width = Resolution_Width;
                deviceresolution.Resolution_Height = Resolution_Height;

                if (LoginDevice == DeviceType.EasyStorage)
                {
                    deviceresolution.Fps = fps;
                    deviceresolution.Bps = bps;
                }

                return true;
            }

            return false;
        }


        /// <summary>
        /// 获取设备硬件信息,序列号等
        /// </summary>
        /// <param name="devicetype"></param>
        /// <param name="password"></param>
        /// <param name="deviceinfo"></param>
        /// <returns>true 成功,false 不成功</returns>
        private bool GetDeviceInfo(DeviceType devicetype, string password, out DeviceInfo deviceinfo)
        {
            deviceinfo = new DeviceInfo();
            int GetZFYInfo_iRet = -1;
            if (LoginDevice == DeviceType.Cammpro)
            {
                ZFYDLL_API_MC.ZFY_INFO uuDevice = new ZFYDLL_API_MC.ZFY_INFO();//执法仪结构信息定义
                ZFYDLL_API_MC.GetZFYInfo(ref uuDevice, password, ref GetZFYInfo_iRet);
                if (GetZFYInfo_iRet == 1)
                {

                    deviceinfo.cSerial = System.Text.Encoding.Default.GetString(uuDevice.cSerial);
                    deviceinfo.userNo = System.Text.Encoding.Default.GetString(uuDevice.userNo);
                    deviceinfo.userName = System.Text.Encoding.Default.GetString(uuDevice.userName);
                    deviceinfo.unitNo = System.Text.Encoding.Default.GetString(uuDevice.unitNo);
                    deviceinfo.unitName = System.Text.Encoding.Default.GetString(uuDevice.unitName);
                    return true;
                }
                else
                    return false;
            }
            return false;

        }


        /// <summary>
        /// 点击登陆
        /// </summary>
        private void LogIn()
        {
            DevicePassword = stxtDevicePassword.Text.Trim();
            if (string.IsNullOrEmpty(DevicePassword))
            {
                updateMessage(slstInfo, "密码不能为空,请重新输入.");
                stxtDevicePassword.Focus();
                return;
            }

            int Battery_iRet = -1;
            int BatteryLevel = -1;
            switch (LoginDevice)
            {
                case DeviceType.NA:
                    break;
                case DeviceType.Cammpro:
                    ZFYDLL_API_MC.ReadDeviceBatteryDumpEnergy(ref BatteryLevel, DevicePassword, ref  Battery_iRet);
                    break;
                case DeviceType.EasyStorage:

                    break;
                default:
                    break;
            }



            if (Battery_iRet != 1)
            {
                updateMessage(slstInfo, "密码错误,登录失败.");
                
                stxtDevicePassword.Focus();
                return;
            }

            //登陆成功
            LogonInitUI();
            //
            bCheckDevice = true;

            //同步时间
            if (SyncDeviceTime(LoginDevice, DevicePassword))
                updateMessage(slstInfo, "自动同步设备时间成功.（" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")");
            else
                updateMessage(slstInfo, "自动设备时间失败.");


            //////////////////////////////////////////////////////////////////////////////
            //执法仪电量
            slblBattery.Text  = "电量:" +BatteryLevel.ToString() + " %";
            tslblBatt.Text = "电量:" + BatteryLevel.ToString() + " %";
            updateMessage(slstInfo, "获取电量值成功(" + BatteryLevel.ToString() + " %).");
            /////////////////////////////////////////////////////////////////////////////
            //获取执法记录仪分辨率
            DeviceResolution DR = new DeviceResolution();
            if (GetDeviceResolution(LoginDevice, DevicePassword, out DR))
            {
               slblResolution.Text  ="分辨率:" + DR.Resolution_Width.ToString() + "*" + DR.Resolution_Height.ToString();
                tslblResolution.Text = "分辨率:" + DR.Resolution_Width.ToString() + "*" + DR.Resolution_Height.ToString();
                updateMessage(slstInfo, "获取视频分辨率参数成功(" + DR.Resolution_Width.ToString() + "*" + DR.Resolution_Height.ToString() + ").");

                if (LoginDevice == DeviceType.EasyStorage)
                    updateMessage(slstInfo, "获取视频码流帧数参数成功(Bps=" + DR.Bps + "bit/s,Fps=" + DR.Fps + "帧/s).");
            }
            ///////////////////////////////////////////////////////////////////////////
            //执法仪信息读取返回值
            DeviceInfo DI = new DeviceInfo();
            if (GetDeviceInfo(LoginDevice, DevicePassword, out DI))
            {
                updateMessage(slstInfo, "获取执法仪本机信息成功.");
                stxtDevID.Text = DI.cSerial; //System.Text.Encoding.Default.GetString(uuDevice.cSerial);
                stxtUserID.Text = DI.userNo; ///System.Text.Encoding.Default.GetString(uuDevice.userNo);
                stxtUserName.Text = DI.userName; // System.Text.Encoding.Default.GetString(uuDevice.userName);
                stxtUnitID.Text = DI.unitNo;  //System.Text.Encoding.Default.GetString(uuDevice.unitNo);
                stxtUnitName.Text = DI.unitName; //System.Text.Encoding.Default.GetString(uuDevice.unitName); 
                tslblSN.Text = "当前设备编号:" + DI.cSerial;
            }
        }



        //登陆后UI初始化设置
        public void LogonInitUI()
        {
            //
            sbtnEdit.Enabled = true;
            sbtnOpenFile.Enabled = false;
            sbtnSyncTime.Enabled = true;
            sbtnEnbaleUDisk.Enabled = true;
            sbtnLogin.Enabled = false;
            sbtnReadDeviceInfo.Enabled = true;
            sbtnOpenUDisk.Enabled = true;
            sbtnChangePwd.Enabled = true;
            scomboUserID.Enabled = false;
            sbtnExit.Enabled = true;
            sbtnOpenUDisk.Enabled = false;
           

            //
            stxtDevicePassword.ReadOnly = true;
         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logindevice"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool SetDeviceMSDC(DeviceType logindevice, string password)
        {
            int iRet_SetMSDC = -1;
            if (logindevice == DeviceType.EasyStorage)
            {
                //BODYCAMDLL_API_YZ.SetMSDC(password, ref iRet_SetMSDC);

                if (iRet_SetMSDC == 1)
                    return true;
                else
                    return false;
            }
            if (logindevice == DeviceType.Cammpro)
            {
                ZFYDLL_API_MC.SetMSDC(password, ref iRet_SetMSDC);
                if (iRet_SetMSDC == 7)
                    return true;
                else
                    return false;
            }

            return false;
        }

        /// <summary>
        /// 向执法仪写入信息
        /// </summary>
        /// <param name="devicetype"></param>
        /// <param name="password"></param>
        /// <param name="deviceinfo"></param>
        /// <returns>true,成功,false失败</returns>
        private bool WriteDeviceInfo(DeviceType devicetype, string password, DeviceInfo deviceinfo)
        {
            int WriteZFYInfo_iRet = -1;
            if (LoginDevice == DeviceType.Cammpro)
            {
                ZFYDLL_API_MC.ZFY_INFO info = new ZFYDLL_API_MC.ZFY_INFO();
                info.cSerial = Encoding.Default.GetBytes(deviceinfo.cSerial.PadRight(8, '\0').ToArray());
                info.userNo = Encoding.Default.GetBytes(deviceinfo.userNo.PadRight(7, '\0').ToArray());
                info.userName = Encoding.Default.GetBytes(deviceinfo.userName.PadRight(33, '\0').ToArray());
                info.unitNo = Encoding.Default.GetBytes(deviceinfo.unitNo.PadRight(13, '\0').ToArray());
                info.unitName = Encoding.Default.GetBytes(deviceinfo.unitName.PadRight(33, '\0').ToArray());
                ZFYDLL_API_MC.WriteZFYInfo(ref info, password, ref WriteZFYInfo_iRet);
            }

            if (WriteZFYInfo_iRet == 1)
                return true;
            return false;

        }


        /// <summary>
        /// 
        /// </summary>
        public class FileInformation
        {

            public string FileName { get; set; }
            public string FilePath { get; set; }
            public string FileDirectory { get; set; }
            public string FileExtension { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class DirectoryAllFiles
        {
            static List<FileInformation> FileList = new List<FileInformation>();
            public static List<FileInformation> GetAllFiles(DirectoryInfo dir)
            {
                FileInfo[] allFile = dir.GetFiles();
                foreach (FileInfo fi in allFile)
                {
                    FileList.Add(new FileInformation
                    {
                        FileName = fi.Name,
                        FilePath = fi.FullName,
                        FileDirectory = fi.DirectoryName,
                        FileExtension = fi.Extension
                    });
                }
                DirectoryInfo[] allDir = dir.GetDirectories();
                foreach (DirectoryInfo d in allDir)
                {
                    GetAllFiles(d);
                }

                return FileList;

            }
        }


        private void AutoCopyBodyFile()
        {
            sgrbUpdateFile.Enabled = false;
            bCopyFile = true;
            sbtnOpenFolder.Enabled = false;
            if (!string.IsNullOrEmpty(stxtCopyFileDestPath.Text.Trim()))
            {
                DirectoryInfo di = new DirectoryInfo(DestinFolder + @"DCIM");
                List<FileInformation> list = new List<FileInformation>();
                list = DirectoryAllFiles.GetAllFiles(new System.IO.DirectoryInfo(DestinFolder + @"DCIM"));
                sbtnCopyFile.Enabled = false;


                foreach (var item in list)
                {

                    this.Invoke((EventHandler)delegate
                    {
                        CopyDestFile(item.FilePath, stxtCopyFileDestPath.Text.Trim() + @"\" + item.FileName, 4096, tsPbar);
                    });

                }
                sbtnCopyFile.Enabled = true;
                sbtnUpdataFile.Enabled = true;
                updateMessage(slstInfo, "复制执勤仪文件完毕.");
            }
            else
            {
                if (Directory.Exists(stxtCopyFileDestPath.Text.Trim()))
                {
                    updateMessage(slstInfo, "存储执勤仪文件的文件夹不存在,请重新选择.");
                    stxtCopyFileDestPath.Focus();
                    stxtCopyFileDestPath.Focus();
                }

            }
            tsPbar.Visible = false;
            sbtnOpenFolder.Enabled = true;
            bCopyFile = false;
            sgrbUpdateFile.Enabled = true;
        }


        /// <summary>
        /// 文件的复制
        /// </summary>
        /// <param FormerFile="string">源文件路径</param>
        /// <param toFile="string">目的文件路径</param> 
        /// <param SectSize="int">传输大小</param> 
        /// <param progressBar="ProgressBar">ProgressBar控件</param> 
        public void CopyDestFile(string FormerFile, string toFile, int SectSize, ToolStripProgressBar tsPbar)
        {
            //progressBar1.Value = 0;//设置进度栏的当前位置为0
            //progressBar1.Minimum = 0;//设置进度栏的最小值为0
            FileInfo fi = new FileInfo(FormerFile);
            updateMessage(slstInfo, "正在复制:" + fi.Name);
            tsPbar.Visible = true;
            tsPbar.Value = 0;
            tsPbar.Minimum = 0;
            FileStream fileToCreate = new FileStream(toFile, FileMode.Create);//创建目的文件，如果已存在将被覆盖
            fileToCreate.Close();//关闭所有资源
            fileToCreate.Dispose();//释放所有资源
            FileStream FormerOpen = new FileStream(FormerFile, FileMode.Open, FileAccess.Read);//以只读方式打开源文件
            FileStream ToFileOpen = new FileStream(toFile, FileMode.Append, FileAccess.Write);//以写方式打开目的文件
            int max = Convert.ToInt32(Math.Ceiling((double)FormerOpen.Length / (double)SectSize));//根据一次传输的大小，计算传输的个数
            //  progressBar1.Maximum = max;//设置进度栏的最大值
            tsPbar.Maximum = max;
            int FileSize;//要拷贝的文件的大小
            if (SectSize < FormerOpen.Length)//如果分段拷贝，即每次拷贝内容小于文件总长度
            {
                byte[] buffer = new byte[SectSize];//根据传输的大小，定义一个字节数组
                int copied = 0;//记录传输的大小
                int tem_n = 1;//设置进度栏中进度块的增加个数
                while (copied <= ((int)FormerOpen.Length - SectSize))//拷贝主体部分
                {
                    Application.DoEvents();
                    FileSize = FormerOpen.Read(buffer, 0, SectSize);//从0开始读，每次最大读SectSize
                    FormerOpen.Flush();//清空缓存
                    ToFileOpen.Write(buffer, 0, SectSize);//向目的文件写入字节
                    ToFileOpen.Flush();//清空缓存
                    ToFileOpen.Position = FormerOpen.Position;//使源文件和目的文件流的位置相同
                    copied += FileSize;//记录已拷贝的大小
                    //progressBar1.Value = progressBar1.Value + tem_n;//增加进度栏的进度块
                    tsPbar.Value = tsPbar.Value + tem_n;

                }
                int left = (int)FormerOpen.Length - copied;//获取剩余大小
                FileSize = FormerOpen.Read(buffer, 0, left);//读取剩余的字节
                FormerOpen.Flush();//清空缓存
                ToFileOpen.Write(buffer, 0, left);//写入剩余的部分
                ToFileOpen.Flush();//清空缓存
            }
            else//如果整体拷贝，即每次拷贝内容大于文件总长度
            {
                byte[] buffer = new byte[FormerOpen.Length];//获取文件的大小
                FormerOpen.Read(buffer, 0, (int)FormerOpen.Length);//读取源文件的字节
                FormerOpen.Flush();//清空缓存
                ToFileOpen.Write(buffer, 0, (int)FormerOpen.Length);//写放字节
                ToFileOpen.Flush();//清空缓存
            }
            FormerOpen.Close();//释放所有资源
            ToFileOpen.Close();//释放所有资源
            // updateMessage(lstInfo, "该文件copy完成.");
            // p.WriteLog("升级文件拷贝完成.");
            //updateMessage(lstInfo, "请拔掉USB数据线，静待系统自动升级.");
            //  p.WriteLog("请拔掉USB数据线，静待系统自动升级.");
            //  progressBar1.Value = 0;
            tsPbar.Value = 0;
            sbtnOpenFile.Enabled = true;
            sbtnUpdataFile.Enabled = true;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="logtype"></param>
        /// <param name="idtype"></param>
        /// <param name="oldpassword"></param>
        /// <param name="newpassword"></param>
        /// <returns></returns>
        private bool SetPwd(DeviceType logtype, ComboBox idtype, string oldpassword, string newpassword)
        {
            int iRet_SetPwd = -1;
            if (logtype == DeviceType.Cammpro)
            {
                //byte[] pwd = new byte[6];
                //pwd = Encoding.Default.GetBytes(newpassword.PadRight(6, '\0').ToArray());
                if (idtype.SelectedIndex == 0) //admin
                    //ZFYDLL_API_MC.SetAdminPassWord(newpassword, oldpassword, ref iRet_SetPwd);
                    ZFYDLL_API_MC.SetAdminPassWord(newpassword, oldpassword, ref iRet_SetPwd);


                if (idtype.SelectedIndex == 1) //user
                    ZFYDLL_API_MC.SetUserPassWord(newpassword, oldpassword, ref iRet_SetPwd);

                if (iRet_SetPwd == 7)
                    return true;
            }
            return false;
        }



        private void sbtnCheckDevice_Click(object sender, EventArgs e)
        {
            int Init_Device_iRet = -1;
            try
            {
                //Commpro
                ZFYDLL_API_MC.Init_Device(IDCode, ref  Init_Device_iRet);
                if (Init_Device_iRet == 1)
                {
                    LoginDevice = DeviceType.Cammpro;
                    updateMessage(slstInfo, "检测设备成功.");
                    sbtnLogin.Enabled = true;
                    sbtnCheckDevice.Enabled = false;
                    scomboUserID.SelectedIndex = 0;
                    scomboUserID.Enabled = true;
                    stxtDevicePassword.ReadOnly = false;
                   // stxtDevicePassword.Focus();
                    
                    return;
                }
                else
                {
                    updateMessage(slstInfo, "未发现可登陆的设备,请重新连接设备重试.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void sbtnLogin_Click(object sender, EventArgs e)
        {
            LogIn();
        }

        private void stxtDevicePassword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

          //  MessageBox.Show(e.IsInputKey.ToString());
     
        }

        private void sbtnRestart_Click(object sender, EventArgs e)
        {
            if (!bCopyFile)
            {
                DialogResult dr = MessageBox.Show("是否确认重启软件,退出点击是(Y),不退出点击否(N)?", "Restart?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    bRestart = true;
                    Application.Restart();

                }
            }
        }

        private void sbtnExit_Click(object sender, EventArgs e)
        {
            if (!bCopyFile)
            {
                if (LoginDevice == DeviceType.EasyStorage)
                    updateMessage(slstInfo, "退出登录成功");
                bCheckDevice = false;
                InitUI();
                bCheckDevice = false;

            }
        }

        private void sbtnEdit_Click(object sender, EventArgs e)
        {
            if (sbtnEdit.Text == "编辑")
            {
                sbtnEdit.Text = "保存";
                this.stxtDevID.ReadOnly = false;
                this.stxtUserID.ReadOnly = false;
                this.stxtUserName.ReadOnly = false;
                this.stxtUnitID.ReadOnly = false;
                this.stxtUnitName.ReadOnly = false;

            }
            else
            {
                sbtnEdit.Text = "编辑";
                this.stxtDevID.ReadOnly = true;
                this.stxtUserID.ReadOnly = true;
                this.stxtUserName.ReadOnly = true;
                this.stxtUnitID.ReadOnly = true;
                this.stxtUnitName.ReadOnly = true;
                DeviceInfo di = new DeviceInfo();
                di.cSerial = this.stxtDevID.Text;
                di.userNo = this.stxtUserID.Text;
                di.userName = this.stxtUserName.Text;
                di.unitNo = this.stxtUnitID.Text;
                di.unitName = this.stxtUnitName.Text;

                if (WriteDeviceInfo(LoginDevice, DevicePassword, di))
                {
                    updateMessage(slstInfo, "向执法仪写入信息成功.");
                    tslblSN.Text = "当前设备编号:" + di.cSerial;
 
                }

            }
        }

        private void sbtnCopyFile_Click(object sender, EventArgs e)
        {

            AutoCopyBodyFile();
        }

        private void sbtnOpenFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog open = new FolderBrowserDialog();
            open.Description = "请选择执法仪文件需要存在的位置";
            if (open.ShowDialog() == DialogResult.OK)
            {
                stxtCopyFileDestPath.Text = open.SelectedPath;
                //stxtCopyFileDestPath.Select(this.stxtCopyFileDestPath.Text.Length , 0);//光标定位到文本最后
            }
            p.CopyDestFolder = stxtCopyFileDestPath.Text.Trim();
            IniFile.IniWriteValue("SysConfig", "CopyDestFolder", p.CopyDestFolder, p.IniPath);
        }

        private void stxtCopyFileDestPath_Paint(object sender, PaintEventArgs e)
        {

        }

        private void sbtnSyncTime_Click(object sender, EventArgs e)
        {
            //同步时间
            if (SyncDeviceTime(LoginDevice, DevicePassword))
                updateMessage(slstInfo, "手动同步设备时间成功.（" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")");
            else
                updateMessage(slstInfo, "手动设备时间失败.");
        }

        private void sbtnEnbaleUDisk_Click(object sender, EventArgs e)
        {
            if (SetDeviceMSDC(LoginDevice, DevicePassword))
            {
                sbtnEdit.Enabled = false;
                sbtnChangePwd.Enabled = false;
                sbtnReadDeviceInfo.Enabled = false;
                sbtnSyncTime.Enabled = false;
                sbtnEnbaleUDisk.Enabled = false;
                sbtnOpenFile.Enabled = true;
                sbtnUpdataFile.Enabled = true;
                stxtFilePath.Enabled = true;

                updateMessage(slstInfo, "执法仪已进入U盘模式.");

            }
        }

        private void sbtnOpenUDisk_Click(object sender, EventArgs e)
        {
            if (DestinFolder != "" && Directory.Exists(DestinFolder))
            {
                System.Diagnostics.Process.Start(@DestinFolder);
            }
            else
            {
                updateMessage(slstInfo, "打开磁盘无效.");
            }
        }

        private void sbtnChangePwd_Click(object sender, EventArgs e)
        {
            if (sbtnChangePwd.Text == "修改密码")
            {

               // grbChangePassword.Enabled = true;
                scomboIDType.SelectedIndex = 0;
                scomboIDType.Enabled = true;
                //btn_ChangePWd.Enabled = false;
                sbtnChangePwd.Text = "放弃修改";
                stxtNewPwd1.ReadOnly = false;
                stxtNewPwd2.ReadOnly = false;
                sbtnChangePwdOK.Enabled = true;
            }
            else
            {

               // grbChangePassword.Enabled = false;
                //btn_ChangePWd.Enabled = 
                scomboIDType.SelectedIndex = -1;
                scomboIDType.Enabled = false;
                sbtnChangePwd.Text = "修改密码";
                stxtNewPwd1.ReadOnly = true;
                stxtNewPwd2.ReadOnly = true;
                stxtNewPwd1.Text = string.Empty;
                stxtNewPwd2.Text = string.Empty;
                sbtnChangePwdOK.Enabled = false;
            }
        }

        private void sbtnChangePwdOK_Click(object sender, EventArgs e)
        {
            if (scomboIDType.SelectedIndex == -1)
            {
                updateMessage(slstInfo, "请要修改密码的账号.");
                scomboIDType.Focus();
                return;
            }
            if (string.IsNullOrEmpty(stxtNewPwd1.Text.Trim()))
            {
                updateMessage(slstInfo, "密码不能为空,请重新输入.");
                stxtNewPwd1.Focus();
                return;
            }
            if (stxtNewPwd1.Text.Trim() != stxtNewPwd2.Text.Trim())
            {
                updateMessage(slstInfo, "两次输入的新密码不一致,请重新输入");
              //  stxtNewPwd1.SelectAll();
                stxtNewPwd1.Focus();
                return;
            }



            DialogResult dr = MessageBox.Show("是否确认修改密码,修改密码后如果忘记,可能需要刷机才可以重置密码.修改点击是(Y),不修改点击否(N)?", "修改密码?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                if (SetPwd(LoginDevice, scomboIDType, DevicePassword, stxtNewPwd1.Text.Trim()))
                {
                    updateMessage(slstInfo, "修改" + scomboIDType.Text + "的密码成功");
                    stxtNewPwd1.Text = string.Empty;
                   stxtNewPwd2.Text = string.Empty;
                    //grbChangePassword.Enabled = false;
                    sbtnChangePwd.Enabled = true;
                    sbtnChangePwd.Text = "修改密码";
                }
            }
        }

        private void sbtnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofg = new OpenFileDialog();
            ofg.Filter = "升级文件(*.Bin)|*.Bin";
            if (ofg.ShowDialog() == DialogResult.OK)//打开文件对话框
            {
                stxtFilePath.Text = ofg.FileName;//获取源文件的路径
                FileInfo fi = new FileInfo(ofg.FileName);
                p.UpdateFile = stxtFilePath.Text.Trim();
                IniFile.IniWriteValue("SysConfig", "UpdateFile", p.UpdateFile, p.IniPath);
                //stxtFilePath.Select(this.txtFilePath.TextLength, 0);//光标定位到文本最后
            }
        }

        private void sbtnUpdataFile_Click(object sender, EventArgs e)
        {
            if (File.Exists(stxtFilePath.Text.Trim()))
            {
                sbtnOpenFile.Enabled = false;
                sbtnUpdataFile.Enabled = false;
                thdAddFile = new Thread(new ThreadStart(SetAddFile));//创建一个线程
                //  thdAddFile.IsBackground = true;
                thdAddFile.Start();//执行当前线程
            }
            else
            {
                updateMessage(slstInfo, "升级文件不存在,请重新确认.");
            }
        }

        private void frmsMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bCopyFile)
                e.Cancel = true;
            else
            {


                if (bRestart)
                {

                }
                else
                {
                    DialogResult dr = MessageBox.Show("是否确认退出软件,退出点击是(Y),不退出点击否(N)?", "Exit?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        Environment.Exit(0);
                    }
                    else
                        e.Cancel = true;
                }
            }
        }
    }
}
