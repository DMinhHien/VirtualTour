using BlazorBootstrap;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using VirtualTour.Model;
namespace VirtualTour.Web.Components.Pages.Admin.Position
{
    partial class SectionManage
    {
        SectionModel newSection = new SectionModel { IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
        SectionModel editingSection; // Used for editing a Section.
        List<SectionModel> Sections;
        public NavigationManager NavigationManager { get; set; }
        public IApiClient ApiClient { get; set; }
        public IToastService ToastService { get; set; }
        Modal ModalDelete;
        Modal ModalCreate;
        Modal ModalEdit;
        MudDataGrid<SectionModel> SectionGrid;
        public int DeleteId { get; set; }
        public SectionManage(IApiClient apiClient, NavigationManager navigationManager, IToastService toastService)
        {
            ApiClient = apiClient;
            NavigationManager = navigationManager;
            ToastService = toastService;
        }
        protected override async Task OnInitializedAsync()
        {
            await LoadSection();
        }
        async Task LoadSection()
        {
            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>("api/Section/getListUse");
            if (res != null && res.Success)
            {
                Sections = JsonConvert.DeserializeObject<List<SectionModel>>(res.Data.ToString());
            }
            else
            {
                ToastService.ShowError("Failed to load sections.");
            }
        }
        async Task CreateSection()
        {
            newSection.Id = (Sections != null && Sections.Any()) ? Sections.Max(r => r.Id) + 1 : 1;
            var res = await ApiClient.PostAsync<BaseResponseModel, SectionModel>("api/Section/create", newSection);
            if (res != null && res.Success == true)
            {
                ToastService.ShowSuccess("Section created successfully.");
                OnHideModalCreateClick();
                await LoadSection();
            }
            await SectionGrid.ReloadServerData();


        }


        async Task EditSection(SectionModel Section)
        {
            ModalEdit.ShowAsync();
            editingSection = new SectionModel
            {
                Id = Section.Id,
                SectName = Section.SectName,
                IsActive = Section.IsActive,
                CreatedAt = Section.CreatedAt,
                UpdatedAt = Section.UpdatedAt
            };

        }

        async Task UpdateSection(SectionModel Section)
        {
            var res = await ApiClient.PutAsync<BaseResponseModel, SectionModel>("api/Section/update", Section);
            if (res != null && res.Success == true)
            {
                ModalEdit.HideAsync();
                editingSection = null;
                ToastService.ShowSuccess($"Update '{Section.SectName}' successfully");
                await LoadSection();
            }
            await SectionGrid.ReloadServerData();
        }

        async Task HandleDelete()
        {
            var res = await ApiClient.DeleteAsync<BaseResponseModel>($"api/Section/delete/{DeleteId}");
            if (res != null && res.Success)
            {
                ToastService.ShowSuccess("Delete Section successfully");
                OnHideModalDeleteClick();
                await LoadSection();
            }
            else
            {
                ToastService.ShowError("Failed to delete Section.");
            }
            await SectionGrid.ReloadServerData();

        }
        void ToggleCreateCard()
        {
            ModalCreate.ShowAsync();
            newSection = new SectionModel { IsActive = true };
        }
        private async Task OnShowModalDeleteClick()
        {
            await ModalDelete.ShowAsync();
        }

        private async Task OnHideModalDeleteClick()
        {
            await ModalDelete.HideAsync();
        }
        private async Task OnHideModalCreateClick()
        {
            await ModalCreate.HideAsync();
        }
        private async Task OnHideModalEditClick()
        {
            await ModalEdit.HideAsync();
        }
    }
}
