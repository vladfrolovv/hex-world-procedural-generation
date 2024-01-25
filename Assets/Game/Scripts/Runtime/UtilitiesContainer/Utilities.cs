#region

#endregion

namespace Game.Runtime.UtilitiesContainer
{
    public static class Utilities
    {

        public static int Step(this float value, float threshold)
        {
            return value < threshold ? 0 : 1;
        }

    }
}
