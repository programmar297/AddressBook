//Document load functions
$(function () {
    $.ajaxSetup({ cache: false });
    $('#appAlert').on('hidden', function () {
        $(this).data('modal', null);
    });
});
//function showAlert
//Creates an alert box for all modules to be used.
//title: Dialog header title
//message: Message to be displayed
//error (true/false): Determines the color of the header on true being an error, which will make the dialog box appear red
//otherwise the dialog header will be displayed in blue
function showAlert(title, message, error) {
    error = typeof error !== 'undefined' ? error : true;
    title = typeof title !== 'undefined' ? title : "Error";
    
    message = typeof message !== 'undefined' ? message : "No message to show";

    var divbody = "<div>" + message + "</div>";
    var dtype = "dialog-primary";
    if (error)
        dtype = "dialog-danger";

    console.log(dtype);
    $('#appDialog').removeClass('dialog-danger').removeClass('dialog-primary').addClass(dtype);
    $('#appDialog-header').html(title);
    $('#appDialog-body').html(divbody);
    $('#appDialog-footer').html('<button id="btnDialogOK" class="btn btn-primary"> OK </button>');
    $("#btnDialogOK").click(function () {
        $('#appAlert').modal('hide');
        return false;
    });

    $('#appAlert').modal('show');
}
//function showConfirm
//Creates an alert box for all modules to be used.
//title: Dialog header title
//message: Message to be displayed
//callback: function to be executed upon selection of YES
function showConfirm(title, message, callback) {
    title = typeof title !== 'undefined' ? title : "Error";
    message = typeof message !== 'undefined' ? message : "No message to show";

    var divbody = "<div>" + message + "</div>";
    var dtype = "dialog-primary";

    $('#appConfirmDialog').addClass(dtype);
    $('#appConfirm-header').html(title);
    $('#appConfirm-body').html(divbody);

    $('#appConfirm-footer').html('<button id="btnConfirmOK" class="btn btn-primary"> YES </button>&nbsp;<button id="btnConfirmCancel" class="btn btn-default"> NO </button>');
    $("#btnConfirmOK").click(function () {
        $('#appConfirm').modal('hide');
        if (callback) callback(true);
    });
    $("#btnConfirmCancel").click(function () {
        $('#appConfirm').modal('hide');
        if (callback) callback(false);
    });

    $('#appConfirm').modal('show');
}
//String prototype functon format:
//makes the string formatable like String.Format functions
String.prototype.format = function () {
    var args = [].slice.call(arguments);
    return this.replace(/(\{\d+\})/g, function (a) {
        return args[+(a.substr(1, a.length - 2)) || 0];
    });
};