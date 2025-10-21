using BlazorBootstrap;
using Blazored.Toast.Services;
using VirtualTour.Model;
using Newtonsoft.Json;
using VirtualTour.Web;

namespace VirtualTour.Web.Components.Pages.Admin
{
    public partial class PermissionManage
    {
        List<PermissionModel> permissions;
        IApiClient ApiClient { get; set; }
        IToastService ToastService { get; set; }
  
        public PermissionManage(IApiClient apiClient, IToastService toastService)
        {
            ToastService = toastService;
            ApiClient = apiClient;

        }
        protected override async Task OnInitializedAsync()
        {
            await LoadPermissions();
        }

        async Task LoadPermissions()
        {
            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>("api/permission/getListUse");
            if (res != null && res.Success)
            {
                permissions = JsonConvert.DeserializeObject<List<PermissionModel>>(res.Data.ToString());
            }
            else
            {
                ToastService.ShowError("Failed to load permissions.");
            }
        }
        private async Task ToggleActive(PermissionModel permission, bool newValue)
        {
            var res = await ApiClient.PutAsync<BaseResponseModel, bool>($"api/permission/updateActiveStatus/{permission.Id}", newValue);
            if (res != null && res.Success == true)
            {
                ToastService.ShowSuccess($"Permission '{permission.PermissionName}' has been {(newValue ? "activated" : "deactivated")}.");
            }
            else
            {
                ToastService.ShowError($"Failed to update permission '{permission.PermissionName}' status.");
            }
            permission.IsActive = newValue;
        }
    }
}
