$("#contactForm").validator().on("submit", function (event) {
    if (event.isDefaultPrevented()) {
        // handle the invalid form...
        formError();
        submitMSG(false, "Llene todos los campos");
    } else {
        // everything looks good!
        event.preventDefault();
        submitForm();
    }
});



/* Validacion de formulario de logeo*/
$("#LoginForm").validator().on("submit", function (event) {
    if (event.isDefaultPrevented()) {
        // handle the invalid form...
        formErrorLogin();
        submitMSGLogin(false, "Llene todos los campos para Iniciar Sesion");
    } else {
        // everything looks good!
        event.preventDefault();
        submitFormLogin();
    }
});



/*Valicacion de formulario de registro*/
$("#RegistrarmeForm").validator().on("submit", function (event) {
    if (event.isDefaultPrevented()) {
        // handle the invalid form...
        formErrorRegistrarte();
        submitMSGRegistrate(false, "Llene todos los campos para registrarte");
    } else {
        // everything looks good!
        event.preventDefault();
        alert('Se Guardo Existosamente');
    }
});



/*Validacion de formulario de recuperacion de contraseņa*/
$("#RecuperarEmailForm").validator().on("submit", function (event) {
    if (event.isDefaultPrevented()) {
        // handle the invalid form...
        formErrorRecuperar();
        submitMSGRecuperarEmail(false, "Escriba tu correo para Recuperar tu cuenta de electronico");
    } else {
        // everything looks good!
        event.preventDefault();
        submitFormRecuperar();
    }
});



/*Validacion de formulario de cambiar contraseņa*/
$("#CambiarClaveForm").validator().on("submit", function (event) {
    
    if (event.isDefaultPrevented()) {
        // handle the invalid form...
        formErrorCambiarClave();
        submitMSGCambiarClave(false, "Llene todos los campos de las contrase\361a");


    } else if ($("#txtContraCambiar").val().length < 6) {

        event.preventDefault();
        formErrorCambiarClave();
        submitMSGCambiarClave(false, "La contrase\361a debe ser igual o mayor a 6 digitos");


    } else if ($("#txtContraCambiarConfir").val().length < 6) {

        event.preventDefault();
        formErrorCambiarClave();
        submitMSGCambiarClave(false, "La confirmacion contrase\361a debe ser igual o mayor a 6 digitos");

    

    } else if ($("#txtContraCambiar").val() != $("#txtContraCambiarConfir").val()) {

        event.preventDefault();
        formErrorCambiarClave();
        submitMSGCambiarClave(false, "Las contrase\361as tienen que ser iguales");
    
    } else {
        event.preventDefault();
        // everything looks good!
        
        submitFormCambiarClave();
    }
});



function submitForm() {
    // Initiate Variables With Form Content
    var name = $("#name").val();
    var email = $("#email").val();
    var msg_subject = $("#msg_subject").val();
    var message = $("#message").val();


    $.ajax({
        type: "POST",
        url: "php/form-process.php",
        data: "name=" + name + "&email=" + email + "&msg_subject=" + msg_subject + "&message=" + message,
        success: function (text) {
            if (text == "success") {
                formSuccess();
            } else {
                formError();
                submitMSG(false, text);
            }
        }
    });
}


//Validar enviar a controlador 
function submitFormLogin() {
    // Initiate Variables With Form Content
    var Correo = $("#txtCorreoLogin").val();
    var Clave = $("#txtPassworkLogin").val();
    var recordarme = $("#txtRecuerdame").val();

    var login = { Correo: Correo, Clave: Clave, recordarme: recordarme };

    var url = $("#LoginForm").attr("action");

    $.ajax({
        type: "POST",
        url: url,
        data: login,
        success: function (text) {
            if (text == "1") {
                formSuccessLogin();
                var url2 = $(location).attr('href');
                if (url2 != "https://localhost:44360/") {
                    $(location).attr('href', 'Inicio');
                } else {
                    location.reload();
                }
            } else if (text == "2") {
                formSuccessLogin();
                var url2 = $(location).attr('href');
                if (url2 != "https://localhost:44360/") {
                    $(location).attr('href', 'MandarAdmin');
                } else {
                    $(location).attr('href', 'Cliente/MandarAdmin');
                }
            } else {
                if (text == 3) {
                    formErrorLogin();
                    submitMSGLogin(false, "Credenciales incorrectas");
                } else if (text == 4) {
                    formErrorLogin();
                    submitMSGLogin(false, "Este correo no esta registrado");
                } else {
                    formErrorLogin();
                    submitMSGLogin(false, "Credenciales incorrectas");
                }
            }
        }
    });
}



/*Enviar datos a controlador Cliente metodo recuperar*/
function submitFormRecuperar() {
    // Initiate Variables With Form Content
    var Correo = $("#txtCorreoRecuperar").val();

    var url = $("#RecuperarEmailForm").attr("action");

    $.ajax({
        type: "POST",
        url: url,
        data: {
            Correo: Correo
        },
        success: function (text) {
            if (text == "1") {
               /* alert('Se le envio al correo una validacion');*/
                formSuccessRecuperar();
            } else {
                formErrorLogin();
                submitMSGRecuperarEmail(false, "Ese correo no esta en el sistema. intente con otro.");
            }
        }
    });
}




/*Enviar datos a controlador Cliente metodo CAmbiar clave*/
function submitFormCambiarClave() {
    // Initiate Variables With Form Content
    var Clave = $("#txtContraCambiar").val();
    var id = $("#txtIdCliente").val();

    var url = $("#CambiarClaveForm").attr("action");

    $.ajax({
        type: "POST",
        url: url,
        data: {
            contra: Clave, id: id
        },
        success: function (text) {
            if (text == "1") {
                
                formSuccessCambiarClave();
            } else {
                formErrorCambiarClave();
                submitMSGCambiarClave(false, "no se pudieron actualizar.");
            }
        }
    });
}



function formSuccess() {
    $("#contactForm")[0].reset();
    submitMSG(true, "")
}


/*Existoso para Recuperar cuenta*/
function formSuccessRecuperar() {
    $("#RecuperarEmailForm")[0].reset();
    submitMSGRecuperarEmail(true, "Se envi\363 al correo una validacion para cambiar si contrase\361a!")
}



/*Existoso para cambiar contraseņa*/
function formSuccessCambiarClave() {
    $("#CambiarClaveForm")[0].reset();
    submitMSGCambiarClave(true, "Se envi\363 al correo una validacion para cambiar si contrase\361a!");
    alert('Contrase&ntilde;a actualzada');
    $(location).attr('href','inicio');
}




/*Existoso para login*/
function formSuccessLogin() {
    $("#LoginForm")[0].reset();
    submitMSGLogin(true, "")
}




function formError() {
    $("#contactForm").removeClass().addClass('shake animated').one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
        $(this).removeClass();
    });
}


/*Error de login*/
function formErrorLogin() {
    $("#LoginForm").removeClass().addClass('shake animated').one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
        $(this).removeClass();
    });
}


/*Error de Registrarte*/
function formErrorRegistrarte() {
    $("#RegistrarmeForm").removeClass().addClass('shake animated').one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
        $(this).removeClass();
    });
}

/*Error de recueprar contraseņa*/
function formErrorRecuperar() {
    $("#RecuperarEmailForm").removeClass().addClass('shake animated').one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
        $(this).removeClass();
    });
}


/*Error de recueprar Cambiar clave*/
function formErrorCambiarClave() {
    $("#CambiarClaveForm").removeClass().addClass('shake animated').one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
        $(this).removeClass();
    });
}


/*Mesaje de error de */
function submitMSG(valid, msg) {
    if (valid) {
        var msgClasses = "h3 text-center tada animated text-success";
    } else {
        var msgClasses = "h3 text-center text-danger";
    }
    $("#msgSubmit").removeClass().addClass(msgClasses).text(msg);
}


/*Mesaje de error de Login*/
function submitMSGLogin(valid, msg) {
    if (valid) {
        var msgClasses = "h3 text-center tada animated text-success";
    } else {
        var msgClasses = "h3 text-center text-danger";
    }
    $("#msgSubmitLogin").removeClass().addClass(msgClasses).text(msg);
}


/*Mesaje de error de Registrarte */
function submitMSGRegistrate(valid, msg) {
    if (valid) {
        var msgClasses = "h3 text-center tada animated text-success";
    } else {
        var msgClasses = "h3 text-center text-danger";
    }
    $("#msgSubmitRegistrate").removeClass().addClass(msgClasses).text(msg);
}


/*Mesaje de error de Recuperar Email*/
function submitMSGRecuperarEmail(valid, msg) {
    if (valid) {
        var msgClasses = "h3 text-center tada animated text-success";
    } else {
        var msgClasses = "h3 text-center text-danger";
    }
    $("#msgSubmitREcuperarEmail").removeClass().addClass(msgClasses).text(msg);
}


/*Mesaje de error al cambiar clave*/
function submitMSGCambiarClave(valid, msg) {
    if (valid) {
        var msgClasses = "h3 text-center tada animated text-success";
    } else {
        var msgClasses = "h3 text-center text-danger";
    }
    $("#msgSubmitCambiarClaveForm").removeClass().addClass(msgClasses).text(msg);
}