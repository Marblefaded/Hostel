using HostelDB.Model;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Suo.Admin.Data.ViewModel
{
    public class PostViewModel : ICloneable
    {
        private Post _item;
        public Post Item => _item;

        public PostViewModel()
        {
            _item = new Post();

        }

        public PostViewModel(Post item)
        {
            _item = item;
        }

        [Key]
        public int PostId
        {
            get => _item.PostId;
            set => _item.PostId = value;
        }
        [Required(ErrorMessage = "Заполните все поля")]
        public string Title
        {
            get => _item.Title;
            set => _item.Title = value;
        }
        [Required(ErrorMessage = "Заполните все поля")]
        public string Text
        {
            get => _item.Text;
            set => _item.Text = value;
        }
        public string ListImageJson
        {
            get => _item.ListImageJson;
            set => _item.ListImageJson = value;
        }

        public List<string> ListImages
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(this.ListImageJson))
                    {
                        return JsonConvert.DeserializeObject<List<string>>(this.ListImageJson);

                    }
                    else
                    {
                        return new List<string>();
                    }
                }
                catch (Exception e)
                {
                    return new List<string>();
                }
            }
        }

        public DateTime CreateDate
        {
            get => _item.CreateDate;
            set => _item.CreateDate = value;
        }
        public string ChangeLog
        {
            get => _item.ChangeLog;
            set => _item.ChangeLog = value;
        }
        public object Clone()
        {
            PostViewModel tempObject = (PostViewModel)this.MemberwiseClone();
            tempObject._item = (Post)_item.Clone();
            return tempObject;
        }
    }
}
