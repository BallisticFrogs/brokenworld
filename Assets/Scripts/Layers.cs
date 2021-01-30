public class Layers
{
    public readonly static int ISLAND = 6;
    public readonly static int FIELDS = 8;
}

public class Masks
{
    public readonly static int ISLAND = 1 << Layers.ISLAND;
    public readonly static int FIELDS = 1 << Layers.FIELDS;
}