using VirtualTour.BL.Repositories;
using VirtualTour.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTour.BL.Services
{
    public interface INodeService
    {
        Task<IEnumerable<NodeModel>> GetAllNodesAsync();
        Task<IEnumerable<LinkedNodes>> GetAllLinksAsync(int nodeId);
        Task CreateNodeAsync(NodeModel nodeModel);
        Task CreateNodeLink(LinkedNodes linkedNode, int startNodeId);
        Task<int> GetMaxIdAsync();
        Task<NodeModel> GetByIdAsync(int id);
        Task DeleteNodeAsync(int id);
        Task UpdateNodeAsync(NodeModel nodeModel);
        Task DeleteNodeLinkAsync(int nodeId, int targetNodeId);
        Task UpdateNodeLinkAsync(LinkedNodes linkedNode, int startNodeId);
        Task SetStartNode(int currentNodeId, int previousNodeId);
        Task<int> GetStartIdAsync();
        Task<IEnumerable<string>> GetAllAreaAsync();
        Task<IEnumerable<string>> GetAllDeptAsync();
        Task<IEnumerable<NodeModel>> GetAllNodesPublicAsync(string tenantId);
    }
    public class NodeService : INodeService
    {
        private readonly INodeRepository _nodeRepository;
        public NodeService(INodeRepository nodeRepository)
        {
            _nodeRepository = nodeRepository;
        }
        public async Task<IEnumerable<NodeModel>> GetAllNodesAsync()
        {
            var nodes = await _nodeRepository.GetAllNodesAsync();

            foreach (var node in nodes)
            {
                var links = await _nodeRepository.GetAllLinks(node.Id);
                node.Links = links.ToList();
            }

            return nodes;
        }
        public async Task<IEnumerable<LinkedNodes>> GetAllLinksAsync(int nodeId)
        {
            return await _nodeRepository.GetAllLinks(nodeId);
        }
        public async Task CreateNodeAsync(NodeModel nodeModel)
        {
            await _nodeRepository.CreateNode(nodeModel);
        }
        public async Task<int> GetMaxIdAsync()
        {
            return await _nodeRepository.GetMaxId();
        }
        public async Task CreateNodeLink(LinkedNodes linkedNode, int startNodeId)
        {
            await _nodeRepository.CreateNodeLink(linkedNode, startNodeId);
        }
        public async Task<NodeModel> GetByIdAsync(int id)
        {
            var node = await _nodeRepository.GetById(id);
            var links = await _nodeRepository.GetAllLinks(node.Id);
            node.Links = links.ToList();
            return node;
        }
        public async Task DeleteNodeAsync(int id)
        {
                await _nodeRepository.DeleteNode(id);
        }
        public async Task UpdateNodeAsync(NodeModel nodeModel)
        {
            await _nodeRepository.UpdateNode(nodeModel);
        }
        public async Task DeleteNodeLinkAsync(int nodeId, int targetNodeId)
        {
            await _nodeRepository.DeleteNodeLink(nodeId, targetNodeId);
        }
        public async Task UpdateNodeLinkAsync(LinkedNodes linkedNode, int startNodeId)
        {
            await _nodeRepository.UpdateNodeLink(linkedNode, startNodeId);
        }
        public async Task SetStartNode(int currentNodeId, int previousNodeId)
        {
            await _nodeRepository.SetStartNode(currentNodeId, previousNodeId);
        }
        public async Task<int> GetStartIdAsync()
        {
            return await _nodeRepository.GetStartId();
        }
        public async Task<IEnumerable<string>> GetAllAreaAsync()
        {
            return await _nodeRepository.GetAllArea();
        }
        public async Task<IEnumerable<string>> GetAllDeptAsync()
        {
            return await _nodeRepository.GetAllDept();
        }
        public async Task<IEnumerable<NodeModel>> GetAllNodesPublicAsync(string tenantId)
        {
            var nodes = await _nodeRepository.GetAllNodesPublicAsync(tenantId);
            foreach (var node in nodes)
            {
                var links = await _nodeRepository.GetAllLinks(node.Id);
                node.Links = links.ToList();
            }
            return nodes;
        }

    }
}
