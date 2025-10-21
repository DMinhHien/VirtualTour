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
        public string selectedWorkshop { get; set; } = string.Empty;
        public string selectedFloor { get; set; } = "None";
        public string selectedArea { get; set; } = "None";
        public string searchFilter { get; set; } = string.Empty;
        public string selectedDept { get; set; } = "None";
        public List<string> Areas { get; set; } = new List<string>();
        public List<string> Depts{ get; set; } = new List<string>();
        public NodeManage(IToastService toastService, IApiClient apiClient)
        {
            _toastService = toastService;
            _apiClient = apiClient;
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadNodes();
            //var currentStartNode = Nodes.FirstOrDefault(n => n.IsStartNode);
            //if (currentStartNode != null)
            //{
            //    currentStartNodeId = currentStartNode.Id;
            //    previousStartNodeId = currentStartNodeId;
            //}
            //else
            //{
            //    currentStartNodeId = 0;
            //    previousStartNodeId = 0;
            //}
            var res = await _apiClient.GetFromJsonAsync<BaseResponseModel>("/api/node/getAllArea");
            if (res.Success&& res.Data != null)
            {
               Areas = JsonConvert.DeserializeObject<List<string>>(res.Data.ToString());
            }
            res = await _apiClient.GetFromJsonAsync<BaseResponseModel>("/api/node/getAllDept");
            if (res.Success && res.Data != null)
            {
                Depts = JsonConvert.DeserializeObject<List<string>>(res.Data.ToString());
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
        //private async Task HandleSelectChange(int newValue)
        //{
        //    var selectedValue = newValue;
        //    previousStartNodeId = currentStartNodeId;
        //    currentStartNodeId = Convert.ToInt32(selectedValue);
        //    var res= await _apiClient.PostAsync<BaseResponseModel, int>($"/api/node/setStartNode/{currentStartNodeId}", previousStartNodeId);
        //    if (res != null && res.Success)
        //    {
        //        _toastService.ShowSuccess("Set Start Node successfully");
        //    }
        //    else
        //    {
        //        _toastService.ShowError("Failed to set Start Node");
        //    }
        //}
    }
}
