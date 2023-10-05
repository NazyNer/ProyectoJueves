window.onload = function () {
  let asignatura = $("#AsignaturaId").val();
  Tareas(asignatura);
};

function FormatearFecha(fecha) {
  var partes = fecha.split("T")[0].split("-");
  var fechaFormateada = partes[2] + "/" + partes[1] + "/" + partes[0];
  return fechaFormateada;
}

$('#AsignaturaId').change(function () {
  var selectedValue = $(this).val();
  Tareas(selectedValue);
});

function Tarea(id, asignatura) {
  let descripcionTarea = $("#DescripcionTarea");
  let tituloTarea = $("#Titulotarea #" + id);
  $("#Titulotarea a.active").removeClass("active");
  $.ajax({
    url: '/Tarea/ObtenerDatos',
    type: 'POST',
    data: { id: id , asignaturaId: asignatura },
    dataType: 'json',
    success: function (result) {
      let tarea = result.mensaje;
      tarea = tarea[0];
      let fecha = FormatearFecha(tarea.fechaDeVencimiento)
      tituloTarea.addClass("active")
      descripcionTarea.empty();
      descripcionTarea.append(`
              <div class="card">
                <div class="card-header">
                ${tarea.titulo}
                </div>
                <div class="card-body">
                  <blockquote class="blockquote mb-0">
                    <p>${tarea.descripcion}</p>
                    <footer class="blockquote-footer">vencimiento: <cite title="Source Title">${fecha}</cite></footer>
                  </blockquote>
                </div>
              </div>
            </div>
              `);
    }
  });
}

function Tareas(Asignatura) {
  let tituloTarea = $("#Titulotarea");
  let descripcionTarea = $("#DescripcionTarea");
  $.ajax({
    url: '/Tarea/ObtenerDatos',
    type: 'POST',
    data: { asignaturaId: Asignatura },
    dataType: 'json',
    success: function (result) {
      tituloTarea.empty();
      descripcionTarea.empty();
      if (result.nonError) {
        let count = 0;
        $.each(result.mensaje, function (index, tarea) {
          let fecha = FormatearFecha(tarea.fechaDeVencimiento)
          let hoy = new Date();
          hoy = "0" + hoy.getDate() + "/" + (hoy.getMonth()+1) + "/" + hoy.getFullYear();
          if (fecha < hoy) {
            tituloTarea.append(`
            <a class="list-group-item list-group-item-action disabled" aria-current="true">
              <div class="d-flex w-100 justify-content-between">
                <h5 class="mb-1">${tarea.titulo}</h5>
                <small class="text-warning">Vencido</small>
              </div>
              <p class="mb-1">vencimiento: ${fecha}</p>
            </a>
            `)
          }else{
            if (count == 0) {
              tituloTarea.append(`
              <a onclick="Tarea(${tarea.tareaId},${Asignatura})" id="${tarea.tareaId}" class="list-group-item list-group-item-action active" active aria-current="true">
                <div class="d-flex w-100 justify-content-between">
                  <h5 class="mb-1">${tarea.titulo}</h5>
                  <small class="text-success">Activo</small>
                </div>
                <p class="mb-1">vencimiento: ${fecha}</p>
              </a>
              `)
              descripcionTarea.append(`
              <div class="card">
                <div class="card-header">
                ${tarea.titulo}
                </div>
                <div class="card-body">
                  <blockquote class="blockquote mb-0">
                    <p>${tarea.descripcion}</p>
                    <footer class="blockquote-footer">vencimiento: <cite title="Source Title">${fecha}</cite></footer>
                  </blockquote>
                </div>
              </div>
            </div>
              `);
            }else{
              tituloTarea.append(`
              <a onclick="Tarea(${tarea.tareaId},${Asignatura})" id="${tarea.tareaId}" class="list-group-item list-group-item-action" active aria-current="true">
                <div class="d-flex w-100 justify-content-between">
                  <h5 class="mb-1">${tarea.titulo}</h5>
                  <small class="text-success">Activo</small>
                </div>
                <p class="mb-1">vencimiento: ${fecha}</p>
              </a>
              `)
            }
          }
          count++;
        });
      }else{
        tituloTarea.empty();
        descripcionTarea.empty();
        tituloTarea.append(`
        <a href="#" class="list-group-item list-group-item-action disabled" aria-current="true">
          <div class="d-flex w-100 justify-content-between">
            <h5 class="mb-1">${result.mensaje}</h5>
          </div>
          <p class="mb-1">...</p>
          <small>...</small>
        </a>
        `)
      }
    }
  });
}