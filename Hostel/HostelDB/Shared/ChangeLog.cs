using System;

namespace HostelDB.DbRepository.Models
{
    public class ChangeLog
    {
        public DateTime Date { get; set; }
        public string User { get; set; }
        public string Operation { get; set; }
    }
}
