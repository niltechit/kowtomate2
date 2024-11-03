var viewMode = '';
var currentFolderPath = '';
var zoom = 1.0;

var viewModeConstants =
{
    fullScreenView : "full-screen-view",
    galleryView : "gallery-view",
    listView : "list-view",
    largeView : "large-view",
    smallView : "small-view",
};

var _loaderHtml = '<div class="spinner-border text-info" role="status"><span class="visually-hidden">Loading...</span></div>';

var driveManager = {
    pageInitialize: function () {
        $('.comment-body').hide();
        $(".paint-tool").show();
        viewMode = viewModeConstants.smallView;

        driveManager.initializeImageSelectors();
        driveManager.initializeImagePreview();
    },
   
    initializeImageSelectors: function () {

        $('.select-all-img').click(function (e) {
            $('.img-selector').prop('checked', this.checked);

            if (viewMode != viewModeConstants.galleryView) {
                if ($("#image-container .img-selector:checked").length > 0) {
                    var selImage = $("#image-container .img-selector:checked").first();
                    $('.sel-file-size').html(selImage.attr('size'));
                    $('.sel-file-date').html(selImage.attr('date'));
                    $('.sel-file-name').html(selImage.attr('file-name'));
                }
                else {
                    driveManager.cleanSelectedFileInfo();
                }
            }

        });

        $("#image-container").on("click", '.img-selector', function () {
            if ($("#image-container .img-selector").length == $("#image-container .img-selector:checked").length) {

                $('.select-all-img').prop('checked', true);
            }
            else {
                $('.select-all-img').prop('checked', false);
            }

            if (viewMode != viewModeConstants.galleryView) {
                if ($(this).prop('checked')) {
                    $('.sel-file-size').html($(this).attr('size'));
                    $('.sel-file-date').html($(this).attr('date'));
                    $('.sel-file-name').html($(this).attr('file-name'));
                    return;
                }

                if ($("#image-container .img-selector:checked").length > 0) {

                    var selImage = $("#image-container .img-selector:checked").first();

                    $('.sel-file-size').html(selImage.attr('size'));
                    $('.sel-file-date').html(selImage.attr('date'));
                    $('.sel-file-name').html(selImage.attr('file-name'));
                }
                else {
                    driveManager.cleanSelectedFileInfo();
                }
            }
        });  
    },
    initializeImagePreview: function () {
        $("#image-container").on("dblclick", '.img-box', function () {
            //var processedImagePath = $(this).find('.processed-img').attr('src');
            var relativePath = $(this).find('.img-selector').val();
            driveManager.getBase64StringByPathAndShowCanvus(relativePath);

            $("#preview-modal .modal-title").html("Image: " + $(this).attr('img-name'));

        });
    },
    saveMarkupImageWithComment: function () {
        debugger;
        //disable buttion
        var btn = $('#bntmarkandreject');
        btn.prop("disabled", true);

        var dataURL = canvas.toDataURL("image/png");
        var comment = $("#previewComment").val();
        var orderId = $("#selectedOrderId").val();
        var orderItemId = $("#selectedOrderItemId").val();
        var contactId = $("#selectedContactId").val();

        var data1 = {
            base64: dataURL,
            drivePath: drivePath,
            comment: comment,
            orderId: orderId,
            OrderItemId: orderItemId,
            CreatedContactId: contactId,
        };

        $.ajax({
            url: '/Images/SaveMarkup',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data1),
        }).done(function (data) {
            debugger;
            console.log(data);

            if (type = "success") {
                debugger;
                /*$("#previewComment").val("");*/
                $("body").find("#closeButton").click();
                $("body").find("#refreshBtnForOrderDetail").click();

                /*$('#preview-modal').modal('hide');*/


                //commonfs.showGeneralNotificationModal('Message', data.message);
                //$("#image-container").find(`[img-name='${data.result}']`).remove();

                //if (viewMode == viewModeConstants.galleryView) {

                //    var imageSize = driveManager.getImageSizeByViewMode(viewMode);
                //    driveManager.showFolderImages(currentFolderPath, imageSize);

                //    //var thumbEle = $('#image-container .thum-img-container img');

                //    //if (thumbEle) {
                //    //    thumbEle.first().trigger('click');
                //    //}
                //}
                //Set next image
            }
            else {
               /* commonfs.showGeneralNotificationModal('Message', data.message);*/
            }

            btn.prop("disabled", false);

        }).fail(function () {
            //driveManager.write("ajax error");
            btn.prop("disabled", false);
        });        
    },
    getViewClassByViewMode: function (viewMode) {
        if (viewMode == viewModeConstants.smallView) {
            return 'col-2';
        }
        else if (viewMode == viewModeConstants.largeView) {
            return 'col-4';
        }

        return 'col-2';
    },
    getImageSizeByViewMode: function (viewMode) {
        if (viewMode == viewModeConstants.smallView) {
            return 300;
        }
        else if (viewMode == viewModeConstants.largeView) {
            return 600;
        }
        else if (viewMode == viewModeConstants.galleryView) {
            return 800;
        }

        return 300;
    },
    dimensionGeneratingPromise: async function (rawImagePath) {
        return new Promise((resolve, reject) => {
            let img = new Image();
            img.src = rawImagePath;

            img.onload = () => {
                let dimension = {
                    width: img.width,
                    height: img.height
                };

                resolve(dimension);
            };
            img.onerror = () => reject(null);
        });
    },
    generateHtmlForGalleryView: async function (images) {

        if (images == null || images.length == 0) {
            return "";
        }

        var thumbs = ``;
        var mainSliderImages = ``;
        var active = `active`;
        var areacurent = `aria-current="true"`;

        var winHeight = $(window).height();

        winHeight = winHeight - 280;
        //alert(winHeight);


        for (var i = 0; i < images.length; i++) {
            var imageData = images[i];

            //var img = `<div class="aics-autostart" data-auto-animation="false">`+
            //                `<div class="images">`+
            //                   `<div class="image-rgt" data-src="` + imagedata.fullpath + `"></div>`+
            //                    `<div class="image-lft" data-src="` + imagedata.rawimagepath + `"></div>`+
            //                `</div>`+
            //            `</div>`;

            //slider images
            //var img = `<img src="` + imageData.fullPath + `" class="" alt="...">`;

            var img = `<div class="js-slider-container gallery-slider-container">
                          <img src="` + imageData.rawImagePath + `" class="js-slider-first" style="width:500px;height:90%;object-fit:contain;background-color:#eef5f9;">
                          <div class="js-slider-last-container">
                            <img src="` + imageData.fullPath + `" class="js-slider-last" style="width:500px;height:90%;object-fit:contain;background-color:#eef5f9;">
                          </div>
                      </div>`;


            mainSliderImages = mainSliderImages + `<div img-path="` + imageData.path + `" img-name="` + imageData.name + `" class="carousel-item ` + active + `">
                          `+ img + `
                          </div>`

            //mainSliderImages = mainSliderImages + `<div img-path="` + imageData.path+`" img-name="` + imageData.name + `" class="carousel-item ` + active + `">
            //                ` + img + `
            //              </div>`;

            //Thumbs images
            thumbs = thumbs + `<div  class="thum-img-container" img-name="` + imageData.name + `"><input type="checkbox" class="img-selector" file-name="` + imageData.name + `" name="img-selector" date="` + imageData.lastModifiedDate + `" size="` + imageData.displaySize + `" value="` + imageData.path + `"/>  <img class="` + active + `"  data-bs-target="#galleryImgPreviewContainer" data-bs-slide-to="` + i + `" ` + areacurent + ` aria-label="Slide ` + i + `" width=90 height=90 img-name="` + imageData.name + `" src="` + imageData.fullPath.replace("/800x0/", "/300x0/") + `"/></div>`
            //thumbs = thumbs + '<img img-name="' + imageData.name +'" src="' + imageData.fullPath + '" data-bs-target="#galleryImgPreviewContainer" data-bs-slide-to="' + i + '" class="' + active + '" aria-current="true" aria-label="Slide ' + i + '">';

            active = ``;
            areacurent = ``;

            console.log(imageData.rawImagePath);
            console.log(imageData.fullPath);

        }

        var html = `<div class="row">
                          <div class="col-4 col-md-5">
                          </div>
                          <div class="col-2 col-md-1 zoomIn">
                            <i class="bi bi-zoom-in"></i>
                          </div>
                          <div class="col-2 col-md-1 zoomOut">
                            <i class="bi bi-zoom-out"></i>
                          </div>
                          <div class="col-4 col-md-5">
                          </div>
                        </div>

                       <div class="row">
                      <div id="galleryImgPreviewContainer" class="carousel slide" data-bs-ride="carousel" data-bs-touch="false" data-bs-interval="false">

                       
                        <div class="carousel-inner" style="height:`+ winHeight +`px !important;">
                           `+ mainSliderImages +`
                        </div>

                         <button class="carousel-control-prev" type="button" data-bs-target="#galleryImgPreviewContainer" data-bs-slide="prev">
                          <span  style="margin-left: -98%;background-color: black;"class="carousel-control-prev-icon" aria-hidden="true"></span>
                          <span class="visually-hidden">Previous</span>
                        </button>
                        <button class="carousel-control-next" type="button" data-bs-target="#galleryImgPreviewContainer" data-bs-slide="next">
                          <span style="margin-right: -98%;background-color: black;"class="carousel-control-next-icon" aria-hidden="true"></span>
                          <span class="visually-hidden">Next</span>
                        </button>

 <div class="carousel-indicators" id="mobile-slider-view"style="overflow: auto;">
                          ` + thumbs + `
                        </div>

                      </div>
                      </div>`;

        return html;
    },
    generateHtmlForRegularView: function (images) {
        var html = "";

        if (images == null || images.length == 0) {
            return '<div class="alert alert-warning" role="alert">There is no file in this folder.</div>';
        }

        for (var i = 0; i < images.length; i++) {
            var imageData = images[i];

            /* var img = '<img alt="..." class="img-thumbnail" src="' + imageData.path + '"/>'*/
            //var img = '<div class="img-comp-container"><div class="img-comp-img"><img class="processed-img" src="' + imageData.fullPath + '" width="100">' +
            //    '</div><div class="img-comp-img img-comp-overlay"><img src="' + imageData.rawImagePath + '" width="100"></div></div>';

            var img = `<div class="thum-img-holder aics-autostart" data-auto-animation="false">
                            <div class="images">
                                <div class="img-com-item image-rgt" data-src="` + imageData.fullPath  + `"></div>
                                <div class="img-com-item image-lft" data-src="` + imageData.rawImagePath + `"></div>
                            </div>
                        </div>`;

            var imageHtml = '<div img-name="' + imageData.name + '" class="img-box ' + driveManager.getViewClassByViewMode(viewMode) + '" style="height:max-content">' + img +
                '<p class="img-chkbox-holder"><input type="checkbox" class="img-selector img-selector-small-view" id="chk-img-' + i + '" name="img-selector" file-name="' + imageData.name + '" date="' + imageData.lastModifiedDate + '" size="' + imageData.displaySize + '" value="' + imageData.path + '" /><label class="img-label" for="chk-img-'+i+'">' + imageData.name + '</label></p></div>';

            //old design
            //var imageHtml = '<div class="img-box" img-name="' + imageData.name+'"><div class="img img-' + i + '">' + img + '</div>' +
            //    '<div><input type="checkbox" value="' + imageData.path+'" class="img-selector" name="img-selector"/> <span>' + imageData.name + '</span></div></div>';
            //console.log(imageHtml);
            html = html + imageHtml;
            
            //else if (imageData.pathType == "2") {
            //    var imageHtml = '<div class="img-box"><div class="img img-' + i + '">' + _loaderHtml + '</div>' +
            //        '<div><input type="checkbox" class="img-selector" name="img-selector"/> <span>' + imageData.name + '</span></div></div>';
            //    //console.log(imageHtml);
            //    html = html + imageHtml;
            //    driveManager.loadBase64StringByPath(".img-" + i, imageData.path);
            //}
        }

        return html;

    },
    showFolderImages: function (path, imageSize) {

        //set folder name
        $('.comment-body').show();
        var imageContainer = $("#image-container");
        imageContainer.html(_loaderHtml);

         driveManager.cleanSelectedFileInfo();

        var url = "";
        if (viewMode == viewModeConstants.smallView || viewMode == viewModeConstants.largeView) {
            url = '/images/getimagesbypath?path=' + encodeURIComponent(path) + '&imgWidth=' + imageSize;
        }
        else if (viewMode == viewModeConstants.galleryView) {
            url = '/images/getimagesbypath?path=' + encodeURIComponent(path) + '&imgWidth=' + imageSize;
            //url = '/images/getimagesbypath?path=' + encodeURIComponent(path) + '&imgHeight=' + 600;
        }

        
        $.ajax({
            url: url,
            dataType: 'json',

        }).done(async function (data) {
            var html = "";

            if (viewMode == viewModeConstants.smallView || viewMode == viewModeConstants.largeView) {
                html = driveManager.generateHtmlForRegularView(data.result);
            }
            else if (viewMode == viewModeConstants.galleryView) {
                html = await driveManager.generateHtmlForGalleryView(data.result);
            }

            //Set Images
            imageContainer.html(html);
            //Preview with actual image inilialize
            //initComparisons();

            if (viewMode == viewModeConstants.galleryView) {
                //Set first element as default
                var activeImg1 = $('.thum-img-container .active');

                if (activeImg1) {
                    var selImage1 = activeImg1.parent().find('.img-selector');
                    $('.sel-file-size').html(selImage1.attr('size'));
                    $('.sel-file-date').html(selImage1.attr('date'));
                    $('.sel-file-name').html(selImage1.attr('file-name'));                    
                }

                //initComparisons();
                $('#galleryImgPreviewContainer').on('slid.bs.carousel', function () {
                    var activeImg = $('.thum-img-container .active');

                    if (activeImg) {
                        var selImage = activeImg.parent().find('.img-selector');
                        $('.sel-file-size').html(selImage.attr('size'));
                        $('.sel-file-date').html(selImage.attr('date'));
                        $('.sel-file-name').html(selImage.attr('file-name'));
                    }
                });

                //$('.aics-autostart').anyImageComparisonSlider();
                $(".js-slider-container").slider({
                    'rollback': false,
                    'duration': 0,
                    'width': '0%'
                });

                // Disable lazzy loading
                //driveManager.preLoadImagesForGalleryView(data.result);

            }
            else {
                $('.aics-autostart').anyImageComparisonSlider();
            }
            
        }).fail(async function () {
            //driveManager.write("ajax error");
        });
    },
    preLoadImagesForGalleryView: async function (data) {
      
        if (data && data.length > 0) {
            for (var i = 1; i < data.length; i++) {
                var imageData = data[i];

                //var image1 = $('<img />').attr('src', imageData.fullPath);
                //image1.hide().appendTo("body")[0];

                //var image2 = $('<img />').attr('src', imageData.rawImagePath);
                //image2.hide().appendTo("body")[0];

                var img = $("<img class=''/>").attr('src', imageData.fullPath)
                    .on('load', function () {
                        if (!this.complete || typeof this.naturalWidth == "undefined" || this.naturalWidth == 0) {
                            //alert('broken image!');
                        } else {
                            this.hide().appendTo("body")[0];
                        }
                    });

                var img2 = $("<img class=''/>").attr('src', imageData.rawImagePath)
                    .on('load', function () {
                        if (!this.complete || typeof this.naturalWidth == "undefined" || this.naturalWidth == 0) {
                            //alert('broken image!');
                        } else {
                            this.hide().appendTo("body")[0];
                        }
                    });

            }
        }
    },
    loadBase64StringByPath: function (selector, path) {

        var url = '/images/GetBase64StringByPath?path=' + encodeURIComponent(path);
        $.ajax({
            url: url,
            dataType: 'json'
        }).done(function (response) {
            console.log(response);

            $('#image-container ' + selector).html('<img alt="..." class="img-thumbnail" src="'+ response.result + '"/>');
        }).fail(function () {
        });
    },
    moveImagesWithComments: function (actionType,selectedImages, comments) {

        var data = new FormData();

        //disable buttion
        var form = $('.comment-body'); 
        form.find('.action-btn').prop("disabled", true);

        //set fields
        data.append("actionType", actionType);
        data.append("selectedImages", selectedImages);
        data.append("comments", comments);

        //set file
        //var files = $(fileContainer).get(0).files;
        //if (files.length > 0)
        //{
        //    data.append("attachmentFile", files[0]);
        //}
        
        $.ajax({
            url: '/Images/MoveFiles',
            type: "POST",
            processData: false,
            data: data,
            dataType: 'json',
            contentType: false
        }).done(function (data) {

            if (data.isSuccess) {

                commonfs.showGeneralNotificationModal('Message', data.message);

                var images = data.result;
                if (images && images.length > 0) {
                    for (var i = 0; i < images.length; i++) {
                        var imageData = images[i];

                        if (imageData.isSuccess) {
                            $("#image-container").find(`[img-name='${imageData.fileName}']`).remove();
                        }
                    }

                    if (viewMode == viewModeConstants.galleryView) {
                        var imageSize = driveManager.getImageSizeByViewMode(viewMode);
                        driveManager.showFolderImages(currentFolderPath, imageSize);
                    }
                }

                //Reset data
                driveManager.resetTool();
            }
            else {
                commonfs.showGeneralNotificationModal('Message', data.message);
            }

            form.find('.action-btn').prop("disabled", false);

        }).fail(function () {
            form.find('.action-btn').prop("disabled", false);
        });
    },
    resetTool: function () {
        $("#comment").val("");
        $('.select-all-img').prop('checked', false);
        $('.img-selector').prop('checked', false);

        driveManager.cleanSelectedFileInfo();
    },
    cleanSelectedFileInfo: function () {
        $('.sel-file-size').html('');
        $('.sel-file-date').html('');
        $('.sel-file-name').html('');
    },
    getBase64StringByPathAndShowCanvus: function (path) {

        $('#preview-modal').modal('show');

        $(".canvasLoader").show();
        $("#markupCanvas").hide();


        $.ajax({
            url: '/Images/GetBase64ByPath?path=' + path,
            method: 'GET',
            success: function (data) {
                if (data.isSuccess) {
                    $(".canvasLoader").hide();
                    $("#markupCanvas").show();
                    init(data.result, path);                    
                }
            },
            error: function (res) {

            }
        });
    }
};

function initDriveManager() {

    $(document).ready(function () {
        debugger;
        //Image Markup
        canvas = document.getElementById('markupCanvas');
        ctx = canvas.getContext("2d");

        //Initialize page 
        driveManager.pageInitialize();

        //Accept Images
        $('.btnAccept').click(function (e) {

            var selectedImgArray = [];

            $('.img-selector:checked').each(function () {
                selectedImgArray.push(this.value);
            });

            if (selectedImgArray.length == 0) {
                commonfs.showGeneralNotificationModal('Message', 'Please select atleast 1 file.');
                return;
            }

            commonfs.showGeneralNotificationModal("Accept Confirmation", "Are you sure you want to accept the selected  " + selectedImgArray.length + " file(s)?", "accept-files", "", "Yes", "No");
        });

        //Reject Images
        $('.btnReject').click(function (e) {
            var selectedImgArray = [];

            $('.img-selector:checked').each(function () {
                selectedImgArray.push(this.value);
            });

            if (selectedImgArray.length == 0) {
                commonfs.showGeneralNotificationModal('Message', 'Please select atleast 1 file.');
                return;
            }

            var comments = $("#comment").val();

            if (comments.length == 0) {
                commonfs.showGeneralNotificationModal('Message', 'Please add comment.');
                return;
            }

            commonfs.showGeneralNotificationModal("Reject Confirmation", "Are you sure you want to reject the selected " + selectedImgArray.length + " file(s)?", "reject-files", "", "Yes", "No");
        });

        $('#GeneralNotificationModal').on("click", "#btnConfimationOK", function () {
            var action = $(this).attr("action");

            if (action == "accept-files") {
                var selectedImgArray = [];
                $('.img-selector:checked').each(function () {
                    selectedImgArray.push(this.value);
                });

                var selectedImages = selectedImgArray.join("|");
                driveManager.moveImagesWithComments("Accepted", selectedImages, "");
                commonfs.hideGeneralNotificationModal();
            }
            else if (action == "reject-files") {
                var selectedImgArray = [];
                $('.img-selector:checked').each(function () {
                    selectedImgArray.push(this.value);
                });
                var selectedImages = selectedImgArray.join("|");

                var comments = $("#comment").val();
                driveManager.moveImagesWithComments("Rejected", selectedImages, comments)
                commonfs.hideGeneralNotificationModal();
            }
        });

        $('#bntmarkandreject').click(function (e) {
            driveManager.saveMarkupImageWithComment();
        });

        //change view event listener
        $(".btn-change-view").click(function (event) {
            event.preventDefault();

            viewMode = $(this).attr("href");

            //if (viewMode == viewModeConstants.galleryView) {
            $(".paint-tool").show();
            //}
            //else {
            //    $(".paint-tool").hide();
            //}

            var imageEle = $("#image-container .img-selector");

            if (imageEle && imageEle.length > 0) {
                var imageSize = driveManager.getImageSizeByViewMode(viewMode);
                driveManager.showFolderImages(currentFolderPath, imageSize);
            }
        });
        //Todo: Rakib 
        $(".paint-tool").click(function () {
            let relativePath = "";

            if (viewMode == viewModeConstants.galleryView) {
                let element = $("#image-container").find(".carousel-item.active");
                relativePath = element.attr("img-path");

                $("#preview-modal .modal-title").html("Image: " + element.attr('img-name'));
            }
            else if (viewMode == viewModeConstants.smallView || viewMode == viewModeConstants.largeView) {


                if ($("#image-container .img-selector:checked").length > 0) {
                    var selImage = $("#image-container .img-selector:checked").first();

                    relativePath = selImage.val();

                    $("#preview-modal .modal-title").html("Image: " + selImage.attr('file-name'));
                }
            }

            if (relativePath && relativePath.length > 0) {
                driveManager.getBase64StringByPathAndShowCanvus(relativePath);
            }
        });

        $("#image-container").on("click", ".zoomIn", function () {

            if (zoom.toFixed(2) == 3.0) return;

            zoom += 0.2;

            let element = $("#image-container").find(".carousel-item.active");
            element.css("transform", "scale(" + zoom + ")");

            element.draggable();
        });

        $("#image-container").on("click", ".zoomOut", function () {

            if (zoom.toFixed(2) == 0.20) return;

            zoom -= 0.2;

            let element = $("#image-container").find(".carousel-item.active");
            element.css("transform", "scale(" + zoom + ")");
        });

        $("#image-container").on("click", ".thum-img-holder", function () {
            //
            var selImage = $(this).parent().find('.img-selector');
            $('.sel-file-size').html(selImage.attr('size'));
            $('.sel-file-date').html(selImage.attr('date'));
            $('.sel-file-name').html(selImage.attr('file-name'));

        });
    });
}

//function drawMap(imgdata) {
//    var image = new Image();
//    image.onload = function () {
//        ctx.drawImage(image, 0, 0);
//    };
//    image.src = imgdata;
//}
