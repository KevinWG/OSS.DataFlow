using System.Threading.Tasks;

namespace OSS.DataFlow
{
    internal static class InterUtils
    {
        public static Task<bool> TrueTask => Task.FromResult(true);

        public static Task<bool> FalseTask => Task.FromResult(false);
    }
}
