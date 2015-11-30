using System;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Project858.Win32
{
    /// <summary>
    /// Win32 API metody
    /// </summary>
    public static class NativeMethods
    {
        #region - Delegate -
        /// <summary>
        /// Specifies the common dialog box hook procedure that is overridden to
        /// add specific functionality to a common dialog box. 
        /// </summary>
        /// <param name="nCode">Code</param>
        /// <param name="wParam">Additional information about the message.</param>
        /// <param name="lParam">Additional information about the message.</param>
        /// <returns>
        /// A zero value if the default dialog box procedure processes the message; a nonzero value if 
        /// the default dialog box procedure ignores the message. 
        /// </returns>
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// An application-defined callback function that processes WM_TIMER messages. 
        /// The TIMERPROC type defines a pointer to this callback function. TimerProc is a placeholder 
        /// for the application-defined function name.
        /// </summary>
        /// <param name="hWnd">A handle to the window associated with the timer.</param>
        /// <param name="uMsg">The WM_TIMER message.</param>
        /// <param name="nIDEvent">The timer's identifier.</param>
        /// <param name="dwTime">
        /// The number of milliseconds that have elapsed since the system was started. 
        /// This is the value returned by the GetTickCount function.
        /// </param>
        public delegate void TimerProc(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime);
        #endregion

        #region - Structs -
        /// <summary>
        /// The BLENDFUNCTION structure controls blending by specifying the blending functions for source and 
        /// destination bitmaps.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            #region - Properties -
            /// <summary>
            /// The source blend operation. Currently, the only source and destination blend operation that has been 
            /// defined is AC_SRC_OVER. For details, see the following Remarks section.
            /// </summary>
            public Byte BlendOp
            {
                get { return _blendOp; }
                set { _blendOp = value; }
            }
            /// <summary>
            /// Must be zero.
            /// </summary>
            public Byte BlendFlags
            {
                get { return _blendFlags; }
                set { _blendFlags = value; }
            }
            /// <summary>
            /// Specifies an alpha transparency value to be used on the entire source bitmap. The SourceConstantAlpha 
            /// value is combined with any per-pixel alpha values in the source bitmap. If you set SourceConstantAlpha 
            /// to 0, it is assumed that your image is transparent. Set the SourceConstantAlpha value to 255 (opaque) 
            /// when you only want to use per-pixel alpha values.
            /// </summary>
            public Byte SourceConstantAlpha
            {
                get { return _sourceConstantAlpha; }
                set { _sourceConstantAlpha = value; }
            }
            /// <summary>
            /// This member controls the way the source and destination bitmaps are interpreted. 
            /// AlphaFormat has the following value.
            /// </summary>
            public Byte AlphaFormat
            {
                get { return _alphaFormat; }
                set { _alphaFormat = value; }
            }
            #endregion

            #region - Variable -
            /// <summary>
            /// The source blend operation. Currently, the only source and destination blend operation that has been 
            /// defined is AC_SRC_OVER. For details, see the following Remarks section.
            /// </summary>
            private Byte _blendOp;
            /// <summary>
            /// Must be zero.
            /// </summary>
            private Byte _blendFlags;
            /// <summary>
            /// Specifies an alpha transparency value to be used on the entire source bitmap. The SourceConstantAlpha 
            /// value is combined with any per-pixel alpha values in the source bitmap. If you set SourceConstantAlpha 
            /// to 0, it is assumed that your image is transparent. Set the SourceConstantAlpha value to 255 (opaque) 
            /// when you only want to use per-pixel alpha values.
            /// </summary>
            private Byte _sourceConstantAlpha;
            /// <summary>
            /// This member controls the way the source and destination bitmaps are interpreted. 
            /// AlphaFormat has the following value.
            /// </summary>
            private Byte _alphaFormat;
            #endregion
        }
        /// <summary>
        /// A pointer to a variable that receives the LUID by which the privilege 
        /// is known on the system specified by the lpSystemName parameter.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LUID
        {
            /// <summary>
            /// The low order part of the 64 bit value.
            /// </summary>
            public int LowPart;
            /// <summary>
            /// The high order part of the 64 bit value.
            /// </summary>
            public int HighPart;
        }
        /// <summary>
        /// The LUID_AND_ATTRIBUTES structure represents a locally unique identifier (LUID) and its attributes.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LUID_AND_ATTRIBUTES
        {
            /// <summary>
            /// Specifies an LUID value.
            /// </summary>
            public LUID pLuid;
            /// <summary>
            /// Specifies attributes of the LUID. This value contains up to 32 one-bit flags. Its meaning is dependent on the definition and use of the LUID.
            /// </summary>
            public int Attributes;
        }
        /// <summary>
        /// The TOKEN_PRIVILEGES structure contains information about a set of privileges for an access token.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TOKEN_PRIVILEGES
        {
            /// <summary>
            /// Specifies the number of entries in the Privileges array.
            /// </summary>
            public int PrivilegeCount;
            /// <summary>
            /// Specifies an array of LUID_AND_ATTRIBUTES structures. Each structure contains the LUID and attributes of a privilege.
            /// </summary>
            public LUID_AND_ATTRIBUTES Privileges;
        }
        #endregion

        #region - Enums -
        /// <summary>
        /// Controls how the window is to be shown. This parameter is ignored the first time an application 
        /// calls ShowWindow, if the program that launched the application provides a STARTUPINFO structure. 
        /// Otherwise, the first time ShowWindow is called, the value should be the value obtained by the 
        /// WinMain function in its nCmdShow parameter. In subsequent calls, this parameter can be one of 
        /// the following values.
        /// </summary>
        [Flags]
        public enum APICmdShowTypes : int
        {
            /// <summary>
            /// Minimizes a window, even if the thread that owns the window is not responding. 
            /// This flag should only be used when minimizing windows from a different thread.
            /// </summary>
            SW_FORCEMINIMIZE = 11,
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            SW_HIDE = 0,	
            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            SW_MAXIMIZE = 3,	
            /// <summary>
            /// Minimizes the specified window and activates the next top-level window in the Z order.
            /// </summary>
            SW_MINIMIZE = 6,	
            /// <summary>
            /// Activates and displays the window. If the window is minimized or maximized, the system restores it 
            /// to its original size and position. An application should specify this flag when 
            /// restoring a minimized window.
            /// </summary>
            SW_RESTORE = 9,	
            /// <summary>
            /// Activates the window and displays it in its current size and position. 
            /// </summary>
            SW_SHOW = 5,	
            /// <summary>
            /// Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed 
            /// to the CreateProcess function by the program that started the application. 
            /// </summary>
            SW_SHOWDEFAULT = 10,
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>
            SW_SHOWMAXIMIZED = 3, 	
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            SW_SHOWMINIMIZED = 2, 	
            /// <summary>
            /// Displays the window as a minimized window. This value is similar to 
            /// SW_SHOWMINIMIZED, except the window is not activated.
            /// </summary>
            SW_SHOWMINNOACTIVE = 7,	
            /// <summary>
            /// Displays the window in its current size and position. This value is similar to SW_SHOW, except that the window is not activated.
            /// </summary>
            SW_SHOWNA = 8,	
            /// <summary>
            /// Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except that the window is not activated.
            /// </summary>
            SW_SHOWNOACTIVATE = 4,	
            /// <summary>
            /// Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
            /// </summary>
            SW_SHOWNORMAL = 1
        }
        /// <summary>
        /// An application cannot change the access control list of an object unless the application 
        /// has the rights to do so. These rights are controlled by a security descriptor in the access 
        /// token for the object.
        /// </summary>
        [Flags]
        public enum APITokenAccessTypes : uint
        {
            /// <summary>
            /// Required to change the default owner, primary group, or DACL of an access token.
            /// </summary>
            TOKEN_ADJUST_DEFAULT = 0x0080,
            /// <summary>
            /// Required to adjust the attributes of the groups in an access token.
            /// </summary>
            TOKEN_ADJUST_GROUPS = 0x0040,
            /// <summary>
            /// Required to enable or disable the privileges in an access token.
            /// </summary>
            TOKEN_ADJUST_PRIVILEGES = 0x0020,
            /// <summary>
            /// Required to adjust the session ID of an access token. The SE_TCB_NAME privilege is required.
            /// </summary>
            TOKEN_ADJUST_SESSIONID = 0x0100,
            /// <summary>
            /// Required to attach a primary token to a process. The SE_ASSIGNPRIMARYTOKEN_NAME privilege is also required to accomplish this task.
            /// </summary>
            TOKEN_ASSIGN_PRIMARY = 0x0001,
            /// <summary>
            /// Required to duplicate an access token.
            /// </summary>
            TOKEN_DUPLICATE = 0x0002,
            /// <summary>
            /// Required to attach an impersonation access token to a process.
            /// </summary>
            TOKEN_IMPERSONATE = 0x0004,
            /// <summary>
            /// Required to query an access token.
            /// </summary>
            TOKEN_QUERY = 0x0008,
            /// <summary>
            /// Required to query an access token.
            /// </summary>
            TOKEN_QUERY_SOURCE = 0x0010
        }
        /// <summary>
        /// Definuje mozne stavy pre stav systemoveho menu itemu
        /// </summary>
        [Flags]
        public enum APIEnableMenuItemStatus : int
        {
            /// <summary>
            /// Not valid
            /// </summary>
            NOT_VALID = -1,
            /// <summary>
            /// Specifies that the parameter gives the command ID of the existing menu item. 
            /// This is the default.
            /// </summary>
            MF_BYCOMMAND = 0x00000000,
            /// <summary>
            /// Specifies that the parameter gives the position of the existing menu item. 
            /// The first item is at position 0.
            /// </summary>
            MF_BYPOSITION = 0x00000400,
            /// <summary>
            ///  Disables the menu item so that it cannot be selected but does not dim it.
            /// </summary>
            MF_DISABLED = 0x00000002,
            /// <summary>
            /// Enables the menu item so that it can be selected and restores it from its dimmed state.
            /// </summary>
            MF_ENABLED = 0x00000000,
            /// <summary>
            /// Disables the menu item so that it cannot be selected and dims it.
            /// </summary>
            MF_GRAYED = 0x00000001
        }
        #endregion

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
        [DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hWnd, APICmdShowTypes iMsg);
        /// <summary>
        /// Determines whether the specified window is minimized (iconic).
        /// </summary>
        /// <param name="hWnd">A handle to the window to be tested.</param>
        /// <returns>
        /// If the window is iconic, the return value is nonzero.
        /// If the window is not iconic, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int IsIconic(IntPtr hWnd);
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
        [DllImport("Advapi32.dll")]
        public static extern int OpenProcessToken(IntPtr processHandle, APITokenAccessTypes desiredAccess, out IntPtr tokenHandle);
        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="handle">A valid handle to an open object.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);
        /// <summary>
        /// Allocates a new console for the calling process.
        /// </summary>
        /// <returns>true if the function succeeds; otherwise, false.</returns>
        /// <remarks>
        /// A process can be associated with only one console,
        /// so the function fails if the calling process already has a console.
        /// http://msdn.microsoft.com/en-us/library/ms681944(VS.85).aspx
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();
        /// <summary>
        /// Detaches the calling process sender its console.
        /// </summary>
        /// <returns>true if the function succeeds; otherwise, false.</returns>
        /// <remarks>
        /// If the calling process is not already attached recipients a console,
        /// the error code returned is ERROR_INVALID_PARAMETER (87).
        /// http://msdn.microsoft.com/en-us/library/ms683150(VS.85).aspx
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeConsole();
        /// <summary>
        /// Attaches the calling process recipients the console of the specified process.
        /// </summary>
        /// <example>
        /// <code>
        /// if (!AttachConsole(ATTACH_PARENT_PROCESS) AND (Marshal.GetLastWin32Error() == ERROR_ACCESS_DENIED)) {}
        /// </code>
        /// </example>
        /// <remarks>
        /// <!--http://209.85.129.132/search?q=cache:19bMz4lDI0kJ:www.pinvoke.net/default.aspx/kernel32/AttachConsole,.html+Dllinport+AttachConsole&cd=1&hl=sk&ct=clnk&gl=sk&client=opera-->
        /// </remarks>
        /// <param name="dwProcessId">[in] Identifier of the process, usually will be ATTACH_PARENT_PROCESS</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. 
        /// To get extended error information, call Marshal.GetLastWin32Error.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AttachConsole(int dwProcessId);
        /// <summary>
        /// Activates the environment application window by calling the SetForegroundWindow Win32 API.
        /// </summary>
        /// <remarks>
        /// This method puts the thread that created the specified window into the foreground and 
        /// activates the window. Keyboard input is directed recipients the window, and various visual cues 
        /// are changed for the user.
        /// </remarks>
        /// <param name="hWnd">
        /// Handle recipients the window that should be activated and brought recipients the foreground.
        /// </param>
        /// <returns>
        /// True indicates that the window was brought recipients the foreground. 
        /// False indicates that the window was not brought recipients the foreground.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
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
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, Boolean bRevert);
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
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int EnableMenuItem(IntPtr hMenu, int nIDEnableItem, int nEnable);
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
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);
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
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);
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
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
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
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        /// <summary>
        /// The DeleteDC function deletes the specified device context (DC).
        /// </summary>
        /// <param name="hdc">
        /// A handle to the device context.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.
        /// </returns>
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);
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
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
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
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);
        /// <summary>
        /// The LoadLibrary function maps the specified executable module into the address space of the calling process.
        /// </summary>
        /// <param name="lpLibFileName">Pointer to a null-terminated string that names the executable module (either a .dll or .exe file). The name specified is the file name of the module and is not related to the name stored in the library module itself, as specified by the LIBRARY keyword in the module-definition (.def) file.</param>
        /// <returns>If the function succeeds, the return value is a handle to the module.<br></br><br>If the function fails, the return value is NULL. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryA", CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary(string lpLibFileName);
        /// <summary>
        /// The FreeLibrary function decrements the reference count of the loaded dynamic-link library (DLL). When the reference count reaches zero, the module is unmapped from the address space of the calling process and the handle is no longer valid.
        /// </summary>
        /// <param name="hLibModule">Handle to the loaded DLL module. The LoadLibrary or GetModuleHandle function returns this handle.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", CharSet = CharSet.Ansi)]
        public static extern int FreeLibrary(IntPtr hLibModule);
        /// <summary>
        /// The GetProcAddress function retrieves the address of an exported function or variable from the specified dynamic-link library (DLL).
        /// </summary>
        /// <param name="hModule">Handle to the DLL module that contains the function or variable. The LoadLibrary or GetModuleHandle function returns this handle.</param>
        /// <param name="lpProcName">Pointer to a null-terminated string containing the function or variable name, or the function's ordinal value. If this parameter is an ordinal value, it must be in the low-order word; the high-order word must be zero.</param>
        /// <returns>If the function succeeds, the return value is the address of the exported function or variable.<br></br><br>If the function fails, the return value is NULL. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        /// <summary>
        /// The SetSuspendState function suspends the system by shutting power down. Depending on the Hibernate parameter, the system either enters a suspend (sleep) state or hibernation (S4). If the ForceFlag parameter is TRUE, the system suspends operation immediately; if it is FALSE, the system requests permission from all applications and device drivers before doing so.
        /// </summary>
        /// <param name="Hibernate">Specifies the state of the system. If TRUE, the system hibernates. If FALSE, the system is suspended.</param>
        /// <param name="ForceCritical">Forced suspension. If TRUE, the function broadcasts a PBT_APMSUSPEND event to each application and driver, then immediately suspends operation. If FALSE, the function broadcasts a PBT_APMQUERYSUSPEND event to each application to request permission to suspend operation.</param>
        /// <param name="DisableWakeEvent">If TRUE, the system disables all wake events. If FALSE, any system wake events remain enabled.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        [DllImport("powrprof.dll", EntryPoint = "SetSuspendState", CharSet = CharSet.Ansi)]
        public static extern int SetSuspendState(int Hibernate, int ForceCritical, int DisableWakeEvent);
        /// <summary>
        /// The OpenProcessToken function opens the access token associated with a process.
        /// </summary>
        /// <param name="ProcessHandle">Handle to the process whose access token is opened.</param>
        /// <param name="DesiredAccess">Specifies an access mask that specifies the requested types of access to the access token. These requested access types are compared with the token's discretionary access-control list (DACL) to determine which accesses are granted or denied.</param>
        /// <param name="TokenHandle">Pointer to a handle identifying the newly-opened access token when the function returns.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        [DllImport("advapi32.dll", EntryPoint = "OpenProcessToken", CharSet = CharSet.Ansi)]
        public static extern int OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);
        /// <summary>
        /// The LookupPrivilegeValue function retrieves the locally unique identifier (LUID) used on a specified system to locally represent the specified privilege name.
        /// </summary>
        /// <param name="lpSystemName">Pointer to a null-terminated string specifying the name of the system on which the privilege name is looked up. If a null string is specified, the function attempts to find the privilege name on the local system.</param>
        /// <param name="lpName">Pointer to a null-terminated string that specifies the name of the privilege, as defined in the Winnt.h header file. For example, this parameter could specify the constant SE_SECURITY_NAME, or its corresponding string, "SeSecurityPrivilege".</param>
        /// <param name="lpLuid">Pointer to a variable that receives the locally unique identifier by which the privilege is known on the system, specified by the lpSystemName parameter.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        [DllImport("advapi32.dll", EntryPoint = "LookupPrivilegeValueA", CharSet = CharSet.Ansi)]
        public static extern int LookupPrivilegeValue(string lpSystemName, string lpName, ref LUID lpLuid);
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
        [DllImport("advapi32.dll", EntryPoint = "AdjustTokenPrivileges", CharSet = CharSet.Ansi)]
        public static extern int AdjustTokenPrivileges(IntPtr TokenHandle, int DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, int BufferLength, ref TOKEN_PRIVILEGES PreviousState, ref int ReturnLength);
        /// <summary>
        /// The ExitWindowsEx function either logs off the current user, shuts down the system, or shuts down and restarts the system. It sends the WM_QUERYENDSESSION message to all applications to determine if they can be terminated.
        /// </summary>
        /// <param name="uFlags">Specifies the type of shutdown.</param>
        /// <param name="dwReserved">This parameter is ignored.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        [DllImport("user32.dll", EntryPoint = "ExitWindowsEx", CharSet = CharSet.Ansi)]
        public static extern int ExitWindowsEx(int uFlags, int dwReserved);
        /// <summary>
        /// The FormatMessage function formats a message string. The function requires a message definition 
        /// as input. The message definition can come from a buffer passed into the function. It can come 
        /// from a message table resource in an already-loaded module. Or the caller can ask the function 
        /// to search the system's message table resource(s) for the message definition. The function finds 
        /// the message definition in a message table resource based on a message identifier and a language 
        /// identifier. The function copies the formatted message text to an output buffer, processing any 
        /// embedded insert sequences if requested.
        /// </summary>
        /// <param name="dwFlags">
        /// Specifies aspects of the formatting process and how to interpret the lpSource parameter. 
        /// The low-order byte of dwFlags specifies how the function handles line breaks in the output buffer. 
        /// The low-order byte can also specify the maximum width of a formatted output line.
        /// </param>
        /// <param name="lpSource">
        /// Specifies the location of the message definition. The type of this parameter depends upon 
        /// the settings in the dwFlags parameter.
        /// </param>
        /// <param name="dwMessageId">
        /// Specifies the message identifier for the requested message. This parameter is ignored if 
        /// dwFlags includes FORMAT_MESSAGE_FROM_STRING.
        /// </param>
        /// <param name="dwLanguageId">Specifies the language identifier for the requested message. This parameter is ignored if dwFlags includes FORMAT_MESSAGE_FROM_STRING.</param>
        /// <param name="lpBuffer">Pointer to a buffer for the formatted (and null-terminated) message. If dwFlags includes FORMAT_MESSAGE_ALLOCATE_BUFFER, the function allocates a buffer using the LocalAlloc function, and places the pointer to the buffer at the address specified in lpBuffer.</param>
        /// <param name="nSize">If the FORMAT_MESSAGE_ALLOCATE_BUFFER flag is not set, this parameter specifies the maximum number of TCHARs that can be stored in the output buffer. If FORMAT_MESSAGE_ALLOCATE_BUFFER is set, this parameter specifies the minimum number of TCHARs to allocate for an output buffer. For ANSI text, this is the number of bytes; for Unicode text, this is the number of characters.</param>
        /// <param name="Arguments">Pointer to an array of values that are used as insert values in the formatted message. A %1 in the format string indicates the first value in the Arguments array; a %2 indicates the second argument; and so on.</param>
        /// <returns>If the function succeeds, the return value is the number of TCHARs stored in the output buffer, excluding the terminating null character.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        [DllImport("user32.dll", EntryPoint = "FormatMessageA", CharSet = CharSet.Ansi)]
        public static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, int Arguments);


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
        [DllImport("User32.dll")]
        public static extern UIntPtr SetTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, TimerProc lpTimerFunc);
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
        [DllImport("User32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
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
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
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
        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(IntPtr idHook);
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
        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);
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
        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);
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
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxLength);
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
        [DllImport("user32.dll")]
        public static extern int EndDialog(IntPtr hDlg, IntPtr nResult);
        #endregion
    }
}
