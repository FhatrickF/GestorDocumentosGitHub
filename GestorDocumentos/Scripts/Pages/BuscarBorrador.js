$(document).ready(function () {
    $('#btn-OR').click(function () {
        p_ = 1;
        setPaginacionOr();
    });
});

function setPaginacionOr() {
    $('#msgError').hide();
    var params = new Object();
    params.Borrador = false;
    params.Pendiente = false;

    if ($("#Borrador").is(":checked")) {
        params.Borrador = true;
    } else {
        params.Borrador = false;
    }

    if ($("#Pendiente").is(":checked")) {
        params.Pendiente = true;
    } else {
        params.Pendiente = false;
    }

    if (!params.Borrador && !params.Pendiente) {
        alert('Debe selecionar una opción');
        return false;
    }

    if (p_ == 1)
        p_ = 0;
    else
        p_ = (p_ - 1) * 10;

    params.pagina = p_;
    $('#paginacion').empty();
    $("#paginacion").html("");
    $.ajax({
        type: "POST",
        url: "/Plantilla/BuscarBorradorOrPendiente",
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: params,
        success: function (d) {
            if (d.numFound == 0) {
                $('#msgError').show();
                $('#tblResultado').hide();
                $('#paginacion').hide();
                $('#lblTotal').hide();
            } else {
                if (d.numFound == "1")
                    $('#lblTotal').html("1 registro encontrado");
                else
                    $('#lblTotal').html(d.numFound + " registros encontrados");

                $('#tablaBody').html("");
                $('#tblResultado').show();
                $('#lblTotal').show();
                for (var i = 0; i < d.docs.length; i++) {
                    var f = "";
                    var r = "<td>";
                    try {
                        f = d.docs[i].Fecha.substring(0, 10);
                        f = f.substring(8, 10) + "-" + f.substring(5, 7) + "-" + f.substring(0, 4);
                        if (f == "01-01-3000")
                            r += "";
                        else
                            r += f + " - ";
                    } catch (e) {
                        r += "";
                    }
                    /* borrador */
                    if (d.docs[i].Estado == "99")
                        r += "<span style=\"color: red\">[Borrador] </span>";
                    if (d.docs[i].Estado == "98")
                        r += "<span style=\"color: blue\">[Pendiente] </span>";

                    if (d.docs[i].Organismo)
                        r += d.docs[i].Organismo + "<br />";
                    if (d.docs[i].Norma)
                        r += "<strong>" + d.docs[i].Norma + "</strong>.- ";
                    if (d.docs[i].Tribunal)
                        r += "<strong>" + d.docs[i].Tribunal + "</strong>.- ";
                    if (d.docs[i].Propiedad)
                        r += d.docs[i].Propiedad;

                    if (d.docs[i].Numero) {
                        r += "Número " + d.docs[i].Numero + ".- ";
                    } else {
                        if (d.docs[i].Articulo) {
                            r += "Artículo número " + d.docs[i].Articulo;
                        }
                        if (d.docs[i].Inciso) {
                            if (d.docs[i].Articulo)
                                r += ", ";
                            r += "Inciso " + d.docs[i].Inciso
                        }
                        r += ".- ";
                    }
                    if (d.docs[i].Titulo)
                        r += d.docs[i].Titulo;
                    if (d.docs[i].Partes)
                        r += d.docs[i].Partes;
                    r += "<br /><strong>Link: </strong>" + d.docs[i].IdDocumento;
                    r += "</td>"

                    r += "<td>";
                    r += "<a class=\"text-blue\" href=\"/Document/Ma_VerDocumento/" + d.docs[i].IdDocumento + "\" >Ver</a>";
                    r += "</td>";
                    if (referencia == "True") {
                        r += "<td></td>";
                    } else {
                        r += "<td>";
                        r += "<a class=\"text-green\" href=\"/Document/Ma_EditarDocumento/" + d.docs[i].IdDocumento + "\" target=\"_blank\">Editar</a>";
                        r += "</td>";
                    }

                    $('#tablaBody').append("<tr>" + r + "</tr>")
                }
                if (d.numFound <= 10) {
                    $('#paginacion').hide();
                } else {
                    $('#paginacion').show();
                    paginacion(d.numFound, p_);
                }

            }
        },
        error: function (xhr, textStatus, errorThrown) {
            alert('Error!!');
        }
    });
};
