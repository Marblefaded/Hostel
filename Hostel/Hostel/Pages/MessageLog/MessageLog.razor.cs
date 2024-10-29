using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;
using DateTime = System.DateTime;

namespace Suo.Admin.Pages.MessageLog
{
    public class MessageLogViewe : ComponentBase
    {

        private MongoClient _mongoClient;
        private IMongoDatabase _mongoDb;
        private IMongoCollection<TelegramMessageViewModel> _mongoCollection;

        private readonly string _passHashString = "$2a$13$eu94Xxlhvaw9RcbD/hnEE.aozz46SqobyrsyTaYu32wS2K7P7611S";
        public bool Pass = false;
        public string TryToEnter = "";

        public string dateFilter = "";    
        public string filterByText = "";
        public string filterByDirection = "";
        public LogApplicationViewModel LogModel = new();


        public List<TelegramMessageViewModel> TelegramMessageViewModels { get; set; }
        public DateTime filterByDate { get; set; }


        [Inject] private LogApplicationService LogService { get; set; }


        protected override async void OnInitialized()
        {
            try
            {
                _mongoClient = new MongoClient("mongodb://localhost:27017");
                _mongoDb = _mongoClient.GetDatabase("MesageLog");
                _mongoCollection = _mongoDb.GetCollection<TelegramMessageViewModel>("Mesages");
                FilterByDate = DateTime.Now;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }



        //Исправить?
        public DateTime FilterByDate
        {
            get => filterByDate;

            set
            {
                filterByDate = value;
                dateFilter = value.ToShortDateString();
                GrandFilter();
            }
        }
        public string FilterByText
        {
            get => filterByText;

            set
            {
                filterByText = value;
                GrandFilter();
            }
        }
        public string FilterByDirection
        {
            get => filterByDirection;

            set
            {
                filterByDirection = value;
                GrandFilter();
            }
        }
       
        protected async void GrandFilter()
        {
            try
            {
                var builder = Builders<TelegramMessageViewModel>.Filter;
                var filter = builder.Where(d => d.DateSend.Contains(dateFilter));
                TelegramMessageViewModels = await _mongoCollection.Find(filter).ToListAsync();
                if (filterByDirection != "")
                {
                    var filteredSecondList = TelegramMessageViewModels.Where(x => x.Direction.ToLower().Contains(filterByDirection.ToLower())).ToList();
                    TelegramMessageViewModels = filteredSecondList;
                }
                if (filterByText != "")
                {
                    var filteredSecondList = TelegramMessageViewModels.Where(x => x.User.ToLower().Contains(filterByText.ToLower())).ToList();
                    TelegramMessageViewModels = filteredSecondList;
                }
                StateHasChanged();

            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
        protected void CheckEnter()
        {
            if (BCrypt.Net.BCrypt.EnhancedVerify(TryToEnter, _passHashString))
            {                
                Pass = true;
                StateHasChanged();
            }
        }
    }

}
