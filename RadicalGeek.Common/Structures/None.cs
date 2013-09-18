using System.Runtime.InteropServices;

namespace RadicalGeek.Common.Structures
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct None
    {
        public static None Value
        {
            get { return default(None); }
        }

        public override bool Equals(object obj)
        {
            return obj is None;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "other")]
        public static bool Equals(None other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "left"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "right")]
        public static bool operator ==(None left, None right)
        {
            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "left"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "right")]
        public static bool operator !=(None left, None right)
        {
            return false;
        }
    }
}
