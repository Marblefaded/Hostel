namespace Suo.Admin.Data
{
    public class Globals
    {
        
        //сделать так чтобы на модальном окне в MVC когда писалась причина, она вставлялось в слово [reason], и выводилось в таблицу Claim
        //Вынести global в отдельный проект

        private static List<ModelSpec> ModelStatusList()
        {
            var list = new List<ModelSpec>
            {
                new ModelSpec {Id = 0, Name = "[facultet]",Type = ModelSpecType.UserData},
                new ModelSpec {Id = 1, Name = "[kurs]",Type = ModelSpecType.UserData},
                new ModelSpec {Id = 2, Name = "[group]",Type = ModelSpecType.UserData},
                new ModelSpec {Id = 3, Name = "[firstname]",Type = ModelSpecType.UserData},
                new ModelSpec {Id = 4, Name = "[middlename]", Type = ModelSpecType.UserData},
                new ModelSpec {Id = 5, Name = "[lastname]", Type = ModelSpecType.UserData},
                new ModelSpec {Id = 6, Name = "[room]", Type = ModelSpecType.UserData},
                new ModelSpec {Id = 7, Name = "[phonenumber]", Type = ModelSpecType.UserData},
                new ModelSpec {Id = 8, Name = "[reason]", Type = ModelSpecType.FillableData, Title = "Укажите причину", FieldType = FieldType.TextArea}
            };
            return list;
        }
    }
}
