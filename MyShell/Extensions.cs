public static class Extensions
{
    public static int ToInt(this string value)
    {
        int p;

        if (!int.TryParse(value.Trim(), out p))
            return -1;

        return p;
    }
}