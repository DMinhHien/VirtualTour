window.currentQrCode = null;
window.qrStyling = async (Data, elementId) => {
    const container = document.getElementById(elementId)
    container.innerHTML = "";
    const qrCode = new QRCodeStyling({
        width: 300,
        height: 300,
        data: Data,
        image: '/logo_framas_black_3.jpg',
        imageOptions: {
            crossOrigin: "anonymous",
            imageSize: 0.5,
            hideBackgroundDots: true,
            margin: 5
        },
        dotsOptions: {
            color: "#FFF",
            type: "square"
        },
        cornersSquareOptions: {
            color: "#FFF",
            type: "square"
        },
        cornersDotOptions: {
            color: "#FFF",
            type: "square"
        },
        backgroundOptions: {
            color: "#000"
        }
    });
    window.currentQrCode = qrCode;
    qrCode.append(container);
};
window.downloadCurrentQrCode = (extension) => {
    if (window.currentQrCode) {
        window.currentQrCode.download({ extension: extension });
    } else {
        console.error("No QR code instance found to download.");
    }
};
window.downloadQrCode =async (Data,extension) => {
    const qrCode = new QRCodeStyling({
        width: 300,
        height: 300,
        data: Data,
        image: '/logo_framas_black_3.jpg',
        imageOptions: {
            crossOrigin: "anonymous",
            imageSize: 0.5,
            hideBackgroundDots: true,
            margin: 5
        },
        dotsOptions: {
            color: "#FFF",
            type: "square",
        },
        cornersSquareOptions: {
            color: "#FFF",
            type: "square"
        },
        cornersDotOptions: {
            color: "#FFF",
            type: "square"
        },
        backgroundOptions: {
            color: "#000"
        }
    });
    qrCode.download({ extension: extension });
   
};