using Microsoft.AspNetCore.Components;
using MudBlazor;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.ViewModel;

namespace Ti.Pm.Web.Pages.ErrorGenerator
{
	public class ErrorGeneratorView : ComponentBase
	{
		[Inject]
		LogApplicationService applicationErrorService { get; set; }


		int y = 0;
		public void GenerateError()
		{
			try
			{
				int x = 1 / y;
			}
			catch (Exception ex)
			{
				if(ex.InnerException != null)
				{
					applicationErrorService.Create(ex.Message, ex.StackTrace, DateTime.Now, ex.InnerException.StackTrace);
				}
				else
				{
					string? message = null;
					applicationErrorService.Create(ex.Message, ex.StackTrace, DateTime.Now, message);
				}
			}
		}
	}
}
