using HostelDB.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Newtonsoft.Json;
using Suo.Admin.Data.EditModel;
using Suo.Admin.Data.RabbitMqService;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;
using Suo.Admin.Pages.Post.EditPost;
using Suo.Admin.Shared;

namespace Suo.Admin.Pages.Post
{
    public class PostView : ComponentBase
    {
        [Inject] protected RabbitService RabbitService { get; set; }
        [Inject] protected TelegramUserService TelegramUserService { get; set; }
        [Inject] private LogApplicationService LogService { get; set; }
        [Inject] private PostService PostService { get; set; }
        [Inject] protected IWebHostEnvironment HostingEnv { get; set; }
        [Inject] protected IDialogService DialogService { get; set; }

        protected List<PostViewModel> Model { get; set; }
        protected List<TelegramUserVieweModel> TelegramUsers { get; set; }

        public PostViewModel mCurrentItem;
        public EditPostItemViewModel mEditViewModel = new EditPostItemViewModel();

        public bool isRemove;
        public LogApplicationViewModel LogModel = new LogApplicationViewModel();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                TelegramUsers = TelegramUserService.Get();
                Model = await PostService.GetAll();
                await InvokeAsync(StateHasChanged);
            }
        }

        public async Task CreateItemAsync()
        {
            try
            {
                mCurrentItem = new PostViewModel();
                mEditViewModel.Item = mCurrentItem;
                mEditViewModel.ListSring = new List<string>();

                var options = new DialogOptions() { CloseButton = true };
                var parameters = new DialogParameters<EditPosts> { { x => x.ViewModel, mEditViewModel } };
                parameters.Add(x => x.Title, "Создание публикации");
                var dialog = DialogService.Show<EditPosts>("Simple Dialog", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    EditPostItemViewModel returnModel = new EditPostItemViewModel();
                    returnModel = (EditPostItemViewModel)result.Data;
                    Save(returnModel.Item);

                    //рассылка уведомление по всем телеграммюзерам, не закончена
                    //string message = $"{returnModel.Item.Title}\n {returnModel.Item.Text}\n подробности в разделе новости СУО";
                    //foreach (var item in TelegramUsers)
                    //{
                    //    var mesageModel = new MessageModelForTg()
                    //    {
                    //        TelegrammUserId = int.Parse(item.TelegramUserId),
                    //        Message = message
                    //    };
                    //    await RabbitService.SendMessageToTgBot(mesageModel);
                    //}
                }
                else
                {
                    //EditPostItemViewModel returnModel = new EditPostItemViewModel();
                    //returnModel = (EditPostItemViewModel)result.Data;
                    //ReloadItem(returnModel.Item);
                }


            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }

        }

        public async Task EditItemAsync(PostViewModel item)
        {
            try
            {
                mEditViewModel.Item = item;
                mEditViewModel.ListSring = new List<string>();

                var options = new DialogOptions() { CloseButton = true };
                var parameters = new DialogParameters<EditPosts> { { x => x.ViewModel, mEditViewModel } };
                parameters.Add(x => x.Title, "Изменение публикации");
                var dialog = DialogService.Show<EditPosts>("Simple Dialog", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    EditPostItemViewModel returnModel = new EditPostItemViewModel();
                    returnModel = (EditPostItemViewModel)result.Data;
                    Save(returnModel.Item);
                }
                else
                {
                    var oldItem = PostService.ReloadItem(item);
                    var index = Model.FindIndex(x => x.PostId == oldItem.PostId);
                    Model[index] = oldItem;
                    StateHasChanged();
                }


            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }

        }

        public async Task DeleteItemAsync(PostViewModel mCurrentItem)
        {
            try
            {
                var dialog = DialogService.Show<DeleteComponent>("Вы точно хотите удалить эту публикацию?");
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    if (mCurrentItem.ListImageJson != null)
                    {
                        List<string> imageList = JsonConvert.DeserializeObject<List<string>>(mCurrentItem.ListImageJson);
                        foreach (var i in imageList)
                        {
                            var str = Path.Combine(HostingEnv.WebRootPath, "Accets", i);
                            if (File.Exists(str))
                            {
                                File.Delete(str);
                            }
                            else
                            {
                                Console.WriteLine("\nFile no exists\n");
                            }
                        }
                    }
                    PostService.Delete(mCurrentItem);
                    Model.Remove(mCurrentItem);
                }
                isRemove = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

        public void Save(PostViewModel item)
        {
            try
            {
                if (item.PostId > 0)
                {
                    var newItem = PostService.Update(item);
                    var index = Model.FindIndex(x => x.PostId == newItem.PostId);
                    Model[index] = newItem;
                }
                else
                {
                    var newItem = PostService.Create(item);
                    Model.Add(newItem);
                }
                StateHasChanged();

            }
            catch (Exception ex)
            {
                if (ex is DbUpdateConcurrencyException)
                {
                    mEditViewModel.IsConcurency = true;
                    mEditViewModel.ConcurencyErrorText = "Данные не актуальны! Обновите страницу";
                }
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
                return;

            }

        }

        public void ReloadItem(PostViewModel item)
        {
            try
            {
                var reloadItem = PostService.ReloadItem(item);
                if (reloadItem == null)
                {
                    Model.Remove(item);
                }
                else
                {
                    if (mEditViewModel.IsConcurency)
                    {
                        mEditViewModel.Item = reloadItem;
                    }

                    var index = Model.FindIndex(x => x.PostId == item.PostId);
                    if (reloadItem.Item == null)
                    {
                        Model.RemoveAt(index);
                    }
                    else
                    {
                        mEditViewModel.IsConcurency = false;
                        Model[index] = reloadItem;
                    }
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                mCurrentItem = item;
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);

            }
        }
    }
}
