using BlazorBootstrap;
using Blazored.Toast.Services;
using VirtualTour.Model;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;

namespace VirtualTour.Web.Components.Pages.Admin.Node
{
    public partial class NodeManage
    {
        private IToastService _toastService;
        private IApiClient _apiClient;
        public List<NodeModel> Nodes;
        public Modal Modal { get; set; }
        public int DeleteID { get; set; }
        public int currentStartNodeId { get; set; } = 0;
        public int previousStartNodeId { get; set; } = 0;
        public int selectedSection { get; set; } = 0;
        public int selectedFloor { get; set; } = 0;
        public int selectedArea { get; set; } = 0;
        public string searchFilter { get; set; } = string.Empty;
        public int selectedDept { get; set; } = 0;
        public List<SectionModel> Sections { get; set; } = new List<SectionModel>();
        public List<FloorModel> Floors { get; set; } = new List<FloorModel>();
        public List<AreaModel> Areas { get; set; } = new List<AreaModel>();
        public List<DeptModel> Depts{ get; set; } = new List<DeptModel>();
        public NodeManage(IToastService toastService, IApiClient apiClient)
        {
            _toastService = toastService;
            _apiClient = apiClient;
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadNodes();
            await LoadSection();
            await LoadFloor();
            await LoadArea();
            await LoadDept();
        }
        async Task LoadSection()
        {
            var res = await _apiClient.GetFromJsonAsync<BaseResponseModel>("api/Section/getListUse");
            if (res != null && res.Success)
            {
                Sections = JsonConvert.DeserializeObject<List<SectionModel>>(res.Data.ToString());
            }
        }
        async Task LoadFloor()
        {
            var res = await _apiClient.GetFromJsonAsync<BaseResponseModel>("api/Floor/getListUse");
            if (res != null && res.Success)
            {
                Floors = JsonConvert.DeserializeObject<List<FloorModel>>(res.Data.ToString());
            }
        }
        async Task LoadArea()
        {
            var res = await _apiClient.GetFromJsonAsync<BaseResponseModel>("api/Area/getListUse");
            if (res != null && res.Success)
            {
                Areas = JsonConvert.DeserializeObject<List<AreaModel>>(res.Data.ToString());
            }
        }
        async Task LoadDept()
        {
            var res = await _apiClient.GetFromJsonAsync<BaseResponseModel>("api/Dept/getListUse");
            if (res != null && res.Success)
            {
                Depts = JsonConvert.DeserializeObject<List<DeptModel>>(res.Data.ToString());
            }
        }
        protected async Task LoadNodes()
        {
            var res = await _apiClient.GetFromJsonAsync< BaseResponseModel> ("/api/node/getListUse");
            if (res.Success)
            {
                Nodes = JsonConvert.DeserializeObject<List<NodeModel>>(res.Data.ToString());
            }
        }
        protected async Task HandleDelete()
        {
            var res = await _apiClient.DeleteAsync<BaseResponseModel>($"/api/node/delete/{DeleteID}");
            if (res != null && res.Success)
            {
                _toastService.ShowSuccess("Delete Node successfully");
                await LoadNodes();
                await OnHideModalClick();
            }
        }
        private async Task OnShowModalClick()
        {
            await Modal.ShowAsync();
        }

        private async Task OnHideModalClick()
        {
            await Modal.HideAsync();
        }
        private async Task HandleSelectChange(int newValue)
        {
            var selectedValue = newValue;
            previousStartNodeId = currentStartNodeId;
            currentStartNodeId = Convert.ToInt32(selectedValue);
            var res = await _apiClient.PostAsync<BaseResponseModel, int>($"/api/node/setStartNode/{currentStartNodeId}", previousStartNodeId);
            if (res != null && res.Success)
            {
                _toastService.ShowSuccess("Set Start Node successfully");
            }
            else
            {
                _toastService.ShowError("Failed to set Start Node");
            }
        }
    }
}
