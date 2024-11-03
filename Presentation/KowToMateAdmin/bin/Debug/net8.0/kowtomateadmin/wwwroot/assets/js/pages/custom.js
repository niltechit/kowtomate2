function saveAsFile(fileName, bytesBase64) {
    var link = document.createElement('a');
    link.download = fileName;
    link.href = 'data:application/octet-stream;base64,' + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
function triggerFileDownload(fileName, url) {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
}
function jsDownloadFile(filename, content) {
    const file = new File([content], filename, { type: "application/octet-stream" });
    const exportUrl = URL.createObjectURL(file);

    // Create the <a> element and click on it
    const a = document.createElement("a");
    document.body.appendChild(a);
    a.href = exportUrl;
    a.download = filename;
    a.target = "_self";
    a.click();

    // We don't need to keep the object url, let's release the memory
    URL.revokeObjectURL(exportUrl);
}

function jsDownloadFile(filename, content) {
    const file = new File([content], filename, { type: "application/octet-stream" });
    const exportUrl = URL.createObjectURL(file);

    // Create the <a> element and click on it
    const a = document.createElement("a");
    document.body.appendChild(a);
    a.href = exportUrl;
    a.download = filename;
    a.target = "_self";
    a.click();

    // We don't need to keep the object url, let's release the memory
    URL.revokeObjectURL(exportUrl);
}
function CustomCofirm(title, message, type) {

    return new Promise((resolve) => {
        Swal.fire({
            title: title,
            text: message,
            icon: type,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.value) {
                resolve(true);

            } else {
                resolve(false)
            }
        })
    })
}




function DeleteConfirmation(title, message, type) {

    return new Promise((resolve) => {
        Swal.fire({
            title: title,
            text: message,
            icon: type,
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#787878",
            confirmButtonText: "Yes, delete it!"
        }).then((result) => {
            //if (result.isConfirmed) {
            //    Swal.fire({
            //        title: "Deleted!",
            //        text: "Your file has been deleted.",
            //        icon: "success"
            //    });
            //}
            if (result.value) {
                resolve(true);

            } else {
                resolve(false)
            }
        })
    })
}



function CustomCofirmation(title, message, type) {

    return new Promise((resolve) => {
        Swal.fire({
            title: title,
            text: message,
            icon: type,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (result.value) {
                resolve(true);

            } else {
                resolve(false)
            }
        })
    })
}
function ReplaceConfirmation(title, message, type) {

    return new Promise((resolve) => {
        Swal.fire({
            title: title,
            text: message,
            icon: type,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Replace'

        }).then((result) => {
            if (result.value) {
                resolve(true);

            } else {
                resolve(false)
            }
        })
    })
}
function YesConfirmation(title, message, type) {

    return new Promise((resolve) => {
        Swal.fire({
            title: title,
            text: message,
            icon: type,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'

        }).then((result) => {
            if (result.value) {
                resolve(true);

            } else {
                resolve(false)
            }
        })
    })
}
// Files and Folder Attach
window.attachFileUploadHandler = () => {
    document.getElementById("myFileUpload").addEventListener("change", function (e) {
        var files = [];
        
        Array.from(e.target.files).forEach(file => {
            files.push({ Name: file.name, Path: file.webkitRelativePath });
        });

        //replcae BlazorApp1 with your application assembly name
        DotNet.invokeMethodAsync('KowToMateAdmin', 'GetSelectedFileDetails', files);
    });
    document.getElementById('#myFileUpload').value = []
};


function getFilePath() {
    $('input[type=file]').change(function () {
        debugger;
        var filePath = $('#myFileUpload').val();
    })
};

window.completeattachFileUploadHandler = () => {
    document.getElementById("myCompleteFileUpload").addEventListener("change", function (e) {
        var files = [];

        Array.from(e.target.files).forEach(file => {
            files.push({ Name: file.name, Path: file.webkitRelativePath });
        });

        //replcae BlazorApp1 with your application assembly name
        DotNet.invokeMethodAsync('KowToMateAdmin', 'GetCompleteSelectedFileDetails', files);
    });
};


function getFilePath() {
    $('input[type=file]').change(function () {
        var completefilePath = $('#myCompleteFileUpload').val();
    })
};

window.completeattachQCFileUploadHandler = () => {
    document.getElementById("myCompleteQCFileUpload").addEventListener("change", function (e) {
        var files = [];

        Array.from(e.target.files).forEach(file => {
            files.push({ Name: file.name, Path: file.webkitRelativePath });
        });

        //replcae BlazorApp1 with your application assembly name
        DotNet.invokeMethodAsync('KowToMateAdmin', 'GetCompleteQCSelectedFileDetails', files);
    });
};
//window.completeSingleattachQCFileUploadHandler = () => {
//    document.getElementById("myCompleteQCFileUpload").addEventListener("change", function (e) {
//        var files = [];

//        Array.from(e.target.files).forEach(file => {
//            files.push({ Name: file.name, Path: file.webkitRelativePath });
//        });

//        //replcae BlazorApp1 with your application assembly name
//        DotNet.invokeMethodAsync('KowToMateAdmin', 'GetCompleteQCSelectedFileDetails', files);
//    });
//};
window.rejectedattachQCFileUploadHandler = () => {
    document.getElementById("myRejectedQCFileUpload").addEventListener("change", function (e) {
        var files = [];

        Array.from(e.target.files).forEach(file => {
            files.push({ Name: file.name, Path: file.webkitRelativePath });
        });

        //replcae BlazorApp1 with your application assembly name
        DotNet.invokeMethodAsync('KowToMateAdmin', 'GetRejectedQCSelectedFileDetails', files);
    });
};
window.replaceattachQCFileUploadHandler = () => {
    document.getElementById("myReplaceQCFileUpload").addEventListener("change", function (e) {
        var files = [];

        Array.from(e.target.files).forEach(file => {
            files.push({ Name: file.name, Path: file.webkitRelativePath });
        });

        //replcae BlazorApp1 with your application assembly name
        DotNet.invokeMethodAsync('KowToMateAdmin', 'GetReplaceQCSelectedFileDetails', files);
    });
};
window.replaceattachQCFileUploadHandler = () => {
    document.getElementById("OrderSOPUploadUpdatedFile").addEventListener("change", function (e) {
        var files = [];

        Array.from(e.target.files).forEach(file => {
            files.push({ Name: file.name, Path: file.webkitRelativePath });
        });

        //replcae BlazorApp1 with your application assembly name
        DotNet.invokeMethodAsync('KowToMateAdmin', 'GetOrderSOPExistingNewItems', files);
    });
};

//window.addOrderNewItemsHandler = () => {
//    document.getElementById("orderNewItemsUpload").addEventListener("change", function (e) {
//        var files = [];

//        Array.from(e.target.files).forEach(file => {
//            files.push({ Name: file.name, Path: file.webkitRelativePath });
//        });

//        //replcae BlazorApp1 with your application assembly name
//        DotNet.invokeMethodAsync('KowToMateAdmin', 'GetOrderNewItems', files);
//    });
//};

window.addOrderNewItemsHandler = () => {
    document.getElementById("editOrderFileItem").addEventListener("change", function (e) {
        var files = [];

        Array.from(e.target.files).forEach(file => {
            files.push({ Name: file.name, Path: file.webkitRelativePath });
        });

        //replcae BlazorApp1 with your application assembly name
        DotNet.invokeMethodAsync('KowToMateAdmin', 'GetOrderNewItems', files);
    });
};
function downloadTextFile(fileName, content) {
    var link = document.createElement('a');
    link.download = fileName;
    link.href = 'data:text/plain;charset=utf-8,' + encodeURIComponent(content);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

// Browser closing 

window.browserReload = {
    initEditor: function (elementId) {
        window.onbeforeunload = function () {
            var mgs = "Are You sure closing the tab ? ";
            return mgs;
        };
    }
}

function getFilePath() {
    debugger;
    $('input[type=file]').change(function () {
        debugger;
        var filePath = $('#fileUpload').val();
    });
}

function check() {
    let inputs = document.getElementById('allOrderItemId');
    inputs.checked = true;
}

function uncheck() {
    debugger;
    let inputs = document.getElementById('allOrderItemId');
    inputs.checked = false;
}

//TODO:Rakib need to remove this code . this writing for catch pc ip
function getIPAddress() {
    debugger;
    window.RTCPeerConnection = window.RTCPeerConnection || window.mozRTCPeerConnection || window.webkitRTCPeerConnection;
    var pc = new RTCPeerConnection({ iceServers: [] }), noop = function () { };
    pc.createDataChannel("");
    pc.createOffer(pc.setLocalDescription.bind(pc), noop);
    pc.onicecandidate = function (ice) {
        if (!ice || !ice.candidate || !ice.candidate.candidate) return;
        var myIP = /([0-9]{1,3}(\.[0-9]{1,3}){3}|[a-f0-9]{1,4}(:[a-f0-9]{1,4}){7})/.exec(ice.candidate.candidate)[1];
        pc.onicecandidate = noop;
        console.log(myIP);
    };
}




//TODO:Rakib need to remove this code . this writing for catch pc ip

