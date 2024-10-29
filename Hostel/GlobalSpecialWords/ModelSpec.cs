namespace GlobalSpecialWords
{
    public enum FieldType
    {
        CheckBox = 1,
        TextBox = 2,
        DateBox = 3,
        TextArea = 4
    }
    public enum ModelSpecType
    {
        UserData = 1,
        FillableData = 2
    }
    public class ModelSpec
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ModelSpecType Type { get; set; }
        public FieldType? FieldType { get; set; }
        public string? Title { get; set; }
        public string Table { get; set; }
        public string Property { get; set; }

    }
}
