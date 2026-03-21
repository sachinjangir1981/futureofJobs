using Dapper;
using Dapper_ORM.Services;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace JobPortal.Web.Models
{

	public interface IContactForms
	{
		int AddContactDetails(ContactForm contact);
        int AddSubscribeFormDetails(SubscribeForm subscribe);

        int AddSubmissionFormDetails(SubmissionForm submission);

        List<ContactForm> GetContactListPaged(int formId, int pageNo, int pageSize);

        ContactForm  GetContactByid(int id);

       int Delete(int id);
    }
	public class ContactFormsServices : IContactForms
    {
		private readonly IDapper _dapper;
		public ContactFormsServices(IDapper dapper)
		{
			this._dapper = dapper;
		}

		public int AddContactDetails(ContactForm contact)
		{
			var dbparams = new DynamicParameters();
			dbparams.Add("action", "INSERT-CONTACT", DbType.String);
			dbparams.Add("@Name", contact.Name, DbType.String);
			dbparams.Add("@Phone", contact.Phone, DbType.String);
			dbparams.Add("@EmailId", contact.Email, DbType.String);
			dbparams.Add("@Message", contact.Message, DbType.String);
			var result = _dapper.Get<int>("FormDetailMaster", dbparams, commandType: CommandType.StoredProcedure) ;
			return result;
		}


        public int AddSubscribeFormDetails(SubscribeForm subscribe)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "INSERT-SUBSCRIBE", DbType.String);
            dbparams.Add("@Name", subscribe.Name, DbType.String);
            dbparams.Add("@Phone", subscribe.Phone, DbType.String);
            dbparams.Add("@EmailId", subscribe.Email, DbType.String);
            dbparams.Add("@House", subscribe.House, DbType.String);
            dbparams.Add("@Street", subscribe.Street, DbType.String);
            dbparams.Add("@City", subscribe.City, DbType.String);
            dbparams.Add("@Pincode", subscribe.Pincode, DbType.String);
            var result = _dapper.Get<int>("FormDetailMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int AddSubmissionFormDetails(SubmissionForm submission)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "INSERT-SUBMISSION", DbType.String);
            dbparams.Add("@Name", submission.Name, DbType.String);
            dbparams.Add("@UserType", submission.UserType, DbType.String);
            dbparams.Add("@EmailId", submission.Email, DbType.String);
            dbparams.Add("@Message", submission.Message, DbType.String);
			dbparams.Add("@House", submission.filename, DbType.String);
			var result = _dapper.Get<int>("FormDetailMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public List<ContactForm> GetContactListPaged(int formId, int pageNo, int pageSize)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "Select-BY-FormId", DbType.String);
            dbparams.Add("@FormId", formId, DbType.Int32);
            dbparams.Add("@PageNo", pageNo, DbType.Int32);
            dbparams.Add("@PageSize", pageSize, DbType.Int32);
           
            var result = _dapper.GetAll<ContactForm>("FormDetailMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public ContactForm GetContactByid(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "Select-BY-Id", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Get<ContactForm>("FormDetailMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int Delete(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "Delete", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Execute("FormDetailMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }
    }


	public class ContactForm
	{
        public int PKID { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; } = string.Empty;
		[Required]
		[DataType(DataType.PhoneNumber)]
		public string Phone { get; set; } = string.Empty;
		public string Message { get; set; }

        public int? FormId { get; set; }

        public int RNUM { get; set; }

    }

    public class SubscribeForm
    {
        public int PKID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = string.Empty;
        public string House { get; set; }
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        public string Pincode { get; set; }

    }

    public class SubmissionForm
    {
        public int PKID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string UserType { get; set; } = string.Empty;
        public string Message {get; set; }

        public string filename { get; set; }
		public IFormFile Document { get; set; }

	}

}
