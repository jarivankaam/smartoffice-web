public class Environment2D
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int MaxHeight { get; set; }
    public int MaxWidth { get; set; }
    public Guid AppUserId { get; set; } // ✅ Ensure this property exists
}