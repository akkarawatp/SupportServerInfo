using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class ComputerEntity
    {
        public ComputerEntity()
        {
            CPUInfo = new CPUEntity { CPUPercentUsage = 0, CPUTemperature = 0 };
            RAmInfo = new RAMEntity { AvailablePhysicalMemoryGB = 0, TotalPhysicalMemoryGB = 0, PercentUsageGB = 0 };
            DriveInfo = new List<DriveEntity>();
        }

        public string OSFullName { get; set; }
        public string OSPlatform { get; set; }
        public string OSVersion { get; set; }
        public string ComputerName { get; set; }
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }

        public CPUEntity CPUInfo { get; set; }
        public RAMEntity RAmInfo { get; set; }
        public List<DriveEntity> DriveInfo { get; set; }
    }

    public class CPUEntity
    {
        public double CPUPercentUsage { get; set; }
        public double CPUTemperature { get; set; }
    }
    public class RAMEntity
    {
        public double AvailablePhysicalMemoryGB { get; set; }
        public double TotalPhysicalMemoryGB { get; set; }
        public double UsagePhysicalMemoryGB
        {
            get
            {
                return TotalPhysicalMemoryGB - AvailablePhysicalMemoryGB;
            }
        }
        public double PercentUsageGB { get; set; }
    }

    public class DriveEntity
    {
        public string DriveName { get; set; }
        public string VolumnLabel { get; set; }
        public double TotalSizeGB { get; set; }
        public double FreeSpaceGB { get; set; }
        public int PercentUsage { get; set; }

    }
}
