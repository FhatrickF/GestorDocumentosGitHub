var p_ = 1;
var referencia = "false";

$(document).ready(function () {

    $('#FechaH, #FechaD').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy'
    });

    $("#chxt").change(function () {
        if ($(this).is(":checked")) {
            $(".chxATN").prop("checked", true);
            tipoNormacheck();
            $("#chxtAM").prop("checked", true);
            materiascheck();
            $("#chxAF").prop("checked", true);
            fisicoheck();
            falloscheck();
            contraloriacheck();
            ryacheck();
        } else {
            $(".chxATN").prop("checked", false);
            tipoNormacheck();
            $("#chxtAM").prop("checked", false);
            materiascheck();
            $("#chxAF").prop("checked", false);
            fisicoheck();
            falloscheck();
            contraloriacheck();
            ryacheck();
        }
    });
    
    $("#chxATN").change(function () {
        if ($(this).is(":checked")) {
            $(".chxATN-T").prop("checked", true);
        } else {
            $(".chxATN-T").prop("checked", false);
        }
    });

    $("#chxtAM").change(function () {
        if ($(this).is(":checked")) {
            $(".chxtAM").prop("checked", true);
            Bioticocheck();
            SConstruidoheck();
            fisicoheck();
        } else {
            $(".chxtAM").prop("checked", false);
            Bioticocheck();
            SConstruidoheck();
            fisicoheck();
            
        }
    });

    $("#chxAB").change(function () {
        if ($(this).is(":checked")) {
            Bioticocheck();
        } else {
            Bioticocheck();
        }
    });

    $("#chxASC").change(function () {
        if ($(this).is(":checked")) {
            SConstruidoheck();
        } else {
            SConstruidoheck();
        }
    });

    $("#chxAF").change(function () {
        if ($(this).is(":checked")) {
            fisicoheck();
        } else {
            fisicoheck();
        }
    });

    $("#chxAFALLO").change(function () {
        if ($(this).is(":checked")) {
            falloscheck();
        } else {
            falloscheck();
        }
    });

    $("#chxAcontraloria").change(function () {
        if ($(this).is(":checked")) {
            contraloriacheck();
        } else {
            contraloriacheck();
        }
    });

    $("#chxAcontraloria").change(function () {
        if ($(this).is(":checked")) {
            contraloriacheck();
        } else {
            contraloriacheck();
        }
    });

    $("#chxrya").change(function () {
        if ($(this).is(":checked")) {
            ryacheck();
        } else {
            ryacheck();
        }
    });

    $('#btn-NM').click(function () {
        p_ = 1;
        setPaginacionNM();
    });

    function setPaginacionNM() {
        $('#msgError').hide();
        var params = new Object();
        params.nor = $('#BuscarNM').toJSON();
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

    function tipoNormacheck() {
        if ($(".chxATN").is(":checked")) {
            $(".chxATN-T").prop("checked", true);
            Bioticocheck();
            SConstruidoheck();
            fisicoheck();

        } else {
            $(".chxATN-T").prop("checked", false);
            Bioticocheck();
            SConstruidoheck();
            fisicoheck();
        }
    };

    function materiascheck() {
        if ($("#chxtAM").is(":checked")) {
            $(".chxtAM").prop("checked", true);
            $(".chxAB").prop("checked", true);
            $(".chxASC").prop("checked", true);
            $(".chxAF").prop("checked", true);
        } else {
            $(".chxtAM").prop("checked", false);
            $(".chxAB").prop("checked", false);
            $(".chxASC").prop("checked", false);
            $(".chxAF").prop("checked", false);
        }
    };

    function Bioticocheck() {
        if ($("#chxAB").is(":checked")) {
            $(".chxAB").prop("checked", true);
        } else {
            $(".chxAB").prop("checked", false);
        }
    };

    function SConstruidoheck() {
        if ($("#chxASC").is(":checked")) {
            $(".chxASC").prop("checked", true);
        } else {
            $(".chxASC").prop("checked", false);
        }
    };

    function fisicoheck() {
        if ($("#chxAF").is(":checked")) {
            $(".chxAF").prop("checked", true);
        } else {
            $(".chxAF").prop("checked", false);
        }
    };

    function falloscheck() {
        if ($("#chxAFALLO").is(":checked")) {
            $(".chxAFALLO").prop("checked", true);
        } else {
            $(".chxAFALLO").prop("checked", false);
        }
    };

    function contraloriacheck() {
        if ($("#chxAcontraloria").is(":checked")) {
            $(".chxAcontraloria").prop("checked", true);
        } else {
            $(".chxAcontraloria").prop("checked", false);
        }
    }

    function ryacheck() {
        if ($("#chxrya").is(":checked")) {
            $(".chxrya").prop("checked", true);
        } else {
            $(".chxrya").prop("checked", false);
        }
    }
});