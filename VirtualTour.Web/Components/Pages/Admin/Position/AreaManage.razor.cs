using BlazorBootstrap;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using VirtualTour.Model;
namespace VirtualTour.Web.Components.Pages.Admin.Position
{
    partial class AreaManage
    {
        AreaModel newArea = new AreaModel { IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
        AreaModel editingArea; // Used for editing a Area.
        List<AreaModel> Areas;
        List<SectionModel> Sections;
        List<FloorModel> Floors;
        public NavigationManager NavigationManager { get; set; }
        public IApiClient ApiClient { get; set; }
        public IToastService ToastService { get; set; }
        public int SelectedSectionId { get; set; }
        public int SelectedFloorId { get; set; }
        Modal ModalDelete;
        Modal ModalCreate;
        Modal ModalEdit;
        MudDataGrid<AreaModel> AreaGrid;
        public int DeleteId { get; set; }
        public AreaManage(IApiClient apiClient, NavigationManager navigationManager, IToastService toastService)
        {
            ApiClient = apiClient;
            NavigationManager = navigationManager;
            ToastService = toastService;
        }
        protected override async Task OnInitializedAsync()
        {
            await LoadArea();
            await LoadSection();
            await LoadFloor();
        }
        async Task LoadArea()
        {
            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>("api/Area/getListUse");
            if (res != null && res.Success)
            {
                Areas = JsonConvert.DeserializeObject<List<AreaModel>>(res.Data.ToString());
            }
            else
            {
                ToastService.ShowError("Failed to load areas.");
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
        async Task CreateArea()
        {
            newArea.SectId = SelectedSectionId;
            newArea.FloorId = SelectedFloorId;
            var res = await ApiClient.PostAsync<BaseResponseModel, AreaModel>("api/Area/create", newArea);
            if (res != null && res.Success == true)
            {
                ToastService.ShowSuccess("Area created successfully.");
                OnHideModalCreateClick();
                await LoadArea();
            }
            await AreaGrid.ReloadServerData();
        }
        void ToggleCreateCard()
        {
            if (SelectedSectionId == 0)
            {
                ToastService.ShowWarning("Please Select a Section before adding Areas.");
                return;
            }
            if (SelectedFloorId == 0)
            {
                ToastService.ShowWarning("Please Select a Floor before adding Areas.");
                return;
            }
            ModalCreate.ShowAsync();
            newArea = new AreaModel { IsActive = true };
        }
        async Task EditArea(AreaModel Area)
        {
            editingArea = new AreaModel
            {
                Id = Area.Id,
                AreaName = Area.AreaName,
                FloorId = Area.FloorId,
                SectId = Area.SectId,
                IsActive = Area.IsActive,
                CreatedAt = Area.CreatedAt,
                UpdatedAt = Area.UpdatedAt
            };
            await ModalEdit.ShowAsync();
        }
        async Task UpdateArea(AreaModel Area)
        {
            var res = await ApiClient.PutAsync<BaseResponseModel, AreaModel>("api/Area/update", Area);
            if (res != null && res.Success == true)
            {
                ToastService.ShowSuccess("Area updated successfully.");
                OnHideModalEditClick();
                await LoadArea();
            }
            await AreaGrid.ReloadServerData();
        }
        async Task HandleDelete()
        {
            var res = await ApiClient.DeleteAsync<BaseResponseModel>($"api/Area/delete/{DeleteId}");
            if (res != null && res.Success == true)
            {
                ToastService.ShowSuccess("Area deleted successfully.");
                OnHideModalDeleteClick();
                await LoadArea();
            }
            await AreaGrid.ReloadServerData();
        }
        void OnHideModalCreateClick()
        {
            newArea = new AreaModel { IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            ModalCreate.HideAsync();
        }
        void OnHideModalEditClick()
        {
            editingArea = null;
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
