window.currentQrCode = null;

window.qrStyling = async (data, elementId) => {
    const container = document.getElementById(elementId);
    container.innerHTML = "";

    const qrCode = new QRCodeStyling({
        width: 300,
        height: 300,
        data: data,
        image: '/logo_framas_black_3.jpg', // đảm bảo file này có trong wwwroot
        imageOptions: {
            crossOrigin: "anonymous",
            imageSize: 0.5,
            hideBackgroundDots: true,
            margin: 5
        },
        dotsOptions: {
            color: "#000",
            type: "square"
        },
        cornersSquareOptions: {
            color: "#000",
            type: "square"
        },
        cornersDotOptions: {
            color: "#000",
            type: "square"
        },
        backgroundOptions: {
            color: "#FFF"
        }
    });

    window.currentQrCode = qrCode;
    qrCode.append(container);
};
// qrcode-helper.js
window.downloadCurrentQrCode = (format, filename) => {
    const canvas = document.querySelector("#qrCodeContainer canvas");
    if (!canvas) return;

    const link = document.createElement("a");
    link.download = `${filename || 'qr_code'}.${format}`;
    link.href = canvas.toDataURL("image/" + format);
    link.click();
};

/*window.downloadCurrentQrCode = (extension) => {
    if (window.currentQrCode) {
        window.currentQrCode.download({ extension: extension });
    } else {
        console.error("QR Code chưa được tạo hoặc bị null.");
    }
};
*/
window.reverseQrStyling = async (data, elementId, isReversed) => {
    const container = document.getElementById(elementId);
    container.innerHTML = "";
    let qrCode;
    if (!isReversed) {
        qrCode = new QRCodeStyling({
            width: 310,
            height: 310,
            margin: 1.7,
            data: data,
            image: '/logo_framas_black_3.jpg', // đảm bảo file này có trong wwwroot
            imageOptions: {
                crossOrigin: "anonymous",
                imageSize: 0.4,
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
            },
            qrOptions: {
                typeNumber: 0, 
                mode: "Byte", 
                errorCorrectionLevel: "H" 
            }
        });
    }
    else {
        qrCode = new QRCodeStyling({
            width: 300,
            height: 300,
            data: data,
            image: '/logo_framas_black_3.jpg', // đảm bảo file này có trong wwwroot
            imageOptions: {
                crossOrigin: "anonymous",
                imageSize: 0.5,
                hideBackgroundDots: true,
                margin: 5
            },
            dotsOptions: {
                color: "#000",
                type: "square"
            },
            cornersSquareOptions: {
                color: "#000",
                type: "square"
            },
            cornersDotOptions: {
                color: "#000",
                type: "square"
            },
            backgroundOptions: {
                color: "#FFF"
            }
        });
    }
    window.currentQrCode = qrCode;
    qrCode.append(container);
};