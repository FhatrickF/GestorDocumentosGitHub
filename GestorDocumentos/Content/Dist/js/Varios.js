var indice = Array("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ");

function SetSelect(datos, idSelect) {
    //quito los options que pudiera tener previamente el combo
    $(idSelect).html("");
    $(idSelect).append($("<option value=''></option>").text("Seleccione una Opción"))
    //recorro cada item que devuelve el servicio web y lo añado como un opcion
    $.each(datos, function() {
        $(idSelect).append($("<option></option>").attr("value", this.Id).text(this.Nombre))
    });

}

function Top() {
    $('html, body').animate({ scrollTop: $("#main").offset().top }, 200);
}

String.prototype.trim = function() {
    return this.replace(/^\s+|\s+$/g, "");
}
String.prototype.ltrim = function() {
    return this.replace(/^\s+/, "");
}
String.prototype.rtrim = function() {
    return this.replace(/\s+$/, "");
}


function htmlEncode(value) {
    var codifi = $('<div/>').text(value).html();
    codifi = codifi.replace(/&amp;/g, '&');
    return codifi;
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}


//function LimpiaForm(miForm) {
$.fn.limpiaForm = function() {
    // recorremos todos los campos que tiene el formulario
    $(':input', this).each(function() {
        var type = this.type;
        var tag = this.tagName.toLowerCase();
        //limpiamos los valores de los camposâ€¦
        if (type == 'text' || type == 'password' || tag == 'textarea')
            this.value = '';
        // excepto de los checkboxes y radios, le quitamos el checked
        // pero su valor no debe ser cambiado
        else if (type == 'checkbox' || type == 'radio')
            this.checked = false;
        // los selects le ponesmos el indice a -
        else if (tag == 'select')
            this.selectedIndex = 0;
    });
};

$.fn.serializeObject = function() {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function() {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};



(function($) {
    $.fn.toJSON = function(options) {

        options = $.extend({}, options);

        var self = this,
                json = {},
                push_counters = {},
                patterns = {
                    "validate": /^[a-zA-Z][a-zA-Z0-9_]*(?:\[(?:\d*|[a-zA-Z0-9_]+)\])*$/,
                    "key": /[a-zA-Z0-9_]+|(?=\[\])/g,
                    "push": /^$/,
                    "fixed": /^\d+$/,
                    "named": /^[a-zA-Z0-9_]+$/
                };


        this.build = function(base, key, value) {
            base[key] = value;
            return base;

        };

        this.push_counter = function(key) {
            if (push_counters[key] === undefined) {
                push_counters[key] = 0;
            }
            return push_counters[key]++;
        };
        //serializeArraySelectText metodo personalizado que permite traer el texto de los select 
        var obj = $(this).serializeArray().concat($(this).serializeArraySelectText());
        //obj.sort(SortByName);
        $.each(obj, function() {

            // skip invalid keys
            if (!patterns.validate.test(this.name)) {
                return;
            }

            var k,
                    keys = this.name.match(patterns.key),
                    merge = this.value,
                    reverse_key = this.name;


            while ((k = keys.pop()) !== undefined) {

                // adjust reverse_key
                reverse_key_back = reverse_key;
                reverse_key = reverse_key.replace(new RegExp("\\[" + k + "\\]$"), '');

                // push
                if (k.match(patterns.push)) {
                    merge = self.build([], self.push_counter(reverse_key), merge);

                }

                // fixed
                else if (k.match(patterns.fixed)) {
                    merge = self.build([], k, merge);

                }

                // named
                else if (k.match(patterns.named)) {
                    merge = self.build({}, k, merge);
                }
            }

            json = $.extend(true, json, merge);
        });


        return json;
    };
})(jQuery);


$.fn.extend({
    serializeArraySelectText: function() {
        return this.map(function() {
            return this.elements ? jQuery.makeArray(this.elements) : this;
        })
        .filter(function() {
            return this.name && !this.disabled &&
                (this.checked || !this.checked || rselectTextarea.test(this.nodeName) || rinput.test(this.type));
        })
        .map(function(i, elem) {
            var val = jQuery(this).find("option:selected").text();

            if (val == null || this.type != 'select-one')
                return null;
            else {
                if (jQuery.isArray(val)) {
                    jQuery.map(val, function(val, i) {
                        return { name: elem.name, value: val.replace(/\r?\n/g, "\r\n") };
                    });
                }
                else {
                    var nom = elem.name;
                    nom = nombreGlosa(nom);
                    return { name: nom, value: val.replace(/\r?\n/g, "\r\n") };
                    /*if (nom.charAt(nom.length - 1) == "]") {
                        return { name: nom.slice(0, -1) + "Glosa]", value: val.replace(/\r?\n/g, "\r\n") };
                    }
                    else
                        return { name: nom + "Glosa", value: val.replace(/\r?\n/g, "\r\n") };*/
                }
            }
        }).get();

    }
});

function nombreGlosa(str, resto) {
    var largo = str.length;
    resto = typeof resto !== 'undefined' ? resto : "";
    if (str.charAt(largo - 1) == "]") {
        ini = str.lastIndexOf("[") + 1;
        entre = str.substr(ini, largo - ini - 1);
        if (!isNaN(entre)) {
            if (str.charAt(ini - 2) == "]")
                return nombreGlosa(str.substr(0, ini - 1), str.substr(ini - 1, str.length - ini + 1));
            else
                return str.substr(0, ini - 1) + "Glosa" + str.substr(ini - 1, str.length) + resto;
        }
        else
            return str.slice(0, -1) + "Glosa]" + resto;
    }
    else
        return str + "Glosa" + resto;
}

function SortByName(a, b) {
    var aName = a.name.toLowerCase();
    var bName = b.name.toLowerCase();
    return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
}


function FormatCurrency(num) {
    num = FormatCurrency2(num);
    var p = num.split(",");
    if (parseInt(p[1]) > 0)
        return p[0] + "," + p[1];
    else
        return p[0];
}

function FormatCurrency2(num) {
    num = parseFloat(num);
    var p = num.toFixed(2).split(".");
    return "" + p[0].split("").reverse().reduce(function(acc, num, i, orig) {
        return num + (i && !(i % 3) ? "." : "") + acc;
    }, "") + "," + p[1];
}

function FormatMoney(num, cantDecimal, simboloDecimal) {
    var p;
    var strDecimal = "";
    var simboloMiles = "";

    num = num.toString();
    simboloDecimal = typeof simboloDecimal !== 'undefined' ? simboloDecimal : '.';
    cantDecimal = typeof cantDecimal !== 'undefined' ? cantDecimal : 0;

    if (simboloDecimal == '.') simboloMiles = ",";
    else simboloMiles = ".";

    num = num.split(simboloMiles).join('');
    num = num.replace(simboloDecimal, ".");

    if (num.indexOf(".") >= 0) {
        num = parseFloat(num);
        p = num.toFixed(cantDecimal).split(".");
        if (p[1] > 0)
            strDecimal = "," + p[1];

    }
    else
        p = num.split(".");

    return "" + p[0].split("").reverse().reduce(function(acc, num, i, orig) {
        return (isNumber(num) ? num : "") + (i && !(i % 3) ? "." : "") + acc;
    }, "") + strDecimal;

}


function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}


function parseFloat2(num) {
    if ((typeof num != "undefined")) {
        num = num.toString();
        num = num.split('.').join('');
        num = num.replace(',', '.');
    }
    return isNumber(num) ? parseFloat(num) : 0;
}


/* Funcion parche para implementar reduce en IE  */
if (!Array.prototype.reduce) {
    Array.prototype.reduce = function reduce(accumulator) {
        if (this === null || this === undefined) throw new TypeError("Object is null or undefined");
        var i = 0, l = this.length >> 0, curr;

        if (typeof accumulator !== "function") // ES5 : "If IsCallable(callbackfn) is false, throw a TypeError exception."
            throw new TypeError("First argument is not callable");

        if (arguments.length < 2) {
            if (l === 0) throw new TypeError("Array length is 0 and no second argument");
            curr = this[0];
            i = 1; // start accumulating at the second element
        }
        else
            curr = arguments[1];

        while (i < l) {
            if (i in this) curr = accumulator.call(undefined, curr, this[i], i, this);
            ++i;
        }

        return curr;
    };
}
/* Fin reduce */

$.urlParam = function(name) {
    var results = new RegExp('[\\?&]' + name + '=([^&#]*)').exec(window.location.href);
    if (results != null)
        return results[1] || 0;
    else
        return results;
    //return results != null ? results[1] || 0 : results;
}