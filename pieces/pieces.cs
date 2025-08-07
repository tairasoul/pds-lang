namespace tairasoul.pdsl.pieces;

public class Coordinate
{
    public int x { get; set; }
    public int y { get; set; }
}

public static class Comment
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
}

public static class JumpSubroutine
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "right" };
}


public static class Label
{
    public static int x { get; set; } = 15;
    public static int y { get; set; } = 11;
    public static Coordinate Text { get; set; } = new Coordinate { x = 15, y = 0 };
    public static string[] ArgumentSides { get; set; } = new[] { "right" };
}