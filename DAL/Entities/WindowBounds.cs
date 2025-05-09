namespace DAL.Entities
{
    public class WindowBounds
    {
        public int Id { get; set; } = 1; // 始终是1

        public double Width { get; set; }
        public double Height { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
    }
}
