window.onload = SearchCarrers();


function SearchCarrers() {
    let tablaCarrera = $("#tbody-Carrer");
    tablaCarrera.empty();
    $.ajax({
        // la URL para la petición
        url: '../../Carrera/SearchCarrers',
        // la información a enviar
        // (también es posible utilizar una cadena de datos)
        data: {},
        // especifica si será una petición POST o GET
        type: 'GET',
        // el tipo de información que se espera de respuesta
        dataType: 'json',
        // código a ejecutar si la petición es satisfactoria;
        // la respuesta es pasada como argumento a la función
        success: function (carrers) {
            tablaCarrera.empty();
            $.each(carrers, function (index, carrer) {
                tablaCarrera.append(`
                <tr>
                    <th scope="row">${carrer.id}</th>
                    <td>${carrer.name}</td>
                    <td>🗑</td>
                    <td>${carrer.duration}</td>
                </tr>
                `);
            })
        }
    });
}

function SaveCarrer() {
   let duracion = $("#Duracion").val();
   let nombre = $("#Nombre").val();
   let id = $("#ID").val()
   console.log(`duracion: ${duracion} y nombre: ${nombre} `)
   $.ajax({
    // la URL para la petición
    url: '../../Carrera/SaveCarrer',
    // la información a enviar
    // (también es posible utilizar una cadena de datos)
    data: { Id: id, Nombre: nombre, Duracion: duracion },
    // especifica si será una petición POST o GET
    type: 'POST',
    // el tipo de información que se espera de respuesta
    dataType: 'json',
    // código a ejecutar si la petición es satisfactoria;
    // la respuesta es pasada como argumento a la función
    success: function (resultado) {
        if (resultado.nonError) {
            $("#staticBackdrop").modal("hide");
            
        }
        else {
            $("#lbl-error").text(resultado.Msj);
        }
    },
    error: function (xhr, status) {
        alert('Error al cargar categorias');
    },

    // código a ejecutar sin importar si la petición falló o no
    complete: function (xhr, status) {
        //alert('Petición realizada');
    }
});
}

$("#Nombre").on("input", function () {
    var input = $(this);
    var posiciónInicial = input[0].selectionStart;  // Guardar la posición del cursor

    input.val(input.val().toUpperCase());  // Convertir texto a mayúsculas

    input[0].setSelectionRange(posiciónInicial, posiciónInicial);  // Restaurar la posición del cursor
});