using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Service
{
    public class Query
    {
        public static StringBuilder UpdateReferenceCount()
        {
            return new StringBuilder(@"UPDATE sst00 SET series = @series WHERE transtype = @transtype;");
        }

        public static StringBuilder UpdateBIRSeries()
        {
            return new StringBuilder(@"UPDATE ssbir SET 
	                                    Prefix = CASE WHEN series = 9999999999 THEN Prefix + 1 ELSE prefix END,
	                                    Series = CASE WHEN series = 9999999999 THEN 1 ELSE Series + 1 END
                                    WHERE transtype = @transtype;");
        }

        public static StringBuilder GetBIRSeries()
        {
            return new StringBuilder(@"SELECT prefix, series FROM ssbir WHERE TransType = @transtype LIMIT 1;");
        }

        public static StringBuilder UpdateTagging(string field, string table)
        {
            return new StringBuilder($@"UPDATE {table} SET {field} = @value WHERE reference=@reference;");
        }

        public static StringBuilder GetLatestTransactionReference()
        {
            return new StringBuilder(@"SELECT CONCAT(transtype,LPAD(series+1, 10, '0')) as reference FROM SST00 WHERE transtype = @transactionType AND module = @module LIMIT 1;");
        }
        public static StringBuilder GetLatestCreditLimitAccountNumber()
        {
            return new StringBuilder(@"SELECT LPAD(IFNULL(MAX(accountNumber *1),0) +1,10,'0') AS 'AccountNumber' FROM ACL00 LIMIT 1;");
        }

        public static StringBuilder DropPrimaryKey(string table, string field)
        {
            return new StringBuilder($@"ALTER TABLE `{table}` 
                                        CHANGE COLUMN `{field}` `{field}` INT(10) UNSIGNED NOT NULL ,
                                        DROP PRIMARY KEY;");
        }

        public static StringBuilder AlterPrimaryKey(string table, string field)
        {
            return new StringBuilder($@"ALTER TABLE `{table}` 
                                        CHANGE COLUMN `{field}` `{field}` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT ,
                                        ADD PRIMARY KEY (`{field}`);");
        }

        public static StringBuilder CountRecord(string table, string field)
        {
            return new StringBuilder($@"SELECT COUNT({field}) as count FROM {table};");
        }

        public static StringBuilder ArrangeDetailNum(string table, string field)
        {
            return new StringBuilder($@"SELECT {field} FROM {table} ORDER BY transNum ASC;");
        }

        public static StringBuilder UpdateDetailNum(string table, string field)
        {
            return new StringBuilder($@"UPDATE {table} SET {field} = @value WHERE transNum=@transNum AND detailNum=@detailNum;");
        }
      
        public static StringBuilder UpdateORDetailNum()
        {
            return new StringBuilder($@"UPDATE ftp00 a, fp100 b SET a.ordetailnum = b.detailnum WHERE a.transnum=b.transnum AND a.transtype=@transtype;");
        }

        public static StringBuilder GetEmployees()
        {
            return new StringBuilder(@"SELECT
	            e.idemployee as EmployeeID,
	            m.lastname,
	            m.firstname,
	            m.middlename,
	            e.idFile as memberid
            FROM employees e
            INNER JOIN members m ON e.idfile = m.idfile");
        }

        public static StringBuilder GetMembers()
        {
            return new StringBuilder(@"select
	            m.idfile `MemberId`,
	            CASE WHEN d.type = 'MEMBER' THEN 'Regular' 
		             ELSE 'Associate' END as `MemberType`,
	            m.lastname,
	            m.firstname,
	            m.middlename,
	            null as `HouseNo`,
	            m.street as `Barangay`,
	            m.municipality as `City`,
	            m.province as `Province`,
	            'Philippines' as `Country`,
	            m.zipcode,
	            m.telephone,
	            m.birthday,
	            m.sex as  `Gender`,
	            m.marital `MaritalStatus`,
	            m.knstat `IsKanegoSosyo`,
	            m.cardno `KanegoCardNumber`,
                CASE WHEN d.filestatus = 'TRANSFERRED' OR d.filestatus = 'WITHDRAWN' THEN 0 ELSE 1 END as Active,
                m.datemember,
	            'MIGRATION-TOOL' as `IdUser`
            from members m 
            INNER JOIN files d ON m.idfile = d.idfile
            WHERE d.type  IN ('MEMBER','AMEMBER') AND d.fileStatus NOT IN ('TRANSFERRED','WITHDRAWN');");
        }

        public static StringBuilder InsertMembers()
        {
            return new StringBuilder(@"INSERT INTO `cci00` (membernum, `memberId`,`memberType`,`lastName`,`firstName`,`middleName`,`houseNo`,`barangay`,`city`,`province`,`country`,`zipcode`,`landlineNo`,`birthdate`,`gender`,`maritalStatus`,`dateMembership`,`isKanegoSosyo`,`kanegoCardNumber`,memberstatus,`idUser`)
                SELECT @membernum, @memberid, @membertype, @lastname, @firstname, @middlename, @houseno, @barangay, @city, @provice, @country, @zipcode, @telephone, @birthdate, @gender,@maritalstatus, @datemember, @iskanego, @kanegono,@active,@iduser FROM DUAL
                WHERE NOT EXISTS(SELECT memberid FROM cci00 WHERE memberid = @memberid) LIMIT 1;");
        }

        public static StringBuilder GetLastMember()
        {
            return new StringBuilder("SELECT MAX(membernum) FROM cci00;");
        }

        public static StringBuilder UpdateCancelledReferenceCount()
        {
            return new StringBuilder(@"UPDATE sst00 SET series = @series WHERE transtype = @transtype AND module = @module;");
        }
    }
}
