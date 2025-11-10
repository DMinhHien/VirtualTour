using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Threading.Tasks;
using VirtualTour.Model;
namespace VirtualTour.Web.Components.Pages.Admin.Node
{
    partial class NodeCreate
    {
        NodeModel node = new NodeModel()
        {
            Links=new List<LinkedNodes>(),
        };
        LinkedNodes LinkedNode = new LinkedNodes();
        private IJSRuntime JS { get; set; }
        private IApiClient _apiClient;
        private IToastService _toastService;
        private IWebHostEnvironment Env { get; set; }
        public NavigationManager Navigation { get; set; }
        private string imageUrl { get; set; } = string.Empty;
        private bool IsViewerReady { get; set; } = false;
        public LinkedNodes selectedHotSpot;
        private List<NodeModel> nodes = new List<NodeModel>();
        private int nextMarkerId = 0;
        public int LinkedNodeId { get; set; } = 0;
        public List<AreaModel> Areas { get; set; } = new List<AreaModel>();
        public List<DeptModel> Depts { get; set; } = new List<DeptModel>();
        public List<SectionModel> Sections { get; set; } = new List<SectionModel>();
        public List<FloorModel> Floors { get; set; } = new List<FloorModel>();
        public class ViewerPosition
        {
            public double yaw { get; set; }
            public double pitch { get; set; }
        }
        public NodeCreate(NavigationManager navigationManager, IJSRuntime js, 
            IWebHostEnvironment env, IApiClient apiClient,IToastService toastService)
        {
            Navigation = navigationManager;
            JS = js;
            Env = env;
            _apiClient = apiClient;
            _toastService = toastService;
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);

            if (query.TryGetValue("workshop", out var workshop))
            {
                node.SectionId = Int32.Parse(workshop.FirstOrDefault());
            }
            if (query.TryGetValue("floor", out var floor))
            {
                var floorValue = Int32.Parse(floor.FirstOrDefault());
                node.FloorId = floorValue == 0 ? 0 : floorValue;
            }
            if (query.TryGetValue("area", out var area))
            {
                var areaValue = Int32.Parse(area.FirstOrDefault());
                node.AreaId = areaValue == 0 ? 0 : areaValue;
            }
            if (query.TryGetValue("dept", out var dept))
            {
                var deptValue = Int32.Parse(dept.FirstOrDefault());
                node.DeptId = deptValue == 0 ? 0 : deptValue;
            }
                await GetId();
            var res = await _apiClient.GetFromJsonAsync<BaseResponseModel>("/api/node/getListUse");
            await LoadSection();
            await LoadFloor();
            await LoadArea();
            await LoadDept();
            //if (res.Success)
            //{
            //    nodes = JsonConvert.DeserializeObject<List<NodeModel>>(res.Data.ToString());
            //}
            //res = await _apiClient.GetFromJsonAsync<BaseResponseModel>("/api/node/getAllArea");
            //if (res.Success && res.Data != null)
            //{
            //    Areas = JsonConvert.DeserializeObject<List<string>>(res.Data.ToString());
            //}
            //res = await _apiClient.GetFromJsonAsync<BaseResponseModel>("/api/node/getAllDept");
            //if (res.Success && res.Data != null)
            //{
            //    Depts = JsonConvert.DeserializeObject<List<string>>(res.Data.ToString());
            //}

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
        private async Task UploadFiles(IBrowserFile file)
        {
            var panorama = Path.Combine(Env.WebRootPath, "images", "viewer");
            if (!Directory.Exists(panorama))
                Directory.CreateDirectory(panorama);
            var ext = Path.GetExtension(file.Name);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(panorama, fileName);
            using var image = await Image.LoadAsync(file.OpenReadStream(15_000_000));
            // Do not mutate or resize the image to preserve quality.
            var encoder = new JpegEncoder { Quality = 100 }; // Use maximum quality
            await using var fs = File.Create(fullPath);
            await image.SaveAsJpegAsync(fs, encoder);
            imageUrl = $"/images/viewer/{fileName}";
            try
            {
                var dotNetRef = DotNetObjectReference.Create(this);
                var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
                var query = QueryHelpers.ParseQuery(uri.Query);
                if (query.TryGetValue("linked", out var linkedId))
                {     
                        LinkedNodeId = int.Parse(linkedId.FirstOrDefault() ?? "0");
                        await JS.InvokeVoidAsync("initViewer", imageUrl, dotNetRef,linkedId);
                        int newMarkerId = nextMarkerId;
                        nextMarkerId++;
                        var newLink = new LinkedNodes
                        {
                            MarkerId = newMarkerId,
                            Yaw = 0,
                            Pitch=0,
                            rotation = 0,
                            Tooltip = "Linked from previous Node",
                            TargetNode = 0
                        };
                        node.Links.Add(newLink);
                        selectedHotSpot = node.Links.LastOrDefault();
                        selectedHotSpot.TargetNode = LinkedNodeId;            
                }
                else
                {
                    await JS.InvokeVoidAsync("initViewer", imageUrl, dotNetRef, 0);
                }
                node.Panorama = imageUrl;
                IsViewerReady = true;
            }
            catch (Exception ex)
            {
                IsViewerReady = false;
            }
        }
        private async Task UploadThumbnailFiles(IBrowserFile file)
        {
            var thumbnails = Path.Combine(Env.WebRootPath, "images", "thumbnails");
            if (!Directory.Exists(thumbnails))
                Directory.CreateDirectory(thumbnails);
            var ext = Path.GetExtension(file.Name);
            var fileName = $"{node.Id}_{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(thumbnails, fileName);
            using var image = await Image.LoadAsync(file.OpenReadStream(15_000_000));
            image.Mutate(x => x.Resize(new ResizeOptions { Mode = SixLabors.ImageSharp.Processing.ResizeMode.Max, Size = new SixLabors.ImageSharp.Size(300, 0) }));
            var encoder = new JpegEncoder { Quality = 50 };
            await using var fs = File.Create(fullPath);
            await image.SaveAsJpegAsync(fs, encoder);
            node.Thumbnail = $"/images/thumbnails/{fileName}";
        }
        private async Task SaveNode()
        {
            if (string.IsNullOrWhiteSpace(node.Panorama))
            {
                _toastService.ShowError("Panorama is required.");
                return;
            }
            if (string.IsNullOrWhiteSpace(node.Thumbnail))
            {
                _toastService.ShowError("Thumbnail is required.");
                return;
            }
            if (string.IsNullOrWhiteSpace(node.Name))
            {
                _toastService.ShowError("Name is required.");
                return;
            }
            if (string.IsNullOrWhiteSpace(node.Caption))
            {
                _toastService.ShowError("Caption is required.");
                return;
            }
            if (node.FloorId== 0)
            {
                _toastService.ShowError("Floor is required.");
                return;
            }
            if (node.AreaId == 0)
            {
                _toastService.ShowError("Area is required.");
                return;
            }
            if (node.DeptId== 0)
            {
                _toastService.ShowError("Department is required.");
                return;
            }
            var res = await _apiClient.PostAsync<BaseResponseModel,NodeModel> ("/api/node/create", node);
            if (res.Success)
            {
                _toastService.ShowSuccess("Node created successfully!");
                Navigation.NavigateTo("/admin/nodes", true);
            }
            else
            {
                _toastService.ShowError("Failed to create node: " + res.ErrorMessage);
            }
        }
        private async Task SetInitialView() {             
            if (IsViewerReady)
            {
                var pos =await JS.InvokeAsync<ViewerPosition>("setInitialView");
                node.DefaultYaw = pos.yaw * 180 / Math.PI;
                node.DefaultPitch = pos.pitch * 180 / Math.PI;
                _toastService.ShowSuccess($"Initial view set: Yaw: {Math.Round(node.DefaultYaw,2)}°, Pitch: {Math.Round(node.DefaultPitch,2)}°");
            }
            else
            {
                _toastService.ShowError("Viewer is not ready. Please upload a panorama image first.");
            }
        }
        private async Task AddHotSpot()
        {
            if (IsViewerReady)
            {
                int newMarkerId = nextMarkerId;
            nextMarkerId++;
            var newLink = new LinkedNodes
            {
                MarkerId = newMarkerId,
                Yaw = 0,    
                rotation = 0,
                Tooltip = "New Hotspot",
                TargetNode = 0   
            };
            node.Links.Add(newLink);
            string markerId = $"{newMarkerId}";
       
            await JS.InvokeVoidAsync("addHotSpot", markerId);
            }
            else
            {
                _toastService.ShowError("Viewer is not ready. Please upload a panorama image first.");
            }
        }
        private async Task DeleteHotSpot()
        {
            await JS.InvokeVoidAsync("removeHotSpot", selectedHotSpot.MarkerId);
            if (selectedHotSpot != null)
            {
                node.Links.Remove(selectedHotSpot);
                _toastService.ShowSuccess($"Hotspot {selectedHotSpot.MarkerId} deleted.");
                selectedHotSpot = null;
                StateHasChanged();
            }
            else
            {
                _toastService.ShowError("No hotspot selected to delete.");
            }
        }
        private async Task RotateHotSpot()
        {
            if (selectedHotSpot.MarkerId >= 0)
            {
                var hotspot = node.Links.FirstOrDefault(x => x.MarkerId == selectedHotSpot.MarkerId);
                hotspot.rotation = hotspot.rotation + 45; 
                await JS.InvokeVoidAsync("rotateHotSpot", selectedHotSpot.MarkerId, hotspot.rotation);
            }
            else
            {
                _toastService.ShowError("No hotspot selected or invalid selection.");
            }

        }
        private async Task GetId()
        {
            var res = await _apiClient.GetFromJsonAsync<BaseResponseModel>("/api/node/getMaxId");
            if (res.Success)
            {
                var maxId = JsonConvert.DeserializeObject<int>(res.Data.ToString());
                node.Id = maxId + 1;
            }
            else
            {
                _toastService.ShowError("Failed to get max ID: " + res.ErrorMessage);
            }
        }
        [JSInvokable]
        public Task UpdateHotSpotPosition(int markerId, double yaw, double pitch)
        {
            // Find the hotspot in node.Links and update its coordinates
            var hotspot = node.Links.FirstOrDefault(x => x.MarkerId == markerId);
            if (hotspot != null)
            {
                hotspot.Yaw = yaw * 180 / Math.PI;
                hotspot.Pitch = pitch * 180 / Math.PI;
            }
            return Task.CompletedTask;
        }
        [JSInvokable]
        public Task HotSpotSelected(int markerId)
        {
            selectedHotSpot = node.Links.FirstOrDefault(x => x.MarkerId == markerId);
            StateHasChanged();
            _toastService.ShowInfo($"Hotspot {markerId} selected. Yaw: {selectedHotSpot.Yaw:F2}, Pitch: {selectedHotSpot.Pitch:F2}");
            return Task.CompletedTask;
        }
        public Task SaveHotSpot()
        {
            if (selectedHotSpot != null)
            {
                if (selectedHotSpot.TargetNode ==0)
                {
                    _toastService.ShowError("Please select a node to link.");
                    return Task.CompletedTask;
                }
                _toastService.ShowSuccess($"Hotspot {selectedHotSpot.MarkerId} saved. Yaw: {selectedHotSpot.Yaw:F2}, Pitch: {selectedHotSpot.Pitch:F2}");
                selectedHotSpot = null;
                StateHasChanged();
            }
            else
            {
                _toastService.ShowError("No hotspot selected to save.");
            }
            return Task.CompletedTask;
        }
    }
}
