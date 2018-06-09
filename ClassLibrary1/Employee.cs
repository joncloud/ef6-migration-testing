using System;

namespace ClassLibrary1
{
    public class Employee
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public virtual Person Person { get; set; }
    }
}
