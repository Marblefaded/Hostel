using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalSpecialWords
{
    public class Globals
    {
        public static List<ModelSpec> GlobalModelSpec => ModelStatusList();
        public static List<TemplateModel> TemplateModelSpec => StatusList();

        private static List<ModelSpec> ModelStatusList()
        {
            var speclist = new List<ModelSpec>
            {
                new ModelSpec {Id = 0, Name = "[facultet]",Type = ModelSpecType.UserData, Table = "User", Property = ""},
                new ModelSpec {Id = 1, Name = "[kurs]",Type = ModelSpecType.UserData, Table = "User", Property = ""},
                new ModelSpec {Id = 2, Name = "[group]",Type = ModelSpecType.UserData, Table = "User", Property = ""},
                new ModelSpec {Id = 3, Name = "[firstname]",Type = ModelSpecType.UserData, Table = "User", Property = "Name"},
                new ModelSpec {Id = 4, Name = "[middlename]", Type = ModelSpecType.UserData, Table = "User", Property = "Secondname"},
                new ModelSpec {Id = 5, Name = "[lastname]", Type = ModelSpecType.UserData, Table = "User", Property = "Surname"},
                new ModelSpec {Id = 6, Name = "[room]", Type = ModelSpecType.UserData, Table = "User", Property = "RoomId"},
                new ModelSpec {Id = 7, Name = "[phonenumber]", Type = ModelSpecType.UserData, Table = "User", Property = ""},
                new ModelSpec {Id = 8, Name = "[reason]", Type = ModelSpecType.FillableData, Title = "Укажите причину", FieldType = FieldType.TextArea},
                new ModelSpec {Id = 9, Name = "[dateofapplication]", Type = ModelSpecType.UserData, Table = "User", Property = ""},

            };
            return speclist;
        }

        private static List<TemplateModel> StatusList()
        {
            var blcklist = new List<TemplateModel>
            {
                new TemplateModel{}
            };
            return blcklist;
        }
    }
}
