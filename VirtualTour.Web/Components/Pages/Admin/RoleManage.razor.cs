using BlazorBootstrap;
using Blazored.Toast.Services;
using VirtualTour.Model;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using VirtualTour.Web;

namespace VirtualTour.Web.Components.Pages.Admin
{
    public partial class RoleManage
    {
        RoleModel newRole = new RoleModel { IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
        RoleModel editingRole; // Used for editing a role.
        List<PermissionModel> availablePermissions;
        List<RoleModel> roles;
      
        List<int> selectedPermissionIds = new List<int>();
   
        List<int> editingSelectedPermissionIds = new List<int>();
        List<int> editingRemovedPermissionIds= new List<int>();
        public NavigationManager NavigationManager { get; set; }
        public IApiClient ApiClient { get; set; }
        public IToastService ToastService { get; set; }
        Modal ModalDelete;
        Modal ModalCreate;
        Modal ModalEdit;
        MudDataGrid<RoleModel> RoleGrid;
        public int DeleteId { get; set; }

        public RoleManage(IApiClient apiClient, NavigationManager navigationManager, IToastService toastService)
        {
            ApiClient = apiClient;
            NavigationManager = navigationManager;
            ToastService = toastService;
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadRole();
            await LoadPermissions();
        }
        async Task LoadRole()
        {
            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>("api/role/getListUse");
            if (res != null && res.Success)
            {
                roles = JsonConvert.DeserializeObject<List<RoleModel>>(res.Data.ToString());
            }
            else
            {
                ToastService.ShowError("Failed to load permissions.");
            }
        }
        async Task LoadPermissions()
        {
            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>("api/permission/getListUse");
            if (res != null && res.Success)
            {
                availablePermissions = JsonConvert.DeserializeObject<List<PermissionModel>>(res.Data.ToString());
            }
            else
            {
                ToastService.ShowError("Failed to load permissions.");
            }
        }

        void TogglePermission(int permissionId, object checkedValue)
        {
            bool isChecked = (bool)checkedValue;
            if (isChecked)
            {
                if (!selectedPermissionIds.Contains(permissionId))
                    selectedPermissionIds.Add(permissionId);
            }
            else
            {
                selectedPermissionIds.Remove(permissionId);
            }
        }

        void ToggleEditPermission(int permissionId, object checkedValue)
        {
            bool isChecked = (bool)checkedValue;
            if (isChecked)
            {
                if (!editingSelectedPermissionIds.Contains(permissionId))
                    editingSelectedPermissionIds.Add(permissionId);
                if (editingRemovedPermissionIds.Contains(permissionId))
                    editingRemovedPermissionIds.Remove(permissionId);
            }
            else
            {
                editingSelectedPermissionIds.Remove(permissionId);
                if (!editingRemovedPermissionIds.Contains(permissionId))
                    editingRemovedPermissionIds.Add(permissionId);
            }
        }

    
        async Task CreateRole()
        {
            newRole.Id = (roles != null && roles.Any()) ? roles.Max(r => r.Id) + 1 : 1;
            var res = await ApiClient.PostAsync<BaseResponseModel, RoleModel>("api/role/create", newRole);
            if (res != null && res.Success==true)
            {
                if (selectedPermissionIds.Any())
                {
                    var resper= await ApiClient.PostAsync<BaseResponseModel, List<int>>($"api/role/assignPermission/{newRole.Id}", selectedPermissionIds);
                    if (resper == null || !resper.Success)
                    {
                        ToastService.ShowError("Failed to assign permissions to the role.");
                        return;
                    }
                }
                ToastService.ShowSuccess("Role created successfully.");
                OnHideModalCreateClick();
                await LoadRole();
            }
            await RoleGrid.ReloadServerData();


        }

 
        async Task EditRole(RoleModel role)
        {
            ModalEdit.ShowAsync();
            editingRole = new RoleModel
            {
                Id = role.Id,
                RoleName = role.RoleName,
                IsActive = role.IsActive,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt
            };
            editingSelectedPermissionIds = new List<int>();
            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>($"api/permission/getListByRoleId/{role.Id}");
            if (res != null && res.Success)
            {
                var permissions = JsonConvert.DeserializeObject<List<PermissionModel>>(res.Data.ToString());
                editingSelectedPermissionIds = permissions.Select(p => p.Id).ToList();
            }
            else
            {
                ToastService.ShowError("Failed to load permissions for the role.");
            }
            
        }

        async Task UpdateRole(RoleModel role)
        {
            var res = await ApiClient.PutAsync<BaseResponseModel, RoleModel>("api/role/update", role);
            if (res != null && res.Success==true)
            {
                if (editingSelectedPermissionIds.Any())
                {
                    await ApiClient.PostAsync<BaseResponseModel, List<int>>($"api/role/assignPermission/{role.Id}", editingSelectedPermissionIds);
                }
                if (editingRemovedPermissionIds.Any())
                {
                    await ApiClient.PostAsync<BaseResponseModel, List<int>>($"api/role/deletePermission/{role.Id}", editingRemovedPermissionIds);
                } 
                ModalEdit.HideAsync();
                editingRole = null;
                editingSelectedPermissionIds.Clear();
                editingRemovedPermissionIds.Clear();
                ToastService.ShowSuccess($"Update '{role.RoleName}' successfully");
                await LoadRole();
            }
            await RoleGrid.ReloadServerData();
        }

        async Task HandleDelete()
        {
            var res= await ApiClient.DeleteAsync<BaseResponseModel>($"api/role/delete/{DeleteId}");
            if (res != null && res.Success)
            {
                ToastService.ShowSuccess("Delete Role successfully");
                OnHideModalDeleteClick();
                await LoadRole();
            }
            else
            {
                ToastService.ShowError("Failed to delete role.");
            }
            await RoleGrid.ReloadServerData();

        }
        void ToggleCreateCard()
        {
            ModalCreate.ShowAsync();
            newRole = new RoleModel { IsActive = true };
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