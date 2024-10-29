using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPOIPdfEngine.Models
{
    public class ReportModel
    {
        [Required(ErrorMessage = "Вы не можете оставить это поле пустым")]
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
        //public string Student { get; set; }
        [Required(ErrorMessage ="Вы не можете оставить это поле пустым")]
        public string RankOfManagement { get; set; }
        [Required(ErrorMessage = "Вы не можете оставить это поле пустым")]
        public string NameOfManager { get; set; }
        public string Facultet { get; set; }
        public string Kurs { get; set; }
        public string Group { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string Room { get; set; }
        public string Phonenumber { get; set; }
        public string Reason { get; set; }
       
        public string Dateofapplication { get; set; }    
        
        public string TimeToGo { get; set; }

        public string TimeToStart { get; set;}
        public string TimeToEnd { get; set;}
    }
}
