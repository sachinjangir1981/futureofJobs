using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace JobPortal.Domain.Models
{
    public class CompareFormData
    {
        public List<FormDetail> FormDetails { get; set; } = new List<FormDetail>();
        public List<FormData> FormDatas { get; set; } = new List<FormData>();

        public List<UserListForCompare2> Users { get; set; } = new List<UserListForCompare2>();
    }

    public class FormDetail
    {
        public int Id { get; set; }
        public string FormName { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }

    }
    public class UserListForCompare
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";

        public string ProfileId { get; set; } = "";
        public string ProfileName { get; set; } = "";

        public string CompleteUserId { get; set; } = "";
        public string CompleteProfile { get; set; } = "";

         
 
    }

    public class UserListForCompare2
    {
        public Guid UserId { get; set; }   
        public string UserName { get; set; } = "";
    }

    public class RatingScore
    {
        public int QuestionId { get; set; }
        public int  Score { get; set; }  
    }

    public class FormData
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public string UserName { get; set; }
    }
}
