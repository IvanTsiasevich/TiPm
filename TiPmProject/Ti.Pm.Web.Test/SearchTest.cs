using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ti.Pm.PmDb;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.Services;
using Ti.Pm.Web.Pages.TaskType;
using Ti.Pm.PmDb.Model;
using Ti.Pm.PmDb;
using Ti.Pm.Web.Data.ViewModel;

namespace Ti.Pm.Web.Test
{
    public class SearchTest : TestContext
    {
        public SearchTest() 
        {
            Services.AddSingleton<IDialogService>(new DialogService());
            //Services.AddSingleton<CreateDialogOptionService>(new CreateDialogOptionService());
            //Services.AddSingleton<LogApplicationService>(new LogApplicationService());
            //Services.AddSingleton<TaskPmService>(new TaskPmService());
            //Services.AddSingleton<TaskTypePmService>(new TaskTypePmService());

        }
        [Fact]
        public void CounterStartsAtZero()
        {
           

            // Arrange
            var cut = RenderComponent<TaskType>();
            // Act
            cut.Find("input").Input("Bug");
            // Assert 
            cut.Find("MudTd").MarkupMatches("<MudTd DataLabel = 'Title' Style = 'width:80%'> Bug </ MudTd >");
        }
    }
}


