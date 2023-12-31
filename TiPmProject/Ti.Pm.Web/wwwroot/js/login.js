localStorage.clear();

var UserLoginValidation = function () {
    var formData = new FormData();
    var formcontroldata = $("#formlogin").serializeArray();
    $.each(formcontroldata, function (i, field) {
        formData.append(field.name, field.value);
    });
    $("#btnLogin").css("pointer-events", "none").val("");
    $("#loader").css("visibility", "visible");
    $.ajax({
        url: $("#formlogin").attr('action'),
        type: $("#formlogin").attr('method'),
        data: formData,
        contentType: false,
        processData: false,
        dataType: 'json',
        cache: false,
        success: function (result) {
            if (result.status == 0) {
                AddValidAnimation("Не удалось войти в систему. Пожалуйста, проверьте свои данные для входа.");
                $("#btnLogin").css("pointer-events", "auto").val("Anmelden");
                $("#loader").css("visibility", "hidden");
            } else if (result.status == 2) {
                AddValidAnimation("У вас нет разрешения на доступ к этой системе.");
                $("#btnLogin").css("pointer-events", "auto").val("Anmelden");
                $("#loader").css("visibility", "hidden");
            }
            else {
                window.location.href = window.location.origin;
            }
        }
    });
};

function AddValidAnimation(validatetionText) {
    $('#divmessage').addClass('fadeIn').html(validatetionText);
    $("#password, #login").addClass("invalid-shake invalid-border");
    $("#password, #login").one('webkitAnimationEnd oanimationend msAnimationEnd animationend', function (event) {
        $(this).removeClass('invalid-shake');
    });
}

window.addEventListener('keypress', function (e) {
    if (e.which === 13) {
        $("#btnLogin").click();
    }
});