window.onload = SearchCarrers();
function ClearModal() {
    $("#Nombre").val("");
    $("#Duracion").val("");
    $("#ID").val("");
    $("#lbl-error").text("");
}

function SearchCarrers() {
    let tablaCarrera = $("#tbody-Carrer");
    let listadoCarrera = $("#ListadoCarrerBody")
    listadoCarrera.empty();
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
                    <td><button onclick="DeleteCarrer(${carrer.id})">🗑</button>
                        <button onclick="SearchCarrer(${carrer.id})">✏</button>
                    </td>
                    <td>${carrer.duration}</td>
                </tr>
                `);
                listadoCarrera.append(`
                <tr>
                    <td>${carrer.name}</td>
                    <td>${carrer.duration}</td>
                </tr>
                `)
            })
        }
    });
}

function SearchCarrer(id) {
    $.ajax({
        // la URL para la petición
        url: '../../Carrera/SearchCarrers',
        // la información a enviar
        // (también es posible utilizar una cadena de datos)
        data: { Id: id },
        // especifica si será una petición POST o GET
        type: 'POST',
        // el tipo de información que se espera de respuesta
        dataType: 'json',
        // código a ejecutar si la petición es satisfactoria;
        // la respuesta es pasada como argumento a la función
        success: function (carrers) {
            ClearModal();
            $("#staticBackdrop").modal("show");
            $("#Nombre").val(carrers.name);
            $("#Duracion").val(carrers.duration);
            $("#ID").val(carrers.id);
        }
    });
}

function SaveCarrer() {
    $("#lbl-error").text("");
    let duracion = $("#Duracion").val();
    let nombre = $("#Nombre").val();
    let id = $("#ID").val()
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
                SearchCarrers();
                $("#staticBackdrop").modal("hide");
            } else {
                $("#lbl-error").text(resultado.msj);
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

function DeleteCarrer(id) {
    $.ajax({
        // la URL para la petición
        url: '../../Carrera/DeleteCarrer',
        // la información a enviar
        // (también es posible utilizar una cadena de datos)
        data: { Id: id },
        // especifica si será una petición POST o GET
        type: 'POST',
        // el tipo de información que se espera de respuesta
        dataType: 'json',
        // código a ejecutar si la petición es satisfactoria;
        // la respuesta es pasada como argumento a la función
        success: function (resultado) {
            if (resultado.nonError) {
                SearchCarrers();
            } else {
                alert(resultado.msj);
            }
        } 
        
    });
}

$("#Nombre").on("input", function () {
    var input = $(this);
    var posiciónInicial = input[0].selectionStart;  // Guardar la posición del cursor

    input.val(input.val().toUpperCase());  // Convertir texto a mayúsculas

    input[0].setSelectionRange(posiciónInicial, posiciónInicial);  // Restaurar la posición del cursor
});
