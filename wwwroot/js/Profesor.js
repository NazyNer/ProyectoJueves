window.onload = function () {
    $.datepicker.regional['es'] = {
        closeText: 'Cerrar',
        prevText: '&#x3C;Ant',
        nextText: 'Sig&#x3E;',
        currentText: 'Hoy',
        monthNames: ['enero', 'febrero', 'marzo', 'abril', 'mayo', 'junio', 'julio', 'agosto', 'septiembre', 'octubre', 'noviembre', 'diciembre'],
        monthNamesShort: ['ene', 'feb', 'mar', 'abr', 'may', 'jun', 'jul', 'ago', 'sep', 'oct', 'nov', 'dic'],
        dayNames: ['domingo', 'lunes', 'martes', 'miércoles', 'jueves', 'viernes', 'sábado'],
        dayNamesShort: ['dom', 'lun', 'mar', 'mié', 'jue', 'vie', 'sáb'],
        dayNamesMin: ['D', 'L', 'M', 'X', 'J', 'V', 'S'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $("#Birthdate").datepicker({
        changeMonth: true,
        changeYear: true,
        showAnim: "fold",
        minDate: "-80Y",
        maxDate: "-18Y",
        beforeShow: function (input, inst) {
            inst.settings = $.extend(inst.settings, $.datepicker.regional['es']);
        }
    });
    SearchProfesores();
}
function FormatearFecha(fecha) {
    var partes = fecha.split("T")[0].split("-");
    var fechaFormateada = partes[2] + "/" + partes[1] + "/" + partes[0];
    return fechaFormateada;
}


function SearchProfesores() {
    let tablaProfesores = $("#tbody-Profesor");
    tablaProfesores.empty();
    $.ajax({
        url: '../../Profesor/SearchProfesores',
        data: {},
        type: 'GET',
        dataType: 'json',
        success: function (profesores) {
            tablaProfesores.empty();
            $.each(profesores, function (index, profesor) {
                var fechaFormateada = FormatearFecha(profesor.birthdate);
                tablaProfesores.append(`
              <tr>
                  <th scope="row">${profesor.dni}</th>
                  <td>${profesor.fullName}</td>
                  <td>${fechaFormateada}</td>
                  <td>
                   <button title="Editar Profesor" onClick="EditStudent(${profesor.profesorId})">✏</button>
                   <button title="Eliminar Profesor" onClick="DeleteProfesor(${profesor.profesorId})">🗑</button>
                  </td>
                  <td>${profesor.email}</td>
                  <td>${profesor.address}</td>
              </tr>
              `);
            })
        }
    })
}

function EditStudent(id) {
    $.ajax({
        url: '../../Profesor/SearchProfesores',
        data: { Id: id },
        type: 'GET',
        dataType: 'json',
        success: function (students) {
            var fechaFormateada = FormatearFecha(students[0].birthdate);
            console.log(students);
            if (students[0].profesorId == id) {
                ClearModal();
                $("#FullName").val(students[0].fullName);
                $("#Birthdate").val(fechaFormateada);
                $("#Address").val(students[0].address);
                $("#Dni").val(students[0].dni);
                $("#Id").val(students[0].profesorId);
                $("#Email").val(students[0].email);
                $("#staticBackdrop").modal("show");
            }
        }
    });
}

function ClearModal() {
    $("#FullName").val("");
    $("#Birthdate").val("");
    $("#Address").val("");
    $("#Email").val("");
    $("#Dni").val("");
    $("#Id").val("");
    $("#lbl-error").text("");
}

function SaveProfesor() {
    $("#lbl-error").text("");
    let FullName = $("#FullName").val();
    let Id = $("#Id").val();
    let Birthdate = $("#Birthdate").val();
    let Address = $("#Address").val();
    let Dni = $("#Dni").val();
    let Email = $("#Email").val();
    if (Dni.length >= 7 && Dni.length <= 8) {
            // La entrada es válida
            console.log("llegue");
            $.ajax({
                url: '../../Profesor/SaveProfesor',
                type: 'POST',
                dataType: 'json',
                data: { Id: Id, FullName: FullName, Birthdate: Birthdate, Address: Address, Dni: Dni, Email: Email },
                async: false,
                success: function (resultado) {
                    console.log(resultado);
                    if (resultado.NonError) {
                        $("#staticBackdrop").modal("hide");
                        SearchProfesores();
                    }
                    else {
                        console.log(resultado);
                        $("#lbl-error").text(resultado.MsjError);
                    }
                },
            });
    }else{
        $("#lbl-error").text("El DNI debe tener entre 7 y 8 caracteres.");
    }
}


function DeleteProfesor(id) {
    $.ajax({
        url: '../../Profesor/DeleteProfesor',
        data: { Id: id },
        type: 'GET',
        dataType: 'json',
        success: function (resultado) {
            if (resultado.NonError) {
                SearchProfesores();
            } else {
                alert(resultado.MsjError);
            }
        }
    });
}


$("#FullName").on("input", function () {
    var input = $(this);
    var startPosition = input[0].selectionStart;  // Guardar la posición del cursor

    input.val(input.val().toUpperCase());  // Convertir texto a mayúsculas

    input[0].setSelectionRange(startPosition, startPosition);  // Restaurar la posición del cursor
});
$("#Address").on("input", function () {
    var input = $(this);
    var startPosition = input[0].selectionStart;  // Guardar la posición del cursor

    input.val(input.val().toUpperCase());  // Convertir texto a mayúsculas

    input[0].setSelectionRange(startPosition, startPosition);  // Restaurar la posición del cursor
});
$("#Email").on("input", function () {
    var input = $(this);
    var startPosition = input[0].selectionStart;  // Guardar la posición del cursor

    input.val(input.val().toLowerCase());  // Convertir texto a mayúsculas

    input[0].setSelectionRange(startPosition, startPosition);  // Restaurar la posición del cursor
});
