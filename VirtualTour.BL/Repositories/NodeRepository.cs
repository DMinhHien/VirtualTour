using Dapper;
using VirtualTour.DataAccess;
using VirtualTour.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;

namespace VirtualTour.BL.Repositories
{
    public interface INodeRepository
    {
        Task<IEnumerable<NodeModel>> GetAllNodesAsync();
        Task<IEnumerable<LinkedNodes>> GetAllLinks(int nodeId);
        Task CreateNode(NodeModel nodeModel);
        Task CreateNodeLink(LinkedNodes linkedNode, int startNodeId);
        Task DeleteNode(int nodeId);
        Task DeleteNodeLink(int sourceNodeId, int targetNodeId);
        Task UpdateNode(NodeModel nodeModel);
        Task UpdateNodeLink(LinkedNodes linkedNode, int startNodeId);
        Task<int> GetMaxId();
        Task<NodeModel> GetById(int id);
        Task SetStartNode(int currentStartNode, int previousStartNode);
        Task<int> GetStartId();
        Task<IEnumerable<string>> GetAllArea();
        Task<IEnumerable<string>> GetAllDept();
    }
    public class NodeRepository : INodeRepository
    {
        private readonly IDbContext _dbContext;
        public NodeRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<NodeModel>> GetAllNodesAsync()
        {
            var storedProcedure = "sp_nodes_get_all";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                    return await connection.QueryAsync<NodeModel>(storedProcedure, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching nodes: {ex.Message}");
            }
        }
        public async Task<IEnumerable<LinkedNodes>> GetAllLinks(int nodeId)
        {
            var storedProcedure = "sp_nodes_get_links";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", nodeId, DbType.Int32);
                    return await connection.QueryAsync<LinkedNodes>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching links for node {nodeId}: {ex.Message}");
            }
        }
        public async Task CreateNode(NodeModel nodeModel)
        {
            var storedProcedure = "sp_nodes_create";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", nodeModel.Id);
                    parameters.Add("@Panorama", nodeModel.Panorama);
                    parameters.Add("@Thumbnail", nodeModel.Thumbnail);
                    parameters.Add("@Name", nodeModel.Name);
                    parameters.Add("@Caption", nodeModel.Caption);
                    parameters.Add("@DefaultYaw", nodeModel.DefaultYaw);
                    parameters.Add("@DefaultPitch", nodeModel.DefaultPitch);
                    parameters.Add("@WorkShop", nodeModel.WorkShop);
                    parameters.Add("@Floor", nodeModel.Floor);
                    parameters.Add("@AreaName", nodeModel.AreaName);
                    parameters.Add("@DeptName", nodeModel.DeptName);
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                    foreach (var link in nodeModel.Links)
                    {
                        var linkParameters = new DynamicParameters();
                        linkParameters.Add("@SourceNode", nodeModel.Id);
                        linkParameters.Add("@TargetNode", link.TargetNode);
                        linkParameters.Add("@Yaw", link.Yaw);
                        linkParameters.Add("@Pitch", link.Pitch);
                        linkParameters.Add("@Rotation", link.rotation);
                        linkParameters.Add("@Tooltip", link.Tooltip);
                        linkParameters.Add("@MarkerId", link.MarkerId);
                        await connection.ExecuteAsync("sp_nodes_create_link", linkParameters, commandType: CommandType.StoredProcedure);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating node: {ex.Message}");
            }
        }
        public async Task<int> GetMaxId()
        {
            var sql = "SELECT MAX(id) AS max_id FROM Nodes;";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var maxId = await connection.QuerySingleAsync<int?>(sql, commandType: CommandType.Text);
                    return maxId ?? 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching max node ID: {ex.Message}");
            }
        }
        public async Task<int> GetStartId()
        {
            var sql = "SELECT id FROM Nodes Where IsStartNode=1;";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    return await connection.QuerySingleAsync<int>(sql, commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching start node ID: {ex.Message}");
            }
        }
        public async Task CreateNodeLink(LinkedNodes linkedNode, int startNodeId)
        {
            var storedProcedure = "sp_nodes_create_link";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@SourceNode", startNodeId);
                    parameters.Add("@TargetNode", linkedNode.TargetNode);
                    parameters.Add("@Yaw", linkedNode.Yaw);
                    parameters.Add("@Pitch", linkedNode.Pitch);
                    parameters.Add("@Rotation", linkedNode.rotation);
                    parameters.Add("@Tooltip", linkedNode.Tooltip);
                    parameters.Add("@MarkerId", linkedNode.MarkerId);
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating node link: {ex.Message}");
            }
        }
        public async Task<NodeModel> GetById(int id)
        {
            var storedProcedure = "sp_nodes_get_by_id";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", id);
                    return await connection.QuerySingleOrDefaultAsync<NodeModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching node by ID {id}: {ex.Message}");
            }
        }
        public async Task UpdateNode(NodeModel nodeModel)
        {
            var storedProcedure = "sp_nodes_update";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", nodeModel.Id);
                    parameters.Add("@Thumbnail", nodeModel.Thumbnail);
                    parameters.Add("@Name", nodeModel.Name);
                    parameters.Add("@Caption", nodeModel.Caption);
                    parameters.Add("@DefaultYaw", nodeModel.DefaultYaw);
                    parameters.Add("@DefaultPitch", nodeModel.DefaultPitch);
                    parameters.Add("@WorkShop", nodeModel.WorkShop);
                    parameters.Add("@Floor", nodeModel.Floor);
                    parameters.Add("@AreaName", nodeModel.AreaName);
                    parameters.Add("@DeptName", nodeModel.DeptName);
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                    var existingLinks = (await connection.QueryAsync<LinkedNodes>(
                    "sp_nodes_get_links",
                    new { Id = nodeModel.Id },
                    commandType: CommandType.StoredProcedure)).ToList();
                    foreach (var link in nodeModel.Links)
                    {
                        var linkParameters = new DynamicParameters();
                        var exists = existingLinks.Any(x => x.MarkerId == link.MarkerId);

                        linkParameters.Add("@SourceNode", nodeModel.Id);
                        linkParameters.Add("@TargetNode", link.TargetNode);
                        linkParameters.Add("@Yaw", link.Yaw);
                        linkParameters.Add("@Pitch", link.Pitch);
                        linkParameters.Add("@Rotation", link.rotation);
                        linkParameters.Add("@Tooltip", link.Tooltip);
                        linkParameters.Add("@MarkerId", link.MarkerId);
                        if (exists)
                        {
                            // Update existing link
                            await connection.ExecuteAsync("sp_nodes_update_link", linkParameters, commandType: CommandType.StoredProcedure);
                        }
                        else
                        {
                            await connection.ExecuteAsync("sp_nodes_create_link", linkParameters, commandType: CommandType.StoredProcedure);
                        }
                    }
                    foreach (var oldLink in existingLinks)
                    {
                        var existsInUpdated = nodeModel.Links.Any(x => x.MarkerId == oldLink.MarkerId);
                        if (!existsInUpdated)
                        {
                            var deleteParameters = new DynamicParameters();
                            deleteParameters.Add("@SourceNode", nodeModel.Id);
                            deleteParameters.Add("@TargetNode", oldLink.TargetNode);
                            await connection.ExecuteAsync("sp_nodes_delete_link", deleteParameters, commandType: CommandType.StoredProcedure);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating node: {ex.Message}");
            }
        }
        public async Task UpdateNodeLink(LinkedNodes linkedNode, int startNodeId)
        {
            var storedProcedure = "sp_nodes_update_link";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@SourceNode", startNodeId);
                    parameters.Add("@TargetNode", linkedNode.TargetNode);
                    parameters.Add("@Yaw", linkedNode.Yaw);
                    parameters.Add("@Pitch", linkedNode.Pitch);
                    parameters.Add("@Rotation", linkedNode.rotation);
                    parameters.Add("@Tooltip", linkedNode.Tooltip);
                    parameters.Add("@MarkerId", linkedNode.MarkerId);
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating node link: {ex.Message}");
            }
        }
        public async Task DeleteNode(int nodeId)
        {
            var storedProcedure = "sp_nodes_delete";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", nodeId);
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting node {nodeId}: {ex.Message}");
            }
        }
        public async Task DeleteNodeLink(int sourceNodeId, int targetNodeId)
        {
            var storedProcedure = "sp_nodes_delete_link";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@SourceNode", sourceNodeId);
                    parameters.Add("@TargetNode", targetNodeId);
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting link from node {sourceNodeId} to {targetNodeId}: {ex.Message}");
            }
        }
        public async Task SetStartNode(int currentStartNode, int previousStartNode)
        {
            var storedProcedure = "sp_nodes_set_start";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", currentStartNode);
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                    if (previousStartNode != 0)
                    {
                        storedProcedure = "sp_nodes_remove_start";
                        var previousParameters = new DynamicParameters();
                        previousParameters.Add("@Id", previousStartNode);
                        await connection.ExecuteAsync(storedProcedure, previousParameters, commandType: CommandType.StoredProcedure);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error setting start node: {ex.Message}");
            }
        }
        public async Task<IEnumerable<string>> GetAllArea()
        {
            var sql = "SELECT AreaName from AreasMaster";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                    return await connection.QueryAsync<string>(sql, commandType: CommandType.Text);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all areas: {ex.Message}");
            }
        }
        public async Task<IEnumerable<string>> GetAllDept()
        {
            var sql = "SELECT DeptName from DeptsMaster";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                    return await connection.QueryAsync<string>(sql, commandType: CommandType.Text);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all departments: {ex.Message}");
            }
        }
    }
}
