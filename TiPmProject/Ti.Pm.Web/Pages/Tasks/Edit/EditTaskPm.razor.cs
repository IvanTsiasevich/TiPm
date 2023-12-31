﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.ViewModel;

namespace Ti.Pm.Web.Pages.Tasks
{
    public class EditTaskPmViewe : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public TaskPmVieweModel TaskPmVieweModel { get; set; }

        public List<TaskTypePmVieweModel> TaskTypePmVieweModels { get; set; }
        public List<ProjectPmVieweModel> ProjectPmVieweModels { get; set; }
        public List<StatusPmVieweModel> StatusPmVieweModels { get; set; }

        [Inject] public TaskTypePmService TaskTypePmService { get; set; }
        [Inject] public ProjectPmService ProjectPmService { get; set; }
        [Inject] public StatusPmService StatusPmService { get; set; }
        [Inject] public LogApplicationService ApplicationErrorService { get; set; }
      
        protected override async Task OnInitializedAsync()
        {
            try
            {
                TaskTypePmVieweModels = await TaskTypePmService.GetAll();
                ProjectPmVieweModels = await ProjectPmService.GetAll();
                StatusPmVieweModels = await StatusPmService.GetAll();
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);               
            }
        }
       
        public void Cancel()
        {
            MudDialog.Cancel();
        }
        public void Save()
        {
            MudDialog.Close(DialogResult.Ok(TaskPmVieweModel));
        }
    }
}