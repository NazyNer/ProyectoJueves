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
    let tablaAsignaturas = $("#AsignaturasTable");
    tablaAsignaturas.addClass("displayHidden");
    let tablaProfesores = $("#tbody-Profesor");
    tablaProfesores.empty();
    $.ajax({
        url: '../../Profesor/SearchProfesores',
        data: {},
        type: 'GET',
        dataType: 'json',
        success: function (profesores) {
            console.log(profesores);
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
                   <button title="Ver Asignaturas" onClick="Asignaturas(${profesor.profesorId})">📚</button>
                  </td>
                  <td>${profesor.email}</td>
                  <td>${profesor.address}</td>
              </tr>
              `);
            })
        }
    })
}

function Asignaturas(ProfesorId) {
    let tabla = $("#AsignaturasTable");
    let tablaAsignaturas = $("#tbody-AsignaturasProfesor")
    let profesor = $("#ProfesorAsignatura");
    tablaAsignaturas.empty();
    $.ajax({
        url: '../../Profesor/Asignaturas',
        data: { Id: ProfesorId },
        type: 'POST',
        dataType: 'json',
        success: function (devolucion) {
            tabla.removeClass("displayHidden");
            if (devolucion.asignaturas.length > 0) {
                profesor.val(ProfesorId);
                $.each(devolucion.asignaturas, function (index, asignatura) {
                    if (devolucion.asignaturasRelacionadas[`${asignatura.asignaturaId}`] != undefined) {
                        tablaAsignaturas.append(`
                        <tr class="table-success">
                            <td>${asignatura.nombre} <button onclick="Agregartarea(${asignatura.asignaturaId}, ${ProfesorId})">📋Crear tareas</button></td>
                            <td><div class="toggle-rect-color"><input type="checkbox"id="${asignatura.asignaturaId}" name="asignaturas" value="${asignatura.asignaturaId}" checked><label for="${asignatura.asignaturaId}"></label></div></td>
                        </tr>
                    `)
                    } else {
                        tablaAsignaturas.append(`
                            <tr class"table-warning">
                                <td>${asignatura.nombre}</td>
                                <td><div class="toggle-rect-color"><input value="${asignatura.asignaturaId}" type="checkbox" id="${asignatura.asignaturaId}" name="asignaturas"><label for="${asignatura.asignaturaId}"></label></div></td>
                            </tr>
                        `)
                    }
                })
            } else {
                tablaAsignaturas.append(`
                <td><p>No hay asignaturas creadas</p></td>
                <td><p>Por favor ve a crear nuevas y volver a intentar</p></td>
                `)
            }
        }
    });
}
{/* <input type="checkbox" name="asignaturas" value="${asignatura.asignaturaId}"></input>    */ }
function GuardarAsignaturas() {
    let tabla = $("#AsignaturasTable");
    let tablaAsignaturas = $("#tbody-AsignaturasProfesor")
    let valoresCheck = [];
    $("input[type=checkbox]:checked").each(function () {
        valoresCheck.push(this.value);
    });
    let IdProfesor = $("#ProfesorAsignatura").val();
    $.ajax({
        url: '../../Profesor/GuardarAsignaturas',
        data: { AsignaturasJs: valoresCheck, ProfesorId: IdProfesor },
        type: 'POST',
        dataType: 'json',
        success: function (resultado) {
            console.log(resultado);
            if (resultado.NonError) {
                tabla.addClass("displayHidden");
                tablaAsignaturas.empty();
            } else {
                tablaAsignaturas.empty();
                tabla.addClass("displayHidden");
                alert(resultado.mensaje);
            }
        }
    });
}
function CerrarTablaAsignatura() {
    let tabla = $("#AsignaturasTable");
    let tablaAsignaturas = $("#tbody-AsignaturasProfesor")
    tabla.addClass("displayHidden");
    tablaAsignaturas.empty();
}

function EditStudent(id) {
    $.ajax({
        url: '../../Profesor/SearchProfesores',
        data: { Id: id },
        type: 'POST',
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

function Agregartarea(asignatura, Profesor) {
    var Seccion = $("#TablaCrearTarea");
    Seccion.empty();
    console.log(asignatura, Profesor);
    Seccion.append(`
    <h1>Crear Tarea</h1>
    
    <form id="crearTareaForm" onsubmit="return false">
    <div class="mb-3">
        <label for="titulo" class="form-label">Título:</label>
        <input type="text" class="form-control" id="titulo" name="Titulo" required>
    </div>
    <div class="mb-3">
        <label for="descripcion" class="form-label">Descripción:</label>
        <textarea id="descripcion" class="form-control" name="Descripcion" rows="4" cols="50" required></textarea>
    <div class="mb-3">
        <label for="fechaCarga" class="form-label">Fecha de Carga:</label>
        <input type="date" id="Birthdate" class="form-control" name="FechaDeCarga" required>
    </div>
    <div class="mb-3">
        <label for="fechaVencimiento" class="form-label">Fecha de Vencimiento:</label>
        <input type="date" id="Birthdate" class="form-control" name="FechaDeVencimiento" required>
    </div>
    <input type="number" id="profesorID" class="form-control" name="ProfesorID" value="${Profesor}" disabled>
    <input type="number" id="AsignaturaID" class="form-control" name="AsignaturaID" value="${asignatura}" disabled>
    </form>
    <button class="btn btn-success"onclick="GuardarTarea()">Guardar Tarea</button>
`);
}

function GuardarTarea() {
    var titulo = $("#titulo").val();
    var descripcion = $("#descripcion").val();
    var fechaCarga = $("input[name='FechaDeCarga']").val();
    var fechaVencimiento = $("input[name='FechaDeVencimiento']").val();
    var profesorID = $("#profesorID").val();
    var AsignaturaID = $("#AsignaturaID").val();
    console.log(
        titulo,
        descripcion,
        fechaCarga,
        fechaVencimiento,
        profesorID,
        AsignaturaID);
    $.ajax({
        url: "../../Tarea/GuardarTarea",
        data: {
            titulo: titulo,
            descripcion: descripcion,
        FechaCarga: fechaCarga,
        FechaVencimiento: fechaVencimiento,
        profesorID: profesorID,
        asignaturaID: AsignaturaID
        },
        type: "POST",
        dataType: 'json',
        success: function (respuesta) {
            console.log(respuesta);
            if(respuesta.nonError){
                window.location.reload();
            }else{
                alert(respuesta.mensaje);
            }
        },
        error: function (error) {
            console.error("Error al crear la tarea", error);
        }
    });
};

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
    } else {
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
