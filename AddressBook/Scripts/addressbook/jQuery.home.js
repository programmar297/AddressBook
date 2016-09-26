//function: createModal(id), id: optional
//Creates the modal dialog for Adding and editing
//If called without id, then it will create an empty modal dialog
//If called with id, then it will create a modal dialog pre-filled with record info
function createModal(id) {
    var d = new Date();
    var n = d.getTime();
    id = typeof id !== 'undefined' ? id : 0;
    var url = '';

    if (parseInt(id) < 1) {
        url = '/Home/Add/?ts=' + n;
    }
    else {
        url = '/Home/Edit/' + id + '/?ts=' + n;
    }
    $.get(url, function (data) {
        $('#modal_container').html(data);
        $('#dlgAddress').modal('show');
        $("#btnDelete").unbind("click");
        $("#btnDelete").click(function () {
            var recId = $(this).data("id");
            showConfirm("Confirmation", "Are you sure you want to delete this record?", function (result) {
                console.log(result);
                if (result == true) {
                    deleteRecord(recId);
                }
            });
            return false;
        });
        $("#btnSave").click(function (event) {
            if (checkRequired()) {
                var d = new Date();
                var n = d.getTime();
                $(this).toggleClass('active');
                $.post("/Home/CheckDuplicate", {
                    id: id, email: $('#txtEmail').val(), ts: n
                }, function (data) {
                    if (data.Success == true) {
                        $('#btnSave').removeClass('active');
                        var cnfmsg = "Proceed to edit this record?";
                        if (parseInt(id) < 1) {
                            cnfmsg = "Proceed to add new address?";
                        }

                        showConfirm("Confirmation required", cnfmsg, function (result) {
                            if (result) {
                                $("#frmAddress").submit();
                            }

                        });
                    }
                    else {
                        hookEdit(".editBtn");
                        hookDelete(".deltBtn");
                        showAlert("Error occured", $('#txtEmail').val() + " already exists in the database, please add a new e-mail address", true);
                        $('#btnSave').removeClass('active');
                        return false;
                    }
                });
            }
        });


    });

}
//function: checkRequired
//To Check the required fields before submitting the form and displaying appropriate messages.
function checkRequired() {
    var mes = '';
    var emailRegEx = /^([\w\.\-_]+)?\w+@[\w-_]+(\.\w+){1,}$/;
    if ($("#txtName").val() == '') mes += "Please input a name<br />";   
    if ($("#txtEmail").val() == '') {
        mes += "Please input an Email address<br />";
    }
    else {
        if(!$("#txtEmail").val().match(emailRegEx)){
            mes += "Please input a valid Email address<br />";
        }
    }

    if (mes === "") {
        return true;
    } else {
        hookEdit(".editBtn");
        hookDelete(".deltBtn");
        showAlert("Error", mes, true);
        return false;
    }
}
//function: loadTable
//Loads the list of records via ajax when searched
function loadTable() {
    var d = new Date();
    var n = d.getTime();
    var searchVal = $("#txtSearch").val();
    $("#hidSearch").val(searchVal);
    $.get("/Home/Search", { search: searchVal, page: $('#hidPageNum').val(), size: $('#hidPageSize').val(), ts: n }, function (returnHtml) {
        $("#listView").empty();
        $("#listView").append(returnHtml);
        hookEdit(".editBtn");
        hookDelete(".deltBtn");
        hookPaging();
        $(".loader").hide();
        $('.btnMap').unbind('click');
        $('.btnMap').click(function () {
            var address = $(this).text();
            getAddress(address);
            createMapModal();
        });
    });
}
//function: onPageChange
//Paging buttons are linked with this function to change the pages
function onPageChange() {
    hookPaging();
    hookEdit(".editBtn");
    hookDelete(".deltBtn");
}
//function: hookEdit
//Edit button hook on DOM refresh
function hookEdit(control) {
    $(control).unbind("click");
    $(control).click(function () {
        var id = $(this).data("id");
        if (id !== 'undefined') {
            createModal(id);
        }
    });
}
//function: hookEdit
//Delete button hook on DOM refresh
function hookDelete(control) {
    $(control).unbind("click");
    $(control).click(function () {
        var recId = $(this).data("id");
        showConfirm("Confirmation", "Are you sure you want to delete this record?", function (result) {
            console.log(result);
            if (result == true) {
                deleteRecord(recId);
            }
        });
        return false;
    });
}
//function: hookPaging
//Controls the record display for number or record buttons
function hookPaging() {
    $('.record-control').click(function () {
        var numRec = $(this).attr("data-rec");
        $('#hidPageSize').val(numRec);
        $('#hidPageNum').val('1');
        loadTable();
    }); 
}
//function: submitAddress(data) : data is the JSON data returned from the server
// fires after the record has been process for submission. 
function submitAddress(data) {
    $('#dlgAddress').modal('hide');
    if (data.Success == true) {
        loadTable();
        var message = '';
        if (data.Mode == 'edit') {
            message = "Address record edited successfully";
        }
        else if (data.Mode == 'insert') {
            message = "New address record added to address book";
        }

        hookEdit(".editBtn");
        hookDelete(".deltBtn");
        showAlert("Success", message, false);

    } else {
        hookEdit(".editBtn");
        hookDelete(".deltBtn");
        showAlert("Error", data.Message, true);
    }
}
//functoin: deleteRecord(id).
//initiates the process of record deletion
function deleteRecord(id) {
    var d = new Date();
    var n = d.getTime();
    id = typeof id !== 'undefined' ? id : -1;
    if (parseInt(id) > 0) {
        $.post("/Home/Delete", { id: id, ts: n }, function (data) {
            if (data.Success == true) {
                $('#dlgAddress').modal('hide');
                loadTable();
            }
            else {
                showAlert("Error", data.Error , true);
            }
        });
    }
}
//function createMapModal
// Creates a modal dialog for Map to be shown in a seperate dialog
function createMapModal() {
    $('#dlgMap').modal('show');
}

//------------------------------------MAP FUNCTIONS-------------------------------//

var geocoder; //Google Geo Coder
var map;      //Google Map object

//Initialize map
function loadMap() {
    geocoder = new google.maps.Geocoder();
    var latlng = new google.maps.LatLng(48.061496, 16.360000);
    var mapOptions = {
        zoom: 10,
        center: latlng
    }
    map = new google.maps.Map(document.getElementById('map'), mapOptions);
}
//function: getAddress(addr): addr is the address to be geo-coded
//This functions get the address from the application and gets the location through google's geo-coder
function getAddress(addr) {
    geocoder.geocode({ 'address': addr }, function (results, status) {
        if (status == 'OK') {
            map.setCenter(results[0].geometry.location);
            var marker = new google.maps.Marker({
                map: map,
                position: results[0].geometry.location
            });
            map.setZoom(13);
        } else {
            showAlert('Google Maps Error', 'Geocode was not successful for the following reason: ' + status, true);
        }
    });
    if (typeof map == "undefined") return;
    setTimeout(function () { resizingMap(); }, 400);
}
//function: resizing map
function resizingMap() {
    if (typeof map == "undefined") return;
    var center = map.getCenter();
    google.maps.event.trigger(map, "resize");
    map.setCenter(center);
}

//Document Loading functions
$(function () {
    $('.navigation').removeClass('active');
    $('#lnkMain').addClass('active');
    var showError = $('#hidshowError').val();
    var msg = $('#hidshowMsg').val();
    if (msg != '') {
        if (showError == 'true')
            showAlert("Error", msg, true);
        else
            showAlert(window.lang.MESSAGE_LABEL, msg, false);
    }

    $("#btnAdd").click(function () {
        createModal();
    });


    hookPaging();
    hookEdit(".editBtn");
    hookDelete(".deltBtn");

    $("#txtSearch").keyup(function () {
        $(".loader").show();
        loadTable();
        return false;
    });

    $('.btnMap').click(function () {
        var address = $(this).text();
        getAddress(address);
        createMapModal();
    });

});