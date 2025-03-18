namespace Server.Data
{
    public class Faculty
    {
        public int FacultyId { get; set; }
        public string FacultyName { get; set; }

        
        public virtual ICollection<Department> Departments { get; set; }
    }
}