 window.initVirtualTourApi = async (containerId, nodeData, dotNetHelper) => {
    // Import modules for the viewer and markers plugin
    const module = await import('@photo-sphere-viewer/core');
    const { Viewer } = module;
    const markersModule = await import('@photo-sphere-viewer/markers-plugin');
    const { MarkersPlugin } = markersModule;

    // Ensure any existing viewer is destroyed
    const container = document.querySelector(`#${containerId}`);
    const baseUrl = 'https://photo-sphere-viewer-data.netlify.app/assets/';
    if (window.viewer && typeof window.viewer.destroy === 'function') {
        window.viewer.destroy();
        window.viewer = null;
    }

    const markers = [];
    if (nodeData.links && nodeData.links.length > 0) {
        nodeData.links.forEach(link => {
            const yaw = (link.yaw ?? link.Yaw) || 0;
            const pitch = (link.pitch ?? link.Pitch) || 0;
            const rotation = (link.rotation ?? link.Rotation) || 0;
            markers.push({
                id: `marker-${link.markerId}`,
                videoLayer: 'Arrow Animation 2.mp4',
                tooltip: `${link.targetName}`,
                size: { width: 220, height: 220 },
                rotation: { roll: `${rotation}deg` },
                chromaKey: {
                    enabled: true,
                    color: '#009200',
                    similarity: 0.1,
                },
                anchor: 'bottom center',
                position: {
                    yaw: `${yaw}deg`,
                    pitch: `${pitch}deg`
                },
                data: { targetNode: link.targetNode }
            });
        });
    }
    window.viewer = new Viewer({
        container: container,
        panorama: nodeData.panorama,
        caption:nodeData.caption,
        defaultYaw: `${nodeData.defaultYaw}deg`,
        defaultPitch: `${nodeData.defaultPitch}deg`,
        loadingImg: baseUrl + 'loader.gif',
        navbar: ['zoom', 'move', 'caption', 'fullscreen'],
        plugins: [
            MarkersPlugin.withConfig({
                markers: markers // Markers added below
            }),
        ],
    });

    const markersPlugin = window.viewer.getPlugin(MarkersPlugin);
    markersPlugin.addEventListener('select-marker', async ({ marker }) => {
        const targetNodeId = marker.data ? marker.data.targetNode : null;
        if (targetNodeId) {
            try {
                dotNetHelper.invokeMethodAsync('GetNextNode', targetNodeId);
            } catch (err) {
                console.error('Error fetching linked node:', err);
            }
        }
    });
};
window.initVirtualTourEdit = async (nodeData, dotNetHelper) => {

    // Import modules for the viewer and markers plugin
    const module = await import('@photo-sphere-viewer/core');
    const { Viewer } = module;
    const markersModule = await import('@photo-sphere-viewer/markers-plugin');
    const { MarkersPlugin } = markersModule;

    // Ensure any existing viewer is destroyed
    const container = document.getElementById('viewer');
    const baseUrl = 'https://photo-sphere-viewer-data.netlify.app/assets/';
    if (window.viewer && typeof window.viewer.destroy === 'function') {
        window.viewer.destroy();
        window.viewer = null;
    }

    const markers = [];
    if (nodeData.links && nodeData.links.length > 0) {
        nodeData.links.forEach(link => {
            const yaw = (link.yaw ?? link.Yaw) || 0;
            const pitch = (link.pitch ?? link.Pitch) || 0;
            const rotation = (link.rotation ?? link.Rotation) || 0;
            markers.push({
                id: `${link.markerId}`,
                videoLayer: 'Arrow Animation 2.mp4',
                tooltip: `${link.targetName}`,
                size: { width: 220, height: 220 },
                rotation: { roll: `${rotation}deg` },
                chromaKey: {
                    enabled: true,
                    color: '#009200',
                    similarity: 0.1,
                },
                anchor: 'bottom center',
                position: {
                    yaw: `${yaw}deg`,
                    pitch: `${pitch}deg`
                },
                data: { targetNode: link.targetNode }
            });
        });
    }
    window.viewer = new Viewer({
        container: container,
        panorama: nodeData.panorama,
        defaultYaw: `${nodeData.defaultYaw}deg`,
        defaultPitch: `${nodeData.defaultPitch}deg`,
        navbar: ['zoom', 'move', 'caption', 'fullscreen'],
        plugins: [
            MarkersPlugin.withConfig({
                markers: markers 
            }),
        ],
    });
    const markerPlugin = viewer.getPlugin(MarkersPlugin);
    let dragging = false;
    let draggedMarker = null;
    let spherical = null;
    markerPlugin.addEventListener('select-marker', ({ marker, rightClick }) => {
        if (rightClick) {
            const numericId = parseInt(marker.id);
            dotNetHelper.invokeMethodAsync('HotSpotSelected', numericId);
            console.log(`Right clicked marker ${marker.id}. SelectedHotSpotId updated.`);
            return;
        }
        if (dragging && draggedMarker?.id === marker.id) {
            dragging = false;
            if (spherical) {
                const numericId = parseInt(draggedMarker.id);
                dotNetHelper.invokeMethodAsync('UpdateHotSpotPosition', numericId, spherical.yaw, spherical.pitch - 0.1);
            }
            draggedMarker = null;
            console.log('Dragging stopped');
        } else {
            dragging = true;
            draggedMarker = marker;
            console.log(`Started dragging marker: ${marker.id}`);
        }
    });
    viewer.container.addEventListener('pointermove', (e) => {
        if (!dragging || !draggedMarker) return;

        const rect = viewer.container.getBoundingClientRect();
        const viewerPoint = {
            x: e.clientX - rect.left,
            y: e.clientY - rect.top,
        };

        const intersections = viewer.renderer.getIntersections(viewerPoint);
        if (!intersections.length) return;

        const point = intersections[0].point;
        spherical = viewer.dataHelper.vector3ToSphericalCoords(point);
        if (!spherical) return;

        markerPlugin.updateMarker({
            id: draggedMarker.id,
            position: {
                yaw: spherical.yaw,
                pitch: spherical.pitch - 0.1,
            },
        });

    });
};

window.initViewer = async (imageUrl, dotNetHelper,linkId) =>{
    const module = await import('@photo-sphere-viewer/core');
    const { Viewer } = module;
    const markersModule = await import("@photo-sphere-viewer/markers-plugin");
    const { MarkersPlugin } = markersModule;
    const container = document.getElementById('viewer');
    const baseUrl = 'https://photo-sphere-viewer-data.netlify.app/assets/';
    if (window.viewer && typeof window.viewer.destroy === 'function') {
        window.viewer.destroy();
        window.viewer = null;
    }
    const linkedMarkers = [];
    if (linkId && linkId.length > 0) {
        linkedMarkers.push({
            id: "0",
            position: { yaw: 0, pitch: 0 },
            videoLayer: 'Arrow Animation 2.mp4',
            size: { width: 220, height: 220 },
            anchor: 'bottom center',
            tooltip: `Previous node`,
            data: {
                generated: true,
            },
            chromaKey: {
                enabled: true,
                color: '#009200',
                similarity: 0.1,
            },
        });
    }
    console.log(linkedMarkers)
    window.viewer = new Viewer({
        container: container,
        panorama: imageUrl,
        defaultYaw: 0,
        navbar: ['zoom', 'move', 'caption', 'fullscreen'],
        plugins: [
            MarkersPlugin.withConfig({
                markers: linkedMarkers
            }),
        ],
    });
    const markers = viewer.getPlugin(MarkersPlugin);
    let dragging = false;
    let draggedMarker = null;
    let spherical = null;
    markers.addEventListener('select-marker', ({ marker, rightClick }) => {
        if (rightClick) {
            const numericId = parseInt(marker.id);
            dotNetHelper.invokeMethodAsync('HotSpotSelected', numericId);
            console.log(`Right clicked marker ${marker.id}. SelectedHotSpotId updated.`);
            return;
        }
        if (dragging && draggedMarker?.id === marker.id) {
            dragging = false;
            if (spherical) {
                const numericId = parseInt(draggedMarker.id);
                dotNetHelper.invokeMethodAsync('UpdateHotSpotPosition', numericId, spherical.yaw, spherical.pitch - 0.1);
            }
            draggedMarker = null;
            console.log('Dragging stopped');
        } else {
            dragging = true;
            draggedMarker = marker;
            console.log(`Started dragging marker: ${marker.id}`);
        }
    });
    viewer.container.addEventListener('pointermove', (e) => {
        if (!dragging || !draggedMarker) return;

        const rect = viewer.container.getBoundingClientRect();
        const viewerPoint = {
            x: e.clientX - rect.left,
            y: e.clientY - rect.top,
        };

        const intersections = viewer.renderer.getIntersections(viewerPoint);
        if (!intersections.length) return;

        const point = intersections[0].point;
        spherical = viewer.dataHelper.vector3ToSphericalCoords(point);
        if (!spherical) return;

        markers.updateMarker({
            id: draggedMarker.id,
            position: {
                yaw: spherical.yaw,
                pitch: spherical.pitch - 0.1,
            },
        });

    });
};
window.initVirtualTourApiIndex = async (containerId, startId) => {
    const baseUrl = 'https://photo-sphere-viewer-data.netlify.app/assets/';
    const caption = 'Cape Florida Light, Key Biscayne <b>&copy; Pixexid</b>';
    const module = await import('@photo-sphere-viewer/core');
    const { Viewer } = module;
    const vtModule = await import("@photo-sphere-viewer/virtual-tour-plugin");
    const { VirtualTourPlugin } = vtModule;
    const galleryModule = await import("@photo-sphere-viewer/gallery-plugin");
    const { GalleryPlugin } = galleryModule;
    const markersModule = await import("@photo-sphere-viewer/markers-plugin");
    const { MarkersPlugin } = markersModule;
    let nodesData;
    try {
        const response = await fetch('http://localhost:5310/api/node/getListUse');
        if (!response.ok) {
            throw new Error(`Failed to fetch nodes: ${response.statusText}`);
        }
        const result = await response.json();
        nodesData = result.data;
    } catch (error) {
        console.error('Error loading nodes:', error);
        return;
    }

    const nodes = nodesData.map(node => ({
        id: String(node.id),
        panorama: node.panorama,
        thumbnail: node.thumbnail,
        name: node.name,
        caption: node.caption ? node.caption : `[${node.id}] ${caption}`,
        markers: node.links && node.links.length > 0 ? node.links.map(link => {
            const yaw = (link.yaw ?? link.Yaw) || 0;
            const pitch = (link.pitch ?? link.Pitch) || 0;
            const rotation = (link.rotation ?? link.Rotation) || 0;
            return {
                id: `marker-${link.targetNode}`,
                videoLayer: 'Arrow Animation 2.mp4',
                tooltip: link.tooltip || link.Tooltip || `Go to node ${link.targetNode}`,
                size: { width: 220, height: 220 },
                rotation: { roll: `${rotation}deg` },
                chromaKey: {
                    enabled: true,
                    color: '#FFFFFF',
                    similarity: 0.1,
                },
                anchor: 'bottom center',
                position: {
                    yaw: `${yaw}deg`,
                    pitch: `${pitch}deg`
                },
                data: { targetNode: link.targetNode }
            };
        }) : []
    }));
    const viewer = new Viewer({
        container: document.querySelector(`#${containerId}`),
        loadingImg: baseUrl + 'loader.gif',
        touchmoveTwoFingers: true,
        mousewheelCtrlKey: true,
        defaultYaw: '130deg',
        navbar: 'zoom move gallery caption fullscreen',

        plugins: [
            MarkersPlugin,
            [GalleryPlugin, {
                thumbnailSize: { width: 100, height: 100 },
            }],
            [VirtualTourPlugin, {
                positionMode: 'manual',
                renderMode: '3d',
                nodes: nodes,
                startNodeId: startId,
            }],
        ],
    });
};
window.setInitialView= function () {
    if (window.viewer) {
        console.log(window.viewer.getPosition())
        return window.viewer.getPosition();
    }
};


