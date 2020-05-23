namespace ProjectArena.Domain.Registry.Helpers
{
    public static class TalentPositionHelper
    {
        public static (int x, int y) GetCoordinatesFromPosition(int position)
        {
            return (x: position / 1000, y: position % 1000);
        }

        public static int GetPositionFromCoordinates(int x, int y)
        {
            return (x * 1000) + y;
        }
    }
}