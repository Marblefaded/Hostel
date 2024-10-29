using HostelDB.Model;
using System.ComponentModel.DataAnnotations;

namespace Suo.Admin.Data.ViewModel
{
    public class ClaimViewModel : ICloneable
    {
        private Claim _item;
        public Claim Item => _item;

        public ClaimViewModel()
        {
            _item = new Claim();

        }

        public ClaimViewModel(Claim item)
        {
            _item = item;
        }

        [Key]
        public int ClaimId
        {
            get => _item.ClaimId;
            set => _item.ClaimId = value;
        }
        [Required]
        public int ClaimTypeId
        {
            get => _item.ClaimTypeId;
            set => _item.ClaimTypeId = value;
        }
        public int Status
        {
            get => _item.Status;
            set => _item.Status = value;
        }
        public string ClaimJson
        {
            get => _item.ClaimJson;
            set => _item.ClaimJson = value;
        }
        public string ChangeLog
        {
            get => _item.ChangeLog;
            set => _item.ChangeLog = value;
        }
        public int ClaimTemplateId
        {
            get => _item.ClaimTemplateId;
            set => _item.ClaimTemplateId = value;
        }
        public int UserId
        {
            get => _item.UserId;
            set => _item.UserId = value;
        }
        public DateTime CreateDate 
        {
            get => _item.CreateDate;
            set => _item.CreateDate = value;
        }
        public byte[] DataClaim
        {
            get => _item.DataClaim;
            set => _item.DataClaim = value;
        }

        public object Clone()
        {
            ClaimViewModel tempObject = (ClaimViewModel)this.MemberwiseClone();
            tempObject._item = (Claim)_item.Clone();
            return tempObject;
        }
    }
}
