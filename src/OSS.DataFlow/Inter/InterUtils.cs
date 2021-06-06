using System.Threading.Tasks;

namespace OSS.DataFlow
{
    public static class InterUtils
    {
        public static Task<bool> TrueTask => Task.FromResult(true);
    }
}
