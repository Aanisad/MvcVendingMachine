namespace MvcVendingMachine.Models
{
    public class student
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string StudentStream { get; set; }

    }

    public class StudentAdditionalInfo
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string FavourateFruit { get; set; }
        public string Hobby { get; set; }
    }
}
