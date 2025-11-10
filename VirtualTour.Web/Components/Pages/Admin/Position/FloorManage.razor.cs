using BlazorBootstrap;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using VirtualTour.Model;
namespace VirtualTour.Web.Components.Pages.Admin.Position
{
    partial class FloorManage
    {
        FloorModel newFloor = new FloorModel { IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
        FloorModel editingFloor; // Used for editing a Floor.
        List<FloorModel> Floors;
        public NavigationManager NavigationManager { get; set; }
        public IApiClient ApiClient { get; set; }
        public IToastService ToastService { get; set; }
        Modal ModalDelete;
        Modal ModalCreate;
        Modal ModalEdit;
        List<SectionModel> Sections;
        public int SelectedSectionId = 0;
        MudDataGrid<FloorModel> FloorGrid;
        public int DeleteId { get; set; }
        public FloorManage(IApiClient apiClient, NavigationManager navigationManager, IToastService toastService)
        {
            ApiClient = apiClient;
            NavigationManager = navigationManager;
            ToastService = toastService;
        }
        protected override async Task OnInitializedAsync()
        {
            await LoadFloor();
            await LoadSection();
        }
        async Task LoadFloor()
        {
            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>("api/Floor/getListUse");
            if (res != null && res.Success)
            {
                Floors = JsonConvert.DeserializeObject<List<FloorModel>>(res.Data.ToString());
            }
            else
            {
                ToastService.ShowError("Failed to load floors.");
            }
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
        async Task CreateFloor()
        {
            newFloor.SectId=SelectedSectionId;
            var res = await ApiClient.PostAsync<BaseResponseModel, FloorModel>("api/Floor/create", newFloor);
            if (res != null && res.Success == true)
            {
                ToastService.ShowSuccess("Floor created successfully.");
                OnHideModalCreateClick();
                await LoadFloor();
            }
            await FloorGrid.ReloadServerData();
        }
        async Task EditFloor(FloorModel Floor)
        {
            editingFloor = new FloorModel
            {
                Id = Floor.Id,
                FloorNum = Floor.FloorNum,
                IsActive = Floor.IsActive,
                CreatedAt = Floor.CreatedAt,
                UpdatedAt = Floor.UpdatedAt
            };
            await ModalEdit.ShowAsync();
        }
        void ToggleCreateCard()
        {
            if (SelectedSectionId==0)
            {
                ToastService.ShowWarning("Please Select a Section before adding Floors.");
                return;
            }
            ModalCreate.ShowAsync();
            newFloor = new FloorModel { IsActive = true };
        }
        async Task UpdateFloor(FloorModel Floor)
        {
            var res = await ApiClient.PutAsync<BaseResponseModel, FloorModel>("api/Floor/update", Floor);
            if (res != null && res.Success == true)
            {
                ToastService.ShowSuccess("Floor updated successfully.");
                editingFloor = null;
                OnHideModalEditClick();
                await LoadFloor();
            }
            await FloorGrid.ReloadServerData();
        }
        async Task HandleDelete()
        {
            var res = await ApiClient.DeleteAsync<BaseResponseModel>($"api/Floor/delete/{DeleteId}");
            if (res != null && res.Success == true)
            {
                ToastService.ShowSuccess("Floor deleted successfully.");
                OnHideModalDeleteClick();
                await LoadFloor();
            }
            await FloorGrid.ReloadServerData();
        }
        void OnHideModalCreateClick()
        {
            newFloor = new FloorModel { IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            ModalCreate.HideAsync();
        }
        void OnHideModalEditClick()
        {
            editingFloor = null;
            ModalEdit.HideAsync();
        }
        void OnHideModalDeleteClick()
        {
            DeleteId = 0;
            ModalDelete.HideAsync();
        }
        private async Task OnShowModalDeleteClick()
        {
            await ModalDelete.ShowAsync();
        }
    }
}
