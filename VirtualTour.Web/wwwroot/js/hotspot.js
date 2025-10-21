window.addHotSpot = function (id) {
    const markersPlugin = window.viewer.getPlugin('markers');
    const yaw = window.viewer.getPosition().yaw;
    const pitch = window.viewer.getPosition().pitch;
    markersPlugin.addMarker({
        id: id,
        position: { yaw: yaw, pitch: pitch },
        videoLayer: 'Arrow Animation 2.mp4',
        size: { width: 220, height: 220 },
        anchor: 'bottom center',
        tooltip:`${id}`,
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
window.removeHotSpot = function (id) {
    const markersPlugin = window.viewer.getPlugin('markers');
    markersPlugin.removeMarker(id);
};

window.rotateHotSpot = function (id, angle) {
    const markersPlugin = window.viewer.getPlugin('markers');
    markersPlugin.updateMarker({
        id: id,
        rotation: { roll: `${angle}deg` },
    });
};