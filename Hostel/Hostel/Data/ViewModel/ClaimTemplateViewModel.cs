using HostelDB.Model;
using System.ComponentModel.DataAnnotations;

namespace Suo.Admin.Data.ViewModel
{
    public class ClaimTemplateViewModel : ICloneable
    {
        private ClaimTemplate _item;
        public ClaimTemplate Item => _item;

        public ClaimTemplateViewModel()
        {
            _item = new ClaimTemplate();

        }

        public ClaimTemplateViewModel(ClaimTemplate item)
        {
            _item = item;
        }

        [Key]
        public int ClaimTemplateId
        {
            get => _item.ClaimTemplateId;
            set => _item.ClaimTemplateId = value;
        }
        [Required(ErrorMessage = "Обязательно укажите название шаблона заявления")]
        public string Title
        {
            get => _item.Title;
            set => _item.Title = value;
        }
        public string ClaimJson
        {
            get => _item.ClaimJson;
            set => _item.ClaimJson = value;
        }
        public int ClaimTypeId
        {
            get => _item.ClaimTypeId;
            set => _item.ClaimTypeId = value;
        }
        public bool IsActive
        {
            get => _item.IsActive;
            set => _item.IsActive = value;
        }
        public string ChangeLog
        {
            get => _item.ChangeLog;
            set => _item.ChangeLog = value;
        }
        public string TemplateModelJson
        {
            get => _item.TemplateModelJson;
            set => _item.TemplateModelJson = value;
        }
        public byte[] DataTemplate
        {
            get => _item.DataTemplate;
            set => _item.DataTemplate = value;
        }
        public bool IsDeleteEnabled { get; set; } = true;

        public object Clone()
        {
            ClaimTemplateViewModel tempObject = (ClaimTemplateViewModel)this.MemberwiseClone();
            tempObject._item = (ClaimTemplate)_item.Clone();
            return tempObject;
        }
    }
}
