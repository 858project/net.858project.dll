using System;
using System.Management;
using System.Security.Cryptography;
using System.Security;
using System.Collections;
using System.Text;

namespace Project858
{
    /// <summary>
    /// Vytvori 16 bitovy unikatny kluc na identifikaciu hardware
    /// </summary>
    public class FingerPrint
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public FingerPrint()
        {

        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get) Unikatny kluc hardware. Example: 4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9
        /// </summary>
        public string Value
        {
            get
            {
                try
                {
                    if (String.IsNullOrEmpty(fingerPrint))
                    {
                        fingerPrint = GetHash("CPU >> " + cpuId() + "\n" +
                                              "BIOS >> " + biosId() + "\n" +
                                              "BASE >> " + baseId() + "\n" +
                                              "DISK >> " + diskId() + "\n" +
                                              "VIDEO >> " + videoId() + "\n" +
                                              "MAC >> " + macId());
                    }
                    return fingerPrint;
                }
                catch (Exception)
                {
                    //vratime prazdy retazec
                    return String.Empty;
                }
            }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Seriove / jedinecne cislo PC
        /// </summary>
        private String fingerPrint = String.Empty;
        #endregion

        #region - Method -
        /// <summary>
        /// Zahesuje vstupny string
        /// </summary>
        /// <param name="s">String ktory chcem hesovat</param>
        /// <returns>Zahesovany unikatny kluc</returns>
        private string GetHash(string s)
        {
            MD5 sec = new MD5CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt));
        }
        /// <summary>
        /// Vrati hesovaci string z dat
        /// </summary>
        /// <param name="bt">Data ktore chcem zahesovat na hexa</param>
        /// <returns>Zahesovane data</returns>
        private string GetHexString(byte[] bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int n, n1, n2;
                n = (int)b;
                n1 = n & 15;
                n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + (int)'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + (int)'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }
        #endregion

        #region Original Device ID Getting Code
        /// <summary>
        /// Vrati hardwrovy identifikator
        /// </summary>
        /// <param name="wmiClass"></param>
        /// <param name="wmiProperty"></param>
        /// <param name="wmiMustBeTrue"></param>
        /// <returns>Hardwarovy identifikator</returns>
        private string identifier (string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = "";
            ManagementClass mc =  new ManagementClass(wmiClass);
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    //Only get the first one
                    if (result == "")
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Vrati hardwrovy identifikator
        /// </summary>
        /// <param name="wmiClass"></param>
        /// <param name="wmiProperty"></param>
        /// <returns>Hardwarovy identifikator</returns>
        private string identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                //Only get the first one
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Vrati ID CPU
        /// </summary>
        /// <returns>Info</returns>
        private string cpuId()
        {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            string retVal = identifier("Win32_Processor", "UniqueId");
            if (retVal == "") //If no UniqueID, use ProcessorID
            {
                retVal = identifier("Win32_Processor", "ProcessorId");
                if (retVal == "") //If no ProcessorId, use Name
                {
                    retVal = identifier("Win32_Processor", "Name");
                    if (retVal == "") //If no Name, use Manufacturer
                    {
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }
                    //Add clock speed for extra security
                    retVal += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return retVal;
        }
        /// <summary>
        /// BIOS Identifier
        /// </summary>
        /// <returns>Info</returns>
        private string biosId()
        {
            return identifier("Win32_BIOS", "Manufacturer")
            + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
            + identifier("Win32_BIOS", "IdentificationCode")
            + identifier("Win32_BIOS", "SerialNumber")
            + identifier("Win32_BIOS", "ReleaseDate")
            + identifier("Win32_BIOS", "Version");
        }
        /// <summary>
        /// Main physical hard drive ID
        /// </summary>
        /// <returns>info</returns>
        private string diskId()
        {
            return identifier("Win32_DiskDrive", "Model")
            + identifier("Win32_DiskDrive", "Manufacturer")
            + identifier("Win32_DiskDrive", "Signature")
            + identifier("Win32_DiskDrive", "TotalHeads");
        }
        /// <summary>
        /// Motherboard ID
        /// </summary>
        /// <returns>info</returns>
        private string baseId()
        {
            return identifier("Win32_BaseBoard", "Model")
            + identifier("Win32_BaseBoard", "Manufacturer")
            + identifier("Win32_BaseBoard", "Name")
            + identifier("Win32_BaseBoard", "SerialNumber");
        }
        /// <summary>
        /// Primary video controller ID
        /// </summary>
        /// <returns>info</returns>
        private string videoId()
        {
            return identifier("Win32_VideoController", "DriverVersion")
            + identifier("Win32_VideoController", "Name");
        }
        /// <summary>
        /// First enabled network card ID
        /// </summary>
        /// <returns>info</returns>
        private string macId()
        {
            return identifier("Win32_NetworkAdapterConfiguration",
                "MACAddress", "IPEnabled");
        }
        #endregion
    }
}