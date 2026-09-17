using Dapper;
using Dapper_ORM.Services;
using System.Data;

namespace JobPortal.Web.Models
{
    public class PreRegistration
    {
        public int PKID { get; set; }
        public string FullAddress { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public string Premises { get; set; }
        public string RegName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string Institution { get; set; }
        public string RcdInsTs { get; set; }
    }

    public class DDLList
    {
        public List<string> Country { get; set; }
        public List<string> State { get; set; }
        public List<string> City { get; set; }
        public List<string> Institute { get; set; }

    }

    public interface IPreRegistration
    {
        List<PreRegistration> GetAll(string? country, string? state, string? city, string? intitute, string? dtStart, string? dtEnd);

        DDLList GetDistinctList();

       int Add(PreRegistration model);
    }

    public class PreRegistrationRepository : IPreRegistration
    {
        private readonly IDapper _dapper;
        public PreRegistrationRepository(IDapper dapper)
        {

            this._dapper = dapper;
        }
        public List<PreRegistration> GetAll(string? country, string? state, string? city, string? intitute, string?  dtStart, string? dtEnd)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SEELCT-ALL", DbType.String);
            dbparams.Add("@country", country==null? "" : country.Replace("Select Country","").Replace("-1",""), DbType.String);
            dbparams.Add("@states", state==null? "" : state.Replace("Select State","").Replace("-1", ""), DbType.String);
            dbparams.Add("@city", city==null? "" : city.Replace("Select City","").Replace("-1", ""), DbType.String);
            dbparams.Add("@Institution", intitute==null? "" : intitute.Replace("Select Institute","").Replace("-1", ""), DbType.String);
            dbparams.Add("@dtStart", dtStart==null ? DateTime.Now.AddYears(-10) : DateTime.Parse(dtStart), DbType.DateTime);
            dbparams.Add("@dtEnd", dtEnd==null ? DateTime.Now.AddYears(10) : DateTime.Parse(dtEnd), DbType.DateTime);
            var result = _dapper.GetAll<PreRegistration>($"PreRegistrationMaster", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }
        public int Add(PreRegistration model)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERT", DbType.String);
            dbparams.Add("@LocationPin", model.FullAddress, DbType.String);
            dbparams.Add("@Country", model.Country, DbType.String);
            dbparams.Add("@States", model.State, DbType.String);
            dbparams.Add("@City", model.City, DbType.String);
            dbparams.Add("@StreetAddress", model.StreetAddress, DbType.String);
            dbparams.Add("@Premises", model.Premises, DbType.String);
            dbparams.Add("@RegName", model.RegName, DbType.String);
            dbparams.Add("@EmailId", model.EmailId, DbType.String);
            dbparams.Add("@MobileNo", model.MobileNo, DbType.String);
            dbparams.Add("@Institution", model.Institution, DbType.String);
            dbparams.Add("@RcdInsTs", model.RcdInsTs, DbType.String);
            var result = _dapper.Execute($"PreRegistrationMaster", dbparams, commandType: CommandType.StoredProcedure);
            return Convert.ToInt32(result);
        }

        public DDLList GetDistinctList()
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SEELCT-DISTINCT-FOR-DROPDOWN", DbType.String);
            using (var multi = _dapper.QueryMultiple("PreRegistrationMaster", dbparams))
            {
                var countrylist = multi.Read<string>().ToList();
                var statelist = multi.Read<string>().ToList();
                var citylist = multi.Read<string>().ToList();
                var institutelist = multi.Read<string>().ToList();

                return new DDLList
                {
                    Country = countrylist,
                    State = statelist,
                    City = citylist,
                    Institute = institutelist
                };
            }

        }
    }
}
