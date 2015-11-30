using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Project858.Win32
{
    /// <summary>
    /// Implementacia Native Method z WinApi
    /// </summary>
    public static class NativeMethodsNet
    {
        #region - Method -
        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window.
        /// </param>
        /// <param name="iMsg">
        /// Controls how the window is to be shown. This parameter is ignored the first time an application 
        /// calls ShowWindow, if the program that launched the application provides a STARTUPINFO structure. 
        /// Otherwise, the first time ShowWindow is called, the value should be the value obtained by the 
        /// WinMain function in its nCmdShow parameter. In subsequent calls, this parameter can be one of 
        /// the following values.
        /// </param>
        /// <returns>
        /// If the window was previously visible, the return value is nonzero. 
        /// If the window was previously hidden, the return value is zero.
        /// </returns>
        public static int ShowWindow(IntPtr hWnd, NativeMethods.APICmdShowTypes iMsg)
        {
            return NativeMethods.ShowWindow(hWnd, iMsg);
        }
        /// <summary>
        /// Determines whether the specified window is minimized (iconic).
        /// </summary>
        /// <param name="hWnd">A handle to the window to be tested.</param>
        /// <returns>
        /// If the window is iconic, the return value is nonzero.
        /// If the window is not iconic, the return value is zero.
        /// </returns>
        public static int IsIconic(IntPtr hWnd)
        {
            return NativeMethods.IsIconic(hWnd);
        }
        /// <summary>
        /// The OpenProcessToken function opens the access token associated with a process.
        /// </summary>
        /// <param name="processHandle">
        /// A handle to the process whose access token is opened. 
        /// The process must have the PROCESS_QUERY_INFORMATION access permission.
        /// </param>
        /// <param name="desiredAccess">
        /// Specifies an access mask that specifies the requested types of access to the access token. 
        /// These requested access types are compared with the discretionary access control list (DACL) 
        /// of the token to determine which accesses are granted or denied.
        /// </param>
        /// <param name="tokenHandle">
        /// A pointer to a handle that identifies the newly opened access token when the function returns.
        /// </param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        public static int OpenProcessToken(IntPtr processHandle, NativeMethods.APITokenAccessTypes desiredAccess, out IntPtr tokenHandle)
        {
            return NativeMethods.OpenProcessToken(processHandle, desiredAccess, out tokenHandle);
        }
        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="handle">A valid handle to an open object.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        public static bool CloseHandle(IntPtr handle)
        {
            return NativeMethods.CloseHandle(handle);
        }
        /// <summary>
        /// Allocate a console if application started sender within windows GUI. 
        /// Detects the presence of an existing console associated with the application and
        /// attaches itself recipients it if available.
        /// </summary>
        /// <exception cref="Exception">
        /// Console Allocation Failed
        /// </exception>
        public static void AllocateConsole()
        {
            //pripojenie konzoly
            if (NativeMethods.AttachConsole(Process.GetCurrentProcess().Id))
            {
                //zalogujeme
                Debug.WriteLine("AttachConsole return true");
            }

            //doslo k chyba v API ?
            if (Marshal.GetLastWin32Error() != 0)
            {
                //zalogujeme
                Debug.WriteLine(String.Format("AllocateConsole Win32 Error: (0)", Marshal.GetLastWin32Error()));
            }

            // A console was not allocated, so we need recipients make one.
            if (!NativeMethods.AllocConsole())
            {
                throw new Exception(String.Format("AllocConsole Win32Error = '{0}'",
                    Marshal.GetLastWin32Error()));
            }

        }
        /// <summary>
        /// Detaches the calling process sender its console.
        /// </summary>
        /// <exception cref="Exception">
        /// Win32 Error
        /// </exception>
        public static void FreeConsole()
        {
            if (!NativeMethods.FreeConsole())
            {
                throw new Exception(String.Format("FreeConsole Win32Error = '{0}'",
                    Marshal.GetLastWin32Error()));
            }
        }
        /// <summary>
        /// Activates the environment application window by calling the SetForegroundWindow Win32 API.
        /// </summary>
        /// <param name="hWnd">
        /// Handle recipients the window that should be activated and brought recipients the foreground.
        /// </param>
        /// <exception cref="Exception">
        /// Win32 Error
        /// </exception>
        /// <exception cref="SetForegroundWindow">
        /// Argument is null
        /// </exception>
        public static void SetForegroundWindow(IntPtr hWnd)
        {
            //osetrenie
            if (hWnd == null)
                throw new ArgumentNullException("hWnd");

            if (!NativeMethods.SetForegroundWindow(hWnd))
            {
                throw new Exception(String.Format("SetForegroundWindow Win32Error = '{0}'",
                    Marshal.GetLastWin32Error()));
            }
        }
        /// <summary>
        /// Allows the application to access the Control menu for copying and modification
        /// </summary>
        /// <param name="hWnd">
        /// Window handle
        /// </param>
        /// <param name="bRevert">
        /// Specifies the action to be taken. If bRevert is FALSE, GetSystemMenu returns a handle to a 
        /// copy of the Control menu currently in use. This copy is initially identical to the Control 
        /// menu but can be modified. If bRevert is TRUE, GetSystemMenu resets the Control menu back 
        /// to the default state. The previous, possibly modified, Control menu, if any, is destroyed. 
        /// The return value is undefined in this case.
        /// </param>
        /// <returns>
        /// Identifies a copy of the Control menu if bRevert is FALSE. If bRevert is TRUE, the return 
        /// value is undefined. The returned pointer may be temporary and should not be stored for later use.
        /// </returns>
        public static IntPtr GetSystemMenu(IntPtr hWnd, Boolean bRevert)
        {
            return NativeMethods.GetSystemMenu(hWnd, bRevert);
        }
        /// <summary>
        /// Enables, disables, or dims a menu item.
        /// </summary>
        /// <param name="hMenu">
        /// Menu handle
        /// </param>
        /// <param name="nIDEnableItem">
        /// Specifies the menu item to be enabled, as determined by nEnable. This parameter can specify 
        /// pop-up menu items as well as standard menu items.
        /// </param>
        /// <param name="nEnable">
        /// Specifies the action to take. It can be a combination of MF_DISABLED, MF_ENABLED, or MF_GRAYED, 
        /// with MF_BYCOMMAND or MF_BYPOSITION. These values can be combined by using the bitwise OR 
        /// operator. These values have the following meanings:
        /// </param>
        /// <returns>
        /// Previous state (MF_DISABLED, MF_ENABLED, or MF_GRAYED) or –1 if not valid.
        /// </returns>
        public static NativeMethods.APIEnableMenuItemStatus EnableMenuItem(IntPtr hMenu, int nIDEnableItem, NativeMethods.APIEnableMenuItemStatus nEnable)
        {
            //osetrenie vstupu
            if (nEnable == NativeMethods.APIEnableMenuItemStatus.NOT_VALID)
                throw new ArgumentException("Argument 'nEnable' can not by 'NOT_VALID' !");

            return (NativeMethods.APIEnableMenuItemStatus)NativeMethods.EnableMenuItem(hMenu, nIDEnableItem, (int)nEnable);
        }
        /// <summary>
        /// Updates the position, size, shape, content, and translucency of a layered window.
        /// </summary>
        /// <param name="hwnd">
        /// A handle to a layered window. A layered window is created by specifying WS_EX_LAYERED when creating the 
        /// window with the CreateWindowEx function.
        /// </param>
        /// <param name="hdcDst">
        /// A handle to a DC for the screen. This handle is obtained by specifying NULL when calling the function. 
        /// It is used for palette color matching when the window contents are updated. If hdcDst isNULL, the default 
        /// palette will be used. If hdcSrc is NULL, hdcDst must be NULL.
        /// </param>
        /// <param name="pptDst">
        /// A pointer to a structure that specifies the new screen position of the layered window. If the current 
        /// position is not changing, pptDst can be NULL.
        /// </param>
        /// <param name="psize">
        /// A pointer to a structure that specifies the new size of the layered window. If the size of the window 
        /// is not changing, psize can be NULL. If hdcSrc is NULL, psize must be NULL.
        /// </param>
        /// <param name="hdcSrc">
        /// A handle to a DC for the surface that defines the layered window. This handle can be obtained by calling 
        /// the CreateCompatibleDC function. If the shape and visual context of the window are not changing, hdcSrc 
        /// can be NULL.
        /// </param>
        /// <param name="pprSrc">
        /// A pointer to a structure that specifies the location of the layer in the device context. 
        /// If hdcSrc is NULL, pptSrc should be NULL.
        /// </param>
        /// <param name="crKey">
        /// A structure that specifies the color key to be used when composing the layered window. 
        /// To generate a COLORREF, use the RGB macro.
        /// </param>
        /// <param name="pblend">
        /// A pointer to a structure that specifies the transparency value to be used when composing the layered window.
        /// </param>
        /// <param name="dwFlags">
        /// This parameter can be one of the following values.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. 
        /// To get extended error information, call GetLastError.
        /// </returns>
        public static bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref NativeMethods.BLENDFUNCTION pblend, Int32 dwFlags)
        {
            return NativeMethods.UpdateLayeredWindow(hwnd, hdcDst, ref pptDst, ref psize, hdcSrc, ref pprSrc, crKey, ref pblend, dwFlags);
        }
        /// <summary>
        /// The GetDC function retrieves a handle to a device context (DC) for the client area of a specified window 
        /// or for the entire screen. You can use the returned handle in subsequent GDI functions to draw in the DC. 
        /// The device context is an opaque data structure, whose values are used internally by GDI.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window whose DC is to be retrieved. If this value is NULL, 
        /// GetDC retrieves the DC for the entire screen.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the DC for the specified window's client area. 
        /// If the function fails, the return value is NULL.
        /// </returns>
        public static IntPtr GetDC(IntPtr hWnd)
        {
            return NativeMethods.GetDC(hWnd);
        }
        /// <summary>
        /// This function creates a memory device context (DC) compatible with the specified device.
        /// </summary>
        /// <param name="hDC">
        /// If this handle is NULL, the function creates a memory device context compatible with the application's 
        /// current screen.
        /// </param>
        /// <returns>
        /// The handle to a memory device context indicates success. NULL indicates failure. To get extended error 
        /// information, call GetLastError.
        /// </returns>
        public static IntPtr CreateCompatibleDC(IntPtr hDC)
        {
            return NativeMethods.CreateCompatibleDC(hDC);
        }
        /// <summary>
        /// The ReleaseDC function releases a device context (DC), freeing it for use by other applications. 
        /// The effect of the ReleaseDC function depends on the type of DC. It frees only common and window DCs. 
        /// It has no effect on class or private DCs.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window whose DC is to be released.
        /// </param>
        /// <param name="hDC">
        /// A handle to the DC to be released.
        /// </param>
        /// <returns>
        /// The return value indicates whether the DC was released. If the DC was released, the return value is 1. 
        /// If the DC was not released, the return value is zero.
        /// </returns>
        public static int ReleaseDC(IntPtr hWnd, IntPtr hDC)
        {
            return NativeMethods.ReleaseDC(hWnd, hDC);
        }
        /// <summary>
        /// The DeleteDC function deletes the specified device context (DC).
        /// </summary>
        /// <param name="hdc">
        /// A handle to the device context.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.
        /// </returns>
        public static bool DeleteDC(IntPtr hdc)
        {
            return NativeMethods.DeleteDC(hdc);
        }
        /// <summary>
        /// The SelectObject function selects an object into the specified device context (DC). 
        /// The new object replaces the previous object of the same type.
        /// </summary>
        /// <param name="hDC">
        /// A handle to the DC.
        /// </param>
        /// <param name="hObject">
        /// A handle to the object to be selected. The specified object must have been created by 
        /// using one of the following functions.
        /// </param>
        /// <returns>
        /// If the selected object is not a region and the function succeeds, the return value is a handle to the 
        /// object being replaced. If the selected object is a region and the function succeeds, the return value 
        /// is one of the following values. SIMPLEREGION - Region consists of a single rectangle.,
        /// COMPLEXREGION - Region consists of more than one rectangle. NULLREGION - Region is empty.
        /// </returns>
        public static IntPtr SelectObject(IntPtr hDC, IntPtr hObject)
        {
            return NativeMethods.SelectObject(hDC, hObject);
        }
        /// <summary>
        /// The DeleteObject function deletes a logical pen, brush, font, bitmap, region, or palette, freeing all 
        /// system resources associated with the object. After the object is deleted, 
        /// the specified handle is no longer valid.
        /// </summary>
        /// <param name="hObject">
        /// A handle to a logical pen, brush, font, bitmap, region, or palette.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the specified handle is not valid or is currently 
        /// selected into a DC, the return value is zero.
        /// </returns>
        public static bool DeleteObject(IntPtr hObject)
        {
            return NativeMethods.DeleteObject(hObject);
        }
        /// <summary>
        /// The LoadLibrary function maps the specified executable module into the address space of the calling process.
        /// </summary>
        /// <param name="lpLibFileName">Pointer to a null-terminated string that names the executable module (either a .dll or .exe file). The name specified is the file name of the module and is not related to the name stored in the library module itself, as specified by the LIBRARY keyword in the module-definition (.def) file.</param>
        /// <returns>If the function succeeds, the return value is a handle to the module.<br></br><br>If the function fails, the return value is NULL. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        public static IntPtr LoadLibrary(string lpLibFileName)
        {
            return NativeMethods.LoadLibrary(lpLibFileName);
        }
        /// <summary>
        /// The FreeLibrary function decrements the reference count of the loaded dynamic-link library (DLL). When the reference count reaches zero, the module is unmapped from the address space of the calling process and the handle is no longer valid.
        /// </summary>
        /// <param name="hLibModule">Handle to the loaded DLL module. The LoadLibrary or GetModuleHandle function returns this handle.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        public static int FreeLibrary(IntPtr hLibModule)
        {
            return NativeMethods.FreeLibrary(hLibModule);
        }
        /// <summary>
        /// The GetProcAddress function retrieves the address of an exported function or variable from the specified dynamic-link library (DLL).
        /// </summary>
        /// <param name="hModule">Handle to the DLL module that contains the function or variable. The LoadLibrary or GetModuleHandle function returns this handle.</param>
        /// <param name="lpProcName">Pointer to a null-terminated string containing the function or variable name, or the function's ordinal value. If this parameter is an ordinal value, it must be in the low-order word; the high-order word must be zero.</param>
        /// <returns>If the function succeeds, the return value is the address of the exported function or variable.<br></br><br>If the function fails, the return value is NULL. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        public static IntPtr GetProcAddress(IntPtr hModule, string lpProcName)
        {
            return NativeMethods.GetProcAddress(hModule, lpProcName);
        }
        /// <summary>
        /// The SetSuspendState function suspends the system by shutting power down. Depending on the Hibernate parameter, the system either enters a suspend (sleep) state or hibernation (S4). If the ForceFlag parameter is TRUE, the system suspends operation immediately; if it is FALSE, the system requests permission from all applications and device drivers before doing so.
        /// </summary>
        /// <param name="Hibernate">Specifies the state of the system. If TRUE, the system hibernates. If FALSE, the system is suspended.</param>
        /// <param name="ForceCritical">Forced suspension. If TRUE, the function broadcasts a PBT_APMSUSPEND event to each application and driver, then immediately suspends operation. If FALSE, the function broadcasts a PBT_APMQUERYSUSPEND event to each application to request permission to suspend operation.</param>
        /// <param name="DisableWakeEvent">If TRUE, the system disables all wake events. If FALSE, any system wake events remain enabled.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        public static int SetSuspendState(int Hibernate, int ForceCritical, int DisableWakeEvent)
        {
            return NativeMethods.SetSuspendState(Hibernate, ForceCritical, DisableWakeEvent);
        }
        /// <summary>
        /// The OpenProcessToken function opens the access token associated with a process.
        /// </summary>
        /// <param name="ProcessHandle">Handle to the process whose access token is opened.</param>
        /// <param name="DesiredAccess">Specifies an access mask that specifies the requested types of access to the access token. These requested access types are compared with the token's discretionary access-control list (DACL) to determine which accesses are granted or denied.</param>
        /// <param name="TokenHandle">Pointer to a handle identifying the newly-opened access token when the function returns.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        public static int OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle)   
        {
            return NativeMethods.OpenProcessToken(ProcessHandle, DesiredAccess, ref TokenHandle);
        }
        /// <summary>
        /// The LookupPrivilegeValue function retrieves the locally unique identifier (LUID) used on a specified system to locally represent the specified privilege name.
        /// </summary>
        /// <param name="lpSystemName">Pointer to a null-terminated string specifying the name of the system on which the privilege name is looked up. If a null string is specified, the function attempts to find the privilege name on the local system.</param>
        /// <param name="lpName">Pointer to a null-terminated string that specifies the name of the privilege, as defined in the Winnt.h header file. For example, this parameter could specify the constant SE_SECURITY_NAME, or its corresponding string, "SeSecurityPrivilege".</param>
        /// <param name="lpLuid">Pointer to a variable that receives the locally unique identifier by which the privilege is known on the system, specified by the lpSystemName parameter.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        public static int LookupPrivilegeValue(string lpSystemName, string lpName, ref NativeMethods.LUID lpLuid)
        {
            return NativeMethods.LookupPrivilegeValue(lpSystemName, lpName, ref lpLuid);
        }
        /// <summary>
        /// The AdjustTokenPrivileges function enables or disables privileges in the specified access token. Enabling or disabling privileges in an access token requires TOKEN_ADJUST_PRIVILEGES access.
        /// </summary>
        /// <param name="TokenHandle">Handle to the access token that contains the privileges to be modified. The handle must have TOKEN_ADJUST_PRIVILEGES access to the token. If the PreviousState parameter is not NULL, the handle must also have TOKEN_QUERY access.</param>
        /// <param name="DisableAllPrivileges">Specifies whether the function disables all of the token's privileges. If this value is TRUE, the function disables all privileges and ignores the NewState parameter. If it is FALSE, the function modifies privileges based on the information pointed to by the NewState parameter.</param>
        /// <param name="NewState">Pointer to a TOKEN_PRIVILEGES structure that specifies an array of privileges and their attributes. If the DisableAllPrivileges parameter is FALSE, AdjustTokenPrivileges enables or disables these privileges for the token. If you set the SE_PRIVILEGE_ENABLED attribute for a privilege, the function enables that privilege; otherwise, it disables the privilege. If DisableAllPrivileges is TRUE, the function ignores this parameter.</param>
        /// <param name="BufferLength">Specifies the size, in bytes, of the buffer pointed to by the PreviousState parameter. This parameter can be zero if the PreviousState parameter is NULL.</param>
        /// <param name="PreviousState">Pointer to a buffer that the function fills with a TOKEN_PRIVILEGES structure that contains the previous state of any privileges that the function modifies. This parameter can be NULL.</param>
        /// <param name="ReturnLength">Pointer to a variable that receives the required size, in bytes, of the buffer pointed to by the PreviousState parameter. This parameter can be NULL if PreviousState is NULL.</param>
        /// <returns>If the function succeeds, the return value is nonzero. To determine whether the function adjusted all of the specified privileges, call Marshal.GetLastWin32Error.</returns>
        public static int AdjustTokenPrivileges(IntPtr TokenHandle, int DisableAllPrivileges, ref NativeMethods.TOKEN_PRIVILEGES NewState, int BufferLength, ref NativeMethods.TOKEN_PRIVILEGES PreviousState, ref int ReturnLength)
        {
            return NativeMethods.AdjustTokenPrivileges(TokenHandle, DisableAllPrivileges, ref NewState, BufferLength, ref PreviousState, ref ReturnLength);
        }
        /// <summary>
        /// The ExitWindowsEx function either logs off the current user, shuts down the system, or shuts down and restarts the system. It sends the WM_QUERYENDSESSION message to all applications to determine if they can be terminated.
        /// </summary>
        /// <param name="uFlags">Specifies the type of shutdown.</param>
        /// <param name="dwReserved">This parameter is ignored.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        public static int ExitWindowsEx(int uFlags, int dwReserved)
        {
            return NativeMethods.ExitWindowsEx(uFlags, dwReserved);
        }
        /// <summary>
        /// The FormatMessage function formats a message string. The function requires a message definition as input. The message definition can come from a buffer passed into the function. It can come from a message table resource in an already-loaded module. Or the caller can ask the function to search the system's message table resource(s) for the message definition. The function finds the message definition in a message table resource based on a message identifier and a language identifier. The function copies the formatted message text to an output buffer, processing any embedded insert sequences if requested.
        /// </summary>
        /// <param name="dwFlags">Specifies aspects of the formatting process and how to interpret the lpSource parameter. The low-order byte of dwFlags specifies how the function handles line breaks in the output buffer. The low-order byte can also specify the maximum width of a formatted output line.</param>
        /// <param name="lpSource">Specifies the location of the message definition. The type of this parameter depends upon the settings in the dwFlags parameter.</param>
        /// <param name="dwMessageId">Specifies the message identifier for the requested message. This parameter is ignored if dwFlags includes FORMAT_MESSAGE_FROM_STRING.</param>
        /// <param name="dwLanguageId">Specifies the language identifier for the requested message. This parameter is ignored if dwFlags includes FORMAT_MESSAGE_FROM_STRING.</param>
        /// <param name="lpBuffer">Pointer to a buffer for the formatted (and null-terminated) message. If dwFlags includes FORMAT_MESSAGE_ALLOCATE_BUFFER, the function allocates a buffer using the LocalAlloc function, and places the pointer to the buffer at the address specified in lpBuffer.</param>
        /// <param name="nSize">If the FORMAT_MESSAGE_ALLOCATE_BUFFER flag is not set, this parameter specifies the maximum number of TCHARs that can be stored in the output buffer. If FORMAT_MESSAGE_ALLOCATE_BUFFER is set, this parameter specifies the minimum number of TCHARs to allocate for an output buffer. For ANSI text, this is the number of bytes; for Unicode text, this is the number of characters.</param>
        /// <param name="Arguments">Pointer to an array of values that are used as insert values in the formatted message. A %1 in the format string indicates the first value in the Arguments array; a %2 indicates the second argument; and so on.</param>
        /// <returns>If the function succeeds, the return value is the number of TCHARs stored in the output buffer, excluding the terminating null character.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        public static int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, int Arguments)
        {
            return NativeMethods.FormatMessage(dwFlags, lpSource, dwMessageId, dwLanguageId, lpBuffer, nSize, Arguments);
        }





        /// <summary>
        /// Creates a timer with the specified time-out value.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window to be associated with the timer. This window must be owned by the calling
        /// thread. If a NULL value for hWnd is passed in along with an nIDEvent of an existing timer, 
        /// that timer will be replaced in the same way that an existing non-NULL hWnd timer will be.
        /// </param>
        /// <param name="nIDEvent">
        /// A nonzero timer identifier. If the hWnd parameter is NULL, and the nIDEvent does not match an 
        /// existing timer then it is ignored and a new timer ID is generated. If the hWnd parameter is not 
        /// NULL and the window specified by hWnd already has a timer with the value nIDEvent, then the 
        /// existing timer is replaced by the new timer. When SetTimer replaces a timer, the timer is reset. 
        /// Therefore, a message will be sent after the current time-out value elapses, but the previously 
        /// set time-out value is ignored. If the call is not intended to replace an existing timer, 
        /// nIDEvent should be 0 if the hWnd is NULL
        /// </param>
        /// <param name="uElapse">
        /// The time-out value, in milliseconds. 
        /// If uElapse is less than USER_TIMER_MINIMUM (0x0000000A), the timeout is set to USER_TIMER_MINIMUM. 
        /// If uElapse is greater than USER_TIMER_MAXIMUM (0x7FFFFFFF), the timeout is set to USER_TIMER_MAXIMUM.
        /// </param>
        /// <param name="lpTimerFunc">
        /// A pointer to the function to be notified when the time-out value elapses. For more information 
        /// about the function, see TimerProc. If lpTimerFunc is NULL, the system posts a WM_TIMER message 
        /// to the application queue. The hwnd member of the message's MSG structure contains the value of 
        /// the hWnd parameter.
        /// </param>
        /// <returns>
        /// If the function succeeds and the hWnd parameter is NULL, the return value is an integer identifying 
        /// the new timer. An application can pass this value to the KillTimer function to destroy the timer.
        /// If the function succeeds and the hWnd parameter is not NULL, then the return value is a nonzero 
        /// integer. An application can pass the value of the nIDEvent parameter to the KillTimer function 
        /// to destroy the timer.
        /// If the function fails to create a timer, the return value is zero. To get extended error information, 
        /// call GetLastError.
        /// </returns>
        public static UIntPtr SetTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, NativeMethods.TimerProc lpTimerFunc)
        {
            return NativeMethods.SetTimer(hWnd, nIDEvent, uElapse, lpTimerFunc);
        }
        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage function calls the window procedure 
        /// for the specified window and does not return until the window procedure has processed the message.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window whose window procedure will receive the message. If this parameter is 
        /// HWND_BROADCAST ((HWND)0xffff), the message is sent to all top-level windows in the system, 
        /// including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but 
        /// the message is not sent to child windows.  Message sending is subject to UIPI. The thread of a 
        /// process can send messages only to message queues of threads in processes of lesser or equal 
        /// integrity level.
        /// </param>
        /// <param name="Msg">
        /// The message to be sent. For lists of the system-provided messages, see System-Defined Messages.
        /// </param>
        /// <param name="wParam">
        /// Additional message-specific information.
        /// </param>
        /// <param name="lParam">
        /// Additional message-specific information.
        /// </param>
        /// <returns>
        /// The return value specifies the result of the message processing; it depends on the message sent.
        /// </returns>
        public static IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam)
        {
            return NativeMethods.SendMessage(hWnd, Msg, wParam, lParam);
        }
        /// <summary>
        /// Installs an application-defined hook procedure into a hook chain. You would install a hook 
        /// procedure to monitor the system for certain types of events. These events are associated either 
        /// with a specific thread or with all threads in the same desktop as the calling thread.
        /// </summary>
        /// <param name="idHook">
        /// The type of hook procedure to be installed. This parameter can be one of the following values.
        /// </param>
        /// <param name="lpfn">
        /// A pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier 
        /// of a thread created by a different process, the lpfn parameter must point to a hook procedure in 
        /// a DLL. Otherwise, lpfn can point to a hook procedure in the code associated with the current process.
        /// </param>
        /// <param name="hInstance">
        /// A handle to the DLL containing the hook procedure pointed to by the lpfn parameter. The hMod 
        /// parameter must be set to NULL if the dwThreadId parameter specifies a thread created by the 
        /// current process and if the hook procedure is within the code associated with the current process.
        /// </param>
        /// <param name="threadId">
        /// \The identifier of the thread with which the hook procedure is to be associated. If this 
        /// parameter is zero, the hook procedure is associated with all existing threads running in the 
        /// same desktop as the calling thread.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the handle to the hook procedure. If the function 
        /// fails, the return value is NULL. To get extended error information, call GetLastError.
        /// </returns>
        public static IntPtr SetWindowsHookEx(int idHook, NativeMethods.HookProc lpfn, IntPtr hInstance, int threadId)
        {
            return NativeMethods.SetWindowsHookEx(idHook, lpfn, hInstance, threadId);
        }
        /// <summary>
        /// Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
        /// </summary>
        /// <param name="idHook">
        /// A handle to the hook to be removed. This parameter is a hook handle obtained by a previous 
        /// call to SetWindowsHookEx.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. 
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError
        /// </returns>
        public static int UnhookWindowsHookEx(IntPtr idHook)
        {
            return NativeMethods.UnhookWindowsHookEx(idHook);
        }
        /// <summary>
        /// Passes the hook information to the next hook procedure in the current hook chain. A hook 
        /// procedure can call this function either before or after processing the hook information.
        /// </summary>
        /// <param name="idHook">
        /// This parameter is ignored.
        /// </param>
        /// <param name="nCode">
        /// The hook code passed to the current hook procedure. The next hook procedure uses this code 
        /// to determine how to process the hook information.
        /// </param>
        /// <param name="wParam">
        /// The wParam value passed to the current hook procedure. The meaning of this parameter depends on 
        /// the type of hook associated with the current hook chain.
        /// </param>
        /// <param name="lParam">
        /// The lParam value passed to the current hook procedure. The meaning of this parameter depends on 
        /// the type of hook associated with the current hook chain.
        /// </param>
        /// <returns>
        /// This value is returned by the next hook procedure in the chain. The current hook procedure must 
        /// also return this value. The meaning of the return value depends on the hook type. For more 
        /// information, see the descriptions of the individual hook procedures.
        /// </returns>
        public static IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam)
        {
            return NativeMethods.CallNextHookEx(idHook, nCode, wParam, lParam);
        }
        /// <summary>
        /// Retrieves the length, in characters, of the specified window's title bar text (if the window 
        /// has a title bar). If the specified window is a control, the function retrieves the length of 
        /// the text within the control. However, GetWindowTextLength cannot retrieve the length of the t
        /// ext of an edit control in another application.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window or control.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the length, in characters, of the text. 
        /// Under certain conditions, this value may actually be greater than the length of the text. 
        /// For more information, see the following Remarks section.
        /// If the window has no text, the return value is zero. To get extended error information, 
        /// call GetLastError.
        /// </returns>
        public static int GetWindowTextLength(IntPtr hWnd)
        {
            return NativeMethods.GetWindowTextLength(hWnd);
        }
        /// <summary>
        /// Copies the text of the specified window's title bar (if it has one) into a buffer.
        /// If the specified window is a control, the text of the control is copied. However, 
        /// GetWindowText cannot retrieve the text of a control in another application.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window or control containing the text.
        /// </param>
        /// <param name="text">
        /// The buffer that will receive the text. If the string is as long or longer than the buffer, 
        /// the string is truncated and terminated with a null character.
        /// </param>
        /// <param name="maxLength">
        /// The maximum number of characters to copy to the buffer, including the null character. 
        /// If the text exceeds this limit, it is truncated.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the length, in characters, of the copied string, 
        /// not including the terminating null character. If the window has no title bar or text, if the 
        /// title bar is empty, or if the window or control handle is invalid, the return value is zero. 
        /// To get extended error information, call GetLastError. 
        /// This function cannot retrieve the text of an edit control in another application.
        /// </returns>
        public static int GetWindowText(IntPtr hWnd, StringBuilder text, int maxLength)
        {
            return NativeMethods.GetWindowText(hWnd, text, maxLength);
        }
        /// <summary>
        /// Destroys a modal dialog box, causing the system to end any processing for the dialog box.
        /// </summary>
        /// <param name="hDlg">
        /// A handle to the dialog box to be destroyed.
        /// </param>
        /// <param name="nResult">
        /// The value to be returned to the application from the function that created the dialog box.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the function fails, the return value 
        /// is zero. To get extended error information, call GetLastError.
        /// </returns>
        public static int EndDialog(IntPtr hDlg, IntPtr nResult)
        {
            return NativeMethods.EndDialog(hDlg, nResult);
        }
        #endregion
    }
}
