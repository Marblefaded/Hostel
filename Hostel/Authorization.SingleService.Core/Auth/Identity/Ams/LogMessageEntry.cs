using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suo.Autorization.SingleService.Core.Auth.Identity
{
    [Table("LogApplication")]
    public class LogMessageEntry
    {
        [Key]
        public int LogApplicationId { get; set; }
        public string ErrorMsg { get; set; }
        public string ErrorContext { get; set; }
        public string UserName { get; set; }
        public DateTime InsertDate { get; set; }
        public bool IsDeleted { get; set; }
        /*public bool IsEnable { get; set; }*/
        public string AppVersion { get; set; }
        public string BrowserInfo { get; set; }
        public string UserData { get; set; }
        public int? ErrorLevel { get; set; }
    }
}
