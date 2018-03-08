using System;
using System.Management;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using Entity;
using OfficeOpenXml;
using System.Data;
using LinqDB.ConnectDB;

namespace SupportServerInfo
{
    public partial class frmSupportServerInfo : Form
    {
        public frmSupportServerInfo()
        {
            InitializeComponent();
        }

        private void frmSupportServerInfo_Load(object sender, EventArgs e)
        {
            BindNetworkDevice();
        }

        #region "Get Computer Information"
        private void BindNetworkDevice()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            if (adapters.Length > 0)
            {
                foreach (NetworkInterface adp in adapters)
                {
                    cmbNetworkDevice.Items.Add(adp.Description);
                }
            }
            SetNetWorkInformation();
        }

        private void SetNetWorkInformation()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["Description"].ToString().Trim() == cmbNetworkDevice.Text)
                {
                    if (Convert.ToBoolean(mo["IPEnabled"]) == true)
                    {
                        string[] arrIP = (string[])(mo["IPAddress"]);
                        lblIPAddrress.Text = arrIP[0];
                        lblMacAddress.Text = mo["MacAddress"].ToString();
                        break;
                    }
                }
            }
        }

        private ComputerEntity GetComInfo(ComputerEntity comInfo)
        {
            Microsoft.VisualBasic.Devices.ComputerInfo info = new Microsoft.VisualBasic.Devices.ComputerInfo();
            comInfo.OSFullName = info.OSFullName;
            //comInfo.OSPlatform = info.OSPlatform;
            comInfo.OSVersion = info.OSVersion;
            comInfo.ComputerName = Environment.MachineName;
            comInfo.IPAddress = lblIPAddrress.Text;
            comInfo.MacAddress = lblMacAddress.Text;
            comInfo.CPUInfo = GetCpuInfo();
            comInfo.RAmInfo = GetRamInfo();
            comInfo.DriveInfo = GetDriveInfo();

            return comInfo;
        }

        private CPUEntity GetCpuInfo() {
            CPUEntity ret = new CPUEntity();
            //'get CPU Usage
            ManagementObjectCollection moReturn;
            ManagementObjectSearcher moSearch = new ManagementObjectSearcher(@"Select * from Win32_Processor");
            moReturn = moSearch.Get();
            foreach (ManagementObject mo in moReturn)
            {
                ret.CPUPercentUsage += Convert.ToDouble(mo["LoadPercentage"]);
            }
            moSearch.Dispose();
            moReturn.Dispose();

            try {
                Int16 _cpu_core = 0;
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", @"SELECT * FROM MSAcpi_ThermalZoneTemperature");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    ret.CPUTemperature = Convert.ToDouble(queryObj["CurrentTemperature"]);
                    _cpu_core += 1;
                }

                ret.CPUTemperature = (ret.CPUTemperature / _cpu_core);
                ret.CPUTemperature = (ret.CPUTemperature / 10) - 273.15;
            }
            catch (Exception ex)
            {
                //_Err = "Management.ManagementException : " & err.Message & vbNewLine & err.StackTrace
            }

            return ret;
        }

        private RAMEntity GetRamInfo()
        {
            RAMEntity ret = new RAMEntity();
            Microsoft.VisualBasic.Devices.ComputerInfo ComInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
            ret.AvailablePhysicalMemoryGB = Math.Round(Convert.ToDouble(ComInfo.AvailablePhysicalMemory / Math.Pow(1024 , 3)), 2);
            ret.TotalPhysicalMemoryGB = Math.Round(Convert.ToDouble(ComInfo.TotalPhysicalMemory / Math.Pow(1024 , 3)), 2);
            double Usage = (((double)ComInfo.TotalPhysicalMemory - (double)ComInfo.AvailablePhysicalMemory) / (double)ComInfo.TotalPhysicalMemory);
            ret.PercentUsageGB = Math.Round(Usage * 100, 2);

            return ret;
        }
        private List<DriveEntity> GetDriveInfo() {
            List<DriveEntity> ret = new List<DriveEntity>();
            DriveInfo[] drives = DriveInfo.GetDrives();
            if (drives.Length > 0)
            {
                foreach (DriveInfo dri in drives)
                {
                    //3 = Local Disk
                    if (dri.DriveType == DriveType.Fixed)
                    {
                        DriveEntity d = new DriveEntity
                        {
                            DriveName = dri.Name.Substring(0, 1),
                            VolumnLabel = dri.VolumeLabel,
                            FreeSpaceGB = Math.Round(Convert.ToDouble(dri.TotalFreeSpace) / Math.Pow(1024 , 3), 2),
                            TotalSizeGB = Math.Round(Convert.ToDouble(dri.TotalSize) / Math.Pow(1024 , 3), 2),
                            PercentUsage = Convert.ToInt16(((dri.TotalSize - dri.TotalFreeSpace) * 100) / dri.TotalSize)
                        };

                        ret.Add(d);
                    }
                }
            }
            
            return ret;
        }
        #endregion

        #region "Database Information"
        private DataTable GetDatabaseFileSize()
        {
            string sql = @"SELECT database_name = DB_NAME(database_id), physical_name,type_desc, ";
            sql += " file_size_mb = CAST((size) * 8. / 1024 AS DECIMAL(8, 2)),";
            sql += " case max_size when -1 then 'Unlimited' else  str((max_size * 8.0) / 1024) end max_size_mb";
            sql += " FROM sys.master_files WITH(NOWAIT)";
            sql += " order by database_name, type_desc desc";

            DataTable dt = SqlDB.ExecuteTable(sql);
            return dt;
        }
        #endregion
        private void GenerateFileExcel()
        {
            ComputerEntity com = new ComputerEntity();
            com = GetComInfo(com);
            
            using (ExcelPackage ep = new ExcelPackage())
            {
                ExcelWorksheet ws = ep.Workbook.Worksheets.Add("Output");
                using (ExcelRange HeadRow = ws.Cells["A1:B1"])
                {
                    HeadRow.Value = "Server Performance Information";
                    HeadRow.Merge = true;
                    HeadRow.Style.Font.Bold = true;
                    HeadRow.Style.Font.Size = 14;
                    HeadRow.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                ws.Cells["A2"].Value = "Report Date :";
                ws.Cells["A3"].Value = "OS Name :";
                ws.Cells["A4"].Value = "Server Name :";
                ws.Cells["A5"].Value = "IP Address :";
                ws.Cells["A6"].Value = "Mac Address :";

                ws.Cells["A8"].Value = "CPU Usage :";
                ws.Cells["A9"].Value = "CPU Temperature :";
                ws.Cells["A10"].Value = "RAM Usage :";
                ws.Cells["A11"].Value = "Disk Usage :";


                ws.Cells["B2"].Value = DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss", new System.Globalization.CultureInfo("en-US"));
                ws.Cells["B3"].Value = com.OSFullName + " (Version " + com.OSVersion + ")";
                ws.Cells["B4"].Value = com.ComputerName;
                ws.Cells["B5"].Value = com.IPAddress;
                ws.Cells["B6"].Value = com.MacAddress;

                ws.Cells["B8"].Value = com.CPUInfo.CPUPercentUsage + " %";
                ws.Cells["B9"].Value = com.CPUInfo.CPUTemperature + " C";
                ws.Cells["B10"].Value = com.RAmInfo.UsagePhysicalMemoryGB + "/" + com.RAmInfo.TotalPhysicalMemoryGB + " GB (" + com.RAmInfo.PercentUsageGB + "%)";
                int i = 11;
                foreach (DriveEntity d in com.DriveInfo)
                {
                    ws.Cells["B" + i.ToString()].Value = d.VolumnLabel + "(" + d.DriveName + ":) " + d.FreeSpaceGB + " GB free of " + d.TotalSizeGB + " (" + (100 - d.PercentUsage) + "%)";
                    i += 1;
                }
                ws.Cells[2, 1, i, 2].AutoFitColumns();

                DataTable dt = GetDatabaseFileSize();
                if (dt.Rows.Count > 0)
                {
                    i++;
                    using (ExcelRange HeadRow = ws.Cells["A" + i.ToString() + ":E" + i.ToString()])
                    {
                        ws.Cells["A" + i.ToString()].Value = "Database File Size";
                        HeadRow.Merge = true;
                        HeadRow.Style.Font.Bold = true;
                        HeadRow.Style.Font.Size = 12;
                        HeadRow.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }
                    i++;

                    //Row Header
                    ws.Cells["A" + i.ToString()].Value = "Database Name";
                    ws.Cells["B" + i.ToString()].Value = "Database File";
                    ws.Cells["C" + i.ToString()].Value = "File Type";
                    ws.Cells["D" + i.ToString()].Value = "File Size(MB)";
                    ws.Cells["E" + i.ToString()].Value = "Maximum Size(MB)";
                    using (ExcelRange HeadRow = ws.Cells["A" + i.ToString() + ":E" + i.ToString()])
                    {
                        HeadRow.Style.Font.Bold = true;
                        HeadRow.Style.Font.Size = 12;
                        HeadRow.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }
                    i++;

                    //Data Row
                    foreach (DataRow dr in dt.Rows)
                    {
                        ws.Cells["A" + i.ToString()].Value = dr["database_name"].ToString();
                        ws.Cells["B" + i.ToString()].Value = dr["physical_name"].ToString();
                        ws.Cells["C" + i.ToString()].Value = dr["type_desc"].ToString();
                        ws.Cells["D" + i.ToString()].Value = dr["file_size_mb"].ToString();
                        ws.Cells["E" + i.ToString()].Value = dr["max_size_mb"].ToString();
                        i++;
                    }
                    i++;
                }

                if (File.Exists(txtOutputFile.Text) == true)
                {
                    File.SetAttributes(txtOutputFile.Text, FileAttributes.Normal);
                    File.Delete(txtOutputFile.Text);
                }

                FileInfo f = new FileInfo(txtOutputFile.Text);
                ep.SaveAs(f);
                System.Threading.Thread.Sleep(5000);
                MessageBox.Show("Complete");
                f = null;
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (cmbNetworkDevice.Text.Trim() == "")
            {
                MessageBox.Show("กรุณาเลือก Network Card", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            GenerateFileExcel();
        }

        private void cmbNetworkDevice_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmbNetworkDevice.Text.Trim() != "")
                SetNetWorkInformation();
        }
    }
}
