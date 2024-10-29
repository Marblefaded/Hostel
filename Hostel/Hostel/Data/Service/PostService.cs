using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using HostelDB.Model;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.Service
{
    public class PostService
    {
        EFRepository<Post> repoPost;
        private HostelDbContext DbContext;

        public PostService(HostelDbContext context)
        {
            repoPost = new EFRepository<Post>(context);
            DbContext = context;
        }
        public async Task<List<PostViewModel>> GetAll()
        {
            var listItems = repoPost.Get();
            var result = listItems.Select(x => Convert(x)).ToList();
            return await Task.FromResult(result);
        }

        private static PostViewModel Convert(Post r)
        {
            var item = new PostViewModel(r);
            return item;
        }

        public PostViewModel ReloadItem(PostViewModel item)
        {
            var x = repoPost.Reload(item.PostId);
            if (x == null)
            {
                return null;
            }
            return Convert(x);
        }

        public void Delete(PostViewModel item)
        {
            var x = repoPost.FindById(item.PostId);
            repoPost.Remove(x);
        }

        public PostViewModel Update(PostViewModel item)
        {
            var x = repoPost.FindByIdForReload(item.PostId);

            x.Title = item.Title;
            x.Text = item.Text;
            x.ListImageJson = item.ListImageJson;
            return Convert(repoPost.Update(x));
        }

        public PostViewModel Create(PostViewModel item)
        {
            item.CreateDate = DateTime.Now;
            var newItem = repoPost.Create(item.Item);
            return Convert(newItem);
        }
    }
}
