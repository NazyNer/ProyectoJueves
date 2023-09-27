window.onload = BuscarAsignaturas();
function LimpiarModal() {
    $("#Nombre").val("");
    $("#CarreraID").val("");
    $("#ID").val(0);
    $("#lbl-error").text("");
}
function BuscarAsignaturas() {
  let tablaAsignatura = $("#tbody-Asignatura");
  $.ajax({
    url: '../../Asignatura/BuscarAsignaturas',
    type: 'GET',
    dataType: 'json',
    success: function (Asignaturas) {
      console.log(Asignaturas);
      tablaAsignatura.empty();
      $.each(Asignaturas, function (index, asignatura){
        tablaAsignatura.append(`
        <tr>
            <th scope="row">${asignatura.asignaturaId}</th>
            <td>${asignatura.nombre}</td>
            <td>
                <button disabled title="Boton deshabilitado" onclick="BuscarAsignatura(${asignatura.asignaturaId})">✏</button>
            </td>
            <td>${asignatura.carreraNombre}</td>
        </tr>
        `);
      });
    }
  });
}
function Guardar() {
    let nombre = $("#Nombre").val();
    let carreraId = $("#CarreraID").val();
    let id =$("#ID").val();
    let error = $("#lbl-error");
    if(nombre != null && carreraId > 0){
      $.ajax({
        url: '../../Asignatura/Guardar',
        data: { Id: id, Nombre: nombre, CarreraId: carreraId },
        type: 'POST',
        dataType: 'json',
        success: function (resultado){
          if (resultado.nonError) {
            $("#staticBackdrop").modal("hide");
            BuscarAsignaturas();
          }else{
            error.text(resultado.msj);
          }
        }
      });
    }else{
      error.text("Todos los campos son obligatorios");
    }
}