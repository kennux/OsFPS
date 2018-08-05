using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsFPS
{
    /// <summary>
    /// All fire modes implemented.
    /// </summary>
    public enum FireMode : byte
    {
        /// <summary>
        /// Not firing at all
        /// </summary>
        NULL,

        /// <summary>
        /// Semi-automatic firing
        /// </summary>
        SEMI_AUTO,

        /// <summary>
        /// Full-automatic firing
        /// </summary>
        FULL_AUTO,
    }
}
