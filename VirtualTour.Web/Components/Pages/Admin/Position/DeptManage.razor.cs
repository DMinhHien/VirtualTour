using BlazorBootstrap;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using VirtualTour.Model;
namespace VirtualTour.Web.Components.Pages.Admin.Position
{
    partial class DeptManage
    {
        DeptModel newDept = new DeptModel { IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
        DeptModel editingDept; // Used for editing a Dept.
        List<DeptModel> Depts;
        List<AreaModel> Areas;
        List<SectionModel> Sections;
        List<FloorModel> Floors;
        public NavigationManager NavigationManager { get; set; }
        public IApiClient ApiClient { get; set; }
        public IToastService ToastService { get; set; }
        public int SelectedAreaId { get; set; }
        public int SelectedSectionId { get; set; }
        public int SelectedFloorId { get; set; }
        Modal ModalDelete;
        Modal ModalCreate;
        Modal ModalEdit;
        MudDataGrid<DeptModel> DeptGrid;
        public int DeleteId { get; set; }
        public DeptManage(IApiClient apiClient, NavigationManager navigationManager, IToastService toastService)
        {
            ApiClient = apiClient;
            NavigationManager = navigationManager;
            ToastService = toastService;
        }
        protected override async Task OnInitializedAsync()
        {
            await LoadDept();
            await LoadArea();
            await LoadSection();
            await LoadFloor();
        }
        async Task LoadDept()
        {
            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>("api/Dept/getListUse");
            if (res != null && res.Success)
            {
                Depts = JsonConvert.DeserializeObject<List<DeptModel>>(res.Data.ToString());
            }
            else
            {
                ToastService.ShowError("Failed to load departments.");
            }
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
        async Task CreateDept()
        {
            newDept.AreaId=SelectedAreaId;
            var res = await ApiClient.PostAsync<BaseResponseModel, DeptModel>("api/Dept/create", newDept);
            if (res != null && res.Success == true)
            {
                ToastService.ShowSuccess("Department created successfully.");
                OnHideModalCreateClick();
                await LoadDept();
            }
            await DeptGrid.ReloadServerData();
        }
        async Task EditDept(DeptModel Dept)
        {
            editingDept = new DeptModel
            {
                Id = Dept.Id,
                DeptName = Dept.DeptName,
                AreaId = Dept.AreaId,
                IsActive = Dept.IsActive,
                CreatedAt = Dept.CreatedAt,
                UpdatedAt = Dept.UpdatedAt
            };
            await ModalEdit.ShowAsync();
        }
        void ToggleCreateCard()
        {
            if (SelectedAreaId == 0)
            {
                ToastService.ShowWarning("Please select an Area before creating a Department.");
                return;
            }
            ModalCreate.ShowAsync();
            newDept = new DeptModel { IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
        }
        async Task UpdateDept(DeptModel Dept)
        {
            var res = await ApiClient.PutAsync<BaseResponseModel, DeptModel>("api/Dept/update", Dept);
            if (res != null && res.Success == true)
            {
                ToastService.ShowSuccess("Department updated successfully.");
                OnHideModalEditClick();
                await LoadDept();
            }
            await DeptGrid.ReloadServerData();
        }
        async Task HandleDelete()
        {
            var res = await ApiClient.DeleteAsync<BaseResponseModel>($"api/Dept/delete/{DeleteId}");
            if (res != null && res.Success == true)
            {
                ToastService.ShowSuccess("Department deleted successfully.");
                OnHideModalDeleteClick();
                await LoadDept();
            }
            await DeptGrid.ReloadServerData();
        }
        void OnHideModalCreateClick()
        {
            newDept = new DeptModel { IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            ModalCreate.HideAsync();
        }
        void OnHideModalEditClick()
        {
            editingDept = null;
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
