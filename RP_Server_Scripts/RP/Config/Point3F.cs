using System.Diagnostics;
using Newtonsoft.Json;

namespace GUC.Scripts.RP.Config
{
    /// <summary>
    /// Simple 3D point struct.
    /// <remarks>Serializable with Newtonsoft.Json</remarks>
    /// </summary>
    [DebuggerDisplay("{X}|{Y}|{Z}")]
    internal struct Point3F
    {
        [JsonConstructor]
        public Point3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; }
        public float Y { get; }
        public float Z { get; }
    }
}
