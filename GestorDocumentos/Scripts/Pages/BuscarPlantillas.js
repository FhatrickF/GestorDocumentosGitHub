var p_ = 1;
var tipo;
var referencia = "false";
$(document).ready(function () {
    $('#btn-NG').click(function () {
        p_ = 1;
        setPaginacionNG();
    });

    $('#btn-NP').click(function () {
        p_ = 1;
        setPaginacionNP();
    });

    $('#btn-aviso').click(function () {
        p_ = 1;
        setPaginacionAV();
    });

    $('#btn-PJ').click(function () {
        p_ = 1;
        setPaginacionPJ();
    });

    $('#btn-LA').click(function () {
        p_ = 1;
        setPaginacionLA();
    });

    $('#btn-BITE').click(function () {
        p_ = 1;
        setPaginacionBITE();
    });


    

});

function pagina(p) {
    p_ = p;
    switch (tipo) {
        case 'NG': setPaginacionNG();
            break;
        case 'BITE': setPaginacionBITE();
            break;
        case 'NP': setPaginacionNP();
            break;
        case 'PJ': setPaginacionPJ();
            break;
        case 'AV': setPaginacionAV();
            break;
        case 'LA': setPaginacionLA();
            break;
    }
}

function setPaginacionNG() {
    $('#msgError').hide();
    var params = new Object();
    params.nor = $('#NormasG').toJSON();
    if (p_ == 1)
        p_ = 0;
    else
        p_ = (p_ - 1) * 10;

    params.nor.Pagina = p_;
    $('#paginacion').empty();
    $("#paginacion").html("");
    $.ajax({
        type: "POST",
        url: "/Plantilla/BuscarNG",
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
                tipo = 'NG';
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

function setPaginacionBITE() {
    $('#msgError').hide();
    var params = new Object();
    params.nor = $('#BuscarBite').toJSON();
    if (p_ == 1)
        p_ = 0;
    else
        p_ = (p_ - 1) * 10;

    params.nor.Pagina = p_;
    $('#paginacion').empty();
    $("#paginacion").html("");
    $.ajax({
        type: "POST",
        url: "/Plantilla/BuscarBITE",
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
                tipo = 'BITE';
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

function setPaginacionNP() {
    $('#msgError').hide();
    var params = new Object();
    params.nor = $('#BuscarNP').toJSON();
    if (p_ == 1)
        p_ = 0;
    else
        p_ = (p_ - 1) * 10;

    params.nor.pagina = p_;
    $('#paginacion').empty();
    $("#paginacion").html("");
    $.ajax({
        type: "POST",
        url: "/Plantilla/BuscarNP",
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
                tipo = 'NP';
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

function setPaginacionAV() {
    $('#msgError').hide();
    var params = new Object();
    params.nor = $('#BuscarAV').toJSON();
    if (p_ == 1)
        p_ = 0;
    else
        p_ = (p_ - 1) * 10;

    params.nor.Pagina = p_;
    $('#paginacion').empty();
    $("#paginacion").html("");
    $.ajax({
        type: "POST",
        url: "/Plantilla/BuscarA",
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
                tipo = 'AV';
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

function setPaginacionPJ() {
    $('#msgError').hide();
    var params = new Object();
    params.nor = $('#BuscarPJ').toJSON();
    if (p_ == 1)
        p_ = 0;
    else
        p_ = (p_ - 1) * 10;

    params.nor.Pagina = p_;
    $('#paginacion').empty();
    $("#paginacion").html("");
    $.ajax({
        type: "POST",
        url: "/Plantilla/BuscarPJ",
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
                tipo = 'PJ';
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

function setPaginacionLA() {
    $('#msgError').hide();
    var params = new Object();
    params.nor = $('#BuscarLA').toJSON();
    if (p_ == 1)
        p_ = 0;
    else
        p_ = (p_ - 1) * 10;

    params.nor.pagina = p_;
    $('#paginacion').empty();
    $("#paginacion").html("");
    $.ajax({
        type: "POST",
        url: "/Plantilla/BuscarLA",
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
                tipo = 'LA';
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

function paginacion(registros, p) {
    if (p == 0)
        p = 1;
    else
        p = (p / 10) + 1;

    var listado = "";
    var i;
    if (registros > 10) {
        var d = (registros / 10);
        if (d % 1 != 0) {
            d = d + 1;
            var d_ = d.toString().split(".");
            d = parseInt(d_);
        }
    } else {
        d = registros;
    }

    var tp = d;
    var lim_iz = 1 + 4;
    var lim_de = tp - 4;

    var pag_sel_l = "<strong>";
    var pag_sel_r = "</strong>";
    var medio = true;

    if (tp <= 7) {
        for (i = 1; i <= tp; i++) {
            var active = "";
            if (i == p)
                active = "active";
            listado += "<li class=\"page-item " + active + "\"><a class=\"page-link\" onclick=\"pagina(" + i + ")\" style=\"cursor:pointer\" >" + i + "</a></li>";
        };
    } else {
        if (p < lim_iz) {
            for (i = 1; i <= lim_iz; i++) {
                var active = "";
                if (i == p)
                    active = "active";
                listado += "<li class=\"page-item " + active + "\"><a class=\"page-link\" onclick=\"pagina(" + i + ")\" style=\"cursor:pointer\" >" + i + "</a></li>";
            };
            listado += "<li class=\"page-item disabled\"><a class=\"page-link \" href=\"#\">...</a></li>";
            listado += "<li class=\"page-item\"><a class=\"page-link\" onclick=\"pagina(" + tp + ")\" style=\"cursor:pointer\" >" + tp + "</a></li>";
            medio = false;
        }
        if (p > lim_de) {
            for (i = lim_de; i <= tp; i++) {
                var active = "";
                if (i == p)
                    active = "active";
                listado += "<li class=\"page-item " + active + "\"><a class=\"page-link\" onclick=\"pagina(" + i + ")\" style=\"cursor:pointer\" >" + i + "</a></li>";
            };
            listado = "<li class=\"page-item\"><a class=\"page-link\" onclick=\"pagina(1)\" style=\"cursor:pointer\">1</a></li>" +
                "<li class=\"page-item disabled\"><a class=\"page-link \" href=\"#\">...</a></li>" + listado;

            medio = false;
        }
        if (medio) {
            listado += "<li class=\"page-item\"><a class=\"page-link\" onclick=\"pagina(1)\" style=\"cursor:pointer\" >1</a></li>";
            listado += "<li class=\"page-item disabled\"><a class=\"page-link \" href=\"#\">...</a></li>";
            listado += "<li class=\"page-item\"><a class=\"page-link\" onclick=\"pagina(" + (p - 1) + ")\" style=\"cursor:pointer\">" + (p - 1) + "</a></li>";
            listado += "<li class=\"page-item active\"><a class=\"page-link\" onclick=\"pagina(" + p + ")\" style=\"cursor:pointer\" >" + p + "</a></li>";
            listado += "<li class=\"page-item\"><a class=\"page-link\" onclick=\"pagina(" + (p + 1) + ")\" style=\"cursor:pointer\">" + (p + 1) + "</a></li>";
            listado += "<li class=\"page-item disabled\"><a class=\"page-link \" href=\"#\">...</a></li>";
            listado += "<li class=\"page-item\"><a class=\"page-link\" onclick=\"pagina(" + tp + ")\" style=\"cursor:pointer\">" + tp + "</a></li>";
        }
    }
    listado = "<nav><ul class=\"pagination\">" + listado + "</ul></nav>";
    $("#paginacion").html(listado);
}