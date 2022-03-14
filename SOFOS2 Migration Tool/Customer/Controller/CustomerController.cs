using SOFOS2_Migration_Tool.Customer.Model;
using SOFOS2_Migration_Tool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Customer.Controller
{
    public class CustomerController
    {
        public List<Employee> GetEmployees()
        {
            try
            {
                var result = new List<Employee>();

                using (var conn = new MySQLHelper(Global.SourceDatabase, Query.GetEmployees()))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new Employee
                            {
                                EmployeeID = dr["EmployeeId"].ToString(),
                                MemberID = dr["memberid"].ToString(),
                                LastName = dr["lastname"].ToString(),
                                FirstName = dr["firstname"].ToString(),
                                MiddleName = dr["middlename"].ToString()
                            });
                        }
                    }
                }

                return result;
            }
            catch
            {

                throw;
            }
        }

        public List<Members> GetCustomer()
        {
            try
            {
                var result = new List<Members>();

                using (var conn = new MySQLHelper(Global.SourceDatabase, Query.GetMembers()))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new Members
                            {
                                Active = Convert.ToBoolean(dr["Active"]),
                                Barangay = dr["barangay"].ToString(),
                                BirthDate = dr["birthday"].ToString(),
                                City = dr["city"].ToString(),
                                DateMembership = dr["datemember"].ToString(),
                                LastName = dr["lastname"].ToString(),
                                FirstName = dr["firstname"].ToString(),
                                MiddleName = dr["middlename"].ToString(),
                                Gender = dr["gender"].ToString(),
                                HouseNo = dr["HouseNo"].ToString(),
                                IdUser = dr["iduser"].ToString(),
                                IsKanegoSosyo = Convert.ToBoolean(dr["iskanegososyo"]),
                                KanegoCardNo = dr["KanegoCardNumber"].ToString(),
                                MaritalStatus = dr["maritalstatus"].ToString(),
                                MemberID = dr["memberid"].ToString(),
                                MemberType = dr["membertype"].ToString(),
                                Telephone = dr["telephone"].ToString(),
                                Province = dr["province"].ToString(),
                                ZipCode = dr["zipcode"].ToString(),
                            });
                        }
                    }
                }

                return result;
            }
            catch
            {

                throw;
            }
        }

        public int InsertCustomer(List<Members> data)
        {
            try
            {
                var members = new Dictionary<string, object>();
                int counter = 0,result = 0;

                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {
                    conn.BeginTransaction();

                    conn.ArgSQLCommand = Query.GetLastMember();

                    var res = conn.GetMySQLScalar();

                    if (res == DBNull.Value)
                        counter = 1;
                    else
                        counter = Convert.ToInt32(res.ToString()) + 1;

                    foreach (var member in data)
                    {
                        members = new Dictionary<string, object>();

                        members.Add("@membernum", counter);
                        members.Add("@memberid", member.MemberID);
                        members.Add("@membertype", member.MemberType);
                        members.Add("@lastname", member.LastName);
                        members.Add("@firstname", member.FirstName);
                        members.Add("@middlename", member.MiddleName);
                        members.Add("@houseno", member.HouseNo);
                        members.Add("@barangay", member.Barangay);
                        members.Add("@city", member.City);
                        members.Add("@provice", member.Province);
                        members.Add("@country", member.Country);
                        members.Add("@zipcode", member.ZipCode);
                        members.Add("@telephone", member.Telephone);
                        members.Add("@birthdate", member.BirthDate);
                        members.Add("@gender", member.Gender);
                        members.Add("@maritalstatus", member.MaritalStatus);
                        members.Add("@iskanego", member.IsKanegoSosyo);
                        members.Add("@kanegono", member.KanegoCardNo);
                        members.Add("@datemember", member.DateMembership);
                        members.Add("@active", member.Active);
                        members.Add("@iduser", member.IdUser);

                        conn.ArgSQLCommand = Query.InsertMembers();
                        conn.ArgSQLParam = members;

                        result += conn.ExecuteMySQL();
                        counter++;
                    }

                    if(result > 0)
                        conn.CommitTransaction();

                }

                return result;
            }
            catch
            {

                throw;
            }
        }

        public int InsertEmployee(List<Employee> data)
        {
            try
            {
                var members = new Dictionary<string, object>();
                var query = new StringBuilder();
                int counter = 0, result = 0;


                foreach (var employee in data)
                {
                    query.Append($@"INSERT INTO `hei00` (memberid, employeeid, employeetype, lastname, firstname, middlename, sssno)
                                  SELECT @memberid{counter}, @employeeid{counter}, 1, @lastname{counter}, @firstname{counter}, @middlename{counter}, '' as sssNo FROM DUAL
                                  WHERE NOT EXISTS(SELECT employeeid FROM hei00 WHERE employeeid = @employeeid{counter}) LIMIT 1;");

                    members.Add($"@memberid{counter}", employee.MemberID);
                    members.Add($"@employeeid{counter}", employee.EmployeeID);
                    members.Add($"@lastname{counter}", employee.LastName);
                    members.Add($"@firstname{counter}", employee.FirstName);
                    members.Add($"@middlename{counter}", employee.MiddleName);
                   

                    //parameters.Add($@"@memberid{counter},@membertype{counter},@lastname{counter},@firstname{counter},@middlename{counter},@houseno{counter},@barangay{counter},@city{counter},@provice{counter},@country{counter},@zipcode{counter},@telephone{counter},@gender{counter},@maritalstatus{counter},@iskanego{counter},@datemember{counter},@datemember{counter},@iduser{counter})");

                    counter++;
                }

                using (var conn = new MySQLHelper(Global.DestinationDatabase, query, members))
                {
                    conn.BeginTransaction();
                    result = conn.ExecuteMySQL();
                    conn.CommitTransaction();
                }

                return result;
            }
            catch
            {

                throw;
            }
        }

    }
}
