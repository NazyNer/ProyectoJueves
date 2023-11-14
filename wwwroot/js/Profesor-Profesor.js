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
  let btnCrearProfesor = $("#btnCrearProfesor");
  btnCrearProfesor.addClass("displayHidden");
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
      tablaProfesores.empty();
      $.each(profesores, function (index, profesor) {
        var fechaFormateada = FormatearFecha(profesor.birthdate);
        tablaProfesores.append(`
            <tr>
                <th scope="row">${profesor.dni}</th>
                <td>${profesor.fullName}</td>
                <td>${fechaFormateada}</td>
                <td>
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
  let thBoton = $("#thAsignatura");
  thBoton.empty();
  thBoton.append(`Crear Tareas <button class="btn btn-danger " onclick="CerrarTablaAsignatura()">Cerrar</button>`)
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
                      <tr class="table-dark">
                          <td>${asignatura.nombre}</td>
                          <td><div class="toggle-rect-color"><button onclick="Agregartarea(${asignatura.asignaturaId}, ${ProfesorId})">📋Crear tareas</button></div></td>
                      </tr>
                  `)
          }
        })
      } else {
        tablaAsignaturas.append(`
              <td><p>No hay asignaturas creadas</p></td>
              `)
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


function Agregartarea(asignatura, Profesor) {
  var Seccion = $("#TablaCrearTarea");
  Seccion.empty();

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

      if (respuesta.nonError) {
        window.location.reload();
      } else {
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

    $.ajax({
      url: '../../Profesor/SaveProfesor',
      type: 'POST',
      dataType: 'json',
      data: { Id: Id, FullName: FullName, Birthdate: Birthdate, Address: Address, Dni: Dni, Email: Email },
      async: false,
      success: function (resultado) {

        if (resultado.NonError) {
          $("#staticBackdrop").modal("hide");
          SearchProfesores();
        }
        else {

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
