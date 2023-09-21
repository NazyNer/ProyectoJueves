window.onload = function() {
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
    minDate: "-70Y",
    maxDate: "-12Y",
    beforeShow: function(input, inst) {
      inst.settings = $.extend(inst.settings, $.datepicker.regional['es']);
    }
  });
  SearchStudents();
}

function SearchStudents() {
  let tablaAlumnos = $("#tbody-Alumno");
  tablaAlumnos.empty();
  $.ajax({
    url: '../../Alumno/SearchStudents',
    data: {},
    type: 'GET',
    dataType: 'json',
    success: function (students) {
      tablaAlumnos.empty();
      $.each(students, function (index, student) {
        var fechaFormateada = FormatearFecha(student.birthdate);
        if (student.isActive) {
          tablaAlumnos.append(`
            <tr class="table-success">
                <th scope="row">${student.id}</th>
                <td>${student.fullName}</td>
                <td>${fechaFormateada}</td>
                <td>
                  <button title="Desactivar Alumno" onclick="ActiOrDesacStudent(${student.id})">❌</button>
                  <button title="Editar Alumno" onclick="EditStudent(${student.id})">✏</button>
                </td>
                <td>${student.carreraName}</td>
            </tr>
            `);
        }else{
          tablaAlumnos.append(`
            <tr class="table-warning">
                <th scope="row">${student.id}</th>
                <td>${student.fullName} (Desactivado)</td>
                <td>${fechaFormateada}</td>
                <td>
                  <button title="Activar Alumno" onclick="ActiOrDesacStudent(${student.id})">✔</button>
                  <button title="Eliminar Alumno" onclick="DeleteStudent(${student.id})">🗑</button>
                </td>
                <td>${student.carreraName}</td>
            </tr>
            `);
        }
      })
  }
  })
}
function FormatearFecha(fecha) {
  var partes = fecha.split("T")[0].split("-");
  var fechaFormateada = partes[2] + "-" + partes[1] + "-" + partes[0];
  return fechaFormateada;
}

function ClearModal() {
  $("#FullName").val("");
  $("#Birthdate").val("");
  $("#CarreraID").val(0);
  $("#Id").val("");
  $("#lbl-error").text("");
}

function SaveStudent() {
  $("#lbl-error").text("");
  let FullName = $("#FullName").val();
  let Id = $("#Id").val();
  let Birthdate = $("#Birthdate").val();
  let CarreraID = $("#CarreraID").val();
  $.ajax({
    url: '../../Alumno/GuardarAlumno',
    type: 'POST',
    dataType: 'json',
    data: {Id: Id, FullName: FullName, Birthdate: Birthdate, CarreraID: CarreraID},
    async: false,
    success: function (resultado) {
      console.log(resultado);
      if (resultado.NonError) {
        $("#staticBackdrop").modal("hide");
        SearchStudents();
      }
      else {
        $("#lbl-error").text(resultado.MsjError);
      }
    },
  });
}

function EditStudent(id) {
  $.ajax({
    url: '../../Alumno/SearchStudents',
    data: {Id: id},
    type: 'GET',
    dataType: 'json',
    success: function (students) {
      console.log(students);
      var fechaFormateada = FormatearFecha(students[0].birthdate);
        if (students[0].id == id) {
          ClearModal();
          $("#FullName").val(students[0].fullName);
          $("#Birthdate").val(fechaFormateada);
          $("#CarreraID").val(students[0].carreraId);
          $("#Id").val(students[0].id);
          $("#staticBackdrop").modal("show");
        }
    }});
}

function DeleteStudent(id) {
  $.ajax({
    url: '../../Alumno/DeleteAlumno',
    data: {Id: id},
    type: 'GET',
    dataType: 'json',
    success: function (resultado) {
      if (resultado.NonError) {
        SearchStudents()
      }else{
        alert(resultado.MsjError);
      }
    }});
}

function ActiOrDesacStudent(studentId){
  if (studentId > 0) {
    $.ajax({
      url: '../../Alumno/ActivarDesactivarAlumno',
      data: {
        id: studentId
      },
      type: 'POST',
      dataType: 'json',
      success: function (resultado) {
        if (resultado.NonError) {
          SearchStudents();
        }else{
          alert(resultado.MsjError);
        }
      }
    });
  }else{
    alert("No se ah seleccionado un alumno");
  }
}





$("#FullName").on("input", function () {
  var input = $(this);
  var startPosition = input[0].selectionStart;  // Guardar la posición del cursor

  input.val(input.val().toUpperCase());  // Convertir texto a mayúsculas

  input[0].setSelectionRange(startPosition, startPosition);  // Restaurar la posición del cursor
});
