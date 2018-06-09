using System;

namespace ClassLibrary1
{
    public class Student
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public virtual Person Person { get; set; }
    }
}
