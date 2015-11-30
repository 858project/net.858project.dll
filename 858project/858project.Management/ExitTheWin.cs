using System;
using System.Collections.Generic;
using System.Text;
using Project858.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Project858.Management
{
    /// <summary>
    /// Implements methods to exit Windows.
    /// </summary>
    public class ExitTheWin
    {
        #region - Constants -
        /// <summary>
        /// Required to enable or disable the privileges in an access token.
        /// </summary>
        private const int TOKEN_ADJUST_PRIVILEGES = 0x20;
        /// <summary>
        /// Required to query an access token.
        /// </summary>
        private const int TOKEN_QUERY = 0x8;
        /// <summary>
        /// The privilege is enabled.
        /// </summary>
        private const int SE_PRIVILEGE_ENABLED = 0x2;
        /// <summary>
        /// Specifies that the function should search the system message-table resource(s) for the 
        /// requested message.
        /// </summary>
        private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
        /// <summary>
        /// Forces processes to terminate. When this flag is set, 
        /// the system does not send the WM_QUERYENDSESSION and WM_ENDSESSION messages. 
        /// This can cause the applications to lose data. Therefore, you should only use this flag in an 
        /// emergency.
        /// </summary>
        private const int EWX_FORCE = 4;
        #endregion

        #region - Public Static Method -
        /// <summary>
        /// Exits windows (and tries to enable any required access rights, if necesarry).
        /// </summary>
        /// <param name="how">One of the RestartOptions values that specifies how to exit windows.</param>
        /// <param name="force">True if the exit has to be forced, false otherwise.</param>
        /// <exception cref="PrivilegeException">There was an error while requesting a required privilege.</exception>
        /// <exception cref="PlatformNotSupportedException">The requested exit method is not supported on this platform.</exception>
        public static void SystemEvent(SystemEventTypes how, bool force)
        {
            switch (how)
            {
                case SystemEventTypes.Suspend:
                    SuspendSystem(false, force);
                    break;
                case SystemEventTypes.Hibernate:
                    SuspendSystem(true, force);
                    break;
                default:
                    ExitWindows((int)how, force);
                    break;
            }
        }
        /// <summary>
        /// Checks whether a specified method exists on the local computer.
        /// </summary>
        /// <param name="library">The library that holds the method.</param>
        /// <param name="method">The entry point of the requested method.</param>
        /// <returns>True if the specified method is present, false otherwise.</returns>
        public static bool CheckEntryPoint(string library, string method)
        {
            IntPtr libPtr = NativeMethodsNet.LoadLibrary(library);

            if (!libPtr.Equals(IntPtr.Zero))
            {
                if (!NativeMethodsNet.GetProcAddress(libPtr, method).Equals(IntPtr.Zero))
                {
                    NativeMethodsNet.FreeLibrary(libPtr);
                    return true;
                }

                NativeMethodsNet.FreeLibrary(libPtr);
            }
            return false;
        }
        #endregion

        #region - Protected Static Method -
        /// <summary>
        /// Exits windows (and tries to enable any required access rights, if necesarry).
        /// </summary>
        /// <param name="how">One of the RestartOptions values that specifies how to exit windows.</param>
        /// <param name="force">True if the exit has to be forced, false otherwise.</param>
        /// <remarks>This method cannot hibernate or suspend the system.</remarks>
        /// <exception cref="PrivilegeException">There was an error while requesting a required privilege.</exception>
        protected static void ExitWindows(int how, bool force)
        {
            ExitTheWin.EnableToken("SeShutdownPrivilege");

            if (force)
                how = how | EWX_FORCE;

            if (NativeMethodsNet.ExitWindowsEx(how, 0) == 0)
                throw new PrivilegeException(FormatError(Marshal.GetLastWin32Error()));
        }
        /// <summary>
        /// Tries to enable the specified privilege.
        /// </summary>
        /// <param name="privilege">The privilege to enable.</param>
        /// <exception cref="PrivilegeException">There was an error while requesting a required privilege.</exception>
        /// <remarks>Thanks to Michael S. Muegel for notifying us about a bug in this code.</remarks>
        protected static void EnableToken(string privilege)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT || !CheckEntryPoint("advapi32.dll", "AdjustTokenPrivileges"))
                return;
            IntPtr tokenHandle = IntPtr.Zero;
            NativeMethods.LUID privilegeLUID = new NativeMethods.LUID();
            NativeMethods.TOKEN_PRIVILEGES newPrivileges = new NativeMethods.TOKEN_PRIVILEGES();
            NativeMethods.TOKEN_PRIVILEGES tokenPrivileges;

            if (NativeMethodsNet.OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref tokenHandle) == 0)
                throw new PrivilegeException(FormatError(Marshal.GetLastWin32Error()));

            if (NativeMethodsNet.LookupPrivilegeValue("", privilege, ref privilegeLUID) == 0)
                throw new PrivilegeException(FormatError(Marshal.GetLastWin32Error()));

            tokenPrivileges.PrivilegeCount = 1;
            tokenPrivileges.Privileges.Attributes = SE_PRIVILEGE_ENABLED;
            tokenPrivileges.Privileges.pLuid = privilegeLUID;

            int size = 4;

            if (NativeMethodsNet.AdjustTokenPrivileges(tokenHandle, 0, ref tokenPrivileges, 4 + (12 * tokenPrivileges.PrivilegeCount), ref newPrivileges, ref size) == 0)
                throw new PrivilegeException(FormatError(Marshal.GetLastWin32Error()));
        }
        /// <summary>
        /// Suspends or hibernates the system.
        /// </summary>
        /// <param name="hibernate">True if the system has to hibernate, false if the system has to be suspended.</param>
        /// <param name="force">True if the exit has to be forced, false otherwise.</param>
        /// <exception cref="PlatformNotSupportedException">The requested exit method is not supported on this platform.</exception>
        protected static void SuspendSystem(bool hibernate, bool force)
        {
            if (!CheckEntryPoint("powrprof.dll", "SetSuspendState"))
                throw new PlatformNotSupportedException("The SetSuspendState method is not supported on this system!");
            
            NativeMethodsNet.SetSuspendState((int)(hibernate ? 1 : 0), (int)(force ? 1 : 0), 0);
        }
        /// <summary>
        /// Formats an error number into an error message.
        /// </summary>
        /// <param name="number">The error number to convert.</param>
        /// <returns>A string representation of the specified error number.</returns>
        protected static string FormatError(int number)
        {
            try
            {
                StringBuilder buffer = new StringBuilder(255);
                NativeMethodsNet.FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, number, 0, buffer, buffer.Capacity, 0);
                return buffer.ToString();
            }
            catch (Exception)
            {
                return "Unspecified error [" + number.ToString() + "]";
            }
        }
        #endregion
    }
}
