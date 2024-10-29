using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System.Text.Json;
using Suo.Admin.Data.EditModel;
using Suo.Admin.Data.ViewModel;

namespace Suo.Autorization.Pages.Post
{
    public class EditPostsView : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Inject]
        protected IJSRuntime Js { get; set; }
        [Inject]
        protected IWebHostEnvironment HostingEnv { get; set; }
        [Parameter]
        public EditPostItemViewModel ViewModel { get; set; }

        [Parameter]
        public EventCallback<PostViewModel> Reload { get; set; }

        [Parameter]
        public string Title { get; set; }

        private List<IBrowserFile> loadedFiles = new();
        private long maxFileSize = 1024 * 1024 * 15;
        private int maxAllowedFiles = 3;
        private bool isLoading;
        private List<string> newFiles = new List<string>();
        

        public async Task SaveFiles(InputFileChangeEventArgs e)
        {
            ViewModel.IsEdit = true;
            isLoading = true;
            loadedFiles.Clear();
            var pathFolder = Path.Combine(HostingEnv.WebRootPath, "Accets");
            if (Directory.Exists(pathFolder))
            {
                Console.WriteLine("\nПапка 'Assets' уже существует или была успешно создана.\n");
            }
            else
            {
                Directory.CreateDirectory(pathFolder);
            }

            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {

                loadedFiles.Add(file);

                var trustedFileNameForFileStorage = Guid.NewGuid().ToString();
                var fileExtension = Path.GetExtension(file.Name);
                var fileName = trustedFileNameForFileStorage + fileExtension;
                var path = Path.Combine(pathFolder, fileName);

                try
                {
                    using (var fs = new FileStream(path, FileMode.Create))
                    {
                        await file.OpenReadStream(maxFileSize).CopyToAsync(fs);
                    }

                    ViewModel.ListSring.Add(fileName);
                    newFiles.Add(path);
                    Console.WriteLine("File saved successfully: " + path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving file: " + ex.Message);

                }
            }
            var str = JsonSerializer.Serialize(ViewModel.ListSring);
            ViewModel.Item.ListImageJson = str;
            isLoading = false;
        }

        public void LoadFiles()
        {
            Js.InvokeVoidAsync("clickInputFile");
        }

        public void CloseAndDelete()
        {
            //if(ViewModel.IsEdit)
            //{
                foreach (var i in newFiles)
                {
                    if (File.Exists(i))
                    {
                        DeleteSingleFile(i);
                    }                   
                }
           
            //}           
        }

        public void DeleteSingleFile(string element)
        {
            File.Delete(element);
            ViewModel.ListSring.Remove(element);
            ViewModel.Item.ListImageJson = JsonSerializer.Serialize(ViewModel.ListSring);
        }
        public void Cancel()
        {
            MudDialog.Cancel();
        }
        public void Save()
        {
            MudDialog.Close(DialogResult.Ok(ViewModel));
        }
    }
}
