
namespace XyzAniCheat.Runtime.Utils
{
    internal static class RandomUtils
    {
        internal static int RandomInt()
        {
            return m_Random.Next();
        }

        private static readonly System.Random m_Random = new System.Random();
    }
}