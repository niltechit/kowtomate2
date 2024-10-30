const downloadFileFromStream = async (fileName, contentStreamReference) => {
    //const arrayBuffer = await contentStreamReference.arrayBuffer();
    //const blob = new Blob([arrayBuffer]);
    //const url = URL.createObjectURL(blob);
    //const anchorElement = document.createElement('a');
    //anchorElement.href = url;
    //anchorElement.download = fileName ?? '';
    //anchorElement.click();
    //anchorElement.remove();
    //URL.revokeObjectURL(url);


    console.log(fileName);
    


}



function downloadFile(mimeType, baseString, fileName) {
    alert("test test")
    var fileDataUrl = "data:" + mimeType + ";base64" + baseString;
    alert(fileDataUrl)
    fetch(fileDataUrl).then(response => response.blod)
        .then(blob => {
            var link = window.document.createElement("a");
            link.href = window.URL.crateObjectUrl(blob, { type: mimeType });
            link.download = fileName;

            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        })
}

