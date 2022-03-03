using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Customer.Model
{
    public class Members
    {
        public string MemberID { get; set; }
        public string MemberType { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string SuffixName { get; set; }
        public string HouseNo { get; set; }
        public string StreetNo { get; set; }
        public string Barangay { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string Mobile { get; set; }
        public string Telephone { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string TinNo { get; set; }
        public string SSSNo { get; set; }
        public string PhilhealthNo { get; set; }
        public string DateMembership { get; set; }
        public bool Active { get; set; }
        public bool IsKanegoSosyo { get; set; }
        public string KanegoCardNo { get; set; }
        public string IdUser { get; set; }
    }

    public class Employee
    {
        public string MemberID { get; set; }
        public string EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string SuffixName { get; set; }
    }
}
