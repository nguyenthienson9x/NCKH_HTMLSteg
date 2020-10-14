$(document).ready(function () {
    user = new User();
});

class User {
    constructor() {
        this.initEvents();
    }

    initEvents() {
        $(".btn-add-item").on("click", { "action": "Add", "userInstance": this }, this.openDialog);
        $(".btn-edit-item").on("click", { "action": "Edit", "userInstance": this }, this.openDialog);
        $(".btn-delete-item").on("click", { "userInstance": this }, this.deleteUser);

        $("#btn-add-user").on("click", this.addNewUser);
        $("#btn-update-user").on("click", this.updateUser);

        $(".chk-choose-file-xml").on("click", this.toggleChooseXML);
        $(".chk-input-html").on("click", this.toggleInputHTML);
    }

    //Mở dialog để thêm hoặc update
    //Xử lý title dialiog và thêm/update ở trong hàm này
    //Dựa vào cái target
    openDialog(e) {
        if (e.data.action == "Add") {
            $("h4.modal-title").html("Thêm người dùng!");
            $("#btn-add-user").removeClass("hide");
            $("#btn-update-user").addClass("hide");
            e.data.userInstance.clearDataDialog();
        }
        else {
            var ID = $(this).parent().parent().attr("id");
            $("h4.modal-title").html("Cập nhật TT người dùng!");
            $("#btn-add-user").addClass("hide");
            $("#btn-update-user").removeClass("hide");
            e.data.userInstance.loadDataToDialog(ID);
            //Load dl vào dialog
        }
        $("#user-dialog").modal("show");
    }


    clearDataDialog() {
        $(".txt-full-name").val("");
        $(".txt-username").val("");
        $(".txt-password").val("");
        $(".txt-message").val("");
    }

    loadDataToDialog(ID) {
        $.ajax({
            url: "/Admin/User/GetByID",
            method: "POST",
            data: { "id": ID },
            success: function (data) {
                if (data == null) {

                }
                else {
                    $(".txt-id").val(data.ID);
                    $(".txt-full-name").val(data.FullName);
                    $(".txt-username").val(data.Username);
                    $(".txt-password").val(data.Password);
                    $(".txt-message").val(data.Message);
                    $(".ddl-status").val(data.Status)
                }
            }
        })
    }

    addNewUser() {
        var fullName = $(".txt-full-name").val();
        var username = $(".txt-username").val();
        var password = $(".txt-password").val();
        var message = $(".txt-message").val();
        var status = $(".ddl-status").val();
        /*var fullName = $(".txt-full-name").val();
        var fullName = $(".txt-full-name").val();*/

        //var user = { FullName: fullName, Username: username, Password: password, Message: message, Status: status };
        var formData = new FormData();
        formData.append("ID", id);
        formData.append("FullName", fullName);
        formData.append("Username", username);
        formData.append("Password", password);
        formData.append("Message", message);
        formData.append("Status", status);
        formData.append("XMLKeyID", xml);
        formData.append("ReturnPageID", html);

        $.ajax({
            xhrFields: {
                withCredentials: true
            },
            url: "/Admin/User/AddUser",
            method: "POST",
            processData: false,  // tell jQuery not to process the data
            contentType: false,
            //data: user,
            data: formData,
            success: function (data) {
                if (data.Result == "OK") {
                    alert("Thêm thành công!");
                    var firstRow = $("table tbody tr:nth-child(1)");
                    var myRow = firstRow.clone();
                    myRow.insertBefore(firstRow);
                    myRow = $("table tbody tr:nth-child(1)");
                    myRow.attr("id", data.ID);

                    /*$("table tr:nth-child(1)").attr("id", data.ID);*/
                    $("table tr:nth-child(1) td:nth-child(2)").html(fullName);
                    $("table tr:nth-child(1) td:nth-child(3)").html(username);
                    $("table tr:nth-child(1) td:nth-child(4)").html(message);
                    $("table tr:nth-child(1) td:nth-child(5)").html(status == "Active" ? "Hoạt động" : "Không hoạt đông");

                    //Đóng dialog
                    $("#user-dialog").modal("hide");
                }
                else {
                    alert("Thao tác thất bại!");
                }
            }
        });
    }

    updateUser() {
        var id = $(".txt-id").val();
        var fullName = $(".txt-full-name").val();
        var username = $(".txt-username").val();
        var password = $(".txt-password").val();
        var message = $(".txt-message").val();
        var status = $(".ddl-status").val();
        var xml = $(".ddl-xml").val();
        var html = $(".ddl-html").val();
        /*var fullName = $(".txt-full-name").val();
        var fullName = $(".txt-full-name").val();*/

        //var user = { ID: id, FullName: fullName, Username: username, Password: password, Message: message, Status: status, XMLKeyID: xml, ReturnPageID: html };
        var formData = new FormData();
        formData.append("ID", id);
        formData.append("FullName", fullName);
        formData.append("Username", username);
        formData.append("Password", password);
        formData.append("Message", message);
        formData.append("Status", status);
        formData.append("XMLKeyID", xml);
        if ($("#inp-file-XML")[0].files.length > 0) {
            formData.append("XMLFile", $("#inp-file-XML")[0].files[0]);
        }
        formData.append("ReturnPageID", html);
        if ($(".chk-input-html").is(":checked")) {
            formData.append("HTMLContent", $("#inp-HTML").val());
        }

        $.ajax({
            xhrFields: {
                withCredentials: true
            },
            url: "/Admin/User/UpdateUser",
            method: "PUT",
            //data: user,
            data: formData,
            processData: false,  // tell jQuery not to process the data
            contentType: false,
            success: function (data) {
                if (data.Result == "OK") {
                    alert("Cập nhật thành công!");
                    //$(`tr #{id} td:nth-child(2)`).html(fullName);
                    $("tr#" + id + " td:nth-child(2)").html(fullName);
                    $("tr#" + id + " td:nth-child(3)").html(username);
                    $("tr#" + id + " td:nth-child(4)").html(message);
                    $("tr#" + id + " td:nth-child(5)").html(status == "Active" ? "Hoạt động" : "Không hoạt đông");
                    $("tr#" + id).attr("status", status);
                    $("#user-dialog").modal("hide");
                }
                else if (data.Result == "Not OK") {
                    alert("Độ dài msg k hợp lệ!");
                }
                else {
                    alert("Thao tác thất bại!");
                }
            }
        });
    }

    deleteUser() {
        var id = $(this).parent().parent().attr("id");
        var status = $(this).parent().parent().attr("status");
        if (status == "Active") {
            alert("Không thể xóa user có trạng thái đang hoạt động");
            return false;
        }
        else {
            if (confirm("Xóa?")) {
                $.ajax({
                    url: "/Admin/User/DeleteUser",
                    method: "Delete",
                    data: { "id": id },
                    success: function (data) {
                        if (data.Result == "OK") {
                            alert("Xóa thành công!");
                            $("tr#" + id).empty();
                        }
                        else {
                            alert("Thao tác thất bại!");
                        }
                    }
                })
            }
        }
    }

    toggleChooseXML() {
        if ($(".chk-choose-file-xml").is(":checked")) {
            $("#inp-file-XML").prop("disabled", false);
        } else {
            $("#inp-file-XML").prop("disabled", true);
        }
    }
    toggleInputHTML() {
        if ($(".chk-input-html").is(":checked")) {
            $("#inp-HTML").removeClass("hide");
        } else {
            $("#inp-HTML").addClass("hide");
        }
    }
}