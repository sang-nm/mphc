;
$(window).trigger("scroll");
$(window).trigger("resize");

function autoCutStr(prefix) {

    if (!prefix || prefix === undefined) {
        prefix = "autoCutStr_";
    }
    $('[class^="' + prefix + '"]').each(function() {
        if ($(this).length > 0) {
            var str = $(this).text();
            str = str.replace('<br/>', '');
            str = str.replace('<br>', '');
            $(this).html(str);
            var len = parseInt($(this).attr("class").substr($(this).attr("class").lastIndexOf("_") + 1));
            var length = str.length;
            if (length > len) {
                if (str.charAt(len) == ' ') {
                    str = str.substr(0, len);
                } else {
                    str = str.substr(0, len);
                    str = str.substr(0, str.lastIndexOf(" "));
                }
                $(this).html(str + "...");
            }
        }

    });
};

;
$(document).ready(function() {
    "use strict";
    autoCutStr();
});
