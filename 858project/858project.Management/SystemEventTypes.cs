using System;
using System.Collections.Generic;
using System.Text;

namespace Project858.Management
{
    /// <summary>
    /// Specifies the type of restart options that an application can use.
    /// </summary>
    public enum SystemEventTypes
    {
        /// <summary>
        /// Shuts down all processes running in the security context of the process that called the ExitWindowsEx function. Then it logs the user off.
        /// </summary>
        LogOff = 0,
        /// <summary>
        /// Shuts down the system and turns off the power. The system must support the power-off feature.
        /// </summary>
        PowerOff = 8,
        /// <summary>
        /// Shuts down the system and then restarts the system.
        /// </summary>
        Reboot = 2,
        /// <summary>
        /// Shuts down the system to a point at which it is safe to turn off the power. All file buffers have been flushed to disk, and all running processes have stopped. If the system supports the power-off feature, the power is also turned off.
        /// </summary>
        ShutDown = 1,
        /// <summary>
        /// Suspends the system.
        /// </summary>
        Suspend = -1,
        /// <summary>
        /// Hibernates the system.
        /// </summary>
        Hibernate = -2,
    }
}
