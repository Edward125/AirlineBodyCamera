using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SDK;

namespace AirlineBodyCamera
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }


        #region 参数定义

        public static string IDCode = string.Empty; //执法仪识别码
        //设置密码
        public static string DevicePassword = string.Empty;
        //设备初始化返回值

        DeviceType LoginDevice;


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


        #region 更新信息
        /// <summary>
        /// 更新信息到listbox中
        /// </summary>
        /// <param name="listbox">listbox name</param>
        /// <param name="message">message</param>
        public static void updateMessage(ListBox listbox, string message)
        {
            if (listbox.Items.Count > 1000)
                listbox.Items.RemoveAt(0);

            string item = string.Empty;
            //listbox.Items.Add("");
            item = DateTime.Now.ToString("HH:mm:ss") + " " + @message;
            listbox.Items.Add(item);
            if (listbox.Items.Count > 1)
            {
                listbox.TopIndex = listbox.Items.Count - 1;
                listbox.SetSelected(listbox.Items.Count - 1, true);
            }
        }
        #endregion


        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void btn_CheckDev_Click(object sender, EventArgs e)
        {
            int Init_Device_iRet = -1;

            try
            {
                 //Commpro
                 ZFYDLL_API_MC.Init_Device(IDCode, ref  Init_Device_iRet);
                 if (Init_Device_iRet == 1)
                 {
                     LoginDevice = DeviceType.Cammpro;
                     updateMessage(lb_StateInfo, "检测设备成功.");
                     this.btnLogon.Enabled = true;
                     this.btnCheckDev.Enabled = false;
                     this.tb_Password.Enabled = true;
                     this.tb_Password.Focus();
                     this.comboUserID.SelectedIndex = 0;
                     comboUserID.Enabled = true;
                     return;
                 }
                else 
                 {
                     updateMessage(lb_StateInfo, "未发现可登陆的设备,请重新连接设备重试.");
                 }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }

        private void btn_Logon_Click(object sender, EventArgs e)
        {
            LogIn();
        }



        /// <summary>
        /// 点击登陆
        /// </summary>
        private void LogIn()
        {
            DevicePassword = tb_Password.Text;
            if (string.IsNullOrEmpty(DevicePassword))
            {
                updateMessage(lb_StateInfo, "密码不能为空,请重新输入.");
                tb_Password.Focus();
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
                updateMessage(lb_StateInfo, "密码错误,登录失败.");
                tb_Password.SelectAll();
                tb_Password.Focus();
                return;
            }

            //登陆成功
            LogonInitUI();

            //同步时间
            if (SyncDeviceTime(LoginDevice, DevicePassword))
                updateMessage(lb_StateInfo, "自动同步设备时间成功.（" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")");
            else
                updateMessage(lb_StateInfo, "自动设备时间失败.");


            //////////////////////////////////////////////////////////////////////////////
            //执法仪电量
            this.tb_Battery.Text = BatteryLevel.ToString() + " %";
            updateMessage(lb_StateInfo, "获取电量值成功(" + BatteryLevel.ToString() + " %).");
            /////////////////////////////////////////////////////////////////////////////
            //获取执法记录仪分辨率
            DeviceResolution DR = new DeviceResolution();
            if (GetDeviceResolution(LoginDevice, DevicePassword, out DR))
            {
                this.tb_Resolution.Text = DR.Resolution_Width.ToString() + "*" + DR.Resolution_Height.ToString();
                updateMessage(lb_StateInfo, "获取视频分辨率参数成功(" + DR.Resolution_Width.ToString() + "*" + DR.Resolution_Height.ToString() + ").");

                if (LoginDevice == DeviceType.EasyStorage)
                    updateMessage(lb_StateInfo, "获取视频码流帧数参数成功(Bps=" + DR.Bps + "bit/s,Fps=" + DR.Fps + "帧/s).");
            }
            ///////////////////////////////////////////////////////////////////////////
            //执法仪信息读取返回值
            DeviceInfo DI = new DeviceInfo();
            if (GetDeviceInfo(LoginDevice, DevicePassword, out DI))
            {
                updateMessage(lb_StateInfo, "获取执法仪本机信息成功.");
                this.tb_DevID.Text = DI.cSerial; //System.Text.Encoding.Default.GetString(uuDevice.cSerial);
                this.tb_UserID.Text = DI.userNo; ///System.Text.Encoding.Default.GetString(uuDevice.userNo);
                this.tb_UserName.Text = DI.userName; // System.Text.Encoding.Default.GetString(uuDevice.userName);
                this.tb_UnitID.Text = DI.unitNo;  //System.Text.Encoding.Default.GetString(uuDevice.unitNo);
                this.tb_UnitName.Text = DI.unitName; //System.Text.Encoding.Default.GetString(uuDevice.unitName);        
            }

            return;

        }



        //登陆后UI初始化设置
        public void LogonInitUI()
        {

            this.btnEdit.Enabled = true;
            //this.btn_ChangePassword.Enabled = true;
            this.btn_FilePathChose.Enabled = false;
            //this.btn_Format.Enabled = false;
            this.btn_SyncDevTime.Enabled = true;
            this.btn_SetMSDC.Enabled = true;
            //this.btn_UpdataFile.Enabled = true;
            //文本编辑框
            this.tb_FilePath.Enabled = false;
            //this.tb_NewPassword.Enabled = true;
            //this.tb_SDCapacity.Enabled = true;
            this.tb_UnitID.Enabled = true;
            this.tb_UnitName.Enabled = true;
            this.tb_UserID.Enabled = true;
            this.tb_UserName.Enabled = true;
            this.tb_DevID.Enabled = true;

            //this.cb_FileType.Enabled = false; 

            this.pg_Updata.Enabled = true;
            //this.cb_FileType.Text = "FAT32";


            //
            this.btnLogon.Enabled = false;
            this.tb_Password.Enabled = false;
            this.btnExit.Enabled = true;
            //
            this.tb_DevID.Enabled = false;
            this.tb_UserID.Enabled = false;
            this.tb_UserName.Enabled = false;
            this.tb_UnitID.Enabled = false;
            this.tb_UnitName.Enabled = false;
            this.btnReadDeviceInfo.Enabled = true;




            grbChangePassword.Enabled = false;
            comboUserID.Enabled = false;
            btn_ChangePWd.Enabled = true;


            //btn_FilePathChose.Enabled = true;
            //btn_UpdataFile.Enabled = true;

        }


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

        private void btnReadDeviceInfo_Click(object sender, EventArgs e)
        {
            ClearDeviceInfo();
            ///////////////////////////////////////////////////////////////////////////
            //执法仪信息读取返回值
            DeviceInfo DI = new DeviceInfo();
            if (GetDeviceInfo(LoginDevice, DevicePassword, out DI))
            {
                updateMessage(lb_StateInfo, "获取执法仪本机信息成功.");
                this.tb_DevID.Text = DI.cSerial; //System.Text.Encoding.Default.GetString(uuDevice.cSerial);
                this.tb_UserID.Text = DI.userNo; ///System.Text.Encoding.Default.GetString(uuDevice.userNo);
                this.tb_UserName.Text = DI.userName; // System.Text.Encoding.Default.GetString(uuDevice.userName);
                this.tb_UnitID.Text = DI.unitNo;  //System.Text.Encoding.Default.GetString(uuDevice.unitNo);
                this.tb_UnitName.Text = DI.unitName; //System.Text.Encoding.Default.GetString(uuDevice.unitName);  
                btnEdit.Text = "编辑";
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void ClearDeviceInfo()
        {
            tb_DevID.Text = string.Empty;
            tb_UnitID.Text = string.Empty;
            tb_UnitName.Text = string.Empty;
            tb_UserID.Text = string.Empty;
            tb_UserName.Text = string.Empty;

        }

        private void btn_SyncDevTime_Click(object sender, EventArgs e)
        {
            //同步时间
            if (SyncDeviceTime(LoginDevice, DevicePassword))
                updateMessage(lb_StateInfo, "手动同步设备时间成功.（" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")");
            else
                updateMessage(lb_StateInfo, "手动设备时间失败.");
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

        private void btn_SetMSDC_Click(object sender, EventArgs e)
        {
            if (SetDeviceMSDC(LoginDevice, DevicePassword))
            {
                this.btnEdit.Enabled = false;
                btn_ChangePWd.Enabled = false;
                btnReadDeviceInfo.Enabled = false;
                this.btn_SyncDevTime.Enabled = false;
                this.btn_FilePathChose.Enabled = true;
                this.btn_UpdataFile.Enabled = true;
                this.btn_SetMSDC.Enabled = false;
                tb_FilePath.Enabled = true;
                updateMessage(lb_StateInfo, "执法仪已进入U盘模式.");

            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (btnEdit.Text == "编辑")
            {
                btnEdit.Text = "保存";
                this.tb_DevID.Enabled = false;
                this.tb_UserID.Enabled = false;
                this.tb_UserName.Enabled = false;
                this.tb_UnitID.Enabled = false;


                DeviceInfo di = new DeviceInfo();
                if (LoginDevice == DeviceType.EasyStorage)
                    this.tb_UserID.Text = this.tb_UserID.Text.PadRight(6, '0');

                di.cSerial = this.tb_DevID.Text;
                di.userNo = this.tb_UserID.Text;
                di.userName = this.tb_UserName.Text;
                di.unitNo = this.tb_UnitID.Text;
                di.unitName = this.tb_UnitName.Text;


                if (WriteDeviceInfo(LoginDevice, DevicePassword, di))
                    updateMessage(lb_StateInfo, "向执法仪写入信息成功.");

            }
            else
                btnEdit.Text = "编辑";
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

        private void btn_FilePathChose_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofg = new OpenFileDialog();
            ofg.Filter = "升级文件(*.Bin)|*.Bin";
            if (ofg.ShowDialog() == DialogResult.OK)//打开文件对话框
            {
                tb_FilePath.Text = ofg.FileName;//获取源文件的路径
            }
        }

        private void btn_UpdataFile_Click(object sender, EventArgs e)
        {

            //str = tb_FilePath.Text;//记录源文件的路径
            //str = "\\" + str.Substring(str.LastIndexOf('\\') + 1, str.Length - str.LastIndexOf('\\') - 1);//获取源文件的名称

            //if (str != "")
            //{
            //    //MessageBox.Show(str);
            //    thdAddFile = new Thread(new ThreadStart(SetAddFile));//创建一个线程
            //    thdAddFile.Start();//执行当前线程
            //}
            //else
            //{
            //    updateMessage(lb_StateInfo, "未选择目的文件，当前不能升级！");
            //}
        }

        private void btnChangePwdOK_Click(object sender, EventArgs e)
        {
            if (comboIDType.SelectedIndex == -1)
            {
                updateMessage(lb_StateInfo, "请要修改密码的账号.");
                comboIDType.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtNewPwd1.Text.Trim()))
            {
                updateMessage(lb_StateInfo, "密码不能为空,请重新输入.");
                txtNewPwd1.Focus();
                return;
            }
            if (txtNewPwd1.Text.Trim() != txtNewPwd2.Text.Trim())
            {
                updateMessage(lb_StateInfo, "两次输入的新密码不一致,请重新输入");
                txtNewPwd1.SelectAll();
                txtNewPwd1.Focus();
                return;
            }



            DialogResult dr = MessageBox.Show("是否确认修改密码,修改密码后如果忘记,可能需要刷机才可以充值密码.修改点击是(Y),不修改点击否(N)?", "Exit?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                if (SetPwd(LoginDevice, comboIDType, DevicePassword, txtNewPwd1.Text.Trim()))
                {
                    updateMessage(lb_StateInfo, "修改" + comboIDType.Text + "的密码成功");
                    txtNewPwd1.Text = string.Empty;
                    txtNewPwd2.Text = string.Empty;
                    grbChangePassword.Enabled = false;
                    btn_ChangePWd.Enabled = true;
                    btn_ChangePWd.Text = "修改密码";
                }
            }
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

        private void btn_ChangePWd_Click(object sender, EventArgs e)
        {
            if (btn_ChangePWd.Text == "修改密码")
            {

                grbChangePassword.Enabled = true;
                comboIDType.SelectedIndex = 0;
                //btn_ChangePWd.Enabled = false;
                btn_ChangePWd.Text = "放弃修改";
            }
            else
            {

                grbChangePassword.Enabled = false;
                //btn_ChangePWd.Enabled = 
                comboIDType.SelectedIndex = -1;
                btn_ChangePWd.Text = "修改密码";
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (LoginDevice == DeviceType.EasyStorage)
            updateMessage(lb_StateInfo, "退出登录成功");
            InitUI();
        }

        //UI初始化设置
        public void InitUI()
        {
            //按钮
            this.btnLogon.Enabled = false;
            this.btnExit.Enabled = false;
            this.btnEdit.Enabled = false;
            this.btn_EcjetSD.Enabled = false;
            this.btn_FilePathChose.Enabled = false;
            this.btn_SyncDevTime.Enabled = false;
            this.btn_SetMSDC.Enabled = false;
            btnCheckDev.Enabled = true;
            btnRestart.Enabled = true;
            comboUserID.Enabled = false;
            comboUserID.SelectedIndex = -1;
            tb_Password.Text = string.Empty;
            tb_Battery.Text = string.Empty;
            tb_Resolution.Text = string.Empty;
           //文本编辑框
            this.tb_Password.Enabled = false;
            this.tb_FilePath.Enabled = false;
            this.tb_UnitID.Enabled = false;
            this.tb_UnitName.Enabled = false;
            this.tb_UserID.Enabled = false;
            this.tb_UserName.Enabled = false;
            this.tb_DevID.Enabled = false;
            this.pg_Updata.Enabled = false;
            this.btn_UpdataFile.Enabled = false;
            btn_ChangePWd.Enabled = false;
            LoginDevice = DeviceType.NA;
            tb_FilePath.Enabled = false;
            ClearDeviceInfo();

        }
    }
}
